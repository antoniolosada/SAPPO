/*  ----------------------------------------------------------------
    http://www.prometec.net/nrf2401
    Prog_79_1_Emisor
    
    Programa para transmitir strings mediante radios NRF2401
    1. Configurar los pines del RF24 para arduino uno o mega
--------------------------------------------------------------------  
*/

#include <nRF24L01.h>
#include <RF24.h>
#include <RF24_config.h>
#include <SPI.h>
#include <VirtualWire.h>
#include <Kalman.h>

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

//Identificación de PINES **************************************
#define IN3  28
#define IN4  29
#define INB  10

#define IN1  26
#define IN2  27
#define INA  11

#define LINEA_der      37
#define LINEA_centro   35
#define LINEA_izq      36

#define RUEDA_der     3
#define RUEDA_izq     2

#define PIN_W  5
#define PIN_R  4
//FIN Identificación de PINES **************************************

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

#define ID_OPERACION_MEDIDA  1
#define ID_ROBOT_RODI        1

#define MAX_TIEMPO_VUELO_SONIDO  40 //Tiempo en ms para que el sonido recorra 8m

byte aTrigger[MAX_SENSORES_ULTRASONIDOS] = {TRIGER1, TRIGER2, TRIGER3, TRIGER4, TRIGER5, TRIGER6, TRIGER7, TRIGER8 };
byte aEcho[MAX_SENSORES_ULTRASONIDOS] = {ECHO1, ECHO2, ECHO3, ECHO4, ECHO5, ECHO6, ECHO7, ECHO8 };

char msg[16]="." ;                             // Array a transmitir


//*************************************************************************************************************
RF24 radio(9,53);                        // Creamos un objeto radio del tipo RF2$ para Arduino Mega
//RF24 radio(8,7);                        // Creamos un objeto radio del tipo RF2$
//*************************************************************************************************************

const uint64_t pipe = 0x00000000E1LL;    // Usamos este canal
const uint64_t broadcast = 0xE8E8F0F0E1LL;    // Usamos este canal

struct sPaquete
{
  byte id_baliza;
  long tiempo;
  byte num_sensor;
};

struct sPaqueteRF433
{
  byte id_robot;
  byte operacion;
};

sPaquete paquete;
sPaqueteRF433 paqueteRF433;
 
void EnviarPulsoUltrasonidos();
 
void setup(void)
{
  for (int i=0; i<MAX_SENSORES_ULTRASONIDOS; i++)
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
  
  
  pinMode(IN3, OUTPUT);
  pinMode(IN4, OUTPUT);
  pinMode(INB, OUTPUT);

  pinMode(IN1, OUTPUT);
  pinMode(IN2, OUTPUT);
  pinMode(INA, OUTPUT);
  
  //Ultrasonidos
  pinMode(PIN_W, OUTPUT); /*salida: para el pulso ultrasónico*/
  pinMode(PIN_R, INPUT); /*entrada: tiempo del rebote del ultrasonido*/
 
  Serial.begin(57600);
  radio.begin();
  radio.setPALevel(RF24_PA_HIGH);
  //radio.openWritingPipe(broadcast);  
  radio.openReadingPipe(0,broadcast+10);
  radio.openReadingPipe(1,broadcast);
  radio.openReadingPipe(2,broadcast+1);
  
  radio.setRetries(5,5);
  radio.setAutoAck(true);
  
  // Initialise the IO and ISR
  vw_set_tx_pin(12);
  vw_setup(4000);      // Bits per sec
  //vw_rx_start();       // Start the receiver PLL running

}          // Abrir para escribir, maximo

 int i = 0;
 Kalman myFilter(0.125,32,1023,0); //suggested initial values for high noise filtering
void loop(void)
{
  float minimo, medida, media, maximo, filtro;
  int medidas;
  
  minimo = 999;
  maximo = 0;
  media = 0;
  medidas = 0;

  for (i=0; i<1; i++)
  {
    medida = RecuperarMedida();
    if ((medida > 10) && (medida < 1000))
    {
      media += medida;
      medidas++;
      filtro = myFilter.getFilteredValue(medida);
    }
    //if ((medida < minimo) && (medida > 0)) minimo = medida;
    if ((medida > maximo) && (medida < 1000)) maximo = medida;
    delay(5);
  }
  //medida = myFilter.getFilteredValue(medida);
  
  if ((medida > 10) && (medida < 1000))
  {
      Serial.print(paquete.id_baliza);
      Serial.print(" - ");
      Serial.println(filtro);
  }

}

float RecuperarMedida()
{
  //radio.write(&paquete, sizeof(paquete));
  EnviarMensaje433Mhz(0);
  EnviarPulsoUltrasonidos();

  radio.startListening();

  unsigned long started_waiting_at = millis();
  bool timeout = false;
  
  float medida;
  while ( (!radio.available()) && ! timeout )       // Esperamos
  {
        if (millis() - started_waiting_at > 80 )
        {
            timeout = true;
            //Serial.println("err");
        }
  }

  if ( !timeout )
  {   // Leemos el mensaje recibido
       unsigned long got_time;
       radio.read( &paquete, sizeof(sPaquete));
       medida = 1.0 * paquete.tiempo / 29.2;   //convertimos a distancia, en cm
  }  
  radio.stopListening();
  
  return medida;
}

float RecuperarMedidaMultiple()
{
  EnviarMensaje433Mhz(0);
  radio.startListening();

  //Enviamos tres pulsos de ultrasonidos para recuperar tres medidas con un solo paquete disparador
  EnviarPulsoUltrasonidos();
  delay(MAX_TIEMPO_VUELO_SONIDO);
  EnviarPulsoUltrasonidos();
  delay(MAX_TIEMPO_VUELO_SONIDO);
  //EnviarPulsoUltrasonidos();
  
  unsigned long started_waiting_at = millis();
  bool timeout = false;
  
  float medida;
  while ( !radio.available() && ! timeout )       // Esperamos
  {
        if (millis() - started_waiting_at > MAX_TIEMPO_VUELO_SONIDO )
        {
            timeout = true;
            Serial.println("err");
        }
  }

  if ( !timeout )
  {   // Leemos el mensaje recibido
       unsigned long got_time;
       radio.read( &paquete, sizeof(sPaquete) );
       medida = 1.0 * paquete.tiempo / 29.2;   //convertimos a distancia, en cm
  }  
  radio.stopListening();
  
  return medida;
}

void EnviarPulsoUltrasonidos()
{
  long tiempo;
  int distanceCm;
  for (int i=0; i<MAX_SENSORES_ULTRASONIDOS; i++)
    digitalWrite(aTrigger[i],LOW); /* Por cuestión de estabilización del sensor*/
  delayMicroseconds(5);
  for (int i=0; i<MAX_SENSORES_ULTRASONIDOS; i++)
    digitalWrite(aTrigger[i], HIGH); /* envío del pulso ultrasónico*/
  delayMicroseconds(15);
  for (int i=0; i<MAX_SENSORES_ULTRASONIDOS; i++)
    digitalWrite(aTrigger[i], LOW); /* envío del pulso ultrasónico*/

/*
  tiempo = pulseIn(ECHO1, HIGH)+1;

  distanceCm = tiempo * 10 / 292/ 2;   //convertimos a distancia, en cm
  
  Serial.println(distanceCm);
  */
}

bool Espera(unsigned long ms, int contador, int op)
{
  #define MAX_US				4294967295

	static unsigned long us_ini[10] = {0,0,0,0,0,0,0,0,0,0};
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

    digitalWrite(13, true); // Flash a light to show transmitting

    vw_send((uint8_t *)&paqueteRF433, strlen(msg));
    vw_wait_tx(); // Wait until the whole message is gone
}

/*
int InterseccionCircunferencias(int iD1, int iD2, struct sBaliza BalizaAnt, struct sBaliza BalizaSig, long &lPosX, long &lPosY)
{
  float fA1, fB1, fC1, fA2, fB2, fC2;
  double D, E, F, a, b, c, d, x1, x2, y1, y2;
  double a1, b1, r1, a2, b2, r2;
  long v1x1, v1y1, v1x2, v1y2;
  
  
  //Calculamos la distancia entre los dos puntos
  v1x1 = BalizaAnt.lPosX;
  v1y1 = BalizaAnt.lPosY;
  
  v1x2 = BalizaSig.lPosX;
  v1y2 = BalizaSig.lPosY;

  a1 = v1x1;
  b1 = v1y1;
  a2 = v1x2;
  b2 = v1y2;
  r1 = iD1;
  r2 = iD2;
  
  fA1 = -2*a1;
  fB1 = -2*b1;
  fC1 = sq(a1) + sq(b1) - sq(r1);
  fA2 = -2*a2;
  fB2 = -2*b2;
  fC2 = sq(a2) + sq(b2) - sq(r2);
  
  D = fA1 - fA2;
  E = fB1 - fB2;
  F = fC1 - fC2;
  
  a = sq(E)/sq(D) +1;
  b = (2*E*F - D*fA1*E + sq(D)*fB1)/sq(D);
  c = sq(F)/sq(D) - fA1*F/D + fC1;
  
  d = (sq(b) - 4*a*c);
  
  if (d >= 0)
  {
    y1 = (-b + sqrt(d))/(2*a);
    x1 = (-E*y1 - F)/D;
    y2 = (-b - sqrt(d))/(2*a);
    x2 = (-E*y2 - F)/D;
    
    if (y1 == y2)
    {
      lPosX = x1;
      lPosY = y1;
    }
    else //Debemos escoger la solución correcta
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

*/
