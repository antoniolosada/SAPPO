#include "iostream"
#include <time.h>

using namespace std; 


// Variables utilizadas en el controlador PID.
unsigned long lastTime = 0;
double errSum = 0, lastErr = 0;
int SampleTime = 500; // Seteamos el tiempo de muestreo en 1 segundo.
double Input, Output, Setpoint;
double kp = 2, ki = 5, kd = 1;

long millis()
{
	return clock();
}

void Compute()
{
	unsigned long now = millis();
	if (!lastTime) lastTime = now - SampleTime;

	int timeChange = (now - lastTime);

	// Calcula todas las variables de error.
	double error = Setpoint - Input;
	errSum += error;
	double dErr = (error - lastErr);
	// Calculamos la función de salida del PID.
	Output = kp * error + ki * errSum + kd * dErr;
	// Guardamos el valor de algunas variables para el próximo ciclo de cálculo.
	lastErr = error;
	lastTime = now;

	//cout << error << "," << Output << endl;
}
/* Establecemos los valores de las constantes para la sintonización.
Debido a que ahora sabemos que el tiempo entre muestras es constante,
no hace falta multiplicar una y otra vez por el cambio de tiempo; podemos
ajustar las constantes Ki y Kd, obteniendose un resultado matemático equivalente
pero más eficiente que en la primera versión de la función. */
void SetTunings(double Kp, double Ki, double Kd)
{
	double SampleTimeInSec = ((double)SampleTime) / 1000;
	kp = Kp;
	ki = Ki * SampleTimeInSec;
	kd = Kd / SampleTimeInSec;
}
void SetSampleTime(int NewSampleTime)
{
	if (NewSampleTime > 0)
	{
		/* si el usuario decide cambiar el tiempo de muestreo durante el funcionamiento, Ki y Kd tendrán
		que ajustarse para reflejar este cambio. */
		double ratio = (double)NewSampleTime / (double)SampleTime;
		ki *= ratio;
		kd /= ratio;
		SampleTime = (unsigned long)NewSampleTime;
	}
}
