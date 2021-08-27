
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.iteminfo_panel = new System.Windows.Forms.Panel();
            this.iteminfo_text = new System.Windows.Forms.RichTextBox();
            this.itemcompare_panel = new System.Windows.Forms.Panel();
            this.itemcompare_text = new System.Windows.Forms.RichTextBox();
            this.ItemCompareGrid = new System.Windows.Forms.DataGridView();
            this.iteminfo_ball = new System.Windows.Forms.DataGridView();
            this.iteminfo_panel.SuspendLayout();
            this.itemcompare_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemCompareGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iteminfo_ball)).BeginInit();
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
            this.iteminfo_panel.Controls.Add(this.iteminfo_ball);
            this.iteminfo_panel.Controls.Add(this.iteminfo_text);
            this.iteminfo_panel.ForeColor = System.Drawing.Color.Black;
            this.iteminfo_panel.Location = new System.Drawing.Point(12, 12);
            this.iteminfo_panel.Name = "iteminfo_panel";
            this.iteminfo_panel.Padding = new System.Windows.Forms.Padding(10);
            this.iteminfo_panel.Size = new System.Drawing.Size(223, 79);
            this.iteminfo_panel.TabIndex = 8;
            this.iteminfo_panel.LocationChanged += new System.EventHandler(this.itemwindow_panel_LocationChanged);
            this.iteminfo_panel.SizeChanged += new System.EventHandler(this.itemwindow_panel_SizeChanged);
            this.iteminfo_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.itemwindow_panel_Paint);
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
            this.iteminfo_text.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.itemwindow_text_ContentsResized);
            // 
            // itemcompare_panel
            // 
            this.itemcompare_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemcompare_panel.AutoSize = true;
            this.itemcompare_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.itemcompare_panel.BackColor = System.Drawing.Color.Black;
            this.itemcompare_panel.CausesValidation = false;
            this.itemcompare_panel.Controls.Add(this.itemcompare_text);
            this.itemcompare_panel.Controls.Add(this.ItemCompareGrid);
            this.itemcompare_panel.ForeColor = System.Drawing.Color.Black;
            this.itemcompare_panel.Location = new System.Drawing.Point(12, 95);
            this.itemcompare_panel.Name = "itemcompare_panel";
            this.itemcompare_panel.Padding = new System.Windows.Forms.Padding(10);
            this.itemcompare_panel.Size = new System.Drawing.Size(123, 69);
            this.itemcompare_panel.TabIndex = 7;
            this.itemcompare_panel.LocationChanged += new System.EventHandler(this.itemwindow_panel_LocationChanged);
            this.itemcompare_panel.SizeChanged += new System.EventHandler(this.itemwindow_panel_SizeChanged);
            this.itemcompare_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.itemwindow_panel_Paint);
            // 
            // itemcompare_text
            // 
            this.itemcompare_text.BackColor = System.Drawing.Color.Black;
            this.itemcompare_text.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.itemcompare_text.CausesValidation = false;
            this.itemcompare_text.Cursor = System.Windows.Forms.Cursors.Default;
            this.itemcompare_text.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.itemcompare_text.ForeColor = System.Drawing.Color.White;
            this.itemcompare_text.Location = new System.Drawing.Point(10, 8);
            this.itemcompare_text.Name = "itemcompare_text";
            this.itemcompare_text.ReadOnly = true;
            this.itemcompare_text.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.itemcompare_text.Size = new System.Drawing.Size(100, 20);
            this.itemcompare_text.TabIndex = 0;
            this.itemcompare_text.TabStop = false;
            this.itemcompare_text.Text = "text";
            this.itemcompare_text.WordWrap = false;
            this.itemcompare_text.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.itemwindow_text_ContentsResized);
            // 
            // ItemCompareGrid
            // 
            this.ItemCompareGrid.AllowUserToAddRows = false;
            this.ItemCompareGrid.AllowUserToDeleteRows = false;
            this.ItemCompareGrid.AllowUserToResizeColumns = false;
            this.ItemCompareGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.ItemCompareGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ItemCompareGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ItemCompareGrid.BackgroundColor = System.Drawing.Color.Black;
            this.ItemCompareGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.ItemCompareGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.ItemCompareGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.DefaultCellStyle = dataGridViewCellStyle8;
            this.ItemCompareGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ItemCompareGrid.EnableHeadersVisualStyles = false;
            this.ItemCompareGrid.GridColor = System.Drawing.Color.White;
            this.ItemCompareGrid.Location = new System.Drawing.Point(10, 26);
            this.ItemCompareGrid.MultiSelect = false;
            this.ItemCompareGrid.Name = "ItemCompareGrid";
            this.ItemCompareGrid.ReadOnly = true;
            this.ItemCompareGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.ItemCompareGrid.RowHeadersVisible = false;
            this.ItemCompareGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Black;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("굴림", 10F);
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Black;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.RowTemplate.Height = 23;
            this.ItemCompareGrid.RowTemplate.ReadOnly = true;
            this.ItemCompareGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.ItemCompareGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.ItemCompareGrid.ShowCellErrors = false;
            this.ItemCompareGrid.ShowCellToolTips = false;
            this.ItemCompareGrid.ShowEditingIcon = false;
            this.ItemCompareGrid.ShowRowErrors = false;
            this.ItemCompareGrid.Size = new System.Drawing.Size(100, 30);
            this.ItemCompareGrid.TabIndex = 0;
            this.ItemCompareGrid.TabStop = false;
            // 
            // iteminfo_ball
            // 
            this.iteminfo_ball.AllowUserToAddRows = false;
            this.iteminfo_ball.AllowUserToDeleteRows = false;
            this.iteminfo_ball.AllowUserToResizeColumns = false;
            this.iteminfo_ball.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.iteminfo_ball.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.iteminfo_ball.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.iteminfo_ball.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.iteminfo_ball.BackgroundColor = System.Drawing.Color.Black;
            this.iteminfo_ball.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.iteminfo_ball.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.iteminfo_ball.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.iteminfo_ball.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.iteminfo_ball.DefaultCellStyle = dataGridViewCellStyle3;
            this.iteminfo_ball.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.iteminfo_ball.EnableHeadersVisualStyles = false;
            this.iteminfo_ball.GridColor = System.Drawing.Color.White;
            this.iteminfo_ball.Location = new System.Drawing.Point(10, 36);
            this.iteminfo_ball.MultiSelect = false;
            this.iteminfo_ball.Name = "iteminfo_ball";
            this.iteminfo_ball.ReadOnly = true;
            this.iteminfo_ball.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.iteminfo_ball.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.iteminfo_ball.RowHeadersVisible = false;
            this.iteminfo_ball.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.iteminfo_ball.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.iteminfo_ball.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.iteminfo_ball.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Black;
            this.iteminfo_ball.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("굴림", 10F);
            this.iteminfo_ball.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.iteminfo_ball.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Black;
            this.iteminfo_ball.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.iteminfo_ball.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.iteminfo_ball.RowTemplate.Height = 23;
            this.iteminfo_ball.RowTemplate.ReadOnly = true;
            this.iteminfo_ball.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.iteminfo_ball.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.iteminfo_ball.ShowCellErrors = false;
            this.iteminfo_ball.ShowCellToolTips = false;
            this.iteminfo_ball.ShowEditingIcon = false;
            this.iteminfo_ball.ShowRowErrors = false;
            this.iteminfo_ball.Size = new System.Drawing.Size(200, 30);
            this.iteminfo_ball.TabIndex = 1;
            this.iteminfo_ball.TabStop = false;
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
            this.Controls.Add(this.itemcompare_panel);
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
            this.itemcompare_panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ItemCompareGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iteminfo_ball)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel iteminfo_panel;
        private System.Windows.Forms.RichTextBox iteminfo_text;
        private System.Windows.Forms.Panel itemcompare_panel;
        private System.Windows.Forms.DataGridView ItemCompareGrid;
        private System.Windows.Forms.RichTextBox itemcompare_text;
        private System.Windows.Forms.DataGridView iteminfo_ball;
    }
}