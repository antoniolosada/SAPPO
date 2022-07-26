#include <SPI.h>
#include "nRF24L01.h"
#include "RF24.h"
#include <Servo.h>

Servo Servo1;
Servo Servo2;

// Hardware configuration
//Set up nRF24L01 radio on SPI bus plus pins 9 & 10 
//RF24 (cepin, cspin) 
RF24 radio(8,7);

//
// Topology
// Radio pipe addresses for the 2 nodes to communicate.
const uint64_t pipes[2] = { 0xF0F0F0F0E1LL, 0xF0F0F0F0D2LL };

void setup(void)
{

 pinMode(7, OUTPUT); 
  Serial.begin(9600);

  // Setup and configure rf radio
  radio.begin();
  // optionally, increase the delay between retries & # of retries
  radio.setRetries(10,0);
  // optionally, reduce the payload size.  seems to
  // improve reliability
  radio.setPayloadSize(8);
  radio.setPALevel(RF24_PA_HIGH);
  radio.setDataRate(RF24_2MBPS);
  radio.setChannel(120);


  // Start listening
  radio.startListening();
  radio.openWritingPipe(pipes[1]);
  radio.openReadingPipe(1,pipes[0]);

  Servo1.attach(5); //derecha mirando de frente
  Servo2.attach(6); //Izquierda mirando de frente
}

unsigned long contador = 0;
bool activar = false;

void loop(void)
{

    // if there is data ready
    if ( radio.available() )
    {
      // Dump the payloads until we've gotten everything
      unsigned long got_time;
      bool done = false;
      while (!done)
      {
        // Fetch the payload, and see if this was the last one.
        done = radio.read( &got_time, sizeof(unsigned long) );
        Serial.print("Dato Recibido =");
        Serial.println(got_time);
 // Delay just a little bit to let the other unit
 // make the transition to receiver
 delay(20);
      }
      
      if (got_time == 1)
        activar = true;
      else
        activar = false;
    }
    
    if (activar)
    {
      for (int i = 80; i< 120; i++)
      {
        Servo1.write(120+(80-i));
        delay(10);
        Servo2.write(i);
        delay(10);
      }
      for (int i = 120; i> 80; i--)
      {
        Servo1.write(80-(i-120));
        delay(10);
        Servo2.write(i);
        delay(10);
      }    
    }
}
