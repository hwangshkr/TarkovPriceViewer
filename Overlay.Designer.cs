
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
            this.iteminfo_text = new System.Windows.Forms.RichTextBox();
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
            this.iteminfo_panel.CausesValidation = false;
            this.iteminfo_panel.Controls.Add(this.iteminfo_text);
            this.iteminfo_panel.ForeColor = System.Drawing.Color.Black;
            this.iteminfo_panel.Location = new System.Drawing.Point(12, 12);
            this.iteminfo_panel.Name = "iteminfo_panel";
            this.iteminfo_panel.Padding = new System.Windows.Forms.Padding(10);
            this.iteminfo_panel.Size = new System.Drawing.Size(123, 43);
            this.iteminfo_panel.TabIndex = 7;
            this.iteminfo_panel.LocationChanged += new System.EventHandler(this.iteminfo_panel_LocationChanged);
            this.iteminfo_panel.SizeChanged += new System.EventHandler(this.iteminfo_panel_SizeChanged);
            this.iteminfo_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.iteminfo_panel_Paint);
            // 
            // iteminfo_text
            // 
            this.iteminfo_text.BackColor = System.Drawing.Color.Black;
            this.iteminfo_text.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.iteminfo_text.CausesValidation = false;
            this.iteminfo_text.Cursor = System.Windows.Forms.Cursors.Default;
            this.iteminfo_text.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.iteminfo_text.ForeColor = System.Drawing.Color.White;
            this.iteminfo_text.Location = new System.Drawing.Point(10, 10);
            this.iteminfo_text.Name = "iteminfo_text";
            this.iteminfo_text.ReadOnly = true;
            this.iteminfo_text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.iteminfo_text.Size = new System.Drawing.Size(100, 20);
            this.iteminfo_text.TabIndex = 0;
            this.iteminfo_text.TabStop = false;
            this.iteminfo_text.Text = "text";
            this.iteminfo_text.WordWrap = false;
            this.iteminfo_text.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.iteminfo_text_ContentsResized);
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.iteminfo_panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Overlay";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Overlay";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Overlay_FormClosing);
            this.iteminfo_panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel iteminfo_panel;
        private System.Windows.Forms.RichTextBox iteminfo_text;
    }
}