/*  ----------------------------------------------------------------
    http://www.prometec.net/nrf2401
    Prog_79_1_Emisor
    
    Programa para transmitir strings mediante radios NRF2401
--------------------------------------------------------------------  
*/

#include <nRF24L01.h>
#include <RF24.h>
#include <RF24_config.h>
#include <SPI.h>

char msg[16]="." ;                             // Array a transmitir
RF24 radio(9,53);                        // Creamos un objeto radio del tipo RF2$
const uint64_t pipe = 0x00000000E1LL;    // Usamos este canal
const uint64_t broadcast = 0x000000000BLL;    // Usamos este canal

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
 
void setup(void)
{
  Serial.begin(9600);
  radio.begin();
  
  pinMode(7, OUTPUT); /*salida: para el pulso ultrasónico*/
  pinMode(6, INPUT); /*entrada: tiempo del rebote del ultrasonido*/
 
  paquete.modo = MODE_ACTIVE;
  paquete.operacion = OP_SET_MODE;
}          // Abrir para escribir
 
void loop(void)
{
  //Activamos las balizas
  paquete.modo = MODE_ACTIVE;
  paquete.operacion = OP_SET_MODE;
  radio.openWritingPipe(broadcast);
  radio.setAutoAck(false);
  radio.setRetries(0,0);
  //Realizamos reintentos durante 2,5 segundos
  Espera(2500, 0, OP_INICIAR);
  while(!Espera(2500, 0, OP_CONSULTAR))
  {
    radio.write(&paquete, sizeof(paquete));
    delayMicroseconds(1000);
  }
  
  //Solicitamos el inicio de la medición con 5 medidas
  //Enviamos las peticiones de medida
  //Recuperamos la media de las medidas
  
  
  digitalWrite(7,LOW); /* Por cuestión de estabilización del sensor*/
  delayMicroseconds(5);
  digitalWrite(7, HIGH); /* envío del pulso ultrasónico*/
  delayMicroseconds(10);
  radio.write(msg, 1);
  delay(50);
}


bool Espera(unsigned long ms, int contador, int op)
{
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
