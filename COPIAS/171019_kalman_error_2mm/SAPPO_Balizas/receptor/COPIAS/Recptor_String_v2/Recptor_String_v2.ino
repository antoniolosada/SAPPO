/*  ----------------------------------------------------------------
    http://www.prometec.net/nrf2401
    Prog_79_1_Emisor
    
    Programa para recibir strings mediante radios NRF2401
--------------------------------------------------------------------  
*/

#include <nRF24L01.h>
#include <RF24.h>
#include <RF24_config.h>
#include <SPI.h>
#include "LowPower.h"

#include <VirtualWire.h>

//#define V1  //Versión con batería
#define V2    //Versión con alimentador de 5V

#ifdef V1
  #define ALIMENTACION       4 

  #define ENCENDER           LOW
  #define APAGAR             HIGH

  #define ENVIAR_PULSO       19
  #define MAX_ULTRASONIDOS   4
  int lectura_pulso[MAX_ULTRASONIDOS] = {18, 16, 15, 14};
#endif
#ifdef V2
  #define ALIMENTACION       4 

  #define ENCENDER           HIGH
  #define APAGAR             LOW

  #define ENVIAR_PULSO       3
  #define MAX_ULTRASONIDOS   4
  int lectura_pulso[MAX_ULTRASONIDOS] = {19, 18, 17, 16};
#endif

#define MAX_DISTANCIAS  5

#define MAX_TIEMPO_LECTURA_SENSORES  300 //Tiempo de espera desde la recepción del primer transductor para detectar la lectura en los demás
#define MAX_TIEMPO_ECO               26000 //uSegundos

#define LONGITUD_PAQUETES  sizeof(sPaquete)

#define SET_MODE          1
#define SET_OPERATION     2

#define MODE_HIBERNATE    1
#define MODE_SUSPEND      2
#define MODE_ALERT        3
#define MODE_ACTIVE       4

#define OP_INICIAR		1
#define OP_CONSULTAR		2

#define PIN_READ_RF      5

struct sPaquete
{
  byte modo;
  byte operacion;
  union uPaquete
  {
    unsigned long direccion;
    unsigned long num_lecturas;
  };
};

sPaquete paquete;
int NumeroLecturas = 0;
float medidas[MAX_DISTANCIAS];

RF24 radio(8,7);
const uint64_t broadcast = 0xE8E8F0F0E1LL;

struct sPrimeraLectura
{
  int sensor;
  int contador;
};


//Espacio para 5 lecturas de los 4 sensores
unsigned long lecturas[MAX_DISTANCIAS][MAX_ULTRASONIDOS];
int numero_lecturas[MAX_ULTRASONIDOS];
sPrimeraLectura primera_lectura[MAX_ULTRASONIDOS];


bool Espera(unsigned long ms, int contador, int op);
int RecuperarMensajeRF_433Mhz(int salida);
void LeerSensorUltrasonidos(int sensor);

void setup(void)
{
  Serial.begin(9600);

  radio.begin();
  radio.openReadingPipe(1,broadcast);
  radio.startListening();

  radio.setPALevel(RF24_PA_HIGH);
  radio.setAutoAck(false);
/*
  radio.setPALevel(RF24_PA_HIGH);
  radio.setAutoAck(1, false);
  radio.setPayloadSize(LONGITUD_PAQUETES);
*/  

  pinMode(ENVIAR_PULSO, OUTPUT); 
  for (int i=0; i<MAX_ULTRASONIDOS; i++)
     pinMode(lectura_pulso[i], INPUT);

  pinMode(ALIMENTACION, OUTPUT);
  digitalWrite(ALIMENTACION, ENCENDER); 
  delay(500);

//  pinMode(PIN_READ_RF, INPUT);  
  // Initialise the IO and ISR
  vw_set_rx_pin(9);
  vw_set_ptt_inverted(true); // Required for DR3100
  vw_setup(4000);      // Bits per sec
  vw_rx_start();       // Start the receiver PLL running
  delay(50);

}

long tiempo;
float distanceCm;

void loop(void)
{
  if (RecuperarMensajeRF_433Mhz(0))
    LeerSensorUltrasonidos(lectura_pulso[0]);
}

void LeerSensorUltrasonidos(int sensor)
{
  digitalWrite(ENVIAR_PULSO,LOW); 
  delayMicroseconds(5);
  digitalWrite(ENVIAR_PULSO, HIGH); 
  delayMicroseconds(10);
  digitalWrite(ENVIAR_PULSO, LOW); 
  //ESpera a que la señal de entrada pase a LOW
  delayMicroseconds(50);
  tiempo = pulseIn(sensor, HIGH)+1;

  distanceCm = 1.0 * tiempo / 29.2;   //convertimos a distancia, en cm
  Serial.println(distanceCm);
}

void RecuperarDistancias()
{
//************************************************
/*
  long tiempo_espera = MAX_TIEMPO_ECO;
  boolean lectura_detectada = false;
  
  for (int j=0; j<MAX_ULTRASONIDOS; j++)
  {
    numero_lecturas[j] = 0;
    for (int i=0; i<MAX_DISTANCIAS; i++)
      lecturas[i][j] = 0;
  }
  
  //Recuperamos 5 lecturas
  for (int l = 0; l < MAX_DISTANCIAS; l++)
  {
      while (!radio.available());

      {
                  radio.read(&paquete, sizeof(paquete));
                  //Esperamos hasta recuperar un pulso alto en alguna entrada
                  byte baliza = 0;
                  byte lectura = LOW;
                  unsigned long tiempo_ini = micros();
                  unsigned long tiempo = tiempo_ini;
  
                  digitalWrite(ENVIAR_PULSO,LOW); 
                  delayMicroseconds(5);
                  digitalWrite(ENVIAR_PULSO, HIGH); 
                  delayMicroseconds(10);
                  digitalWrite(ENVIAR_PULSO, LOW); 
                  //ESpera a que la señal de entrada pase a LOW
                  delayMicroseconds(2000);
      
                  tiempo_espera = MAX_TIEMPO_ECO;
                  lectura_detectada = false;
                  
                  Espera(tiempo_espera, 0, OP_INICIAR);
                  tiempo = 0;
                  tiempo_ini = micros();
                  boolean lectura_sensor[MAX_ULTRASONIDOS];
                  
                  for (int i = 0; i<MAX_ULTRASONIDOS; i++)
                    lectura_sensor[i] = false;
                    
                  while(Espera(tiempo_espera, 0, OP_CONSULTAR))
                  {
                    for(int i=0; i<MAX_ULTRASONIDOS; i++)
                    {
                      if (digitalRead(lectura_pulso[i]) == LOW)
                      {
                        if (!lectura_detectada)
                        {
                            Espera(MAX_TIEMPO_LECTURA_SENSORES, 0, OP_INICIAR);
                            tiempo_espera = MAX_TIEMPO_LECTURA_SENSORES;
                            lectura_detectada = true;
                        }
                        if (!lectura_sensor[i])
                        {
                          lectura_sensor[i] = true;
                          tiempo = micros();
                          lecturas[l][i] = tiempo - tiempo_ini;
                          numero_lecturas[i]++;
                          //Serial.println(lectura_pulso[i]);
                        }
                      }
                    }
                  }
                  
                  //Esperamos hasta recuperar un pulso alto en alguna entrada
                  //tiempo = pulseIn(ECO_PULSO, HIGH);
  
                  //Serial.println(tiempo*0.034); 
      }
  }
    float media = 0;
    //Buscamos un sensor con todas las lecturas
    for (int i=0; i<MAX_ULTRASONIDOS; i++)
    {
      if (numero_lecturas[i] == MAX_DISTANCIAS)
      {
        int sensor = i;
        //Realizamos la media de las lecturas del sensor
        for (int j=0; j<MAX_DISTANCIAS; j++)
        {
          media += lecturas[j][sensor];
        }
        media = (media*0.034) / MAX_DISTANCIAS;
        Serial.print(media); 
        Serial.print("   - "); 
        Serial.println(i); 
        
        int distancias[MAX_DISTANCIAS];
        //Comprobamos las distancias acumuladas de cada valor con los demás
        for (int i=0; i<MAX_DISTANCIAS; i++)
        {
          distancias[i] = 0;
          for (int j=0; j<MAX_DISTANCIAS; j++)
          {
            if (j!=i)
            {
              distancias[i] += abs(lecturas[j][sensor] - lecturas[i][sensor]);
            }
          }
        }
        if (media > 148)
        {
          for (int j=0; j<MAX_DISTANCIAS; j++)
          {
            Serial.println(lecturas[j][sensor]*0.034);
          }
        }
        break;

      }
    }
*/  
}

bool Espera(unsigned long us, int contador, int op)
{
#define MAX_US				4294967295

	static unsigned long us_ini[10] = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
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

		if (us_actual[contador] - us_ini[contador] >= us)
		{
			us_ini[contador] = 0;
			return false;
		}
		else
			return true;
	}
}

void ordenar_vector(unsigned long vNumeros[MAX_ULTRASONIDOS])
{
  unsigned long temp;
 /* Ordenamos los números del vector vNumeros por el método de burbuja */
  for (int i = 0; i < (MAX_ULTRASONIDOS - 1); i++) 
  { 
    for (int j = i + 1; j < MAX_ULTRASONIDOS; j++) 
    { 
      if (vNumeros[j] < vNumeros[i]) 
      { 
        temp = vNumeros[j]; 
        vNumeros[j] = vNumeros[i]; 
        vNumeros[i] = temp; 
      } 
    } 
  } 
}

int RecuperarMensajeRF_433Mhz(int salida)
{
    uint8_t buf[VW_MAX_MESSAGE_LEN];
    uint8_t buflen = VW_MAX_MESSAGE_LEN;
    if (vw_get_message(buf, &buflen)) // Non-blocking
    {
        int i;

        // Message with a good checksum received, dump it.
        if (salida) 
        {
            Serial.print("Got: ");
            
            for (i = 0; i < buflen; i++)
            {
                Serial.print(buf[i], HEX);
                Serial.print(" ");
            }
            Serial.println("");
        }
        return 1;
    }
    return 0;
}
