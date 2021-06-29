
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
            this.next = new System.Windows.Forms.Button();
            this.imagefound = new System.Windows.Forms.PictureBox();
            this.imagesub = new System.Windows.Forms.PictureBox();
            this.imageshould = new System.Windows.Forms.PictureBox();
            this.texttest = new System.Windows.Forms.PictureBox();
            this.texttest2 = new System.Windows.Forms.PictureBox();
            this.testdrawbox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imagefound)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imagesub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageshould)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.texttest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.texttest2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testdrawbox)).BeginInit();
            this.SuspendLayout();
            // 
            // testB
            // 
            this.testB.Location = new System.Drawing.Point(1107, 692);
            this.testB.Name = "testB";
            this.testB.Size = new System.Drawing.Size(145, 57);
            this.testB.TabIndex = 0;
            this.testB.Text = "TestButton";
            this.testB.UseVisualStyleBackColor = true;
            // 
            // next
            // 
            this.next.Location = new System.Drawing.Point(1177, 663);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(75, 23);
            this.next.TabIndex = 3;
            this.next.Text = "next";
            this.next.UseVisualStyleBackColor = true;
            // 
            // imagefound
            // 
            this.imagefound.Location = new System.Drawing.Point(1127, 55);
            this.imagefound.Name = "imagefound";
            this.imagefound.Size = new System.Drawing.Size(100, 100);
            this.imagefound.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imagefound.TabIndex = 4;
            this.imagefound.TabStop = false;
            // 
            // imagesub
            // 
            this.imagesub.Location = new System.Drawing.Point(1127, 267);
            this.imagesub.Name = "imagesub";
            this.imagesub.Size = new System.Drawing.Size(100, 100);
            this.imagesub.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imagesub.TabIndex = 4;
            this.imagesub.TabStop = false;
            // 
            // imageshould
            // 
            this.imageshould.Location = new System.Drawing.Point(1127, 161);
            this.imageshould.Name = "imageshould";
            this.imageshould.Size = new System.Drawing.Size(100, 100);
            this.imageshould.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageshould.TabIndex = 4;
            this.imageshould.TabStop = false;
            // 
            // texttest
            // 
            this.texttest.Location = new System.Drawing.Point(1127, 373);
            this.texttest.Name = "texttest";
            this.texttest.Size = new System.Drawing.Size(100, 100);
            this.texttest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.texttest.TabIndex = 4;
            this.texttest.TabStop = false;
            // 
            // texttest2
            // 
            this.texttest2.Location = new System.Drawing.Point(1107, 479);
            this.texttest2.Name = "texttest2";
            this.texttest2.Size = new System.Drawing.Size(155, 22);
            this.texttest2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.texttest2.TabIndex = 4;
            this.texttest2.TabStop = false;
            // 
            // testdrawbox
            // 
            this.testdrawbox.BackColor = System.Drawing.Color.Black;
            this.testdrawbox.Location = new System.Drawing.Point(13, 9);
            this.testdrawbox.Margin = new System.Windows.Forms.Padding(0);
            this.testdrawbox.Name = "testdrawbox";
            this.testdrawbox.Size = new System.Drawing.Size(1091, 739);
            this.testdrawbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.testdrawbox.TabIndex = 5;
            this.testdrawbox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.testdrawbox);
            this.Controls.Add(this.imageshould);
            this.Controls.Add(this.texttest2);
            this.Controls.Add(this.texttest);
            this.Controls.Add(this.imagesub);
            this.Controls.Add(this.imagefound);
            this.Controls.Add(this.next);
            this.Controls.Add(this.testB);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_closed);
            this.Load += new System.EventHandler(this.MainForm_load);
            ((System.ComponentModel.ISupportInitialize)(this.imagefound)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imagesub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageshould)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.texttest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.texttest2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testdrawbox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button testB;
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.PictureBox imagefound;
        private System.Windows.Forms.PictureBox imagesub;
        private System.Windows.Forms.PictureBox imageshould;
        private System.Windows.Forms.PictureBox texttest;
        private System.Windows.Forms.PictureBox texttest2;
        private System.Windows.Forms.PictureBox testdrawbox;
    }
}

