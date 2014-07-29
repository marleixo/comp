namespace DMExport
{
    partial class FormMain
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
            this.txtExportLocation = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.treeView = new System.Windows.Forms.TreeView();
            this.backgroundWorkerExport = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerLoad = new System.ComponentModel.BackgroundWorker();
            this.txtFeatureName = new System.Windows.Forms.TextBox();
            this.lblFeatureName = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatusCaption = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblImidiateStatus = new System.Windows.Forms.Label();
            this.cbEnableAutoComplete = new System.Windows.Forms.CheckBox();
            this.btnLoadSelection = new System.Windows.Forms.Button();
            this.btnSaveSelection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select which objects to export";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 582);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Export location";
            // 
            // txtExportLocation
            // 
            this.txtExportLocation.Enabled = false;
            this.txtExportLocation.Location = new System.Drawing.Point(12, 601);
            this.txtExportLocation.Name = "txtExportLocation";
            this.txtExportLocation.Size = new System.Drawing.Size(428, 20);
            this.txtExportLocation.TabIndex = 3;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(446, 599);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(108, 23);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(12, 644);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(446, 644);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(108, 23);
            this.btnExport.TabIndex = 7;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Location = new System.Drawing.Point(12, 71);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(542, 366);
            this.treeView.TabIndex = 8;
            this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
            // 
            // backgroundWorkerExport
            // 
            this.backgroundWorkerExport.WorkerReportsProgress = true;
            this.backgroundWorkerExport.WorkerSupportsCancellation = true;
            // 
            // txtFeatureName
            // 
            this.txtFeatureName.Enabled = false;
            this.txtFeatureName.Location = new System.Drawing.Point(12, 544);
            this.txtFeatureName.Name = "txtFeatureName";
            this.txtFeatureName.Size = new System.Drawing.Size(542, 20);
            this.txtFeatureName.TabIndex = 9;
            // 
            // lblFeatureName
            // 
            this.lblFeatureName.AutoSize = true;
            this.lblFeatureName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFeatureName.Location = new System.Drawing.Point(9, 524);
            this.lblFeatureName.Name = "lblFeatureName";
            this.lblFeatureName.Size = new System.Drawing.Size(108, 17);
            this.lblFeatureName.TabIndex = 10;
            this.lblFeatureName.Text = "Feature name";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 465);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(542, 23);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 11;
            // 
            // lblStatusCaption
            // 
            this.lblStatusCaption.AutoSize = true;
            this.lblStatusCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusCaption.Location = new System.Drawing.Point(9, 491);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(47, 15);
            this.lblStatusCaption.TabIndex = 13;
            this.lblStatusCaption.Text = "Status: ";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(51, 491);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(93, 15);
            this.lblStatus.TabIndex = 14;
            this.lblStatus.Text = "Progress Status";
            // 
            // lblImidiateStatus
            // 
            this.lblImidiateStatus.AutoSize = true;
            this.lblImidiateStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImidiateStatus.Location = new System.Drawing.Point(9, 447);
            this.lblImidiateStatus.Name = "lblImidiateStatus";
            this.lblImidiateStatus.Size = new System.Drawing.Size(0, 15);
            this.lblImidiateStatus.TabIndex = 15;
            // 
            // cbEnableAutoComplete
            // 
            this.cbEnableAutoComplete.AutoSize = true;
            this.cbEnableAutoComplete.Checked = true;
            this.cbEnableAutoComplete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEnableAutoComplete.Location = new System.Drawing.Point(12, 22);
            this.cbEnableAutoComplete.Name = "cbEnableAutoComplete";
            this.cbEnableAutoComplete.Size = new System.Drawing.Size(131, 17);
            this.cbEnableAutoComplete.TabIndex = 17;
            this.cbEnableAutoComplete.Text = "Enable Auto-Complete";
            this.cbEnableAutoComplete.UseVisualStyleBackColor = true;
            // 
            // btnLoadSelection
            // 
            this.btnLoadSelection.Location = new System.Drawing.Point(333, 18);
            this.btnLoadSelection.Name = "btnLoadSelection";
            this.btnLoadSelection.Size = new System.Drawing.Size(107, 23);
            this.btnLoadSelection.TabIndex = 18;
            this.btnLoadSelection.Text = "Import Selection";
            this.btnLoadSelection.UseVisualStyleBackColor = true;
            this.btnLoadSelection.Click += new System.EventHandler(this.btnLoadSelection_Click);
            // 
            // btnSaveSelection
            // 
            this.btnSaveSelection.Location = new System.Drawing.Point(447, 18);
            this.btnSaveSelection.Name = "btnSaveSelection";
            this.btnSaveSelection.Size = new System.Drawing.Size(107, 23);
            this.btnSaveSelection.TabIndex = 19;
            this.btnSaveSelection.Text = "Save Selection";
            this.btnSaveSelection.UseVisualStyleBackColor = true;
            this.btnSaveSelection.Click += new System.EventHandler(this.btnSaveSelection_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(566, 679);
            this.Controls.Add(this.btnSaveSelection);
            this.Controls.Add(this.btnLoadSelection);
            this.Controls.Add(this.cbEnableAutoComplete);
            this.Controls.Add(this.lblImidiateStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblStatusCaption);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblFeatureName);
            this.Controls.Add(this.txtFeatureName);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtExportLocation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormMain";
            this.Text = "DM Export Tool";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_Closed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtExportLocation;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.TreeView treeView;
        private System.ComponentModel.BackgroundWorker backgroundWorkerExport;
		private System.ComponentModel.BackgroundWorker backgroundWorkerLoad;
		private System.Windows.Forms.TextBox txtFeatureName;
		private System.Windows.Forms.Label lblFeatureName;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatusCaption;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblImidiateStatus;
        private System.Windows.Forms.CheckBox cbEnableAutoComplete;
        private System.Windows.Forms.Button btnLoadSelection;
        private System.Windows.Forms.Button btnSaveSelection;
    }
}