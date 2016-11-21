/*
 * Created by SharpDevelop.
 * User: Burhan
 * Date: 11/05/2014
 * Time: 01:02 ص
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Voronoi2
{
  sealed partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
      this.GenerateButton = new System.Windows.Forms.Button();
      this.DiagramHost = new System.Windows.Forms.PictureBox();
      this.OutputBox = new System.Windows.Forms.RichTextBox();
      this.SiteCounter = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this.DiagramHost)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.SiteCounter)).BeginInit();
      this.SuspendLayout();
      // 
      // GenerateButton
      // 
      this.GenerateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.GenerateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.GenerateButton.Location = new System.Drawing.Point(628, 860);
      this.GenerateButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.GenerateButton.Name = "GenerateButton";
      this.GenerateButton.Size = new System.Drawing.Size(158, 42);
      this.GenerateButton.TabIndex = 0;
      this.GenerateButton.Text = "Generate";
      this.GenerateButton.UseVisualStyleBackColor = true;
      this.GenerateButton.Click += new System.EventHandler(this.OnGenerateClick);
      // 
      // DiagramHost
      // 
      this.DiagramHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.DiagramHost.Location = new System.Drawing.Point(18, 18);
      this.DiagramHost.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.DiagramHost.Name = "DiagramHost";
      this.DiagramHost.Size = new System.Drawing.Size(768, 788);
      this.DiagramHost.TabIndex = 1;
      this.DiagramHost.TabStop = false;
      this.DiagramHost.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnDiagramHostMouseMove);
      // 
      // OutputBox
      // 
      this.OutputBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.OutputBox.Location = new System.Drawing.Point(795, 18);
      this.OutputBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.OutputBox.Name = "OutputBox";
      this.OutputBox.Size = new System.Drawing.Size(462, 881);
      this.OutputBox.TabIndex = 4;
      this.OutputBox.Text = "";
      this.OutputBox.WordWrap = false;
      // 
      // SiteCounter
      // 
      this.SiteCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.SiteCounter.Location = new System.Drawing.Point(628, 824);
      this.SiteCounter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.SiteCounter.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.SiteCounter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.SiteCounter.Name = "SiteCounter";
      this.SiteCounter.Size = new System.Drawing.Size(158, 26);
      this.SiteCounter.TabIndex = 5;
      this.SiteCounter.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
      this.SiteCounter.ValueChanged += new System.EventHandler(this.OnSiteCounterChanged);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.ClientSize = new System.Drawing.Size(1276, 920);
      this.Controls.Add(this.SiteCounter);
      this.Controls.Add(this.OutputBox);
      this.Controls.Add(this.DiagramHost);
      this.Controls.Add(this.GenerateButton);
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "MainForm";
      this.Text = "Voronoi";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      ((System.ComponentModel.ISupportInitialize)(this.DiagramHost)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.SiteCounter)).EndInit();
      this.ResumeLayout(false);

		}
		private System.Windows.Forms.NumericUpDown SiteCounter;
		private System.Windows.Forms.RichTextBox OutputBox;
		private System.Windows.Forms.PictureBox DiagramHost;
		private System.Windows.Forms.Button GenerateButton;
	}
}
