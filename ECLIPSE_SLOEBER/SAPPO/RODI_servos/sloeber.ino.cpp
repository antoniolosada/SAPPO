#ifdef __IN_ECLIPSE__
//This is a automatic generated file
//Please do not modify this file
//If you touch this file your change will be overwritten during the next build
//This file has been generated on 2022-10-17 19:25:36

#include "Arduino.h"
#include <ServoTimer2.h>
#include <SingleEMAFilterLib.h>
#include <MeanFilterLib.h>
#include "FiltroKalman.h"
#include "PID_v2.h"
#include "SoftwareServo.h"
#include "list.h"
#include "node_cpp.h"
#include "Kalman.h"
#include <RF24.h>
#include <RF24_config.h>
#include <SPI.h>
#include <VirtualWire.h>
#include <nRF24L01.h>
#include <PlotPlus.h>
#include "RODI_servos.h"
#include "list.cpp"
#include "node_cpp.cpp"

void setup(void) ;
void loop(void) ;
void test() ;
void PruebaODO() ;
void LOG_ruedas(int log) ;
int Mapear(int dir, int v1m, int v1M, int v2m, int v2M, int Velocidad) ;
void ActivarTimer2() ;
void LOG() ;
void moverServo(int pin, int angulo) ;
void CargarDatosBalizas() ;
boolean LeerMedidasSensores(int emisor) ;
float Tiempo2Distancia(long Tiempo) ;
bool RecuperarMedidasBalizas(int algoritmo, bool salida) ;
long LeerSensorUltrasonidos(int iSensor) ;
void LeerSensoresUltrasonidos() ;
float RecuperarMedida() ;
boolean RecuperarMedidaMultiple(int emisor, int LOG) ;
bool RecuperarDistanciasSonar(int Sensor) ;
float RecuperarOrientacion(int Baliza, float Medida, int NumSensores) ;
void EnviarPulsoUltrasonidos(int emisor) ;
bool Espera(unsigned long ms, int contador, int op) ;
void EnviarMensaje433Mhz(int lectura) ;
void InicializarDatosBalizas(struct sBaliza *Balizas) ;
int InterseccionCircunferencias(int iD1, int iD2, struct sBaliza BalizaAnt,                                  struct sBaliza BalizaSig, long &lPosX,                                  long &lPosY) ;
int freeRam() ;
void PruebaBluetooth() ;
void EjecutarComando() ;
void RecuperarComandos() ;
void ControlMotores(int PotDerecha, int PotIzquierdo, int op) ;
void MoverRueda(byte rueda, byte direccion, int velocidad) ;
void Tono(int PWM, int ms) ;
void CambioMovimiento(byte Estado) ;
void RecuperarMedidasMultiples() ;
void intRueda_der() ;
void intRueda_izq() ;
void ContarPulsoRueda(int rueda, int lecturaAct) ;
void ControlVelocidad(int rueda) ;

#include "RODI_servos.ino"


#endif
