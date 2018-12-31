using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace image_room
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Bitmap GetGray(Bitmap im)
        {
            Rectangle r1 = new Rectangle(0, 0, im.Width, im.Height);
            BitmapData d1 = im.LockBits(r1, ImageLockMode.ReadOnly, im.PixelFormat);
            IntPtr ptr1 = d1.Scan0;
            byte[] rgb1 = new byte[d1.Stride * im.Height];
            int skip1 = d1.Stride - im.Width * 3;
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
            im.UnlockBits(d1);

            Bitmap g = new Bitmap(im.Width, im.Height, PixelFormat.Format8bppIndexed);
            Rectangle r2 = new Rectangle(0, 0, g.Width, g.Height);
            BitmapData d2 = g.LockBits(r2, ImageLockMode.ReadWrite, g.PixelFormat);
            IntPtr ptr2 = d2.Scan0;
            byte[] rgb2 = new byte[d2.Stride * g.Height];
            int skip2 = d2.Stride - g.Width;

            for (int y = 0, counter1 = 0, counter2 = 0; y < g.Height; ++y, counter1 += skip1, counter2 += skip2)
                for (int x = 0; x < g.Width; ++x, counter1 += 3, counter2 += 1)
                    rgb2[counter2] = Convert.ToByte(0.299 * (double)rgb1[counter1 + 2] + 0.587 * (double)rgb1[counter1 + 1] + 0.114 * (double)rgb1[counter1]);
            System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);

            ColorPalette myPalette = g.Palette;
            for (int i = 0; i < 256; ++i) myPalette.Entries[i] = Color.FromArgb(i, i, i);
            g.Palette = myPalette;
            g.UnlockBits(d2);
            return g;
        }
        
        Bitmap ImageRes, Image1;

        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "所有檔案(*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if((myStream = openFileDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            ImageRes = (Bitmap)Bitmap.FromStream(myStream);
                            if(ImageRes.PixelFormat != PixelFormat.Format8bppIndexed)
                            {
                                Image1 = GetGray(ImageRes);
                            }
                            else
                            {
                                Image1 = ImageRes;
                            }
                            pictureBox1.Image = Image1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Can't open file" + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int position_x = Convert.ToInt32(textBox1.Text);
                int position_y = Convert.ToInt32(textBox2.Text);
                int height = Convert.ToInt32(textBox3.Text);
                if (height > (Image1.Height - position_y)) { height = Image1.Height - position_y; MessageBox.Show("height too bigger"); }
                int width = Convert.ToInt32(textBox4.Text);
                if(width > (Image1.Width - position_x)) { width = Image1.Width - position_x; MessageBox.Show("width too bigger"); }
                double ratio = Convert.ToDouble(textBox5.Text);

                Rectangle r1 = new Rectangle(0, 0, Image1.Width, Image1.Height);
                BitmapData d1 = Image1.LockBits(r1, ImageLockMode.ReadOnly, Image1.PixelFormat);
                IntPtr ptr1 = d1.Scan0;
                byte[] rgb1 = new byte[d1.Stride * Image1.Height];
                int skip1 = d1.Stride - Image1.Width;
                System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
                Image1.UnlockBits(d1);

                Bitmap s = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                Rectangle r2 = new Rectangle(0, 0, s.Width, s.Height);
                BitmapData d2 = s.LockBits(r2, ImageLockMode.ReadWrite, s.PixelFormat);
                IntPtr ptr2 = d2.Scan0;
                byte[] rgb2 = new byte[d2.Stride * s.Height];
                int skip2 = d2.Stride - s.Width;
                
                for(int y = 0,counter = 0; y < s.Height; ++y, counter+=skip2)
                {
                    for(int x = 0; x < s.Width; ++x, ++counter)
                    {
                        rgb2[counter] = rgb1[(position_y + y) * d1.Stride + position_x + x];
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);
                ColorPalette myPalette = s.Palette;
                for (int i = 0; i < 256; ++i) myPalette.Entries[i] = Color.FromArgb(i, i, i);
                s.Palette = myPalette;
                s.UnlockBits(d2);
                pictureBox2.Image = s;

                //frist order
                Bitmap first_img = new Bitmap(Convert.ToInt32(s.Width * ratio),Convert.ToInt32(s.Height*ratio),s.PixelFormat);
                Rectangle r3 = new Rectangle(0, 0, first_img.Width, first_img.Height);
                BitmapData d3 = first_img.LockBits(r3, ImageLockMode.ReadWrite, first_img.PixelFormat);
                IntPtr ptr3 = d3.Scan0;
                byte[] rgb3 = new byte[d3.Stride * first_img.Height];
                int skip3 = d3.Stride - first_img.Width;

                for (int y = 0, counter = 0; y < first_img.Height; ++y,counter += skip3)
                {
                    for(int x = 0; x<first_img.Width; ++x, ++counter)
                    {
                        int h = Convert.ToInt32(y / ratio);
                        int w = Convert.ToInt32(x / ratio);
                        int r = Convert.ToInt32(y % ratio);
                        if (h<(s.Height-1) && w<(s.Width-1))
                        {
                            rgb3[counter] = Convert.ToByte((rgb2[h*d2.Stride+w] * (ratio - r) + rgb2[h * d2.Stride + w +1] * r) / ratio);
                        }
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgb3, 0, ptr3, rgb3.Length);
                first_img.Palette = myPalette;
                first_img.UnlockBits(d3);
                pictureBox3.Image = first_img;

                //bilinear
                Bitmap bi_img = new Bitmap(Convert.ToInt32(s.Width * ratio), Convert.ToInt32(s.Height * ratio), s.PixelFormat);
                Rectangle r4 = new Rectangle(0, 0, bi_img.Width, bi_img.Height);
                BitmapData d4 = bi_img.LockBits(r4, ImageLockMode.ReadWrite, bi_img.PixelFormat);
                IntPtr ptr4 = d4.Scan0;
                byte[] rgb4 = new byte[d4.Stride * bi_img.Height];
                int skip4 = d4.Stride - bi_img.Width;

                for (int y = 0, counter = 0; y < bi_img.Height; ++y, counter += skip4)
                {
                    for (int x = 0; x < bi_img.Width; ++x, ++counter)
                    {
                        double fh = y / ratio;
                        int h = Convert.ToInt32(fh);
                        double fw = x / ratio;
                        int w = Convert.ToInt32(fw);
                        if (h < (s.Height - 1) && w < (s.Width - 1))
                        {
                            double q11 = rgb2[h*d2.Stride+w];
                            double q21 = rgb2[h * d2.Stride + w + 1];
                            double q12 = rgb2[(h + 1) * d2.Stride + w];
                            double q22 = rgb2[(h + 1) * d2.Stride + w + 1];

                            double R1 = q11 * (w + 1 - fw) + q21 * (fw - w);
                            double R2 = q12 * (w + 1 - fw) + q22 * (fw - w);
                            double n = R1 * (h + 1 - fh) + R2 * (fh - h);
                            if (n > 255) n = 255;
                            else if (n<0)n = 0;
                            rgb4[counter] = Convert.ToByte(n);

                        }
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgb4, 0, ptr4, rgb4.Length);
                bi_img.Palette = myPalette;
                bi_img.UnlockBits(d4);
                pictureBox4.Image = bi_img;

            }
            catch(Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
        }
    }
}
