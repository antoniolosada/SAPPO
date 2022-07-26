/*  ----------------------------------------------------------------
http://www.prometec.net/nrf2401
Prog_79_1_Emisor

Programa para transmitir strings mediante radios NRF2401
1. Configurar los pines del RF24 para arduino uno o mega


Motor D punto central adelante < 70-80 < atras
Motor I punto central atras < 77 < adelante

Avanzar muy despacio hacia adelante
mi 85
md 55

COM5 38400 BT


-Orden real de los conectores de los transceptores
1230
/--\
                        -> Parte trasera
\--/
0123

-Orden en el array
7654
/--\
                        -> Parte trasera
\--/
0123
--------------------------------------------------------------------
*/


#include "RODI_servos.h"

//#include "ServoTimer2.h"
int SensorEmisor = EMISION_CONJUNTA;
byte aTrigger[MAX_EMISORES_ULTRASONIDOS] = {TRIGER1, TRIGER2, TRIGER3, TRIGER4,
                                            TRIGER5, TRIGER7, TRIGER8, TRIGER6};
byte aEcho[MAX_EMISORES_ULTRASONIDOS] = {ECHO1, ECHO2, ECHO3, ECHO4,
                                         ECHO5, ECHO7, ECHO8, ECHO6};
String comando;

char msg[16] = "."; // Array a transmitir


List<sBaliza> ListaBalizas;
List<sMedidas> ListaMedidasBalizas;
sProcesoMedida ProcesoMedida;

RF24 radio(9,53); 

const uint64_t pipe1 = 0x00000000E1LL;    // Usamos este canal
const uint64_t pipe2 = 0x00000000E2LL;    // Usamos este canal
const uint64_t broadcast = 0xE8E8F0F0E1LL; // Usamos este canal
char direccion[3][6] = {"0Nodo", "1Nodo", "2Nodo"};

sPaquete paquete[MAX_BALIZAS];
sPaqueteRF433 paqueteRF433;

void CargarDatosBalizas();
void EnviarPulsoUltrasonidos(int emisor);
void ActivarTimer2();
int algoKalman = KALMAN1;

boolean PararMotores = true;

SoftwareServo Motor[2];
FiltroKalman2D fk;       // FiltroKalman para 1 dimension
FiltroKalman1D fkRuedaI; // FiltroKalman para 1 dimension
Vec2f out, input;

sOdometria Odo[2];
double SetpointVal[2], Setpoint[2], Input[2], Output[2];
PID2 Pid[2] = {PID2(&Input[IZQUIERDA], &Output[IZQUIERDA], &Setpoint[IZQUIERDA],
                    0.6, 0.3, 0.01, REVERSE),
               PID2(&Input[DERECHA], &Output[DERECHA], &Setpoint[DERECHA], 0.6,
                    0.3, 0.01, REVERSE)};

double InputGiro = 1, OutputGiro = 1, SetpointGiro = 1;
PID2 PidGiro(&InputGiro, &OutputGiro, &SetpointGiro, 2, 0, 0, DIRECT);

char PotenciaArranque[2] = {15, 0};
byte PotenciaMinima[2] = {0, 0};
byte PotenciaMaxima[2] = {40, 40};
Kalman myFilterX(0.125, 32, 1023,
                 0); // suggested initial values for high noise filtering
Kalman myFilterY(0.125, 32, 1023,
                 0); // suggested initial values for high noise filtering
                     // Kalman myFilterY(0.125, 32, 1023, 0); //suggested
                     // initial values for high noise filtering
float filtroX, filtroY;
Kalman klmRev[2] = {Kalman(0.125, 32, 1023, 0), Kalman(0.125, 32, 1023, 0)};

/*********************************************************************************************************************************************************************************************/

void setup(void) {

  // turn the PID on
  for (int i = 0; i < 2; i++) {
    Pid[i].SetMode(AUTOMATIC);
    Pid[i].SetSampleTime(SAMPLE_TIME);
    Pid[i].SetOutputLimits(PotenciaMinima[i], PotenciaMaxima[i]);
  }
  for (int i = 0; i < 2; i++) {
    SetpointVal[i] = SET_POINT_INI;
    Setpoint[i] = SET_POINT_INI;
    Output[i] = 0;
    Input[i] = 0;
  }

  pinMode(13, OUTPUT);

  pinMode(RUEDA_DER_ODO, INPUT);
  pinMode(RUEDA_IZQ_ODO, INPUT);

  for (int i = 0; i < 1; i++)
    for (int j = 0; j < 1; j++) {
      Odo[i].Contador[j].pulsos = Odo[i].Contador[j].Tiempo =
          Odo[i].Contador[j].Ultimo_ms = 0;
    }

  Motor[IZQUIERDA].attach(SERVO_IZQ);
  Motor[DERECHA].attach(SERVO_DER);
  // Motor[IZQUIERDA].write(MOTOR_I_PARADO);
  // Motor[DERECHA].write(MOTOR_D_PARADO);

  for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++) {
    pinMode(aTrigger[i], OUTPUT);
    pinMode(aEcho[i], INPUT);
  }

  // Motores
  pinMode(LINEA_der, INPUT);
  pinMode(LINEA_izq, INPUT);
  pinMode(LINEA_centro, INPUT);

  pinMode(RUEDA_der, INPUT);
  pinMode(RUEDA_izq, INPUT);

  // Ultrasonidos
  pinMode(PIN_W, OUTPUT); /*salida: para el pulso ultras?nico*/
  pinMode(PIN_R, INPUT);  /*entrada: tiempo del rebote del ultrasonido*/

  Serial.begin(57600);

/*************************  CODIGO DE PRUEBA ********************************/
pinMode(12, OUTPUT);
 return;
/*************************  CODIGO DE PRUEBA ********************************/

  // Initialise the IO and ISR
  vw_set_tx_pin(12);
  vw_setup(2000); // Bits per sec
                  // vw_rx_start();       // Start the receiver PLL running


  // Configurar puerto para el m?dulo bluetooth
  Serial1.begin(38400);
  pinMode(BT_ENABLE, OUTPUT);
  digitalWrite(BT_ENABLE, HIGH);

  radio.begin();
  radio.setPALevel(RF24_PA_HIGH);
  //radio.openReadingPipe(0, broadcast);
  radio.openReadingPipe(0, pipe1);
  radio.openReadingPipe(1, pipe2);

  radio.setChannel(108);
  radio.setRetries(5, 5);
  radio.setAutoAck(true);

  CargarDatosBalizas();

  // Muestra la memoria de datos disponible
  Serial.print("M.Free: ");
  Serial.println(freeRam());
  Serial1.println("------------------------------------");
  delay(2);

  // attachInterrupt(digitalPinToInterrupt(RUEDA_DER_ODO), intRueda_der,
  // CHANGE); attachInterrupt(digitalPinToInterrupt(RUEDA_IZQ_ODO), intRueda_izq,
  // CHANGE);

  // ArrancarMotor();

  ActivarTimer2();

  fk.init(0.001, 0.025);
  fkRuedaI.init(0.001, 0.025);

  Odo[DERECHA].pulsos_totales == -1;
  Odo[DERECHA].ultimo_valor = digitalRead(RUEDA_DER_ODO);
  Odo[IZQUIERDA].pulsos_totales == -1;
  Odo[IZQUIERDA].ultimo_valor = digitalRead(RUEDA_IZQ_ODO);

  PidGiro.SetMode(AUTOMATIC);
  PidGiro.SetSampleTime(SAMPLE_TIME);
}



/************************************************************************************************************************************************************************************
En este bucle debemos recuperar las medidas de las balizas.
En cada iteraci?n el sistema envia un paquete de readio seguido de un tren de
pulsos utlras?nicos por el collar de ultraosnidos ominidereccional. Las balizas
recuperan la informaci?n del paquete de radio e inician un contador hasta la
llegada del tren de pulsos de ultrasonidos Una vez recuperado
*************************************************************************************************************************************************************************************/

long iteracion = 0;
long repeticiones = 0;
long pulsos_ant = 0;

void loop(void) {
  Serial1.println(repeticiones++);
  delay(1000);
  
  // PruebaODO();
  // return;
  
  /*RecuperarOrientacion();
  delay(1000);*/
  
  /*************************  CODIGO DE PRUEBA ********************************/

  unsigned long contador = 0;
  byte salida = LOW;
  while (true)
  {
    if (contador == 0) 
    {
      digitalWrite(12, salida);
      Serial.println(salida);
    }
    if (contador == 60000)
    {
      salida = !salida;
      contador = 0;
    }
    else
      contador++;
  }


  return;

  /*unsigned long contador = 0;
  while (true)
  {
    if (contador == 0) EnviarMensaje433Mhz(0);
    if (contador == 60000)
      contador = 0;
    else
      contador++;
  }*/

  return;
  /*************************  CODIGO DE PRUEBA ********************************/



  // Recuperar informaci?n de balizas de modo consecutivo
  bool medidas = RecuperarMedidasBalizas(algoKalman, false);
  delay(5);
  
  RecuperarComandos();
  

  if (iteracion == MUESTREO_POSICION) {
          iteracion = 0;
          if (algoKalman == KALMAN1) {
            Serial1.print(filtroX);
            Serial1.print(",");
            Serial1.print(filtroY);
            Serial1.print(",");
            Serial1.print(Odo[DERECHA].pulsos_totales);
            Serial1.print(",");
            Serial1.println(Odo[IZQUIERDA].pulsos_totales);
          } else if (algoKalman == KALMAN2) {
            Serial1.print(out[0]);
            Serial1.print(",");
            Serial1.print(out[1]);
            Serial1.println("");
          }
  } else
   if (medidas) iteracion++;
}

#define TIMER2_OVF 200
int contador = TIMER2_OVF;
int pulso_der = LOW;
int pulso_izq = LOW;
long contadorODO = 0;
char lecturader, lecturaizq;
#define CICLO_ODO 20 // Se controla la odometr?a cada 8ms

ISR(TIMER2_COMPA_vect) { // timer1 interrupt 8kHz toggles pin 9

  if (contadorODO++ > CICLO_ODO) {
    // Incremento de tiempo en caso de que el tiempo grabado sea menor que el
    // tiempo actual transcurrido desde el ultimo cambio
    for (int rueda = 0; rueda < 2; rueda++) {
      for (int lectura = 0; lectura < 2; lectura++) {
        long tiempo = millis() - Odo[rueda].Contador[lectura].Ultimo_ms;
        if (tiempo > Odo[rueda].Contador[lectura].Tiempo)
          Odo[rueda].Contador[lectura].Tiempo = tiempo;
      }
    }

    lecturader = digitalRead(RUEDA_DER_ODO);
    if (lecturader != pulso_der) {
      ContarPulsoRueda(DERECHA, lecturader);
      pulso_der = lecturader;
    }

    lecturaizq = digitalRead(RUEDA_IZQ_ODO);
    if (lecturaizq != pulso_izq) {
      ContarPulsoRueda(IZQUIERDA, lecturaizq);
      pulso_izq = lecturaizq;
    }
    contadorODO = 0;
  }

  if (!PararMotores)
    if (--contador == 0) {
      contador = TIMER2_OVF;
      SoftwareServo::refresh();
    }
}

#define INC 5

void PruebaODO() {
  float tiempoD, tiempoI;

  PararMotores = 1;

  tiempoD = (int)(Odo[DERECHA].Contador[0].Tiempo / 10);
  Setpoint[DERECHA] = 27;
  Input[DERECHA] = tiempoD;
  Motor[DERECHA].write(MOTOR_D_PARADO + Output[DERECHA]);
  Pid[DERECHA].Compute();

  tiempoI = (int)(Odo[IZQUIERDA].Contador[0].Tiempo / 10);
  Input[IZQUIERDA] = tiempoI;
  Setpoint[IZQUIERDA] = 27;
  Motor[IZQUIERDA].write(MOTOR_I_PARADO - Output[IZQUIERDA]);
  Pid[IZQUIERDA].Compute();

  delay(50);

  // Serial.print((int)(Input[DERECHA]));
  // Serial.print(Output[DERECHA]);
  // Serial.println("");

  // Serial.print(" - ");
  // Serial.print(InputGiro);
  // Serial.print(" - ");
  // Serial.println(OutputGiro);

  // SetpointGiro = 1.5;
  // Motor[DERECHA].write(MOTOR_D_PARADO + 20 + OutputGiro * INC);
  ////Rebajamos los contadores
  // if (Odo[DERECHA].pulsos_totales == 0)
  //	InputGiro = 1;
  // else
  //	InputGiro = 1.0 * Odo[IZQUIERDA].pulsos_totales /
  //Odo[DERECHA].pulsos_totales;

  // LOG_ruedas(1);
}

void LOG_ruedas(int log) {
  static int v = 0;

  if (log == 0) {
    Serial.print(digitalRead(RUEDA_IZQ_ODO));
    if (Odo[IZQUIERDA].pulsos_totales != pulsos_ant) {
      Serial.print("-");
      Serial.println(Odo[IZQUIERDA].pulsos_totales);
      pulsos_ant++;
    }
  }
  if (log == 1) {
    Serial.print(digitalRead(RUEDA_DER_ODO));
    if (Odo[DERECHA].pulsos_totales != pulsos_ant) {
      Serial.print("-");
      Serial.print(Odo[DERECHA].pulsos_totales);
      Serial.print(" - ");
      Serial.println(1.0 * Odo[IZQUIERDA].pulsos_totales /
                     Odo[DERECHA].pulsos_totales);
      pulsos_ant++;
    }
  }
  if (log == 2) {
    Serial.print(Odo[IZQUIERDA].pulsos_totales / 10);
    Serial.print(", ");
    Serial.println(Odo[DERECHA].pulsos_totales / 10);
  }
  if (log == 3) {
    Serial.print(Input[IZQUIERDA]);
    Serial.print(", ");
    Serial.println(Input[DERECHA]);
  }
}

int Mapear(int dir, int v1m, int v1M, int v2m, int v2M, int Velocidad) {
  double ratio = (double)abs(v1M - v1m) / (v2M - v2m);
  return (int)(dir * (Velocidad - v2m) * ratio + v1m);
}

void ActivarTimer2() {
  cli();
  // set timer2 interrupt at 8kHz
  TCCR2A = 0; // set entire TCCR2A register to 0
  TCCR2B = 0; // same for TCCR2B
  TCNT2 = 0;  // initialize counter value to 0
             // set compare match register for 8khz increments
  OCR2A = 249; // = (16*10^6) / (8000*8) - 1 (must be <256)
               // turn on CTC mode
  TCCR2A |= (1 << WGM21);
  // Set CS21 bit for 8 prescaler
  TCCR2B |= (1 << CS21);
  // enable timer compare interrupt
  TIMSK2 |= (1 << OCIE2A);
  sei();
}

void LOG() {}

void moverServo(int pin, int angulo) {
  float pausa;
  pausa = angulo * 2000.0 / 180.0 + 500;
  digitalWrite(pin, HIGH);
  delayMicroseconds(pausa);
  digitalWrite(pin, LOW);
  delayMicroseconds(2500 - pausa);
}
void CargarDatosBalizas() {
  sBaliza Baliza;
  Baliza.codigo = 1;
  Baliza.id_baliza = 1;
  Baliza.id_habitacion = 1;
  Baliza.grados_cobertura = 90;
  Baliza.direccion = 1;
  Baliza.numero_sensores = 4;
  Baliza.X = 10;
  Baliza.Y = 10;
  Baliza.Z = 10; // Altura de los sensores desde el suelo
  ListaBalizas.add_head(Baliza);

  Baliza.codigo = 2;
  Baliza.id_baliza = 2;
  Baliza.id_habitacion = 1;
  Baliza.grados_cobertura = 90;
  Baliza.direccion = 1;
  Baliza.numero_sensores = 4;
  Baliza.X = 210;
  Baliza.Y = 10;
  Baliza.Z = 10;
  ListaBalizas.add_head(Baliza);
}

boolean LeerMedidasSensores(int emisor) {
  for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++)
    paquete[i].id_baliza = 0;

  EnviarMensaje433Mhz(0);
  EnviarPulsoUltrasonidos(emisor);

  radio.startListening();

  unsigned long started_waiting_at = millis();
  bool timeout = false;

  float medida;
  int numero_medidas = 0;
  // Recuperamos todas las medidas posibles de las balizas
  while (
      !timeout) // Esperamos hasta el m?ximo tiempo de vuelo de una se?al de US
  {
    while ((!radio.available()) && !timeout) // Esperamos
    {
      if (millis() - started_waiting_at > MAX_TIEMPO_VUELO_SONIDO)
        timeout = true;
    }

    if (!timeout) { // Leemos el mensaje recibido
      unsigned long got_time;
      radio.read(&paquete[numero_medidas], sizeof(sPaquete));
      medida = 1.0 * paquete[numero_medidas].tiempo /
               29.2; // convertimos a distancia, en cm
      paquete[numero_medidas].tiempo = medida;
      paquete[numero_medidas].tiempo_local = micros();
      numero_medidas++;
    }
  }

  radio.stopListening();

  return numero_medidas;
}

float Tiempo2Distancia(long Tiempo) { return (1.0 * Tiempo / 29.2); }

bool RecuperarMedidasBalizas(int algoritmo, bool salida) {
  float minimo, medida[2], media, maximo;
  int medidas, id_baliza_x = 0, id_baliza_y = 0;
  boolean timeout;

  minimo = 999;
  maximo = 0;
  media = 0;
  medidas = 0;

  // Recuperamos dos medidas de sensores contrapuestos v?lidos para
  // triangulaci?n
  timeout = RecuperarMedidaMultiple(SensorEmisor);

  // Si timeout = true implica que no hemos podido recuperar dos medidas v?lidas
  // para triangular
  if (!timeout) {
    // Primero comprobamos que los identificadores de las balizas son correctas
    if (paquete[0].id_baliza != paquete[1].id_baliza != 0) {
      // Convertimos el tiempo a cm
      medida[paquete[0].id_baliza - 1] = Tiempo2Distancia(paquete[0].tiempo);
      medida[paquete[1].id_baliza - 1] = Tiempo2Distancia(paquete[1].tiempo);

      // Verificamos si las dos medidas son correctas
      if (((medida[0] > 10) && (medida[0] < 1000)) &&
          ((medida[1] > 10) && (medida[1] < 1000))) {

        switch (algoritmo) {
        case KALMAN1: {
          filtroX = myFilterX.getFilteredValue(medida[0]);
          filtroY = myFilterY.getFilteredValue(medida[1]);
          if (salida) {
            Serial1.print(filtroX);
            Serial1.print(",");
            Serial1.print(filtroY);
          }
          break;
        }
        case KALMAN2: {
          input[0] = medida[0];
          input[1] = medida[1];
          fk.update(input);
          out = fk.getEstimation();
          if (salida) {
            Serial1.print(out[0]);
            Serial1.print(",");
            Serial1.print(out[1]);
          }
          break;
        }
        }
        if (salida == SALIDA_COMPLETA) {
          Serial.print(paquete[0].id_baliza);
          Serial.print(" - ");
          Serial.print(paquete[1].id_baliza);
          Serial.print(" - ");
          Serial.print(paquete[0].tiempo);
          Serial.print(" - ");
          Serial.print(paquete[1].tiempo);
          Serial.print(" - ");
          Serial.print(filtroX);
          Serial.print(" - ");
          Serial.println(filtroY);
        } else if (salida)
          Serial1.println("");

        return true;
      }
    }
  }
  return false;
}

float RecuperarMedida() {
  sPaquete paquete;
  // radio.write(&paquete, sizeof(paquete));
  EnviarMensaje433Mhz(0);
  EnviarPulsoUltrasonidos(SensorEmisor);

  radio.startListening();

  unsigned long started_waiting_at = millis();
  bool timeout = false;

  float medida;
  while ((!radio.available()) && !timeout) // Esperamos
  {
    if (millis() - started_waiting_at > 80) {
      timeout = true;
      //Serial.println("err");
    }
  }

  if (!timeout) { // Leemos el mensaje recibido
    unsigned long got_time;
    radio.read(&paquete, sizeof(sPaquete));
    medida = 1.0 * paquete.tiempo / 29.2; // convertimos a distancia, en cm
  }
  radio.stopListening();

  return medida;
}

// Recupera medida de todas las balizas

boolean RecuperarMedidaMultiple(int emisor) {
  paquete[0].id_baliza = 0;
  paquete[1].id_baliza = 0;
  // radio.write(&paquete, sizeof(paquete));
  EnviarMensaje433Mhz(0);
  EnviarPulsoUltrasonidos(emisor);

  radio.startListening();

  unsigned long started_waiting_at = millis();
  bool timeout = false;

  float medida;
  int numero_medidas = 0;
  while (!timeout && (numero_medidas < NUM_MEDIDAS_RECUPERAR)) // Esperamos
  {
    while ((!radio.available()) && !timeout) // Esperamos
    {
      if (millis() - started_waiting_at > TIMEOUT_MEDIDA_US) {
        timeout = true;
         //Serial.println("err");
      }
    }

    if (!timeout) { // Leemos el mensaje recibido
      unsigned long got_time;
      radio.read(&paquete[numero_medidas], sizeof(sPaquete));
      // medida = 1.0 * paquete[numero_medidas].tiempo / 29.2;   //convertimos a
      // distancia, en cm paquete[numero_medidas].tiempo = medida;
      Serial1.print("medida ");
      Serial1.println(paquete[numero_medidas].id_baliza);
      numero_medidas++;
    }
  }

  if (!timeout)
  {
    int baliza2, baliza1;
    if (paquete[0].id_baliza == 1)
    {
        baliza1 = 0;
        baliza2 = 1;      
    }
    else
    {
        baliza1 = 1;
        baliza2 = 0;
    }
      Serial1.print("lectura: ");
      Serial1.print(paquete[baliza2].tiempo);Serial1.print(" - ");Serial1.print(paquete[baliza2].id_baliza);Serial1.print(" , ");
      Serial1.print(paquete[baliza1].tiempo);Serial1.print(" - ");Serial1.println(paquete[baliza1].id_baliza);
  }

  radio.stopListening();

  return timeout;
}

#define SIN_LECTURA 255
void RecuperarOrientacion() {
  sPaquete paquete[NUM_MEDIDAS_RECUPERAR][MAX_EMISORES_ULTRASONIDOS];
  // radio.write(&paquete, sizeof(paquete));

  // Recuperamos las medidas desde todos los sensores

  for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS; emisor++)
    for (int baliza = 0; baliza < NUM_MEDIDAS_RECUPERAR; baliza++)
      paquete[baliza][emisor].id_baliza = SIN_LECTURA;

  for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS; emisor++) {
    EnviarMensaje433Mhz(0);
    EnviarPulsoUltrasonidos(emisor);

    radio.startListening();

    unsigned long started_waiting_at = millis();
    bool timeout = false;

    float medida;
    int numero_medidas = 0;
    while (!timeout && (numero_medidas < NUM_MEDIDAS_RECUPERAR)) // Esperamos
    {
      while ((!radio.available()) && !timeout) // Esperamos
      {
        if (millis() - started_waiting_at > TIMEOUT_MEDIDA_US)
          timeout = true;
      }

      if (!timeout) { // Leemos el mensaje recibido
        unsigned long got_time;
        radio.read(&paquete[numero_medidas][emisor], sizeof(sPaquete));
        numero_medidas++;
      }
    }
    radio.stopListening();

    delay(50);
  }

  for (int emisor = 0; emisor < MAX_EMISORES_ULTRASONIDOS; emisor++) {
    Serial.print("Emisor: ");
    Serial.println(emisor);

    for (int baliza = 0; baliza < NUM_MEDIDAS_RECUPERAR; baliza++) {
      if (paquete[baliza][emisor].id_baliza != SIN_LECTURA) {
        Serial.print("  ->Baliza: ");
        Serial.print(paquete[baliza][emisor].id_baliza);
        Serial.print(", Distancia: ");
        Serial.print(Tiempo2Distancia(paquete[baliza][emisor].tiempo));
        Serial.print(", Sensor: ");
        Serial.println(paquete[baliza][emisor].num_sensor);
      }
    }
  }
  Serial.println("-----------------");
  return;
}

void EnviarPulsoUltrasonidos(int emisor) {
  long tiempo;
  int distanceCm;
  if (emisor != EMISION_CONJUNTA) {
    digitalWrite(aTrigger[emisor],
                 LOW); /* Por cuesti?n de estabilizaci?n del sensor*/
    delayMicroseconds(5);
    digitalWrite(aTrigger[emisor], HIGH); /* env?o del pulso ultras?nico*/
    delayMicroseconds(15);
    digitalWrite(aTrigger[emisor], LOW); /* env?o del pulso ultras?nico*/
  } else {
    for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++)
        digitalWrite(aTrigger[i], LOW); /* Por cuestion de estabilizacion del sensor*/
    delayMicroseconds(5);
    for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++)
        digitalWrite(aTrigger[i], HIGH); /* envio del pulso ultrasonico*/
    delayMicroseconds(15);
    for (int i = 0; i < MAX_EMISORES_ULTRASONIDOS; i++)
        digitalWrite(aTrigger[i], LOW); /* envio del pulso ultrasonico*/
  }

  /*
  tiempo = pulseIn(ECHO1, HIGH)+1;

  distanceCm = tiempo * 10 / 292/ 2;   //convertimos a distancia, en cm

  Serial.println(distanceCm);
  */
}

bool Espera(unsigned long ms, int contador, int op) {
#define MAX_US 4294967295

  static unsigned long us_ini[10] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
  unsigned long us_actual[10];

  us_actual[contador] = micros();

  if (op == OP_INICIAR)
    us_ini[contador] = micros();
  else {
    if (us_actual[contador] < us_ini[contador]) {
      us_actual[contador] = MAX_US - us_ini[contador] + us_actual[contador];
      us_ini[contador] = 1;
    }

    if (us_actual[contador] - us_ini[contador] >= ms * 1000) {
      us_ini[contador] = 0;
      return false;
    } else
      return true;
  }
}

void EnviarMensaje433Mhz(int lectura) {
  paqueteRF433.id_robot = ID_ROBOT_RODI;
  paqueteRF433.id_robot = ID_OPERACION_MEDIDA;

  // digitalWrite(13, true); // Flash a light to show transmitting

  vw_send((uint8_t *)&paqueteRF433, strlen(msg));
  vw_wait_tx(); // Wait until the whole message is gone
}

void InicializarDatosBalizas(struct sBaliza *Balizas) {
  Balizas[0].id_baliza = 0;
  Balizas[0].id_habitacion = 0;

  return;
}

int InterseccionCircunferencias(int iD1, int iD2, struct sBaliza BalizaAnt,
                               
                                struct sBaliza BalizaSig, long &lPosX,
                               
                                long &lPosY) {
  float fA1, fB1, fC1, fA2, fB2, fC2;
  double D, E, F, a, b, c, d, x1, x2, y1, y2;
  double a1, b1, r1, a2, b2, r2;
  long v1x1, v1y1, v1x2, v1y2;

  // Calculamos la distancia entre los dos puntos
  v1x1 = BalizaAnt.X;
  v1y1 = BalizaAnt.Y;

  v1x2 = BalizaSig.X;
  v1y2 = BalizaSig.Y;

  a1 = v1x1;
  b1 = v1y1;
  a2 = v1x2;
  b2 = v1y2;
  r1 = iD1;
  r2 = iD2;

  fA1 = -2 * a1;
  fB1 = -2 * b1;
  fC1 = sq(a1) + sq(b1) - sq(r1);
  fA2 = -2 * a2;
  fB2 = -2 * b2;
  fC2 = sq(a2) + sq(b2) - sq(r2);

  D = fA1 - fA2;
  E = fB1 - fB2;
  F = fC1 - fC2;

  a = sq(E) / sq(D) + 1;
  b = (2 * E * F - D * fA1 * E + sq(D) * fB1) / sq(D);
  c = sq(F) / sq(D) - fA1 * F / D + fC1;

  d = (sq(b) - 4 * a * c);

  if (d >= 0) {
    y1 = (-b + sqrt(d)) / (2 * a);
    x1 = (-E * y1 - F) / D;
    y2 = (-b - sqrt(d)) / (2 * a);
    x2 = (-E * y2 - F) / D;

    if (y1 == y2) {
      lPosX = x1;
      lPosY = y1;
    } else // Debemos escoger la soluci?n correcta
    {
      lPosX = x1;
      lPosY = y1;
    }

    return 1;
  } else {
    // No se actualizan los valores
    return 0;
  }
}

int freeRam() {
  extern int __heap_start, *__brkval;
  int v;
  return (int)&v - (__brkval == 0 ? (int)&__heap_start : (int)__brkval);
}

void PruebaBluetooth() {
  // Si hay datos disponibles en el monitor serie
  while (Serial.available()) {
    // Escribimos los valores en el m?dulo bluetooth HC-06
    Serial1.write(Serial.read());
  }
  // Si hay datos disponibles en el m?dulo bluetooth HC-06
  while (Serial1.available()) {
    // Mostramos los valores en el monitor serie
    Serial.write(Serial1.read());
  }
}

char Comando[10] = "";
HardwareSerial Puerto = Serial1;

void EjecutarComando() {
  if (comando.substring(0, 3) == "son") {
    if (comando.length() != 4)
      return;
    int Sensor = comando.substring(3).toInt();
    //Los sensores se numeran del 1-8. Si es un 0 se activan todos, sino se activa solo el que se indica
    if (Sensor == 0)
      SensorEmisor = EMISION_CONJUNTA;
    else
      SensorEmisor = Sensor-1;

    Serial.print("Ejec: son");
    Serial.println(Sensor);
  } 
  else if (comando.substring(0, 2) == "md") {
  // Arranque motor derecho
    if (comando.length() != 5)
      return;
    PararMotores = false;
    int Velocidad = comando.substring(2).toInt();
    Motor[DERECHA].write(Velocidad);
  } else if (comando.substring(0, 2) == "mi") {
    if (comando.length() != 5)
      return;
    PararMotores = false;
    int Velocidad = comando.substring(2).toInt();
    Motor[IZQUIERDA].write(Velocidad);
  } else if (comando == "stop") {
    if (comando.length() != 4)
      return;
    PararMotores = true;
    for (int i = 0; i < 2; i++)
      Motor[i].write(MOTOR_D_PARADO);
    delay(100);
    // PararMotores = true;
    contador = TIMER2_OVF;
  } else if (comando == "pause") {
    if (comando.length() != 5)
      return;
    PararMotores = true;
  } else if (comando == "resume") {
    if (comando.length() != 6)
      return;
    contador = TIMER2_OVF;
    PararMotores = false;
  } else if (comando.substring(0, 2) == "kl") {
    if (comando.length() != 10)
      return;
    double k1 = comando.substring(2, 5).toInt() * 1.0 / 1000;
    double k2 = comando.substring(6, 9).toInt() * 1.0 / 1000;

    fk.init(k1, k2);
  } else if (comando.substring(0, 2) == "al") {
    if (comando.length() != 3)
      return;
    algoKalman = comando.substring(2).toInt();
  } else if (comando == "clearodo") {
    Odo[DERECHA].pulsos_totales = Odo[IZQUIERDA].pulsos_totales = 0;
  }
}

void RecuperarComandos() {
  // Si hay datos disponibles en el m?dulo bluetooth HC-06
  while (Serial1.available()) {
    char c[2] = {0, 0};
    c[0] = Serial1.read();
    if (c[0] != '.') {
      comando.concat(c);
    } else {
      EjecutarComando();
      comando = "";
    }
  }
}

void MoverRueda(byte rueda, byte direccion, int velocidad) {
  if (velocidad > 255)
    velocidad = 255;

  if (rueda == DERECHA) {
    if (direccion == ADELANTE) {
    } else {
    }
  } else {
    if (direccion == ADELANTE) {
    } else {
    }
  }
}

void Tono(int PWM, int ms) {
  analogWrite(TONE, PWM);
  delay(ms);
  analogWrite(TONE, 0);
}

void CambioMovimiento(byte Estado) {
  MoverRueda(IZQUIERDA, ATRAS, VELOCIDAD_RAPIDA);
  MoverRueda(DERECHA, ATRAS, VELOCIDAD_RAPIDA);
  delay(MS_FRENADA);
  MoverRueda(IZQUIERDA, ADELANTE, PARADO);
  MoverRueda(DERECHA, ADELANTE, PARADO);

  if (PITIDO_ESTADOS) {
    for (int i = 0; i < Estado; i++) {
      Tono(200, 50);
      delay(200);
      Tono(200, 0);
      delay(200);
    }
    Tono(200, 0);
  } else {
    Tono(200, 50);
  }
  delay(MS_ESPERA);
}

void RecuperarMedidasMultiples() {
  float minimo, medida[2], media, maximo, filtroX, filtroY;
  int medidas, id_baliza_x = 0, id_baliza_y = 0;
  boolean timeout;
  sMedidas MedidaBaliza;

  minimo = 999;
  maximo = 0;
  media = 0;
  medidas = 0;

  // Recuperamos todas las medidas de todas las balizas que hayan contestado
  // Las medidas est?n en el array paquetes con numero_medidas unidades
  int numero_medidas = LeerMedidasSensores(EMISION_CONJUNTA);

  for (int i = 0; i < numero_medidas; i++) {
    bool BalizaNueva = false;
    sMedidas *pMedidasBaliza = ListaMedidasBalizas.search_code(
        paquete[i].id_baliza); // Localizamos la zona de medidas de la baliza

    sMedida Medida;
    Medida.tiempo_local = paquete[i].tiempo_local; // micros() del reloj actual
    Medida.tiempo_vuelo =
        paquete[i]
            .tiempo; // Tiempo de vuelo de la se?al recuperada por la baliza
    Medida.num_sensor =
        paquete[i].num_sensor; // N?mero de transductor de la baliza que ha
                               // captado antes la se?al de US

    if (pMedidasBaliza ==
        NULL) { // No se ha localizado la zona de medidas, es una baliza nueva
      sMedidas MedidasBaliza;
      MedidasBaliza.codigo = MedidasBaliza.id_baliza = paquete[i].id_baliza;
      MedidasBaliza.tiempo_vida =
          millis(); // Inicio de tiempo de vida de la primera detecci?n de una
                    // se?al de esta baliza
      ListaMedidasBalizas.add_head(MedidasBaliza);
      MedidasBaliza.MedidasCorrectas = 0; // Inicializamos las medidas correctas

      pMedidasBaliza = &MedidasBaliza;
      BalizaNueva = true;
    }
    // Ahora comprobamos los m?rgenes de la medida
    // 1. Control de m?rgenes
    pMedidasBaliza->numero_medidas++;
    pMedidasBaliza->tiempo_ultima_medida = millis();
    if ((Medida.tiempo_vuelo < MIN_CONTROL_TIEMPO_VUELO) &&
        (Medida.tiempo_vuelo > MAX_CONTROL_TIEMPO_VUELO)) { // Medida err?nea
      pMedidasBaliza->FallosTotales++;
      pMedidasBaliza->FallosAcumulados++;
    } else // Medida correcta, ajustamos la altura
    {
      // Localizamos la informaci?n de la baliza en la Lista de Balizas para
      // recuperar su altura en eje Z
      long altura_baliza = ListaBalizas.search_code(paquete[i].id_baliza)->Z;
      // Los sensores rel robot, los sensores de la baliza y la recta recorrida
      // por el sonido forma un tri?ngulo rect?ngulo
      float h = Medida.tiempo_vuelo;
      float Altura = abs(altura_baliza - ROBOT_ALTURA_SUELO_SENSORES);
      float distancia_ajustada_altura = pow(h, 2) - pow(Altura, 2);

      if (BalizaNueva) { // Si cumple los m?rgenes, al ser una baliza nueva se
                         // inserta la medida directamente (no tenemos datos que
                         // cruzar)
        pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS, Medida);
        pMedidasBaliza->MedidasCorrectas++;
      } else
      // 2. Control de medias.
      //	 Debemos comprobar la media de variaci?n de las ?ltimas 5
      //medidas
      {
        // Hemos excedido el tiempo de inactividad sin detectar ninguna medida
        // de la baliza
        if (abs(pMedidasBaliza->tiempo_vida -
                pMedidasBaliza->tiempo_ultima_medida) >
            MAX_TIEMPO_INACTIVIDAD) { // Debemos ignorar todas las medidas
                                      // anteriores e inicializar todos los
                                      // contadores
          pMedidasBaliza->tiempo_vida = pMedidasBaliza->tiempo_ultima_medida;
          pMedidasBaliza->Medida.del_all();
          pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS, Medida);
          pMedidasBaliza->numero_ultima_medida =
              ProcesoMedida.numero_total_medidas;
          pMedidasBaliza->Inicializar();
        } else // La medida se encuentra dentro del margen temporal
        {
          int numero_medidas = pMedidasBaliza->Medida.m_num_nodes;

          // Reseteamos los valores si se ha reseteado el contador de micros()
          // porque no se pueden comparar las medidas
          if (numero_medidas)
            if (pMedidasBaliza->Medida.search(0)->tiempo_vuelo >
                Medida.tiempo_local) {
              pMedidasBaliza->Medida.del_all();
              pMedidasBaliza->Medida.add_head_windows(NUM_MEDIDAS_BALIZAS,
                                                      Medida);
              pMedidasBaliza->numero_ultima_medida =
                  ProcesoMedida.numero_total_medidas;
              pMedidasBaliza->Inicializar();
            } else {
              // Solo realizamos el control de medias si tenemos como minimo
              // NUM_MEDIDAS_VALIDAS Obtenemos la pendiente media con respecto al
              // tiempo Obtenemos la media de las medidas almacenadas La
              // pendiente entre la medida media y la actual no debe superar en 3
              // veces la pendiente media
              if (numero_medidas >= NUM_MEDIDAS_VALIDAS) {
                sPaquete Medidas[NUM_MEDIDAS_BALIZAS];
                double Delta[NUM_MEDIDAS_BALIZAS]; // Pendientes de variaci?n de
                                                   // distancia con respecto al
                                                   // tiempo
                int numeros_deltas = 0;
                double Media_delta;
                double Media_tiempo;
                long ultimo_tiempo_vuelo; // Ultimo valor de tiempo de vuelo
                long ultimo_tiempo_local; // Ultimo valor de tiempo de vuelo

                // La medida mas nueva es la primera. invertimos el array
                for (int i = numero_medidas - 1; i; i--) {
                  Media_tiempo += Medidas[i].tiempo =
                      pMedidasBaliza->Medida.search(i)->tiempo_vuelo;
                  Medidas[i].tiempo_local =
                      pMedidasBaliza->Medida.search(i)->tiempo_local;
                }
                for (int i = 1; i < numero_medidas; i++) {
                  double tiempo_entre_medidas =
                      Medidas[i].tiempo_local - Medidas[i - 1].tiempo_local;
                  double diferencia_entre_medidas =
                      abs(Medidas[i].tiempo - Medidas[i - 1].tiempo);
                  if (tiempo_entre_medidas >
                      0) // Si se ha reseteado el contado de micros() ignoramos
                         // la medida
                    Media_delta +=
                        (Delta[numeros_deltas++] =
                             diferencia_entre_medidas / tiempo_entre_medidas);
                }
                Media_delta /= numeros_deltas; // Media de variaci?n ponderada
                Media_tiempo /= numero_medidas;

                if (abs(ultimo_tiempo_vuelo - Media_tiempo) /
                        (Medida.tiempo_local - ultimo_tiempo_local) >
                    Media_delta * MARGEN_MEDIDA_DELTA) {
                  pMedidasBaliza->Medida.add_head_windows(
                      NUM_MEDIDAS_BALIZAS,
                      Medida); // Mantenemos las ?ltimas NUM_MEDIDAS_BALIZAS
                               // medidas
                  pMedidasBaliza->numero_ultima_medida =
                      ProcesoMedida.numero_total_medidas;
                  pMedidasBaliza->MedidasCorrectas++;
                } else
                  pMedidasBaliza->FallosTotales++;
              }
            }
        }
      }
    }
  } // for balizas

  byte BalizasDetectadasUltimaMedida[2] = {0, 0};
  byte numero_balizas = 0;
  // Primero comprobamos si hemos recuperado informaci?n de las dos balizas
  // activas seleccionadas para la medida
  if (ProcesoMedida.num_balizas == 2) {
    for (int i = 0; i < ListaMedidasBalizas.m_num_nodes; i++) {
      if (ListaMedidasBalizas.search(i)->numero_ultima_medida ==
          ProcesoMedida.numero_total_medidas) {
        if ((ProcesoMedida.BalizasSeleccionadas[0] ==
             ListaMedidasBalizas.search(i)->id_baliza)) {
          BalizasDetectadasUltimaMedida[0] =
              ProcesoMedida.BalizasSeleccionadas[0];
          numero_balizas++;
        }
        if ((ProcesoMedida.BalizasSeleccionadas[1] ==
             ListaMedidasBalizas.search(i)->id_baliza)) {
          BalizasDetectadasUltimaMedida[1] =
              ProcesoMedida.BalizasSeleccionadas[0];
          numero_balizas++;
        }
      }
    }
    // Comprobamos si seguimos detectando las mismas balizas
    if (numero_balizas == 2) {
      // Inicializamos el contador de errores acumulados de lectura
      ProcesoMedida.ErroresBalizasSeleccionadas[0] =
          ProcesoMedida.ErroresBalizasSeleccionadas[1] = 0;
      // Procedemos a triangular la posici?n
    } else // No hemos podido localizar las dos balizas
    {
      for (int i = 0; i < 2; i++) {
        if (!BalizasDetectadasUltimaMedida[i]) {
          ProcesoMedida.ErroresBalizasSeleccionadas[i]++;
          if (ProcesoMedida.errores_acumulados >=
              ERRORES_CAMBIO_BALIZA) { // Hemos excedido el m?ximo n?mero de
                                       // errores de la baliza, debemos buscar
                                       // otra
          }
        }
      }
    }
  }

  delay(5); // Retardo entre medidas
}

void intRueda_der() {
  int lecturaAct = digitalRead(RUEDA_DER_ODO);
  ContarPulsoRueda(DERECHA, lecturaAct);
}

void intRueda_izq() {
  int lecturaAct = digitalRead(RUEDA_IZQ_ODO);

  ContarPulsoRueda(IZQUIERDA, lecturaAct);
}
#define TIEMPO_RETARDO_US 10000

enum eEstadoTrans : byte { validada, pendiente, no_activa, espera };

struct sTransicion {
  long tiempo_us;
  char lectura;
  eEstadoTrans estado;
};
#define SIN_TRANSICION -1
#define MIN_TIEMPO_PULSO 6000        // us
#define MIN_TIEMPO_PULSO_VALIDO 7000 // us

/*
        * Si estoy leyendo el valor de pulso X y tengo una se?al <> X
                1. Si no hay activa transici?n, la activo
                2. Si hay activa transici?n, sigue contando el tiempo de
   transici?n
    * Si estoy leyendo el valor de pulso X y tengo una se?al == X
                1. Si no hay activa transici?n, no hago nada sigo leyendo X
                2. Si hay activa una transcion

*/
sTransicion uno[2] = {{0, 0, no_activa}, {0, 0, no_activa}},
            cero[2] = {{0, 0, no_activa}, {0, 0, no_activa}};

#define TIEMPO_PULSO 60000
#define TIEMPO_PULSO_INTERFERENCIA 4000
#define MIN_TIEMPO 1000

void ContarPulsoRueda(int rueda, int lecturaAct) {

  bool ini_pulso = false;
  long tiempo_us = micros();
  long tiempo_ms = millis();

  sTransicion *tmp_uno, *tmp_cero;
  if (lecturaAct == LOW) {
    tmp_uno = &uno[rueda];
    tmp_cero = &cero[rueda];
  } else {
    tmp_uno = &cero[rueda];
    tmp_cero = &uno[rueda];
  }

  if (tmp_cero->estado == no_activa) {
    // Se inicializa el pulso
    tmp_cero->tiempo_us = tiempo_us;
    tmp_cero->estado = pendiente;
  }

  if (tmp_uno->estado == pendiente) {
    ini_pulso = false;
    // Si el largo de uno es suficiente para detectar un pulso se detecta
    long tiempo = tiempo_us - tmp_uno->tiempo_us;
    if (tiempo > TIEMPO_PULSO) {
      Odo[rueda].Contador[lecturaAct].pulsos++;
      Odo[rueda].Contador[lecturaAct].Tiempo =
          tiempo_ms - Odo[rueda].Contador[lecturaAct].Ultimo_ms;
      Odo[rueda].Contador[lecturaAct].Ultimo_ms = tiempo_ms;

      Odo[rueda].pulsos_totales++;
      Odo[rueda].tiempo_total = tiempo_ms - Odo[rueda].Ultimo_ms_total;
      Odo[rueda].Ultimo_ms_total = tiempo_ms;

      ini_pulso = true;
      tmp_uno->estado = no_activa;
    }
    // Si el largo de uno es mas grande que una interferencia
    else if (tiempo < TIEMPO_PULSO_INTERFERENCIA) {
      ini_pulso = false;
      tmp_uno->estado = no_activa;
    }
    // Siempre que no se detecte una interferencia se inicializa el pulso
    // contrario
    if (ini_pulso) {
      // Se inicializa el pulso contrario
      tmp_cero->tiempo_us = tiempo_us;
      tmp_cero->estado = pendiente;
    }
  }
}

// ***************** No empleadas *******************************
void ControlVelocidad(int rueda) {
  static long lastTime = 0;
  long now = millis();
  long timeChange = (now - lastTime);

  long contador;
  long ultimoTimer;
  long Timer;
  Input[rueda] = Odo[rueda].tiempo_total;
  Input[rueda] = klmRev[rueda].getFilteredValue(Input[rueda]);
  if (timeChange >= SAMPLE_TIME) {
    lastTime = now;

    Pid[rueda].Compute();
    // if (rueda == 0)
    //	MotorD.write(Output[rueda]);
    // else
    //	MotorI.write(Output[rueda]);
  }
}
