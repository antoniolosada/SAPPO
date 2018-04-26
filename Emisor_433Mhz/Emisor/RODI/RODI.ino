/*
  INDICA LA ZONA DE CÓDIGO QUE SE ACTIVARÁ
*/

#define DEBUG_LOCALIZACION
//#define DEBUG_SIGUE_LINEAS

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

#define TONE  8

#define DERECHA        1
#define IZQUIERDA      0

#define ADELANTE       0
#define ATRAS          1

#define VELOCIDAD_RAPIDA         200
#define VELOCIDAD_MENOS_RAPIDA   170
#define PARADO                    50
#define VELOCIDAD_LENTA          150

#define CENTRADO                    0
#define DERECHA                     1
#define IZQUIERDA                   2


#define EST_CENTRADO                1
#define EST_ESPERA_IZQ              2
#define EST_ESPERA_DER              3
#define EST_ESPERA_CENTRADO         4 
#define EST_PARADO                  5  

#define MS_FRENADA 40
#define MS_ESPERA  200
#define PITIDO_ESTADOS  0

long contRueda_der = 0;
long contRueda_izq = 0;
byte Estado = EST_CENTRADO;
byte lado = CENTRADO;


#include <VirtualWire.h>

void setup() {
  // put your setup code here, to run once:
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

#ifndef DEBUG_LOCALIZACION
  attachInterrupt(0, intRueda_der, RISING);
  attachInterrupt(0, intRueda_izq, RISING);

  delay(2000);
  analogWrite(TONE, 200);
  delay(500);
  analogWrite(TONE, 0);
  delay(1000);
#endif



#ifndef DEBUG_LOCALIZACION
   MoverRueda(IZQUIERDA, ADELANTE, VELOCIDAD_RAPIDA);
   MoverRueda(DERECHA, ADELANTE, VELOCIDAD_RAPIDA);
#endif
  
  Serial.begin(9600);


#ifdef DEBUG_LOCALIZACION
  // Initialise the IO and ISR
  vw_setup(2000);      // Bits per sec
  //vw_rx_start();       // Start the receiver PLL running
#endif
}

void intRueda_der()
{
  contRueda_der++;
}
void intRueda_izq()
{
  contRueda_izq++;
}
void Tono(int PWM, int ms)
{
  analogWrite(TONE, PWM);
  delay(ms);
  analogWrite(TONE, 0);
}
void MoverRueda(byte rueda, byte direccion, byte velocidad)
{
  if (rueda == DERECHA)
  {
    if (direccion == ADELANTE)
    {
      digitalWrite(IN3, LOW);
      digitalWrite(IN4, HIGH);
    }   
    else
    {
      digitalWrite(IN3, HIGH);
      digitalWrite(IN4, LOW);
    }
    analogWrite(INB, velocidad);
  }
  else
  {
    if (direccion == ADELANTE)
    {
      digitalWrite(IN2, LOW);
      digitalWrite(IN1, HIGH);
    }   
    else
    {
      digitalWrite(IN2, HIGH);
      digitalWrite(IN1, LOW);
    }
    analogWrite(INA, velocidad);
  }
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
     for(int i = 0; i<Estado; i++)
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

void PararMotores()
{
   MoverRueda(DERECHA, ADELANTE, PARADO);
   MoverRueda(IZQUIERDA, ADELANTE, PARADO);
   Tono(300, EST_PARADO);
}

//****************************************************************   LOOP ************************************************************
//****************************************************************   LOOP ************************************************************
//****************************************************************   LOOP ************************************************************
          
void loop() {
    const char *msg = "hello";
    uint8_t buf[VW_MAX_MESSAGE_LEN];
    uint8_t buflen = VW_MAX_MESSAGE_LEN;
    digitalWrite(13, true); // Flash a light to show transmitting
    vw_send((uint8_t *)msg, strlen(msg));
    vw_wait_tx(); // Wait until the whole message is gone


  
#ifndef DEBUG_LOCALIZACION
  // put your main code here, to run repeatedly: 
  // put your main code here, to run repeatedly: 

  byte CENTRO = !digitalRead(LINEA_centro);
  byte DER = !digitalRead(LINEA_der);
  byte IZQ = !digitalRead(LINEA_izq);
  byte SLADO;
  if (lado == DERECHA)
    SLADO = DER;
  else
    SLADO = IZQ;

  //if (DER && IZQ) PararMotores();
  switch(Estado)
  {
    case EST_ESPERA_CENTRADO:
    {
      if (!DER && !IZQ && CENTRO)
      {
         CambioMovimiento(EST_CENTRADO);
         MoverRueda(DERECHA, ADELANTE, VELOCIDAD_RAPIDA);
         MoverRueda(IZQUIERDA, ADELANTE, VELOCIDAD_RAPIDA);
         Estado = EST_CENTRADO;
      }
      break;
    }
    case EST_CENTRADO:
    {
      if (DER && !IZQ)
      {
         CambioMovimiento(EST_ESPERA_IZQ);
         MoverRueda(DERECHA, ADELANTE, PARADO);
         MoverRueda(IZQUIERDA, ADELANTE, VELOCIDAD_LENTA);
         Estado = EST_ESPERA_IZQ;     
      }
      else if (IZQ && !DER)
      {
         CambioMovimiento(EST_ESPERA_DER);
         MoverRueda(IZQUIERDA, ADELANTE, PARADO);
         MoverRueda(DERECHA, ADELANTE, VELOCIDAD_LENTA);
         Estado = EST_ESPERA_DER;     
      }
      break;
    }
    case EST_ESPERA_DER:
    {
      if (!IZQ && CENTRO)
      {
         CambioMovimiento(EST_CENTRADO);
         MoverRueda(DERECHA, ADELANTE, VELOCIDAD_RAPIDA);
         MoverRueda(IZQUIERDA, ADELANTE, VELOCIDAD_RAPIDA);
         Estado = EST_CENTRADO;
      }
      else if (DER)
      {
         CambioMovimiento(EST_ESPERA_CENTRADO);
         MoverRueda(IZQUIERDA, ADELANTE, VELOCIDAD_MENOS_RAPIDA);
         MoverRueda(DERECHA, ADELANTE, VELOCIDAD_RAPIDA);
        Estado = EST_ESPERA_CENTRADO;
      }
      break;
    }
    case EST_ESPERA_IZQ:
    {
      if (!DER && CENTRO)
      {
         CambioMovimiento(EST_CENTRADO);
         MoverRueda(DERECHA, ADELANTE, VELOCIDAD_RAPIDA);
         MoverRueda(IZQUIERDA, ADELANTE, VELOCIDAD_RAPIDA);
         Estado = EST_CENTRADO;
      }
      else if (IZQ)
      {
        CambioMovimiento(EST_ESPERA_CENTRADO);
        MoverRueda(DERECHA, ADELANTE, VELOCIDAD_RAPIDA);
        MoverRueda(IZQUIERDA, ADELANTE, VELOCIDAD_MENOS_RAPIDA);
        Estado = EST_ESPERA_CENTRADO;
      }
      break;
    }
  }
#endif
}
