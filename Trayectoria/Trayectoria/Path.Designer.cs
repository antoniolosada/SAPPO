namespace Trayectoria
{
    partial class frmTrayectoria
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTrayectoria));
            this.picMapa = new System.Windows.Forms.PictureBox();
            this.InsertarNodos = new System.Windows.Forms.Button();
            this.UnirNodos = new System.Windows.Forms.Button();
            this.AddNodos = new System.Windows.Forms.Button();
            this.lstEnlaces = new System.Windows.Forms.ListBox();
            this.cmdRuta = new System.Windows.Forms.Button();
            this.cmdCalcularRuta = new System.Windows.Forms.Button();
            this.cmdBuscarRuta = new System.Windows.Forms.Button();
            this.cmdMedirDistancia = new System.Windows.Forms.Button();
            this.tbEscalaX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdDefinirObstaculo = new System.Windows.Forms.Button();
            this.cmdGrabarRuta = new System.Windows.Forms.Button();
            this.chkPrecisionObstaculos = new System.Windows.Forms.CheckBox();
            this.cmdLeerCfg = new System.Windows.Forms.Button();
            this.chkPintarColision = new System.Windows.Forms.CheckBox();
            this.cbPuerto = new System.Windows.Forms.ComboBox();
            this.cmdInicarRobot = new System.Windows.Forms.Button();
            this.tmrControlPosicion = new System.Windows.Forms.Timer(this.components);
            this.lblPosOk = new System.Windows.Forms.Label();
            this.lblPosBal1 = new System.Windows.Forms.Label();
            this.tmrActualizarPosicion = new System.Windows.Forms.Timer(this.components);
            this.lblPosBal2 = new System.Windows.Forms.Label();
            this.tbPos2X = new System.Windows.Forms.TextBox();
            this.grpPanel = new System.Windows.Forms.GroupBox();
            this.tbPos2Y = new System.Windows.Forms.TextBox();
            this.cmdPanel = new System.Windows.Forms.Button();
            this.cmdColocarBaliza = new System.Windows.Forms.Button();
            this.lblCoorY = new System.Windows.Forms.Label();
            this.lblCoorX = new System.Windows.Forms.Label();
            this.tmrPintarRobot = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.tbEscalaY = new System.Windows.Forms.TextBox();
            this.tbQ = new System.Windows.Forms.TextBox();
            this.tbR = new System.Windows.Forms.TextBox();
            this.tbP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.cmdSeguirRuta = new System.Windows.Forms.Button();
            this.lblms = new System.Windows.Forms.Label();
            this.cmdMapear = new System.Windows.Forms.Button();
            this.cmdBorrarBalizas = new System.Windows.Forms.Button();
            this.cmdBorrarNodos = new System.Windows.Forms.Button();
            this.chkLecturaCOM = new System.Windows.Forms.CheckBox();
            this.panelLOG = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdBorrarLOG = new System.Windows.Forms.Button();
            this.cmdCopiarLOG = new System.Windows.Forms.Button();
            this.tbLOG = new System.Windows.Forms.TextBox();
            this.cmdVerLOG = new System.Windows.Forms.Button();
            this.cmdTest = new System.Windows.Forms.Button();
            this.picRadar = new System.Windows.Forms.PictureBox();
            this.tbLonX = new System.Windows.Forms.TextBox();
            this.tbLonY = new System.Windows.Forms.TextBox();
            this.tbEscalaSim = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.pbMedidas = new System.Windows.Forms.ProgressBar();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblRawBal1 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblRawBal2 = new System.Windows.Forms.Label();
            this.cmdOrientacion = new System.Windows.Forms.Button();
            this.cmdPos = new System.Windows.Forms.Button();
            this.cmdReset = new System.Windows.Forms.Button();
            this.pbArriba = new System.Windows.Forms.PictureBox();
            this.pbDerecha = new System.Windows.Forms.PictureBox();
            this.pbAbajo = new System.Windows.Forms.PictureBox();
            this.pbIzquierda = new System.Windows.Forms.PictureBox();
            this.pbParar = new System.Windows.Forms.PictureBox();
            this.chkKalman = new System.Windows.Forms.CheckBox();
            this.lblSensorBal1 = new System.Windows.Forms.Label();
            this.lblSensorBal2 = new System.Windows.Forms.Label();
            this.Sens = new System.Windows.Forms.Label();
            this.cmdRadar = new System.Windows.Forms.Button();
            this.cbRobot = new System.Windows.Forms.ComboBox();
            this.pbArribaDespacio = new System.Windows.Forms.PictureBox();
            this.cmdPos0 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.lblPos = new System.Windows.Forms.TextBox();
            this.pbIzq = new System.Windows.Forms.ProgressBar();
            this.pbDer = new System.Windows.Forms.ProgressBar();
            this.tbIzq = new System.Windows.Forms.TextBox();
            this.tbDer = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picMapa)).BeginInit();
            this.grpPanel.SuspendLayout();
            this.panelLOG.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRadar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbArriba)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDerecha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAbajo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIzquierda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbParar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbArribaDespacio)).BeginInit();
            this.SuspendLayout();
            // 
            // picMapa
            // 
            this.picMapa.BackColor = System.Drawing.Color.White;
            this.picMapa.Image = global::Trayectoria.Properties.Resources.salon;
            this.picMapa.InitialImage = global::Trayectoria.Properties.Resources.salon;
            this.picMapa.Location = new System.Drawing.Point(2, 1);
            this.picMapa.Name = "picMapa";
            this.picMapa.Size = new System.Drawing.Size(1107, 674);
            this.picMapa.TabIndex = 1;
            this.picMapa.TabStop = false;
            this.picMapa.Click += new System.EventHandler(this.picMapa_Click_1);
            this.picMapa.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picMapa_MouseDown);
            // 
            // InsertarNodos
            // 
            this.InsertarNodos.Location = new System.Drawing.Point(1157, 677);
            this.InsertarNodos.Name = "InsertarNodos";
            this.InsertarNodos.Size = new System.Drawing.Size(91, 28);
            this.InsertarNodos.TabIndex = 2;
            this.InsertarNodos.Text = "Ruta+Corta";
            this.InsertarNodos.UseVisualStyleBackColor = true;
            this.InsertarNodos.Click += new System.EventHandler(this.InsertarNodos_Click);
            // 
            // UnirNodos
            // 
            this.UnirNodos.Location = new System.Drawing.Point(232, 682);
            this.UnirNodos.Name = "UnirNodos";
            this.UnirNodos.Size = new System.Drawing.Size(118, 32);
            this.UnirNodos.TabIndex = 3;
            this.UnirNodos.Text = "UnirNodos";
            this.UnirNodos.UseVisualStyleBackColor = true;
            this.UnirNodos.Click += new System.EventHandler(this.UnirNodos_Click);
            // 
            // AddNodos
            // 
            this.AddNodos.Location = new System.Drawing.Point(111, 682);
            this.AddNodos.Name = "AddNodos";
            this.AddNodos.Size = new System.Drawing.Size(118, 32);
            this.AddNodos.TabIndex = 4;
            this.AddNodos.Text = "AñadirNodos";
            this.AddNodos.UseVisualStyleBackColor = true;
            this.AddNodos.Click += new System.EventHandler(this.AddNodos_Click);
            // 
            // lstEnlaces
            // 
            this.lstEnlaces.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lstEnlaces.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstEnlaces.FormattingEnabled = true;
            this.lstEnlaces.ItemHeight = 20;
            this.lstEnlaces.Location = new System.Drawing.Point(1115, 81);
            this.lstEnlaces.Name = "lstEnlaces";
            this.lstEnlaces.Size = new System.Drawing.Size(163, 324);
            this.lstEnlaces.TabIndex = 5;
            // 
            // cmdRuta
            // 
            this.cmdRuta.Location = new System.Drawing.Point(352, 683);
            this.cmdRuta.Name = "cmdRuta";
            this.cmdRuta.Size = new System.Drawing.Size(118, 32);
            this.cmdRuta.TabIndex = 6;
            this.cmdRuta.Text = "Definir Ruta";
            this.cmdRuta.UseVisualStyleBackColor = true;
            this.cmdRuta.Click += new System.EventHandler(this.cmdRuta_Click);
            // 
            // cmdCalcularRuta
            // 
            this.cmdCalcularRuta.Location = new System.Drawing.Point(111, 740);
            this.cmdCalcularRuta.Name = "cmdCalcularRuta";
            this.cmdCalcularRuta.Size = new System.Drawing.Size(118, 32);
            this.cmdCalcularRuta.TabIndex = 7;
            this.cmdCalcularRuta.Text = "Pintar Ruta";
            this.cmdCalcularRuta.UseVisualStyleBackColor = true;
            this.cmdCalcularRuta.Click += new System.EventHandler(this.cmdCalcularRuta_Click);
            // 
            // cmdBuscarRuta
            // 
            this.cmdBuscarRuta.Location = new System.Drawing.Point(471, 714);
            this.cmdBuscarRuta.Name = "cmdBuscarRuta";
            this.cmdBuscarRuta.Size = new System.Drawing.Size(106, 30);
            this.cmdBuscarRuta.TabIndex = 8;
            this.cmdBuscarRuta.Text = "Buscar Ruta";
            this.cmdBuscarRuta.UseVisualStyleBackColor = true;
            this.cmdBuscarRuta.Click += new System.EventHandler(this.cmdBuscarRuta_Click);
            // 
            // cmdMedirDistancia
            // 
            this.cmdMedirDistancia.Location = new System.Drawing.Point(111, 713);
            this.cmdMedirDistancia.Name = "cmdMedirDistancia";
            this.cmdMedirDistancia.Size = new System.Drawing.Size(118, 32);
            this.cmdMedirDistancia.TabIndex = 9;
            this.cmdMedirDistancia.Text = "Medir distancia";
            this.cmdMedirDistancia.UseVisualStyleBackColor = true;
            this.cmdMedirDistancia.Click += new System.EventHandler(this.cmdMedirDistancia_Click);
            // 
            // tbEscalaX
            // 
            this.tbEscalaX.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEscalaX.Location = new System.Drawing.Point(1204, 709);
            this.tbEscalaX.Name = "tbEscalaX";
            this.tbEscalaX.Size = new System.Drawing.Size(64, 25);
            this.tbEscalaX.TabIndex = 10;
            this.tbEscalaX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbEscalaX.Leave += new System.EventHandler(this.tbEscala_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1133, 712);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Escala X";
            // 
            // cmdDefinirObstaculo
            // 
            this.cmdDefinirObstaculo.Location = new System.Drawing.Point(232, 713);
            this.cmdDefinirObstaculo.Name = "cmdDefinirObstaculo";
            this.cmdDefinirObstaculo.Size = new System.Drawing.Size(118, 32);
            this.cmdDefinirObstaculo.TabIndex = 12;
            this.cmdDefinirObstaculo.Text = "Definir obstáculo";
            this.cmdDefinirObstaculo.UseVisualStyleBackColor = true;
            this.cmdDefinirObstaculo.Click += new System.EventHandler(this.cmdDefinirObstaculo_Click);
            // 
            // cmdGrabarRuta
            // 
            this.cmdGrabarRuta.Location = new System.Drawing.Point(935, 712);
            this.cmdGrabarRuta.Name = "cmdGrabarRuta";
            this.cmdGrabarRuta.Size = new System.Drawing.Size(84, 32);
            this.cmdGrabarRuta.TabIndex = 13;
            this.cmdGrabarRuta.Text = "Grabar Cfg";
            this.cmdGrabarRuta.UseVisualStyleBackColor = true;
            this.cmdGrabarRuta.Click += new System.EventHandler(this.cmdGrabarRuta_Click);
            // 
            // chkPrecisionObstaculos
            // 
            this.chkPrecisionObstaculos.AutoSize = true;
            this.chkPrecisionObstaculos.Checked = true;
            this.chkPrecisionObstaculos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPrecisionObstaculos.Location = new System.Drawing.Point(680, 696);
            this.chkPrecisionObstaculos.Name = "chkPrecisionObstaculos";
            this.chkPrecisionObstaculos.Size = new System.Drawing.Size(125, 17);
            this.chkPrecisionObstaculos.TabIndex = 14;
            this.chkPrecisionObstaculos.Text = "Precisión Obstáculos";
            this.chkPrecisionObstaculos.UseVisualStyleBackColor = true;
            // 
            // cmdLeerCfg
            // 
            this.cmdLeerCfg.Location = new System.Drawing.Point(471, 683);
            this.cmdLeerCfg.Name = "cmdLeerCfg";
            this.cmdLeerCfg.Size = new System.Drawing.Size(106, 32);
            this.cmdLeerCfg.TabIndex = 15;
            this.cmdLeerCfg.Text = "Leer Cfg";
            this.cmdLeerCfg.UseVisualStyleBackColor = true;
            this.cmdLeerCfg.Click += new System.EventHandler(this.cmdLeerCfg_Click);
            // 
            // chkPintarColision
            // 
            this.chkPintarColision.AutoSize = true;
            this.chkPintarColision.Location = new System.Drawing.Point(680, 680);
            this.chkPintarColision.Name = "chkPintarColision";
            this.chkPintarColision.Size = new System.Drawing.Size(102, 17);
            this.chkPintarColision.TabIndex = 16;
            this.chkPintarColision.Text = "Pintar colisiones";
            this.chkPintarColision.UseVisualStyleBackColor = true;
            // 
            // cbPuerto
            // 
            this.cbPuerto.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPuerto.FormattingEnabled = true;
            this.cbPuerto.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12",
            "COM13",
            "COM14",
            "COM15",
            "COM16",
            "COM17",
            "COM18",
            "COM19",
            "COM20",
            "COM21",
            "COM22",
            "COM23",
            "COM24",
            "COM25",
            "COM26",
            "COM27",
            "COM28",
            "COM29",
            "COM30",
            "COM31",
            "COM32",
            "COM33",
            "COM34",
            "COM35",
            "COM36",
            "COM37",
            "COM38",
            "COM39",
            "COM40",
            "COM41",
            "COM42",
            "COM43",
            "COM44",
            "COM45",
            "COM46",
            "COM47",
            "COM48",
            "COM49",
            "COM50",
            "COM51",
            "COM52",
            "COM53",
            "COM54",
            "COM55",
            "COM56",
            "COM57",
            "COM58",
            "COM59",
            "COM60",
            "COM61",
            "COM62",
            "COM63",
            "COM64",
            "COM65",
            "COM66",
            "COM67",
            "COM68",
            "COM69",
            "COM70"});
            this.cbPuerto.Location = new System.Drawing.Point(596, 680);
            this.cbPuerto.Name = "cbPuerto";
            this.cbPuerto.Size = new System.Drawing.Size(78, 26);
            this.cbPuerto.TabIndex = 17;
            this.cbPuerto.Text = "COM10";
            // 
            // cmdInicarRobot
            // 
            this.cmdInicarRobot.Location = new System.Drawing.Point(583, 714);
            this.cmdInicarRobot.Name = "cmdInicarRobot";
            this.cmdInicarRobot.Size = new System.Drawing.Size(69, 29);
            this.cmdInicarRobot.TabIndex = 19;
            this.cmdInicarRobot.Tag = "OFF";
            this.cmdInicarRobot.Text = "Conectar";
            this.cmdInicarRobot.UseVisualStyleBackColor = true;
            this.cmdInicarRobot.Click += new System.EventHandler(this.cmdIniciarRobot_Click);
            // 
            // tmrControlPosicion
            // 
            this.tmrControlPosicion.Interval = 500;
            this.tmrControlPosicion.Tick += new System.EventHandler(this.tmrControlPosicion_Tick);
            // 
            // lblPosOk
            // 
            this.lblPosOk.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPosOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblPosOk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPosOk.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosOk.Location = new System.Drawing.Point(1115, 1);
            this.lblPosOk.Name = "lblPosOk";
            this.lblPosOk.Size = new System.Drawing.Size(163, 50);
            this.lblPosOk.TabIndex = 20;
            this.lblPosOk.Text = "PosOk";
            this.lblPosOk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPosBal1
            // 
            this.lblPosBal1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPosBal1.BackColor = System.Drawing.Color.White;
            this.lblPosBal1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPosBal1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosBal1.Location = new System.Drawing.Point(1147, 506);
            this.lblPosBal1.Name = "lblPosBal1";
            this.lblPosBal1.Size = new System.Drawing.Size(62, 22);
            this.lblPosBal1.TabIndex = 21;
            this.lblPosBal1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrActualizarPosicion
            // 
            this.tmrActualizarPosicion.Tick += new System.EventHandler(this.tmrActualizarPosicion_Tick);
            // 
            // lblPosBal2
            // 
            this.lblPosBal2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPosBal2.BackColor = System.Drawing.Color.White;
            this.lblPosBal2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPosBal2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosBal2.Location = new System.Drawing.Point(1215, 506);
            this.lblPosBal2.Name = "lblPosBal2";
            this.lblPosBal2.Size = new System.Drawing.Size(63, 23);
            this.lblPosBal2.TabIndex = 22;
            this.lblPosBal2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbPos2X
            // 
            this.tbPos2X.Font = new System.Drawing.Font("Microsoft Sans Serif", 129.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPos2X.Location = new System.Drawing.Point(19, 19);
            this.tbPos2X.Name = "tbPos2X";
            this.tbPos2X.Size = new System.Drawing.Size(784, 203);
            this.tbPos2X.TabIndex = 23;
            this.tbPos2X.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // grpPanel
            // 
            this.grpPanel.Controls.Add(this.tbPos2Y);
            this.grpPanel.Controls.Add(this.tbPos2X);
            this.grpPanel.Location = new System.Drawing.Point(12, 121);
            this.grpPanel.Name = "grpPanel";
            this.grpPanel.Size = new System.Drawing.Size(819, 444);
            this.grpPanel.TabIndex = 24;
            this.grpPanel.TabStop = false;
            this.grpPanel.Visible = false;
            // 
            // tbPos2Y
            // 
            this.tbPos2Y.Font = new System.Drawing.Font("Microsoft Sans Serif", 129.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPos2Y.Location = new System.Drawing.Point(19, 228);
            this.tbPos2Y.Name = "tbPos2Y";
            this.tbPos2Y.Size = new System.Drawing.Size(784, 203);
            this.tbPos2Y.TabIndex = 24;
            this.tbPos2Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmdPanel
            // 
            this.cmdPanel.Location = new System.Drawing.Point(782, 740);
            this.cmdPanel.Name = "cmdPanel";
            this.cmdPanel.Size = new System.Drawing.Size(85, 31);
            this.cmdPanel.TabIndex = 25;
            this.cmdPanel.Tag = "OFF";
            this.cmdPanel.Text = "Mostrar Panel";
            this.cmdPanel.UseVisualStyleBackColor = true;
            this.cmdPanel.Click += new System.EventHandler(this.cmdPanel_Click);
            // 
            // cmdColocarBaliza
            // 
            this.cmdColocarBaliza.Location = new System.Drawing.Point(352, 714);
            this.cmdColocarBaliza.Name = "cmdColocarBaliza";
            this.cmdColocarBaliza.Size = new System.Drawing.Size(118, 32);
            this.cmdColocarBaliza.TabIndex = 26;
            this.cmdColocarBaliza.Text = "Colocar Baliza";
            this.cmdColocarBaliza.UseVisualStyleBackColor = true;
            this.cmdColocarBaliza.Click += new System.EventHandler(this.cmdColocarBaliza_Click);
            // 
            // lblCoorY
            // 
            this.lblCoorY.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblCoorY.BackColor = System.Drawing.Color.White;
            this.lblCoorY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCoorY.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoorY.Location = new System.Drawing.Point(1215, 476);
            this.lblCoorY.Name = "lblCoorY";
            this.lblCoorY.Size = new System.Drawing.Size(63, 26);
            this.lblCoorY.TabIndex = 28;
            this.lblCoorY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCoorX
            // 
            this.lblCoorX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblCoorX.BackColor = System.Drawing.Color.White;
            this.lblCoorX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCoorX.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoorX.Location = new System.Drawing.Point(1147, 476);
            this.lblCoorX.Name = "lblCoorX";
            this.lblCoorX.Size = new System.Drawing.Size(62, 26);
            this.lblCoorX.TabIndex = 27;
            this.lblCoorX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrPintarRobot
            // 
            this.tmrPintarRobot.Enabled = true;
            this.tmrPintarRobot.Interval = 250;
            this.tmrPintarRobot.Tick += new System.EventHandler(this.tmrPintarRobot_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(1134, 739);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 17);
            this.label3.TabIndex = 30;
            this.label3.Text = "Escala Y";
            // 
            // tbEscalaY
            // 
            this.tbEscalaY.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEscalaY.Location = new System.Drawing.Point(1204, 736);
            this.tbEscalaY.Name = "tbEscalaY";
            this.tbEscalaY.Size = new System.Drawing.Size(64, 25);
            this.tbEscalaY.TabIndex = 29;
            this.tbEscalaY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbQ
            // 
            this.tbQ.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbQ.Location = new System.Drawing.Point(856, 680);
            this.tbQ.Name = "tbQ";
            this.tbQ.Size = new System.Drawing.Size(64, 25);
            this.tbQ.TabIndex = 31;
            this.tbQ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbR
            // 
            this.tbR.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbR.Location = new System.Drawing.Point(952, 680);
            this.tbR.Name = "tbR";
            this.tbR.Size = new System.Drawing.Size(64, 25);
            this.tbR.TabIndex = 32;
            this.tbR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbP
            // 
            this.tbP.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbP.Location = new System.Drawing.Point(1045, 680);
            this.tbP.Name = "tbP";
            this.tbP.Size = new System.Drawing.Size(64, 25);
            this.tbP.TabIndex = 33;
            this.tbP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1022, 683);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 17);
            this.label4.TabIndex = 34;
            this.label4.Text = "p";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(930, 683);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 17);
            this.label5.TabIndex = 35;
            this.label5.Text = "r";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(834, 683);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 17);
            this.label6.TabIndex = 36;
            this.label6.Text = "q";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(704, 741);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 30);
            this.button1.TabIndex = 37;
            this.button1.Text = "P.Kalman";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmdSeguirRuta
            // 
            this.cmdSeguirRuta.Location = new System.Drawing.Point(471, 741);
            this.cmdSeguirRuta.Name = "cmdSeguirRuta";
            this.cmdSeguirRuta.Size = new System.Drawing.Size(106, 32);
            this.cmdSeguirRuta.TabIndex = 38;
            this.cmdSeguirRuta.Tag = "OFF";
            this.cmdSeguirRuta.Text = "Seguir Ruta";
            this.cmdSeguirRuta.UseVisualStyleBackColor = true;
            this.cmdSeguirRuta.Click += new System.EventHandler(this.cmdSeguirRuta_Click);
            // 
            // lblms
            // 
            this.lblms.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblms.BackColor = System.Drawing.Color.White;
            this.lblms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblms.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblms.Location = new System.Drawing.Point(1214, 414);
            this.lblms.Name = "lblms";
            this.lblms.Size = new System.Drawing.Size(64, 27);
            this.lblms.TabIndex = 39;
            this.lblms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdMapear
            // 
            this.cmdMapear.Location = new System.Drawing.Point(782, 712);
            this.cmdMapear.Name = "cmdMapear";
            this.cmdMapear.Size = new System.Drawing.Size(85, 32);
            this.cmdMapear.TabIndex = 40;
            this.cmdMapear.Text = "Mapear";
            this.cmdMapear.UseVisualStyleBackColor = true;
            this.cmdMapear.Click += new System.EventHandler(this.cmdMapear_Click);
            // 
            // cmdBorrarBalizas
            // 
            this.cmdBorrarBalizas.Location = new System.Drawing.Point(352, 741);
            this.cmdBorrarBalizas.Name = "cmdBorrarBalizas";
            this.cmdBorrarBalizas.Size = new System.Drawing.Size(118, 32);
            this.cmdBorrarBalizas.TabIndex = 41;
            this.cmdBorrarBalizas.Text = "BorrarBalizas";
            this.cmdBorrarBalizas.UseVisualStyleBackColor = true;
            this.cmdBorrarBalizas.Click += new System.EventHandler(this.cmdBorrarBalizas_Click);
            // 
            // cmdBorrarNodos
            // 
            this.cmdBorrarNodos.Location = new System.Drawing.Point(232, 740);
            this.cmdBorrarNodos.Name = "cmdBorrarNodos";
            this.cmdBorrarNodos.Size = new System.Drawing.Size(118, 32);
            this.cmdBorrarNodos.TabIndex = 42;
            this.cmdBorrarNodos.Text = "Borrar Nodos";
            this.cmdBorrarNodos.UseVisualStyleBackColor = true;
            this.cmdBorrarNodos.Click += new System.EventHandler(this.cmdBorrarNodos_Click);
            // 
            // chkLecturaCOM
            // 
            this.chkLecturaCOM.AutoSize = true;
            this.chkLecturaCOM.Location = new System.Drawing.Point(935, 754);
            this.chkLecturaCOM.Name = "chkLecturaCOM";
            this.chkLecturaCOM.Size = new System.Drawing.Size(86, 17);
            this.chkLecturaCOM.TabIndex = 43;
            this.chkLecturaCOM.Text = "LecturaCOM";
            this.chkLecturaCOM.UseVisualStyleBackColor = true;
            // 
            // panelLOG
            // 
            this.panelLOG.Controls.Add(this.groupBox1);
            this.panelLOG.Location = new System.Drawing.Point(788, 12);
            this.panelLOG.Name = "panelLOG";
            this.panelLOG.Size = new System.Drawing.Size(309, 643);
            this.panelLOG.TabIndex = 44;
            this.panelLOG.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmdBorrarLOG);
            this.groupBox1.Controls.Add(this.cmdCopiarLOG);
            this.groupBox1.Controls.Add(this.tbLOG);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(309, 640);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LOG";
            // 
            // cmdBorrarLOG
            // 
            this.cmdBorrarLOG.Location = new System.Drawing.Point(42, 0);
            this.cmdBorrarLOG.Name = "cmdBorrarLOG";
            this.cmdBorrarLOG.Size = new System.Drawing.Size(86, 23);
            this.cmdBorrarLOG.TabIndex = 4;
            this.cmdBorrarLOG.Text = "Borrar LOG";
            this.cmdBorrarLOG.UseVisualStyleBackColor = true;
            this.cmdBorrarLOG.Click += new System.EventHandler(this.cmdBorrarLOG_Click);
            // 
            // cmdCopiarLOG
            // 
            this.cmdCopiarLOG.Location = new System.Drawing.Point(130, 0);
            this.cmdCopiarLOG.Name = "cmdCopiarLOG";
            this.cmdCopiarLOG.Size = new System.Drawing.Size(107, 23);
            this.cmdCopiarLOG.TabIndex = 3;
            this.cmdCopiarLOG.Text = "Copiar LOG";
            this.cmdCopiarLOG.UseVisualStyleBackColor = true;
            this.cmdCopiarLOG.Click += new System.EventHandler(this.cmdCopiarLOG_Click);
            // 
            // tbLOG
            // 
            this.tbLOG.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLOG.Location = new System.Drawing.Point(0, 23);
            this.tbLOG.Multiline = true;
            this.tbLOG.Name = "tbLOG";
            this.tbLOG.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLOG.Size = new System.Drawing.Size(303, 617);
            this.tbLOG.TabIndex = 2;
            // 
            // cmdVerLOG
            // 
            this.cmdVerLOG.Location = new System.Drawing.Point(583, 742);
            this.cmdVerLOG.Name = "cmdVerLOG";
            this.cmdVerLOG.Size = new System.Drawing.Size(119, 31);
            this.cmdVerLOG.TabIndex = 45;
            this.cmdVerLOG.Text = "Ver LOG";
            this.cmdVerLOG.UseVisualStyleBackColor = true;
            this.cmdVerLOG.Click += new System.EventHandler(this.cmdVerLOG_Click);
            // 
            // cmdTest
            // 
            this.cmdTest.Location = new System.Drawing.Point(1025, 713);
            this.cmdTest.Name = "cmdTest";
            this.cmdTest.Size = new System.Drawing.Size(84, 31);
            this.cmdTest.TabIndex = 46;
            this.cmdTest.Text = "Prueba";
            this.cmdTest.UseVisualStyleBackColor = true;
            this.cmdTest.Click += new System.EventHandler(this.cmdTest_Click);
            // 
            // picRadar
            // 
            this.picRadar.Image = ((System.Drawing.Image)(resources.GetObject("picRadar.Image")));
            this.picRadar.Location = new System.Drawing.Point(1192, 571);
            this.picRadar.Name = "picRadar";
            this.picRadar.Size = new System.Drawing.Size(86, 84);
            this.picRadar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picRadar.TabIndex = 47;
            this.picRadar.TabStop = false;
            // 
            // tbLonX
            // 
            this.tbLonX.Location = new System.Drawing.Point(1137, 583);
            this.tbLonX.Name = "tbLonX";
            this.tbLonX.Size = new System.Drawing.Size(46, 20);
            this.tbLonX.TabIndex = 49;
            this.tbLonX.Text = "2,5";
            // 
            // tbLonY
            // 
            this.tbLonY.Location = new System.Drawing.Point(1136, 609);
            this.tbLonY.Name = "tbLonY";
            this.tbLonY.Size = new System.Drawing.Size(47, 20);
            this.tbLonY.TabIndex = 50;
            this.tbLonY.Text = "2,5";
            // 
            // tbEscalaSim
            // 
            this.tbEscalaSim.Location = new System.Drawing.Point(1136, 635);
            this.tbEscalaSim.Name = "tbEscalaSim";
            this.tbEscalaSim.Size = new System.Drawing.Size(47, 20);
            this.tbEscalaSim.TabIndex = 51;
            this.tbEscalaSim.Text = "180";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1117, 587);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 52;
            this.label7.Text = "X";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1117, 609);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 13);
            this.label8.TabIndex = 53;
            this.label8.Text = "Y";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1114, 640);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(25, 13);
            this.label9.TabIndex = 54;
            this.label9.Text = "Esc";
            // 
            // pbMedidas
            // 
            this.pbMedidas.BackColor = System.Drawing.SystemColors.HighlightText;
            this.pbMedidas.Location = new System.Drawing.Point(1115, 54);
            this.pbMedidas.Maximum = 30;
            this.pbMedidas.Name = "pbMedidas";
            this.pbMedidas.Size = new System.Drawing.Size(163, 21);
            this.pbMedidas.TabIndex = 55;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(1119, 506);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 16);
            this.label10.TabIndex = 56;
            this.label10.Text = "Dist";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(1119, 482);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(31, 16);
            this.label11.TabIndex = 57;
            this.label11.Text = "Pos";
            // 
            // lblRawBal1
            // 
            this.lblRawBal1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblRawBal1.BackColor = System.Drawing.Color.White;
            this.lblRawBal1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRawBal1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRawBal1.Location = new System.Drawing.Point(1147, 532);
            this.lblRawBal1.Name = "lblRawBal1";
            this.lblRawBal1.Size = new System.Drawing.Size(62, 22);
            this.lblRawBal1.TabIndex = 58;
            this.lblRawBal1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(1119, 532);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(27, 16);
            this.label13.TabIndex = 60;
            this.label13.Text = "Bal";
            // 
            // lblRawBal2
            // 
            this.lblRawBal2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblRawBal2.BackColor = System.Drawing.Color.White;
            this.lblRawBal2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRawBal2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRawBal2.Location = new System.Drawing.Point(1215, 532);
            this.lblRawBal2.Name = "lblRawBal2";
            this.lblRawBal2.Size = new System.Drawing.Size(63, 23);
            this.lblRawBal2.TabIndex = 59;
            this.lblRawBal2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdOrientacion
            // 
            this.cmdOrientacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOrientacion.Location = new System.Drawing.Point(864, 740);
            this.cmdOrientacion.Name = "cmdOrientacion";
            this.cmdOrientacion.Size = new System.Drawing.Size(65, 32);
            this.cmdOrientacion.TabIndex = 61;
            this.cmdOrientacion.Text = "Orientación";
            this.cmdOrientacion.UseVisualStyleBackColor = true;
            this.cmdOrientacion.Click += new System.EventHandler(this.cmdOrientacion_Click);
            // 
            // cmdPos
            // 
            this.cmdPos.Location = new System.Drawing.Point(652, 714);
            this.cmdPos.Name = "cmdPos";
            this.cmdPos.Size = new System.Drawing.Size(50, 29);
            this.cmdPos.TabIndex = 62;
            this.cmdPos.Tag = "OFF";
            this.cmdPos.Text = "Pos";
            this.cmdPos.UseVisualStyleBackColor = true;
            this.cmdPos.Click += new System.EventHandler(this.cmdPos_Click);
            // 
            // cmdReset
            // 
            this.cmdReset.Location = new System.Drawing.Point(704, 713);
            this.cmdReset.Name = "cmdReset";
            this.cmdReset.Size = new System.Drawing.Size(78, 31);
            this.cmdReset.TabIndex = 63;
            this.cmdReset.Tag = "OFF";
            this.cmdReset.Text = "Reset";
            this.cmdReset.UseVisualStyleBackColor = true;
            this.cmdReset.Click += new System.EventHandler(this.cmdReset_Click);
            // 
            // pbArriba
            // 
            this.pbArriba.Image = ((System.Drawing.Image)(resources.GetObject("pbArriba.Image")));
            this.pbArriba.Location = new System.Drawing.Point(40, 676);
            this.pbArriba.Name = "pbArriba";
            this.pbArriba.Size = new System.Drawing.Size(30, 29);
            this.pbArriba.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbArriba.TabIndex = 64;
            this.pbArriba.TabStop = false;
            this.pbArriba.Click += new System.EventHandler(this.pbArriba_Click);
            this.pbArriba.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbArriba_MouseDown);
            this.pbArriba.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbArriba_MouseUp);
            // 
            // pbDerecha
            // 
            this.pbDerecha.Image = ((System.Drawing.Image)(resources.GetObject("pbDerecha.Image")));
            this.pbDerecha.Location = new System.Drawing.Point(68, 719);
            this.pbDerecha.Name = "pbDerecha";
            this.pbDerecha.Size = new System.Drawing.Size(30, 24);
            this.pbDerecha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbDerecha.TabIndex = 65;
            this.pbDerecha.TabStop = false;
            this.pbDerecha.Click += new System.EventHandler(this.pbDerecha_Click);
            this.pbDerecha.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbDerecha_MouseDown);
            this.pbDerecha.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbDerecha_MouseUp);
            // 
            // pbAbajo
            // 
            this.pbAbajo.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pbAbajo.Image = ((System.Drawing.Image)(resources.GetObject("pbAbajo.Image")));
            this.pbAbajo.Location = new System.Drawing.Point(11, 719);
            this.pbAbajo.Name = "pbAbajo";
            this.pbAbajo.Size = new System.Drawing.Size(30, 23);
            this.pbAbajo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbAbajo.TabIndex = 66;
            this.pbAbajo.TabStop = false;
            this.pbAbajo.Click += new System.EventHandler(this.pbAbajo_Click);
            this.pbAbajo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbAbajo_MouseDown);
            this.pbAbajo.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbAbajo_MouseUp);
            // 
            // pbIzquierda
            // 
            this.pbIzquierda.Image = ((System.Drawing.Image)(resources.GetObject("pbIzquierda.Image")));
            this.pbIzquierda.Location = new System.Drawing.Point(41, 741);
            this.pbIzquierda.Name = "pbIzquierda";
            this.pbIzquierda.Size = new System.Drawing.Size(30, 23);
            this.pbIzquierda.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIzquierda.TabIndex = 67;
            this.pbIzquierda.TabStop = false;
            this.pbIzquierda.Click += new System.EventHandler(this.pbIzquierda_Click);
            this.pbIzquierda.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbIzquierda_MouseDown);
            this.pbIzquierda.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbIzquierda_MouseUp);
            // 
            // pbParar
            // 
            this.pbParar.Image = ((System.Drawing.Image)(resources.GetObject("pbParar.Image")));
            this.pbParar.Location = new System.Drawing.Point(41, 719);
            this.pbParar.Name = "pbParar";
            this.pbParar.Size = new System.Drawing.Size(30, 23);
            this.pbParar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbParar.TabIndex = 68;
            this.pbParar.TabStop = false;
            this.pbParar.Click += new System.EventHandler(this.pbParar_Click);
            this.pbParar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbParar_MouseDown);
            this.pbParar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbParar_MouseUp);
            // 
            // chkKalman
            // 
            this.chkKalman.AutoSize = true;
            this.chkKalman.Location = new System.Drawing.Point(1123, 661);
            this.chkKalman.Name = "chkKalman";
            this.chkKalman.Size = new System.Drawing.Size(61, 17);
            this.chkKalman.TabIndex = 69;
            this.chkKalman.Text = "Kalman";
            this.chkKalman.UseVisualStyleBackColor = true;
            this.chkKalman.CheckedChanged += new System.EventHandler(this.chkKalman_CheckedChanged);
            // 
            // lblSensorBal1
            // 
            this.lblSensorBal1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblSensorBal1.BackColor = System.Drawing.Color.White;
            this.lblSensorBal1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSensorBal1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSensorBal1.Location = new System.Drawing.Point(1146, 447);
            this.lblSensorBal1.Name = "lblSensorBal1";
            this.lblSensorBal1.Size = new System.Drawing.Size(62, 26);
            this.lblSensorBal1.TabIndex = 70;
            this.lblSensorBal1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSensorBal2
            // 
            this.lblSensorBal2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblSensorBal2.BackColor = System.Drawing.Color.White;
            this.lblSensorBal2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSensorBal2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSensorBal2.Location = new System.Drawing.Point(1214, 447);
            this.lblSensorBal2.Name = "lblSensorBal2";
            this.lblSensorBal2.Size = new System.Drawing.Size(63, 26);
            this.lblSensorBal2.TabIndex = 71;
            this.lblSensorBal2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Sens
            // 
            this.Sens.AutoSize = true;
            this.Sens.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Sens.Location = new System.Drawing.Point(1117, 453);
            this.Sens.Name = "Sens";
            this.Sens.Size = new System.Drawing.Size(31, 16);
            this.Sens.TabIndex = 72;
            this.Sens.Text = "Sen";
            // 
            // cmdRadar
            // 
            this.cmdRadar.Location = new System.Drawing.Point(864, 712);
            this.cmdRadar.Name = "cmdRadar";
            this.cmdRadar.Size = new System.Drawing.Size(65, 31);
            this.cmdRadar.TabIndex = 73;
            this.cmdRadar.Text = "Radar";
            this.cmdRadar.UseVisualStyleBackColor = true;
            this.cmdRadar.Click += new System.EventHandler(this.cmdRadar_Click);
            // 
            // cbRobot
            // 
            this.cbRobot.FormattingEnabled = true;
            this.cbRobot.Items.AddRange(new object[] {
            "Simulador",
            "RODI",
            "DANI"});
            this.cbRobot.Location = new System.Drawing.Point(1119, 560);
            this.cbRobot.Name = "cbRobot";
            this.cbRobot.Size = new System.Drawing.Size(64, 21);
            this.cbRobot.TabIndex = 74;
            // 
            // pbArribaDespacio
            // 
            this.pbArribaDespacio.Image = ((System.Drawing.Image)(resources.GetObject("pbArribaDespacio.Image")));
            this.pbArribaDespacio.Location = new System.Drawing.Point(40, 701);
            this.pbArribaDespacio.Name = "pbArribaDespacio";
            this.pbArribaDespacio.Size = new System.Drawing.Size(30, 19);
            this.pbArribaDespacio.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbArribaDespacio.TabIndex = 75;
            this.pbArribaDespacio.TabStop = false;
            this.pbArribaDespacio.Click += new System.EventHandler(this.pbArribaDespacio_Click);
            this.pbArribaDespacio.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbArribaDespacio_MouseDown);
            this.pbArribaDespacio.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbArribaDespacio_MouseUp);
            // 
            // cmdPos0
            // 
            this.cmdPos0.Location = new System.Drawing.Point(-3, 677);
            this.cmdPos0.Name = "cmdPos0";
            this.cmdPos0.Size = new System.Drawing.Size(36, 20);
            this.cmdPos0.TabIndex = 76;
            this.cmdPos0.Text = "0,0";
            this.cmdPos0.UseVisualStyleBackColor = true;
            this.cmdPos0.Click += new System.EventHandler(this.cmdPos0_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(-3, 694);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(36, 21);
            this.button3.TabIndex = 77;
            this.button3.Text = "M";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // lblPos
            // 
            this.lblPos.Location = new System.Drawing.Point(78, 686);
            this.lblPos.Name = "lblPos";
            this.lblPos.Size = new System.Drawing.Size(20, 20);
            this.lblPos.TabIndex = 78;
            // 
            // pbIzq
            // 
            this.pbIzq.Location = new System.Drawing.Point(2, 752);
            this.pbIzq.Name = "pbIzq";
            this.pbIzq.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.pbIzq.Size = new System.Drawing.Size(42, 19);
            this.pbIzq.TabIndex = 79;
            // 
            // pbDer
            // 
            this.pbDer.Location = new System.Drawing.Point(68, 752);
            this.pbDer.Name = "pbDer";
            this.pbDer.Size = new System.Drawing.Size(42, 19);
            this.pbDer.TabIndex = 80;
            // 
            // tbIzq
            // 
            this.tbIzq.Location = new System.Drawing.Point(2, 719);
            this.tbIzq.Name = "tbIzq";
            this.tbIzq.Size = new System.Drawing.Size(10, 20);
            this.tbIzq.TabIndex = 81;
            // 
            // tbDer
            // 
            this.tbDer.Location = new System.Drawing.Point(99, 719);
            this.tbDer.Name = "tbDer";
            this.tbDer.Size = new System.Drawing.Size(9, 20);
            this.tbDer.TabIndex = 82;
            // 
            // frmTrayectoria
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1284, 781);
            this.Controls.Add(this.tbDer);
            this.Controls.Add(this.tbIzq);
            this.Controls.Add(this.pbIzquierda);
            this.Controls.Add(this.pbDer);
            this.Controls.Add(this.pbIzq);
            this.Controls.Add(this.lblPos);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.cmdPos0);
            this.Controls.Add(this.pbArribaDespacio);
            this.Controls.Add(this.cbRobot);
            this.Controls.Add(this.cmdRadar);
            this.Controls.Add(this.lblSensorBal1);
            this.Controls.Add(this.Sens);
            this.Controls.Add(this.lblSensorBal2);
            this.Controls.Add(this.chkKalman);
            this.Controls.Add(this.pbParar);
            this.Controls.Add(this.pbAbajo);
            this.Controls.Add(this.pbDerecha);
            this.Controls.Add(this.pbArriba);
            this.Controls.Add(this.cmdReset);
            this.Controls.Add(this.cmdPos);
            this.Controls.Add(this.cmdOrientacion);
            this.Controls.Add(this.lblRawBal1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblRawBal2);
            this.Controls.Add(this.lblCoorX);
            this.Controls.Add(this.lblPosBal1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.pbMedidas);
            this.Controls.Add(this.tbEscalaSim);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbLonY);
            this.Controls.Add(this.tbLonX);
            this.Controls.Add(this.picRadar);
            this.Controls.Add(this.cmdTest);
            this.Controls.Add(this.cmdVerLOG);
            this.Controls.Add(this.panelLOG);
            this.Controls.Add(this.chkLecturaCOM);
            this.Controls.Add(this.cmdBorrarNodos);
            this.Controls.Add(this.cmdBorrarBalizas);
            this.Controls.Add(this.cmdMapear);
            this.Controls.Add(this.lblms);
            this.Controls.Add(this.cmdSeguirRuta);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbP);
            this.Controls.Add(this.tbR);
            this.Controls.Add(this.tbQ);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbEscalaY);
            this.Controls.Add(this.lblCoorY);
            this.Controls.Add(this.cmdColocarBaliza);
            this.Controls.Add(this.cmdPanel);
            this.Controls.Add(this.grpPanel);
            this.Controls.Add(this.lblPosBal2);
            this.Controls.Add(this.lblPosOk);
            this.Controls.Add(this.cmdInicarRobot);
            this.Controls.Add(this.cbPuerto);
            this.Controls.Add(this.chkPintarColision);
            this.Controls.Add(this.cmdLeerCfg);
            this.Controls.Add(this.chkPrecisionObstaculos);
            this.Controls.Add(this.cmdGrabarRuta);
            this.Controls.Add(this.cmdDefinirObstaculo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbEscalaX);
            this.Controls.Add(this.cmdMedirDistancia);
            this.Controls.Add(this.cmdBuscarRuta);
            this.Controls.Add(this.cmdCalcularRuta);
            this.Controls.Add(this.cmdRuta);
            this.Controls.Add(this.lstEnlaces);
            this.Controls.Add(this.AddNodos);
            this.Controls.Add(this.UnirNodos);
            this.Controls.Add(this.InsertarNodos);
            this.Controls.Add(this.picMapa);
            this.Name = "frmTrayectoria";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Cálculo de trayectorias";
            this.Activated += new System.EventHandler(this.frmTrayectoria_Activated);
            this.Load += new System.EventHandler(this.frmTrayectoria_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picMapa)).EndInit();
            this.grpPanel.ResumeLayout(false);
            this.grpPanel.PerformLayout();
            this.panelLOG.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRadar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbArriba)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDerecha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbAbajo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIzquierda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbParar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbArribaDespacio)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picMapa;
        private System.Windows.Forms.Button InsertarNodos;
        private System.Windows.Forms.Button UnirNodos;
        private System.Windows.Forms.Button AddNodos;
        private System.Windows.Forms.ListBox lstEnlaces;
        private System.Windows.Forms.Button cmdRuta;
        private System.Windows.Forms.Button cmdCalcularRuta;
        private System.Windows.Forms.Button cmdBuscarRuta;
        private System.Windows.Forms.Button cmdMedirDistancia;
        private System.Windows.Forms.TextBox tbEscalaX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdDefinirObstaculo;
        private System.Windows.Forms.Button cmdGrabarRuta;
        private System.Windows.Forms.CheckBox chkPrecisionObstaculos;
        private System.Windows.Forms.Button cmdLeerCfg;
        private System.Windows.Forms.CheckBox chkPintarColision;
        private System.Windows.Forms.ComboBox cbPuerto;
        private System.Windows.Forms.Button cmdInicarRobot;
        private System.Windows.Forms.Timer tmrControlPosicion;
        private System.Windows.Forms.Label lblPosOk;
        private System.Windows.Forms.Label lblPosBal1;
        private System.Windows.Forms.Timer tmrActualizarPosicion;
        private System.Windows.Forms.Label lblPosBal2;
        private System.Windows.Forms.TextBox tbPos2X;
        private System.Windows.Forms.GroupBox grpPanel;
        private System.Windows.Forms.TextBox tbPos2Y;
        private System.Windows.Forms.Button cmdPanel;
        private System.Windows.Forms.Button cmdColocarBaliza;
        private System.Windows.Forms.Label lblCoorY;
        private System.Windows.Forms.Label lblCoorX;
        private System.Windows.Forms.Timer tmrPintarRobot;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbEscalaY;
        private System.Windows.Forms.TextBox tbQ;
        private System.Windows.Forms.TextBox tbR;
        private System.Windows.Forms.TextBox tbP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button cmdSeguirRuta;
        private System.Windows.Forms.Label lblms;
        private System.Windows.Forms.Button cmdMapear;
        private System.Windows.Forms.Button cmdBorrarBalizas;
        private System.Windows.Forms.Button cmdBorrarNodos;
        private System.Windows.Forms.CheckBox chkLecturaCOM;
        private System.Windows.Forms.Panel panelLOG;
        private System.Windows.Forms.Button cmdVerLOG;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdCopiarLOG;
        private System.Windows.Forms.TextBox tbLOG;
        private System.Windows.Forms.Button cmdBorrarLOG;
        private System.Windows.Forms.Button cmdTest;
        internal System.Windows.Forms.PictureBox picRadar;
        private System.Windows.Forms.TextBox tbLonX;
        private System.Windows.Forms.TextBox tbLonY;
        private System.Windows.Forms.TextBox tbEscalaSim;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ProgressBar pbMedidas;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblRawBal1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblRawBal2;
        private System.Windows.Forms.Button cmdOrientacion;
        private System.Windows.Forms.Button cmdPos;
        private System.Windows.Forms.Button cmdReset;
        private System.Windows.Forms.PictureBox pbArriba;
        private System.Windows.Forms.PictureBox pbDerecha;
        private System.Windows.Forms.PictureBox pbAbajo;
        private System.Windows.Forms.PictureBox pbIzquierda;
        private System.Windows.Forms.PictureBox pbParar;
        private System.Windows.Forms.CheckBox chkKalman;
        private System.Windows.Forms.Label lblSensorBal1;
        private System.Windows.Forms.Label lblSensorBal2;
        private System.Windows.Forms.Label Sens;
        private System.Windows.Forms.Button cmdRadar;
        private System.Windows.Forms.ComboBox cbRobot;
        private System.Windows.Forms.PictureBox pbArribaDespacio;
        private System.Windows.Forms.Button cmdPos0;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox lblPos;
        private System.Windows.Forms.ProgressBar pbIzq;
        private System.Windows.Forms.ProgressBar pbDer;
        private System.Windows.Forms.TextBox tbIzq;
        private System.Windows.Forms.TextBox tbDer;
    }
}

