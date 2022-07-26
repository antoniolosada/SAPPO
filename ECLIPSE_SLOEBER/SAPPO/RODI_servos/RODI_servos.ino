//The setup function is called once at startup of the sketch
/*  ----------------------------------------------------------------
M�dulo BT HC-05 (1)
Comandos AT a 38400. Se arranca con el bot�n pulsado
La velocidad del puerto de arduino es 9600 < velocidad puerto BT 38400
LA configuraci�n del m�dulo 1 es AT+UART=38400,1,0 -> 1 bit de parada y 0 bits de paridad y 8 de datos
AT+UART=34800,1,0
Clave 1234


http://www.prometec.net/nrf2401
Prog_79_1_Emisor

Programa para transmitir strings mediante radios NRF2401
1. Configurar los pines del RF24 para arduino uno o mega


Motor D punto central adelante < 70-80 < atras
Motor I punto central atras < 77 < adelante

Avanzar muy despacio hacia adelante
mi 85
md 55

COM5 38400 BT


-Orden real de los conectores de los transceptores
1230
/--\
                        -> Parte trasera
\--/
0123

-Orden en el array
7654
/--\
                        -> Parte trasera
\--/
0123
--------------------------------------------------------------------
*/

#ifndef RODI_SERVOS_H_CARGADA
	#include <ServoTimer2.h>
	#include <SingleEMAFilterLib.h>
	#include <MeanFilterLib.h>
	#include "FiltroKalman.h"
	#include "PID_v2.h"
	#include "SoftwareServo.h"
	#include "list.h"
	#include "node_cpp.h"
	#include "Kalman.h"
	#include <RF24.h>
	#include <RF24_config.h>
	#include <SPI.h>
	#include <VirtualWire.h>
	#include <nRF24L01.h>
	#include <PlotPlus.h>
	#include "RODI_servos.h"

	#include "list.cpp"
	#include "node_cpp.cpp"
#endif

/*  Funcines ************************************************************************************************/
bool RecuperarMedidasBalizas(int algoritmo, bool salida);
int freeRam();
void RecuperarComandos();
void ContarPulsoRueda(int rueda, int lecturaAct);
void EnviarMensaje433Mhz(int lectura);
boolean RecuperarMedidaMultiple(int emisor, int LOG);
void setup(void) ;
void loop(void) ;
ISR(TIMER2_COMPA_vect) ;
void PruebaODO() ;
void LOG_ruedas(int log) ;
int Mapear(int dir, int v1m, int v1M, int v2m, int v2M, int Velocidad) ;
void ActivarTimer2() ;
void LOG() ;
void moverServo(int pin, int angulo) ;
void CargarDatosBalizas() ;
boolean LeerMedidasSensores(int emisor) ;
float Tiempo2Distancia(long Tiempo) ;
void LeerSensoresUltrasonidos();
long LeerSensorUltrasonidos(int iSensor);
float RecuperarOrientacion(int Baliza, float Medida, int NumSensores);
bool RecuperarDistanciasSonar(int Sensor);
void test();
/*  Funcines ************************************************************************************************/


//#include "ServoTimer2.h"
int SensorEmisor = EMISION_CONJUNTA;
bool RecuperarPosicion = false;

byte aTrigger[MAX_EMISORES_ULTRASONIDOS] =
						{TRIGER6, TRIGER7, TRIGER8, TRIGER5, TRIGER4, TRIGER3, TRIGER2, TRIGER1};
byte aEcho[MAX_EMISORES_ULTRASONIDOS] =
						{ECHO6, ECHO7, ECHO8, ECHO5, ECHO4, ECHO3, ECHO2, ECHO1};
//byte aTrigger[MAX_EMISORES_ULTRASONIDOS] =
//				{TRIGER1, TRIGER2, TRIGER3, TRIGER4,TRIGER5, TRIGER7, TRIGER8, TRIGER6};
//byte aEcho[MAX_EMISORES_ULTRASONIDOS] =
//				{ECHO1, ECHO2, ECHO3, ECHO4,ECHO5, ECHO7, ECHO8, ECHO6};
long  aLectDistancia[MAX_EMISORES_ULTRASONIDOS];

String comando;

char msg[16] = "."; // Array a transmitir


List<sBaliza> ListaBalizas;
List<sMedidas> ListaMedidasBalizas;
sProcesoMedida ProcesoMedida;

RF24 radio(9,53);

const uint64_t pipe1 = 0x00000000E1LL;    // Usamos este canal
const uint64_t pipe2 = 0x00000000E2LL;    // Usamos este canal
const uint64_t broadcast = 0xE8E8F0F0E1LL; // Usamos este canal
char direccion[3][6] = {"0Nodo", "1Nodo", "2Nodo"};

sPaquete paquete[MAX_BALIZAS];
sPaqueteRF433 paqueteRF433;

void CargarDatosBalizas();
void EnviarPulsoUltrasonidos(int emisor);
void ActivarTimer2();
long TiempoArranque = 0;

boolean PararMotores = true;

SoftwareServo Motor[2];

FiltroKalman2D fk;       // FiltroKalman para 1 dimension
FiltroKalman1D fkRuedaI; // FiltroKalman para 1 dimension
Vec2f out, input;

sOdometria Odo[2];
double SetpointVal[2], Setpoint[2], Input[2], Output[2];
PID2 Pid[2] = {PID2(&Input[IZQUIERDA], &Output[IZQUIERDA], &Setpoint[IZQUIERDA],
                    0.6, 0.3, 0.01, REVERSE),
               PID2(&Input[DERECHA], &Output[DERECHA], &Setpoint[DERECHA], 0.6,
                    0.3, 0.01, REVERSE)};

double InputGiro = 1, OutputGiro = 1, SetpointGiro = 1;
PID2 PidGiro(&InputGiro, &OutputGiro, &SetpointGiro, 2, 0, 0, DIRECT);

int sensor_baliza[2] = {-1, -1};
char PotenciaArranque[2] = {15, 0};
byte PotenciaMinima[2] = {0, 0};
byte PotenciaMaxima[2] = {40, 40};
Kalman myFilterX(0.125, 32, 1023,
                 0); // suggested initial values for high noise filtering
Kalman myFilterY(0.125, 32, 1023,
                 0); // suggested initial values for high noise filtering
                     // Kalman myFilterY(0.125, 32, 1023, 0); //suggested
                     // initial values for high noise filtering
float KalmanX, KalmanY;
float MediaX, MediaY;
float MedidaX, MedidaY;
float filtroX, filtroY;
float filtroX_media, filtroY_media;
float filtroX_EMA, filtroY_EMA;
float filtroX_FIR, filtroY_FIR;
//int filtro = KALMAN1 | MEDIA | EMA | MEDIA_EVOLUTIVA | MEDIA_COMPENSADA;
int filtro = KALMAN2 | MEDIA_EVOLUTIVA;
int algoKalman = KALMAN2;

Kalman klmRev[2] = {Kalman(0.125, 32, 1023, 0), Kalman(0.125, 32, 1023, 0)};
#define WINDOWS_SIZE 10
MeanFilter<float> filtroMediaX(WINDOWS_SIZE);
MeanFilter<float> filtroMediaY(WINDOWS_SIZE);

SingleEMAFilter<float> singleEMAFilterX(0.2);
SingleEMAFilter<float> singleEMAFilterY(0.2);


/*****************************************************************   SETUP    ********************************************************************/
/*****************************************************************   SETUP    ********************************************************************/
/*****************************************************************   SETUP    ********************************************************************/

void setup(void) {

  // turn the PID on
  for (int i = 0; i < 2; i++) {
    Pid[i].SetMode(AUTOMATIC);
    Pid[i].SetSampleTime(SAMPLE_TIME);
    Pid[i].SetOutputLimits(PotenciaMinima[i], PotenciaMaxima[i]);
  }
  for (int i = 0; i < 2; i++) {
    SetpointVal[i] = SET_POINT_INI;
    Setpoint[i] = SET_POINT_INI;
    Output[i] = 0;
    Input[i] = 0;
  }

  pinMode(13, OUTPUT);

  pinMode(RUEDA_DER_ODO, INPUT);
  pinMode(RUEDA_IZQ_ODO, INPUT);

  for (int i = 0; i < 1; i++)
    for (int j = 0; j < 1; j++) {
      Odo[i].Contador[j].pulsos = Odo[i].Contador[j].Tiempo =
          Odo[i].Contador[j].Ultimo_ms = 0;
    }

  //Motor[IZQUIERDA].attach(SERVO_IZQ);
  //Motor[DERECHA].attach(SERVO_DER);
  // Motor[IZQUIERDA].write(MOTOR_I_PARADO);
  // Motor[DERECHA].write(MOTOR_D_PARADO);

  for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++) {
    pinMode(aTrigger[i], OUTPUT);
    pinMode(aEcho[i], INPUT);
  }

  // Motores
  pinMode(LINEA_der, INPUT);
  pinMode(LINEA_izq, INPUT);
  pinMode(LINEA_centro, INPUT);

  pinMode(RUEDA_der, INPUT);
  pinMode(RUEDA_izq, INPUT);

  // Ultrasonidos
  pinMode(PIN_W, OUTPUT); /*salida: para el pulso ultras?nico*/
  pinMode(PIN_R, INPUT);  /*entrada: tiempo del rebote del ultrasonido*/

  // Initialise the IO and ISR
  vw_set_tx_pin(PIN_RADIO_TX);
  vw_setup(2500); // Bits per sec
                  // vw_rx_start();       // Start the receiver PLL running


  //****************Velocidad de puertos serie *********************************
  Serial.begin(57600);
  Serial1.begin(38400);   // Configurar puerto para el m?dulo bluetooth
  //****************Velocidad de puertos serie *********************************


  pinMode(BT_ENABLE, OUTPUT);
  digitalWrite(BT_ENABLE, HIGH);

  radio.begin();
  //radio.setPALevel(RF24_PA_HIGH);
  //radio.openReadingPipe(0, broadcast);
  radio.openReadingPipe(1, pipe1);
  radio.openReadingPipe(2, pipe2);

  radio.setChannel(108);
  radio.setRetries(3, 3);
  radio.setAutoAck(true);

  CargarDatosBalizas();

  // attachInterrupt(digitalPinToInterrupt(RUEDA_DER_ODO), intRueda_der,
  // CHANGE); attachInterrupt(digitalPinToInterrupt(RUEDA_IZQ_ODO), intRueda_izq,
  // CHANGE);

  // ArrancarMotor();

  //ActivarTimer2();

  fk.init(0.001, 0.025);
  fkRuedaI.init(0.001, 0.025);

  Odo[DERECHA].pulsos_totales == -1;
  Odo[DERECHA].ultimo_valor = digitalRead(RUEDA_DER_ODO);
  Odo[IZQUIERDA].pulsos_totales == -1;
  Odo[IZQUIERDA].ultimo_valor = digitalRead(RUEDA_IZQ_ODO);

  PidGiro.SetMode(AUTOMATIC);
  PidGiro.SetSampleTime(SAMPLE_TIME);

  delay(10);
  // Muestra la memoria de datos disponible
  Serial1.println("------------------------------------");
  Serial.print("M.Free: ");
  Serial.println(freeRam());
  Serial1.println("------------------------------------");
  Serial1.println("------------------------------------");
  Serial1.print("M.Free: ");
  Serial1.println(freeRam());
  Serial1.println("------------------------------------");
  delay(2);
}

/*****************************************************************   LOOP   ********************************************************************/

long iteracion = 0;
long repeticiones = 0;
long pulsos_ant = 0;
long cont_loop = 0;
unsigned long contTimeOff_us = 0;

#define REC_MEDIDAS  1

void loop(void)
{
	//test();
	//return; /*******************************************************************************************************************************/
	cont_loop++;

	//Tiempo de parada de emisi�n de ultrasonidos
	if (contTimeOff_us > 0)
		if ((millis() - contTimeOff_us > TIME_OFF_MS) || (millis() < contTimeOff_us))
		{
			RecuperarPosicion = false;
			contTimeOff_us = 0;
		}
	//Tiempo de parada de los motores
	if (TiempoArranque > 0)
		if ((millis() - TiempoArranque > MAX_MS_FUNC_MOTOR) || (millis() < TiempoArranque))
			ControlMotores(-1, -1, OP_MOTOR_PARAR);

	bool medidas = false;

	Espera(20, 0, OP_INICIAR);
	while(Espera(20, 0, OP_CONSULTAR))
		if (!PararMotores) SoftwareServo::refresh();

	RecuperarComandos();

    if ((cont_loop % REC_MEDIDAS) == 0 )
    {
    	if (RecuperarPosicion)
    		medidas = RecuperarMedidasBalizas(algoKalman, false);
    }
   	else
	{
//	  filtroX = myFilterX.getFilteredValue(filtroX);
//	  filtroY = myFilterY.getFilteredValue(filtroY);
	}

	if (medidas)
	{
		//LOG
//		Serial.print(MediaX);
//		Serial.print(",");
//		Serial.println(MedidaX);


		// Salida para EXCEL
		// Salida por serie BT
		//plot(5*filtroX, 5*filtroY); Serial.println("");
		SoftwareServo::refresh();

		Serial1.print("BAL:");
		Serial1.print(MediaX);
		Serial1.print(",");
		Serial1.print(MediaY);
		Serial1.print(",");

	    if (algoKalman == KALMAN1)
	    {
			Serial1.print(filtroX);
			Serial1.print(",");
			Serial1.print(filtroY);
			KalmanX = filtroX;
			KalmanY = filtroY;
	    }
	    else
	    {
			Serial1.print(out[0]);
			Serial1.print(",");
			Serial1.print(out[1]);
			KalmanX = out[0];
			KalmanY = out[1];
	    }
		Serial1.print(",");
		Serial1.print(Odo[DERECHA].pulsos_totales);
		Serial1.print(",");
		Serial1.print(Odo[IZQUIERDA].pulsos_totales);

		SoftwareServo::refresh();
		Serial1.print(",");
		Serial1.print(sensor_baliza[0]);
		Serial1.print(",");
		Serial1.println(sensor_baliza[1]);
	}
}

void(* resetSoftware)(void) = 0;

/****  TEST *********************************************/
void test()
{
	// PruebaODO();
    // return;

	float orientacion = RecuperarOrientacion(1, MediaX, 8);
	delay(50);
	Serial.print("Orientacion: ");
	Serial.println(orientacion);

	return;
//	LeerSensoresUltrasonidos();
//	for (int i=0; i< MAX_EMISORES_ULTRASONIDOS; i++)
//	{
//		  Serial.print(aLectDistancia[i]);
//		  Serial.print(", ");
//	}
//	Serial.println("");
//	delay (30);
}
/****  END TEST *********************************************/

#define TIMER2_OVF 200
int contador = TIMER2_OVF;
int pulso_der = LOW;
int pulso_izq = LOW;
long contadorODO = 0;
char lecturader, lecturaizq;
#define CICLO_ODO 20 // Se controla la odometr?a cada 8ms

/*
ISR(TIMER2_COMPA_vect) { // timer1 interrupt 8kHz toggles pin 9

  if (contadorODO++ > CICLO_ODO) {
    // Incremento de tiempo en caso de que el tiempo grabado sea menor que el
    // tiempo actual transcurrido desde el ultimo cambio
    for (int rueda = 0; rueda < 2; rueda++) {
      for (int lectura = 0; lectura < 2; lectura++) {
        long tiempo = millis() - Odo[rueda].Contador[lectura].Ultimo_ms;
        if (tiempo > Odo[rueda].Contador[lectura].Tiempo)
          Odo[rueda].Contador[lectura].Tiempo = tiempo;
      }
    }

    lecturader = digitalRead(RUEDA_DER_ODO);
    if (lecturader != pulso_der) {
      ContarPulsoRueda(DERECHA, lecturader);
      pulso_der = lecturader;
    }

    lecturaizq = digitalRead(RUEDA_IZQ_ODO);
    if (lecturaizq != pulso_izq) {
      ContarPulsoRueda(IZQUIERDA, lecturaizq);
      pulso_izq = lecturaizq;
    }
    contadorODO = 0;
  }

  if (!PararMotores)
    if (--contador == 0) {
      contador = TIMER2_OVF;
    }
}
*/

#define INC 5

void PruebaODO() {
  float tiempoD, tiempoI;

  PararMotores = 1;

  tiempoD = (int)(Odo[DERECHA].Contador[0].Tiempo / 10);
  Setpoint[DERECHA] = 27;
  Input[DERECHA] = tiempoD;
  Motor[DERECHA].write(MOTOR_D_PARADO + Output[DERECHA]);
  Pid[DERECHA].Compute();

  tiempoI = (int)(Odo[IZQUIERDA].Contador[0].Tiempo / 10);
  Input[IZQUIERDA] = tiempoI;
  Setpoint[IZQUIERDA] = 27;
  Motor[IZQUIERDA].write(MOTOR_I_PARADO - Output[IZQUIERDA]);
  Pid[IZQUIERDA].Compute();

  delay(50);

  // Serial.print((int)(Input[DERECHA]));
  // Serial.print(Output[DERECHA]);
  // Serial.println("");

  // Serial.print(" - ");
  // Serial.print(InputGiro);
  // Serial.print(" - ");
  // Serial.println(OutputGiro);

  // SetpointGiro = 1.5;
  // Motor[DERECHA].write(MOTOR_D_PARADO + 20 + OutputGiro * INC);
  ////Rebajamos los contadores
  // if (Odo[DERECHA].pulsos_totales == 0)
  //	InputGiro = 1;
  // else
  //	InputGiro = 1.0 * Odo[IZQUIERDA].pulsos_totales /
  //Odo[DERECHA].pulsos_totales;

  // LOG_ruedas(1);
}

void LOG_ruedas(int log) {
  static int v = 0;

  if (log == 0) {
    Serial.print(digitalRead(RUEDA_IZQ_ODO));
    if (Odo[IZQUIERDA].pulsos_totales != pulsos_ant) {
      Serial.print("-");
      Serial.println(Odo[IZQUIERDA].pulsos_totales);
      pulsos_ant++;
    }
  }
  if (log == 1) {
    Serial.print(digitalRead(RUEDA_DER_ODO));
    if (Odo[DERECHA].pulsos_totales != pulsos_ant) {
      Serial.print("-");
      Serial.print(Odo[DERECHA].pulsos_totales);
      Serial.print(" - ");
      Serial.println(1.0 * Odo[IZQUIERDA].pulsos_totales /
                     Odo[DERECHA].pulsos_totales);
      pulsos_ant++;
    }
  }
  if (log == 2) {
    Serial.print(Odo[IZQUIERDA].pulsos_totales / 10);
    Serial.print(", ");
    Serial.println(Odo[DERECHA].pulsos_totales / 10);
  }
  if (log == 3) {
    Serial.print(Input[IZQUIERDA]);
    Serial.print(", ");
    Serial.println(Input[DERECHA]);
  }
}

int Mapear(int dir, int v1m, int v1M, int v2m, int v2M, int Velocidad) {
  double ratio = (double)abs(v1M - v1m) / (v2M - v2m);
  return (int)(dir * (Velocidad - v2m) * ratio + v1m);
}

void ActivarTimer2() {
  cli();
  // set timer2 interrupt at 8kHz
  TCCR2A = 0; // set entire TCCR2A register to 0
  TCCR2B = 0; // same for TCCR2B
  TCNT2 = 0;  // initialize counter value to 0
             // set compare match register for 8khz increments
  OCR2A = 249; // = (16*10^6) / (8000*8) - 1 (must be <256)
               // turn on CTC mode
  TCCR2A |= (1 << WGM21);
  // Set CS21 bit for 8 prescaler
  TCCR2B |= (1 << CS21);
  // enable timer compare interrupt
  TIMSK2 |= (1 << OCIE2A);
  sei();
}

void LOG() {}

void moverServo(int pin, int angulo) {
  float pausa;
  pausa = angulo * 2000.0 / 180.0 + 500;
  digitalWrite(pin, HIGH);
  delayMicroseconds(pausa);
  digitalWrite(pin, LOW);
  delayMicroseconds(2500 - pausa);
}
void CargarDatosBalizas() {
  sBaliza Baliza;
  Baliza.codigo = 1;
  Baliza.id_baliza = 1;
  Baliza.id_habitacion = 1;
  Baliza.grados_cobertura = 90;
  Baliza.direccion = 1;
  Baliza.numero_sensores = 4;
  Baliza.X = 10;
  Baliza.Y = 10;
  Baliza.Z = 10; // Altura de los sensores desde el suelo
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

boolean LeerMedidasSensores(int emisor) {
  for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++)
    paquete[i].id_baliza = 0;

  EnviarMensaje433Mhz(0);
  EnviarPulsoUltrasonidos(emisor);

  radio.startListening();

  unsigned long started_waiting_at = millis();
  bool timeout = false;

  float medida;
  int numero_medidas = 0;
  // Recuperamos todas las medidas posibles de las balizas
  while (
      !timeout) // Esperamos hasta el m?ximo tiempo de vuelo de una se?al de US
  {
    while ((!radio.available()) && !timeout) // Esperamos
    {
      if (millis() - started_waiting_at > MAX_TIEMPO_VUELO_SONIDO)
        timeout = true;
    }

    if (!timeout) { // Leemos el mensaje recibido
      unsigned long got_time;
      radio.read(&paquete[numero_medidas], sizeof(sPaquete));
      medida = 1.0 * paquete[numero_medidas].tiempo /
               29.2; // convertimos a distancia, en cm
      paquete[numero_medidas].tiempo = medida;
      paquete[numero_medidas].tiempo_local = micros();
      numero_medidas++;
    }
  }

  radio.stopListening();

  return numero_medidas;
}

float Tiempo2Distancia(long Tiempo) { return (1.0 * Tiempo * 0.0346); } // 0.0343 cm/us

bool RecuperarMedidasBalizas(int algoritmo, bool salida)
{
  float minimo, medida[2], media, maximo;
  int medidas, id_baliza_x = 0, id_baliza_y = 0;
  boolean timeout;
  int baliza1, baliza2;

  minimo = 999;
  maximo = 0;
  media = 0;
  medidas = 0;

  // Recuperamos dos medidas de sensores contrapuestos v?lidos para
  // triangulaci?n
  //timeout = RecuperarMedidaMultiple(SensorEmisor, LOG_MEDIDAS | LOG_LECTURAS);
  timeout = RecuperarMedidaMultiple(SensorEmisor, false);

  // Si timeout = true implica que no hemos podido recuperar dos medidas v?lidas
  // para triangular
  if (!timeout)
  {
    // Primero comprobamos que los identificadores de las balizas son correctas
    if ((paquete[0].id_baliza != paquete[1].id_baliza) && (paquete[1].id_baliza+paquete[1].id_baliza != 0))
    {
        baliza1 = (paquete[0].id_baliza == 1)?0:1;
        baliza2 = (baliza1 == 0)?1:0;

      // Convertimos el tiempo a cm
        medida[0] = Tiempo2Distancia(paquete[baliza1].tiempo);
        medida[1] = Tiempo2Distancia(paquete[baliza2].tiempo);

        sensor_baliza[0] = paquete[baliza1].num_sensor;
        sensor_baliza[1] = paquete[baliza2].num_sensor;

      //Salida EXCEL
	  //plot(paquete[0].tiempo/10, paquete[1].tiempo/10); Serial.println("");

      // Verificamos si las dos medidas son correctas
      if (((medida[0] > 10) && (medida[0] < 1000)) &&
          ((medida[1] > 10) && (medida[1] < 1000)))
      {
    	  MedidaX = medida[0];
    	  MedidaY = medida[1];

    	  if (filtro & KALMAN1)
    	  {
			  filtroX = myFilterX.getFilteredValue(medida[0]);
			  filtroY = myFilterY.getFilteredValue(medida[1]);
			  if (salida)
			  {
				Serial.print(filtroX);
				Serial.print(",");
				Serial.print(filtroY);
			  }
    	  }
    	  if (filtro & KALMAN2)
    	  {
			  input[0] = medida[0];
			  input[1] = medida[1];
			  fk.update(input);
			  out = fk.getEstimation();
			  if (salida)
			  {
				Serial.print(out[0]);
				Serial.print(",");
				Serial.print(out[1]);
			  }
    	  }
    	  if (filtro & MEDIA)
    	  {
			  filtroX_media = filtroMediaX.AddValue(medida[0]);
			  filtroY_media = filtroMediaY.AddValue(medida[1]);
			  if (salida)
			  {
				Serial.print(filtroX_media);
				Serial.print(",");
				Serial.print(filtroY_media);
			  }
    	  }
    	  if (filtro & EMA)
    	  {
			  singleEMAFilterX.AddValue(medida[0]);
			  singleEMAFilterY.AddValue(medida[1]);
			  filtroX_EMA = singleEMAFilterX.GetLowPass();
			  filtroY_EMA = singleEMAFilterY.GetLowPass();

			  if (salida)
			  {
				Serial.print(filtroX_EMA);
				Serial.print(",");
				Serial.print(filtroY_EMA);
			  }
    	  }
    	  if (filtro & MEDIA_EVOLUTIVA)
    	  {
    		  if (MediaX == 0)
    			  MediaX = medida[0];
    		  else
    			  MediaX = (MediaX+MedidaX) /2;
    		  if (MediaY == 0)
    			  MediaY = medida[1];
			  else
				  MediaY = (MediaY+MedidaY) /2;

			  if (salida)
			  {
				Serial.print(MediaX);
				Serial.print(",");
				Serial.print(MediaY);
			  }
    	  }
    	  if (salida == SALIDA_COMPLETA)
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
    	  else if (salida)
    		  Serial.println("");
  		return true;
      } //if medida
    } //if paquete
  } //if timeout
  return false;
}

long LeerSensorUltrasonidos(int iSensor)
{
	  long tiempo_ini = micros();

	  EnviarPulsoUltrasonidos(iSensor);

	  return pulseIn(aEcho[iSensor], LOW, MAX_TIEMPO_ECO);

}

void LeerSensoresUltrasonidos()
{
  long tiempo_ini = micros();
  int lectura_recuperada = MAX_EMISORES_ULTRASONIDOS;

  for(int i=0; i<MAX_EMISORES_ULTRASONIDOS; i++)
	  aLectDistancia[i]=0;

  EnviarPulsoUltrasonidos(EMISION_CONJUNTA);
  // Esperamos a que la se�al de lectura pase a LOW por un transductor
  while (digitalRead(aEcho[0]) == LOW);

  long tiempo_espera = MAX_TIEMPO_ECO;
  //Inicializamos el contador de tiempo
  Espera(tiempo_espera, 0, OP_INICIAR);

  lectura_recuperada = MAX_EMISORES_ULTRASONIDOS;

  while((Espera(tiempo_espera, 0, OP_CONSULTAR)) && (lectura_recuperada)) //Dentro del tiempo m�ximo de eco intentamos captar la lectura de todos los sensores
  {
      for(int i=0; i<MAX_EMISORES_ULTRASONIDOS; i++)
      {
        if (digitalRead(aEcho[i]) == LOW) //Hemos detectado el tren de pulsos ultras�nicos en el sensor i
        {
        	if (aLectDistancia[i] == 0)
        	{
            	aLectDistancia[i]=micros()-tiempo_ini;
            	lectura_recuperada--;
        	}
        }
      }
  }
}


float RecuperarMedida() {
  sPaquete paquete;
  // radio.write(&paquete, sizeof(paquete));
  EnviarMensaje433Mhz(0);
  EnviarPulsoUltrasonidos(SensorEmisor);

  radio.startListening();

  unsigned long started_waiting_at = millis();
  bool timeout = false;

  float medida;
  while ((!radio.available()) && !timeout) // Esperamos
  {
    if (millis() - started_waiting_at > 80) {
      timeout = true;
      //Serial.println("err");
    }
  }

  if (!timeout) { // Leemos el mensaje recibido
    unsigned long got_time;
    radio.read(&paquete, sizeof(sPaquete));
    medida = 1.0 * paquete.tiempo / 29.2; // convertimos a distancia, en cm
  }
  radio.stopListening();

  return medida;
}

// Recupera medida de todas las balizas

boolean RecuperarMedidaMultiple(int emisor, int LOG) {
  paquete[0].id_baliza = 0;
  paquete[1].id_baliza = 0;
  // radio.write(&paquete, sizeof(paquete));
  EnviarMensaje433Mhz(0);
  EnviarPulsoUltrasonidos(emisor);

  radio.startListening();

  unsigned long started_waiting_at = millis();
  bool timeout = false;

  float medida;
  int numero_medidas = 0;
  while (!timeout && (numero_medidas < NUM_BALIZAS_RECUPERAR)) // Esperamos
  {
    while ((!radio.available()) && !timeout) // Esperamos
    {
    	if (!PararMotores) SoftwareServo::refresh();

    	if (millis() - started_waiting_at > TIMEOUT_MEDIDA_US) {
    		timeout = true;
    		//Serial.println("err");
      }
    }

    if (!timeout) { // Leemos el mensaje recibido
      unsigned long got_time;
      radio.read(&paquete[numero_medidas], sizeof(sPaquete));
      radio.flush_rx();
      // medida = 1.0 * paquete[numero_medidas].tiempo / 29.2;   //convertimos a
      // distancia, en cm paquete[numero_medidas].tiempo = medida;

      if (LOG & LOG_MEDIDAS)
      {
    	  if (paquete[numero_medidas].id_baliza == 2)
    	  {
              Serial.print("medida ");
              Serial.print(paquete[numero_medidas].id_baliza);
              Serial.print(",");
              Serial.print(paquete[numero_medidas].num_sensor);
              Serial.print(",");
              Serial.println(cont_loop);
    	  }
      }
      numero_medidas++;
    }
  }

  if (!timeout)
  {
    int baliza2, baliza1;
    if (paquete[0].id_baliza == 1)
    {
        baliza1 = 0;
        baliza2 = 1;
    }
    else
    {
        baliza1 = 1;
        baliza2 = 0;
    }
    if (LOG & LOG_LECTURAS)
    {
      Serial1.print("lectura: ");
      Serial1.print(paquete[baliza2].tiempo);
      Serial1.print(" - ");
      Serial1.print(paquete[baliza2].id_baliza);
      Serial1.print(" , ");
      Serial1.print(paquete[baliza1].tiempo);
      Serial1.print(" - ");
      Serial1.println(paquete[baliza1].id_baliza);
    }
}

  radio.stopListening();

  return timeout;
}

#define SIN_LECTURA 255

bool RecuperarDistanciasSonar(int Sensor)
{
	#define MAX_MS 50000
	long aTiempo[MAX_EMISORES_ULTRASONIDOS+1];

	aTiempo[0] = MAX_EMISORES_ULTRASONIDOS;
	for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS; emisor++) {
		EnviarPulsoUltrasonidos(emisor);

		aTiempo[emisor+1] = pulseIn(aEcho[emisor], HIGH);
		if (aTiempo[emisor+1] > MAX_MS)
			aTiempo[emisor+1] = 0;

		delay(MIN_TIMEOUT_MS_US);
	}

	Serial1.print("SON:");
	for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS+1; emisor++) {
		Serial1.print(aTiempo[emisor]);
		if (emisor +1 < MAX_EMISORES_ULTRASONIDOS+1) Serial1.print(",");
	}
	Serial1.println("");
}

float RecuperarOrientacion(int Baliza, float Medida, int NumSensores) {
#define MAX_MEDIDAS	3
	int numero_baliza = 0;
	int Sensor = -1;
	float DistanciaSensor = MAX_DISTANCIA_US_CM;
	float Medidas[MAX_EMISORES_ULTRASONIDOS];
	float Diferencia;

	sPaquete paquete[MAX_EMISORES_ULTRASONIDOS][MAX_MEDIDAS][NUM_BALIZAS_RECUPERAR];
	// radio.write(&paquete, sizeof(paquete));

	// Recuperamos las medidas desde todos los sensores
	for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS; emisor++)
	{
		for (int baliza = 0; baliza < NUM_BALIZAS_RECUPERAR; baliza++)
			for (int medida = 0; medida < MAX_MEDIDAS; medida++)
				paquete[emisor][medida][baliza].id_baliza = SIN_LECTURA;
	}

	for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS; emisor++)
	{
		for (int medida = 0; medida < MAX_MEDIDAS; medida++)
		{
			Espera(MIN_TIMEOUT_MS_US, 0, OP_INICIAR);
			numero_baliza = 0;
			EnviarMensaje433Mhz(0);
			EnviarPulsoUltrasonidos(emisor);

			radio.startListening();

			unsigned long started_waiting_at = millis();
			bool timeout = false;

			while (!timeout && (numero_baliza < NUM_BALIZAS_RECUPERAR)) // Esperamos
			{
				while ((!radio.available()) && !timeout) // Esperamos
				{
					if (millis() - started_waiting_at > TIMEOUT_MEDIDA_US)
						timeout = true;
				}

				if (!timeout) { // Leemos el mensaje recibido
					unsigned long got_time;
					radio.read(&paquete[emisor][medida][numero_baliza],
							sizeof(sPaquete));
					numero_baliza++;
				}
			}
			radio.stopListening();
			while(Espera(MIN_TIMEOUT_MS_US, 0, OP_CONSULTAR));
		} //for medida
	} //for emisor

	//Calculamos la media de todas las medidas
	for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS; emisor++)
	{
		Medidas[emisor] = MAX_DISTANCIA_US_CM;
		for (int medida = 0; medida < MAX_MEDIDAS; medida++) {
			for (int baliza = 0; baliza < NUM_BALIZAS_RECUPERAR; baliza++) {
				if (paquete[emisor][medida][baliza].id_baliza == Baliza) {
					Diferencia = Medida
							- Tiempo2Distancia(paquete[emisor][medida][baliza].tiempo);
					Medidas[emisor] = (
							Medidas[emisor] != MAX_DISTANCIA_US_CM ?
									(Medidas[emisor] / 2 + Diferencia / 2) :
									Diferencia);
				}
			}
		}
	}

	//Identificamos las medidas m�s pr�ximas a la real
	for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS; emisor++) {
		if (abs(Medidas[emisor]) < DistanciaSensor)
		{
			DistanciaSensor = abs(Medidas[emisor]);
			Sensor = emisor;
		}
		Serial.print(emisor);
		Serial.print(" , ");
		Serial.println(Medidas[emisor]);
	}

	if (Sensor != -1)
	{
		Serial.print("Medida: ");
		Serial.print(Medida);
		Serial.print(", Distancia: ");
		Serial.print(DistanciaSensor);
		Serial.print(", Emisor: ");
		Serial.println(Sensor);
	}

	/*LOG de la informaci�n de alcane de los sensores*/
	//  for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS; emisor++)
	//  {
	//	Serial.print("ORIENTACION SENSOR: ");
	//	if (Sensor == emisor)
	//		Serial.print("(S) ");
	//	Serial.println(emisor);
	//
	//	for (int baliza = 0; baliza < NUM_MEDIDAS_RECUPERAR; baliza++)
	//	{
	//	  if (paquete[baliza][emisor].id_baliza != SIN_LECTURA)
	//	  {
	//		Serial.print("  ->Baliza: ");
	//		Serial.print(paquete[baliza][emisor].id_baliza);
	//		Serial.print(", Distancia: ");
	//		Serial.print(Tiempo2Distancia(paquete[baliza][emisor].tiempo));
	//		Serial.print(", Sensor: ");
	//		Serial.println(paquete[baliza][emisor].num_sensor);
	//	  }//if
	//	}//for
	//  }
	float Orientacion =
			(Sensor == -1) ?
					-1 : (360 / NumSensores) / 2 + Sensor * (360 / NumSensores);

	return Orientacion;
}

void EnviarPulsoUltrasonidos(int emisor) {
  long tiempo;
  int distanceCm;
  if (emisor != EMISION_CONJUNTA) {
    digitalWrite(aTrigger[emisor],
                 LOW); /* Por cuesti?n de estabilizaci?n del sensor*/
    delayMicroseconds(5);
    digitalWrite(aTrigger[emisor], HIGH); /* env?o del pulso ultras?nico*/
    delayMicroseconds(15);
    digitalWrite(aTrigger[emisor], LOW); /* env?o del pulso ultras?nico*/
  } else {
    for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++)
        digitalWrite(aTrigger[i], LOW); /* Por cuestion de estabilizacion del sensor*/
    delayMicroseconds(5);
    for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++)
        digitalWrite(aTrigger[i], HIGH); /* envio del pulso ultrasonico*/
    delayMicroseconds(15);
    for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++)
        digitalWrite(aTrigger[i], LOW); /* envio del pulso ultrasonico*/
  }

  /* C�lculo de distancia en cm
  tiempo = pulseIn(ECHO1, HIGH)+1;

  distanceCm = tiempo * 10 / 292/ 2;   //convertimos a distancia, en cm

  Serial.println(distanceCm);
  */
}

bool Espera(unsigned long ms, int contador, int op) {
#define MAX_US 4294967295

  static unsigned long us_ini[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
  unsigned long us_actual[10];

  us_actual[contador] = micros();

  if (op == OP_INICIAR)
    us_ini[contador] = micros();
  else {
    if (us_actual[contador] < us_ini[contador]) {
      us_actual[contador] = MAX_US - us_ini[contador] + us_actual[contador];
      us_ini[contador] = 1;
    }

    if (us_actual[contador] - us_ini[contador] >= ms * 1000) {
      us_ini[contador] = 0;
      return false;
    } else
      return true;
  }
}

void EnviarMensaje433Mhz(int lectura) {
  paqueteRF433.id_robot = ID_ROBOT_RODI;
  paqueteRF433.operacion = ID_OPERACION_MEDIDA;

  // digitalWrite(13, true); // Flash a light to show transmitting

  vw_send((uint8_t *)&paqueteRF433, 2);

  Espera(35, 0, OP_INICIAR);
  while(Espera(35, 0, OP_CONSULTAR))
	  SoftwareServo::refresh();
  vw_wait_tx(); // Wait until the whole message is gone duraci�n= 38400 us

//  while(vw_tx_active())
//	  SoftwareServo::refresh();

}

void InicializarDatosBalizas(struct sBaliza *Balizas) {
  Balizas[0].id_baliza = 0;
  Balizas[0].id_habitacion = 0;

  return;
}

int InterseccionCircunferencias(int iD1, int iD2, struct sBaliza BalizaAnt,

                                struct sBaliza BalizaSig, long &lPosX,

                                long &lPosY) {
  float fA1, fB1, fC1, fA2, fB2, fC2;
  double D, E, F, a, b, c, d, x1, x2, y1, y2;
  double a1, b1, r1, a2, b2, r2;
  long v1x1, v1y1, v1x2, v1y2;

  // Calculamos la distancia entre los dos puntos
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
  b = (2 * E * F - D * fA1 * E + sq(D) * fB1) / sq(D);
  c = sq(F) / sq(D) - fA1 * F / D + fC1;

  d = (sq(b) - 4 * a * c);

  if (d >= 0) {
    y1 = (-b + sqrt(d)) / (2 * a);
    x1 = (-E * y1 - F) / D;
    y2 = (-b - sqrt(d)) / (2 * a);
    x2 = (-E * y2 - F) / D;

    if (y1 == y2) {
      lPosX = x1;
      lPosY = y1;
    } else // Debemos escoger la soluci?n correcta
    {
      lPosX = x1;
      lPosY = y1;
    }

    return 1;
  } else {
    // No se actualizan los valores
    return 0;
  }
}

int freeRam() {
  extern int __heap_start, *__brkval;
  int v;
  return (int)&v - (__brkval == 0 ? (int)&__heap_start : (int)__brkval);
}

void PruebaBluetooth() {
  // Si hay datos disponibles en el monitor serie
  while (Serial.available()) {
    // Escribimos los valores en el m?dulo bluetooth HC-06
    Serial1.write(Serial.read());
  }
  // Si hay datos disponibles en el m?dulo bluetooth HC-06
  while (Serial1.available()) {
    // Mostramos los valores en el monitor serie
    Serial.write(Serial1.read());
  }
}

char Comando[10] = "";
HardwareSerial Puerto = Serial1;

void EjecutarComando() {
    if (comando.substring(0, 3) == "mot") {
  	    if (comando.length() != 10)
  	    	return;
        int d = comando.substring(3,6).toInt();
        int i = comando.substring(7,10).toInt();
        Serial.print(d);
        Serial.print(", ");
        Serial.println(i);
        if (d == 999)
        	ControlMotores(0, 0, OP_MOTOR_PARAR);
        else
        	ControlMotores(MIN_AVANCE_MOTOR_DER-d, MIN_AVANCE_MOTOR_IZQ+i, OP_MOTOR_VEL);
    }
    else if (comando.substring(0, 4) == "ava0") {
    	ControlMotores(MIN_AVANCE_MOTOR_DER, MIN_AVANCE_MOTOR_IZQ, OP_MOTOR_VEL);
    }
    else if (comando.substring(0, 3) == "ava") {
    	ControlMotores(AVANCE_MOTOR_DER, AVANCE_MOTOR_IZQ, OP_MOTOR_VEL);
	} else if (comando.substring(0, 3) == "ret") {
		ControlMotores(MIN_RET_MOTOR_DER, MIN_RET_MOTOR_IZQ, OP_MOTOR_VEL);
	} else if (comando.substring(0, 3) == "der") {
		ControlMotores(MOTOR_D_PARADO, MIN_GIRO_MOTOR_IZQ, OP_MOTOR_VEL);
	} else if (comando.substring(0, 3) == "izq") {
		ControlMotores(MIN_GIRO_MOTOR_DER, MOTOR_I_PARADO, OP_MOTOR_VEL);
	} else if (comando.substring(0, 3) == "par") {
		ControlMotores(MOTOR_D_PARADO, MOTOR_I_PARADO, OP_MOTOR_PARAR);
	} else if (comando.substring(0, 5) == "reset") {
		resetSoftware();
	} else if (comando.substring(0, 4) == "pos0") {
		RecuperarPosicion = false;
		contTimeOff_us = 0;
	} else if (comando.substring(0, 4) == "pos1") {
		RecuperarPosicion = true;
		contTimeOff_us = millis();
	}
  else if (comando.substring(0, 3) == "son") {
    if (comando.length() != 4)
      return;
    int Sensor = comando.substring(3).toInt();
    //Los sensores se numeran del 1-8. Si es un 0 se activan todos, sino se activa solo el que se indica
    if (Sensor == 0)
      SensorEmisor = EMISION_CONJUNTA;
    else
      SensorEmisor = Sensor-1;
    bool op = RecuperarDistanciasSonar(Sensor);

    Serial.print("RUN_SEN:");
    Serial.println(Sensor);
  }
  else if (comando.substring(0, 3) == "ori") {
  // Recuperar orientaci�n
	  if (comando.length() != 4)
	     return;
      int Baliza = comando.substring(3).toInt();
	  float orientacion = RecuperarOrientacion(Baliza, KalmanX, 8);

	  Serial1.print("ORI:");
	  Serial1.print(orientacion);
	  Serial1.print(",");
	  Serial1.println(Baliza);

	  Serial.print("RUN_ORI:");
	  Serial.println(orientacion);
  } else if (comando.substring(0, 2) == "md") {
  // Arranque motor derecho
    if (comando.length() != 5)
      return;
    PararMotores = false;
    int Velocidad = comando.substring(2).toInt();
    Motor[DERECHA].write(Velocidad);
  } else if (comando.substring(0, 2) == "mi") {
    if (comando.length() != 5)
      return;
    PararMotores = false;
    int Velocidad = comando.substring(2).toInt();
    Motor[IZQUIERDA].write(Velocidad);
  } else if (comando == "stop") {
    if (comando.length() != 4)
      return;
    ControlMotores(-1, -1, OP_MOTOR_PARAR);
    PararMotores = true;
    delay(100);
    // PararMotores = true;
    contador = TIMER2_OVF;
  } else if (comando == "pause") {
    if (comando.length() != 5)
      return;
    PararMotores = true;
  } else if (comando == "resume") {
    if (comando.length() != 6)
      return;
    contador = TIMER2_OVF;
    PararMotores = false;
  } else if (comando.substring(0, 2) == "kl") {
    if (comando.length() != 10)
      return;
    double k1 = comando.substring(2, 5).toInt() * 1.0 / 1000;
    double k2 = comando.substring(6, 9).toInt() * 1.0 / 1000;

    fk.init(k1, k2);
  } else if (comando.substring(0, 2) == "al") {
    if (comando.length() != 3)
      return;
    algoKalman = comando.substring(2).toInt();
  } else if (comando == "clearodo") {
    Odo[DERECHA].pulsos_totales = Odo[IZQUIERDA].pulsos_totales = 0;
  }
}

void RecuperarComandos() {
  // Si hay datos disponibles en el m?dulo bluetooth HC-06
  while (Serial1.available()) {
    char c[2] = {0, 0};
    c[0] = Serial1.read();
    if (c[0] != '.') {
      comando.concat(c);
    } else {
      Serial.println(comando);
      EjecutarComando();
      comando = "";
    }
  }
}

void ControlMotores(int PotDerecha, int PotIzquierdo, int op)
{
	if (op == OP_MOTOR_PARAR) {
		if (!PararMotores) {
			Motor[IZQUIERDA].write(MOTOR_I_PARADO);
			Motor[DERECHA].write(MOTOR_D_PARADO);
			Motor[IZQUIERDA].detach();
			Motor[DERECHA].detach();
			PararMotores = true;
			TiempoArranque = 0;
			return;
		}
	} else if (PararMotores) {
		Motor[IZQUIERDA].attach(SERVO_IZQ);
		Motor[DERECHA].attach(SERVO_DER);
		Motor[IZQUIERDA].write(MOTOR_I_PARADO);
		Motor[DERECHA].write(MOTOR_D_PARADO);
		PararMotores = false;
	}
	//Serial.println(PotIzquierdo);
	Motor[IZQUIERDA].write(PotIzquierdo);
	Motor[DERECHA].write(PotDerecha);
	//Serial.println(PotDerecha);
	TiempoArranque = millis();
}

void MoverRueda(byte rueda, byte direccion, int velocidad) {
  if (velocidad > 255)
    velocidad = 255;

  if (rueda == DERECHA) {
    if (direccion == ADELANTE) {
    } else {
    }
  } else {
    if (direccion == ADELANTE) {
    } else {
    }
  }
}

void Tono(int PWM, int ms) {
  analogWrite(TONE, PWM);
  delay(ms);
  analogWrite(TONE, 0);
}

void CambioMovimiento(byte Estado) {
  MoverRueda(IZQUIERDA, ATRAS, VELOCIDAD_RAPIDA);
  MoverRueda(DERECHA, ATRAS, VELOCIDAD_RAPIDA);
  delay(MS_FRENADA);
  MoverRueda(IZQUIERDA, ADELANTE, PARADO);
  MoverRueda(DERECHA, ADELANTE, PARADO);

  if (PITIDO_ESTADOS) {
    for (int i = 0; i < Estado; i++) {
      Tono(200, 50);
      delay(200);
      Tono(200, 0);
      delay(200);
    }
    Tono(200, 0);
  } else {
    Tono(200, 50);
  }
  delay(MS_ESPERA);
}

void RecuperarMedidasMultiples() {
  float minimo, medida[2], media, maximo, filtroX, filtroY;
  int medidas, id_baliza_x = 0, id_baliza_y = 0;
  boolean timeout;
  sMedidas MedidaBaliza;

  minimo = 999;
  maximo = 0;
  media = 0;
  medidas = 0;

  // Recuperamos todas las medidas de todas las balizas que hayan contestado
  // Las medidas est?n en el array paquetes con numero_medidas unidades
  int numero_medidas = LeerMedidasSensores(EMISION_CONJUNTA);

  for (int i = 0; i < numero_medidas; i++) {
    bool BalizaNueva = false;
    sMedidas *pMedidasBaliza = ListaMedidasBalizas.search_code(
        paquete[i].id_baliza); // Localizamos la zona de medidas de la baliza

    sMedida Medida;
    Medida.tiempo_local = paquete[i].tiempo_local; // micros() del reloj actual
    Medida.tiempo_vuelo =
        paquete[i]
            .tiempo; // Tiempo de vuelo de la se?al recuperada por la baliza
    Medida.num_sensor =
        paquete[i].num_sensor; // N?mero de transductor de la baliza que ha
                               // captado antes la se?al de US

    if (pMedidasBaliza ==
        NULL) { // No se ha localizado la zona de medidas, es una baliza nueva
      sMedidas MedidasBaliza;
      MedidasBaliza.codigo = MedidasBaliza.id_baliza = paquete[i].id_baliza;
      MedidasBaliza.tiempo_vida =
          millis(); // Inicio de tiempo de vida de la primera detecci?n de una
                    // se?al de esta baliza
      ListaMedidasBalizas.add_head(MedidasBaliza);
      MedidasBaliza.MedidasCorrectas = 0; // Inicializamos las medidas correctas

      pMedidasBaliza = &MedidasBaliza;
      BalizaNueva = true;
    }
    // Ahora comprobamos los m?rgenes de la medida
    // 1. Control de m?rgenes
    pMedidasBaliza->numero_medidas++;
    pMedidasBaliza->tiempo_ultima_medida = millis();
    if ((Medida.tiempo_vuelo < MIN_CONTROL_TIEMPO_VUELO) &&
        (Medida.tiempo_vuelo > MAX_CONTROL_TIEMPO_VUELO)) { // Medida err?nea
      pMedidasBaliza->FallosTotales++;
      pMedidasBaliza->FallosAcumulados++;
    } else // Medida correcta, ajustamos la altura
    {
      // Localizamos la informaci?n de la baliza en la Lista de Balizas para
      // recuperar su altura en eje Z
      long altura_baliza = ListaBalizas.search_code(paquete[i].id_baliza)->Z;
      // Los sensores rel robot, los sensores de la baliza y la recta recorrida
      // por el sonido forma un tri?ngulo rect?ngulo
      float h = Medida.tiempo_vuelo;
      float Altura = abs(altura_baliza - ROBOT_ALTURA_SUELO_SENSORES);
      float distancia_ajustada_altura = pow(h, 2) - pow(Altura, 2);

      if (BalizaNueva) { // Si cumple los m?rgenes, al ser una baliza nueva se
                         // inserta la medida directamente (no tenemos datos que
                         // cruzar)
        pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS, Medida);
        pMedidasBaliza->MedidasCorrectas++;
      } else
      // 2. Control de medias.
      //	 Debemos comprobar la media de variaci?n de las ?ltimas 5
      //medidas
      {
        // Hemos excedido el tiempo de inactividad sin detectar ninguna medida
        // de la baliza
        if (abs(pMedidasBaliza->tiempo_vida -
                pMedidasBaliza->tiempo_ultima_medida) >
            MAX_TIEMPO_INACTIVIDAD) { // Debemos ignorar todas las medidas
                                      // anteriores e inicializar todos los
                                      // contadores
          pMedidasBaliza->tiempo_vida = pMedidasBaliza->tiempo_ultima_medida;
          pMedidasBaliza->Medida.del_all();
          pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS, Medida);
          pMedidasBaliza->numero_ultima_medida =
              ProcesoMedida.numero_total_medidas;
          pMedidasBaliza->Inicializar();
        } else // La medida se encuentra dentro del margen temporal
        {
          int numero_medidas = pMedidasBaliza->Medida.m_num_nodes;

          // Reseteamos los valores si se ha reseteado el contador de micros()
          // porque no se pueden comparar las medidas
          if (numero_medidas)
            if (pMedidasBaliza->Medida.search(0)->tiempo_vuelo >
                Medida.tiempo_local) {
              pMedidasBaliza->Medida.del_all();
              pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS,
                                                      Medida);
              pMedidasBaliza->numero_ultima_medida =
                  ProcesoMedida.numero_total_medidas;
              pMedidasBaliza->Inicializar();
            } else {
              // Solo realizamos el control de medias si tenemos como minimo
              // NUM_MEDIDAS_VALIDAS Obtenemos la pendiente media con respecto al
              // tiempo Obtenemos la media de las medidas almacenadas La
              // pendiente entre la medida media y la actual no debe superar en 3
              // veces la pendiente media
              if (numero_medidas >= NUM_MEDIDAS_VALIDAS) {
                sPaquete Medidas[NUM_MEDIDAS_BALIZAS];
                double Delta[NUM_MEDIDAS_BALIZAS]; // Pendientes de variaci?n de
                                                   // distancia con respecto al
                                                   // tiempo
                int numeros_deltas = 0;
                double Media_delta;
                double Media_tiempo;
                long ultimo_tiempo_vuelo; // Ultimo valor de tiempo de vuelo
                long ultimo_tiempo_local; // Ultimo valor de tiempo de vuelo

                // La medida mas nueva es la primera. invertimos el array
                for (int i = numero_medidas - 1; i; i--) {
                  Media_tiempo += Medidas[i].tiempo =
                      pMedidasBaliza->Medida.search(i)->tiempo_vuelo;
                  Medidas[i].tiempo_local =
                      pMedidasBaliza->Medida.search(i)->tiempo_local;
                }
                for (int i = 1; i < numero_medidas; i++) {
                  double tiempo_entre_medidas =
                      Medidas[i].tiempo_local - Medidas[i - 1].tiempo_local;
                  double diferencia_entre_medidas =
                      abs(Medidas[i].tiempo - Medidas[i - 1].tiempo);
                  if (tiempo_entre_medidas >
                      0) // Si se ha reseteado el contado de micros() ignoramos
                         // la medida
                    Media_delta +=
                        (Delta[numeros_deltas++] =
                             diferencia_entre_medidas / tiempo_entre_medidas);
                }
                Media_delta /= numeros_deltas; // Media de variaci?n ponderada
                Media_tiempo /= numero_medidas;

                if (abs(ultimo_tiempo_vuelo - Media_tiempo) /
                        (Medida.tiempo_local - ultimo_tiempo_local) >
                    Media_delta * MARGEN_MEDIDA_DELTA) {
                  pMedidasBaliza->Medida.add_head_windows(
                      NUM_MEDIDAS_BALIZAS,
                      Medida); // Mantenemos las ?ltimas NUM_MEDIDAS_BALIZAS
                               // medidas
                  pMedidasBaliza->numero_ultima_medida =
                      ProcesoMedida.numero_total_medidas;
                  pMedidasBaliza->MedidasCorrectas++;
                } else
                  pMedidasBaliza->FallosTotales++;
              }
            }
        }
      }
    }
  } // for balizas

  byte BalizasDetectadasUltimaMedida[2] = {0, 0};
  byte numero_balizas = 0;
  // Primero comprobamos si hemos recuperado informaci?n de las dos balizas
  // activas seleccionadas para la medida
  if (ProcesoMedida.num_balizas == 2) {
    for (int i = 0; i < ListaMedidasBalizas.m_num_nodes; i++) {
      if (ListaMedidasBalizas.search(i)->numero_ultima_medida ==
          ProcesoMedida.numero_total_medidas) {
        if ((ProcesoMedida.BalizasSeleccionadas[0] ==
             ListaMedidasBalizas.search(i)->id_baliza)) {
          BalizasDetectadasUltimaMedida[0] =
              ProcesoMedida.BalizasSeleccionadas[0];
          numero_balizas++;
        }
        if ((ProcesoMedida.BalizasSeleccionadas[1] ==
             ListaMedidasBalizas.search(i)->id_baliza)) {
          BalizasDetectadasUltimaMedida[1] =
              ProcesoMedida.BalizasSeleccionadas[0];
          numero_balizas++;
        }
      }
    }
    // Comprobamos si seguimos detectando las mismas balizas
    if (numero_balizas == 2) {
      // Inicializamos el contador de errores acumulados de lectura
      ProcesoMedida.ErroresBalizasSeleccionadas[0] =
          ProcesoMedida.ErroresBalizasSeleccionadas[1] = 0;
      // Procedemos a triangular la posici?n
    } else // No hemos podido localizar las dos balizas
    {
      for (int i = 0; i < 2; i++) {
        if (!BalizasDetectadasUltimaMedida[i]) {
          ProcesoMedida.ErroresBalizasSeleccionadas[i]++;
          if (ProcesoMedida.errores_acumulados >=
              ERRORES_CAMBIO_BALIZA) { // Hemos excedido el m?ximo n?mero de
                                       // errores de la baliza, debemos buscar
                                       // otra
          }
        }
      }
    }
  }

  delay(5); // Retardo entre medidas
}

void intRueda_der() {
  int lecturaAct = digitalRead(RUEDA_DER_ODO);
  ContarPulsoRueda(DERECHA, lecturaAct);
}

void intRueda_izq() {
  int lecturaAct = digitalRead(RUEDA_IZQ_ODO);

  ContarPulsoRueda(IZQUIERDA, lecturaAct);
}
#define TIEMPO_RETARDO_US 10000

enum eEstadoTrans : byte { validada, pendiente, no_activa, espera };

struct sTransicion {
  long tiempo_us;
  char lectura;
  eEstadoTrans estado;
};
#define SIN_TRANSICION -1
#define MIN_TIEMPO_PULSO 6000        // us
#define MIN_TIEMPO_PULSO_VALIDO 7000 // us

/*
        * Si estoy leyendo el valor de pulso X y tengo una se?al <> X
                1. Si no hay activa transici?n, la activo
                2. Si hay activa transici?n, sigue contando el tiempo de
   transici?n
    * Si estoy leyendo el valor de pulso X y tengo una se?al == X
                1. Si no hay activa transici?n, no hago nada sigo leyendo X
                2. Si hay activa una transcion

*/
sTransicion uno[2] = {{0, 0, no_activa}, {0, 0, no_activa}},
            cero[2] = {{0, 0, no_activa}, {0, 0, no_activa}};

#define TIEMPO_PULSO 60000
#define TIEMPO_PULSO_INTERFERENCIA 4000
#define MIN_TIEMPO 1000

void ContarPulsoRueda(int rueda, int lecturaAct) {

  bool ini_pulso = false;
  long tiempo_us = micros();
  long tiempo_ms = millis();

  sTransicion *tmp_uno, *tmp_cero;
  if (lecturaAct == LOW) {
    tmp_uno = &uno[rueda];
    tmp_cero = &cero[rueda];
  } else {
    tmp_uno = &cero[rueda];
    tmp_cero = &uno[rueda];
  }

  if (tmp_cero->estado == no_activa) {
    // Se inicializa el pulso
    tmp_cero->tiempo_us = tiempo_us;
    tmp_cero->estado = pendiente;
  }

  if (tmp_uno->estado == pendiente) {
    ini_pulso = false;
    // Si el largo de uno es suficiente para detectar un pulso se detecta
    long tiempo = tiempo_us - tmp_uno->tiempo_us;
    if (tiempo > TIEMPO_PULSO) {
      Odo[rueda].Contador[lecturaAct].pulsos++;
      Odo[rueda].Contador[lecturaAct].Tiempo =
          tiempo_ms - Odo[rueda].Contador[lecturaAct].Ultimo_ms;
      Odo[rueda].Contador[lecturaAct].Ultimo_ms = tiempo_ms;

      Odo[rueda].pulsos_totales++;
      Odo[rueda].tiempo_total = tiempo_ms - Odo[rueda].Ultimo_ms_total;
      Odo[rueda].Ultimo_ms_total = tiempo_ms;

      ini_pulso = true;
      tmp_uno->estado = no_activa;
    }
    // Si el largo de uno es mas grande que una interferencia
    else if (tiempo < TIEMPO_PULSO_INTERFERENCIA) {
      ini_pulso = false;
      tmp_uno->estado = no_activa;
    }
    // Siempre que no se detecte una interferencia se inicializa el pulso
    // contrario
    if (ini_pulso) {
      // Se inicializa el pulso contrario
      tmp_cero->tiempo_us = tiempo_us;
      tmp_cero->estado = pendiente;
    }
  }
}

// ***************** No empleadas *******************************
void ControlVelocidad(int rueda) {
  static long lastTime = 0;
  long now = millis();
  long timeChange = (now - lastTime);

  long contador;
  long ultimoTimer;
  long Timer;
  Input[rueda] = Odo[rueda].tiempo_total;
  Input[rueda] = klmRev[rueda].getFilteredValue(Input[rueda]);
  if (timeChange >= SAMPLE_TIME) {
    lastTime = now;

    Pid[rueda].Compute();
    // if (rueda == 0)
    //	MotorD.write(Output[rueda]);
    // else
    //	MotorI.write(Output[rueda]);
  }
}

/*****************************************  Funciones de prueba ****************************************************/
/*****************************************  Funciones de prueba ****************************************************/
/*****************************************  Funciones de prueba ****************************************************/

/*
 //Prueba de m�dulos bluetooth
 void loop()
 {
  	long i;
	while(true)
	{
		     if (Serial1.available())
		     {
		            Serial.write(Serial1.read());
		     }

			  if (Serial.available())
			  {
				  char c= Serial.read();
				  Serial1.write(c);
			  }
	}
 }
 */