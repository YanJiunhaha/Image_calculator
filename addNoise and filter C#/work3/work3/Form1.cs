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

namespace work3
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
            //double sum = 0;
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
        private double BoxMuller(Random rand,double mean,double stdDev)
        {
            
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }
        private Bitmap GaussianNoise(Bitmap im)
        {
            Rectangle r1 = new Rectangle(0, 0, im.Width, im.Height);
            BitmapData d1 = im.LockBits(r1, ImageLockMode.ReadOnly, im.PixelFormat);
            IntPtr ptr1 = d1.Scan0;
            byte[] rgb1 = new byte[d1.Stride * im.Height];
            int skip1 = d1.Stride - im.Width;

            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
            im.UnlockBits(d1);

            Bitmap o = new Bitmap(im.Width, im.Height, im.PixelFormat);
            BitmapData d2 = o.LockBits(r1, ImageLockMode.ReadWrite, o.PixelFormat);
            IntPtr ptr2 = d2.Scan0;

            //add noise
            double mean = 0, stdDev = 27;
            Random rand = new Random(); //reuse this if you are generating many
            for (int y = 0, counter = 0; y < im.Height; ++y, counter += skip1)
            {
                for (int x = 0; x < im.Width; ++x, ++counter)
                {
                    double n = Convert.ToDouble(rgb1[counter]) + BoxMuller(rand, mean, stdDev);
                    //if (rand.NextDouble() > 0.5) n += BoxMuller(rand, mean, stdDev);
                    if (n < 0) n = 0;
                    else if (n > 255) n = 255;
                    rgb1[counter] = Convert.ToByte(n);
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgb1, 0, ptr2, rgb1.Length);
            o.Palette = im.Palette;
            o.UnlockBits(d2);
            return o;
        }
        private Bitmap SaltPepperNoise(Bitmap im)
        {
            Rectangle r1 = new Rectangle(0, 0, im.Width, im.Height);
            BitmapData d1 = im.LockBits(r1, ImageLockMode.ReadOnly, im.PixelFormat);

            IntPtr ptr1 = d1.Scan0;

            byte[] rgb1 = new byte[d1.Stride * im.Height];
            int skip1 = d1.Stride - im.Width;

            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
            im.UnlockBits(d1);

            Bitmap o = new Bitmap(im.Width, im.Height, im.PixelFormat);
            BitmapData d2 = o.LockBits(r1, ImageLockMode.ReadWrite, o.PixelFormat);
            IntPtr ptr2 = d2.Scan0;
            //add noise
            Random rand = new Random(); //reuse this if you are generating many
            for (int y = 0, counter = 0; y < im.Height; ++y, counter += skip1)
            {
                for (int x = 0; x < im.Width; ++x, ++counter)
                {
                    if (rand.NextDouble() <0.2)
                    {
                        if (rand.NextDouble() < 0.5)
                        {
                            rgb1[counter] = 255;
                        }
                        else
                        {
                            rgb1[counter] = 0;
                        }
                    }
                    
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgb1, 0, ptr2, rgb1.Length);
            o.Palette = im.Palette;
            o.UnlockBits(d2);
            return o;
        }
        Bitmap imageRes,image1,image2;

        private void button3_Click(object sender, EventArgs e)
        {
            Rectangle r1 = new Rectangle(0, 0, image1.Width, image1.Height);
            Rectangle r2 = new Rectangle(0, 0, image2.Width, image2.Height);
            BitmapData d1 = image1.LockBits(r1, ImageLockMode.ReadOnly, image1.PixelFormat);
            BitmapData d2 = image2.LockBits(r2, ImageLockMode.ReadOnly, image2.PixelFormat);
            IntPtr ptr1 = d1.Scan0;
            IntPtr ptr2 = d2.Scan0;
            byte[] rgb1 = new byte[d1.Stride * image1.Height];
            byte[] rgb2 = new byte[d2.Stride * image2.Height];

            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgb2, 0, rgb2.Length);

            image1.UnlockBits(d1);
            image2.UnlockBits(d2);



            Bitmap o1 = new Bitmap(image1.Width, image1.Height, image1.PixelFormat);
            Bitmap o2 = new Bitmap(image2.Width, image2.Height, image2.PixelFormat);
            Rectangle r3 = new Rectangle(0, 0, o1.Width, o1.Height);
            Rectangle r4 = new Rectangle(0, 0, o2.Width, o2.Height);
            BitmapData d3 = o1.LockBits(r3, ImageLockMode.ReadWrite, o1.PixelFormat);
            BitmapData d4 = o2.LockBits(r4, ImageLockMode.ReadWrite, o2.PixelFormat);
            IntPtr ptr3 = d3.Scan0;
            IntPtr ptr4 = d4.Scan0;
            byte[] rgb3 = new byte[d3.Stride * o1.Height];
            byte[] rgb4 = new byte[d4.Stride * o2.Height];

            for (int y = 1; y < o1.Height - 1; ++y)
            {
                for (int x = 1; x < o1.Width - 1; ++x)
                {
                    byte[] mask = new byte[3*3];
                    for (int i = -1,counter=0; i <= 1; ++i)
                    {
                        for (int j = -1; j <= 1; ++j,++counter)
                        {
                            mask[counter]= rgb1[(y + i) * d1.Stride + (x + j)];
                        }
                    }
                    Array.Sort(mask);
                    rgb3[y * d3.Stride + x] = mask[5];
                }
            }
            for (int y = 1; y < o2.Height - 1; ++y)
            {
                for (int x = 1; x < o2.Width - 1; ++x)
                {
                    byte[] mask = new byte[3 * 3];
                    for (int i = -1, counter = 0; i <= 1; ++i)
                    {
                        for (int j = -1; j <= 1; ++j, ++counter)
                        {
                            mask[counter] = rgb2[(y + i) * d2.Stride + (x + j)];
                        }
                    }
                    Array.Sort(mask);
                    rgb4[y * d4.Stride + x] = mask[5];
                }
            }
            o1.Palette = image1.Palette;
            o2.Palette = image2.Palette;
            System.Runtime.InteropServices.Marshal.Copy(rgb3, 0, ptr3, rgb3.Length);
            System.Runtime.InteropServices.Marshal.Copy(rgb4, 0, ptr4, rgb4.Length);
            o1.UnlockBits(d3);
            o2.UnlockBits(d4);
            pictureBox2.Image = o1;
            pictureBox4.Image = o2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Rectangle r1 = new Rectangle(0, 0, image1.Width, image1.Height);
            Rectangle r2 = new Rectangle(0, 0, image2.Width, image2.Height);
            BitmapData d1 = image1.LockBits(r1, ImageLockMode.ReadOnly, image1.PixelFormat);
            BitmapData d2 = image2.LockBits(r2, ImageLockMode.ReadOnly, image2.PixelFormat);
            IntPtr ptr1 = d1.Scan0;
            IntPtr ptr2 = d2.Scan0;
            byte[] rgb1 = new byte[d1.Stride * image1.Height];
            byte[] rgb2 = new byte[d2.Stride * image2.Height];

            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgb2, 0, rgb2.Length);

            image1.UnlockBits(d1);
            image2.UnlockBits(d2);



            Bitmap o1 = new Bitmap(image1.Width, image1.Height, image1.PixelFormat);
            Bitmap o2 = new Bitmap(image2.Width, image2.Height, image2.PixelFormat);
            Rectangle r3 = new Rectangle(0, 0, o1.Width, o1.Height);
            Rectangle r4 = new Rectangle(0, 0, o2.Width, o2.Height);
            BitmapData d3 = o1.LockBits(r3, ImageLockMode.ReadWrite, o1.PixelFormat);
            BitmapData d4 = o2.LockBits(r4, ImageLockMode.ReadWrite, o2.PixelFormat);
            IntPtr ptr3 = d3.Scan0;
            IntPtr ptr4 = d4.Scan0;
            byte[] rgb3 = new byte[d3.Stride * o1.Height];
            byte[] rgb4 = new byte[d4.Stride * o2.Height];

            for (int y = 1; y < o1.Height-1; ++y)
            {
                for (int x = 1; x < o1.Width-1; ++x)
                {
                    double sum=0;
                    for(int i = -1; i <= 1; ++i)
                    {
                        for(int j = -1; j <= 1; ++j)
                        {
                            sum += rgb1[(y + i) * d1.Stride + (x + j)];
                        }
                    }
                    rgb3[y * d3.Stride + x] = Convert.ToByte(sum / 9);
                }
            }
            for (int y = 1; y < o2.Height - 1; ++y)
            {
                for (int x = 1; x < o2.Width - 1; ++x)
                {
                    double sum = 0;
                    for (int i = -1; i <= 1; ++i)
                    {
                        for (int j = -1; j <= 1; ++j)
                        {
                            sum += rgb2[(y + i) * d2.Stride + (x + j)];
                        }
                    }
                    rgb4[y * d4.Stride + x] = Convert.ToByte(sum / 9);
                }
            }
            o1.Palette = image1.Palette;
            o2.Palette = image2.Palette;
            System.Runtime.InteropServices.Marshal.Copy(rgb3, 0, ptr3, rgb3.Length);
            System.Runtime.InteropServices.Marshal.Copy(rgb4, 0, ptr4, rgb4.Length);
            o1.UnlockBits(d3);
            o2.UnlockBits(d4);
            pictureBox2.Image = o1;
            pictureBox4.Image = o2;
        }

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
                            imageRes = (Bitmap)Bitmap.FromStream(myStream);
                            if (imageRes.PixelFormat == PixelFormat.Format24bppRgb)
                            {
                                imageRes = GetGray(imageRes);
                            }
                            image1 = GaussianNoise(imageRes);
                            pictureBox1.Image = image1;
                            image2 = SaltPepperNoise(imageRes);
                            pictureBox3.Image = image2;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error.Could not read file:" + ex.Message);
                }
            }
        }
    }
}
