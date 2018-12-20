using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace image4
{
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        enum yan : int
        {
            EROSION = 0,
            DILATION
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
        private Bitmap GetBin(Bitmap im)
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

            for (int i = 0; i < rgb1.Length; ++i)
                rgb2[i] = Convert.ToByte((rgb1[i] >= 127) ? 255 : 0);
            System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);
            ColorPalette myPalette = g.Palette;
            for (int i = 0; i < 256; ++i) myPalette.Entries[i] = Color.FromArgb(i, i, i);
            g.Palette = myPalette;
            g.UnlockBits(d2);
            return g;
        }

        Bitmap image1;
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
                            image1 = (Bitmap)Bitmap.FromStream(myStream);
                            if (image1.PixelFormat != PixelFormat.Format8bppIndexed)
                            {
                                image1 = GetGray(image1);
                            }
                            image1 = GetBin(image1);
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
        private Bitmap Con(Bitmap im, int maskSize, yan flag)//flag=1:Dilation  0:Erostion
        {
            Rectangle r1 = new Rectangle(0, 0, im.Width, im.Height);
            BitmapData d1 = im.LockBits(r1, ImageLockMode.ReadOnly, im.PixelFormat);
            IntPtr ptr1 = d1.Scan0;
            byte[] rgb1 = new byte[d1.Stride * im.Height];
            int skip1 = d1.Stride - im.Width;
            System.Runtime.InteropServices.Marshal.Copy(ptr1, rgb1, 0, rgb1.Length);
            im.UnlockBits(d1);

            Bitmap g = new Bitmap(im.Width, im.Height, PixelFormat.Format8bppIndexed);
            Rectangle r2 = new Rectangle(0, 0, g.Width, g.Height);
            BitmapData d2 = g.LockBits(r2, ImageLockMode.ReadWrite, g.PixelFormat);
            IntPtr ptr2 = d2.Scan0;
            byte[] rgb2 = new byte[d2.Stride * g.Height];
            int skip2 = d2.Stride - g.Width;
            bool exist = false;
            for (int y = 0, counter = 0; y < im.Height; ++y, counter += skip1)
                for (int x = 0; x < im.Width; ++x, counter += 1)
                {
                    exist = false;
                    for (int i = -maskSize / 2; i <= maskSize / 2; ++i)
                        for (int j = -maskSize / 2; j <= maskSize / 2; ++j)
                        {
                            int temp = counter + i * d1.Stride + j;
                            if (temp >= 0 && temp < rgb1.Length)
                            {
                                if (flag==yan.DILATION)
                                {
                                    if (rgb1[temp] == 255) exist = true;
                                }
                                else
                                {
                                    if (rgb1[temp] == 0) exist = true;
                                }
                            }
                        }
                    if (flag==yan.DILATION)
                    {
                        if (exist)
                            rgb2[counter] = 255;
                        else
                            rgb2[counter] = 0;
                    }
                    else
                    {
                        if (!exist)
                            rgb2[counter] = 255;
                        else
                            rgb2[counter] = 0;
                    }
                }

            System.Runtime.InteropServices.Marshal.Copy(rgb2, 0, ptr2, rgb2.Length);
            ColorPalette myPalette = g.Palette;
            for (int i = 0; i < 256; ++i) myPalette.Entries[i] = Color.FromArgb(i, i, i);
            g.Palette = myPalette;
            g.UnlockBits(d2);
            return g;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int maskSize = Convert.ToInt32(textBox1.Text);
                if (maskSize <= 0) { MessageBox.Show("MaskSize need bigger than 0."); return; }
                if (maskSize % 2 == 0) { MessageBox.Show("MaskSize must been odd."); return; }
                image1 = Con(image1, maskSize, yan.DILATION);
                pictureBox2.Image = image1;
            }
            catch (Exception ex) { MessageBox.Show("Error:" + ex.Message); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int maskSize = Convert.ToInt32(textBox1.Text);
                if (maskSize <= 0) { MessageBox.Show("MaskSize need bigger than 0."); return; }
                if (maskSize % 2 == 0) { MessageBox.Show("MaskSize must been odd."); return; }
                image1 = Con(image1, maskSize, yan.EROSION);
                pictureBox2.Image = image1;
            }
            catch (Exception ex) { MessageBox.Show("Error:" + ex.Message); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int maskSize = Convert.ToInt32(textBox1.Text);
                if (maskSize <= 0) { MessageBox.Show("MaskSize need bigger than 0."); return; }
                if (maskSize % 2 == 0) { MessageBox.Show("MaskSize must been odd."); return; }
                image1 = Con(image1, maskSize, yan.EROSION);
                image1 = Con(image1, maskSize, yan.DILATION);
                pictureBox2.Image = image1;
            }
            catch (Exception ex) { MessageBox.Show("Error:" + ex.Message); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                int maskSize = Convert.ToInt32(textBox1.Text);
                if (maskSize <= 0) { MessageBox.Show("MaskSize need bigger than 0."); return; }
                if (maskSize % 2 == 0) { MessageBox.Show("MaskSize must been odd."); return; }
                image1 = Con(image1, maskSize, yan.DILATION);
                image1 = Con(image1, maskSize, yan.EROSION);
                pictureBox2.Image = image1;
            }
            catch (Exception ex) { MessageBox.Show("Error:" + ex.Message); }
        }
    }
}
