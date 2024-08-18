using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SimpleAppForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //this.transparentPanel.BackColor = Color.FromArgb(128, Color.White);
            //this.transparentPanel.BackgroundImage = null;

            //this.Controls.Remove(this.fontButton1);
            //this.transparentPanel.Controls.Add(this.fontButton1);
            //this.fontButton1.Location = new System.Drawing.Point(15, 202);
            //this.fontButton1.BringToFront();

            // フォームの設定
            this.Text = "Sample WinForms App";
            this.Size = new Size(800, 600);

            // 背景画像の設定
            

            // パネル上にボタンを3つ追加
            for (int i = 0; i < 3; i++)
            {
                Button button = new Button();
                button.Text = $"Button {i + 1}";
                button.Size = new Size(80, 30);
                button.Location = new Point(30, 30 + i * 40);
                transparentPanel.Controls.Add(button);
            }
        }


        private void panelButton_Click(object sender, EventArgs e)
        {

        }
    }

    // カスタム透過パネルクラス
    public class TransparentPanel : Panel
    {
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.Parent != null)
            {
                // 親の背景を描画
                var parentGraphics = e.Graphics;
                var parentRectangle = new Rectangle(this.Left, this.Top, this.Width, this.Height);

                // 親のクライアント領域を描画
                parentGraphics.TranslateTransform(-this.Left, -this.Top);
                var pe = new PaintEventArgs(parentGraphics, parentRectangle);
                this.InvokePaintBackground(this.Parent, pe);
                this.InvokePaint(this.Parent, pe);
                parentGraphics.TranslateTransform(this.Left, this.Top);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.BackgroundImage != null)
            {
                // 画像を半透明で描画
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    ColorMatrix matrix = new ColorMatrix
                    {
                        Matrix33 = 0.5f // 透明度を設定 (0.0f = 完全に透明, 1.0f = 不透明)
                    };
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    e.Graphics.DrawImage(
                        this.BackgroundImage,
                        new Rectangle(0, 0, this.Width, this.Height),
                        0, 0, this.BackgroundImage.Width, this.BackgroundImage.Height,
                        GraphicsUnit.Pixel,
                        attributes);
                }
            }
        }
    }
}

