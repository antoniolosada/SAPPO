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
 
#define PIN_W  5
#define PIN_R  4
 
void setup(void)
{
  pinMode(PIN_W, OUTPUT); /*salida: para el pulso ultrasónico*/
  pinMode(PIN_R, INPUT); /*entrada: tiempo del rebote del ultrasonido*/
 
  Serial.begin(115200);
  radio.begin();
  radio.setPALevel(RF24_PA_HIGH);
  radio.openWritingPipe(broadcast);  
  
  radio.setRetries(0,0);
  radio.setAutoAck(false);
  
  paquete.modo = MODE_ACTIVE;
  paquete.operacion = OP_SET_MODE;
}          // Abrir para escribir
 
 int i = 0;
void loop(void)
{
  long tiempo;
  int distanceCm;
  
  radio.write(&paquete, sizeof(paquete));
  digitalWrite(PIN_W,LOW); /* Por cuestión de estabilización del sensor*/
  delayMicroseconds(5);
  digitalWrite(PIN_W, HIGH); /* envío del pulso ultrasónico*/
  delayMicroseconds(10);
  digitalWrite(PIN_W, LOW); /* envío del pulso ultrasónico*/

  /*
  tiempo = pulseIn(PIN_R, HIGH)+1;

  distanceCm = tiempo * 10 / 292/ 2;   //convertimos a distancia, en cm
  
  */
  Serial.println(distanceCm);
  delay(30);
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
