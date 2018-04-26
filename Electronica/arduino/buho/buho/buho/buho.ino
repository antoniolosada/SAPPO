#include <Servo.h>

Servo Servo1;
Servo Servo2;

void setup() {
  // put your setup code here, to run once:
  Servo1.attach(18); //derecha mirando de frente
  Servo2.attach(45); //Izquierda mirando de frente

}

void loop() {
  // put your main code here, to run repeatedly: 

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
