using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trayectoria
{
    public class ComunicacionPuertoSerie
    {
        public static int TipoMedida = 1;
        public delegate void RecibidaLocalizacion(double PosX, double PosY, long[] Distancias, int SensorBal1, int SensorBal2, int op);
        public long NumeroLecturas = 0;

        const int MAX_SENSORES = 12;
        long[] Distancias = new long[MAX_SENSORES+1];
        public RecibidaLocalizacion mRecibidaLectura;
        public double DistBal1 = 0;
        public double DistBal2 = 0;
        public double Bal1p = 0;
        public double Bal2p = 0;
        public long Medidas = 0;
        public Timer tmrPosicionAviso;
        // Nueva instancia de la clase
        public SerialPort serialPort = new SerialPort();
        public String Lectura = "";

        public ComunicacionPuertoSerie(String puerto, RecibidaLocalizacion LocalizacionOK)
        {
            mRecibidaLectura = LocalizacionOK;
            // Asignamos las propiedades
            serialPort.BaudRate = 38400;
            serialPort.PortName = puerto;

            // Creamos el evento
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(SerialPort_ErrorReceived);

            // Controlamos que el puerto indicado esté operativo
            try
            {
                // Abrimos el puerto serie
                serialPort.Open();
            }
            catch
            {

            }

        }
        public void CerrarPuerto()
        {
            serialPort.Close();
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine(e.ToString());
            SerialPort Puerto = (SerialPort)sender;
            Puerto.Close();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String Bal1 = "";
            String Bal2 = "";
            try
            {
                // Obtenemos el puerto serie que lanza el evento
                SerialPort currentSerialPort = (SerialPort)sender;

                // Leemos el dato recibido del puerto serie
                string inData = currentSerialPort.ReadLine();
                if (inData != "")
                {
                    NumeroLecturas++;
                    Lectura = inData;
                    if (Lectura.Substring(0, 4) == "BAL:")
                    {
                        Lectura = Lectura.Substring(4);
                        String[] DistanciasBal = Lectura.Split(',');
                        //[0],[1] -> Media, [2],[3] -> Kalman, [4][5] Odo der, izq, [6][7] sensor bal1, bal2

                        if (TipoMedida == 1)
                        {
                            Bal1 = DistanciasBal[0];
                            Bal2 = DistanciasBal[1];
                        }
                        else
                        {
                            Bal1 = DistanciasBal[2];
                            Bal2 = DistanciasBal[3];
                        }
                        Bal1 = Bal1.Replace('.', ',');
                        Bal2 = Bal2.Replace('.', ',');
                        double dBal1 = Convert.ToDouble(Bal1);
                        double dBal2 = Convert.ToDouble(Bal2);
                        int SensorBal1 = -1;
                        int SensorBal2 = -1; 
                        Medidas++;
                        //Console.WriteLine(Medidas + "- X=" + PosX + ",Y=" + PosY);
                        Bal1p = 0;
                        Bal2p = 0;
                        if (DistanciasBal.Length > 2)
                            Bal1p = Convert.ToDouble(DistanciasBal[2].Replace('.', ','));
                        if (DistanciasBal.Length > 3)
                            Bal2p = Convert.ToDouble(DistanciasBal[3].Replace('.', ','));
                        DistBal1 = dBal1;
                        DistBal2 = dBal2;
                        SensorBal1 = Convert.ToInt16(DistanciasBal[6]);
                        SensorBal2 = Convert.ToInt16(DistanciasBal[7]);
                        mRecibidaLectura(DistBal1, DistBal2, Distancias, SensorBal1, SensorBal2, frmTrayectoria.OP_POSICION); // "-> RecibidaPosicion()"
                    }
                    else if (Lectura.Substring(0, 4) == "ORI:")
                    {
                        Lectura = Lectura.Substring(4);
                        String[] Orientacion = Lectura.Split(',');
                        mRecibidaLectura(Convert.ToDouble(Orientacion[0].Replace('.', ',')), Convert.ToDouble(Orientacion[1]), Distancias, 0, 0, frmTrayectoria.OP_ORIENTACION); // "-> RecibidaPosicion()"
                    }
                    else if (Lectura.Substring(0, 4) == "SON:")
                    {
                        Lectura = Lectura.Substring(4);
                        String[] DistSensores = Lectura.Split(',');
                        for (int i = 0; i < DistSensores.Length; i++)
                            Distancias[i] = Convert.ToInt64(DistSensores[i]);

                        mRecibidaLectura(0, 0, Distancias, 0, 0, frmTrayectoria.OP_SONAR); // "-> RecibidaPosicion()"
                    }
                    else if (Lectura.Substring(0, 4) == "POS:")
                    {
                        Lectura = Lectura.Substring(4);

                        String[] Orientacion = Lectura.Split(',');
                        mRecibidaLectura(Convert.ToDouble(Orientacion[0].Replace('.', ',')), Convert.ToDouble(Orientacion[1]), Distancias, 0, 0, frmTrayectoria.OP_ORIENTACION); // "-> RecibidaPosicion()"
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERR:" + ex.ToString());
            }
        }
    }
}
