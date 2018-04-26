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
        public delegate void RecibidaLocalizacion(double PosX, double PosY);

        public RecibidaLocalizacion mRecibidaLocalizacion;
        public double PosX = 0;
        public double PosY = 0;
        public long Medidas = 0;
        public Timer tmrPosicionAviso;
        // Nueva instancia de la clase
        public SerialPort serialPort = new SerialPort();
        public String Lectura = "";

        public ComunicacionPuertoSerie(String puerto, RecibidaLocalizacion LocalizacionOK)
        {
            mRecibidaLocalizacion = LocalizacionOK;
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
            String CoorX = "";
            String CoorY = "";
            try
            {
                // Obtenemos el puerto serie que lanza el evento
                SerialPort currentSerialPort = (SerialPort)sender;

                // Leemos el dato recibido del puerto serie
                string inData = currentSerialPort.ReadLine();
                if (inData != "")
                {
                    Lectura = inData;
                    String[] Coordenadas = Lectura.Split(',');
                    CoorX = Coordenadas[0];
                    CoorY = Coordenadas[1];
                    CoorX = CoorX.Replace('.', ',');
                    CoorY = CoorY.Replace('.', ',');
                    double dPosX = Convert.ToDouble(CoorX);
                    double dPosY = Convert.ToDouble(CoorY);
                    Medidas++;
                    //Console.WriteLine(Medidas + "- X=" + PosX + ",Y=" + PosY);
                    PosX = dPosX;
                    PosY = dPosY;
                    mRecibidaLocalizacion(PosX, PosY);
                    //Console.WriteLine(PosX + ", " + PosY);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(CoorX + "," + CoorY);
            }
        }
    }
}
