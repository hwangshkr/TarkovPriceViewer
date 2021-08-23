﻿
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle31 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle32 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle33 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle34 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle35 = new System.Windows.Forms.DataGridViewCellStyle();
            this.iteminfo_panel = new System.Windows.Forms.Panel();
            this.iteminfo_text = new System.Windows.Forms.RichTextBox();
            this.itemcompare_panel = new System.Windows.Forms.Panel();
            this.ItemCompareGrid = new System.Windows.Forms.DataGridView();
            this.itemcompare_text = new System.Windows.Forms.RichTextBox();
            this.iteminfo_panel.SuspendLayout();
            this.itemcompare_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemCompareGrid)).BeginInit();
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
            this.itemcompare_panel.Location = new System.Drawing.Point(12, 61);
            this.itemcompare_panel.Name = "itemcompare_panel";
            this.itemcompare_panel.Padding = new System.Windows.Forms.Padding(10);
            this.itemcompare_panel.Size = new System.Drawing.Size(123, 69);
            this.itemcompare_panel.TabIndex = 7;
            this.itemcompare_panel.LocationChanged += new System.EventHandler(this.itemwindow_panel_LocationChanged);
            this.itemcompare_panel.SizeChanged += new System.EventHandler(this.itemwindow_panel_SizeChanged);
            this.itemcompare_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.itemwindow_panel_Paint);
            // 
            // ItemCompareGrid
            // 
            this.ItemCompareGrid.AllowUserToAddRows = false;
            this.ItemCompareGrid.AllowUserToDeleteRows = false;
            this.ItemCompareGrid.AllowUserToResizeColumns = false;
            this.ItemCompareGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle31.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle31.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle31.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle31.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle31.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle31.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle31.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle31;
            this.ItemCompareGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ItemCompareGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ItemCompareGrid.BackgroundColor = System.Drawing.Color.Black;
            this.ItemCompareGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.ItemCompareGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle32.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle32.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle32.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle32.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle32.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle32.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle32.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle32;
            this.ItemCompareGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle33.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle33.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle33.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle33.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle33.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle33.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle33.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.DefaultCellStyle = dataGridViewCellStyle33;
            this.ItemCompareGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ItemCompareGrid.EnableHeadersVisualStyles = false;
            this.ItemCompareGrid.GridColor = System.Drawing.Color.White;
            this.ItemCompareGrid.Location = new System.Drawing.Point(10, 26);
            this.ItemCompareGrid.MultiSelect = false;
            this.ItemCompareGrid.Name = "ItemCompareGrid";
            this.ItemCompareGrid.ReadOnly = true;
            this.ItemCompareGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle34.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle34.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle34.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle34.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle34.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle34.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle34.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle34;
            this.ItemCompareGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle35.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle35.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle35.Font = new System.Drawing.Font("굴림", 10F);
            dataGridViewCellStyle35.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle35.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle35.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle35.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.RowsDefaultCellStyle = dataGridViewCellStyle35;
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel iteminfo_panel;
        private System.Windows.Forms.RichTextBox iteminfo_text;
        private System.Windows.Forms.Panel itemcompare_panel;
        private System.Windows.Forms.DataGridView ItemCompareGrid;
        private System.Windows.Forms.RichTextBox itemcompare_text;
    }
}