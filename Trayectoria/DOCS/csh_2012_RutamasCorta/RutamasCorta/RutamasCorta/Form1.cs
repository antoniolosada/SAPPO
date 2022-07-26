/// ISC jonathan Lucas Flores
/// E-mail: jonathaa_242@hotmail.com, jonathanlf242@gmail.com
/// San Miguel Xalepec Puebla, Mexico
/// el algoritmo esta basado en el siguiente video: http://www.youtube.com/watch?v=6rl0ghgPfK0
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RutamasCorta.Clases;

namespace RutamasCorta
{
    public partial class frmRutas : Form
    {
        int ID, ID1 ; //variables para el origen y destino
        int ID3;//variable que se utiliza cuando se dibuja la ultima linea que une a la raiz con el nodo correspondiente
        int Pro, Pro1; //variables para el origen y destino
        int Acu, Acu1; //variables para el origen y destino
        int Distancia = 0; //variable en la cual se guarda la distancia calculada aleatoriamente
        int X, Y;//variables para saber las coordenadas del control o punto al que el usuario le dio click y posteriormente se utilizaran para dibujar las lineas.
        bool Origen = false;
        bool Destino = false;
        bool Seleccionar_destino = false;
        /// <summary>
        /// Variable que llevara el conteo para saber en que posicion colocaremos el nuevo punto (Label)
        /// </summary>
        int contador = 0;
        /// <summary>
        /// Variable que me indicara si el usuario esta agregando Puntos al Panel
        /// </summary>
        bool Agregar=false ;        
        public frmRutas()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Arreglo de 100 posiciones de objetos de tipo Label
        /// </summary>
        Label[] Puntos;
        private void frmRutas_Load(object sender, EventArgs e)
        {
            //inicializamos el arreglo al cargar el programa
            Puntos = new Label[100];
        }
        private void btnAgregarPuntos_Click(object sender, EventArgs e)
        {
            if (!Agregar)
            {
                //el usuarios a presionado el boton de agregar nuevos puntos
                Agregar = true;
                //ponemos de color rojo el boton que indica si es que el usuario esta agregando puntos
                btnAgregarPuntos.BackColor = Color.Red;
                btnUnirPuntos.Enabled = false;
                BtnSeleccionarDestino.Enabled = false;
            }
            else if (Agregar)
            {
                Agregar = false;
                btnAgregarPuntos.BackColor = Control.DefaultBackColor;
                btnUnirPuntos.Enabled = true ;
                BtnSeleccionarDestino.Enabled = true ;
            }
           
        }
        private void splitContainer1_Panel2_MouseDown(object sender, MouseEventArgs e)
        {
           if (Agregar) //si el usuario presiono el boton agregar
            {
                clsPunto punto = new clsPunto(); //creamos un objeto de la clase clspunto
                punto.P_id = contador; ////el objeto punto en su propiedad p_id le asignamos el valor de contador
                punto.Text = Convert.ToString(contador);//asignamos lo que se va a mostra de texto en el control
                punto.Width = 40;//establecemos el ancho del control
                punto.Height = 15;//establecemos el alto
                punto.P_acumulado = 0;//establecemos el valor acumulado por defaul es 0
                punto.P_procedencia = 0;//establecemos la procedencia por default es 0
                punto.Location = new Point(e.X, e.Y);//ponemos las coordenadas en las que se va amostrar el control
                punto.BackColor = Color.YellowGreen ;                //establecemos el color de fondo del control
                punto.Click += new System.EventHandler(this.label_click);//agregamos un nuevo evento (CLICK) el control creado
                Puntos[contador] = punto;//agregamos el objeto al arreglo de objetos de la clase clsPunto
                splitContainer1.Panel2.Controls.Add(Puntos[contador]);//agregamos el control al panel2 de control Split
                contador += 1;//incrementamos el contador
            }
        }        
        private void label_click(object sender, EventArgs e)
        {
            clsPunto l = sender as clsPunto; //convertimos el objeto sender a un tipo de clsPunto
            //comparamos si la variable es false es que no ha seleccionado un origen
            if (Origen == false)
            {               //guardamos en variables ciertos valores que nos van a hacer falta 
                ID = l.P_id;
                Pro = l.P_procedencia;
                Acu = l.P_acumulado;
                X = l.Location.X;
                Y = l.Location.Y;
                Origen = true;
                Destino = false;
                return;
            }else if (Destino ==false )
            {
                //chacamos si selecciono la misma etiqueta
                if (ID == l.P_id)
                {
                    //Seleccionar_destino = true;
                    Destino = false ;
                    Origen = false ;
                    return;
                }
                //generar el numero aleatorio (Distancia Origen-Destino)
                //esta parte es muy importante ya que yo ga los calculos de la distancia con un random, en el caso de que 
                //los valores se ingresaran manualmente solo habia de cambiar un linea de codigo ejeplo de como quedaria
                //si el valor(distancia) la ingrrsaramoa atravez de una caja de Texto
                //Distancia = int.Parse(txtdistacia.text);
                //solo nos faltaria agregar la caja de texto al programa y llamarla txtdistancia
                Random rnd = new Random();
                Distancia = rnd.Next(3, 15);//generamos el numero en un rango del 3 al 15
                lbxdistancias.Items.Add(Convert.ToString(ID) + "," + Convert.ToString(l.P_id) + " = " + Convert.ToString(Distancia));//agregamos al listbox el origen, destino y su respectiva distancia entre ambos puntos
                int nuevo_acumulado = Acu + Distancia; //calculamos la nueva distancia
                if (l.P_acumulado <= 0)//chhecamos si la distancia del objeto al cual le dimos click es <=0
                {//esto indica que el punto no a sido unido con ningun otro punto
                    l.P_acumulado = nuevo_acumulado;//agregamos la nueva distancia
                    l.P_procedencia = ID;                   //y colocamos la procedencia (De donde viene)
                }
                else//de lo contrario
                {
                    //Esto sucede cuando ya hallamos unido este punto con otro punto y por ende el valor de su distancia ya no es cero
                    //
                    if (nuevo_acumulado <= l.P_acumulado)//checamos si la distancia acumulada es menor que la distancia que tiene el objeto alcual le dimos clik
                    {//esto lo hacemos ya que si la distancia que tiene es menor que la distancia calculada del punto d procedencia + la neva aleatoria
                        l.P_acumulado = nuevo_acumulado;//colocamos la nueva distancia "que es menor -> calculando ruta mas corta"
                        l.P_procedencia = ID;                       //asignamos la procedencia
                    }
                }
                //mostramos informacion de como quedo el nuevo punto
                l.Text = Convert.ToString(l.P_id) + "[" + Convert .ToString ( l.P_acumulado) + "," + Convert .ToString ( l.P_procedencia) + "]";               
              //  l.Text = Convert.ToString(l.P_id);
                //dibujamos la linea
                System.Drawing.Graphics milapiz = splitContainer1.Panel2.CreateGraphics();
                Pen lapiz = new Pen(Color.Black  );
              
                milapiz.DrawLine(lapiz, X, Y, l.Location.X, l.Location.Y);
            
                Origen = false ;
                Destino = true;
                return;
            }
            else if (Seleccionar_destino)//si el usuario a decidodo que el sistema trace la ruta mas corta seleccionara cual cera el punto destino
            {                            // por default el sistema tomara que el punto de inicio es el punto 0
                //Nota: Tomar en cuenta que el trazado de cual es la ruta mas corta se hace del nodo destino hacia el nodo Origen (Raiz)
                l.BackColor = Color.Red;//marcamos de color rojo el destino
                System.Drawing.Graphics milapiz = splitContainer1.Panel2.CreateGraphics();//creamos un nuevo objeto para dibujar la linea
                Pen lapiz = new Pen(Color.Yellow ,2 );
                //trazamos nuevamente la linea que indica la ruta mas conrta del Origen 0 al destino Seleccionado
                clsPunto pt = Puntos [l.P_procedencia ] as clsPunto ;
                ID3 = l.P_procedencia;
                milapiz.DrawLine(lapiz,l.Location.X, l.Location.Y,pt.Location .X ,pt.Location .Y );//esta linea es la primera entre el destino y el punto anterior al que haga referencia
                lblruta.Text = l.P_id .ToString () + l.P_procedencia .ToString ();
                int s = 0;
                for (int i = contador - 1; i >= 0; i--)//recorremo el numero de nodos que esta dado por la variable contador
                {
                    clsPunto p = Puntos [ID3 ] as clsPunto ;//convertimos el objeto  actual a uno de tipo clsPunto
                   // MessageBox.Show("p  " + Convert.ToString(p.P_procedencia));
                    if (p.P_procedencia != 1)//checamos si la propiedad procedencia es diferente a cero esto indicaria que el siguiente punto no es el origen (Raiz)
                    {
                        clsPunto p1 = Puntos[p.P_procedencia] as clsPunto;//convertimos a tipo clsPunto el objeto del arreglo al cual tiene como referencia el objeto actual
                     //   MessageBox.Show("p1  " + Convert.ToString(p1.P_procedencia));
                        milapiz.DrawLine(lapiz, p.Location.X, p.Location.Y, p1.Location.X, p1.Location.Y);//dibujamos la linea
                        lblruta.Text = lblruta.Text +  p1.P_id.ToString () ;
                        ID3 = p1.P_id; //obtengo en que punto me quede para poder trazar la ultima linea que unira el punto raiz con el que corresponda
                        X = p1.Location.X;
                        Y = p1.Location.Y;
                        s += 1;
                    }
                    else
                    {
                        clsPunto p1 = Puntos[0] as clsPunto;//convertimos el objeto (Raiz) a tipo clsPunto
                        clsPunto p2 = Puntos[ID3] as clsPunto;
                        milapiz.DrawLine(lapiz, p.Location.X, p.Location.Y, p2.Location.X, p2.Location.Y);//dibujamos la ultima linea
                            lblruta.Text += p2.P_id;
                        return;
                    }
                }
                int cont = 0;
                string ord;
                for (  int cad = 0; cad <= lblruta.Text.Length - 1;cad++ )
                {
                    if (int.Parse ( lblruta.Text.Substring(cad, 1)) >=1)
                    {
                        cont += 1;
                    }
                }
               lblruta.Text = lblruta.Text.Substring(0, cont) + "0";
             }
        }
        private void BtnSeleccionarDestino_Click(object sender, EventArgs e)
        {
            //lanzamos la bandera de que va a seleccionar undestino para poder trazar la ruta mas corta
            Seleccionar_destino = true;
            Destino = true;
            Origen = true;
            if (pre == false)
            {
                pre = true;
                //lanzamos la bandera de que va a seleccionar un destino para poder trazar la ruta mas corta
                Seleccionar_destino = true;
                Destino = true;
                Origen = true;
                BtnSeleccionarDestino.BackColor = Color.Red;
                btnAgregarPuntos.Enabled = false;
                btnUnirPuntos .Enabled  = false;
            }
            else if (pre)
            {
                pre = false;
                BtnSeleccionarDestino.BackColor = Control.DefaultBackColor;
                btnAgregarPuntos.Enabled = true;
                btnUnirPuntos .Enabled = true;
            }
        }
        bool pre=false ;
        private void btnUnirPuntos_Click(object sender, EventArgs e)
        {
            if (pre == false)
            {
                pre = true;
                btnUnirPuntos.BackColor = Color.Red;
                btnAgregarPuntos.Enabled = false;
                BtnSeleccionarDestino.Enabled = false;
            }
            else if (pre)
            {
                pre = false ;
                btnUnirPuntos.BackColor = Control.DefaultBackColor;
                btnAgregarPuntos.Enabled = true;
                BtnSeleccionarDestino.Enabled = true ;
            }

        }

    }
}