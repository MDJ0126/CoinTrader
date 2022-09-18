namespace CoinTrader.Forms
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.accessKeyText = new System.Windows.Forms.TextBox();
            this.secretKeyText = new System.Windows.Forms.TextBox();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "AccessKey";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "SecretKey";
            // 
            // accessKeyText
            // 
            this.accessKeyText.Location = new System.Drawing.Point(118, 96);
            this.accessKeyText.Name = "accessKeyText";
            this.accessKeyText.Size = new System.Drawing.Size(294, 21);
            this.accessKeyText.TabIndex = 2;
            this.accessKeyText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // secretKeyText
            // 
            this.secretKeyText.Location = new System.Drawing.Point(118, 135);
            this.secretKeyText.Name = "secretKeyText";
            this.secretKeyText.Size = new System.Drawing.Size(294, 21);
            this.secretKeyText.TabIndex = 2;
            this.secretKeyText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox2_KeyDown);
            // 
            // metroButton1
            // 
            this.metroButton1.Location = new System.Drawing.Point(118, 172);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(294, 33);
            this.metroButton1.TabIndex = 3;
            this.metroButton1.Text = "Login";
            this.metroButton1.UseSelectable = true;
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 218);
            this.Controls.Add(this.metroButton1);
            this.Controls.Add(this.secretKeyText);
            this.Controls.Add(this.accessKeyText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "LoginForm";
            this.Resizable = false;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox accessKeyText;
        private System.Windows.Forms.TextBox secretKeyText;
        private MetroFramework.Controls.MetroButton metroButton1;
    }
}