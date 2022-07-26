#include "Arduino.h"
//The setup function is called once at startup of the sketch
#include <VirtualWire.h>
#include <eRCaGuy_Timer2_Counter.h>

#define PIN_TX  9
#define PIN_RX  3
#define PIN_EN_TX 10

#define PIN_COM_RX 4

#define PIN_TRIGGER_RX  7
#define PIN_ECHO_RX     6

#define PIN_TRIGGER_TX  5

void ConfigurarTimer()
{
    // Configura��o do TIMER1
  TCCR1A = 0;                // El registro de control A queda todo en 0
  TCCR1B = 0;                //limpia registrador
  TCNT1  = 0;                //Inicializa el temporizador
  OCR1A = 0xFFFF;            // carga el registrador de comparaci�n: 16MHz/1024/1Hz -1 = 15624 = 0X3D08
  //TCCR1B |= (1 << WGM12)|(1<<CS10)|(1 << CS12);   // modo CTC, prescaler de 1024: CS12 = 1 e CS10 = 1
  TCCR1B |= (1 << WGM12)|(1<<CS10);   // modo CTC, prescaler de 1024: CS12 = 1 e CS10 = 1
  //TIMSK1 |= (1 << OCIE1A);  // habilita interrupci�n por igualdade de comparaci�n
}

/**************************************   SEPTUP   **************************************/
void setup() {
  // put your setup code here, to run once:
	pinMode(PIN_RX, INPUT_PULLUP);
  //pinMode(PIN_RX, INPUT);
  pinMode(PIN_TX, OUTPUT);
  //digitalWrite(PIN_TX, HIGH);

  pinMode(PIN_ECHO_RX, INPUT);
  pinMode(PIN_TRIGGER_RX, OUTPUT);
  pinMode(PIN_TRIGGER_TX, OUTPUT);
  pinMode(PIN_EN_TX, OUTPUT);

  pinMode(PIN_COM_RX, INPUT);

  digitalWrite(PIN_EN_TX, LOW);

//Configuraci�n de radio
  vw_set_rx_pin(PIN_RX);
  vw_set_tx_pin(PIN_TX);
  vw_set_ptt_inverted(true); // Required for DR3100
  vw_setup(5000);      // Bits per sec
  vw_rx_start();       // Start the receiver PLL running
  delay(50);

  Serial.begin(9600);
  //ConfigurarTimer();

  timer2.setup();
  attachInterrupt(digitalPinToInterrupt(PIN_RX), blink, RISING );
}

/********************************** VARIABLES ***************************************/

char *cad = "hola";
char cad1[20];
uint8_t buf[200];
int pin;
int valor  = HIGH;

int cont = 0;
int cont_igual = 0;
int cont_diferente = 0;

int cont_igual0 = 0;

int cont_igual1 = 0;

int espera_us = 1;
int espera_us_fin = 48;

//x16 en precisi�n
//int espera_us = 670;
//int espera_us_fin = 750;

int iteraciones_us = 200;

int repeticiones = 100;
long tiempo = 0;

unsigned int medidas[101];
float distancia[101];
int cont_distancia = 0;

//float medidas[101];

int pin1 = 0;


/**************************************   INT   **************************************/
void blink()
{
	cli();
	if (cont <= repeticiones)
	{
	    //medidas[cont] = micros();
		medidas[cont] = timer2.get_count();
	    cont++;
	}
	sei();
}

/**************************************   INT   **************************************/
/*
int cont_sync = 0;
int bloqueo=0;
int tiempo_pulso = 0;
int cont_pulso_corto = 0;
int cont_pulso_largo = 0;
void blink()
{
	if (bloqueo) return;

	if (cont <= repeticiones)
	{
	    medidas[cont] = micros();
	    cont++;

	    if (cont >= 2)
	    {
	    	tiempo_pulso = (medidas[cont-1]-medidas[cont-2]);
	    	if ((tiempo_pulso > 75) && (tiempo_pulso < 98))
	    		cont_pulso_corto++;
    		else if ((tiempo_pulso > 160) && (tiempo_pulso < 190))
			{
    			//Si hemos detectado antes 4 pulsos cortos marcamos una marca de sincronismo
    			if (cont_pulso_corto == 4) cont_sync++;
    			//Si detectamos pulso largo inicializamos el seguimiento del patr�n de lanzamiento
	    		cont_pulso_corto = 0;
    		}
	    }
	    if (cont_sync == 4)
	    {
	    	Serial.println(".");
	    	bloqueo=1;
	        digitalWrite(PIN_TRIGGER_RX, LOW);
	        delayMicroseconds(2);
	        digitalWrite(PIN_TRIGGER_RX, HIGH);
	        delayMicroseconds(10);
	        digitalWrite(PIN_TRIGGER_RX, LOW);
	        delayMicroseconds(5);

	        tiempo = pulseIn(PIN_ECHO_RX, HIGH);

	        //Serial.println(tiempo);
	        if (cont_distancia <= 100)
	        	distancia[cont_distancia++] = tiempo*0.34;

	        cont_sync = 0;
	    	bloqueo=0;
	    }
	}
}
*/
/*********************************** MEdida de distancia SYNC PIN *********************************************/
void loop()
{
  static unsigned long t_start = timer2.get_count(); //units of 0.5us; the count accumulated by Timer2_Counter
  pin = 0;
  repeticiones = 100;
  cont=0;
  //cad1 = "sssssss";

  medidas[0] = micros();
  int modo = 0;
  if (modo == 0)
  {
	  //Sincronizaci�n por cambio de pin
	   while(cont < repeticiones) Serial.print("");

	   int cont1 = 0;
	   cont1=0;
	   while(cont1 < repeticiones)
	   {
	     Serial.print((medidas[cont1]-medidas[cont1-1]));
	     Serial.print(",");
	     if (!(cont1%60)) Serial.println("");
	     cont1++;
	   }
	   cont = 0;
  }
  else
  {
	  while(cont_distancia < repeticiones) Serial.print("");
	  int cont1 = 0;
	  cont1=0;
	  while(cont1 < repeticiones)
	  {
	    Serial.print(distancia[cont1]);
	    Serial.print(",");
	    if (!(cont1%60)) Serial.println("");
	    cont1++;
	  }
	  cont_distancia = 0;
  }


}
/*********************************** MEdida de distancia SYNC PIN *********************************************/



//******************Medidas de distancias SYNC paquete ***********************************************************************************
void loop7()
{
  cont = 0;
  while(cont < repeticiones)
  {
    //Buf -> datos, cad1 -> n� bytes
    while (!vw_get_message(buf, (uint8_t *)cad1));

    digitalWrite(PIN_TRIGGER_RX, LOW);
    delayMicroseconds(2);
    digitalWrite(PIN_TRIGGER_RX, HIGH);
    delayMicroseconds(10);
    digitalWrite(PIN_TRIGGER_RX, LOW);
    delayMicroseconds(5);

    tiempo = pulseIn(PIN_ECHO_RX, HIGH);

    //Serial.println(tiempo);
    medidas[cont++] = tiempo*0.34;
  }

  cont =0;
  while(cont < repeticiones)
  {
    Serial.print(medidas[cont++]);
    Serial.print(",");
    if (!(cont%20)) Serial.println("");
  }

  return;
}
//******************Medidas de distancias SYNC paquete ***********************************************************************************


//VArias SYNC
void loop6()
{
  static unsigned long t_start = timer2.get_count(); //units of 0.5us; the count accumulated by Timer2_Counter
  pin = 0;
  repeticiones = 100;
  cont=0;
  medidas[0] = timer2.get_count(); //micros();



  *cad1 = "sssssss";
  /****  Recuperar se�al de sincronizaci�n ****/
  while(cont < repeticiones)
  {
    //Buf -> datos, cad1 -> n� bytes
    while (!vw_get_message(buf, (uint8_t *)cad1));
    cont++;
    medidas[cont] =micros();
  }

/*
 //Sincronizaci�n por cambio de pin
  while(cont < repeticiones)
  {
    pin1 = digitalRead(PIN_RX);
    if (pin != pin1)
    {
        cont++;
        medidas[cont] = micros(); //timer2.get_count(); //
        pin = pin1;
    }
  }
*/
  cont = 0;
  while(cont++ < repeticiones)
  {
    Serial.print((medidas[cont]-medidas[cont-1])/2);
    Serial.print(",");
    if (!(cont%60)) Serial.println("");
  }
  delay(5000);
  return;
  /****  Recuperar se�al de sincronizaci�n ****/



  cont = 0;
  while(cont < repeticiones)
  {

    //ESperamos por la se�al de sincronizaci�n
    while (!vw_get_message(buf, (uint8_t *)cad1));

    digitalWrite(PIN_TRIGGER_RX, LOW);
    delayMicroseconds(2);
    digitalWrite(PIN_TRIGGER_RX, HIGH);
    delayMicroseconds(10);
    digitalWrite(PIN_TRIGGER_RX, LOW);
    delayMicroseconds(5);

    tiempo = pulseIn(PIN_ECHO_RX, HIGH);

    //Serial.println(tiempo);
    medidas[cont++] = tiempo*0.34;

  }

  for (int i = 1; i<repeticiones; i++)
  {
    Serial.println(medidas[i]);
  }

  return;
  while(true)
  {
    Serial.println(digitalRead(PIN_COM_RX));
    delay(200);
  }

}


/*
    Control de ultrasonidos y radio con emisor y receptor conectados a distintos arduinos ****************************************************************
    Prueba ultrasonidos enlace sync por radio
        Esperas con delayMicroseconds

*/
void loop11()
{
    cont=0;
  while(cont++ < repeticiones)
  {
  Serial.print(digitalRead(PIN_RX));
  delay(10);
  }
  Serial.println("");
  return;

}


/*
    Control de ultrasonidos y radio con emisor y receptor conectados a distintos arduinos ****************************************************************
    Prueba ultrasonidos enlace sync con cable
        Esperas con contador interno

*/
void loop3()
{
  tiempo = micros();
  TCNT1 = 0;
  while (TCNT1 < 16*100); //2 uS
  //Serial.println(micros()-tiempo);

  cont = 0;
  while(cont < repeticiones)
  {
    //ESperamos por la se�al de sincronizaci�n
    while(!digitalRead(PIN_COM_RX));

    digitalWrite(PIN_TRIGGER_RX, LOW);
    TCNT1 = 0;
    while (TCNT1 < 16*2); //2 uS
    digitalWrite(PIN_TRIGGER_RX, HIGH);
    TCNT1 = 0;
    while (TCNT1 < 16*10); //2 uS
    digitalWrite(PIN_TRIGGER_RX, LOW);
    TCNT1 = 0;
    while (TCNT1 < 16*5); //2 uS

    //tiempo = pulseIn(PIN_ECHO_RX, HIGH);
    //Control de tiempo de activaci�n por contador
    long CNT1 = 0;
    CNT1 = 0;
    TCNT1 = 0;

    //while(digitalRead(PIN_ECHO_RX) == LOW);
    while((PIND & 64) == 0);
    long vueltas = 0;
    while(true)
    {
      //if (digitalRead(PIN_ECHO_RX) == LOW) break;
      if ((PIND & 64) == 0) break;
      if (TCNT1 > 60000)
      {
        TCNT1 = 0;
        CNT1 = 60000;
      }
    }
    tiempo = (CNT1+TCNT1);

    //Serial.println(tiempo);
    medidas[cont++] = tiempo/16*.34;

  }
  for (int i = 0; i<repeticiones; i++)
  {
    Serial.println(medidas[i]);
  }
}

/*
    Control de ultrasonidos y radio con emisor y receptor conectados a distintos arduinos ****************************************************************
    Prueba ultrasonidos enlace sync con cable
        Esperas con delayMicroseconds

*/
void loop4()
{
  cont = 0;
  while(cont < repeticiones)
  {

    //ESperamos por la se�al de sincronizaci�n
    while(!digitalRead(PIN_COM_RX));

    digitalWrite(PIN_TRIGGER_RX, LOW);
    delayMicroseconds(2);
    digitalWrite(PIN_TRIGGER_RX, HIGH);
    delayMicroseconds(10);
    digitalWrite(PIN_TRIGGER_RX, LOW);
    delayMicroseconds(5);

    tiempo = pulseIn(PIN_ECHO_RX, HIGH);

    //Serial.println(tiempo);
    medidas[cont++] = tiempo*0.34;

  }

  for (int i = 1; i<repeticiones; i++)
  {
    Serial.println(medidas[i]);
  }

  return;
  while(true)
  {
    Serial.println(digitalRead(PIN_COM_RX));
    delay(200);
  }

}

/*
    Control de ultrasonidos y radio con emisor y receptor conectados al mismo arduino
*/

void loop1() {

while (cont++ < repeticiones)
{
  delay(100);
  digitalWrite(PIN_TRIGGER_RX, LOW);
  digitalWrite(PIN_TRIGGER_TX, LOW);
  delayMicroseconds(2);
  digitalWrite(PIN_TRIGGER_TX, HIGH);
  digitalWrite(PIN_TRIGGER_RX, HIGH);
  delayMicroseconds(10);
  digitalWrite(PIN_TRIGGER_TX, LOW);
  digitalWrite(PIN_TRIGGER_RX, LOW);
  delayMicroseconds(10);

   tiempo = pulseIn(PIN_ECHO_RX, HIGH);

   //Serial.println(tiempo);
   medidas[cont] = tiempo;

}

for (int i = 1; i<repeticiones; i++)
{
  Serial.println(medidas[i]);
}

return;
while (espera_us < espera_us_fin)
{
    espera_us++;
    while (true)
    {
        cont++;
        delay(10);

        if (cont == iteraciones_us)
        {
          Serial.print(espera_us);
          Serial.print(",");
          Serial.print(cont_igual);
          Serial.print(",");
          Serial.print(cont_diferente);
          Serial.print(",");
          Serial.print(cont_igual0);
          Serial.print(",");
          Serial.println(cont_igual1);

          cont = 0;
          cont_igual = 0;
          cont_diferente = 0;
          cont_igual0 = 0;
          cont_igual1 = 0;
          valor = HIGH;
          break;
        }
        else
        {
          digitalWrite(9, valor);
          //long rmicros = micros();
          TCNT1 = 0;
          //while (TCNT1 < espera_us<<4);
          //while (TCNT1 < espera_us); //Aumentando la precisi�n, el rango se mantiene igual

          //delayMicroseconds(espera_us);
          //delay(80); // la portadora tiene una duraci�n de tx m�xima de 80 ms
          pin = digitalRead(8);
          //Serial.println(micros()-rmicros); //Los delaymicros son precisos, pero micros() tiene un error de 4 us

          if (valor == pin)
          {
            cont_igual++;
            //Serial.print(pin);
          }
          else
          {
            cont_diferente++;
            //Serial.print("_");
          }

          if (valor == HIGH)
          {
            if (valor == pin) cont_igual1++;
          }
          else
          {
            if (valor == pin) cont_igual0++;
          }

          valor = !valor;

          // if (cont % 200 == 0)
          // {
          //   Serial.println("");
          // }
        }
    }

}



  /*vw_send((uint8_t *)cad, 4);
  delay(1000);

    if (vw_get_message(buf, (uint8_t *)cad1)) // Non-blocking
    {
      Serial.println(cad1);
    }*/

}
