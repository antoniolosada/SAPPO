#include <VirtualWire.h>

void setup() {
  // put your setup code here, to run once:
  pinMode(8, INPUT);
  pinMode(9, OUTPUT);
  digitalWrite(9, HIGH);

  /*vw_set_rx_pin(8);
  vw_set_tx_pin(9);
  vw_set_ptt_inverted(true); // Required for DR3100
  vw_setup(2000);      // Bits per sec
  vw_rx_start();       // Start the receiver PLL running
  delay(50);*/


  Serial.begin(57600);
}

char *cad = "hola";
char cad1[20];
uint8_t buf[200];
int pin;
int valor  = 0;

int cont = 0;
void loop() {
  cont++;
  // put your main code here, to run repeatedly:

  delay(10);

  digitalWrite(9, valor);
  delayMicroseconds(120);
  pin = digitalRead(8);

  if (valor == pin)
    Serial.print(pin);
  else
    Serial.print("_");

  valor = !valor;

if (cont > 200) 
{
  cont = 0;
  Serial.println("");

}

  /*vw_send((uint8_t *)cad, 4);
  delay(1000);

    if (vw_get_message(buf, (uint8_t *)cad1)) // Non-blocking
    {
      Serial.println(cad1);
    }*/

}
