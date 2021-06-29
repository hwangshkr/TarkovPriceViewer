
namespace TarkovPriceViewer
{
    partial class Overlay
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
            this.iteminfo_panel = new System.Windows.Forms.Panel();
            this.itemprice = new System.Windows.Forms.Label();
            this.itemname = new System.Windows.Forms.Label();
            this.itemdealer = new System.Windows.Forms.Label();
            this.iteminfo_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // iteminfo_panel
            // 
            this.iteminfo_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.iteminfo_panel.AutoSize = true;
            this.iteminfo_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.iteminfo_panel.BackColor = System.Drawing.Color.White;
            this.iteminfo_panel.Controls.Add(this.itemdealer);
            this.iteminfo_panel.Controls.Add(this.itemprice);
            this.iteminfo_panel.Controls.Add(this.itemname);
            this.iteminfo_panel.Location = new System.Drawing.Point(12, 12);
            this.iteminfo_panel.Name = "iteminfo_panel";
            this.iteminfo_panel.Size = new System.Drawing.Size(45, 59);
            this.iteminfo_panel.TabIndex = 7;
            // 
            // itemprice
            // 
            this.itemprice.AutoSize = true;
            this.itemprice.BackColor = System.Drawing.Color.Transparent;
            this.itemprice.Location = new System.Drawing.Point(4, 24);
            this.itemprice.Margin = new System.Windows.Forms.Padding(3);
            this.itemprice.Name = "itemprice";
            this.itemprice.Size = new System.Drawing.Size(38, 12);
            this.itemprice.TabIndex = 1;
            this.itemprice.Text = "label1";
            // 
            // itemname
            // 
            this.itemname.AutoSize = true;
            this.itemname.BackColor = System.Drawing.Color.Transparent;
            this.itemname.Location = new System.Drawing.Point(4, 4);
            this.itemname.Margin = new System.Windows.Forms.Padding(3);
            this.itemname.Name = "itemname";
            this.itemname.Size = new System.Drawing.Size(38, 12);
            this.itemname.TabIndex = 0;
            this.itemname.Text = "label1";
            // 
            // itemdealer
            // 
            this.itemdealer.AutoSize = true;
            this.itemdealer.BackColor = System.Drawing.Color.Transparent;
            this.itemdealer.Location = new System.Drawing.Point(4, 44);
            this.itemdealer.Margin = new System.Windows.Forms.Padding(3);
            this.itemdealer.Name = "itemdealer";
            this.itemdealer.Size = new System.Drawing.Size(38, 12);
            this.itemdealer.TabIndex = 1;
            this.itemdealer.Text = "label1";
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.iteminfo_panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Overlay";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Overlay";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.iteminfo_panel.ResumeLayout(false);
            this.iteminfo_panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel iteminfo_panel;
        private System.Windows.Forms.Label itemprice;
        private System.Windows.Forms.Label itemname;
        private System.Windows.Forms.Label itemdealer;
    }
}