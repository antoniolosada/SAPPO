namespace RutamasCorta
{
    partial class frmRutas
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
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
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblruta = new System.Windows.Forms.Label();
            this.lbxdistancias = new System.Windows.Forms.ListBox();
            this.BtnSeleccionarDestino = new System.Windows.Forms.Button();
            this.btnUnirPuntos = new System.Windows.Forms.Button();
            this.btnAgregarPuntos = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.lblruta);
            this.splitContainer1.Panel1.Controls.Add(this.lbxdistancias);
            this.splitContainer1.Panel1.Controls.Add(this.BtnSeleccionarDestino);
            this.splitContainer1.Panel1.Controls.Add(this.btnUnirPuntos);
            this.splitContainer1.Panel1.Controls.Add(this.btnAgregarPuntos);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.splitContainer1.Panel2.BackgroundImage = global::RutamasCorta.Properties.Resources.salon;
            this.splitContainer1.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.splitContainer1.Panel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer1_Panel2_MouseDown);
            this.splitContainer1.Size = new System.Drawing.Size(1108, 538);
            this.splitContainer1.SplitterDistance = 140;
            this.splitContainer1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 333);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Ruta Mas Corta";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(71, 265);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 20);
            this.label1.TabIndex = 4;
            // 
            // lblruta
            // 
            this.lblruta.AutoSize = true;
            this.lblruta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblruta.Location = new System.Drawing.Point(12, 363);
            this.lblruta.Name = "lblruta";
            this.lblruta.Size = new System.Drawing.Size(0, 20);
            this.lblruta.TabIndex = 3;
            // 
            // lbxdistancias
            // 
            this.lbxdistancias.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbxdistancias.FormattingEnabled = true;
            this.lbxdistancias.ItemHeight = 20;
            this.lbxdistancias.Location = new System.Drawing.Point(3, 113);
            this.lbxdistancias.Name = "lbxdistancias";
            this.lbxdistancias.Size = new System.Drawing.Size(132, 204);
            this.lbxdistancias.TabIndex = 0;
            // 
            // BtnSeleccionarDestino
            // 
            this.BtnSeleccionarDestino.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnSeleccionarDestino.Enabled = false;
            this.BtnSeleccionarDestino.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSeleccionarDestino.Location = new System.Drawing.Point(3, 84);
            this.BtnSeleccionarDestino.Name = "BtnSeleccionarDestino";
            this.BtnSeleccionarDestino.Size = new System.Drawing.Size(133, 23);
            this.BtnSeleccionarDestino.TabIndex = 2;
            this.BtnSeleccionarDestino.Text = "Destino";
            this.BtnSeleccionarDestino.UseVisualStyleBackColor = true;
            this.BtnSeleccionarDestino.Click += new System.EventHandler(this.BtnSeleccionarDestino_Click);
            // 
            // btnUnirPuntos
            // 
            this.btnUnirPuntos.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnUnirPuntos.Enabled = false;
            this.btnUnirPuntos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnirPuntos.Location = new System.Drawing.Point(4, 52);
            this.btnUnirPuntos.Name = "btnUnirPuntos";
            this.btnUnirPuntos.Size = new System.Drawing.Size(131, 26);
            this.btnUnirPuntos.TabIndex = 1;
            this.btnUnirPuntos.Text = "Unir Puntos";
            this.btnUnirPuntos.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnUnirPuntos.UseVisualStyleBackColor = true;
            this.btnUnirPuntos.Click += new System.EventHandler(this.btnUnirPuntos_Click);
            // 
            // btnAgregarPuntos
            // 
            this.btnAgregarPuntos.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAgregarPuntos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregarPuntos.Location = new System.Drawing.Point(5, 21);
            this.btnAgregarPuntos.Name = "btnAgregarPuntos";
            this.btnAgregarPuntos.Size = new System.Drawing.Size(131, 25);
            this.btnAgregarPuntos.TabIndex = 0;
            this.btnAgregarPuntos.Text = "Agregar Puntos";
            this.btnAgregarPuntos.UseVisualStyleBackColor = true;
            this.btnAgregarPuntos.Click += new System.EventHandler(this.btnAgregarPuntos_Click);
            // 
            // frmRutas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1108, 538);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1124, 577);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1124, 577);
            this.Name = "frmRutas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Algoritmo de DIJKSTRA ruta corta ";
            this.Load += new System.EventHandler(this.frmRutas_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnUnirPuntos;
        private System.Windows.Forms.Button btnAgregarPuntos;
        private System.Windows.Forms.Button BtnSeleccionarDestino;
        private System.Windows.Forms.ListBox lbxdistancias;
        private System.Windows.Forms.Label lblruta;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

