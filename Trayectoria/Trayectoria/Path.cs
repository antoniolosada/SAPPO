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

namespace Trayectoria
{

    public partial class frmTrayectoria : Form
    {
        static Timer stmrControlPosicion;
        delegate void PosicionRecibida();

        const int MANUAL = 0;
        const int DIRECT = 0;
        const int REVERSE = 1;

        const int FACTOR_VELOCIDAD = 20;
        const double FACTOR_COMPENSACION_MOTOR_DER = 1;
        const double FACTOR_COMPENSACION_MOTOR_IZQ = 1.20;

        double CoordenadaX = 0;
        double CoordenadaY = 0;

        ComunicacionPuertoSerie Puerto;
        static double Kp = 2.0, Ki = 5, Kd = 1;
        PID myPID = new PID(Kp, Ki, Kd, DIRECT);

        bool ConfiguracionRecuperada = false;
        double PosX = 0, PosY = 0;
        int motor_derecho = 0;
        int motor_izquierdo = 1;
        int simx_opmode_oneshot = 0;
        int robot = 0;
        int gclientID =0;

        public const int TIEMPO_RECORRIDO_INICIAL = 1000; //Arramca durante 2s para establecer la dirección
        public const int MS_CONTROL = 1000;
        public const int MAX_MEDIDAS_DIR = 10;
        public const double VELOCIDAD_ANGULAR = 0.5;
        public const double VELOCIDAD_ANGULAR_MIN = 0.0001;
        public const double DISTANCIA_SALTO_NODO = 0.100; // 50cm
        public const int OP_INICIAR = 0;
        public const int OP_ESPERA = 1;

        public const int MAX_VALOR_OUT_PID = 100;
        public const int FACTOR_SALIDA = 50;
        public const double MAX_VEL_ANG = 0.8;	//Máxima diferencia de velocidad con corrección de trayectoria
        public const int VEL_MIN_RODI = 90;
        public const double MAX_VEL_FIJA = 0.5;	//Velocidad lineal sin corrección de trayectoria
        public const double MIN_VEL_FIJA = 0.8; //Velocidad lineal con corrección de trayectoria
        public const int MIN_GRADOS_RECTA = 8;

        List<Button> ListaBotones = new List<Button>();
        Configuration config;
        System.Collections.Specialized.NameValueCollection appSettings;
        const int PUNTO_ALTO = 17;
        const int PUNTO_ANCHO = 17;
        int NodoInicial = -1;
        int NodoFinal = -1;
        Point PuntoInicial;
        Point PuntoFinal;
        enum Estados { Vacio, InsertarNodos, UnirNodos, EstablecerCamino, NodoInicial, NodoFinal, BuscarRuta, MedirDistancia, DefinirObstaculos, ColocarBaliza };
        Estados Estado = Estados.Vacio;
        Estados SubEstado = Estados.Vacio;
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
                if (lblPosOk.BackColor == Color.Green)
                {
                    if ((lblCoorX.Text != "") && (lblCoorY.Text != ""))
                    {
                        int X = (int)Convert.ToDouble(lblCoorX.Text);
                        int Y = (int)Convert.ToDouble(lblCoorY.Text);
                        Point p = new Point((int)(X), (int)(Y));
                        Bitmap img = new Bitmap("Robot.bmp");
                        g.DrawImage(img, p);
                    }
                }

                g.Dispose();
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
            GrabarConfiguracion();
        }

        void GrabarConfiguracion()
        {
            config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            //Grabamos los enlaces
            int pos = 0;
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
                    double DistanciaPuntos = Distancia(PuntoInicial, PuntoFinal);
                    int Escala;
                    if (tbEscalaX.Text == "")
                        Escala = 0;
                    else
                        Escala = (int)Convert.ToInt16(tbEscalaX.Text);
                    MessageBox.Show("Distancia en puntos: " + DistanciaPuntos + ", mm: " + DistanciaPuntos * Escala);
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
                cmdInicarRobot.Text = "Iniciar Robot";
                cmdInicarRobot.Tag = "OFF";
                Puerto.serialPort.Write("stop-.");
                cmdInicarRobot.BackColor = Color.LightGray;
                tmrControlPosicion.Stop();
                tmrControlPosicion.Enabled = false;
                tmrActualizarPosicion.Enabled = false;
                tmrPintarRobot.Enabled = false;
            }
            else
            { //Abrir Puerto
                Puerto = new ComunicacionPuertoSerie(cbPuerto.Text, RecibidaPosicion);
                lblPosOk.BackColor = Color.White;
                tmrControlPosicion.Enabled = true;
                tmrActualizarPosicion.Enabled = true;
                tmrControlPosicion.Start();
                cmdInicarRobot.Text = "Parar Robot";
                cmdInicarRobot.Tag = "ON";
                cmdInicarRobot.BackColor = Color.LightGreen;
                tmrPintarRobot.Enabled = true;
                Puerto.serialPort.Write("stop-.");

                //ControlVelocidadRobot();
            }
        }

        int simxGetObjectPosition(int clientID, int robot, int op, ref float[] position, int simx_opmode_oneshot)
        {
            return 0;
        }
        int simxGetConnectionId(int clientID)
        {
            if (cmdInicarRobot.Tag.ToString() == "ON")
                return 1;
            else
                return -1;
        }
        int Mapear(int dir, int v1m, int v1M, int v2m, int v2M, int Velocidad)
        {
            double ratio = (double)Math.Abs(v1M - v1m) / (v2M - v2m);
            return (int)(dir * (Velocidad-v2m) * ratio + v1m);
        }
        void simxSetJointTargetVelocity(int clienID, int motor, double pos, int op)
        {
            if (cmdInicarRobot.Tag.ToString() == "OFF")
                return;

            int Velocidad = (int)(pos * FACTOR_VELOCIDAD);
            Console.WriteLine("v=" + Velocidad);
            if (motor == motor_derecho)
            {
                if (pos == 0)
                    Puerto.serialPort.Write("stop-.");
                else
                {
                    //0-200 => 0-52
                    Velocidad = Mapear(-1, 69, 0, 0, 50, Velocidad);
                    Console.WriteLine("VD= " + Velocidad.ToString());
                    Puerto.serialPort.Write("md" + Velocidad.ToString("0##") + ".");
                }
            }
            else
            {
                if (pos == 0)
                    Puerto.serialPort.Write("stop-.");
                else
                {
                    //0-200 => 80-180
                    Velocidad = Mapear(1, 79, 100, 0, 50, Velocidad);
                    Console.WriteLine("VI= " + Velocidad.ToString());
                    Puerto.serialPort.Write("mi" + Velocidad.ToString("0##") + ".");
                }
            }
        }
        enum Fases : int { arranque_inicial, establecer_vector_direccion, iniciar_ruta, seguir_ruta };
        void ControlVelocidadRobot()
        {
            double Xpos, Ypos;
            double Xant, Yant;
            int clientID = 0;
            bool retorno_posicion = false;
            float[] position = new float[3];
            Fases fase = Fases.arranque_inicial;
            long ms_inicio_simulacion = 0;
            double Xini = 0, Yini = 0;
            double VectorDireccionRobot;
            double VectorDireccionTrayectoria;
            double DifVector;
            double DifReal;
            int IndSigNodo = 1;
            long ticks = 0; //Número de iteraciones de control
            Nodo SigNodo = new Nodo();

            if (ListaNodosRuta.Count == 0)
            {
                MessageBox.Show("Debe Buscar antes la ruta");
                return;
            }

            simxSetJointTargetVelocity(clientID, motor_derecho, 0, simx_opmode_oneshot);
            simxSetJointTargetVelocity(clientID, motor_izquierdo, 0, simx_opmode_oneshot);

            while (simxGetConnectionId(clientID) != -1) //Este bucle funcionara hasta que se pare la simulacion
	        {
                Application.DoEvents();
		        long ms = clock();
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

                if ((lblPosX.Text == "") || (lblPosY.Text == "") || (lblPosOk.BackColor != Color.Green))
                    retorno_posicion = false;
                else
                    retorno_posicion = true;

                if (retorno_posicion)
		        {
                    //Última posición calculada con la información recibida del robot
                    Xpos = CoordenadaX;
                    Ypos = CoordenadaY;

			        ms_inicio_simulacion = ms;

                    long cl = clock();

			        //Realizamos el control de movimiento cada MS_CONTROL
			        if ((ms % MS_CONTROL) == 0)
			        {
				        ticks++;
				        //cout << Xpos << ", " << Ypos << "," << position[0] << "," << position[1] << "," << position[2] << "," << cl << endl;

				        if (fase == Fases.arranque_inicial)
				        { //En el arranque debemos avanzar X segundos para poder establecer el vector de dirección
					        Xini = Xpos;
					        Yini = Ypos;

                            AsignarVelocidad(-VELOCIDAD_ANGULAR, -VELOCIDAD_ANGULAR);
                            fase = Fases.establecer_vector_direccion;

                            Espera(TIEMPO_RECORRIDO_INICIAL, 0, OP_INICIAR);
                        }
			            else if (fase == Fases.establecer_vector_direccion)
			            {
                            Application.DoEvents();
				            if (!Espera(TIEMPO_RECORRIDO_INICIAL, 0, OP_ESPERA))
				            {
					            fase = Fases.iniciar_ruta;
					            SigNodo = ListaNodosRuta[IndSigNodo];

                                AsignarVelocidad(0, 0);
                            }
			            } //Tan(a) = y/x  v=<x1-x2, y1-y2>
                        else if (fase == Fases.iniciar_ruta)
			            {
				            double DistanciaNodo;
                            //Esperamos a que se estabilice la medida
                            Espera(1000, 0, OP_INICIAR);
                            while (Espera(1000, 0, OP_ESPERA))
                                Application.DoEvents();
                            //Calculamos el vector de desplazamiento del robot
                            Xpos = CoordenadaX;
                            Ypos = CoordenadaY;
                            VectorDireccionRobot = DireccionVector(Xini, Yini, Xpos, Ypos);
                            
                            //cout << Xini << " , " << Yini << " , " << Xpos << " , " << Ypos << "Vector dir robot: " << VectorDireccionRobot << endl;
                            //cout << "Vector dir robot: " << VectorDireccionRobot << endl;

                            //Calculamos la dirección del vector que une la posición actual del robot con el siguiente punto de la trayectoria
                            VectorDireccionTrayectoria = DireccionVector(Xpos, Ypos, PosicionRobot(SigNodo.X), PosicionRobot(SigNodo.Y));
				            //cout << Xpos << " , " << Ypos << " , " << PosicionRobot(SigNodo.X) << " , " << PosicionRobot(SigNodo.Y) << endl;
				            //cout << "Vector dir path: " << VectorDireccionTrayectoria << endl;

				            //Comprobamos la distancia a la que nos encontramos del nodo. Si es menor a la mínima saltamos de nodo de referencia
				            DistanciaNodo = ModuloVector(Xpos, Ypos, PosicionRobot(SigNodo.X), PosicionRobot(SigNodo.Y));
				            if (DistanciaNodo<DISTANCIA_SALTO_NODO)
				            {
					            IndSigNodo++;
					            SigNodo = ListaNodos[IndSigNodo];
				            }
				            //Calculamos la diferencia de dirección entre los vectores
				            myPID.mySetpoint = 0;
				            if (VectorDireccionRobot > 0)
				            {
					            DifVector = VectorDireccionTrayectoria - VectorDireccionRobot;
					            if (Math.Abs(DifVector) > 180)
						            DifReal = 360 - Math.Abs(DifVector);
					            else
						            DifReal = Math.Abs(DifVector);

                                myPID.myInput= DifReal;
					            myPID.Compute();


						        //La respuesta del PID debe ser proporcional a la diferencia del ángulo
						        //Cuanto mas pequeño es el ángulo menor peso tiene la parte variable


						        //float Salida = Output / FACTOR_SALIDA;
						        double Salida = Margenes(myPID.myOutput, 0, MAX_VALOR_OUT_PID, 0, MAX_VEL_ANG);

                                Salida = myPID.myOutput / (FACTOR_SALIDA);
						        double invSalida = MAX_VEL_ANG - Salida;

                                Console.WriteLine("vt: " + VectorDireccionTrayectoria + " - vr: " + VectorDireccionRobot + "Dif.Vector: "+ DifReal+" - PID: " + Salida + " - Vel: " + (MAX_VEL_ANG - Salida));

                                //float VelocidadFija = VEL_FIJA - Margenes(DifReal, 0, 30, 0, VEL_FIJA);

						        double VelocidadFija = MAX_VEL_FIJA;

						        if (DifReal<MIN_GRADOS_RECTA)

                                    VelocidadFija = MAX_VEL_FIJA;
						        else
							        VelocidadFija = MIN_VEL_FIJA;

						        //Si la diferencia del vector de dirección es positiva giramos a la izquierda, sino a la derecha
						        //Si la diferencia es superior a 180 grados realizamos el camino complementario por ser más corto

						        if (DifReal<MIN_GRADOS_RECTA)

                                    AsignarVelocidad(-VelocidadFija, -(VelocidadFija));
						        else if ((DifVector > 0) && (DifVector< 180))

                                    AsignarVelocidad(-VelocidadFija, -(VelocidadFija + invSalida));
						        else

                                    AsignarVelocidad(-(VelocidadFija + invSalida), -VelocidadFija);
						

						        //if ((DifVector > 0) && (DifVector < 180))
						        //	AsignarVelocidad(-(VEL_FIJA + Salida), -(VEL_FIJA + invSalida));
						        //else
						        //	AsignarVelocidad(-(VEL_FIJA + invSalida), -(VEL_FIJA + Salida));

						        //if ((DifVector > 0) && (DifVector < 180))
						        //	AsignarVelocidad(-Salida, -(MAX_VEL_ANG - Salida));
						        //else
						        //	AsignarVelocidad(-(MAX_VEL_ANG - Salida), -Salida);

						        //if ((DifVector > 0) && (DifVector < 180))
						        //	AsignarVelocidad(-VELOCIDAD_ANGULAR_MIN, -(MAX_VEL_ANG-Salida));
						        //else
						        //	AsignarVelocidad(-(MAX_VEL_ANG - Salida), -VELOCIDAD_ANGULAR_MIN);

						        Xini = Xpos;
						        Yini = Ypos;
					        }
				        }

				        //Recordamos la posición del último punto de control
				        Xant = Xpos;
				        Yant = Ypos;
			        }
		        }
    	    }
            Puerto.serialPort.Write("stop-.");
            Application.DoEvents();
            Puerto.CerrarPuerto();
            Puerto = null;
            return;
        }
        const long MAX_US = 4294967295;

        static long[] us_ini = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        bool Espera(long ms, int contador, int op)
        {
            long[] us_actual = new long[10];

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
            lblms.Text = (micros()/1000).ToString();
            if ((PosX != 0) && (PosY != 0))
            {
                tmrControlPosicion.Stop();
                ContadorPosicion = 0;
                tmrControlPosicion.Enabled = true;
                tmrControlPosicion.Start();
                lblPosX.Text = PosX.ToString();
                lblPosY.Text = PosY.ToString();
                PosX = 0;
                PosY = 0;
                Medida Medida1 = new Medida();
                Medida1.IDBaliza = "1";
                Medida1.Distancia = Convert.ToDouble(lblPosX.Text);
                Medida Medida2 = new Medida();
                Medida2.IDBaliza = "2";
                Medida2.Distancia = Convert.ToDouble(lblPosY.Text);
                double Xpos = 0, Ypos = 0;
                double EscalaX = Convert.ToDouble(tbEscalaX.Text);
                double EscalaY = Convert.ToDouble(tbEscalaY.Text);
                InterseccionCircunferencias(Medida1, Medida2, ref Xpos, ref Ypos, EscalaX, EscalaY);
                lblCoorX.Text = Xpos.ToString();
                lblCoorY.Text = Ypos.ToString();

                CoordenadaX = Xpos;
                CoordenadaY = Ypos;
                tbPos2X.Text = ((int)Xpos).ToString();
                tbPos2Y.Text = ((int)Ypos).ToString();
                lblCoorX.Refresh();
                lblCoorY.Refresh();
                lblPosX.Refresh();
                lblPosY.Refresh();
                lblPosOk.BackColor = Color.Green;
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
            //CAmbiamos el signo, ya que el simulador necesita velocidad negativas, pero arduino positivas

            simxSetJointTargetVelocity(gclientID, motor_derecho, -D, simx_opmode_oneshot);
            simxSetJointTargetVelocity(gclientID, motor_izquierdo, -I, simx_opmode_oneshot);
        }

        private void tmrPintarRobot_Tick(object sender, EventArgs e)
        {
            if (lblPosOk.BackColor == Color.Green)
                PintarGrafo();
        }

        private void cmdSeguirRuta_Click(object sender, EventArgs e)
        {
            ControlVelocidadRobot();
        }

        private void cmdMapear_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Mapear(1, 79, 112, 0, 50, Convert.ToInt16(tbQ.Text)).ToString("0#") + " - d " + Mapear(-1, 69, 0, 0, 50, Convert.ToInt16(tbQ.Text)).ToString());
        }

        double Margenes(double valor, double minv, double maxv, double mins, double maxs)
        {
            if (valor > maxv) valor = maxv;

            return (valor - minv) / maxv * maxs + mins;
        }

        //Delegados ********************************************************************************************************************************************************************************
        void RecibidaPosicion(double dPosX, double dPosY)
        {
            PosX = dPosX;
            PosY = dPosY;
            ContadorPosicion = -1;
        }
        int InterseccionCircunferencias(Medida Medida1, Medida Medida2, ref double lPosX, ref double lPosY, double EscalaX, double EscalaY)
        {
            double fA1, fB1, fC1, fA2, fB2, fC2;
                double D, E, F, a, b, c, d, x1, x2, y1, y2;
                double a1, b1, r1, a2, b2, r2;
                double v1x1, v1y1, v1x2, v1y2;

            Baliza Baliza2 = ListaBalizas.Find(X => X.Id == Medida1.IDBaliza);
            Baliza Baliza1 = ListaBalizas.Find(X => X.Id == Medida2.IDBaliza);

            //Calculamos la distancia entre los dos puntos
            v1x1 = Baliza1.PosX;
            v1y1 = Baliza1.PosY;
  
            v1x2 = Baliza2.PosX;
            v1y2 = Baliza2.PosY;

            a1 = v1x1;
            b1 = v1y1;
            a2 = v1x2;
            b2 = v1y2;
            r1 = Medida1.Distancia*EscalaX;
            r2 = Medida2.Distancia*EscalaY;
  
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
    
            if (y1 == y2)
            {
                lPosX = x1;
                lPosY = y1;
            }
            else //Debemos escoger la solución correcta
            {
                lPosX = x2;
                lPosY = y2;
            }
    
            return 1;
            }
            else
            {
            //No se actualizan los valores
            return 0;
            }

        }

    }

}

