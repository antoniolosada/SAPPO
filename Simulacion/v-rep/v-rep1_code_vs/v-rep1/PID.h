extern int SampleTime; // Seteamos el tiempo de muestreo en 1 segundo.
extern double Input, Output, Setpoint;
extern double kp, ki, kd;

void Compute();
void SetTunings(double Kp, double Ki, double Kd);
