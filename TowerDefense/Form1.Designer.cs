namespace TowerDefense
{
    partial class Form1
    {
        /// <summary>
        /// Vyžaduje se proměnná návrháře.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Uvolněte všechny používané prostředky.
        /// </summary>
        /// <param name="disposing">hodnota true, když by se měl spravovaný prostředek odstranit; jinak false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kód generovaný Návrhářem Windows Form

        /// <summary>
        /// Metoda vyžadovaná pro podporu Návrháře - neupravovat
        /// obsah této metody v editoru kódu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.render = new System.Windows.Forms.Timer(this.components);
            this.chbDebug = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // render
            // 
            this.render.Enabled = true;
            this.render.Interval = 15;
            this.render.Tick += new System.EventHandler(this.render_Tick);
            // 
            // chbDebug
            // 
            this.chbDebug.AutoSize = true;
            this.chbDebug.BackColor = System.Drawing.Color.Transparent;
            this.chbDebug.Location = new System.Drawing.Point(12, 12);
            this.chbDebug.Name = "chbDebug";
            this.chbDebug.Size = new System.Drawing.Size(58, 17);
            this.chbDebug.TabIndex = 0;
            this.chbDebug.Text = "Debug";
            this.chbDebug.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chbDebug);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Tower Defense";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer render;
        private System.Windows.Forms.CheckBox chbDebug;
    }
}

