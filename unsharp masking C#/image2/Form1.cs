using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace image2
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

            Bitmap im_gray = new Bitmap(im.Width, im.Height, PixelFormat.Format8bppIndexed);
            Rectangle r2 = new Rectangle(0, 0, im_gray.Width, im_gray.Height);
            BitmapData d2 = im_gray.LockBits(r2, ImageLockMode.ReadWrite, im_gray.PixelFormat);
            IntPtr ptr2 = d2.Scan0;
            byte[] rgb2 = new byte[d2.Stride * im_gray.Height];
            int skip2 = d2.Stride - im_gray.Width;

            int counter1 = 0, counter2 = 0;
            for (int y = 0; y < im_gray.Height; ++y)
            {
                for (int x = 0; x < im_gray.Width; ++x)
                {
                    try
                    {
                        rgb2[counter2] = Convert.ToByte(0.299 * (double)rgb1[counter1 + 2] + 0.587 * (double)rgb1[counter1 + 1] + 0.114 * (double)rgb1[counter1]);
                    }
                    catch
                    {
                        MessageBox.Show("cnt1:" + counter1.ToString() + "cnt2:" + counter2.ToString());
                    }
                    counter1 += 3; counter2 += 1;
                }
                counter1 += skip1; counter2 += skip2;
            }
            System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);

            ColorPalette myPaletee = im_gray.Palette;
            //using (Bitmap temp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed)) myPaletee = temp.Palette;
            for (int i = 0; i < 256; ++i) myPaletee.Entries[i] = Color.FromArgb(i, i, i);
            im_gray.Palette = myPaletee;
            im.UnlockBits(d1);
            im_gray.UnlockBits(d2);
            return im_gray;
        }

        private void GetMS(Bitmap im,ref int max,ref int min)
        {
            Rectangle r = new Rectangle(0, 0, im.Width, im.Height);
            BitmapData d = im.LockBits(r, ImageLockMode.ReadOnly, im.PixelFormat);
            IntPtr ptr = d.Scan0;
            byte[] rgb = new byte[d.Stride * im.Height];
            int skip = d.Stride - im.Width;
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgb, 0, rgb.Length);
            
            max = 0; min = 255;
            int counter = 0;
            for (int y = 0; y < im.Height; ++y)
            {
                for(int x = 0; x < im.Width; ++x)
                {
                    if (max < rgb[counter]) max = rgb[counter];
                    if (min > rgb[counter]) min = rgb[counter];
                    counter+=1;
                }counter += skip;
            }
            im.UnlockBits(d);
        }

        Bitmap image1;
        int max = 0, min = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "";
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
                            image1 = (Bitmap)Bitmap.FromStream(myStream);
                            if (image1.PixelFormat == PixelFormat.Format24bppRgb)
                                image1 = GetGray(image1);
                            GetMS(image1,ref max,ref min);
                            textBox2.Text = min.ToString();
                            textBox3.Text = max.ToString();
                            pictureBox1.Image = image1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error.Could not read file:" + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int maskSize = Convert.ToInt32(textBox1.Text);
                int histA = Convert.ToInt32(textBox2.Text);
                int histB = Convert.ToInt32(textBox3.Text);

                if (maskSize % 2 == 0) { MessageBox.Show("mask size has been odd.");return; }
                if (histA < min) { MessageBox.Show("A need bigger than "+min.ToString()+"."); return; }
                if (histB > max) { MessageBox.Show("B need smaller than " + max.ToString() + "."); return; }

                Rectangle r1 = new Rectangle(0, 0, image1.Width, image1.Height);
                BitmapData d1 = image1.LockBits(r1, ImageLockMode.ReadOnly, image1.PixelFormat);
                IntPtr ptr1 = d1.Scan0;
                byte[] rgb1 = new byte[d1.Stride * image1.Height];
                int skip1 = d1.Stride - image1.Width;
                System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);

                Bitmap image2 = new Bitmap(image1.Width, image1.Height, image1.PixelFormat);
                Rectangle r2 = new Rectangle(0, 0, image2.Width, image2.Height);
                BitmapData d2 = image2.LockBits(r2, ImageLockMode.ReadWrite, image2.PixelFormat);
                IntPtr ptr2 = d2.Scan0;
                byte[] rgb2 = new byte[d2.Stride * image2.Height];
                int skip2 = d2.Stride - image2.Width;

                ColorPalette myPalette = image1.Palette;
                image2.Palette = myPalette;


                for(int y = 0,counter=0; y < image1.Height; ++y,counter+=skip1)
                {
                    for(int x = 0; x < image1.Width; ++x,counter+=1)
                    {
                        if(x>maskSize/2 && x<(image1.Width-maskSize/2) && y>maskSize/2 && y < (image1.Height - maskSize / 2))
                        {
                            int sum=0;
                            for(int i = -maskSize / 2; i <= maskSize / 2; ++i)
                            {
                                for(int j = -maskSize / 2; j <= maskSize / 2; ++j)
                                {
                                    sum +=Convert.ToInt32( rgb1[i * d1.Stride + j + counter]);
                                }
                            }
                            rgb2[counter] = Convert.ToByte(sum / 9);
                        }
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);
                pictureBox3.Image = image2;

                Bitmap image3 = new Bitmap(image1.Width, image1.Height, image1.PixelFormat);
                Rectangle r3 = new Rectangle(0, 0, image3.Width, image3.Height);
                BitmapData d3 = image3.LockBits(r3, ImageLockMode.ReadWrite, image3.PixelFormat);
                IntPtr ptr3 = d3.Scan0;
                image3.Palette = myPalette;

                int max2 = 0, min2 = 255;
                for (int y = 0, counter = 0; y < image2.Height; ++y, counter += skip2)
                {
                    for (int x = 0; x < image2.Width; ++x, counter += 1)
                    {
                        if (max2 < rgb2[counter]) max2 = rgb2[counter];
                        if (min2 > rgb2[counter]) min2 = rgb2[counter];
                    }
                }
                for (int y = 0, counter = 0; y < image2.Height; ++y, counter += skip2)
                {
                    for (int x = 0; x < image2.Width; ++x, counter += 1)
                    {
                        rgb2[counter] = (byte)((histB - histA) * (int)(rgb2[counter]-min2) / (max2 - min2)+histA);
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr3, rgb2.Length);
                pictureBox4.Image = image3;

                Bitmap image4 = new Bitmap(image1.Width, image1.Height, image1.PixelFormat);
                Rectangle r4 = new Rectangle(0, 0, image4.Width, image4.Height);
                BitmapData d4 = image4.LockBits(r4, ImageLockMode.ReadWrite, image4.PixelFormat);
                IntPtr ptr4 = d4.Scan0;
                image4.Palette = myPalette;

                int[] acc = new int[256];

                for (int y = 0, counter = 0; y < image2.Height; ++y, counter += skip2)
                {
                    for (int x = 0; x < image2.Width; ++x, counter += 1)
                    {
                        int ans = rgb1[counter] - rgb2[counter];
                        rgb2[counter] = (byte)(ans < 0 ? 0 : ans);
                        ++acc[rgb2[counter]];
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr4, rgb2.Length);
                pictureBox5.Image = image4;

                Bitmap image5 = new Bitmap(image1.Width, image1.Height, image1.PixelFormat);
                Rectangle r5 = new Rectangle(0, 0, image5.Width, image5.Height);
                BitmapData d5 = image5.LockBits(r5, ImageLockMode.ReadWrite, image5.PixelFormat);
                IntPtr ptr5 = d5.Scan0;
                image5.Palette = myPalette;
                int sumArr=acc[0];
                byte[] Index = new byte[256];
                Index[0] = (byte)(255 * sumArr / (image4.Height * image4.Width));
                for(int i = 1; i < acc.Length; ++i)
                {
                    sumArr += acc[i];
                    Index[i] = (byte)(255 * sumArr / (image4.Height * image4.Width));
                }

                for (int y = 0, counter = 0; y < image5.Height; ++y, counter += skip2)
                {
                    for (int x = 0; x < image5.Width; ++x, counter += 1)
                    {
                        rgb2[counter] = Index[rgb2[counter]];
                    }
                }
                System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr5, rgb2.Length);
                pictureBox6.Image = image5;
                pictureBox2.Image = image5;

                image1.UnlockBits(d1);
                image2.UnlockBits(d2);
                image3.UnlockBits(d3);
                image4.UnlockBits(d4);
                image5.UnlockBits(d5);
            }
            catch (Exception ex) { MessageBox.Show("Error:" + ex.Message); }

        }
    }
}
