
namespace TarkovPriceViewer
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayshow = new System.Windows.Forms.ToolStripMenuItem();
            this.trayexit = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.Donate = new System.Windows.Forms.Button();
            this.Update = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button5 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.MadeBy = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.TarkovMarket = new System.Windows.Forms.Button();
            this.TarkovWiki = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.DataProvidedBy = new System.Windows.Forms.Label();
            this.TrayMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = this.TrayMenu;
            this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
            this.TrayIcon.Text = "TarkovPriceViewer";
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseDoubleClick);
            // 
            // TrayMenu
            // 
            this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayshow,
            this.trayexit});
            this.TrayMenu.Name = "TrayMenu";
            this.TrayMenu.Size = new System.Drawing.Size(105, 48);
            // 
            // trayshow
            // 
            this.trayshow.Name = "trayshow";
            this.trayshow.Size = new System.Drawing.Size(104, 22);
            this.trayshow.Text = "Show";
            this.trayshow.Click += new System.EventHandler(this.TrayShow_Click);
            // 
            // trayexit
            // 
            this.trayexit.Name = "trayexit";
            this.trayexit.Size = new System.Drawing.Size(104, 22);
            this.trayexit.Text = "Exit";
            this.trayexit.Click += new System.EventHandler(this.TrayExit_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(58, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // Donate
            // 
            this.Donate.Location = new System.Drawing.Point(97, 74);
            this.Donate.Name = "Donate";
            this.Donate.Size = new System.Drawing.Size(75, 23);
            this.Donate.TabIndex = 4;
            this.Donate.Text = "Donate";
            this.Donate.UseVisualStyleBackColor = true;
            // 
            // Update
            // 
            this.Update.Location = new System.Drawing.Point(97, 20);
            this.Update.Name = "Update";
            this.Update.Size = new System.Drawing.Size(75, 23);
            this.Update.TabIndex = 5;
            this.Update.Text = "Update";
            this.Update.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(101, 41);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button4);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(58, 165);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 100);
            this.panel2.TabIndex = 1;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(101, 41);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 1;
            this.button4.Text = "button3";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "label1";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button5);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Location = new System.Drawing.Point(58, 271);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 100);
            this.panel3.TabIndex = 1;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(101, 41);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 1;
            this.button5.Text = "button3";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "label1";
            // 
            // MadeBy
            // 
            this.MadeBy.Location = new System.Drawing.Point(37, 49);
            this.MadeBy.Name = "MadeBy";
            this.MadeBy.Size = new System.Drawing.Size(75, 23);
            this.MadeBy.TabIndex = 5;
            this.MadeBy.Text = "MadeBy";
            this.MadeBy.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(380, 411);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(86, 16);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // TarkovMarket
            // 
            this.TarkovMarket.Location = new System.Drawing.Point(84, 32);
            this.TarkovMarket.Name = "TarkovMarket";
            this.TarkovMarket.Size = new System.Drawing.Size(75, 23);
            this.TarkovMarket.TabIndex = 5;
            this.TarkovMarket.Text = "TarkovMarket";
            this.TarkovMarket.UseVisualStyleBackColor = true;
            // 
            // TarkovWiki
            // 
            this.TarkovWiki.Location = new System.Drawing.Point(84, 3);
            this.TarkovWiki.Name = "TarkovWiki";
            this.TarkovWiki.Size = new System.Drawing.Size(75, 23);
            this.TarkovWiki.TabIndex = 5;
            this.TarkovWiki.Text = "TarkovWiki";
            this.TarkovWiki.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.DataProvidedBy);
            this.panel4.Controls.Add(this.TarkovWiki);
            this.panel4.Controls.Add(this.TarkovMarket);
            this.panel4.Location = new System.Drawing.Point(419, 271);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(200, 100);
            this.panel4.TabIndex = 7;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.MadeBy);
            this.panel5.Controls.Add(this.Update);
            this.panel5.Controls.Add(this.Donate);
            this.panel5.Location = new System.Drawing.Point(419, 149);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(200, 100);
            this.panel5.TabIndex = 8;
            // 
            // DataProvidedBy
            // 
            this.DataProvidedBy.AutoSize = true;
            this.DataProvidedBy.Location = new System.Drawing.Point(37, 63);
            this.DataProvidedBy.Name = "DataProvidedBy";
            this.DataProvidedBy.Size = new System.Drawing.Size(94, 12);
            this.DataProvidedBy.TabIndex = 6;
            this.DataProvidedBy.Text = "DataProvidedBy";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "TarkovPriceViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_closed);
            this.Load += new System.EventHandler(this.MainForm_load);
            this.Move += new System.EventHandler(this.MainForm_Move);
            this.TrayMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.ContextMenuStrip TrayMenu;
        private System.Windows.Forms.ToolStripMenuItem trayshow;
        private System.Windows.Forms.ToolStripMenuItem trayexit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Donate;
        private System.Windows.Forms.Button Update;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button MadeBy;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button TarkovMarket;
        private System.Windows.Forms.Button TarkovWiki;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label DataProvidedBy;
        private System.Windows.Forms.Panel panel5;
    }
}

