#include "iostream"
#include "time.h"
#include "PID_v1.h"

using namespace std;

extern "C" {
#include "extApi.h"
	/*#include "extApiCustom.h" if you wanna use custom remote API functions! */
}
#define PI		3.14159265
#define MAX_NODOS 30

struct Nodo
{
	unsigned char ID;
	float X;
	float Y;
};

//Nodo ListaNodos[MAX_NODOS] = { { 0, 667, 444 },
//								{ 21, 630, 432 },
//								{ 20, 565, 375 },
//								{ 15, 501, 328 },
//								{ 16, 539, 288 },
//								{ 12, 530, 145 },
//								{ 11, 462, 166 },
//								{ 10, 416, 139 },
//								{ 9, 325, 145 } };

Nodo ListaNodos[MAX_NODOS] = { { 0, 0.0246, -0.7425},
								{	1, 0.2765, -0.0574 },
								{	2, 1.0602, -0.3971 },
								{	3, 1.3222, -1.3535 },
								{	4, -0.0013, -1.8366 },
								{	5, -1.7063, -1.7625 },
								{	6, -1.9826, 0.1074 },
								{	7, -1.7856, 1.6676 },
								{   8, 1.41150, 1.3875 },
};

#define OP_INICIAR			0
#define OP_ESPERA			1

float DESPLAZAMIENTO_COOR_ROBOT = 2.220;
int NumeroNodos = 9;
float Escala = 300.5;
int Pos = 0;
float Xpos;
float Ypos;

//Posición en la última entrada de control
float Xant;
float Yant;

//Variables para guardar motores
int motor_derecho;
int motor_izquierdo;


void IniciarSimulacion(int clientID);
bool Espera(unsigned long ms, int contador, int op);
float ModuloVector(float X1, float Y1, float X2, float Y2);
float DireccionVector(float X1, float Y1, float X2, float Y2);
unsigned long micros();
simxFloat PosicionRobot(float PosPlano);
simxFloat PosicionPlano(float PosRobot);
int gclientID;

//Define Variables we'll be connecting to
double Setpoint, Input, Output;
//Specify the links and initial tuning parameters
//double Kp = 2, Ki = 5, Kd = 1;
double Kp = 2., Ki = 5, Kd = 1;
PID myPID(&Input, &Output, &Setpoint, Kp, Ki, Kd, DIRECT);

int main(int argc, char** argv)
{
	Setpoint = 0;
	//turn the PID on
	myPID.SetMode(AUTOMATIC);

	ListaNodos[0].ID = 0;

	for (int i = 0 ; i < NumeroNodos; i++)
		cout << PosicionRobot((float)ListaNodos[i].X) << " , " << PosicionRobot((float)ListaNodos[i].Y) << endl;

	int portNb = 19997;   //Se define el puerto de conexión

						  //Conectar con V-REP
	int clientID = simxStart("127.0.0.1", portNb, true, true, 5000, 5);
	gclientID = clientID;

	//Si la conexión es exitosa iniciar la simulación
	if (clientID >-1)
	{
		simxStartSimulation(clientID, simx_opmode_oneshot);

		IniciarSimulacion(clientID);

		simxStopSimulation(clientID, simx_opmode_oneshot_wait);
	}
	simxFinish(clientID);

}

float ModuloVector(float X1, float Y1, float X2, float Y2)
{
	return sqrt(pow(X2 - X1, 2) + pow(Y2 - Y1, 2));
}

float DireccionVector(float X1, float Y1, float X2, float Y2)
{
	//Devuelve el ángulo del vector de 0 a 360º en sentido antihorario
	float X = X2 - X1;
	float Y = Y2 - Y1;
	float angulo = atan2(Y, X) * 180 / PI;
	if (angulo < 0)
		angulo = angulo + 180 * 2;
	return angulo;
}
unsigned long micros()
{
	return clock() * 1000;
}

simxFloat PosicionRobot(float PosPlano)
{
	return PosPlano;
	//return (float)PosPlano / Escala - DESPLAZAMIENTO_COOR_ROBOT;
}
simxFloat PosicionPlano(float PosRobot)
{
	return PosRobot;
	//return (PosRobot + DESPLAZAMIENTO_COOR_ROBOT) * Escala;
}

void AsignarVelocidad(float D, float I)
{
	simxSetJointTargetVelocity(gclientID, motor_derecho, D, simx_opmode_oneshot);
	simxSetJointTargetVelocity(gclientID, motor_izquierdo, I, simx_opmode_oneshot);
}

float Margenes(float valor, float minv, float maxv, float mins, float maxs)
{
	if (valor > maxv) valor = maxv;

	return (valor-minv) / maxv*maxs+mins;
}

void IniciarSimulacion(int clientID)
{

	//Variable que guarda el obstaculo detectado
	simxUChar obstaculo;

	//Variable para guardar el sensor de distancia
	int sensor;

	//Cuerpo robot
	int robot;


	//Aqui guardamos los dos motores y el sensor
	int valido = simxGetObjectHandle(clientID, "motor_derecho", &motor_derecho, simx_opmode_blocking);
	int valido2 = simxGetObjectHandle(clientID, "motor_izquierdo", &motor_izquierdo, simx_opmode_blocking);
	int valido_sensor = simxGetObjectHandle(clientID, "sensor", &sensor, simx_opmode_blocking);
	int valido_robot = simxGetObjectHandle(clientID, "robot", &robot, simx_opmode_blocking);

	//Si no se ha podido acceder a alguno de estos componentes mostramos un mensaje de error y salimos del programa
	if (valido != 0 || valido2 != 0 || valido_sensor != 0 || valido_robot != 0)
	{
		//cout << "ERROR: No se ha podido conectar con el robot" << endl;
		simxFinish(clientID);
		return;
	}

	// el sensor
	simxReadProximitySensor(clientID, sensor, &obstaculo, NULL, NULL, NULL, simx_opmode_streaming);

	simxFloat position[3];
	int retorno_posicion;

	enum Fases:int { arranque_inicial, establecer_vector_direccion, iniciar_ruta, seguir_ruta};
	Fases fase = arranque_inicial;

	long ms_inicio_simulacion = 0;
	
	simxSetJointTargetVelocity(clientID, motor_derecho, 0, simx_opmode_oneshot);
	simxSetJointTargetVelocity(clientID, motor_izquierdo,0, simx_opmode_oneshot);

#define TIEMPO_RECORRIDO_INICIAL	1000 //Arramca durante 2s para establecer la dirección
#define MS_CONTROL					200
#define MAX_MEDIDAS_DIR				10
#define VELOCIDAD_ANGULAR			0.5
#define VELOCIDAD_ANGULAR_MIN		0.0001
#define DISTANCIA_SALTO_NODO		0.100 // 50cm

	float Xini, Yini;
	float direccion[MAX_MEDIDAS_DIR];
	float VectorDireccionRobot;
	float VectorDireccionTrayectoria;
	float DifVector;
	float DifReal;
	int IndSigNodo = 1;
	long ticks = 0; //Número de iteraciones de control
	Nodo SigNodo;
	while (simxGetConnectionId(clientID) != -1) //Este bucle funcionara hasta que se pare la simulacion
	{
		long ms = clock();
		//simxReadProximitySensor(clientID, sensor, &obstaculo, NULL, NULL, NULL, simx_opmode_buffer); //Leer el sensor
		//if (obstaculo == 0) //Si no se detecta ningun obstaculo, avanzar
		//{
		//	simxSetJointTargetVelocity(clientID, motor_derecho, -2, simx_opmode_oneshot);
		//	simxSetJointTargetVelocity(clientID, motor_izquierdo, -2, simx_opmode_oneshot);
		//}
		//else //Giraremos si encontramos algo en el camino del robot
		//{
		//	simxSetJointTargetVelocity(clientID, motor_derecho, 2, simx_opmode_oneshot);
		//	simxSetJointTargetVelocity(clientID, motor_izquierdo, -2, simx_opmode_oneshot);
		//}

		retorno_posicion = simxGetObjectPosition(clientID, robot, -1, position, simx_opmode_oneshot);

		if (retorno_posicion == 0)
		{
			ms_inicio_simulacion = ms;

			Xpos = position[0];
			Ypos = position[1];
			long cl = clock();

			//Realizamos el control de movimiento cada MS_CONTROL
			if (!(ms % MS_CONTROL))
			{
				ticks++;
				//cout << Xpos << ", " << Ypos << "," << position[0] << "," << position[1] << "," << position[2] << "," << cl << endl;

				if (fase == arranque_inicial)
				{ //En el arranque debemos avanzar X segundos para poder establecer el vector de dirección
					Xini = position[0];
					Yini = position[1];
					AsignarVelocidad(-VELOCIDAD_ANGULAR, -VELOCIDAD_ANGULAR);
					fase = establecer_vector_direccion;
					Espera(TIEMPO_RECORRIDO_INICIAL, 0, OP_INICIAR);
				}
				else if (fase == establecer_vector_direccion)
				{

					if (!Espera(TIEMPO_RECORRIDO_INICIAL, 0, OP_ESPERA))
					{
						fase = iniciar_ruta;
						SigNodo = ListaNodos[IndSigNodo];
						AsignarVelocidad(0, 0);
					}
				} //Tan(a) = y/x  v=<x1-x2, y1-y2>
				else if (fase == iniciar_ruta)
				{
					float DistanciaNodo;
					//Calculamos el vector de desplazamiento del robot
					VectorDireccionRobot = DireccionVector(Xini, Yini, Xpos, Ypos);

					//cout << Xini << " , " << Yini << " , " << Xpos << " , " << Ypos << "Vector dir robot: " << VectorDireccionRobot << endl;
					//cout << "Vector dir robot: " << VectorDireccionRobot << endl;

					//Calculamos la dirección del vector que une la posición actual del robot con el siguiente punto de la trayectoria
					VectorDireccionTrayectoria = DireccionVector(Xpos, Ypos, PosicionRobot(SigNodo.X), PosicionRobot(SigNodo.Y));
					//cout << Xpos << " , " << Ypos << " , " << PosicionRobot(SigNodo.X) << " , " << PosicionRobot(SigNodo.Y) << endl;
					//cout << "Vector dir path: " << VectorDireccionTrayectoria << endl;

					//Comprobamos la distancia a la que nos encontramos del nodo. Si es menor a la mínima saltamos de nodo de referencia
					DistanciaNodo = ModuloVector(Xpos, Ypos, PosicionRobot(SigNodo.X), PosicionRobot(SigNodo.Y));
					if (DistanciaNodo < DISTANCIA_SALTO_NODO)
					{
						IndSigNodo++;
						SigNodo = ListaNodos[IndSigNodo];
					}
					//Calculamos la diferencia de dirección entre los vectores
					Setpoint = 0;
					if (VectorDireccionRobot)
					{
						DifVector = VectorDireccionTrayectoria - VectorDireccionRobot;
						if (abs(DifVector) > 180)
							DifReal = 360 - abs(DifVector);
						else
							DifReal = abs(DifVector);

						Input = DifReal;
						myPID.Compute();

#define MAX_VALOR_OUT_PID	100
#define FACTOR_SALIDA	50
#define MAX_VEL_ANG		1.5	//Máxima diferencia de velocidad con corrección de trayectoria
#define MAX_VEL_FIJA	2	//Velocidad lineal sin corrección de trayectoria
#define MIN_VEL_FIJA	0.8 //Velocidad lineal con corrección de trayectoria
#define MIN_GRADOS_RECTA  8

						//La respuesta del PID debe ser proporcional a la diferencia del ángulo
						//Cuanto mas pequeño es el ángulo menor peso tiene la parte variable


						//float Salida = Output / FACTOR_SALIDA;
						float Salida = Margenes(Output, 0, MAX_VALOR_OUT_PID, 0, MAX_VEL_ANG);

						Salida = Output / (FACTOR_SALIDA);
						float invSalida = MAX_VEL_ANG-Salida;

						//float VelocidadFija = VEL_FIJA - Margenes(DifReal, 0, 30, 0, VEL_FIJA);

						cout << IndSigNodo << " - PID input: " << DifVector << " dif: " << DifReal << "o: " << Salida << endl;

						float VelocidadFija = MAX_VEL_FIJA;

						if (DifReal < MIN_GRADOS_RECTA)
							VelocidadFija = MAX_VEL_FIJA;
						else
							VelocidadFija = MIN_VEL_FIJA;

						//Si la diferencia del vector de dirección es positiva giramos a la izquierda, sino a la derecha
						//Si la diferencia es superior a 180 grados realizamos el camino complementario por ser más corto

						if (DifReal < MIN_GRADOS_RECTA)
							AsignarVelocidad(-VelocidadFija, -(VelocidadFija));
						else if ((DifVector > 0) && (DifVector < 180))
							AsignarVelocidad(-VelocidadFija, -(VelocidadFija + invSalida));
						else
							AsignarVelocidad(-(VelocidadFija + invSalida), -VelocidadFija);
						

						//if ((DifVector > 0) && (DifVector < 180))
						//	AsignarVelocidad(-(VEL_FIJA + Salida), -(VEL_FIJA + invSalida));
						//else
						//	AsignarVelocidad(-(VEL_FIJA + invSalida), -(VEL_FIJA + Salida));

						//if ((DifVector > 0) && (DifVector < 180))
						//	AsignarVelocidad(-Salida, -(MAX_VEL_ANG - Salida));
						//else
						//	AsignarVelocidad(-(MAX_VEL_ANG - Salida), -Salida);

						//if ((DifVector > 0) && (DifVector < 180))
						//	AsignarVelocidad(-VELOCIDAD_ANGULAR_MIN, -(MAX_VEL_ANG-Salida));
						//else
						//	AsignarVelocidad(-(MAX_VEL_ANG - Salida), -VELOCIDAD_ANGULAR_MIN);

						Xini = Xpos;
						Yini = Ypos;
					}
				}

				//Recordamos la posición del último punto de control
				Xant = Xpos;
				Yant = Ypos;
			}
		}
	}

	simxFinish(clientID); //Siempre hay que parar la simulación
	//cout << "Simulacion finalizada" << endl;
	return;
}

bool Espera(unsigned long ms, int contador, int op)
{
#define MAX_US				4294967295

	static unsigned long us_ini[10] = { 0,0,0,0,0,0,0,0,0,0 };
	unsigned long us_actual[10];

	us_actual[contador] = micros();

	if (op == OP_INICIAR)
		us_ini[contador] = micros();
	else
	{
		if (us_actual[contador] < us_ini[contador])
		{
			us_actual[contador] = MAX_US - us_ini[contador] + us_actual[contador];
			us_ini[contador] = 1;
		}

		if (us_actual[contador] - us_ini[contador] >= ms * 1000)
		{
			us_ini[contador] = 0;
			return false;
		}
		else
			return true;
	}
}

