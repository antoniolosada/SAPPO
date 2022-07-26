#include "FiltroKalman.h"
#include "PID_v2.h"
#include "SoftwareServo.h"
#include "list.h"
#include "node_cpp.h"
#include <Kalman.h>
#include <RF24.h>
#include <RF24_config.h>
#include <SPI.h>
#include <VirtualWire.h>
#include <nRF24L01.h>

#define NUM_MEDIDAS_RECUPERAR 2
#define EMISION_CONJUNTA -1

#define KALMAN1 1
#define KALMAN2 2

#define SALIDA 1
#define SALIDA_COMPLETA 2

// Inclusiones de ficheros fuente alternativos. Si no se incluyen falla el
// proceso de build
#include "list.cpp"
#include "node_cpp.cpp"

#define MOTOR_D_PARADO 110
#define MOTOR_I_PARADO 110

#define MS_FRENADA 40
#define MS_ESPERA 200
#define PITIDO_ESTADOS 0
#define TIMEOUT_MEDIDA_US 60

#define VELOCIDAD_RAPIDA 200
#define VELOCIDAD_MENOS_RAPIDA 170
#define PARADO 0
#define VELOCIDAD_LENTA 150

#define MODE_HIBERNATE 1
#define MODE_SUSPEND 2
#define MODE_ALERT 3
#define MODE_ACTIVE 4

#define OP_READ_INIT 1
#define OP_READ_DISTANCE 2
#define OP_READ_FIN 3
#define OP_SET_MODE 4

#define OP_INICIAR 1
#define OP_CONSULTAR 2

// Identificaci?n de PINES **************************************
#define BT_ENABLE 21
#define RUEDA_DER_ODO 3
#define RUEDA_IZQ_ODO 20

#define BT_ENABLE 21
#define SERVO_DER 32
#define SERVO_IZQ 33

#define LINEA_der 37
#define LINEA_centro 35
#define LINEA_izq 36

#define RUEDA_der 3
#define RUEDA_izq 2

#define PIN_W 5
#define PIN_R 4
// FIN Identificaci?n de PINES **************************************

#define TONE 8
#define MAX_EMISORES_ULTRASONIDOS 8
#define TRIGER1 38
#define TRIGER2 39
#define TRIGER3 40
#define TRIGER4 41

#define TRIGER5 22
#define TRIGER6 23
#define TRIGER7 24
#define TRIGER8 25

//---------------------------------
#define ECHO1 42
#define ECHO2 43
#define ECHO3 44
#define ECHO4 45

#define ECHO5 46
#define ECHO6 47
#define ECHO7 48
#define ECHO8 49

#define ID_OPERACION_MEDIDA 1
#define ID_ROBOT_RODI 1
#define NUM_MEDIDAS_BALIZAS 20 // N?mero m?ximo de medidas que almacenamos
#define NUM_MEDIDAS_VALIDAS                                                    \
  10 // N?mero m?nimo de medidas para realizar el control de medias
#define NUM_MEDIDAS_MAL_RESET                                                  \
  10 // N?mero de medidas descartadas para resetear la lista de medidas
#define NUM_MEDIDAS_OK_RESET                                                   \
  10 // N?mero de medidas buenas para borrar el acumulado de errores
#define MIN_CONTROL_TIEMPO_VUELO                                               \
  10 // M?rgenes de control de tiempo de vuelo de las balizas
#define MAX_CONTROL_TIEMPO_VUELO 1000
#define MARGEN_MEDIDA_DELTA                                                    \
  4 // Margen para que la nueva medida se considere v?lida
#define ROBOT_ALTURA_SUELO_SENSORES 15
#define MAX_TIEMPO_INACTIVIDAD                                                 \
  1000 // ms. Si durante este tiempo no se localiza una valiza, se entiende que
       // comenzamos a almacenar las medidas
#define ERRORES_CAMBIO_BALIZA                                                  \
  4 // N?mero de errores para buscar nuevas balizas para la medida

#define MAX_BALIZAS                                                            \
  10 // N?mero m?ximo de balizas que pueden leerse de modo simult?neo

#define MAX_TIEMPO_VUELO_SONIDO 30 // Tiempo en ms para que el sonido recorra 8m

#define NUM_BALIZAS 2

#define MAX_MEDIDAS 4

#define IZQUIERDA 0
#define DERECHA 1
#define ADELANTE 0
#define ATRAS 1

#define POTENCIA_MINIMA 10
#define POTENCIA_ARRANQUE 140
#define SAMPLE_TIME 150
#define SET_POINT_INI 17

#define MUESTREO_POSICION 10

//*************************************************************************************************************
//                        CONFIGURACI?N DE BALIZAS
//*************************************************************************************************************

struct sPulsos {
  long pulsos;
  long Ultimo_ms;
  long Tiempo;
};
struct sOdometria {
public:
  sPulsos Contador[2];
  long pulsos_totales;
  long Ultimo_ms_total;
  byte ultimo_valor;
  long tiempo_total;
};

struct sBaliza {
  long codigo;
  int id_baliza;
  int id_habitacion;
  int X;
  int Y;
  int Z; // Altura de los sensores desde el suelo
  int grados_cobertura;
  int numero_sensores;
  int direccion;

public:
  void print();
};

void sBaliza::print() {
  Serial.print(F("ID Baliza "));
  Serial.println(id_baliza);
  Serial.print(F("ID Habitacion "));
  Serial.println(id_habitacion);
  Serial.print("X ");
  Serial.println(X);
  Serial.print("Y ");
  Serial.println(Y);
  Serial.print("Z ");
  Serial.println(Z);
  Serial.print(F("Gradros de cobertura "));
  Serial.println(grados_cobertura);
  Serial.print(F("N?mero sensores "));
  Serial.println(numero_sensores);
  Serial.print(F("Direcci?n "));
  Serial.println(direccion);
}

struct sMedida {
  byte codigo;
  float tiempo_local;
  float tiempo_vuelo;
  float tiempo_vuelo_ajustado; // Ajustado con el control de altura entre baliza
                               // y movil
  int num_sensor;
};

struct sMedidas {
  byte codigo;
  byte id_baliza;
  List<sMedida> Medida;
  int numero_medidas;
  long MedidasCorrectas;
  int FallosTotales;
  int FallosAcumulados;
  long tiempo_vida;
  long tiempo_ultima_medida;
  long numero_ultima_medida;
public:
  void Inicializar() {
    numero_medidas = 1;
    MedidasCorrectas = 1;
    FallosAcumulados = 0;
    FallosTotales = 0;
  }
};

struct sProcesoMedida {
  byte BalizasSeleccionadas[2];
  byte ErroresBalizasSeleccionadas[2];
  byte num_balizas;
  byte errores_acumulados;
  long numero_total_medidas;

  sProcesoMedida() {
    for (int i = 0; i < 2; i++)
      BalizasSeleccionadas[2] = ErroresBalizasSeleccionadas[2] = 0;
    num_balizas = 0;
    numero_total_medidas = 0;
    errores_acumulados = 0;
  }
};

struct sPaquete {
  byte id_baliza;
  long tiempo;
  byte num_sensor;
  long tiempo_local;
};

struct sPaqueteRF433 {
  byte id_robot;
  byte operacion;
};
