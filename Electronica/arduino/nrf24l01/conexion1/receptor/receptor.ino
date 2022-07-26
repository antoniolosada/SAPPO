#include <SPI.h>
#include "nRF24L01.h"
#include "RF24.h"

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
  radio.setRetries(10,20);
  // optionally, reduce the payload size.  seems to
  // improve reliability
  radio.setPayloadSize(8);
  radio.setPALevel(RF24_PA_HIGH);
  radio.setDataRate(RF24_1MBPS);
  radio.setChannel(120);


  // Start listening
  radio.startListening();
  radio.openWritingPipe(pipes[1]);
  radio.openReadingPipe(1,pipes[0]);
}

unsigned long contador = 0;

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

      // First, stop listening so we can talk
      radio.stopListening();

      // Send the final one back.
      radio.write( &got_time, sizeof(unsigned long) );
      Serial.println("Enviando Respuesta");

      // Now, resume listening so we catch the next packets.
      radio.startListening();
    }
}
