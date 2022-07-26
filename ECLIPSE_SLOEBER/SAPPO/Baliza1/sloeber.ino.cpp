#ifdef __IN_ECLIPSE__
//This is a automatic generated file
//Please do not modify this file
//If you touch this file your change will be overwritten during the next build
//This file has been generated on 2022-06-18 09:32:59

#include "Arduino.h"
#include "Arduino.h"
#include <nRF24L01.h>
#include <RF24.h>
#include <RF24_config.h>
#include <SPI.h>
#include "LowPower.h"
#include <VirtualWire.h>

void setup(void) ;
void loop(void) ;
void LeerSensorUltrasonidos(int sensor) ;
char ComprobarFalloSensor(int sensor) ;
bool LeerSensoresUltrasonidos() ;
void LeerSensoresUltrasonidosMedidaMultiple() ;
void RecuperarOperacionLecturas() ;
void RecuperarDistancia(int l) ;
bool Espera(unsigned long us, int contador, int op) ;
void ordenar_vector(unsigned long *vNumeros) ;
int RecuperarMensajeRF_433Mhz(int salida) ;

#include "Baliza1.ino"


#endif
