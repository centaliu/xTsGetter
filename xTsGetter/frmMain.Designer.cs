namespace xTsGetter
{
	partial class frmMain
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
			this.txtURLs = new System.Windows.Forms.TextBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDecode = new System.Windows.Forms.Button();
			this.lblStatus = new System.Windows.Forms.Label();
			this.btnDownload = new System.Windows.Forms.Button();
			this.txtDest = new System.Windows.Forms.TextBox();
			this.chkStop = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// txtURLs
			// 
			this.txtURLs.Location = new System.Drawing.Point(0, 0);
			this.txtURLs.Multiline = true;
			this.txtURLs.Name = "txtURLs";
			this.txtURLs.Size = new System.Drawing.Size(431, 315);
			this.txtURLs.TabIndex = 0;
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(12, 321);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(64, 30);
			this.btnAdd.TabIndex = 1;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnDecode
			// 
			this.btnDecode.Location = new System.Drawing.Point(84, 321);
			this.btnDecode.Name = "btnDecode";
			this.btnDecode.Size = new System.Drawing.Size(64, 30);
			this.btnDecode.TabIndex = 2;
			this.btnDecode.Text = "Decode";
			this.btnDecode.UseVisualStyleBackColor = true;
			this.btnDecode.Click += new System.EventHandler(this.btnDecode_Click);
			// 
			// lblStatus
			// 
			this.lblStatus.AutoSize = true;
			this.lblStatus.Location = new System.Drawing.Point(12, 364);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(37, 13);
			this.lblStatus.TabIndex = 3;
			this.lblStatus.Text = "Status";
			// 
			// btnDownload
			// 
			this.btnDownload.Location = new System.Drawing.Point(159, 321);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(64, 30);
			this.btnDownload.TabIndex = 4;
			this.btnDownload.Text = "Download";
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
			// 
			// txtDest
			// 
			this.txtDest.Location = new System.Drawing.Point(229, 331);
			this.txtDest.Name = "txtDest";
			this.txtDest.Size = new System.Drawing.Size(202, 20);
			this.txtDest.TabIndex = 5;
			this.txtDest.Text = "D:\\Opensource\\Not\\flv\\";
			// 
			// chkStop
			// 
			this.chkStop.AutoSize = true;
			this.chkStop.Location = new System.Drawing.Point(379, 365);
			this.chkStop.Name = "chkStop";
			this.chkStop.Size = new System.Drawing.Size(48, 17);
			this.chkStop.TabIndex = 6;
			this.chkStop.Text = "Stop";
			this.chkStop.UseVisualStyleBackColor = true;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(435, 386);
			this.Controls.Add(this.chkStop);
			this.Controls.Add(this.txtDest);
			this.Controls.Add(this.btnDownload);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.btnDecode);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.txtURLs);
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Text = "xTsGetter";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtURLs;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDecode;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Button btnDownload;
		private System.Windows.Forms.TextBox txtDest;
		private System.Windows.Forms.CheckBox chkStop;
	}
}

