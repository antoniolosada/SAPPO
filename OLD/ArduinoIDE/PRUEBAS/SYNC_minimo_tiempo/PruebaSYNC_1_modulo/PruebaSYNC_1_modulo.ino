/*
Estudio de sincronización en un solo arduino empleando los módulos simples de tx y rx de 433 Mhz
*/

//#include <VirtualWire.h>

#define PIN_TX  9
#define PIN_RX  8
#define PIN_EN_TX 10

#define PIN_TRIGGER_RX  7
#define PIN_ECHO_RX     6

#define PIN_TRIGGER_TX  5

void ConfigurarTimer()
{
    // Configuração do TIMER1
  TCCR1A = 0;                // El registro de control A queda todo en 0
  TCCR1B = 0;                //limpia registrador
  TCNT1  = 0;                //Inicializa el temporizador
  OCR1A = 0xFFFF;            // carga el registrador de comparación: 16MHz/1024/1Hz -1 = 15624 = 0X3D08
  //TCCR1B |= (1 << WGM12)|(1<<CS10)|(1 << CS12);   // modo CTC, prescaler de 1024: CS12 = 1 e CS10 = 1  
  TCCR1B |= (1 << WGM12)|(1<<CS10);   // modo CTC, prescaler de 1024: CS12 = 1 e CS10 = 1  
  //TIMSK1 |= (1 << OCIE1A);  // habilita interrupción por igualdade de comparación
}

void setup() {
  // put your setup code here, to run once:
  pinMode(PIN_RX, INPUT);
  pinMode(PIN_TX, OUTPUT);
  digitalWrite(PIN_TX, HIGH);

  pinMode(PIN_ECHO_RX, INPUT);
  pinMode(PIN_TRIGGER_RX, OUTPUT);
  pinMode(PIN_TRIGGER_TX, OUTPUT);
  pinMode(PIN_EN_TX, OUTPUT);
  
  digitalWrite(PIN_EN_TX, LOW);

  /*vw_set_rx_pin(8);
  vw_set_tx_pin(9);
  vw_set_ptt_inverted(true); // Required for DR3100
  vw_setup(2000);      // Bits per sec
  vw_rx_start();       // Start the receiver PLL running
  delay(50);*/

  //ConfigurarTimer();
  Serial.begin(9600);
}

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

//x16 en precisión
//int espera_us = 670;
//int espera_us_fin = 750;

int iteraciones_us = 200;

void loop2() {

    int repeticiones = 100;
    int cont = 0;
    long tiempo = 0;

    long medidas[101];

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
}

void loop() {
  int pin = 0;
  int cont = 100;
  int valor = 0;

  while(true)
  {
    delayMicroseconds(10);
    
    if (cont-- == 0)
    {
      cont = 50; 
      valor = !valor;
      digitalWrite(PIN_TX, valor);
      Serial.println(valor);
    }
    pin = digitalRead(PIN_RX);
    Serial.print(pin);
  }

}

void loop3() {

    int repeticiones = 100;
    int cont = 0;
    long tiempo = 0;

    long medidas[101];
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
          //while (TCNT1 < espera_us); //Aumentando la precisión, el rango se mantiene igual 

          delayMicroseconds(espera_us);
          //delay(80); // la portadora tiene una duración de tx máxima de 80 ms
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
