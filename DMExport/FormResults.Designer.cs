namespace DMExport
{
    partial class FormResults
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

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
            this.lblExportResult = new System.Windows.Forms.Label();
            this.lstItems = new System.Windows.Forms.ListBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblExportResult
            // 
            this.lblExportResult.Location = new System.Drawing.Point(10, 9);
            this.lblExportResult.Name = "lblExportResult";
            this.lblExportResult.Size = new System.Drawing.Size(472, 50);
            this.lblExportResult.TabIndex = 0;
            this.lblExportResult.Text = "export result";
            // 
            // lstItems
            // 
            this.lstItems.FormattingEnabled = true;
            this.lstItems.Location = new System.Drawing.Point(10, 62);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(472, 82);
            this.lstItems.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(382, 150);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOkClick);
            // 
            // Warnings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(494, 185);
            this.ControlBox = false;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lstItems);
            this.Controls.Add(this.lblExportResult);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Warnings";
            this.Text = "DM Export Tool. Export Completed.";
            this.Load += new System.EventHandler(this.Warnings_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblExportResult;
        private System.Windows.Forms.ListBox lstItems;
        private System.Windows.Forms.Button btnOk;
    }
}