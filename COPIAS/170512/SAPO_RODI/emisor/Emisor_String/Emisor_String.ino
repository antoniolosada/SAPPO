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

#define TRIGER1      38
#define TRIGER2      39
#define TRIGER3      40
#define TRIGER4      41

#define ECHO1        42
#define ECHO2        43
#define ECHO3        44
#define ECHO4        45

char msg[16]="." ;                             // Array a transmitir


//*************************************************************************************************************
RF24 radio(9,53);                        // Creamos un objeto radio del tipo RF2$ para Arduino Mega
//RF24 radio(8,7);                        // Creamos un objeto radio del tipo RF2$
//*************************************************************************************************************

const uint64_t pipe = 0x00000000E1LL;    // Usamos este canal
const uint64_t broadcast = 0xE8E8F0F0E1LL;    // Usamos este canal

struct sPaquete
{
	byte modo;
	byte operacion;
	union 
	{
		unsigned long direccion;
		unsigned long num_lecturas;
	};
};

sPaquete paquete;
 
void EnviarPulsoUltrasonidos();
 
void setup(void)
{
  pinMode(TRIGER1, OUTPUT);
  pinMode(TRIGER2, OUTPUT);
  pinMode(TRIGER3, OUTPUT);
  pinMode(TRIGER4, OUTPUT); 

  pinMode(ECHO1, INPUT); 
  pinMode(ECHO2, INPUT); 
  pinMode(ECHO3, INPUT); 
  pinMode(ECHO4, INPUT);   
  
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
 
  Serial.begin(9600);
  radio.begin();
  radio.setPALevel(RF24_PA_HIGH);
  radio.openWritingPipe(broadcast);  
  
  radio.setRetries(0,0);
  radio.setAutoAck(false);
  
  paquete.modo = MODE_ACTIVE;
  paquete.operacion = OP_SET_MODE;

  // Initialise the IO and ISR
  vw_setup(4000);      // Bits per sec
  //vw_rx_start();       // Start the receiver PLL running

}          // Abrir para escribir

 int i = 0;
void loop(void)
{
  
  //radio.write(&paquete, sizeof(paquete));
  EnviarMensaje433Mhz();
  EnviarPulsoUltrasonidos();

  delay(300);
}

void EnviarPulsoUltrasonidos()
{
  long tiempo;
  int distanceCm;
  digitalWrite(TRIGER1,LOW); /* Por cuestión de estabilización del sensor*/
  delayMicroseconds(5);
  digitalWrite(TRIGER1, HIGH); /* envío del pulso ultrasónico*/
  delayMicroseconds(10);
  digitalWrite(TRIGER1, LOW); /* envío del pulso ultrasónico*/
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

void EnviarMensaje433Mhz()
{
    const char *msg = "hello";
    uint8_t buf[VW_MAX_MESSAGE_LEN];
    uint8_t buflen = VW_MAX_MESSAGE_LEN;
    digitalWrite(13, true); // Flash a light to show transmitting
    vw_send((uint8_t *)msg, strlen(msg));
    vw_wait_tx(); // Wait until the whole message is gone
}

