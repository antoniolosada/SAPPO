/*  ----------------------------------------------------------------
http://www.prometec.net/nrf2401
Prog_79_1_Emisor

Programa para transmitir strings mediante radios NRF2401
1. Configurar los pines del RF24 para arduino uno o mega


//Motor D punto central adelante < 70-80 < atras
//Motor I punto central atras < 77 < adelante

//Avanzar muy despacio hacia adelante
//mi 85
//md 55
--------------------------------------------------------------------
*/

#include "list.h"
#include "node_cpp.h"
#include <nRF24L01.h>
#include <RF24.h>
#include <RF24_config.h>
#include <SPI.h>
#include "PID_v2.h"
#include <VirtualWire.h>
#include <Kalman.h>
#include "SoftwareServo.h"
//#include "ServoTimer2.h"



//Inclusiones de ficheros fuente alternativos. Si no se incluyen falla el proceso de build
#include "node_cpp.cpp"
#include "list.cpp"

#define MOTOR_D_PARADO	80
#define MOTOR_I_PARADO	77

#define MS_FRENADA 40
#define MS_ESPERA  200
#define PITIDO_ESTADOS  0

#define DERECHA        1
#define IZQUIERDA      0

#define ADELANTE       0
#define ATRAS          1

#define VELOCIDAD_RAPIDA         200
#define VELOCIDAD_MENOS_RAPIDA   170
#define PARADO                    0
#define VELOCIDAD_LENTA          150

#define MODE_HIBERNATE		1
#define MODE_SUSPEND		2
#define MODE_ALERT			3
#define MODE_ACTIVE			4

#define OP_READ_INIT		1
#define OP_READ_DISTANCE	2
#define OP_READ_FIN			3
#define OP_SET_MODE			4

#define OP_INICIAR			1
#define OP_CONSULTAR		2

//Identificaci�n de PINES **************************************
#define SERVO_DER  33
#define SERVO_IZQ  32

#define LINEA_der      37
#define LINEA_centro   35
#define LINEA_izq      36

#define RUEDA_der     3
#define RUEDA_izq     2

#define PIN_W  5
#define PIN_R  4
//FIN Identificaci�n de PINES **************************************

#define TONE  8
#define MAX_SENSORES_ULTRASONIDOS 8
#define TRIGER1      38
#define TRIGER2      39
#define TRIGER3      40
#define TRIGER4      41

#define TRIGER5      22
#define TRIGER6      23
#define TRIGER7      24
#define TRIGER8      25
//---------------------------------
#define ECHO1        42
#define ECHO2        43
#define ECHO3        44
#define ECHO4        45

#define ECHO5        46
#define ECHO6        47
#define ECHO7        48
#define ECHO8        49

#define ID_OPERACION_MEDIDA			1
#define ID_ROBOT_RODI				1
#define NUM_MEDIDAS_BALIZAS			20	//N�mero m�ximo de medidas que almacenamos
#define NUM_MEDIDAS_VALIDAS			10  //N�mero m�nimo de medidas para realizar el control de medias
#define NUM_MEDIDAS_MAL_RESET		10  //N�mero de medidas descartadas para resetear la lista de medidas
#define NUM_MEDIDAS_OK_RESET		10  //N�mero de medidas buenas para borrar el acumulado de errores
#define MIN_CONTROL_TIEMPO_VUELO	10  //M�rgenes de control de tiempo de vuelo de las balizas
#define MAX_CONTROL_TIEMPO_VUELO	1000
#define MARGEN_MEDIDA_DELTA			4	//Margen para que la nueva medida se considere v�lida
#define ROBOT_ALTURA_SUELO_SENSORES	15
#define MAX_TIEMPO_INACTIVIDAD		1000 //ms. Si durante este tiempo no se localiza una valiza, se entiende que comenzamos a almacenar las medidas
#define ERRORES_CAMBIO_BALIZA		4 //N�mero de errores para buscar nuevas balizas para la medida		

#define MAX_BALIZAS				10 //N�mero m�ximo de balizas que pueden leerse de modo simult�neo

#define MAX_TIEMPO_VUELO_SONIDO 30 //Tiempo en ms para que el sonido recorra 8m

#define NUM_BALIZAS				2


byte aTrigger[MAX_SENSORES_ULTRASONIDOS] = { TRIGER1, TRIGER2, TRIGER3, TRIGER4, TRIGER5, TRIGER6, TRIGER7, TRIGER8 };
byte aEcho[MAX_SENSORES_ULTRASONIDOS] = { ECHO1, ECHO2, ECHO3, ECHO4, ECHO5, ECHO6, ECHO7, ECHO8 };

#define MAX_MEDIDAS 4
String comando;

char msg[16] = ".";                             // Array a transmitir


												//*************************************************************************************************************
												//                        CONFIGURACI�N DE BALIZAS
												//*************************************************************************************************************

struct sBaliza
{
	long codigo;
	int  id_baliza;
	int  id_habitacion;
	int  X;
	int  Y;
	int  Z; //Altura de los sensores desde el suelo
	int  grados_cobertura;
	int  numero_sensores;
	int  direccion;
public:
	void print();
};

void sBaliza::print()
{
	Serial.print(F("ID Baliza ")); Serial.println(id_baliza);
	Serial.print(F("ID Habitacion ")); Serial.println(id_habitacion);
	Serial.print("X "); Serial.println(X);
	Serial.print("Y "); Serial.println(Y);
	Serial.print("Z "); Serial.println(Z);
	Serial.print(F("Gradros de cobertura ")); Serial.println(grados_cobertura);
	Serial.print(F("N�mero sensores ")); Serial.println(numero_sensores);
	Serial.print(F("Direcci�n ")); Serial.println(direccion);
}

struct sMedida
{
	byte codigo;
	float tiempo_local;
	float tiempo_vuelo;
	float tiempo_vuelo_ajustado; //Ajustado con el control de altura entre baliza y movil
	int num_sensor;
};

struct sMedidas
{
	byte codigo;
	byte id_baliza;
	List<sMedida> Medida;
	int numero_medidas;
	long MedidasCorrectas;
	int FallosTotales;
	int FallosAcumulados;
	long tiempo_vida;
	long tiempo_ultima_medida;
	long numero_ultima_medida;
public:
	void Inicializar()
	{
		numero_medidas = 1;
		MedidasCorrectas = 1;
		FallosAcumulados = 0;
		FallosTotales = 0;
	}
};

struct sProcesoMedida
{
	byte BalizasSeleccionadas[2];
	byte ErroresBalizasSeleccionadas[2];
	byte num_balizas;
	byte errores_acumulados;
	long numero_total_medidas;

	sProcesoMedida()
	{
		for (int i = 0; i < 2; i++)
			BalizasSeleccionadas[2] = ErroresBalizasSeleccionadas[2] = 0;
		num_balizas = 0;
		numero_total_medidas = 0;
		errores_acumulados = 0;
	}
};
List<sBaliza> ListaBalizas;
List<sMedidas> ListaMedidasBalizas;
sProcesoMedida ProcesoMedida;

//*************************************************************************************************************
RF24 radio(9, 53);                        // Creamos un objeto radio del tipo RF2$ para Arduino Mega
										  //RF24 radio(8,7);                        // Creamos un objeto radio del tipo RF2$
										  //*************************************************************************************************************

const uint64_t pipe = 0x00000000E1LL;    // Usamos este canal
const uint64_t broadcast = 0xE8E8F0F0E1LL;    // Usamos este canal
char direccion[3][6] = { "0Nodo","1Nodo","2Nodo" };

struct sPaquete
{
	byte id_baliza;
	long tiempo;
	byte num_sensor;
	long tiempo_local;
};

struct sPaqueteRF433
{
	byte id_robot;
	byte operacion;
};

sPaquete paquete[MAX_BALIZAS];
sPaqueteRF433 paqueteRF433;

void EnviarPulsoUltrasonidos();
void ActivarTimer2();

boolean PararMotores = true;

SoftwareServo MotorD, MotorI;

void setup(void)
{
	pinMode(13, OUTPUT);

	MotorI.attach(SERVO_IZQ);
	MotorD.attach(SERVO_DER);
	MotorI.write(MOTOR_I_PARADO);
	MotorD.write(MOTOR_D_PARADO);

	for (int i = 0; i<MAX_SENSORES_ULTRASONIDOS; i++)
	{
		pinMode(aTrigger[i], OUTPUT);
		pinMode(aEcho[i], INPUT);
	}

	//Motores
	pinMode(LINEA_der, INPUT);
	pinMode(LINEA_izq, INPUT);
	pinMode(LINEA_centro, INPUT);

	pinMode(RUEDA_der, INPUT);
	pinMode(RUEDA_izq, INPUT);

	//Ultrasonidos
	pinMode(PIN_W, OUTPUT); /*salida: para el pulso ultras�nico*/
	pinMode(PIN_R, INPUT); /*entrada: tiempo del rebote del ultrasonido*/

	Serial.begin(57600);

	//Configurar puerto para el m�dulo bluetooth
	Serial1.begin(38400);
	pinMode(21, OUTPUT);
	digitalWrite(21, HIGH);

	radio.begin();
	radio.setPALevel(RF24_PA_HIGH);
	radio.openReadingPipe(0, broadcast);

	radio.setChannel(108);
	radio.setRetries(5, 5);
	radio.setAutoAck(true);

	// Initialise the IO and ISR
	vw_set_tx_pin(12);
	vw_setup(4000);      // Bits per sec
						 //vw_rx_start();       // Start the receiver PLL running

	CargarDatosBalizas();

	//Muestra la memoria de datos disponible
	Serial.print("M.Free: ");
	Serial.println(freeRam());
	delay(2);

	//attachInterrupt(digitalPinToInterrupt(2), intRueda_der, RISING);
	//attachInterrupt(digitalPinToInterrupt(3), intRueda_izq, RISING);

	//ArrancarMotor();

	ActivarTimer2();

	MotorD.write(MOTOR_D_PARADO);
	MotorI.write(MOTOR_I_PARADO);
}

int i = 0;

Kalman myFilterX(0.125, 32, 1023, 0); //suggested initial values for high noise filtering
Kalman myFilterY(0.125, 32, 1023, 0); //suggested initial values for high noise filtering


Kalman klmRev[2] = { Kalman(5,5,10,0) , Kalman(5,5,10,0) };

/************************************************************************************************************************************************************************************
En este bucle debemos recuperar las medidas de las balizas.
En cada iteraci�n el sistema envia un paquete de readio seguido de un tren de pulsos utlras�nicos por el collar de ultraosnidos
ominidereccional.
Las balizas recuperan la informaci�n del paquete de radio e inician un contador hasta la llegada del tren de pulsos de ultrasonidos
Una vez recuperado
*************************************************************************************************************************************************************************************/

void loop(void)
{
	//Recuperar informaci�n de dos balizas de modo consecutivo
	RecuperarMedidasDosBalizas(1);
	delay(5);
	RecuperarComandos();
}


#define TIMER2_OVF	200
int contador = TIMER2_OVF;

ISR(TIMER2_COMPA_vect) {//timer1 interrupt 8kHz toggles pin 9
	if (!PararMotores)
		if (--contador == 0)
		{
			contador = TIMER2_OVF;
			SoftwareServo::refresh();
		}
}

void ActivarTimer2() 
{
	cli();
	//set timer2 interrupt at 8kHz
	TCCR2A = 0;// set entire TCCR2A register to 0
	TCCR2B = 0;// same for TCCR2B
	TCNT2 = 0;//initialize counter value to 0
			  // set compare match register for 8khz increments
	OCR2A = 249;// = (16*10^6) / (8000*8) - 1 (must be <256)
				// turn on CTC mode
	TCCR2A |= (1 << WGM21);
	// Set CS21 bit for 8 prescaler
	TCCR2B |= (1 << CS21);
	// enable timer compare interrupt
	TIMSK2 |= (1 << OCIE2A);
	sei();
}

void LOG()
{
}

void moverServo(int pin, int angulo)
{
	float pausa;
	pausa = angulo*2000.0 / 180.0 + 500;
	digitalWrite(pin, HIGH);
	delayMicroseconds(pausa);
	digitalWrite(pin, LOW);
	delayMicroseconds(2500 - pausa);
}
void CargarDatosBalizas()
{
	sBaliza Baliza;
	Baliza.codigo = 1;
	Baliza.id_baliza = 1;
	Baliza.id_habitacion = 1;
	Baliza.grados_cobertura = 90;
	Baliza.direccion = 1;
	Baliza.numero_sensores = 4;
	Baliza.X = 10;
	Baliza.Y = 10;
	Baliza.Z = 10; //Altura de los sensores desde el suelo
	ListaBalizas.add_head(Baliza);

	Baliza.codigo = 2;
	Baliza.id_baliza = 2;
	Baliza.id_habitacion = 1;
	Baliza.grados_cobertura = 90;
	Baliza.direccion = 1;
	Baliza.numero_sensores = 4;
	Baliza.X = 210;
	Baliza.Y = 10;
	Baliza.Z = 10;
	ListaBalizas.add_head(Baliza);
}

boolean LeerMedidasSensores()
{
	for (int i = 0; i < MAX_SENSORES_ULTRASONIDOS; i++)
		paquete[i].id_baliza = 0;

	EnviarMensaje433Mhz(0);
	EnviarPulsoUltrasonidos();

	radio.startListening();

	unsigned long started_waiting_at = millis();
	bool timeout = false;

	float medida;
	int numero_medidas = 0;
	//Recuperamos todas las medidas posibles de las balizas
	while (!timeout)       // Esperamos hasta el m�ximo tiempo de vuelo de una se�al de US
	{
		while ((!radio.available()) && !timeout)       // Esperamos
		{
			if (millis() - started_waiting_at > MAX_TIEMPO_VUELO_SONIDO)
				timeout = true;
		}

		if (!timeout)
		{   // Leemos el mensaje recibido
			unsigned long got_time;
			radio.read(&paquete[numero_medidas], sizeof(sPaquete));
			medida = 1.0 * paquete[numero_medidas].tiempo / 29.2;   //convertimos a distancia, en cm
			paquete[numero_medidas].tiempo = medida;
			paquete[numero_medidas].tiempo_local = micros();
			numero_medidas++;
		}
	}


	radio.stopListening();

	return numero_medidas;
}

bool RecuperarMedidasDosBalizas(int EnvioBT)
{
	float minimo, medida[2], media, maximo, filtroX, filtroY;
	int medidas, id_baliza_x = 0, id_baliza_y = 0;
	boolean timeout;

	minimo = 999;
	maximo = 0;
	media = 0;
	medidas = 0;

	//Recuperamos dos medidas de sensores contrapuestos v�lidos para triangulaci�n
	timeout = RecuperarMedidaMultiple();

	//Si timeout = true implica que no hemos podido recuperar dos medidas v�lidas para triangular
	if (!timeout)
	{
		//Primero comprobamos que los identificadores de las balizas son correctas
		if (paquete[0].id_baliza != paquete[1].id_baliza != 0)
		{
			//Ahora comprobamos 
			medida[paquete[0].id_baliza - 1] = paquete[0].tiempo;
			medida[paquete[1].id_baliza - 1] = paquete[1].tiempo;

			//Verificamos si las dos medidas son correctas
			if (((medida[0] > 10) && (medida[0] < 1000)) && ((medida[1] > 10) && (medida[1] < 1000)))
			{
				filtroX = myFilterX.getFilteredValue(medida[0]);
				filtroY = myFilterY.getFilteredValue(medida[1]);

				if (EnvioBT == 1)
				{
					Serial1.print(filtroX);
					Serial1.print(",");
					Serial1.print(filtroY);
					Serial1.println("");
				}
				else if (EnvioBT == 3)
				{
					Serial.print(paquete[0].id_baliza);
					Serial.print(" - ");
					Serial.print(paquete[1].id_baliza);
					Serial.print(" - ");
					Serial.print(paquete[0].tiempo);
					Serial.print(" - ");
					Serial.print(paquete[1].tiempo);
					Serial.print(" - ");
					Serial.print(filtroX);
					Serial.print(" - ");
					Serial.println(filtroY);
				}

				return true;
			}
		}

	}
	return false;
}

float RecuperarMedida()
{
	sPaquete paquete;
	//radio.write(&paquete, sizeof(paquete));
	EnviarMensaje433Mhz(0);
	EnviarPulsoUltrasonidos();

	radio.startListening();

	unsigned long started_waiting_at = millis();
	bool timeout = false;

	float medida;
	while ((!radio.available()) && !timeout)       // Esperamos
	{
		if (millis() - started_waiting_at > 80)
		{
			timeout = true;
			//Serial.println("err");
		}
	}

	if (!timeout)
	{   // Leemos el mensaje recibido
		unsigned long got_time;
		radio.read(&paquete, sizeof(sPaquete));
		medida = 1.0 * paquete.tiempo / 29.2;   //convertimos a distancia, en cm
	}
	radio.stopListening();

	return medida;
}

// Recupera medida de todas las balizas

boolean RecuperarMedidaMultiple()
{
	paquete[0].id_baliza = 0;
	paquete[1].id_baliza = 0;
	//radio.write(&paquete, sizeof(paquete));
	EnviarMensaje433Mhz(0);
	EnviarPulsoUltrasonidos();

	radio.startListening();

	unsigned long started_waiting_at = millis();
	bool timeout = false;

	float medida;
	int numero_medidas = 0;
	while (!timeout && (numero_medidas < 2))       // Esperamos
	{
		while ((!radio.available()) && !timeout)       // Esperamos
		{
			if (millis() - started_waiting_at > 80)
			{
				timeout = true;
				//Serial.println("err");
			}
		}

		if (!timeout)
		{   // Leemos el mensaje recibido
			unsigned long got_time;
			radio.read(&paquete[numero_medidas], sizeof(sPaquete));
			medida = 1.0 * paquete[numero_medidas].tiempo / 29.2;   //convertimos a distancia, en cm
			paquete[numero_medidas].tiempo = medida;
			numero_medidas++;
		}
	}


	radio.stopListening();

	return timeout;
}

void EnviarPulsoUltrasonidos()
{
	long tiempo;
	int distanceCm;
	for (int i = 0; i<MAX_SENSORES_ULTRASONIDOS; i++)
		digitalWrite(aTrigger[i], LOW); /* Por cuesti�n de estabilizaci�n del sensor*/
	delayMicroseconds(5);
	for (int i = 0; i<MAX_SENSORES_ULTRASONIDOS; i++)
		digitalWrite(aTrigger[i], HIGH); /* env�o del pulso ultras�nico*/
	delayMicroseconds(15);
	for (int i = 0; i<MAX_SENSORES_ULTRASONIDOS; i++)
		digitalWrite(aTrigger[i], LOW); /* env�o del pulso ultras�nico*/

										/*
										tiempo = pulseIn(ECHO1, HIGH)+1;

										distanceCm = tiempo * 10 / 292/ 2;   //convertimos a distancia, en cm

										Serial.println(distanceCm);
										*/
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

void EnviarMensaje433Mhz(int lectura)
{
	paqueteRF433.id_robot = ID_ROBOT_RODI;
	paqueteRF433.id_robot = ID_OPERACION_MEDIDA;

	//digitalWrite(13, true); // Flash a light to show transmitting

	vw_send((uint8_t *)&paqueteRF433, strlen(msg));
	vw_wait_tx(); // Wait until the whole message is gone
}


void InicializarDatosBalizas(struct sBaliza *Balizas)
{
	Balizas[0].id_baliza = 0;
	Balizas[0].id_habitacion = 0;

	return;
}



int InterseccionCircunferencias(int iD1, int iD2, struct sBaliza BalizaAnt, struct sBaliza BalizaSig, long &lPosX, long &lPosY)
{
	float fA1, fB1, fC1, fA2, fB2, fC2;
	double D, E, F, a, b, c, d, x1, x2, y1, y2;
	double a1, b1, r1, a2, b2, r2;
	long v1x1, v1y1, v1x2, v1y2;


	//Calculamos la distancia entre los dos puntos
	v1x1 = BalizaAnt.X;
	v1y1 = BalizaAnt.Y;

	v1x2 = BalizaSig.X;
	v1y2 = BalizaSig.Y;

	a1 = v1x1;
	b1 = v1y1;
	a2 = v1x2;
	b2 = v1y2;
	r1 = iD1;
	r2 = iD2;

	fA1 = -2 * a1;
	fB1 = -2 * b1;
	fC1 = sq(a1) + sq(b1) - sq(r1);
	fA2 = -2 * a2;
	fB2 = -2 * b2;
	fC2 = sq(a2) + sq(b2) - sq(r2);

	D = fA1 - fA2;
	E = fB1 - fB2;
	F = fC1 - fC2;

	a = sq(E) / sq(D) + 1;
	b = (2 * E*F - D*fA1*E + sq(D)*fB1) / sq(D);
	c = sq(F) / sq(D) - fA1*F / D + fC1;

	d = (sq(b) - 4 * a*c);

	if (d >= 0)
	{
		y1 = (-b + sqrt(d)) / (2 * a);
		x1 = (-E*y1 - F) / D;
		y2 = (-b - sqrt(d)) / (2 * a);
		x2 = (-E*y2 - F) / D;

		if (y1 == y2)
		{
			lPosX = x1;
			lPosY = y1;
		}
		else //Debemos escoger la soluci�n correcta
		{
			lPosX = x1;
			lPosY = y1;
		}

		return 1;
	}
	else
	{
		//No se actualizan los valores
		return 0;
	}

}

int freeRam()
{
	extern int __heap_start, *__brkval;
	int v;
	return (int)&v - (__brkval == 0 ? (int)&__heap_start : (int)__brkval);
}

void PruebaBluetooth()
{
	// Si hay datos disponibles en el monitor serie
	while (Serial.available())
	{
		// Escribimos los valores en el m�dulo bluetooth HC-06
		Serial1.write(Serial.read());
	}
	// Si hay datos disponibles en el m�dulo bluetooth HC-06
	while (Serial1.available())
	{
		// Mostramos los valores en el monitor serie
		Serial.write(Serial1.read());
	}
}

char Comando[10] = "";
HardwareSerial Puerto = Serial1;

void EjecutarComando()
{
	//Arranque motor derecho
	Serial.println(comando);
	if (comando.length() != 5) return;
	if (comando.substring(0, 2) == "md")
	{
		PararMotores = false;
		int Velocidad = comando.substring(2).toInt();
		MotorD.write(Velocidad);
	}
	else if (comando.substring(0, 2) == "mi")
	{
		PararMotores = false;
		int Velocidad = comando.substring(2).toInt();
		MotorI.write(Velocidad);
	}
	else if (comando == "stop-")
	{
		MotorD.write(MOTOR_D_PARADO);
		MotorI.write(MOTOR_I_PARADO);
		delay(100);
		PararMotores = true;
		contador = TIMER2_OVF;
	}
	else if (comando == "pause")
	{
		PararMotores = true;
	}
	else if (comando == "resum")
	{
		contador = TIMER2_OVF;
		PararMotores = false;
	}
	else if (comando.substring(0, 2) == "kl")
	{
	}
}

void RecuperarComandos()
{
	// Si hay datos disponibles en el m�dulo bluetooth HC-06
	while (Serial1.available())
	{
		char c[2] = { 0,0 };
		c[0] = Serial1.read();
		if (c[0] != '.')
		{
			comando.concat(c);
		}
		else
		{
			EjecutarComando();
			comando = "";
		}
	}
}

void MoverRueda(byte rueda, byte direccion, int velocidad)
{
	if (velocidad > 255)velocidad = 255;

	if (rueda == DERECHA)
	{
		if (direccion == ADELANTE)
		{
		}
		else
		{
		}
	}
	else
	{
		if (direccion == ADELANTE)
		{
		}
		else
		{
		}
	}
}

void Tono(int PWM, int ms)
{
	analogWrite(TONE, PWM);
	delay(ms);
	analogWrite(TONE, 0);
}

void CambioMovimiento(byte Estado)
{
	MoverRueda(IZQUIERDA, ATRAS, VELOCIDAD_RAPIDA);
	MoverRueda(DERECHA, ATRAS, VELOCIDAD_RAPIDA);
	delay(MS_FRENADA);
	MoverRueda(IZQUIERDA, ADELANTE, PARADO);
	MoverRueda(DERECHA, ADELANTE, PARADO);

	if (PITIDO_ESTADOS)
	{
		for (int i = 0; i<Estado; i++)
		{
			Tono(200, 50);
			delay(200);
			Tono(200, 0);
			delay(200);
		}
		Tono(200, 0);
	}
	else
	{
		Tono(200, 50);
	}
	delay(MS_ESPERA);
}

void RecuperarMedidasMultiples()
{
	float minimo, medida[2], media, maximo, filtroX, filtroY;
	int medidas, id_baliza_x = 0, id_baliza_y = 0;
	boolean timeout;
	sMedidas MedidaBaliza;

	minimo = 999;
	maximo = 0;
	media = 0;
	medidas = 0;

	//Recuperamos todas las medidas de todas las balizas que hayan contestado
	//Las medidas est�n en el array paquetes con numero_medidas unidades
	int numero_medidas = LeerMedidasSensores();

	for (int i = 0; i < numero_medidas; i++)
	{
		bool BalizaNueva = false;
		sMedidas *pMedidasBaliza = ListaMedidasBalizas.search_code(paquete[i].id_baliza); //Localizamos la zona de medidas de la baliza

		sMedida Medida;
		Medida.tiempo_local = paquete[i].tiempo_local;	//micros() del reloj actual 
		Medida.tiempo_vuelo = paquete[i].tiempo;		//Tiempo de vuelo de la se�al recuperada por la baliza
		Medida.num_sensor = paquete[i].num_sensor;		//N�mero de transductor de la baliza que ha captado antes la se�al de US


		if (pMedidasBaliza == NULL)
		{ //No se ha localizado la zona de medidas, es una baliza nueva
			sMedidas MedidasBaliza;
			MedidasBaliza.codigo = MedidasBaliza.id_baliza = paquete[i].id_baliza;
			MedidasBaliza.tiempo_vida = millis();			//Inicio de tiempo de vida de la primera detecci�n de una se�al de esta baliza
			ListaMedidasBalizas.add_head(MedidasBaliza);
			MedidasBaliza.MedidasCorrectas = 0;				//Inicializamos las medidas correctas

			pMedidasBaliza = &MedidasBaliza;
			BalizaNueva = true;
		}
		//Ahora comprobamos los m�rgenes de la medida
		//1. Control de m�rgenes
		pMedidasBaliza->numero_medidas++;
		pMedidasBaliza->tiempo_ultima_medida = millis();
		if ((Medida.tiempo_vuelo < MIN_CONTROL_TIEMPO_VUELO) && (Medida.tiempo_vuelo > MAX_CONTROL_TIEMPO_VUELO))
		{ //Medida err�nea
			pMedidasBaliza->FallosTotales++;
			pMedidasBaliza->FallosAcumulados++;
		}
		else //Medida correcta, ajustamos la altura
		{
			//Localizamos la informaci�n de la baliza en la Lista de Balizas para recuperar su altura en eje Z
			long altura_baliza = ListaBalizas.search_code(paquete[i].id_baliza)->Z;
			//Los sensores rel robot, los sensores de la baliza y la recta recorrida por el sonido forma un tri�ngulo rect�ngulo
			float h = Medida.tiempo_vuelo;
			float Altura = abs(altura_baliza - ROBOT_ALTURA_SUELO_SENSORES);
			float distancia_ajustada_altura = pow(h, 2) - pow(Altura, 2);

			if (BalizaNueva)
			{ //Si cumple los m�rgenes, al ser una baliza nueva se inserta la medida directamente (no tenemos datos que cruzar)
				pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS, Medida);
				pMedidasBaliza->MedidasCorrectas++;
			}
			else
				//2. Control de medias. 
				//	 Debemos comprobar la media de variaci�n de las �ltimas 5 medidas	
			{
				//Hemos excedido el tiempo de inactividad sin detectar ninguna medida de la baliza
				if (abs(pMedidasBaliza->tiempo_vida - pMedidasBaliza->tiempo_ultima_medida) > MAX_TIEMPO_INACTIVIDAD)
				{ //Debemos ignorar todas las medidas anteriores e inicializar todos los contadores
					pMedidasBaliza->tiempo_vida = pMedidasBaliza->tiempo_ultima_medida;
					pMedidasBaliza->Medida.del_all();
					pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS, Medida);
					pMedidasBaliza->numero_ultima_medida = ProcesoMedida.numero_total_medidas;
					pMedidasBaliza->Inicializar();
				}
				else //La medida se encuentra dentro del margen temporal
				{
					int numero_medidas = pMedidasBaliza->Medida.m_num_nodes;

					//Reseteamos los valores si se ha reseteado el contador de micros() porque no se pueden comparar las medidas
					if (numero_medidas)
						if (pMedidasBaliza->Medida.search(0)->tiempo_vuelo > Medida.tiempo_local)
						{
							pMedidasBaliza->Medida.del_all();
							pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS, Medida);
							pMedidasBaliza->numero_ultima_medida = ProcesoMedida.numero_total_medidas;
							pMedidasBaliza->Inicializar();
						}
						else
						{
							//Solo realizamos el control de medias si tenemos como minimo NUM_MEDIDAS_VALIDAS
							//Obtenemos la pendiente media con respecto al tiempo
							//Obtenemos la media de las medidas almacenadas
							//La pendiente entre la medida media y la actual no debe superar en 3 veces la pendiente media
							if (numero_medidas >= NUM_MEDIDAS_VALIDAS)
							{
								sPaquete Medidas[NUM_MEDIDAS_BALIZAS];
								double Delta[NUM_MEDIDAS_BALIZAS]; //Pendientes de variaci�n de distancia con respecto al tiempo
								int numeros_deltas = 0;
								double Media_delta;
								double Media_tiempo;
								long ultimo_tiempo_vuelo; //Ultimo valor de tiempo de vuelo
								long ultimo_tiempo_local; //Ultimo valor de tiempo de vuelo

														  //La medida mas nueva es la primera. invertimos el array
								for (int i = numero_medidas - 1; i; i--)
								{
									Media_tiempo += Medidas[i].tiempo = pMedidasBaliza->Medida.search(i)->tiempo_vuelo;
									Medidas[i].tiempo_local = pMedidasBaliza->Medida.search(i)->tiempo_local;
								}
								for (int i = 1; i < numero_medidas; i++)
								{
									double tiempo_entre_medidas = Medidas[i].tiempo_local - Medidas[i - 1].tiempo_local;
									double diferencia_entre_medidas = abs(Medidas[i].tiempo - Medidas[i - 1].tiempo);
									if (tiempo_entre_medidas > 0) //Si se ha reseteado el contado de micros() ignoramos la medida
										Media_delta += (Delta[numeros_deltas++] = diferencia_entre_medidas / tiempo_entre_medidas);
								}
								Media_delta /= numeros_deltas; //Media de variaci�n ponderada
								Media_tiempo /= numero_medidas;

								if (abs(ultimo_tiempo_vuelo - Media_tiempo) / (Medida.tiempo_local - ultimo_tiempo_local) > Media_delta*MARGEN_MEDIDA_DELTA)
								{
									pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS, Medida); //Mantenemos las �ltimas NUM_MEDIDAS_BALIZAS medidas
									pMedidasBaliza->numero_ultima_medida = ProcesoMedida.numero_total_medidas;
									pMedidasBaliza->MedidasCorrectas++;
								}
								else
									pMedidasBaliza->FallosTotales++;
							}
						}
				}
			}
		}
	} //for balizas

	byte BalizasDetectadasUltimaMedida[2] = { 0,0 };
	byte numero_balizas = 0;
	//Primero comprobamos si hemos recuperado informaci�n de las dos balizas activas seleccionadas para la medida
	if (ProcesoMedida.num_balizas == 2)
	{
		for (int i = 0; i < ListaMedidasBalizas.m_num_nodes; i++)
		{
			if (ListaMedidasBalizas.search(i)->numero_ultima_medida == ProcesoMedida.numero_total_medidas)
			{
				if ((ProcesoMedida.BalizasSeleccionadas[0] == ListaMedidasBalizas.search(i)->id_baliza))
				{
					BalizasDetectadasUltimaMedida[0] = ProcesoMedida.BalizasSeleccionadas[0];
					numero_balizas++;
				}
				if ((ProcesoMedida.BalizasSeleccionadas[1] == ListaMedidasBalizas.search(i)->id_baliza))
				{
					BalizasDetectadasUltimaMedida[1] = ProcesoMedida.BalizasSeleccionadas[0];
					numero_balizas++;
				}
			}
		}
		//Comprobamos si seguimos detectando las mismas balizas
		if (numero_balizas == 2)
		{
			//Inicializamos el contador de errores acumulados de lectura
			ProcesoMedida.ErroresBalizasSeleccionadas[0] = ProcesoMedida.ErroresBalizasSeleccionadas[1] = 0;
			//Procedemos a triangular la posici�n
		}
		else //No hemos podido localizar las dos balizas
		{
			for (int i = 0; i < 2; i++)
			{
				if (!BalizasDetectadasUltimaMedida[i])
				{
					ProcesoMedida.ErroresBalizasSeleccionadas[i]++;
					if (ProcesoMedida.errores_acumulados >= ERRORES_CAMBIO_BALIZA)
					{ //Hemos excedido el m�ximo n�mero de errores de la baliza, debemos buscar otra

					}
				}
			}

		}
	}

	delay(5); //Retardo entre medidas
}
