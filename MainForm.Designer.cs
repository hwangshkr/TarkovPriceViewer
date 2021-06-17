
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
            this.testB = new System.Windows.Forms.Button();
            this.testimageview = new System.Windows.Forms.Panel();
            this.testdrawbox = new System.Windows.Forms.PictureBox();
            this.testimageview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testdrawbox)).BeginInit();
            this.SuspendLayout();
            // 
            // testB
            // 
            this.testB.Location = new System.Drawing.Point(658, 435);
            this.testB.Name = "testB";
            this.testB.Size = new System.Drawing.Size(145, 57);
            this.testB.TabIndex = 0;
            this.testB.Text = "TestButton";
            this.testB.UseVisualStyleBackColor = true;
            this.testB.Click += new System.EventHandler(this.testB_Click);
            // 
            // testimageview
            // 
            this.testimageview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.testimageview.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.testimageview.BackColor = System.Drawing.Color.Black;
            this.testimageview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.testimageview.Controls.Add(this.testdrawbox);
            this.testimageview.Location = new System.Drawing.Point(13, 13);
            this.testimageview.Margin = new System.Windows.Forms.Padding(0);
            this.testimageview.Name = "testimageview";
            this.testimageview.Size = new System.Drawing.Size(640, 480);
            this.testimageview.TabIndex = 2;
            // 
            // testdrawbox
            // 
            this.testdrawbox.BackColor = System.Drawing.Color.Transparent;
            this.testdrawbox.Location = new System.Drawing.Point(0, 0);
            this.testdrawbox.Margin = new System.Windows.Forms.Padding(0);
            this.testdrawbox.Name = "testdrawbox";
            this.testdrawbox.Size = new System.Drawing.Size(640, 480);
            this.testdrawbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.testdrawbox.TabIndex = 0;
            this.testdrawbox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 512);
            this.Controls.Add(this.testimageview);
            this.Controls.Add(this.testB);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.testimageview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.testdrawbox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button testB;
        private System.Windows.Forms.Panel testimageview;
        private System.Windows.Forms.PictureBox testdrawbox;
    }
}

