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

#define MAX_ULTRASONIDOS   4
#define MAX_DISTANCIAS     20
#define MAX_MICROS_ESPERA  4000 //12m recorridos por el sonido

#define ENVIAR_PULSO       5
#define ALIMENTACION       4 /* alimentación ON -> LOW*/
#define LED                3

#define ENCENDER           LOW
#define APAGAR             HIGH

#define LONGITUD_PAQUETES  sizeof(sPaquete)

#define SET_MODE          1
#define SET_OPERATION     2

#define MODE_HIBERNATE    1
#define MODE_SUSPEND      2
#define MODE_ALERT        3
#define MODE_ACTIVE       4

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
int lectura_pulso[MAX_ULTRASONIDOS] = {2, 6, 9,10};

void setup(void)
{
  Serial.begin(9600);
  
  radio.begin();
  radio.setPALevel(RF24_PA_MIN);
  radio.setAutoAck(1, false);
  radio.setPayloadSize(LONGITUD_PAQUETES);
  
  radio.openReadingPipe(1,broadcast);
  radio.startListening();
  
  pinMode(ENVIAR_PULSO, OUTPUT); 
  for (int i=0; i<MAX_ULTRASONIDOS; i++)
     pinMode(lectura_pulso[i], INPUT);
  pinMode(ALIMENTACION, OUTPUT);
  digitalWrite(ALIMENTACION, LOW); 
}
 
void loop(void)
{
    if (radio.available())
    {
      radio.read(&paquete, LONGITUD_PAQUETE);
      //Comprobamos la instrucción
      switch(paquete.modo)
      {
        case SET_MODE:
        {
          switch(paquete.operacion)
          {
            case MODE_HIBERNATE:
            case MODE_SUSPEND:
            case MODE_ALERT:
            case MODE_ACTIVE:
          }
          
          break;
        }
        case SET_OPERATION:
        {
          switch(paquete.operacion)
          {
            case OP_READ_INIT:
            {
              NumeroLecturas = paquete.num_lecturas;
            }
            case OP_READ_DISTANCE:
            {
                digitalWrite(ENVIAR_PULSO,LOW); /* Por cuestión de estabilización del sensor*/
                delayMicroseconds(5);
                digitalWrite(ENVIAR_PULSO, HIGH); /* envío del pulso ultrasónico*/
                delayMicroseconds(10);
    
                //Esperamos hasta recuperar un pulso alto en alguna entrada
                byte baliza = 0;
                byte lectura = LOW;
                unsigned long tiempo_ini = micros();
                unsigned long tiempo = tiempo_ini;
                while(tiempo - tiempo_ini < MAX_MICROS_ESPERA )
                {
                  lectura = digitalRead(lectura_pulso[baliza]);
                  baliza = (baliza+1)%MAX_ULTRASONIDOS;
                  tiempo = micros();
                  
                  if (lectura == HIGH)
                    break;
                }
                int done = radio.read(msg, 1); 
                medidas[cont++] = tiempo*0.034; 
                
                break;
            }
          }
          break;
        }
      }
    }



  //LowPower.powerDown(SLEEP_8S, ADC_OFF, BOD_OFF);
  cont = 0;
  while(cont < nm)
  {
    if (radio.available())
    {
      digitalWrite(ENVIAR_PULSO,LOW); /* Por cuestión de estabilización del sensor*/
      delayMicroseconds(5);
      digitalWrite(ENVIAR_PULSO, HIGH); /* envío del pulso ultrasónico*/
      delayMicroseconds(10);
      tiempo=pulseIn(6, HIGH); 

      int done = radio.read(msg, 1); 
      medidas[cont++] = tiempo*0.034; 

    //Serial.println(tiempo*0.034);
    }
  }
  float suma = 0;
  for(int i=0; i<nm; i++)
  {
    suma += medidas[i];
  }
    Serial.println(suma/nm);
}

