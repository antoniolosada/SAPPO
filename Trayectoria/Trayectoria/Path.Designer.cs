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
            this.label2 = new System.Windows.Forms.Label();
            this.cmdInicarRobot = new System.Windows.Forms.Button();
            this.tmrControlPosicion = new System.Windows.Forms.Timer(this.components);
            this.lblPosOk = new System.Windows.Forms.Label();
            this.lblPosX = new System.Windows.Forms.Label();
            this.tmrActualizarPosicion = new System.Windows.Forms.Timer(this.components);
            this.lblPosY = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.picMapa)).BeginInit();
            this.grpPanel.SuspendLayout();
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
            this.InsertarNodos.Location = new System.Drawing.Point(1156, 675);
            this.InsertarNodos.Name = "InsertarNodos";
            this.InsertarNodos.Size = new System.Drawing.Size(91, 28);
            this.InsertarNodos.TabIndex = 2;
            this.InsertarNodos.Text = "Ruta+Corta";
            this.InsertarNodos.UseVisualStyleBackColor = true;
            this.InsertarNodos.Click += new System.EventHandler(this.InsertarNodos_Click);
            // 
            // UnirNodos
            // 
            this.UnirNodos.Location = new System.Drawing.Point(136, 681);
            this.UnirNodos.Name = "UnirNodos";
            this.UnirNodos.Size = new System.Drawing.Size(118, 32);
            this.UnirNodos.TabIndex = 3;
            this.UnirNodos.Text = "UnirNodos";
            this.UnirNodos.UseVisualStyleBackColor = true;
            this.UnirNodos.Click += new System.EventHandler(this.UnirNodos_Click);
            // 
            // AddNodos
            // 
            this.AddNodos.Location = new System.Drawing.Point(12, 681);
            this.AddNodos.Name = "AddNodos";
            this.AddNodos.Size = new System.Drawing.Size(118, 32);
            this.AddNodos.TabIndex = 4;
            this.AddNodos.Text = "AñadirNodos";
            this.AddNodos.UseVisualStyleBackColor = true;
            this.AddNodos.Click += new System.EventHandler(this.AddNodos_Click);
            // 
            // lstEnlaces
            // 
            this.lstEnlaces.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstEnlaces.FormattingEnabled = true;
            this.lstEnlaces.ItemHeight = 20;
            this.lstEnlaces.Location = new System.Drawing.Point(1115, 121);
            this.lstEnlaces.Name = "lstEnlaces";
            this.lstEnlaces.Size = new System.Drawing.Size(163, 424);
            this.lstEnlaces.TabIndex = 5;
            // 
            // cmdRuta
            // 
            this.cmdRuta.Location = new System.Drawing.Point(260, 681);
            this.cmdRuta.Name = "cmdRuta";
            this.cmdRuta.Size = new System.Drawing.Size(118, 32);
            this.cmdRuta.TabIndex = 6;
            this.cmdRuta.Text = "Definir Ruta";
            this.cmdRuta.UseVisualStyleBackColor = true;
            this.cmdRuta.Click += new System.EventHandler(this.cmdRuta_Click);
            // 
            // cmdCalcularRuta
            // 
            this.cmdCalcularRuta.Location = new System.Drawing.Point(12, 757);
            this.cmdCalcularRuta.Name = "cmdCalcularRuta";
            this.cmdCalcularRuta.Size = new System.Drawing.Size(118, 32);
            this.cmdCalcularRuta.TabIndex = 7;
            this.cmdCalcularRuta.Text = "Pintar Ruta";
            this.cmdCalcularRuta.UseVisualStyleBackColor = true;
            this.cmdCalcularRuta.Click += new System.EventHandler(this.cmdCalcularRuta_Click);
            // 
            // cmdBuscarRuta
            // 
            this.cmdBuscarRuta.Location = new System.Drawing.Point(399, 719);
            this.cmdBuscarRuta.Name = "cmdBuscarRuta";
            this.cmdBuscarRuta.Size = new System.Drawing.Size(118, 32);
            this.cmdBuscarRuta.TabIndex = 8;
            this.cmdBuscarRuta.Text = "Buscar Ruta";
            this.cmdBuscarRuta.UseVisualStyleBackColor = true;
            this.cmdBuscarRuta.Click += new System.EventHandler(this.cmdBuscarRuta_Click);
            // 
            // cmdMedirDistancia
            // 
            this.cmdMedirDistancia.Location = new System.Drawing.Point(12, 719);
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
            this.cmdDefinirObstaculo.Location = new System.Drawing.Point(136, 719);
            this.cmdDefinirObstaculo.Name = "cmdDefinirObstaculo";
            this.cmdDefinirObstaculo.Size = new System.Drawing.Size(118, 32);
            this.cmdDefinirObstaculo.TabIndex = 12;
            this.cmdDefinirObstaculo.Text = "Definir obstáculo";
            this.cmdDefinirObstaculo.UseVisualStyleBackColor = true;
            this.cmdDefinirObstaculo.Click += new System.EventHandler(this.cmdDefinirObstaculo_Click);
            // 
            // cmdGrabarRuta
            // 
            this.cmdGrabarRuta.Location = new System.Drawing.Point(935, 719);
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
            this.cmdLeerCfg.Location = new System.Drawing.Point(1025, 719);
            this.cmdLeerCfg.Name = "cmdLeerCfg";
            this.cmdLeerCfg.Size = new System.Drawing.Size(84, 32);
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
            "COM0",
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
            this.cbPuerto.Location = new System.Drawing.Point(583, 684);
            this.cbPuerto.Name = "cbPuerto";
            this.cbPuerto.Size = new System.Drawing.Size(91, 26);
            this.cbPuerto.TabIndex = 17;
            this.cbPuerto.Text = "COM5";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(523, 687);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 18);
            this.label2.TabIndex = 18;
            this.label2.Text = "Puerto";
            // 
            // cmdInicarRobot
            // 
            this.cmdInicarRobot.Location = new System.Drawing.Point(399, 681);
            this.cmdInicarRobot.Name = "cmdInicarRobot";
            this.cmdInicarRobot.Size = new System.Drawing.Size(118, 32);
            this.cmdInicarRobot.TabIndex = 19;
            this.cmdInicarRobot.Tag = "OFF";
            this.cmdInicarRobot.Text = "Iniciar Robot";
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
            this.lblPosOk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPosOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblPosOk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPosOk.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosOk.Location = new System.Drawing.Point(1115, 1);
            this.lblPosOk.Name = "lblPosOk";
            this.lblPosOk.Size = new System.Drawing.Size(163, 150);
            this.lblPosOk.TabIndex = 20;
            this.lblPosOk.Text = "PosOk";
            this.lblPosOk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPosX
            // 
            this.lblPosX.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPosX.BackColor = System.Drawing.Color.White;
            this.lblPosX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPosX.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosX.Location = new System.Drawing.Point(1115, 636);
            this.lblPosX.Name = "lblPosX";
            this.lblPosX.Size = new System.Drawing.Size(83, 36);
            this.lblPosX.TabIndex = 21;
            this.lblPosX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrActualizarPosicion
            // 
            this.tmrActualizarPosicion.Tick += new System.EventHandler(this.tmrActualizarPosicion_Tick);
            // 
            // lblPosY
            // 
            this.lblPosY.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPosY.BackColor = System.Drawing.Color.White;
            this.lblPosY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPosY.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosY.Location = new System.Drawing.Point(1204, 636);
            this.lblPosY.Name = "lblPosY";
            this.lblPosY.Size = new System.Drawing.Size(74, 36);
            this.lblPosY.TabIndex = 22;
            this.lblPosY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.grpPanel.Location = new System.Drawing.Point(117, 83);
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
            this.cmdPanel.Location = new System.Drawing.Point(260, 757);
            this.cmdPanel.Name = "cmdPanel";
            this.cmdPanel.Size = new System.Drawing.Size(118, 32);
            this.cmdPanel.TabIndex = 25;
            this.cmdPanel.Tag = "OFF";
            this.cmdPanel.Text = "Mostrar Panel";
            this.cmdPanel.UseVisualStyleBackColor = true;
            this.cmdPanel.Click += new System.EventHandler(this.cmdPanel_Click);
            // 
            // cmdColocarBaliza
            // 
            this.cmdColocarBaliza.Location = new System.Drawing.Point(260, 719);
            this.cmdColocarBaliza.Name = "cmdColocarBaliza";
            this.cmdColocarBaliza.Size = new System.Drawing.Size(118, 32);
            this.cmdColocarBaliza.TabIndex = 26;
            this.cmdColocarBaliza.Text = "Colocar Baliza";
            this.cmdColocarBaliza.UseVisualStyleBackColor = true;
            this.cmdColocarBaliza.Click += new System.EventHandler(this.cmdColocarBaliza_Click);
            // 
            // lblCoorY
            // 
            this.lblCoorY.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCoorY.BackColor = System.Drawing.Color.White;
            this.lblCoorY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCoorY.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoorY.Location = new System.Drawing.Point(1204, 593);
            this.lblCoorY.Name = "lblCoorY";
            this.lblCoorY.Size = new System.Drawing.Size(74, 35);
            this.lblCoorY.TabIndex = 28;
            this.lblCoorY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCoorX
            // 
            this.lblCoorX.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCoorX.BackColor = System.Drawing.Color.White;
            this.lblCoorX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCoorX.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCoorX.Location = new System.Drawing.Point(1115, 593);
            this.lblCoorX.Name = "lblCoorX";
            this.lblCoorX.Size = new System.Drawing.Size(83, 35);
            this.lblCoorX.TabIndex = 27;
            this.lblCoorX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrPintarRobot
            // 
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
            this.tbQ.Location = new System.Drawing.Point(856, 685);
            this.tbQ.Name = "tbQ";
            this.tbQ.Size = new System.Drawing.Size(64, 25);
            this.tbQ.TabIndex = 31;
            this.tbQ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbR
            // 
            this.tbR.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbR.Location = new System.Drawing.Point(952, 685);
            this.tbR.Name = "tbR";
            this.tbR.Size = new System.Drawing.Size(64, 25);
            this.tbR.TabIndex = 32;
            this.tbR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbP
            // 
            this.tbP.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbP.Location = new System.Drawing.Point(1045, 685);
            this.tbP.Name = "tbP";
            this.tbP.Size = new System.Drawing.Size(64, 25);
            this.tbP.TabIndex = 33;
            this.tbP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1022, 688);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 17);
            this.label4.TabIndex = 34;
            this.label4.Text = "p";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(930, 688);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 17);
            this.label5.TabIndex = 35;
            this.label5.Text = "r";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(834, 688);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 17);
            this.label6.TabIndex = 36;
            this.label6.Text = "q";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(856, 719);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(65, 32);
            this.button1.TabIndex = 37;
            this.button1.Text = "P.Kalman";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // cmdSeguirRuta
            // 
            this.cmdSeguirRuta.Location = new System.Drawing.Point(399, 757);
            this.cmdSeguirRuta.Name = "cmdSeguirRuta";
            this.cmdSeguirRuta.Size = new System.Drawing.Size(118, 32);
            this.cmdSeguirRuta.TabIndex = 38;
            this.cmdSeguirRuta.Tag = "OFF";
            this.cmdSeguirRuta.Text = "Seguir Ruta";
            this.cmdSeguirRuta.UseVisualStyleBackColor = true;
            this.cmdSeguirRuta.Click += new System.EventHandler(this.cmdSeguirRuta_Click);
            // 
            // lblms
            // 
            this.lblms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblms.BackColor = System.Drawing.Color.White;
            this.lblms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblms.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblms.Location = new System.Drawing.Point(1117, 548);
            this.lblms.Name = "lblms";
            this.lblms.Size = new System.Drawing.Size(161, 35);
            this.lblms.TabIndex = 39;
            this.lblms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdMapear
            // 
            this.cmdMapear.Location = new System.Drawing.Point(776, 724);
            this.cmdMapear.Name = "cmdMapear";
            this.cmdMapear.Size = new System.Drawing.Size(65, 32);
            this.cmdMapear.TabIndex = 40;
            this.cmdMapear.Text = "Mapear";
            this.cmdMapear.UseVisualStyleBackColor = true;
            this.cmdMapear.Click += new System.EventHandler(this.cmdMapear_Click);
            // 
            // frmTrayectoria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1290, 793);
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
            this.Controls.Add(this.lblCoorX);
            this.Controls.Add(this.cmdColocarBaliza);
            this.Controls.Add(this.cmdPanel);
            this.Controls.Add(this.grpPanel);
            this.Controls.Add(this.lblPosY);
            this.Controls.Add(this.lblPosX);
            this.Controls.Add(this.lblPosOk);
            this.Controls.Add(this.cmdInicarRobot);
            this.Controls.Add(this.label2);
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
            this.Text = "Cálculo de trayectorias";
            this.Activated += new System.EventHandler(this.frmTrayectoria_Activated);
            this.Load += new System.EventHandler(this.frmTrayectoria_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picMapa)).EndInit();
            this.grpPanel.ResumeLayout(false);
            this.grpPanel.PerformLayout();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdInicarRobot;
        private System.Windows.Forms.Timer tmrControlPosicion;
        private System.Windows.Forms.Label lblPosOk;
        private System.Windows.Forms.Label lblPosX;
        private System.Windows.Forms.Timer tmrActualizarPosicion;
        private System.Windows.Forms.Label lblPosY;
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
    }
}

