using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace image1
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
            int skip1 = d1.Stride - im.Width*3;
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);

            Bitmap im_gray = new Bitmap(im.Width, im.Height, PixelFormat.Format8bppIndexed);
            Rectangle r2 = new Rectangle(0, 0, im_gray.Width, im_gray.Height);
            BitmapData d2 = im_gray.LockBits(r2, ImageLockMode.ReadWrite, im_gray.PixelFormat);
            IntPtr ptr2 = d2.Scan0;
            byte[] rgb2 = new byte[d2.Stride * im_gray.Height];
            int skip2 = d2.Stride - im_gray.Width;

            int counter1 = 0,counter2=0;
            double sum = 0;
            for(int y=0; y < im_gray.Height; ++y)
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
                    counter1 += 3;counter2 += 1;
                }
                counter1 += skip1;counter2 += skip2;
            }
            System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);

            ColorPalette myPaletee = im_gray.Palette ;
            //using (Bitmap temp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed)) myPaletee = temp.Palette;
            for (int i = 0; i < 256; ++i) myPaletee.Entries[i] = Color.FromArgb(i, i, i);
            im_gray.Palette = myPaletee;
            im.UnlockBits(d1);
            im_gray.UnlockBits(d2);
            return im_gray;
        }
        private Bitmap DrawHistogram(byte[] data)
        {
            int[] cnt = new int[256];
            foreach (byte element in data)
                cnt[element]++;
            int max = 0;
            foreach (int element in cnt)
                if (max < element) max = element;
            Bitmap his = new Bitmap(256, 192, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(his);

            g.Clear(Color.White);

            Pen curPen =new Pen(Brushes.Black, 1);

            for(int i = 0; i < 256; ++i)
            {
                g.DrawLine(curPen, i, 192, i, 192 - 192 * cnt[i] / max);
            }

            g.Dispose();
            return his;
        }
        Bitmap image1;
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
            Rectangle rect1 = new Rectangle(0, 0, image1.Width, image1.Height);
            BitmapData data1 = image1.LockBits(rect1, ImageLockMode.ReadWrite, image1.PixelFormat);

            Bitmap image2 = new Bitmap(image1.Width, image1.Height, image1.PixelFormat);
            Rectangle rect2=new Rectangle(0, 0, image2.Width, image2.Height);
            BitmapData data2 = image2.LockBits(rect2, ImageLockMode.ReadWrite, image2.PixelFormat);

            IntPtr ptr1 = data1.Scan0;
            IntPtr ptr2 = data2.Scan0;

            byte[] rgb1 = new byte[data1.Stride * image1.Height];
            byte[] rgb2 = new byte[data2.Stride * image2.Height];
            
            int skip1 = data1.Stride - image1.Width;
            int skip2 = data2.Stride - image2.Width;

            System.Runtime.InteropServices.Marshal.Copy(ptr1,rgb1,0,rgb1.Length);

            int counter = 0;
            int[] probability = new int[256];
            for(int y = 0; y < image1.Height; ++y)
            {
                for(int x = 0; x < image1.Width; ++x)
                {
                    probability[rgb1[counter]]++;
                    counter += 1;
                }
                counter += skip1;
            }

            int[] acc = new int[256];
            for(int i = 0; i < probability.Length; ++i)
            {
                int sum = 0;
                for(int j = 0; j < i; ++j)
                {
                    sum += probability[j];
                }
                acc[i] = sum*255/(image1.Height*image1.Width);
            }

            counter = 0;
            for(int y = 0; y < image2.Height; ++y)
            {
                for(int x = 0; x < image2.Width; ++x)
                {
                    rgb2[counter] = (byte)acc[rgb1[counter]];
                    counter += 1;
                }
                counter += skip2;
            }
            
            ColorPalette myPalette = image1.Palette;
            image2.Palette = myPalette;

            System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);

            pictureBox3.Image = DrawHistogram(rgb1);
            pictureBox4.Image = DrawHistogram(rgb2);

            image1.UnlockBits(data1);
            image2.UnlockBits(data2);
            pictureBox2.Image = image2;
        }

    }
}
