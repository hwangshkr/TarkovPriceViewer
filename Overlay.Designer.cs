
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
            this.itemusage = new System.Windows.Forms.Label();
            this.traderprice = new System.Windows.Forms.Label();
            this.itemtrader = new System.Windows.Forms.Label();
            this.itemprice = new System.Windows.Forms.Label();
            this.itemname = new System.Windows.Forms.Label();
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
            this.iteminfo_panel.BackColor = System.Drawing.Color.Black;
            this.iteminfo_panel.Controls.Add(this.itemusage);
            this.iteminfo_panel.Controls.Add(this.traderprice);
            this.iteminfo_panel.Controls.Add(this.itemtrader);
            this.iteminfo_panel.Controls.Add(this.itemprice);
            this.iteminfo_panel.Controls.Add(this.itemname);
            this.iteminfo_panel.ForeColor = System.Drawing.Color.Black;
            this.iteminfo_panel.Location = new System.Drawing.Point(12, 12);
            this.iteminfo_panel.Name = "iteminfo_panel";
            this.iteminfo_panel.Padding = new System.Windows.Forms.Padding(10);
            this.iteminfo_panel.Size = new System.Drawing.Size(124, 119);
            this.iteminfo_panel.TabIndex = 7;
            this.iteminfo_panel.LocationChanged += new System.EventHandler(this.iteminfo_panel_LocationChanged);
            this.iteminfo_panel.SizeChanged += new System.EventHandler(this.iteminfo_panel_SizeChanged);
            this.iteminfo_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.iteminfo_panel_Paint);
            // 
            // itemusage
            // 
            this.itemusage.AutoSize = true;
            this.itemusage.BackColor = System.Drawing.Color.Transparent;
            this.itemusage.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.itemusage.ForeColor = System.Drawing.Color.White;
            this.itemusage.Location = new System.Drawing.Point(14, 94);
            this.itemusage.Margin = new System.Windows.Forms.Padding(3);
            this.itemusage.Name = "itemusage";
            this.itemusage.Size = new System.Drawing.Size(69, 12);
            this.itemusage.TabIndex = 1;
            this.itemusage.Text = "Needs for";
            this.itemusage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // traderprice
            // 
            this.traderprice.AutoSize = true;
            this.traderprice.BackColor = System.Drawing.Color.Transparent;
            this.traderprice.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.traderprice.ForeColor = System.Drawing.Color.White;
            this.traderprice.Location = new System.Drawing.Point(14, 74);
            this.traderprice.Margin = new System.Windows.Forms.Padding(3);
            this.traderprice.Name = "traderprice";
            this.traderprice.Size = new System.Drawing.Size(97, 12);
            this.traderprice.TabIndex = 0;
            this.traderprice.Text = "Trader Price :";
            this.traderprice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // itemtrader
            // 
            this.itemtrader.AutoSize = true;
            this.itemtrader.BackColor = System.Drawing.Color.Transparent;
            this.itemtrader.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.itemtrader.ForeColor = System.Drawing.Color.White;
            this.itemtrader.Location = new System.Drawing.Point(14, 54);
            this.itemtrader.Margin = new System.Windows.Forms.Padding(3);
            this.itemtrader.Name = "itemtrader";
            this.itemtrader.Size = new System.Drawing.Size(58, 12);
            this.itemtrader.TabIndex = 0;
            this.itemtrader.Text = "Trader :";
            this.itemtrader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // itemprice
            // 
            this.itemprice.AutoSize = true;
            this.itemprice.BackColor = System.Drawing.Color.Transparent;
            this.itemprice.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.itemprice.ForeColor = System.Drawing.Color.White;
            this.itemprice.Location = new System.Drawing.Point(14, 34);
            this.itemprice.Margin = new System.Windows.Forms.Padding(3);
            this.itemprice.Name = "itemprice";
            this.itemprice.Size = new System.Drawing.Size(82, 12);
            this.itemprice.TabIndex = 0;
            this.itemprice.Text = "Flea Price :";
            this.itemprice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // itemname
            // 
            this.itemname.AutoSize = true;
            this.itemname.BackColor = System.Drawing.Color.Transparent;
            this.itemname.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.itemname.ForeColor = System.Drawing.Color.White;
            this.itemname.Location = new System.Drawing.Point(14, 17);
            this.itemname.Margin = new System.Windows.Forms.Padding(3);
            this.itemname.Name = "itemname";
            this.itemname.Size = new System.Drawing.Size(53, 12);
            this.itemname.TabIndex = 0;
            this.itemname.Text = "Name :";
            this.itemname.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
        private System.Windows.Forms.Label itemtrader;
        private System.Windows.Forms.Label traderprice;
        private System.Windows.Forms.Label itemusage;
    }
}