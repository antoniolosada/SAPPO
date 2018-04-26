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

#define ID_BALIZA    0 //********************************************** ID_BALIZA

#ifdef V1
  #define ALIMENTACION       4 

  #define ENCENDER           HIGH
  #define APAGAR             LOW

  #define ENVIAR_PULSO       19
  #define MAX_ULTRASONIDOS   4
  #define PIN_RX_RF433Mhz    9
  int lectura_pulso[MAX_ULTRASONIDOS] = {18, 16, 15, 14};
#endif



#ifdef V2
  #define ALIMENTACION       4 

  #define ENCENDER           HIGH
  #define APAGAR             LOW

  #define ENVIAR_PULSO       3
  #define MAX_ULTRASONIDOS   4
  #define PIN_RX_RF433Mhz    9
  int lectura_pulso[MAX_ULTRASONIDOS] = {19, 18, 17, 16};
#endif

#define MAX_DISTANCIAS  5

#define MAX_TIEMPO_LECTURA_SENSORES  300 //Tiempo de espera desde la recepción del primer transductor para detectar la lectura en los demás
#define MAX_TIEMPO_ECO               30000 //uSegundos

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

#define ID_OPERACION_MEDIDA  1
#define ID_ROBOT_RODI        1

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

int NumeroLecturas = 0;
float medidas[MAX_DISTANCIAS];
int lectura_actual = 0;
int numero_lecturas = 0;

RF24 radio(8,7);
const uint64_t broadcast = 0xE8E8F0F0E1LL+ID_BALIZA;

struct sLectura
{
  byte lectura_detectada;
  int sensor;
  long tiempo;
  long TiempoSensor[MAX_ULTRASONIDOS];
} aLecturas[MAX_DISTANCIAS];

bool Espera(unsigned long ms, int contador, int op);
int RecuperarMensajeRF_433Mhz(int salida);
void LeerSensorUltrasonidos(int sensor);
void LeerSensoresUltrasonidos();
void LeerSensoresUltrasonidosMedidaMultiple();

void setup(void)
{
  Serial.begin(57600);

  radio.begin();
  radio.openReadingPipe(0,broadcast);
  radio.startListening();
  radio.stopListening();

  radio.setPALevel(RF24_PA_HIGH);
  radio.setAutoAck(true);

  radio.openWritingPipe(broadcast);
  
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
  vw_set_rx_pin(PIN_RX_RF433Mhz);
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
      LeerSensoresUltrasonidos();
      //LeerSensoresUltrasonidosMedidaMultiple();    




//    InicializarLecturas();
    //Espera por paquete con número de lectura
    //Recuperar información de la lectura
}

void LeerSensorUltrasonidos(int sensor)
{
  digitalWrite(ENVIAR_PULSO,LOW); 
  delayMicroseconds(5);
  digitalWrite(ENVIAR_PULSO, HIGH); 
  delayMicroseconds(10);
  digitalWrite(ENVIAR_PULSO, LOW); 
  //ESpera a que la señal de entrada pase a LOW
  delayMicroseconds(10);

  tiempo = pulseIn(sensor, HIGH)+1;

  paquete.num_sensor = 0;
  paquete.tiempo = tiempo;
  bool ok = radio.write( &paquete, sizeof(sPaquete) );

  distanceCm = 1.0 * tiempo / 29.2;   //convertimos a distancia, en cm
  Serial.print(distanceCm);
  if (ok)
       Serial.println("        ok...");
  else
       Serial.println("        failed");
}

void LeerSensoresUltrasonidos()
{
  long tiempo_ini = micros();  
  digitalWrite(ENVIAR_PULSO,LOW); 
  delayMicroseconds(5);
  digitalWrite(ENVIAR_PULSO, HIGH); 
  delayMicroseconds(10);
  digitalWrite(ENVIAR_PULSO, LOW); 
  //ESpera a que la señal de entrada pase a LOW
  delayMicroseconds(10);

  while(!digitalRead(lectura_pulso[3]));

  paquete.id_baliza = ID_BALIZA;

  long tiempo_espera = MAX_TIEMPO_ECO;
  //Inicializamos el contador de tiempo
  Espera(tiempo_espera, 0, OP_INICIAR);

  bool lectura_recuperada = 0;
  bool ok = 0;
  while((Espera(tiempo_espera, 0, OP_CONSULTAR)) && (!lectura_recuperada)) //Dentro del tiempo máximo de eco intentamos captar la lectura de todos los sensores
  {
      for(int i=0; i<MAX_ULTRASONIDOS; i++)
      {
        if (digitalRead(lectura_pulso[i]) == LOW) //Hemos detectado el tren de pulsos ultrasónicos en el sensor i
        {
          lectura_recuperada = true;
          paquete.tiempo = micros()-tiempo_ini;
          paquete.num_sensor = i;
          ok = radio.write( &paquete, sizeof(sPaquete) );
          break;
        }
      }
  }
  
  //distanceCm = 1.0 * tiempo / 29.2;   //convertimos a distancia, en cm
  Serial.println(paquete.tiempo);
  if (ok)
       Serial.println("         ok");
  else
       Serial.println("        failed");
}

void LeerSensoresUltrasonidosMedidaMultiple()
{

  paquete.id_baliza = ID_BALIZA;

  long tiempo_espera = MAX_TIEMPO_ECO;
  //Inicializamos el contador de tiempo
  Espera(tiempo_espera*3, 1, OP_INICIAR);

  bool lectura_recuperada = 0;
  bool ok = 0;
  int numero_lectura = 0;
  long tiempo[3];
  int sensor[3];
  
  long tiempo_ini = micros();  
  //Continuamos mientras no hayamos recuperados las 3 medidas o excedamos el tiempo total calculado para las tres
  while ((numero_lectura < 3) && (Espera(tiempo_espera*3, 1, OP_CONSULTAR)))
  {
    digitalWrite(ENVIAR_PULSO, HIGH); 
    delayMicroseconds(15);
    digitalWrite(ENVIAR_PULSO, LOW); 
    //ESpera a que la señal de entrada pase a LOW
    while(digitalRead(lectura_pulso[0]) != LOW);
    while(digitalRead(lectura_pulso[1]) != LOW);
    while(digitalRead(lectura_pulso[2]) != LOW);
    while(digitalRead(lectura_pulso[3]) != LOW);
    delayMicroseconds(50);

    lectura_recuperada = false;
    Espera(tiempo_espera, 0, OP_INICIAR);
    while((Espera(tiempo_espera, 0, OP_CONSULTAR)) && (!lectura_recuperada)) //Dentro del tiempo máximo de eco intentamos captar la lectura de todos los sensores
    {
        for(int i=0; i<MAX_ULTRASONIDOS; i++)
        {
          if (digitalRead(lectura_pulso[i]) == LOW) //Hemos detectado el tren de pulsos ultrasónicos en el sensor i
          {
            lectura_recuperada = true;
            tiempo[numero_lectura] = micros()-tiempo_ini;
            sensor[numero_lectura] = i;
            break;
          }
        }
    }
    if (lectura_recuperada)
    {
      numero_lectura++;
    } 
  }

  if (numero_lectura > 0)
  {
    long media = 0;
      int i;
      for (i=0; i<numero_lectura; i++)
        media += tiempo[i];
      media /= numero_lectura;
      
      paquete.tiempo = media;
      paquete.num_sensor = i;
      ok = radio.write( &paquete, sizeof(sPaquete) );

      //distanceCm = 1.0 * tiempo / 29.2;   //convertimos a distancia, en cm
      
      Serial.print(tiempo[2]-tiempo[1]);
      Serial.print(",");
      Serial.print(tiempo[1]-tiempo[0]);
      Serial.print(",");
      Serial.print(media);
      Serial.print("-");
      Serial.print(numero_lectura);
      if (ok)
           Serial.println("         ok");
      else
           Serial.println("        failed");
           
  }
}

/*
  Siempre estamos a la espera de leer un paquete
  El paquete contiene información sobre el código del robot emisor y el número de lecturas que faltan
  Si estábamos en el medio de una operación de distancia múltiple continuamos
  Si es la primera distancia de la operación de distancia múltiple, incializamos los arrays de lecturas
  De cada lectura recuperamos la información del código del robot que lo pide, los sensores que han detectado los US y su tiempo.
*/
void RecuperarOperacionLecturas()
{
  
  long tiempo_espera = MAX_TIEMPO_ECO;
  int numero_lecturas = 0;
  int lectura_actual = 0;
  
    for (int l=0; l<MAX_DISTANCIAS; l++)
    {
      aLecturas[l].lectura_detectada = false;
      aLecturas[l].sensor = 0;
      aLecturas[l].tiempo = 0;
      for (int j=0; j<MAX_ULTRASONIDOS; j++)
      {
        aLecturas[l].TiempoSensor[j] = 0;
      }
    }
}
void RecuperarDistancia(int l)
{
//************************************************
    long tiempo_espera;
    
    // Esperamos por la recepción del paqueteq
    while (!(lectura_actual = RecuperarMensajeRF_433Mhz(0)));
    
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
    delayMicroseconds(50);
    
    tiempo_espera = MAX_TIEMPO_ECO;
    //Inicializamos el contador de tiempo
    Espera(tiempo_espera, 0, OP_INICIAR);
    tiempo = 0;
    tiempo_ini = micros();
    boolean lectura_sensor[MAX_ULTRASONIDOS];
    //Se indica la falta de lectura de todos los sensores            
    for (int i = 0; i<MAX_ULTRASONIDOS; i++)
      lectura_sensor[i] = false;
                  
    while(Espera(tiempo_espera, 0, OP_CONSULTAR)) //Dentro del tiempo máximo de eco intentamos captar la lectura de todos los sensores
    {
        for(int i=0; i<MAX_ULTRASONIDOS; i++)
        {
          if (digitalRead(lectura_pulso[i]) == LOW) //Hemos detectado el tren de pulsos ultrasónicos en el sensor i
          {
            tiempo = micros();
            if (!aLecturas[l].lectura_detectada) //Es la primera detección del pulso en esta lectura
            {
              numero_lecturas++;
              aLecturas[l].lectura_detectada = true;
              aLecturas[l].sensor = i;
              aLecturas[l].tiempo = tiempo - tiempo_ini;
              //Una vez detectada la primera lectura reducimos el tiempo para detectar los demás
              Espera(MAX_TIEMPO_LECTURA_SENSORES, 0, OP_INICIAR);
              tiempo_espera = MAX_TIEMPO_LECTURA_SENSORES;
            }
            aLecturas[l].TiempoSensor[i] = aLecturas[l].tiempo; //Guardamos la información de todas las lecturas que entran en tiempo
          }
        }
    }
}

bool Espera(unsigned long us, int contador, int op)
{
#define MAX_US				4294967295

	static unsigned long us_ini[10] = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	unsigned long us_actual;

	us_actual = micros();

	if (op == OP_INICIAR)
		us_ini[contador] = micros();
	else
	{
		if (us_actual < us_ini[contador])
                	us_actual = MAX_US - us_ini[contador] + us_actual;

		if (us_actual - us_ini[contador] >= us)
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

//Devulve el número de lectura
int RecuperarMensajeRF_433Mhz(int salida)
{
    uint8_t buf[VW_MAX_MESSAGE_LEN];
    if (vw_get_message(buf, (uint8_t *)&paqueteRF433)) // Non-blocking
    {
        int i;

        // El mensaje informa del número de lectura
        if (salida) 
        {
            Serial.print("Lectura: ");
            Serial.println(paqueteRF433.id_robot);
        }
        return 1;
    }
    return 0;
}
