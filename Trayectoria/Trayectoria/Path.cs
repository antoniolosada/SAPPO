using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using remoteApiNETWrapper;
using System.Threading;

/*
 * Si las balizas están conectadas a la placa arduino hay que arrancar monitores serie para que funcionen
 * Arrancar monitor serie de RODI
 */

namespace Trayectoria
{

    public partial class frmTrayectoria : Form
    {
        bool Simulador = true;
        const int MAX_SENSORES_US = 12;

        public const double LONG_APOTEMA_BALIZA = 7.541;
        public const double LONG_APOTEMA_ROBOT = 2.897;
        public const double AJUSTE_X = -14;
        public const double AJUSTE_Y = -14;

        public const int OP_POSICION = 0;
        public const int OP_ORIENTACION = 1;
        public const int OP_SONAR = 3;

        static System.Windows.Forms.Timer stmrControlPosicion;
        delegate void PosicionRecibida();

        const int MS_CONTROL =2;
        const int MS_CONTROL_MEDIDA = 3;

        const int MANUAL = 0;
        const int DIRECT = 0;
        const int REVERSE = 1;

        const int FACTOR_VELOCIDAD = 20;
        const double FACTOR_COMPENSACION_MOTOR_DER = 1;
        const double FACTOR_COMPENSACION_MOTOR_IZQ = 1.20;

        double CoordenadaX = 0;
        double CoordenadaY = 0;

        ComunicacionPuertoSerie Puerto;
        //static double Kp = 2, Ki = 2, Kd = 0.2;
        static double Kp = 2, Ki = 2, Kd = 0.2;
        PID myPID = new PID(Kp, Ki, Kd, DIRECT);

        public bool RecuperandoOrientacion = false;
        bool ConfiguracionRecuperada = false;
        double dBal1 = 0, dBal2 = 0;
        int iSensorBal1 = -1, iSensorBal2 = -1;
        double Orientacion = -1;
        int BalOrientacion = 0;
        int motor_derecho = 0;
        int motor_izquierdo = 1;
        int sensor = 2;
        long simx_opmode_oneshot = 0;
        long simx_opmode_blocking = 0x010000;
        long simx_opmode_oneshot_wait = 0x010000;
        int robot = 0;
        int gclientID =0;
        long NumeroLecturas = 0;

        public const int TIEMPO_RECORRIDO_INICIAL = 2000; //Arramca durante 2s para establecer la dirección
        public const int TIEMPO_ESTABILIZACION_KALMAN = 800;
        public const int CS_CONTROL = 100;
        public const int MAX_MEDIDAS_DIR = 10;
        public const double VELOCIDAD_ANGULAR_ARRANQUE_DER_RODI = 126;
        public const double VELOCIDAD_ANGULAR_ARRANQUE_IZQ_RODI = 90;
        public const double VELOCIDAD_ANGULAR_ARRANQUE_SIM = 0.5;
        public const double PARAR_MOTOR = 999;
        public const double VELOCIDAD_ANGULAR_MIN = 0.0001;
        public double DISTANCIA_SALTO_NODO = 25;
        public double DISTANCIA_NODO_PARA_VEL_MIN = 30;
        public double MODULO_MIN_MEDIDA = 6;
        public double MODULO_MIN_MEDIDA_AVANCE = 6;
        public double MIN_MODULO_ACT_POSICION = 12;

        public const int OP_INICIAR = 0;
        public const int OP_ESPERA = 1;

        public const int MAX_VALOR_OUT_PID = 255;
        public const int FACTOR_SALIDA = 1;
        public double MAX_VEL_ANG = 0;	//Máxima diferencia de velocidad con corrección de trayectoria

        public const int VEL_MIN_RODI = 90;
        public const double MAX_VEL_FIJA_SIM = 0.6;	//Velocidad lineal sin corrección de trayectoria
        public const double MIN_VEL_FIJA_SIM = 0.2; //Velocidad lineal con corrección de trayectoria

        public const double MIN_VEL_RODI_IZQ = 0; //Velocidad lineal con corrección de trayectoria
        public const double MIN_VEL_RODI_DER = 4; //Velocidad lineal con corrección de trayectoria
        public const double MAX_VEL_RODI_DER = 9; //Velocidad lineal con corrección de trayectoria
        public const double MAX_VEL_RODI_IZQ = 7; //Velocidad lineal con corrección de trayectoria

        public const int MIN_GRADOS_RECTA = 15;

        long[] Distancias = new long[MAX_SENSORES_US+1];
        int NumSensoresDistancia = 0;
        List<Button> ListaBotones = new List<Button>();
        Configuration config;
        System.Collections.Specialized.NameValueCollection appSettings;
        const int PUNTO_ALTO = 17;
        const int PUNTO_ANCHO = 17;
        int NodoInicial = -1;
        int NodoFinal = -1;
        Point PuntoInicial;
        Point PuntoFinal;

        int clientID;
        enum Estados { Vacio, InsertarNodos, UnirNodos, EstablecerCamino, NodoInicial, NodoFinal, BuscarRuta, MedirDistancia, DefinirObstaculos, ColocarBaliza };
        Estados Estado = Estados.Vacio;
        Estados SubEstado = Estados.Vacio;

        class PintarVector
        {
            public Point ini, fin;
            public Color color;
        }
        class Medida
        {
            public String IDBaliza;
            public double Distancia;
        };
        struct Nodo
        {
            public int ID;
            public int X;
            public int Y;
            public int tipo; //0-Normal, 1-RutaInicio, 2-RutaFin
        };
        class Enlace
        {
            public int ID1;
            public int ID2;
            public bool ruta;
            public double Distancia = -1;
            public int num_nodo_ruta = -1;
        };
        class Obstaculo
        {
            public Point P1;
            public Point P2;
        };
        class Baliza
        {
            public string Id;
            public int PosX;
            public int PosY;
        };
        int NumeroNodos = 0;
        List<Nodo> ListaNodos = new List<Nodo>();
        List<Enlace> ListaEnlaces = new List<Enlace>();
        List<Obstaculo> ListaObstaculos = new List<Obstaculo>();
        List<Baliza> ListaBalizas = new List<Baliza>();
        List<Nodo> ListaNodosRuta = new List<Nodo>();
        Enlace Union;

        List<PintarVector> ListaVectores = new List<PintarVector>();

        public int MARGEN = 6;

        public frmTrayectoria()
        {
            InitializeComponent();
            stmrControlPosicion = tmrControlPosicion;
        }

        private void picMapa_MouseClick(object sender, MouseEventArgs e)
        {
            Image currentImage = picMapa.Image;
            Bitmap imageAux = new Bitmap(currentImage.Width, currentImage.Height);
            using (Graphics g = Graphics.FromImage(imageAux))
            {
                Brush brush = new SolidBrush(Color.Red);
                Pen pen = new Pen(Color.Black, 10);
                g.DrawImage(currentImage, new Point(0, 0));
                g.Flush();

                Control control = (Control)sender;
                Point punto1 = ((Control)sender).PointToScreen(new Point(e.X, e.Y)); ;
                Point punto = new Point(e.X, e.Y);


                g.DrawRectangle(pen, new Rectangle(punto.X, punto.Y, 50, 50));
                g.FillRectangle(brush, new Rectangle(punto.X, punto.Y, 50, 50));
                g.Dispose();
                picMapa.Image = imageAux;
                picMapa.Invalidate();
            }
            picMapa.Refresh();
        }

        private void picMapa_Click(object sender, EventArgs e)
        {

        }

        private void PintarGrafo()
        {
            //Image currentImage = picMapa.Image;
            Image currentImage = picMapa.InitialImage;
            Bitmap imageAux = new Bitmap(currentImage.Width, currentImage.Height);
            using (Graphics g = Graphics.FromImage(imageAux))
            {
                g.DrawImage(currentImage, new Point(0, 0));
                g.Flush();

                //Pintamos los enlaces
                foreach (Enlace Union in ListaEnlaces)
                {
                    Nodo Nodo1 = ListaNodos.Find(x => x.ID == Union.ID1);
                    Nodo Nodo2 = ListaNodos.Find(x => x.ID == Union.ID2);
                    ColorearVertice(g, Nodo1, Color.Green);
                    ColorearVertice(g, Nodo2, Color.Green);

                    Pen pen;
                    if (Union.ruta)
                        pen = new Pen(Color.Red, 3);
                    else
                        pen = new Pen(Color.Yellow, 3);

                    g.DrawLine(pen, new Point(Nodo1.X, Nodo1.Y), new Point(Nodo2.X, Nodo2.Y));
                }
                //Pintamos los nodos
                foreach (Nodo Punto in ListaNodos)
                {
                    if ((Punto.ID == NodoInicial) || (Punto.ID == NodoFinal))
                        ColorearVertice(g, Punto, Color.LightSalmon);
                    else
                        ColorearVertice(g, Punto, Color.Yellow);
                }
                //Pintamos los obstáculos
                foreach (Obstaculo o in ListaObstaculos)
                {
                    g.DrawLine(new Pen(Color.LightCoral, 2), o.P1, o.P2);
                }
                //Pintamos las balizas
                foreach (Baliza Bal in ListaBalizas)
                {
                    PintarBaliza(g, Bal);
                }
                //Pintamos el robot
                if ((lblCoorX.Text != "") && (lblCoorY.Text != ""))
                {
                    double EscalaX = Convert.ToDouble(tbEscalaX.Text);
                    double EscalaY = Convert.ToDouble(tbEscalaY.Text);
                    int X = 0, Y = 0;
                    if (Simulador)
                    {
                        X = (int)(Convert.ToDouble(lblCoorX.Text) );
                        Y = (int)(Convert.ToDouble(lblCoorY.Text) );
                    }
                    else
                    {//Si no estamos simulando pasamos de cm a mm
                        X = (int)(Convert.ToDouble(lblCoorX.Text) * 10 / EscalaX);
                        Y = (int)(Convert.ToDouble(lblCoorY.Text) * 10 / EscalaY);
                    }
                    Point p = new Point((int)(X), (int)(Y));
                    Bitmap img = new Bitmap("Robot.bmp");
                    g.DrawImage(img, p);
                }
                //Pintamos los vectores de dirección
                foreach (PintarVector p in ListaVectores)
                {
                    g.DrawLine(new Pen(p.color, 2), p.ini, p.fin);
                }

                g.Dispose();
            }
            picMapa.Image = imageAux;
            picMapa.Invalidate();
            picMapa.Refresh();
        }

        private void PintarRecta(Color color, Point p1, Point p2)
        {
            //Image currentImage = picMapa.Image;
            Image currentImage = picMapa.Image;
            Bitmap imageAux = new Bitmap(currentImage.Width, currentImage.Height);
            using (Graphics g = Graphics.FromImage(imageAux))
            {
                g.DrawLine(new Pen(color, 2), p1, p2);
            }
            picMapa.Image = imageAux;
            picMapa.Invalidate();
            picMapa.Refresh();
        }

        private void Pintar(int TipoGrafico, Pen pen, Point P1, Point P2)
        {
            Image currentImage = picMapa.Image;
            Bitmap imageAux = new Bitmap(currentImage.Width, currentImage.Height);
            using (Graphics g = Graphics.FromImage(imageAux))
            {
                g.DrawImage(currentImage, new Point(0, 0));
                g.Flush();

                g.DrawLine(pen, P1, P2);

                g.Dispose();
            }
            picMapa.Image = imageAux;
            picMapa.Invalidate();
            picMapa.Refresh();
            currentImage.Dispose();
            GC.Collect();
        }

        void ColorearVertice(Graphics g, Nodo Punto, Color ColorVertice)
        {
            Brush brush = new SolidBrush(ColorVertice);
            Brush brushNegro = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.Black, 1);

            g.DrawRectangle(pen, new Rectangle(Punto.X, Punto.Y, PUNTO_ALTO, PUNTO_ANCHO));
            g.FillRectangle(brush, new Rectangle(Punto.X + 1, Punto.Y + 1, PUNTO_ALTO - 2, PUNTO_ANCHO - 2));
            g.DrawString((Punto.ID).ToString(), new Font("Arial", 12), brushNegro, new Point(Punto.X - 2, Punto.Y - 2));
        }
        void PintarBaliza(Graphics g, Baliza Bal)
        {
            const int BALIZA_ALTO = 15;
            Brush brush = new SolidBrush(Color.White);
            Brush brushNegro = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.Black, 1);

            g.DrawRectangle(pen, new Rectangle(Bal.PosX, Bal.PosY, BALIZA_ALTO, BALIZA_ALTO));
            g.FillRectangle(brush, new Rectangle(Bal.PosX + 1, Bal.PosY + 1, BALIZA_ALTO - 2, BALIZA_ALTO - 2));
            g.DrawString((Bal.Id).ToString(), new Font("Arial", 12), brushNegro, new Point(Bal.PosX - 2, Bal.PosY - 2));
        }
        private Nodo LocalizarNodo(int X, int Y)
        {
            foreach (Nodo Punto in ListaNodos)
            {
                if ((X >= Punto.X) && (Y >= Punto.Y) && (X <= Punto.X + PUNTO_ANCHO) && (Y <= Punto.Y + PUNTO_ALTO))
                    return Punto;
            }
            Nodo Punto1 = new Nodo();
            Punto1.ID = -1;

            return Punto1;
        }

        private void InsertarNodos_Click(object sender, EventArgs e)
        {


            // Definición de la matriz de adyacencia del digrafo mostrado en la figura 1
            int[,] L ={
            {-1, 10, 18, -1 , -1, -1 , -1},
            {-1, -1 , 6, -1 , 3, -1, -1},
            {-1, -1 , -1, 3, -1, 20, -1},
            {-1, -1 , 2, -1 , -1, -1, 2},
            {-1, -1 , -1, 6, -1, -1, 10},
            {-1, -1 , -1, -1 , -1, -1 , -1},
            {-1, -1 , 10, -1 , -1, 5, -1}
            };
            Dijkstra prueba = new Dijkstra((int)Math.Sqrt(L.Length), L);
            prueba.CorrerDijkstra();
            Console.WriteLine
            ("La solucion de la ruta mas corta tomando como nodo inicial el NODO 1 es: ");
            int nodo = 1;
            foreach (int i in prueba.D)
            {
                Console.Write("Distancia minima a nodo " + nodo + " es ");
                Console.WriteLine(i);
                nodo++;
            }
            Console.WriteLine();
            Console.WriteLine("Presione la tecla Enter para salir.");
            Console.Read();
        }

        private void AddNodos_Click(object sender, EventArgs e)
        {
            ActivarBoton("AddNodos");
            Estado = Estados.InsertarNodos;
        }

        private void UnirNodos_Click(object sender, EventArgs e)
        {
            Estado = Estados.UnirNodos;
            SubEstado = Estados.NodoInicial;
            Union = new Enlace();
            ActivarBoton("UnirNodos");
        }

        private void cmdRuta_Click(object sender, EventArgs e)
        {
            if (Estado != Estados.EstablecerCamino)
            {
                Estado = Estados.EstablecerCamino;
                SubEstado = Estados.NodoInicial;
                ActivarBoton("cmdRuta");
            }
            else if (SubEstado == Estados.NodoInicial)
            {
                SubEstado = Estados.NodoFinal;
            }
        }

        private void cmdCalcularRuta_Click(object sender, EventArgs e)
        {
            TrazarRuta(NodoInicial, NodoFinal);
        }

        private void cmdBuscarRuta_Click(object sender, EventArgs e)
        {
            Estado = Estados.BuscarRuta;
            SubEstado = Estados.NodoInicial;
        }

        private void picMapa_Click_1(object sender, EventArgs e)
        {

        }

        private void cmdMedirDistancia_Click(object sender, EventArgs e)
        {
            Estado = Estados.MedirDistancia;
            SubEstado = Estados.NodoInicial;
        }

        double Distancia(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        Point LocalizarPuntoObstaculo(int X, int Y)
        {
            int MARGEN = this.MARGEN;
            if (!chkPrecisionObstaculos.Checked) MARGEN = 0;

            foreach (Obstaculo o in ListaObstaculos)
            {
                List<Point> ListaPuntos = new List<Point>();
                ListaPuntos.Add(o.P1);
                ListaPuntos.Add(o.P2);
                foreach (Point punto in ListaPuntos)
                    if ((X - MARGEN < punto.X) && (X + MARGEN > punto.X) && (Y - MARGEN < punto.Y) && (Y + MARGEN > punto.Y))
                    { //Finalizamos un obstáculo
                        SubEstado = Estados.NodoInicial;
                        return new Point(punto.X, punto.Y);
                    }
            }
            return new Point(X, Y);
        }

        private void tbEscala_Leave(object sender, EventArgs e)
        {
            //appSettings["Escala"] = tbEscalaX.Text;
            //config.Save(ConfigurationSaveMode.Modified);
        }

        private void frmTrayectoria_Load(object sender, EventArgs e)
        {
            config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            tbEscalaX.Text = LeerClave("Escala");
        }

        private void cmdDefinirObstaculo_Click(object sender, EventArgs e)
        {
            if (Estado == Estados.DefinirObstaculos)
            {
                Estado = Estados.Vacio;
                ActivarBoton("");
            }
            else
            {
                Estado = Estados.DefinirObstaculos;
                SubEstado = Estados.NodoInicial;
                ActivarBoton("cmdDefinirObstaculo");
            }
        }

        private void cmdGrabarRuta_Click(object sender, EventArgs e)
        {
            if (tbEscalaSim.Text == "")
                MessageBox.Show("Faltan datos por configurar");
            else
                GrabarConfiguracion();
        }

        void GrabarConfiguracion()
        {
            config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            //Grabamos los enlaces
            int pos = 0;
            GrabarClave("EscalaSim", tbEscalaSim.Text ); //180
            GrabarClave("SimLonX", tbLonX.Text); //2,5
            GrabarClave("SimLonY", tbLonY.Text); //2,5
            GrabarClave("Robot", cbRobot.Text);
            GrabarClave("Kalman", chkKalman.Checked?"S":"N");

            GrabarClave("EscalaX", tbEscalaX.Text);
            GrabarClave("EscalaY", tbEscalaY.Text);
            GrabarClave("Enlace_NUM", pos.ToString());
            foreach (Enlace Union in ListaEnlaces)
            {
                GrabarClave("Enlace" + pos + "_ID1", Union.ID1.ToString());
                GrabarClave("Enlace" + pos + "_ID2", Union.ID2.ToString());
                GrabarClave("Enlace" + pos + "_RUTA", Union.ruta.ToString());
                GrabarClave("Enlace" + pos + "_DIST", Union.Distancia.ToString());
                pos++;
                GrabarClave("Enlace_NUM", pos.ToString());
            }
            //Grabamos los nodos
            pos = 0;
            GrabarClave("Nodo_NUM", pos.ToString());
            foreach (Nodo Punto in ListaNodos)
            {
                GrabarClave("Nodo" + pos + "_X", Punto.X.ToString());
                GrabarClave("Nodo" + pos + "_Y", Punto.Y.ToString());
                GrabarClave("Nodo" + pos + "_ID", Punto.ID.ToString());
                pos++;
                GrabarClave("Nodo_NUM", pos.ToString());
            }
            //Grabamos los obstáculos
            pos = 0;
            GrabarClave("Obstaculo_NUM", pos.ToString());
            foreach (Obstaculo o in ListaObstaculos)
            {
                GrabarClave("Obstaculo" + pos + "_P1_X", o.P1.X.ToString());
                GrabarClave("Obstaculo" + pos + "_P1_Y", o.P1.Y.ToString());
                GrabarClave("Obstaculo" + pos + "_P2_X", o.P2.X.ToString());
                GrabarClave("Obstaculo" + pos + "_P2_Y", o.P2.Y.ToString());
                pos++;
                GrabarClave("Obstaculo_NUM", pos.ToString());
            }
            //Grabamos las balizas
            pos = 0;
            GrabarClave("Baliza_NUM", pos.ToString());
            foreach (Baliza b in ListaBalizas)
            {
                GrabarClave("Baliza" + pos + "_X", b.PosX.ToString());
                GrabarClave("Baliza" + pos + "_Y", b.PosY.ToString());
                GrabarClave("Baliza" + pos + "_ID", b.Id.ToString());
                pos++;
                GrabarClave("Baliza_NUM", pos.ToString());
            }
            config.Save(ConfigurationSaveMode.Modified);
        }

        void RecuperarConfiguracion()
        {
            tbEscalaSim.Text = LeerClave("EscalaSim");
            tbLonX.Text = LeerClave("SimLonX");
            tbLonY.Text = LeerClave("SimLonY");
            cbRobot.Text = LeerClave("Robot");
            chkKalman.Checked = LeerClave("Kalman") == "S" ? true : false;
            Simulador = (cbRobot.Text == "Simulador");


            tbEscalaX.Text = LeerClave("EscalaX");
            tbEscalaY.Text = LeerClave("EscalaY");
            ConfiguracionRecuperada = true;
            int NumeroNodos = Convert.ToInt16(LeerClave("Nodo_NUM"));
            int NumeroEnlaces = Convert.ToInt16(LeerClave("Enlace_NUM"));
            int NumeroObstaculos = Convert.ToInt16(LeerClave("Obstaculo_NUM"));
            int NumeroBalizas = Convert.ToInt16(LeerClave("Baliza_NUM"));

            ListaNodos = new List<Nodo>();
            for (int i = 0; i < NumeroNodos; i++)
            {
                Nodo nodo = new Nodo();
                nodo.ID = Convert.ToInt16(LeerClave("Nodo" + i + "_ID"));
                nodo.X = Convert.ToInt16(LeerClave("Nodo" + i + "_X"));
                nodo.Y = Convert.ToInt16(LeerClave("Nodo" + i + "_Y"));

                ListaNodos.Add(nodo);
            }

            ListaEnlaces = new List<Enlace>();
            for (int i = 0; i < NumeroEnlaces; i++)
            {
                Enlace enlace = new Enlace();
                enlace.ID1 = Convert.ToInt16(LeerClave("Enlace" + i + "_ID1"));
                enlace.ID2 = Convert.ToInt16(LeerClave("Enlace" + i + "_ID2"));
                enlace.ruta = (LeerClave("Enlace" + i + "_RUTA") == "True" ? true : false);
                enlace.Distancia = Convert.ToDouble(LeerClave("Enlace" + i + "_DIST"));

                ListaEnlaces.Add(enlace);
            }

            ListaBalizas = new List<Baliza>();
            for (int i = 0; i < NumeroBalizas; i++)
            {
                Baliza Bal = new Baliza();
                Bal.Id = LeerClave("Baliza" + i + "_ID");
                Bal.PosX = Convert.ToInt16(LeerClave("Baliza" + i + "_X"));
                Bal.PosY = Convert.ToInt16(LeerClave("Baliza" + i + "_Y"));
                ListaBalizas.Add(Bal);
            }

            ListaObstaculos = new List<Obstaculo>();
            for (int i = 0; i < NumeroObstaculos; i++)
            {
                Obstaculo obstaculo = new Obstaculo();
                obstaculo.P1 = new Point(Convert.ToInt16(LeerClave("Obstaculo" + i + "_P1_X")),
                                            Convert.ToInt16(LeerClave("Obstaculo" + i + "_P1_Y")));
                obstaculo.P2 = new Point(Convert.ToInt16(LeerClave("Obstaculo" + i + "_P2_X")),
                                            Convert.ToInt16(LeerClave("Obstaculo" + i + "_P2_Y")));

                ListaObstaculos.Add(obstaculo);
            }

            PintarGrafo();
        }

        void GrabarClave(string clave, string valor)
        {
            config.AppSettings.Settings.Remove(clave);
            config.AppSettings.Settings.Add(clave, valor);
        }
        string LeerClave(string clave)
        {
            try
            {
                return config.AppSettings.Settings[clave].Value;
            }
            catch
            {
                return "";
            }
        }

        void ActivarBoton(string NombreBoton)
        {
            foreach (Button b in ListaBotones)
            {
                if (b.Name != NombreBoton)
                    b.BackColor = Color.LightSeaGreen;
                else
                    b.BackColor = Color.LightSalmon;
            }
        }

        private void frmTrayectoria_Activated(object sender, EventArgs e)
        {
            if (ListaBotones.Count == 0)
            {
                ListaBotones.Add(AddNodos);
                ListaBotones.Add(UnirNodos);
                ListaBotones.Add(cmdRuta);
                ListaBotones.Add(cmdDefinirObstaculo);
            }
        }
        private void cmdLeerCfg_Click(object sender, EventArgs e)
        {
            RecuperarConfiguracion();
        }

        int LocalizarNodoMasCercano(Point puntoRobot, ref List<Enlace> ListaRutasDistancia)
        {
            //1. Calculamos la distancia desde la posición del robot a todos los puntos
            //2. Seleccionamos las dos distancias más cercanas
            //3. Calculamos la ruta a ambos nodos
            //4. Si los dos forman parte de la misma ruta A,B
            //  obtenemos la dirección de ambos y la dirección del vector A-B
            //  Empleamos el enlace que apunte hacia la misma dirección que A-B
            // 5. Si forman rutas distintas nos quedamos con la ruta más corta
            List<Enlace> ListaRutas = new List<Enlace>();

            foreach (Nodo n in ListaNodos)
            {
                bool IntersectaObstaculo = false;
                //Comprobamos si el segmento intersecta con algún obstáculo
                foreach (Obstaculo o in ListaObstaculos)
                {
                    if (chkPintarColision.Checked)
                        Pintar(0, new Pen(Color.Pink, 3), o.P1, o.P2);

                    if (Interseccion(puntoRobot, new Point(n.X, n.Y), o.P1, o.P2))
                    {
                        if (chkPintarColision.Checked)
                        {
                            Pintar(0, new Pen(Color.Red, 3), puntoRobot, new Point(n.X, n.Y));
                            Pintar(0, new Pen(Color.Blue, 6), o.P1, o.P2);
                        }
                        IntersectaObstaculo = true;
                        break;
                    }
                    else if (chkPintarColision.Checked)
                        Pintar(0, new Pen(Color.Green, 3), puntoRobot, new Point(n.X, n.Y));

                }
                if (!IntersectaObstaculo)
                {
                    Enlace enlace = new Enlace();
                    enlace.ID1 = -1;
                    enlace.ID2 = n.ID;
                    enlace.Distancia = Distancia(puntoRobot, new Point(n.X, n.Y));
                    ListaRutas.Add(enlace);
                }
            }
            //Ordenamos por distancia los puntos de la ruta alcanzables desde la posición actual
            ListaRutasDistancia = ListaRutas.OrderBy(o => o.Distancia).ToList();

            foreach (Enlace e in ListaRutasDistancia)
                return e.ID2;

            return -1;
        }

        bool Interseccion(Point pr, Point prf, Point oi, Point of)
        {
            double a1, b1, a2, b2, t, Xi, Yi;
            int nodoX = prf.X;
            int nodoY = prf.Y;
            Point P1 = new Point(oi.X, oi.Y);
            Point P2 = new Point(of.X, of.Y);

            //rectas Y = aX + b
            //segmento robot-nodo
            t = ((nodoX == pr.X) ? 0 : ((double)nodoY - pr.Y) / (nodoX - pr.X));
            a1 = t;
            b1 = -a1 * nodoX + nodoY;

            //segmento obstáculo
            t = ((P1.X == P2.X) ? 0 : ((double)P1.Y - P2.Y) / (P1.X - P2.X));
            a2 = t;
            b2 = -a2 * P1.X + P1.Y;

            //Intersección
            if (a1 == a2)
                return false; //Son paralelas
            else //Debemos comprobar si el punto de corte forma parte del segmento del obstáculo
            {
                //Punto de intersección
                Xi = (b2 - b1) / (a1 - a2);
                Yi = a1 * Xi + b1;

                int Xmin = 0, Xmax = 0, Ymin = 0, Ymax = 0;
                MinMax(pr.X, prf.X, ref Xmin, ref Xmax);
                MinMax(pr.Y, prf.Y, ref Ymin, ref Ymax);
                if (((Xi >= Xmin) && (Xi <= Xmax)) && ((Yi >= Ymin) && (Yi <= Ymax)))
                { //El punto debe estar entro los límites del segmento del obstáculo y del vector que une la posición con el nodo
                    MinMax(oi.X, of.X, ref Xmin, ref Xmax);
                    MinMax(oi.Y, of.Y, ref Ymin, ref Ymax);
                    if (((Xi >= Xmin) && (Xi <= Xmax)) && ((Yi >= Ymin) && (Yi <= Ymax)))
                    { //El punto debe estar entro los límites del segmento del obstáculo y del vector que une la posición con el nodo
                        return true;
                    }
                }
            }
            return false;
        }

        void MinMax(int X1, int X2, ref int Xmin, ref int Xmax)
        {
            if (X1 > X2)
            {
                Xmin = X2;
                Xmax = X1;
            }
            else
            {
                Xmin = X1;
                Xmax = X2;
            }
        }
        void TrazarRuta(int NodoInicial, int NodoFinal)
        {
            int[,] Matriz = new int[ListaNodos.Count, ListaNodos.Count];

            if ((NodoInicial != NodoFinal) && (NodoInicial + NodoFinal > 0))
            {
                //Borramos la anterior ruta
                for (int i = 0; i < ListaEnlaces.Count; i++)
                {
                    Enlace Union = ListaEnlaces[i];
                    Union.ruta = false;
                    ListaEnlaces[i] = Union;
                }
                int pos_i, pos_j;
                //Generamos la matriz de adyacencia
                for (int i = 0; i < ListaNodos.Count; i++)
                {
                    for (int j = 0; j < ListaNodos.Count; j++)
                    {
                        pos_i = (i + NodoInicial - 1) % ListaNodos.Count;
                        pos_j = (j + NodoInicial - 1) % ListaNodos.Count;
                        if (pos_i < 0) pos_i = ListaNodos.Count - pos_i;
                        if (pos_j < 0) pos_j = ListaNodos.Count - pos_j;
                        Enlace Union = ListaEnlaces.Find(x => ((x.ID1 == pos_i + 1) && (x.ID2 == pos_j + 1)));
                        if (Union == null)
                            Matriz[i, j] = -1;
                        else
                            Matriz[i, j] = Convert.ToInt16(Union.Distancia);
                    }
                }
                Dijkstra RutaMasCorta = new Dijkstra((int)Math.Sqrt(Matriz.Length), Matriz);
                RutaMasCorta.CorrerDijkstra();
                int NodoFinal1 = NodoFinal;
                NodoFinal1 -= NodoInicial;
                if (NodoFinal1 < 0) NodoFinal1 = ListaNodos.Count - NodoFinal1;
                Stack<int> Trayectoria = RutaMasCorta.Ruta(NodoFinal1);
                Console.Write("0,");
                int Nodo1, Nodo2;

                //Recuperamos el camino
                Nodo1 = Nodo2 = NodoInicial - 1;
                int num_nodo = 1;
                while (Trayectoria.Count > 0)
                {
                    int Ruta = (Trayectoria.Pop() + NodoInicial - 1) % ListaNodos.Count;
                    Nodo2 = Ruta;
                    int[] pos = new int[2];
                    pos[0] = ListaEnlaces.FindIndex(x => (x.ID1 == Nodo1 + 1) && (x.ID2 == Nodo2 + 1));
                    pos[1] = ListaEnlaces.FindIndex(x => (x.ID1 == Nodo2 + 1) && (x.ID2 == Nodo1 + 1));
                    if (pos[0] != -1)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Enlace Union = ListaEnlaces[pos[i]];
                            Union.num_nodo_ruta = num_nodo++;
                            Union.ruta = true;
                            ListaEnlaces[pos[i]] = Union;
                            Nodo1 = Nodo2;
                        }
                    }
                    Console.Write(Ruta + ",");
                }
                PintarGrafo();
            }
            else
                MessageBox.Show("No está definido el grafo");
        }
        void CalcularRecorridoRobot(int NodoInicial, int NodoFinal, ref List<Nodo> ListaNodosRuta)
        {

            //Localizamos la ruta que une el nodo más cercano con el destino
            int NodoInicial1 = 0, NodoFinal1 = 0;
            if (NodoFinal != -1)
            {
                MinMax(NodoInicial, NodoFinal, ref NodoInicial1, ref NodoFinal1);
                TrazarRuta(NodoInicial1, NodoFinal1);
            }
            //Refinamos la ruta por si podemos entrar más adelante en la ruta
            List<Enlace> EnlacesRuta;
            if (NodoInicial == NodoInicial1)
                EnlacesRuta = ListaEnlaces.OrderBy(o => -o.num_nodo_ruta).ToList();
            else
                EnlacesRuta = ListaEnlaces.OrderBy(o => o.num_nodo_ruta).ToList();

            Console.Write("Ruta: ");
            Enlace en1 = new Enlace();
            int NodoAnt = -1;

            Nodo NodoSalida = new Nodo();
            NodoSalida.ID = 0;
            NodoSalida.X = PuntoFinal.X;
            NodoSalida.Y = PuntoFinal.Y;
            //ListaNodosRuta.Add(NodoSalida);
            Stack<Nodo> PilaRuta = new Stack<Nodo>();
            foreach (Enlace en in EnlacesRuta)
            {
                if ((en.ruta) && (NodoAnt != en.ID1))
                {
                    NodoAnt = en.ID1;
                    Console.Write(en.ID1 + ",");
                    Nodo NodoRuta = new Nodo();
                    NodoRuta = ListaNodos.Find(x => x.ID == en.ID1);
                    PilaRuta.Push(NodoRuta);
                    //Intentamos localizar el nodo en la lista de nodos alcanzables en línea recta
                    /*
                        Esto último tiene el problema de que puede no intersectar, pero si sumamos el ancho del robot, no podrá llegar, mientras el camino seguro está definido 
                        para que se mueva el robot sin problemas. Podriamos trazar un segmento tan ancho como el robot que una el punto origen con los nodos, si el segmento completo 
                        no colisiona con un obstáculo, el recorte del camino es seguro.
                        Adicionalmente, aunque podamos acortar una ruta, puede ser posible que acortemos con un nodo que está al lado de una puerta, con lo que con ciertas configuraciones
                        de robot, no podríamos girar para pasar la puerta. Cada nodo debe indicar el ángulo de giro necesario si se acorta una ruta comenzando por él.
                    */
                    //if (ListaRutasDistancia.FindIndex(x => x.ID2 == en.ID1) >= 0)
                    //{
                    //    //Cortamos la ruta, ya que este nodo es alcanable directamente desde el orígen en línea recta
                    //    break;
                    //}
                }
            }
            foreach (Nodo n in PilaRuta)
                ListaNodosRuta.Add(n);
        }
        //Pulsación de ratón en el mapa **CTR******************************************************************************************************************
        private void picMapa_MouseDown(object sender, MouseEventArgs e)
        {
            if (Estado == Estados.InsertarNodos)
            {
                Nodo Punto = new Nodo();
                Punto.tipo = 0;
                Punto.X = e.X;
                Punto.Y = e.Y;
                Punto.ID = ++NumeroNodos;
                ListaNodos.Add(Punto);
            }
            else if (Estado == Estados.UnirNodos)
            {
                Nodo Punto = LocalizarNodo(e.X, e.Y);
                if (Punto.ID != -1)
                {
                    if (SubEstado == Estados.NodoInicial)
                    {
                        Union = new Enlace();
                        Union.ID1 = Punto.ID;
                        SubEstado = Estados.NodoFinal;
                    }
                    else
                    {
                        if (Union.ID1 == Punto.ID)
                        {
                            MessageBox.Show("Debe seleccionar un nodo distinto");
                            return;
                        }
                        SubEstado = Estados.NodoInicial;
                        Union.ID2 = Punto.ID;
                        Nodo Nodo1 = ListaNodos.Find(x => x.ID == Union.ID1);
                        Nodo Nodo2 = ListaNodos.Find(x => x.ID == Union.ID2);
                        Union.Distancia = Math.Sqrt(Math.Pow(Nodo1.X - Nodo2.X, 2) + Math.Pow(Nodo1.Y - Nodo2.Y, 2));
                        Union.ruta = false;
                        Enlace Union1 = new Enlace();
                        Union1.ID1 = Union.ID2;
                        Union1.ID2 = Union.ID1;
                        Union1.Distancia = Union.Distancia;
                        Union1.ruta = false;
                        ListaEnlaces.Add(Union);
                        ListaEnlaces.Add(Union1);
                        lstEnlaces.Items.Add(Nodo1.ID.ToString() + "," + Nodo2.ID.ToString() + " = " + Union.Distancia.ToString());
                    }
                }
                else
                    MessageBox.Show("Debe Pulsar en un nodo");
            }
            else if (Estado == Estados.EstablecerCamino)
            {
                Nodo Punto = LocalizarNodo(e.X, e.Y);
                if (SubEstado == Estados.NodoInicial)
                {
                    NodoInicial = Punto.ID;
                    SubEstado = Estados.NodoFinal;
                }
                else
                {
                    NodoFinal = Punto.ID;
                    SubEstado = Estados.NodoInicial;
                    Estado = Estados.Vacio;
                    ActivarBoton("");

                }
            }
            else if (Estado == Estados.BuscarRuta)
            {
                if (SubEstado == Estados.NodoInicial)
                {
                    Nodo Punto = LocalizarNodo(e.X, e.Y);
                    if (Punto.ID == -1)
                        MessageBox.Show("Debe seleccionar un nodo");
                    else
                    {
                        NodoInicial = Punto.ID;
                        SubEstado = Estados.NodoFinal;
                    }
                }
                else
                {
                    PuntoFinal = new Point(e.X, e.Y);
                    SubEstado = Estados.NodoInicial;
                    Estado = Estados.Vacio;
                    ActivarBoton("");
                    List<Enlace> ListaRutasDistancia = new List<Enlace>();
                    NodoFinal = LocalizarNodoMasCercano(PuntoFinal, ref ListaRutasDistancia);
                    ListaNodosRuta = new List<Nodo>();
                    CalcularRecorridoRobot(NodoInicial, NodoFinal, ref ListaNodosRuta);
                    Console.WriteLine();
                    foreach (Nodo n in ListaNodosRuta)
                    {
                        Console.WriteLine("Nodo: " + n.ID + ", X=" + n.X + ", Y=" + n.Y);
                    }
                }

            }
            else if (Estado == Estados.MedirDistancia)
            {
                if (SubEstado == Estados.NodoInicial)
                {
                    PuntoInicial = new Point(e.X, e.Y);
                    SubEstado = Estados.NodoFinal;
                }
                else
                {
                    PuntoFinal = new Point(e.X, e.Y);
                    SubEstado = Estados.NodoInicial;

                    Point pi = PuntoInicial;
                    Point pf = PuntoFinal;
                    double DistanciaPuntos = Distancia(pi, pf);

                    double EscalaX = (double)Convert.ToDouble(tbEscalaX.Text);
                    double EscalaY = (double)Convert.ToDouble(tbEscalaY.Text);
                    PuntoInicial.X = (int)(PuntoInicial.X * EscalaX);
                    PuntoInicial.Y = (int)(PuntoInicial.Y * EscalaY);
                    PuntoFinal.X = (int)(PuntoFinal.X * EscalaX);
                    PuntoFinal.Y = (int)(PuntoFinal.Y * EscalaY);

                    double Distanciamm = Distancia(PuntoInicial, PuntoFinal);
                    double Orientacion = DireccionVector(PuntoInicial.X, PuntoInicial.Y, PuntoFinal.X, PuntoFinal.Y);
                    MessageBox.Show("Distancia Puntos: " + DistanciaPuntos+ ", Distancia mm: " + Distanciamm + 
                         ", Orientación:   " + Math.Round(Orientacion,3) + " - mm: ( " + PuntoInicial.X + " , " + PuntoInicial.Y + " - " + PuntoFinal.X + " , " + PuntoFinal.Y + " )" +
                         " - Ptos: ( " + pi.X + " , " + pi.Y + " - " + pf.X + " , " + pf.Y + " )"
                         );
                }
            }
            else if (Estado == Estados.DefinirObstaculos)
            {
                if (SubEstado == Estados.NodoInicial)
                {
                    PuntoInicial = LocalizarPuntoObstaculo(e.X, e.Y);
                    SubEstado = Estados.NodoFinal;
                }
                else
                {
                    PuntoFinal = LocalizarPuntoObstaculo(e.X, e.Y);
                    Obstaculo LineaObstaculo = new Obstaculo();
                    LineaObstaculo.P1 = PuntoInicial;
                    LineaObstaculo.P2 = PuntoFinal;
                    ListaObstaculos.Add(LineaObstaculo);
                    PuntoInicial = PuntoFinal;
                }
            }
            else if (Estado == Estados.ColocarBaliza)
            {
                Baliza Bal = new Baliza();
                Bal.Id = Microsoft.VisualBasic.Interaction.InputBox("Identificador de la Baliza");
                Bal.PosX = e.X;
                Bal.PosY = e.Y;
                ListaBalizas.Add(Bal);
            }

            PintarGrafo();
        }

        long clock()
        {
            return System.DateTime.Now.Ticks/10;
        }
        private void cmdIniciarRobot_Click(object sender, EventArgs e)
        {
            if (!ConfiguracionRecuperada)
                MessageBox.Show("No se ha cargado la configuración");
            else
                CambiarEstadoPuerto();
        }

        void CambiarEstadoPuerto()
        {
            if (cmdInicarRobot.Tag.ToString() == "ON")
            { //Cerrar puerto
                cmdInicarRobot.Text = "Conectar";
                cmdInicarRobot.Tag = "OFF";
                try {
                    Puerto.serialPort.Write("stop.");
                    Puerto.serialPort.Write("pos0.");
                }
                catch (Exception e) { }
                cbPuerto.BackColor = Color.White;
                tmrControlPosicion.Stop();
                tmrControlPosicion.Enabled = false;
                tmrActualizarPosicion.Enabled = false;
                tmrPintarRobot.Enabled = false;
                cmdPos.Tag = "OFF";
                cmdPos.BackColor = Color.Red;
                Puerto.CerrarPuerto();
            }
            else
            { //Abrir Puerto
                Puerto = new ComunicacionPuertoSerie(cbPuerto.Text, RecibidaPosicion);
                lblPosOk.BackColor = Color.White;
                tmrControlPosicion.Enabled = true;
                tmrActualizarPosicion.Enabled = true;
                tmrControlPosicion.Start();
                cmdInicarRobot.Text = "Desconectar";
                cmdInicarRobot.Tag = "ON";
                cbPuerto.BackColor = Color.LightGreen;
                tmrPintarRobot.Enabled = true;
                try{
                    Puerto.serialPort.Write("stop.");
                    Puerto.serialPort.Write("pos1.");
                }
                catch (Exception e) { }
                cmdPos.Tag = "ON";
                cmdPos.BackColor = Color.Green;

                //ControlVelocidadRobot();
            }
        }

        int simxGetObjectPosition(int clientID, int robot, int op, ref float[] position, int simx_opmode_oneshot)
        {
            return 0;
        }
        int simxGetConnectionId(int clientID)
        {
            if (Simulador)
                return VREPWrapper.simxGetConnectionId(clientID);
            else
            {
                if (cmdInicarRobot.Tag.ToString() == "ON")
                    return 1;
                else
                    return -1;
            }
        }
        int Mapear(int dir, int v1m, int v1M, int v2m, int v2M, int Velocidad)
        {
            double ratio = (double)Math.Abs(v1M - v1m) / (v2M - v2m);
            return (int)(dir * (Velocidad-v2m) * ratio + v1m);
        }
        void simxSetJointTargetVelocity(int clienID, int motor, double pos, long op)
        {
            VREPWrapper.simxSetJointTargetVelocity(clienID, motor, (float)pos, (simx_opmode)op);
        }
        enum Fases : int { arranque_inicial, establecer_vector_direccion, iniciar_ruta, seguir_ruta };
        void ControlVelocidadRobot(int ClientID)
        {
            int ProhibirGiro = 0;
            const int NUM_MEDIDAS = 1;
            const int GIRO_DERECHA = -1;
            const int GIRO_IZQUIERDA = 1;
            const int MIN_DISTANCIA_TRAYECTORIA_CONTROL = 15;
            double Xpos, Ypos;
            double Xant, Yant;
            double[] aXpos = new double[NUM_MEDIDAS];
            double[] aYpos = new double[NUM_MEDIDAS];
            int iContMedidas = 0;
            int clientID = 0;
            bool retorno_posicion = false;
            float[] position = new float[3];
            Fases fase = Fases.arranque_inicial;
            long ms_inicio_simulacion = 0;
            double Xini = 0, Yini = 0;
            double XiniAnt = 0, YiniAnt = 0;
            double VectorDireccionRobot;
            double VectorDireccionAproximacion;
            double VectorDireccionH;
            double DifVectorDireccionH;
            double ModH;
            double VectorDireccionTrayectoria = 0;
            double DifVector;
            double DifVectorTrayectoria;
            double DifReal;
            double ModSegTray;
            double ModDirAprox;
            double ModIniTray;
            double Xd = 0, Yd = 0;
            long ms = 0;
            int IndSigNodo = 0;
            long ticks = 0; //Número de iteraciones de control
            Nodo SigNodo = new Nodo();
            Nodo? NodoAct = null;
            Random Error = new Random();

            if (ListaNodosRuta.Count == 0)
            {
                MessageBox.Show("Debe cargar la ruta");
                return;
            }

            myPID.SetMode(PID.AUTOMATIC);

            if (Simulador)
            {
                //Aqui guardamos los dos motores y el sensor
                int valido = (int) VREPWrapper.simxGetObjectHandle(clientID, "motor_derecho", out motor_derecho, (simx_opmode) simx_opmode_blocking);
                int valido2 = (int)VREPWrapper.simxGetObjectHandle(clientID, "motor_izquierdo", out motor_izquierdo, (simx_opmode) simx_opmode_blocking);
                int valido_sensor = (int)VREPWrapper.simxGetObjectHandle(clientID, "sensor", out sensor, (simx_opmode) simx_opmode_blocking);
                int valido_robot = (int)VREPWrapper.simxGetObjectHandle(clientID, "robot", out robot, (simx_opmode) simx_opmode_blocking);

                //Si no se ha podido acceder a alguno de estos componentes mostramos un mensaje de error y salimos del programa
                if (valido != 0 || valido2 != 0 || valido_sensor != 0 || valido_robot != 0)
                {
                    //cout << "ERROR: No se ha podido conectar con el robot" << endl;
                    VREPWrapper.simxFinish(clientID);
                    MessageBox.Show("ERROR recuperando objetos de la simulación");
                    return;
                }
                MODULO_MIN_MEDIDA = 9;
                MODULO_MIN_MEDIDA_AVANCE = 9;
                DISTANCIA_SALTO_NODO = 25;
                DISTANCIA_NODO_PARA_VEL_MIN = 25;
                MIN_MODULO_ACT_POSICION = 9;
                MAX_VEL_ANG = 3;
                pbDer.Maximum = (int)MAX_VEL_ANG;
                pbIzq.Maximum = (int)MAX_VEL_ANG;
            }
            else if (cbRobot.Text == "DANI")
            {
                //Kalman
                //MODULO_MIN_MEDIDA = 6;
                //DISTANCIA_SALTO_NODO = 40;
                //MIN_MODULO_ACT_POSICION = 12;
                //MAX_VEL_ANG = 20;
                MODULO_MIN_MEDIDA = 9;
                MODULO_MIN_MEDIDA_AVANCE = 4;
                DISTANCIA_SALTO_NODO = 60;
                DISTANCIA_NODO_PARA_VEL_MIN = 70;
                MIN_MODULO_ACT_POSICION = 14;
                MAX_VEL_ANG = 20;
                pbDer.Maximum = (int)MAX_VEL_ANG;
                pbIzq.Maximum = (int)MAX_VEL_ANG;
            }
            else if (cbRobot.Text == "RODI")
            {
                MODULO_MIN_MEDIDA = 9;
                MODULO_MIN_MEDIDA_AVANCE = 4;
                DISTANCIA_SALTO_NODO = 60;
                DISTANCIA_NODO_PARA_VEL_MIN = 80;
                MIN_MODULO_ACT_POSICION = 14;
                MAX_VEL_ANG = 20;
                pbDer.Maximum = (int)MAX_VEL_ANG;
                pbIzq.Maximum = (int)MAX_VEL_ANG;
            }

            AsignarVelocidad(0, 0);
            if (Orientacion == -1)
                fase = Fases.arranque_inicial;
            else
                fase = Fases.iniciar_ruta;


            while ((simxGetConnectionId(clientID) != -1) && SeguimientoActivo ) //Este bucle funcionara hasta que se pare la simulacion
	        {
                double VelRuedaDer = 0;
                double VelRuedaIzq = 0;
                Application.DoEvents();
		        long ds = clock()/100; //clock() = ms

                #region "sim_obstaculos"
                //simxReadProximitySensor(clientID, sensor, &obstaculo, NULL, NULL, NULL, simx_opmode_buffer); //Leer el sensor
                //if (obstaculo == 0) //Si no se detecta ningun obstaculo, avanzar
                //{
                //	simxSetJointTargetVelocity(clientID, motor_derecho, -2, simx_opmode_oneshot);
                //	simxSetJointTargetVelocity(clientID, motor_izquierdo, -2, simx_opmode_oneshot);
                //}
                //else //Giraremos si encontramos algo en el camino del robot
                //{
                //	simxSetJointTargetVelocity(clientID, motor_derecho, 2, simx_opmode_oneshot);
                //	simxSetJointTargetVelocity(clientID, motor_izquierdo, -2, simx_opmode_oneshot);
                //}
                #endregion

                if (Simulador)
                {
                    if ((ds % MS_CONTROL_MEDIDA) == 0)
                    {
                        retorno_posicion = false;
                        int error = (int)VREPWrapper.simxGetObjectPosition(clientID, robot, -1, position, simx_opmode.oneshot_wait);
                        if (error == 0)
                        {
                            CoordenadaX = (-position[0] + float.Parse(tbLonX.Text)) * float.Parse(tbEscalaSim.Text);
                            CoordenadaY = (position[1] + float.Parse(tbLonY.Text)) * float.Parse(tbEscalaSim.Text);

                            //Añadir error
                            CoordenadaX += Error.Next(0, 8) - 4;
                            CoordenadaY += Error.Next(0, 8) - 4;

                            retorno_posicion = true;
                            lblCoorX.Text = CoordenadaX.ToString();
                            lblCoorY.Text = CoordenadaY.ToString();
                        }
                    }
                }
                else
                {
                    if ((lblPosBal1.Text == "") || (lblPosBal2.Text == "") || (lblPosOk.BackColor != Color.LightGreen))
                        retorno_posicion = false;
                    else
                        retorno_posicion = true;
                }

                PintarGrafo();

                if (retorno_posicion)
		        {
                    //Última posición calculada con la información recibida del robot
                    if (Simulador)
                    {
                        Xpos = CoordenadaX;
                        Ypos = CoordenadaY;
                        MODULO_MIN_MEDIDA = 9;
                    }
                    else
                    {
                        Xpos = CoordenadaX * 10 / Convert.ToDouble(tbEscalaX.Text);
                        Ypos = CoordenadaY * 10 / Convert.ToDouble(tbEscalaY.Text);
                        MODULO_MIN_MEDIDA = 18;
                    }

                    ms_inicio_simulacion = ds * 100;

                    long cl = clock();

			        //Realizamos el control de movimiento cada MS_CONTROL
			        if ((Simulador?(cl % MS_CONTROL) == 0:true))
			        {
				        ticks++;

                        if (ms != 0)
                            lblms.Text = ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - ms)/10).ToString();
                        ms = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                        if (fase == Fases.arranque_inicial)
                        { //En el arranque debemos avanzar X segundos para poder establecer el vector de dirección
                            LOG("arranque_inicial");
                            Xini = Xpos;
                            Yini = Ypos;

                            if (cbRobot.Text == "DANI")
                            {
                                VelRuedaDer = VELOCIDAD_ANGULAR_ARRANQUE_DER_RODI;
                                VelRuedaIzq = VELOCIDAD_ANGULAR_ARRANQUE_IZQ_RODI;
                            }
                            else
                            {
                                if (Simulador)
                                {
                                    VelRuedaDer = VELOCIDAD_ANGULAR_ARRANQUE_SIM;
                                    VelRuedaIzq = VELOCIDAD_ANGULAR_ARRANQUE_SIM;
                                }
                                else if (cbRobot.Text == "RODI")
                                {
                                    VelRuedaDer = 9;
                                    VelRuedaIzq = 10;
                                }
                                AsignarVelocidad(VelRuedaDer, VelRuedaIzq);
                            }
                            fase = Fases.establecer_vector_direccion;

                            Espera(TIEMPO_RECORRIDO_INICIAL, 0, OP_INICIAR);
                        }
                        else if (fase == Fases.establecer_vector_direccion)
                        {
                            Application.DoEvents();
                            if (!Espera(TIEMPO_RECORRIDO_INICIAL, 0, OP_ESPERA))
                            {
                                LOG("establecer_vector_direccion");
                                fase = Fases.iniciar_ruta;

                                SigNodo = ListaNodosRuta[IndSigNodo];
                                VectorDireccionTrayectoria = DireccionVector(Xini, Yini, SigNodo.X, SigNodo.Y);

                                if (Simulador)
                                    AsignarVelocidad(0, 0);
                                else
                                {
                                    AsignarVelocidad(PARAR_MOTOR, PARAR_MOTOR);
                                    if (chkKalman.Checked)
                                    {
                                        Espera(TIEMPO_ESTABILIZACION_KALMAN, 0, OP_INICIAR);
                                        while (Espera(TIEMPO_ESTABILIZACION_KALMAN, 0, OP_ESPERA)) ;//Esperamos un tiempo para que se estabilice el filtro
                                    }
                                }
                            }
                        } //Tan(a) = y/x  v=<x1-x2, y1-y2>
                        else if (fase == Fases.iniciar_ruta)
                        {
                            double DistanciaNodo = 0;
                            double ModuloVectorDireccionRobot;
                            //Esperamos a que se estabilice la medida
                            //Espera(1000, 0, OP_INICIAR);
                            //while (Espera(1000, 0, OP_ESPERA))
                            //    Application.DoEvents();

                            //Calculamos el vector de desplazamiento del robot
                            if (Orientacion != -1)
                            {
                                VectorDireccionRobot = Orientacion;
                                ModuloVectorDireccionRobot = MODULO_MIN_MEDIDA;
                                Orientacion = -1;
                                picRadar.Refresh();
                            }
                            else
                            {
                                VectorDireccionRobot = DireccionVector(Xini, Yini, Xpos, Ypos);
                                ModuloVectorDireccionRobot = ModuloVector(Xini, Yini, Xpos, Ypos);
                            }

                            VectorDireccionAproximacion = DireccionVector(Xpos, Ypos, SigNodo.X, SigNodo.Y);

                            double VelocidadFijaDer = MAX_VEL_FIJA_SIM;
                            double VelocidadFijaIzq = MAX_VEL_FIJA_SIM;

                            //Si el desplazamiento ha sido menor que el error medio preferimos esperar por una nueva medida ************************
                            if (ModuloVectorDireccionRobot < MODULO_MIN_MEDIDA)
                                continue;
                            else //Calculamos la media del punto final
                            {
                                aXpos[iContMedidas] = Xpos;
                                aYpos[iContMedidas] = Ypos;
                                iContMedidas++;
                                if (iContMedidas < NUM_MEDIDAS)
                                {
                                    continue;
                                }
                                else
                                {
                                    double XMedia = 0, YMedia = 0;
                                    for (int i = 0; i < NUM_MEDIDAS; i++)
                                        XMedia += aXpos[i];
                                    for (int i = 0; i < NUM_MEDIDAS; i++)
                                        YMedia += aYpos[i];
                                    XMedia /= NUM_MEDIDAS;
                                    YMedia /= NUM_MEDIDAS;
                                    Xpos = XMedia;
                                    Ypos = YMedia;
                                    iContMedidas = 0;
                                }
                            }

                            pbMedidas.Value = (int)ModuloVectorDireccionRobot > pbMedidas.Maximum ? pbMedidas.Maximum : (int)ModuloVectorDireccionRobot;

                            myPID.mySetpoint = 0;
                            DifVectorTrayectoria = 0;
                            DifVectorDireccionH = 0;
                            ModH = 0;

                            //Calculamos la diferencia de dirección entre los vectores
                            if (NodoAct == null)
                                DifVector = DifAngular(VectorDireccionAproximacion, VectorDireccionRobot);
                            else
                            {
                                DifVector = DifAngular(VectorDireccionAproximacion, VectorDireccionRobot);
                                DifVectorTrayectoria = DifAngular(VectorDireccionTrayectoria, VectorDireccionAproximacion);

                                //Calculamos la distancia con la trayectoria
                                ModSegTray = ModuloVector(NodoAct.Value.X, NodoAct.Value.Y, SigNodo.X, SigNodo.Y);
                                ModDirAprox = ModuloVector(Xpos, Ypos, SigNodo.X, SigNodo.Y);
                                ModIniTray = ModuloVector(NodoAct.Value.X, NodoAct.Value.Y, Xpos, Ypos);
                                double s = (ModSegTray + ModDirAprox + ModIniTray) / 2;
                                double h = 2 / ModSegTray * Math.Sqrt(s * (s - ModSegTray) * (s - ModDirAprox) * (s - ModIniTray));
                                //Calculamos la distancia del final a la que se encuentra la altura
                                double d = Math.Sqrt(Math.Pow(ModDirAprox, 2) - Math.Pow(h, 2));
                                //d = Math.Pow(ModDirAprox, 2) / ModSegTray;
                                double X1 = NodoAct.Value.X;
                                double Y1 = NodoAct.Value.Y;
                                double X2 = SigNodo.X;
                                double Y2 = SigNodo.Y;
                                double A, B;
                                EcuacionRecta(X1, Y1, X2, Y2, out A, out B);
                                double h1 = Math.Abs((A * Xpos - Ypos + B)) / Math.Sqrt(Math.Pow(A, 2) + 1);

                                //Calculamos la razón de la intersección de h (fracción entre los dos segmentos que dividen la trayectoria)
                                double r = (ModSegTray - d) / d;
                                //Obtenemos las coordenadas de la intersección de la altura con la trayectoria
                                Xd = ((double)NodoAct.Value.X + r * SigNodo.X) / (1 + r);
                                Yd = ((double)NodoAct.Value.Y + r * SigNodo.Y) / (1 + r);

                                ModH = ModuloVector(Xpos, Ypos, Xd, Yd);
                                VectorDireccionH = DireccionVector(Xd, Yd, Xpos, Ypos);
                                DifVectorDireccionH = DifAngular(VectorDireccionTrayectoria, VectorDireccionH);
                            }

  #region "Dibujar vectores"
                            //Pintamos los dos vectores
                            PintarVector v1 = new PintarVector(); //Vector de orientación del robot en tiempo real
                            PintarVector v2 = new PintarVector(); //VEctor de aproximación al siguiente nodo
                            PintarVector v3 = new PintarVector(); //Vector distancia perpendicular a la trayectoria
                            PintarVector v4 = new PintarVector(); //Vector de alejamiento del nodo anterior
                            PintarVector v5 = new PintarVector(); //Vector de trayectoria del robot (más largo que el de orientación)

                            v1.ini = new Point((int)Xini, (int)Yini);
                            v1.fin = new Point((int)Xpos, (int)Ypos);
                            v1.color = Color.Red;

                            v2.ini = new Point((int)Xpos, (int)Ypos);
                            v2.fin = new Point((int)SigNodo.X, (int)SigNodo.Y);
                            v2.color = Color.Blue;

                            ListaVectores.Clear();
                            if (XiniAnt != 0)
                            {
                                v5.ini = new Point((int)XiniAnt, (int)YiniAnt);
                                v5.fin = new Point((int)Xpos, (int)Ypos);
                                v5.color = Color.LightCyan;
                                ListaVectores.Add(v5);
                            }
                            if (NodoAct != null)
                            {
                                //Inicio de segmento
                                v4.ini = new Point((int)NodoAct.Value.X, (int)NodoAct.Value.Y);
                                v4.fin = new Point((int)Xpos, (int)Ypos);
                                v4.color = Color.LightPink;
                                ListaVectores.Add(v4);
                                //Distancia perpendicular a la trayectoria
                                v3.ini = new Point((int)Xpos, (int)Ypos);
                                v3.fin = new Point((int)Xd, (int)Yd);
                                v3.color = Color.Green;
                                ListaVectores.Add(v3);
                            }
                            ListaVectores.Add(v1);
                            ListaVectores.Add(v2);
  #endregion

                            double AngMayor = VectorDireccionAproximacion > VectorDireccionRobot ? VectorDireccionAproximacion : VectorDireccionRobot;
                            DifReal = Math.Abs(DifVector);

                            //Si DifVector es + se gira a la derecha, sino a la izquierda. DifReal contiene la diferencia en ángulo siempre +.
                            myPID.myInput = DifVector;
                            myPID.Compute();
                            //La respuesta del PID debe ser proporcional a la diferencia del ángulo
                            //Cuanto mas pequeño es el ángulo menor peso tiene la parte variable
                            //float Salida = Output / FACTOR_SALIDA;
                            double Salida = 0;
                            double SalidaPID = 0;

                            SalidaPID = Margenes(myPID.myOutput, 0, MAX_VALOR_OUT_PID, 0, MAX_VEL_ANG);

                            Salida = Margenes(DifReal, 0, 180, 0, MAX_VEL_ANG);

                            if ((DifReal < MIN_GRADOS_RECTA) && (Salida < 0.2))
                            {
                                if (Simulador)
                                {
                                    VelocidadFijaDer = MAX_VEL_FIJA_SIM;
                                    VelocidadFijaIzq = MAX_VEL_FIJA_SIM;
                                }
                                else if (cbRobot.Text == "RODI")
                                {
                                    VelocidadFijaDer = MAX_VEL_RODI_DER;
                                    VelocidadFijaIzq = MAX_VEL_RODI_IZQ;
                                }
                                lblPosOk.ForeColor = Color.Red;
                            }
                            else
                            {
                                if (Simulador)
                                {
                                    VelocidadFijaDer = MIN_VEL_FIJA_SIM;
                                    VelocidadFijaIzq = MIN_VEL_FIJA_SIM;
                                }
                                else if (cbRobot.Text == "RODI")
                                {
                                    VelocidadFijaDer = MIN_VEL_RODI_DER;
                                    VelocidadFijaIzq = MIN_VEL_RODI_IZQ;
                                }
                                lblPosOk.ForeColor = Color.Black;
                            }

                            Console.WriteLine("vt: " + VectorDireccionAproximacion + " - vr: " + VectorDireccionRobot + " Dif.Vector: " + DifVector + " - PID: " + Salida + " - Vel: " + (MAX_VEL_ANG - Salida));

                            LOG(DifVector, Salida, VectorDireccionAproximacion, DifVectorDireccionH);

                            //Si en algún momento estamos mejor orientados con algun nodo posterior y no obstáculos por el medio, cambiamos de nodo de referencia

                            //Si estamos alejados de la trayectoria, no permitimos que se gire para alejarse más
                            //Siempre que la trayectoria y el vector de aproximación apunten al mismo lado
                            ProhibirGiro = 0;
                            if (DifVectorTrayectoria < 75)
                            {
                                if (ModH > MIN_DISTANCIA_TRAYECTORIA_CONTROL)
                                {
                                    if (DifVectorDireccionH < 0)
                                    {
                                        ProhibirGiro = GIRO_DERECHA;
                                        tbDer.BackColor = Color.Red;
                                        tbIzq.BackColor = Color.White;
                                    }
                                    else
                                    {
                                        ProhibirGiro = GIRO_IZQUIERDA;
                                        tbIzq.BackColor = Color.Red;
                                        tbDer.BackColor = Color.White;
                                    }
                                }
                                else
                                {
                                    tbIzq.BackColor = Color.White;
                                    tbDer.BackColor = Color.White;
                                }
                            }
                            else
                            {
                                tbIzq.BackColor = Color.White;
                                tbDer.BackColor = Color.White;
                            }

                            if ((DifVector > 0))
                            {
                                VelRuedaDer = VelocidadFijaDer;
                                if (ProhibirGiro != GIRO_DERECHA)
                                {
                                    VelRuedaIzq = VelocidadFijaIzq + Salida;
                                    pbDer.Value = (int)Salida;
                                }
                                else
                                    VelRuedaIzq = VelocidadFijaIzq;
                            }
                            else
                            {
                                VelRuedaIzq = VelocidadFijaIzq;
                                if (ProhibirGiro != GIRO_IZQUIERDA)
                                {
                                    VelRuedaDer = VelocidadFijaDer + Salida;
                                    pbIzq.Value = (int)Salida;
                                }
                                else
                                    VelRuedaDer = VelocidadFijaDer;
                            }
                            AsignarVelocidad(VelRuedaDer, VelRuedaIzq);

                            if (ModuloVectorDireccionRobot > MIN_MODULO_ACT_POSICION)
                            {
                                XiniAnt = Xini;
                                YiniAnt = Yini;

                                Xini = Xpos;
                                Yini = Ypos;
                            }

                            //Comprobamos la distancia a la que nos encontramos del nodo. Si es menor a la mínima saltamos de nodo de referencia
                            DistanciaNodo = ModuloVector(Xpos, Ypos, PosicionRobot(SigNodo.X), PosicionRobot(SigNodo.Y));

                            if (DistanciaNodo < DISTANCIA_SALTO_NODO)
                            {
                                IndSigNodo++;
                                if (IndSigNodo < ListaNodosRuta.Count)
                                {
                                    NodoAct = SigNodo;
                                    SigNodo = ListaNodosRuta[IndSigNodo];
                                    VectorDireccionTrayectoria = DireccionVector(NodoAct.Value.X, NodoAct.Value.Y, SigNodo.X, SigNodo.Y);
                                }
                                else
                                {
                                    ListaVectores = new List<PintarVector>();
                                    PintarGrafo();
                                    return;
                                }
                            }

                        }

                        //Recordamos la posición del último punto de control
                        Xant = Xpos;
				        Yant = Ypos;
			        }
		        }
                Application.DoEvents();
            }
            ListaVectores = new List<PintarVector>();
            PintarGrafo();
            return;
        }
        double DifAngular(double a1, double a2)
        {
            double a = a1 - a2;
            if (Math.Abs(a) <= 180) return a;

            if (a1 > a2)
                return  -((360 - a1) + a2);
            else
                return  ((360 - a2) + a1);
        }
        void EcuacionRecta(double X1, double Y1, double X2, double Y2, out double A, out double B)
        {
            A = (Y2 - Y1) / (X2 - X1);
            B = -A * X1 + Y1;
        }
        const long MAX_US = 4294967295;

        static long[] us_ini = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static long[] us_actual = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        bool Espera(long ms, int contador, int op)
        {

            us_actual[contador] = micros();

            if (op == OP_INICIAR)
                us_ini[contador] = micros();
            else
            {
                if (us_actual[contador] < us_ini[contador])
                {
                    us_actual[contador] = MAX_US - us_ini[contador] + us_actual[contador];
                    us_ini[contador] = 1;
                }

                if (us_actual[contador] - us_ini[contador] >= ms*1000)
                {
                    us_ini[contador] = 0;
                    return false;
                }
                else
                    return true;
            }
            return false;
        }

        double ModuloVector(double X1, double Y1, double X2, double Y2)
        {
            return Math.Sqrt(Math.Pow(X2 - X1, 2) + Math.Pow(Y2 - Y1, 2));
        }

        double DireccionVector(double X1, double Y1, double X2, double Y2)
        {
            //                      (0,0)
            //   180 grados <------------------> Eje X 0 grados
            //                       |\     
            //                       | \
            //                       |  \ 55 grados
            //                       v 
            //                     Eje Y
            //                   90 grados
            //Devuelve el ángulo del vector de 0 a 360º en sentido antihorario
            double X = X2 - X1;
            double Y = Y2 - Y1;
            double angulo = Math.Atan2(Y, X) * 180 / Math.PI;
            if (angulo < 0)
                angulo = angulo + 180 * 2;

            return angulo;
        }
        long micros()
        {
            return clock();
        }

        double PosicionRobot(double PosPlano)
        {
            return PosPlano;
            //return (float)PosPlano / Escala - DESPLAZAMIENTO_COOR_ROBOT;
        }
        double PosicionPlano(float PosRobot)
        {
            return PosRobot;
            //return (PosRobot + DESPLAZAMIENTO_COOR_ROBOT) * Escala;
        }

        static int ContadorPosicion = 0;

        public bool SeguimientoActivo = false;

        private void tmrControlPosicion_Tick(object sender, EventArgs e)
        {
            ContadorPosicion++;
            if (ContadorPosicion == 1)
                lblPosOk.BackColor = Color.Yellow;
            else if (ContadorPosicion == 2)
                lblPosOk.BackColor = Color.Red;
            else if (ContadorPosicion == 6)
                lblPosOk.BackColor = Color.Black;
        }

        private void tmrActualizarPosicion_Tick(object sender, EventArgs e)
        {
            lblPos.BackColor = lblPosOk.BackColor;

            lblms.Text = Puerto.NumeroLecturas.ToString();
            if (chkLecturaCOM.Checked )
                Console.WriteLine(Puerto.Lectura);

            if ((dBal1 != 0) && (dBal2 != 0))
            {
                tmrControlPosicion.Stop();
                ContadorPosicion = 0;
                tmrControlPosicion.Enabled = true;
                tmrControlPosicion.Start();

                lblPosBal1.Text = (dBal1 + LONG_APOTEMA_BALIZA + LONG_APOTEMA_ROBOT + AJUSTE_X).ToString(); 
                lblPosBal2.Text = (dBal2 + LONG_APOTEMA_BALIZA + LONG_APOTEMA_ROBOT + AJUSTE_Y).ToString(); 
                lblRawBal1.Text = dBal1.ToString();
                lblRawBal2.Text = dBal2.ToString();
                lblSensorBal1.Text  = iSensorBal1.ToString();
                lblSensorBal2.Text  = iSensorBal2.ToString();

                dBal1 = 0;
                dBal2 = 0;

                double Xcoor = 0, Ycoor = 0;
                double XposKalman = 0, YposKalman = 0;
                CalcularInterseccion(Convert.ToDouble(lblPosBal1.Text) , Convert.ToDouble(lblPosBal2.Text) , ref Xcoor, ref Ycoor) ;
                //Lo pasamos de mm a cm de nuevo x10
                Xcoor = Xcoor / 10;
                Ycoor = Ycoor / 10;
                lblCoorX.Text = Xcoor.ToString();
                lblCoorY.Text = Ycoor.ToString();

                CoordenadaX = Xcoor; 
                CoordenadaY = Ycoor;
                //Panel grande
                tbPos2X.Text = ((int)Xcoor).ToString(); 
                tbPos2Y.Text = ((int)Ycoor).ToString();

                CalcularInterseccion(Convert.ToDouble(lblPosBal1.Text) , Convert.ToDouble(lblPosBal2.Text) , ref XposKalman, ref YposKalman);

                //LOG(Math.Round(Xcoor,3), Math.Round(Ycoor,3), Math.Round(XposKalman, 3), Math.Round(YposKalman, 3));

                lblCoorX.Refresh();
                lblCoorY.Refresh();
                lblPosBal1.Refresh();
                lblPosBal2.Refresh();
                lblPosOk.BackColor = Color.LightGreen;
                lblPosOk.Refresh();
            }
        }

        private void cmdPanel_Click(object sender, EventArgs e)
        {
            if (cmdPanel.Tag.ToString() == "OFF")
            {
                cmdPanel.Tag = "ON";
                grpPanel.Visible = true;
                cmdPanel.Text = "Ocultar Panel";
            }
            else
            {
                cmdPanel.Tag = "OFF";
                grpPanel.Visible = false;
                cmdPanel.Text = "Mostrar Panel";
            }
        }

        private void cmdColocarBaliza_Click(object sender, EventArgs e)
        {
            Estado = Estados.ColocarBaliza;
            SubEstado = Estados.NodoInicial;
            ActivarBoton("cmdColocarBaliza");
        }

        void AsignarVelocidad(double D, double I)
        {
            if (Simulador)
            {
                //CAmbiamos el signo, ya que el simulador necesita velocidad negativas, pero arduino positivas
                simxSetJointTargetVelocity(gclientID, motor_derecho, D, simx_opmode_oneshot);
                simxSetJointTargetVelocity(gclientID, motor_izquierdo, I, simx_opmode_oneshot);
            }
            else
            {
                Puerto.serialPort.Write("mot" + ((int)D).ToString("0##") + "," + ((int)I).ToString("0##") + ".");
            }
        }

        private void tmrPintarRobot_Tick(object sender, EventArgs e)
        {
            if (lblPosOk.BackColor == Color.LightGreen)
                PintarGrafo();
        }

        private void cmdSeguirRuta_Click(object sender, EventArgs e)
        {
            Simulador = (cbRobot.Text == "Simulador"); 
            if (cmdSeguirRuta.Text != "Parar Seguimiento")
            {
                cmdSeguirRuta.Text = "Parar Seguimiento";
                SeguimientoActivo = true;
                if (Simulador)
                {
                    int portNb = 19997;
                    clientID = VREPWrapper.simxStart("127.0.0.1", portNb, true, true, 5000, 5);

                    //Si la conexión es exitosa iniciar la simulación
                    if (clientID > -1)
                    {
                        VREPWrapper.simxStartSimulation(clientID, (simx_opmode)simx_opmode_oneshot);
                        ControlVelocidadRobot(clientID);
                        VREPWrapper.simxStopSimulation(clientID, (simx_opmode)simx_opmode_oneshot_wait);
                    }
                    VREPWrapper.simxFinish(clientID);
                }
                else if (cmdInicarRobot.Tag.ToString() == "ON")
                    ControlVelocidadRobot(0);
                else
                    MessageBox.Show("Robot no conectado");
            }
            cmdSeguirRuta.Text = "Seguir Ruta";
            SeguimientoActivo = false;
        }

        private void cmdMapear_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Mapear(1, 79, 112, 0, 50, Convert.ToInt16(tbQ.Text)).ToString("0#") + " - d " + Mapear(-1, 69, 0, 0, 50, Convert.ToInt16(tbQ.Text)).ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Puerto.serialPort.Write("kl" + tbQ.Text + tbR.Text);
        }

        private void cmdBorrarBalizas_Click(object sender, EventArgs e)
        {
            ListaBalizas = new List<Baliza>();
            PintarGrafo();
        }

        private void cmdBorrarNodos_Click(object sender, EventArgs e)
        {
            ListaNodos = new List<Nodo>();
            ListaEnlaces = new List<Enlace>();
            PintarGrafo();
        }

        private void cmdVerLOG_Click(object sender, EventArgs e)
        {
            if (panelLOG.Visible)
                panelLOG.Visible = false;
            else
                panelLOG.Visible = true;

            if (this.Location.X > 0)
                panelLOG.Location = new Point(0, 12);
            else
                panelLOG.Location = new Point(788, 12);

        }

        private void LOG(string cad, double d1, double d2, double d3, double d4)
        {
            tbLOG.Text = tbLOG.Text + cad;
            LOG(d1, d2, d3, d4);
        }
        private void LOG(double d1, double d2, double d3, double d4)
        {
            tbLOG.Text = tbLOG.Text + Math.Round(d1,2).ToString() + "; " + Math.Round(d2, 2).ToString() + "; " + Math.Round(d3, 2).ToString() + "; " + Math.Round(d4, 2).ToString() + "; " + Environment.NewLine;
            tbLOG.SelectionStart = tbLOG.Text.Length;
            tbLOG.ScrollToCaret();
        }
        private void LOG(string cad)
        {
            tbLOG.Text = tbLOG.Text + cad + Environment.NewLine;
            tbLOG.SelectionStart = tbLOG.Text.Length;
            tbLOG.ScrollToCaret();
        }

        private void cmdCopiarLOG_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(tbLOG.Text);
        }

        private void cmdBorrarLOG_Click(object sender, EventArgs e)
        {
            tbLOG.Text = "";
            if (!Simulador)
                Puerto.NumeroLecturas = 0;
        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            LOG(DifAngular(0, 90), DifAngular(0, 270), DifAngular(180, 90), DifAngular(180, 270));
            return;

            // 0,0 esquina inferior izquierda. 0 grados ----->  dirección antihoraria
            LOG(DireccionVector(10, 10, 20, 10), DireccionVector(20, 20, 20, 10),0,0);
            return;

            Medida Medida1 = new Medida();
            Medida Medida2 = new Medida();
            Medida1.Distancia = 80;
            Medida1.IDBaliza = "2";
            Medida2.Distancia = 80;
            Medida2.IDBaliza = "1";
            double EscalaX = Convert.ToDouble(tbEscalaX.Text);
            double EscalaY = Convert.ToDouble(tbEscalaY.Text);

            double Xpos = 0, Ypos = 0;

            InterseccionCircunferencias(Medida1, Medida2, ref Xpos, ref Ypos, 4, 4);

            double v1x1, v1y1, v1x2, v1y2;
            Baliza Baliza2 = ListaBalizas.Find(X => X.Id == Medida1.IDBaliza);
            Baliza Baliza1 = ListaBalizas.Find(X => X.Id == Medida2.IDBaliza);

            //Calculamos la distancia entre los dos puntos
            v1x1 = Baliza1.PosX/EscalaX ;
            v1y1 = Baliza1.PosY/EscalaY ;
            

            v1x2 = Baliza2.PosX/EscalaX ;
            v1y2 = Baliza2.PosY/EscalaY;

            Xpos = Xpos;
            Ypos = Ypos;

            lblCoorX.Text = Xpos.ToString();
            lblCoorY.Text = Ypos.ToString();
            //PintarGrafo();
            tmrPintarRobot.Enabled = true;
            lblPosOk.BackColor = Color.LightGreen;

            MessageBox.Show(Xpos.ToString() + " - " + Ypos.ToString() + Environment.NewLine+ v1x1+", "+v1y1+" - "+v1x2+", "+v1y2 + " - " + (v1y1-v1y2));
            
        }

        double Margenes(double valor, double minv, double maxv, double mins, double maxs)
        {
            if (valor > maxv) valor = maxv;

            return (valor - minv) / maxv * maxs + mins;
        }

        //Delegados ********************************************************************************************************************************************************************************
        void RecibidaPosicion(double pdBal1, double pdBal2, long[] aDistancias, int SensorBal1, int SensorBal2, int op)
        {
            switch (op)
            {
                case OP_POSICION:
                    dBal1 = pdBal1;
                    dBal2 = pdBal2;
                    iSensorBal1 = SensorBal1;
                    iSensorBal2 = SensorBal2;
                    ContadorPosicion = -1;
                    break;
                case OP_ORIENTACION:
                    Orientacion = pdBal1;
                    BalOrientacion = Convert.ToInt16(pdBal2);
                    break;
                case OP_SONAR:
                    Distancias = aDistancias;
                    break;
            }

        }

        private void cmdOrientacion_Click(object sender, EventArgs e)
        {

            cmdOrientacion.Enabled = false;
            RecuperandoOrientacion = true;
            RecuperarOrientacion();
            RecuperandoOrientacion = false;
            cmdOrientacion.Enabled = true;

        }
        void RecuperarDistanciasSonar()
        {
            Distancias[0] = -1;
            Espera(300, 0, OP_INICIAR);
            Puerto.serialPort.Write("son1.");
            while (Espera(6000, 0, OP_ESPERA) && (Distancias[0] == -1)) { Thread.Yield(); Application.DoEvents(); }
            if (Distancias[0] != -1)
                MostrarDistanciasSonar((int)Distancias[0]);
        }
        void RecuperarOrientacion()
        {
            double dOrientacion = 0;
            Orientacion = -1;
            dOrientacion = -1;
            Espera(300, 0, OP_INICIAR);
            Puerto.serialPort.Write("ori1.");
            while (Espera(6000, 0, OP_ESPERA) && (Orientacion == -1)) Thread.Yield();
            LOG(Orientacion, 0, 0, 0);
            if (Orientacion != -1)
                dOrientacion = Orientacion;

            //Dorientacion es la orientación con respecto al segmento que une la posición del robot con la baliza

            Point pRobot = new Point((int)(Convert.ToDouble(lblCoorX.Text)*10), (int)(Convert.ToDouble(lblCoorY.Text)*10));
            Baliza pBaliza = ListaBalizas.Find(a => a.Id == "1");
            double DirVectorBaliza = DireccionVector((int)pRobot.X, (int)pRobot.Y, 
                                                (int)(pBaliza.PosX*Convert.ToDouble(tbEscalaX.Text)), (int)(pBaliza.PosY * Convert.ToDouble (tbEscalaY.Text)));
            double DireccionGlobal = 0;

            if (dOrientacion >= 0 && dOrientacion < 180)
                DireccionGlobal = DirVectorBaliza - dOrientacion;
            else
                DireccionGlobal = DirVectorBaliza + 360 - dOrientacion;

            picRadar.Refresh();
            MostrarOrientacion(DireccionGlobal, Color.White);
            MostrarOrientacion(dOrientacion, Color.Green);
        }
        void CalcularInterseccion(double DistanciaBal1, double DistanciaBal2, ref double Xpos, ref double Ypos)
        {
            Medida Medida1 = new Medida();
            Medida1.IDBaliza = "1";
            Medida1.Distancia = DistanciaBal1*10; //Pasamos de cm a mm
            Medida Medida2 = new Medida();
            Medida2.IDBaliza = "2";
            Medida2.Distancia = DistanciaBal2*10;
            double EscalaX = Convert.ToDouble(tbEscalaX.Text);
            double EscalaY = Convert.ToDouble(tbEscalaY.Text);
            InterseccionCircunferencias(Medida1, Medida2, ref Xpos, ref Ypos, EscalaX, EscalaY);
        }

        private void cmdPos_Click(object sender, EventArgs e)
        {
            if (cmdInicarRobot.Tag.ToString() == "ON")
            {
                if (cmdPos.Tag.ToString() == "ON")
                {
                    Puerto.serialPort.Write("pos0.");
                    cmdPos.Tag = "OFF";
                    cmdPos.BackColor = Color.Red;
                }
                else 
                {
                    Puerto.serialPort.Write("pos1.");
                    cmdPos.Tag = "ON";
                    cmdPos.BackColor = Color.Green;
                }
            }
        }

        private void cmdReset_Click(object sender, EventArgs e)
        {
            Puerto.serialPort.Write("reset.");
        }

        private void pbArriba_Click(object sender, EventArgs e)
        {
            if (cmdInicarRobot.Tag.ToString() == "ON")
                Puerto.serialPort.Write("mot" + ((int)MAX_VEL_RODI_DER).ToString("0##") + "," + ((int)MAX_VEL_RODI_IZQ).ToString("0##") + ".");
                //Puerto.serialPort.Write("ava1.");
        }

        private void pbIzquierda_Click(object sender, EventArgs e)
        {
            if (cmdInicarRobot.Tag.ToString() == "ON")
                Puerto.serialPort.Write("ret1.");
        }

        private void pbDerecha_Click(object sender, EventArgs e)
        {
            if (cmdInicarRobot.Tag.ToString() == "ON")
                Puerto.serialPort.Write("izq1.");
        }

        private void pbAbajo_Click(object sender, EventArgs e)
        {
            if (cmdInicarRobot.Tag.ToString() == "ON")
                Puerto.serialPort.Write("der1.");
        }

        private void pbParar_Click(object sender, EventArgs e)
        {
            if (cmdInicarRobot.Tag.ToString() == "ON")
                Puerto.serialPort.Write("par1.");
        }

        private void pbArriba_MouseDown(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30,35);
        }

        private void pbArriba_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30, 26);
        }

        private void chkKalman_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkKalman.Checked)
                ComunicacionPuertoSerie.TipoMedida = 1;
            else
                ComunicacionPuertoSerie.TipoMedida = 2;
        }

        private void pbIzquierda_MouseDown(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30, 35);
        }

        private void pbIzquierda_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30, 26);
        }

        private void pbDerecha_MouseDown(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(35, 29);
        }

        private void pbDerecha_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30, 29);

        }

        private void pbAbajo_MouseDown(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(35, 29);
        }

        private void pbAbajo_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30, 29);
        }

        private void pbParar_MouseDown(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30, 35);
        }

        private void cmdRadar_Click(object sender, EventArgs e)
        {
            RecuperarDistanciasSonar();
        }

        private void chkCoppeliaSim_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pbArribaDespacio_Click(object sender, EventArgs e)
        {
            if (cmdInicarRobot.Tag.ToString() == "ON")
                //                Puerto.serialPort.Write("ava0.");
                Puerto.serialPort.Write("mot" + ((int)MIN_VEL_RODI_DER).ToString("0##") + "," + ((int)MIN_VEL_RODI_IZQ).ToString("0##") + ".");

        }

        private void pbArribaDespacio_MouseDown(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30, 35);
        }

        private void pbArribaDespacio_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30, 19);
        }

        private void cmdPos0_Click(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Location = new Point(600, 0);
        }

        private void pbParar_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).Size = new Size(30, 25);
        }

        int InterseccionCircunferencias(Medida Medida1, Medida Medida2, ref double lPosX, ref double lPosY, double EscX, double EscY)
        {
            double fA1, fB1, fC1, fA2, fB2, fC2;
                double D, E, F, a, b, c, d, x1, x2, y1, y2;
                double a1, b1, r1, a2, b2, r2;
                double v1x1, v1y1, v1x2, v1y2;

            Baliza Baliza1 = ListaBalizas.Find(X => X.Id == Medida1.IDBaliza);
            Baliza Baliza2 = ListaBalizas.Find(X => X.Id == Medida2.IDBaliza);

            //Calculamos la distancia entre los dos puntos
            v1x1 = (double)Baliza1.PosX * EscX;
            v1y1 = (double)Baliza1.PosY * EscY;
  
            v1x2 = (double)Baliza2.PosX * EscX;
            v1y2 = (double)Baliza2.PosY * EscY;

            a1 = v1x1;
            b1 = v1y1;
            a2 = v1x2;
            b2 = v1y2;
            r1 = Medida1.Distancia;
            r2 = Medida2.Distancia;
  
            fA1 = -2*a1;
            fB1 = -2*b1;
            fC1 = Math.Pow(a1, 2) + Math.Pow(b1, 2) - Math.Pow(r1, 2);
                fA2 = -2*a2;
            fB2 = -2*b2;
            fC2 = Math.Pow(a2, 2) + Math.Pow(b2, 2) - Math.Pow(r2, 2);

                D = fA1 - fA2;
            E = fB1 - fB2;
            F = fC1 - fC2;
  
            a = Math.Pow(E, 2)/ Math.Pow(D, 2) +1;
            b = (2*E* F - D* fA1*E + Math.Pow(D, 2)*fB1)/ Math.Pow(D, 2);
                c = Math.Pow(F, 2)/ Math.Pow(D, 2) - fA1* F/D + fC1;
  
            d = (Math.Pow(b, 2) - 4*a* c);
  
            if (d >= 0)
            {
                y1 = (-b + Math.Sqrt(d))/(2*a);
                x1 = (-E* y1 - F)/D;
                y2 = (-b - Math.Sqrt(d))/(2*a);
                x2 = (-E* y2 - F)/D;
    
                if ((y1 < 0) || (x1 < 0))
                {
                    lPosX = x2;
                    lPosY = y2;
                }
                else //Debemos escoger la solución correcta
                {
                    lPosX = x1;
                    lPosY = y1;
                }
    
                return 1;
            }
            else
            {
                //No se actualizan los valores
                return 0;
            }

        }

        void MostrarOrientacion(double Orientacion, Color color)
        {
            const int RADAR_X = 42;
            const int RADAR_Y = 42;

            if (Orientacion != -1)
            {
                Graphics g = picRadar.CreateGraphics();
                Pen p3 = new Pen(color, 3);
                Pen p2 = new Pen(color, 2);
                Pen p1 = new Pen(color, 1);
                LOG(Math.Cos(Math.PI / 2), Math.Sin(Math.PI / 2), 0, 0);

                g.DrawLine(p3, RADAR_X, RADAR_Y, RADAR_X + (int)(Math.Cos(Orientacion * Math.PI / 180) * 28), RADAR_Y + (int)(Math.Sin(Orientacion * Math.PI / 180) * 28));
                g.DrawLine(p2, RADAR_X, RADAR_Y, RADAR_X + (int)(Math.Cos(Orientacion * Math.PI / 180) * 32), RADAR_Y + (int)(Math.Sin(Orientacion * Math.PI / 180) * 32));
                g.DrawLine(p1, RADAR_X, RADAR_Y, RADAR_X + (int)(Math.Cos(Orientacion * Math.PI / 180) * 35), RADAR_Y + (int)(Math.Sin(Orientacion * Math.PI / 180) * 35));
            }
        }

        void MostrarDistanciasSonar(int NumSensores)
        {
            const int VALOR_DISTANCIA_CM_MIN = 1;
            const int VALOR_DISTANCIA_CM_MAX = 50;
            const int R_MAX = 28;
            const int REC_MIN = 28;

            picRadar.Refresh();
            Graphics g = picRadar.CreateGraphics();
            int AnguloIni;
            int AngTam;
            int r;

            Pen pen;

            for (int i = 0; i < MAX_SENSORES_US; i++)
            {
                r = (int)(0.0343*Distancias[i+1]); //cm 

                if (r > VALOR_DISTANCIA_CM_MAX)
                    r = R_MAX;
                else if (r < VALOR_DISTANCIA_CM_MIN)
                    r = 0;
                else
                    r = (int)((float)R_MAX / (VALOR_DISTANCIA_CM_MAX - VALOR_DISTANCIA_CM_MIN) * (r - VALOR_DISTANCIA_CM_MIN));

                AngTam = 360 / NumSensores;
                AnguloIni = AngTam * i;

                int CentroX = picRadar.Width / 2;
                int CentroY = picRadar.Height / 2;

                if (i == 0)
                    pen = new Pen(Color.White, 1);
                else
                    pen = new Pen(Color.Yellow, 1);

                for (int j = 0; j < r; j++)
                    g.DrawArc(pen, new Rectangle(REC_MIN - j, REC_MIN - j, picRadar.Width - 2 * REC_MIN + 2 * j, picRadar.Height - 2 * REC_MIN + 2 * j), AnguloIni, AngTam);

                g.DrawLine(new Pen(Color.LightPink, 2), CentroX, CentroY, CentroX * 2-2, CentroY);

                g.DrawLine(new Pen(Color.LightPink, 2), CentroX * 2-2, CentroY, CentroX*2- 6, CentroY + 6);
                g.DrawLine(new Pen(Color.LightPink, 2), CentroX * 2-2, CentroY, CentroX*2- 6, CentroY - 6);
            }
        }

    } //class

}

