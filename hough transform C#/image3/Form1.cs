using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace image3
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

        private Bitmap Get24bits(Bitmap im)
        {
            Rectangle r1 = new Rectangle(0, 0, im.Width, im.Height);
            BitmapData d1 = im.LockBits(r1, ImageLockMode.ReadOnly, im.PixelFormat);
            IntPtr ptr1 = d1.Scan0;
            byte[] rgb1 = new byte[d1.Stride * im.Height];
            int skip1 = d1.Stride - im.Width;
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
            im.UnlockBits(d1);

            Bitmap o = new Bitmap(im.Width, im.Height, PixelFormat.Format24bppRgb);
            Rectangle r2 = new Rectangle(0, 0, o.Width, o.Height);
            BitmapData d2 = o.LockBits(r2, ImageLockMode.ReadWrite, o.PixelFormat);
            IntPtr ptr2 = d2.Scan0;
            byte[] rgb2 = new byte[d2.Stride * o.Height];
            int skip2 = d2.Stride - o.Width*3;
            
            for(int y = 0, counter1 = 0, counter2 = 0; y < im.Height; ++y, counter1 += skip1, counter2 += skip2)
            {
                for(int x = 0; x < im.Width; ++x, counter1 += 1, counter2 += 3)
                {
                    rgb2[counter2] = rgb2[counter2 + 1] = rgb2[counter2 + 2] = rgb1[counter1];
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);
            o.UnlockBits(d2);
            
            return o;
        }

        Bitmap imageRes, image1;
        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "所有檔案(*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            imageRes = (Bitmap)Bitmap.FromStream(myStream);
                            if (imageRes.PixelFormat != PixelFormat.Format8bppIndexed)
                            {
                                image1 = GetGray(imageRes);
                            } else
                            {
                                image1 = imageRes;
                                imageRes = Get24bits(image1);
                            }
                            pictureBox1.Image = imageRes;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error.Could not read file:" + ex.Message);
                }
            }
        }
        
        private Bitmap Convolve(Bitmap im, int[] mask, int maskSize,int threshold=0)
        {
             Rectangle r1 = new Rectangle(0, 0, im.Width, im.Height);
             BitmapData d1 = im.LockBits(r1, ImageLockMode.ReadOnly, im.PixelFormat);
             IntPtr ptr1 = d1.Scan0;
             byte[] rgb1 = new byte[d1.Stride * im.Height];
             int skip = d1.Stride - im.Width;
             System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
             im.UnlockBits(d1);

             Bitmap o = new Bitmap(im.Width, im.Height, PixelFormat.Format8bppIndexed);
             Rectangle r2 = new Rectangle(0, 0, o.Width, o.Height);
             BitmapData d2 = o.LockBits(r2, ImageLockMode.ReadWrite, o.PixelFormat);
             IntPtr ptr2 = d2.Scan0;
             byte[] rgb2 = new byte[d2.Stride * o.Height];

             for (int y = 0, counter = 0; y < o.Height; ++y, counter += skip)
             {
                 for (int x = 0; x < o.Width; ++x, ++counter)
                 {
                     double sum = 0;
                     if (x > maskSize / 2 && x < (o.Width - maskSize / 2) && y > maskSize / 2 && y < (o.Height - maskSize / 2))
                     {

                         for (int i = -maskSize / 2; i <= maskSize / 2; ++i)
                         {
                             for (int j = -maskSize / 2; j <= maskSize / 2; ++j)
                             {
                                 sum += rgb1[counter + i * d2.Stride + j] * mask[maskSize * (i + maskSize / 2) + j + maskSize / 2];
                             }
                         }
                         sum = (sum < 0) ? 0 - sum : sum;
                         sum = (sum > 255) ? 255 : sum;
                         sum = (sum>=threshold)?sum: 0;
                         rgb2[counter] = Convert.ToByte(sum);
                     }
                 }
             }
             System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);
             ColorPalette myPalette = o.Palette;
             for (int i = 0; i < 256; ++i) myPalette.Entries[i] = Color.FromArgb(i, i, i);
             o.Palette = myPalette;
             o.UnlockBits(d2);
             return o;
        }

        private Bitmap Add(Bitmap ima,Bitmap imb)
        {
            Rectangle r1 = new Rectangle(0, 0, ima.Width, ima.Height);
            BitmapData d1 = ima.LockBits(r1, ImageLockMode.ReadOnly, ima.PixelFormat);
            IntPtr ptr1 = d1.Scan0;
            byte[] rgb1 = new byte[d1.Stride * ima.Height];
            int skip = d1.Stride - ima.Width;
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
            ima.UnlockBits(d1);

            Rectangle r2 = new Rectangle(0, 0, imb.Width, imb.Height);
            BitmapData d2 = imb.LockBits(r2, ImageLockMode.ReadOnly, imb.PixelFormat);
            IntPtr ptr2 = d2.Scan0;
            byte[] rgb2 = new byte[d2.Stride * imb.Height];
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgb2, 0, rgb2.Length);
            imb.UnlockBits(d2);

            Bitmap o = new Bitmap(ima.Width, ima.Height, PixelFormat.Format8bppIndexed);
            Rectangle r3 = new Rectangle(0, 0, o.Width, o.Height);
            BitmapData d3 = o.LockBits(r3, ImageLockMode.ReadWrite, o.PixelFormat);
            IntPtr ptr3 = d3.Scan0;
            byte[] rgb3 = new byte[d3.Stride * o.Height];


            for(int y = 0, counter = 0; y < o.Height; ++y, counter += skip) 
                for(int x = 0,sum=0; x < o.Width; ++x, ++counter)
                {
                    sum = rgb1[counter] + rgb2[counter];
                    sum = (sum < 0) ? 0 : sum;
                    sum = (sum > 255) ? 255 : sum;
                    rgb3[counter] = Convert.ToByte(sum);
                }
            System.Runtime.InteropServices.Marshal.Copy(rgb3, 0, ptr3, rgb3.Length);
            ColorPalette myPalette = o.Palette;
            for (int i = 0; i < 256; ++i) myPalette.Entries[i] = Color.FromArgb(i, i, i);
            o.Palette = myPalette;
            o.UnlockBits(d3);
            return o;
        }

        private Bitmap Hough(Bitmap im,int threshold) {
            Rectangle r1 = new Rectangle(0, 0, im.Width, im.Height);
            BitmapData d1 = im.LockBits(r1, ImageLockMode.ReadOnly, im.PixelFormat);
            IntPtr ptr1 = d1.Scan0;
            byte[] rgb1 = new byte[d1.Stride * im.Height];
            int skip1 = d1.Stride - im.Width;
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
            im.UnlockBits(d1);

            double D = Math.Sqrt(Math.Pow(im.Width, 2) + Math.Pow(im.Height, 2));
            int iD = ((int)D < D) ? (int)D + 1:(int)D; 
            Bitmap o = new Bitmap(180,iD*2,PixelFormat.Format8bppIndexed);
            ColorPalette myPalette = o.Palette;
            for (int i = 0; i < 256; ++i) myPalette.Entries[i]=Color.FromArgb(i,i,i);
            o.Palette = myPalette;

            Rectangle r2 = new Rectangle(0, 0, o.Width, o.Height);
            BitmapData d2 = o.LockBits(r2, ImageLockMode.ReadWrite, o.PixelFormat);
            IntPtr ptr2 = d2.Scan0;
            byte[] rgb2 = new byte[d2.Stride * o.Height];
            int skip2 = d2.Stride - o.Width;
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgb2, 0, rgb2.Length);

            int[] Irgb2 = new int[d2.Stride * o.Height];
             
            for (int y = 0, counter = 0; y < im.Height; ++y, counter += skip1)
            {
                for(int x = 0; x < im.Width; ++x, ++counter)
                {
                    if (rgb1[counter] > 0)
                    {
                        for(double theta = -90; theta < 90; ++theta)
                        {
                            ++Irgb2[Convert.ToInt32(x * Math.Cos(theta / 180 * Math.PI) + y * Math.Sin(theta / 180 * Math.PI)+iD) * d2.Stride + (90 + Convert.ToInt32(theta))];
                        }
                    }
                }
            }
            int max = 0, min = 255;
            foreach(int x in Irgb2)
            {
                if (x > max) max = x;
                if (x < min) min = x;
            }
            if (max > 255) {
                double ratio =255.0/(max-min);
                for (int y = 0, counter = 0; y < o.Height; ++y, counter += skip2)
                {
                    for(int x = 0; x < o.Width; ++x, ++counter)
                    {
                        Irgb2[counter] = Convert.ToInt32((Irgb2[counter] - min) * ratio);
                    }
                }
            }

            for (int i = 0; i < Irgb2.Length; ++i) rgb2[i] = Convert.ToByte(Irgb2[i]);
            System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);
            o.UnlockBits(d2);

            Rectangle r3 = new Rectangle(0, 0, imageRes.Width, imageRes.Height);
            BitmapData d3 = imageRes.LockBits(r3, ImageLockMode.ReadWrite, imageRes.PixelFormat);
            IntPtr ptr3 = d3.Scan0;
            byte[] rgb3 = new byte[d3.Stride * imageRes.Height];
            int skip3 = d3.Stride - imageRes.Width * 3;
            System.Runtime.InteropServices.Marshal.Copy(ptr3, rgb3, 0, rgb3.Length);

            for (int y = 0, counter = 0; y < o.Height; ++y, counter += skip2)
            {
                for (int x = 0; x < o.Width; ++x, counter += 1)
                {
                    if (Irgb2[counter] > threshold)
                    {
                        for (int i = 0, j; i < imageRes.Width; ++i)
                        {
                            double temp = ((Convert.ToDouble(y) - iD - Convert.ToDouble(i) * Math.Cos(Convert.ToDouble(x - 90) / 180 * Math.PI)) / Math.Sin(Convert.ToDouble(x - 90) / 180 * Math.PI));
                            if (temp > 0 && temp < imageRes.Height - 1)
                            {
                                j = Convert.ToInt32(temp);

                                rgb3[j * d3.Stride + i * 3] = rgb3[j * d3.Stride + i * 3 + 1] = 0;
                                rgb3[j * d3.Stride + i * 3 + 2] = 255;
                            }
                        }
                        for (int j = 0, i; j < imageRes.Height; ++j)
                        {
                            double temp = ((Convert.ToDouble(y) - iD - Convert.ToDouble(j) * Math.Sin(Convert.ToDouble(x - 90) / 180 * Math.PI)) / Math.Cos(Convert.ToDouble(x - 90) / 180 * Math.PI));
                            if (temp > 0 && temp < imageRes.Height - 1)
                            {
                                i = Convert.ToInt32(temp);

                                rgb3[j * d3.Stride + i * 3] = rgb3[j * d3.Stride + i * 3 + 1] = 0;
                                rgb3[j * d3.Stride + i * 3 + 2] = 255;
                            }
                        }
                        //for(int i = 0; i < imageRes.Height; ++i)
                        //{
                        //    for (int j = 0; j < imageRes.Width; ++j)
                        //    {
                        //        if (y == (Math.Cos(Math.PI * (x - 90) / 180)) * j + Math.Sin(Math.PI * (y - 90) / 180) * i)
                        //        {
                        //            rgb3[i * d3.Stride + j * 3] = rgb3[i * d3.Stride + j * 3 + 1] = 0;
                        //            rgb3[i * d3.Stride + j * 3 + 2] = 255;
                        //        }
                        //    }
                        //}

                    }
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(rgb3, 0, ptr3, rgb3.Length);
            imageRes.UnlockBits(d3);

            return o;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try {
                int threshold=Convert.ToInt32(textBox1.Text);
                int lineThr = Convert.ToInt32(textBox2.Text);

                if (threshold < 0) { MessageBox.Show("Threshold need bigger than 0");return; }
                else if (threshold > 255) { MessageBox.Show("Threshold need smaller than 255");return; }
                
                int[] maskH = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
                int[] maskV = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
                int[] maskR = { 0, 1, 1, -1, 0, 1, -1, -1, 0 };
                int[] maskL = { -1, -1, 0, -1, 0, 1, 0, 1, 1 };
                int maskSize = 3;
                Bitmap sobelV = Convolve(image1, maskV, maskSize,threshold);
                Bitmap sobelH = Convolve(image1, maskH, maskSize,threshold);
                Bitmap sobel90 = Add(sobelV, sobelH);

                Bitmap sobelR = Convolve(image1, maskR, maskSize,threshold);
                Bitmap sobelL = Convolve(image1, maskL, maskSize,threshold);
                Bitmap sobel45 = Add(sobelR, sobelL);
                Bitmap sobel = Add(sobel45, sobel90);
                pictureBox3.Image = sobel;

                Bitmap test = Hough(sobel,lineThr);
                pictureBox4.Image = test;
                pictureBox2.Image = imageRes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
        }
    }
}
