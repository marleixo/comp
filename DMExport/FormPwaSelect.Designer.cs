namespace DMExport
{
    partial class FormPwaSelect
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
            this.txtServerURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server URL:";
            // 
            // txtServerURL
            // 
            this.txtServerURL.Location = new System.Drawing.Point(84, 15);
            this.txtServerURL.Name = "txtServerURL";
            this.txtServerURL.Size = new System.Drawing.Size(290, 20);
            this.txtServerURL.TabIndex = 1;
            this.txtServerURL.Text = "http://epm2010svr/PWA_PT";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(363, 30);
            this.label2.TabIndex = 2;
            this.label2.Text = "Specify the URL of the Project Web Access Server where you would like to backup o" +
    "r restore settings.";
            // 
            // btnContinue
            // 
            this.btnContinue.Location = new System.Drawing.Point(142, 100);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(107, 23);
            this.btnContinue.TabIndex = 3;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Location = new System.Drawing.Point(268, 100);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(107, 23);
            this.btnQuit.TabIndex = 4;
            this.btnQuit.Text = "Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // FormPwaSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 136);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServerURL);
            this.Controls.Add(this.label1);
            this.Name = "FormPwaSelect";
            this.Text = "Select Server URL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServerURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnQuit;
    }
}

