#include "Arduino.h"
//The setup function is called once at startup of the sketch
/*  ----------------------------------------------------------------
    http://www.prometec.net/nrf2401
    Prog_79_1_Emisor

    Programa para recibir strings mediante radios NRF2401
	ATENCION!!!!!!
	Colocar el pin de RESET para subir el sketch
--------------------------------------------------------------------
*/

#include <nRF24L01.h>
#include <RF24.h>
#include <RF24_config.h>
#include <SPI.h>
#include "LowPower.h"

#include <VirtualWire.h>


/*  Estas inclusiones las realiza automáticamente el fichero sloeber.ino.cpp
#define ID_BALIZA    2 //********************************************** ID_BALIZA
#include "..\Baliza1\Baliza1.ino"
*/


