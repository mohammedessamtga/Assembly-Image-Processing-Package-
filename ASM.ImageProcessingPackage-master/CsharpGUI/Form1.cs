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
using System.Runtime.InteropServices;
using System.Drawing.Imaging; 


namespace CsharpGUI
{
    public partial class Form1 : Form
    {

        [DllImport("Project.dll")]
        private static extern int Sum(int y, int b);

        [DllImport("Project.dll")]
        private static extern int SumArr([In] int[] arr, int sz);

        [DllImport("Project.dll")]
        private static extern void ToUpper([In, Out]char[] arr, int sz);

        [DllImport("Project.dll")]
        private static extern void Invert([In, Out] int[] redChannel, [In, Out] int[] greenChannel,
                                          [In, Out] int[] blueChannel, int imageSize);

        [DllImport("Project.dll")]
        private static extern void EqualizeHistogram([In, Out] int[] redChannel, int imageSize);

        [DllImport("Project.dll")]
        private static extern void ImageToGray([In, Out] int[] redChannel, [In, Out] int[] greenChannel,
                                          [In, Out] int[] blueChannel, int imagesize);

        [DllImport("Project.dll")]
        private static extern void AddImages([In, Out] int[] firstChannelToAdd, [In] int[] secondChannel, int imageSize);

        [DllImport("Project.dll")]
        private static extern void SubImages([In, Out] int[] firstChannelToSub, [In] int[] secondChannelToSub, int imageSize);
        [DllImport("Project.dll")]
        private static extern void Sobel([In, Out] int[] firstChannelToSub, int width , int height, int direction);
        public ImageBuffers BuffersFirstImage
        {
            get
            {
                if (this.inputImage_pictureBox != null && this.inputImage_pictureBox.Image != null && FirstImage != null)
                {
                    return ImageHelper.GetBuffersFromImage(this.FirstImage);
                }
                return null;
            }
        }

        public ImageBuffers BuffersSecondImage
        {
            get
            {
                if (this.inputImage2_pictureBox1 != null && this.inputImage2_pictureBox1.Image != null && SecondImage != null)
                {
                    return ImageHelper.GetBuffersFromImage(this.SecondImage);
                }
                return null;
            }
        }
        public static int [] Padding(int [] channel , int height , int width)
        {
            height += 2;
            width += 2;
            int c = 0;
            int[] arr = new int[(height * width)];
            for(int i = 0; i < width;i++)
            {
                for(int j = 0; j<height; j++)
                {
                    if((i==0)||(j==0)||(i==width-1)||(j==height-1))
                    {
                        arr[(i * height) + j] = 0;
                       
                    }
                    else
                    {
                        arr[(i * height) + j] = channel[c];
                        c++;
                    }
                }
              
            }
            return arr;
        }

        public Bitmap FirstImage { get; set; }

        public Bitmap SecondImage { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\Libraries\\Pictures";
            openFileDialog1.Filter = "*.BMP;*.PPM;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            string fname = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        string ext = Path.GetExtension(openFileDialog1.FileName);
                        fname = openFileDialog1.FileName;
                        using (myStream)
                        {
                            this.FirstImage = new Bitmap(myStream);
                            this.inputImage_pictureBox.Image = this.FirstImage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void secondImage_button2_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\Libraries\\Pictures";
            openFileDialog1.Filter = "*.BMP;*.PPM;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            string fname = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        string ext = Path.GetExtension(openFileDialog1.FileName);
                        fname = openFileDialog1.FileName;
                        using (myStream)
                        {
                            this.SecondImage = new Bitmap(myStream);
                            this.inputImage2_pictureBox1.Image = this.SecondImage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }



        private void invert_button_Click(object sender, EventArgs e)
        {
            //Get first image buffers
            var buffersOfFirstImage = BuffersFirstImage;

            int width = buffersOfFirstImage.Width;
            int height = buffersOfFirstImage.Height;
            int imageSize = width * height;

            Invert(buffersOfFirstImage.RedChannel, buffersOfFirstImage.BlueChannel, buffersOfFirstImage.GreenChannel, imageSize);
            //Refelct the result to the GUI
            //1. Convert the output channels into bitmap
            var outputBuffersObject = ImageHelper.CreateNewImageBuffersObject(buffersOfFirstImage.RedChannel, buffersOfFirstImage.GreenChannel, buffersOfFirstImage.BlueChannel, width, height);
            this.outputImage_pictureBox.Image = ImageHelper.GetImageFromBuffers(outputBuffersObject).BitmapObject;
        }

        private void loadSecondImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\Libraries\\Pictures";
            openFileDialog1.Filter = "*.BMP;*.PPM;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            string fname = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        string ext = Path.GetExtension(openFileDialog1.FileName);
                        fname = openFileDialog1.FileName;
                        using (myStream)
                        {
                            this.SecondImage = new Bitmap(myStream);
                            this.inputImage2_pictureBox1.Image = this.SecondImage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            var buffersOfFirstImage = BuffersFirstImage;

            int width = buffersOfFirstImage.Width;
            int height = buffersOfFirstImage.Height;
            int imageSize = width * height;

            EqualizeHistogram(buffersOfFirstImage.RedChannel, imageSize);
            EqualizeHistogram(buffersOfFirstImage.BlueChannel, imageSize);
            EqualizeHistogram(buffersOfFirstImage.GreenChannel, imageSize);

            var outputBuffersObject = ImageHelper.CreateNewImageBuffersObject(buffersOfFirstImage.RedChannel, buffersOfFirstImage.GreenChannel, buffersOfFirstImage.BlueChannel, width, height);
            this.outputImage_pictureBox.Image = ImageHelper.GetImageFromBuffers(outputBuffersObject).BitmapObject;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var buffersOfFirstImage = BuffersFirstImage;

            int width = buffersOfFirstImage.Width;
            int height = buffersOfFirstImage.Height;
            int imageSize = width * height;

            ImageToGray(buffersOfFirstImage.RedChannel, buffersOfFirstImage.GreenChannel, buffersOfFirstImage.BlueChannel, imageSize);

            var outputBuffersObject = ImageHelper.CreateNewImageBuffersObject(buffersOfFirstImage.RedChannel, buffersOfFirstImage.GreenChannel, buffersOfFirstImage.BlueChannel, width, height);
            this.outputImage_pictureBox.Image = ImageHelper.GetImageFromBuffers(outputBuffersObject).BitmapObject;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            var FirstImage = BuffersFirstImage;

            int width1 = FirstImage.Width;
            int height1 = FirstImage.Height;
            int imageSize1 = width1 * height1;

            var secondImage = BuffersSecondImage;
            int width2 = BuffersSecondImage.Width;
            int height2 = BuffersSecondImage.Height;
            int imageSize2 = width2 * height2;
            if (imageSize1 != imageSize2)
            {
                MessageBox.Show("Please Enter Two Images Of Same Sizes ");
            }
            else
            {
                
                AddImages(FirstImage.RedChannel, secondImage.RedChannel, imageSize1);
                AddImages(FirstImage.BlueChannel, secondImage.BlueChannel, imageSize1);
                AddImages(FirstImage.GreenChannel, secondImage.GreenChannel, imageSize1);
                var outputBuffersObject = ImageHelper.CreateNewImageBuffersObject(FirstImage.RedChannel, FirstImage.GreenChannel, FirstImage.BlueChannel, width1, height1);
                this.outputImage_pictureBox.Image = ImageHelper.GetImageFromBuffers(outputBuffersObject).BitmapObject;
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            var FirstImage = BuffersFirstImage;

            int width1 = FirstImage.Width;
            int height1 = FirstImage.Height;
            int imageSize1 = width1 * height1;

            var secondImage = BuffersSecondImage;
            int width2 = BuffersSecondImage.Width;
            int height2 = BuffersSecondImage.Height;
            int imageSize2 = width2 * height2;
            if (imageSize1 != imageSize2)
            {
                MessageBox.Show("Please Enter Two Images Of Same Sizes ");
            }
            else
            {
                
                SubImages(FirstImage.RedChannel, secondImage.RedChannel, imageSize1);
                SubImages(FirstImage.BlueChannel, secondImage.BlueChannel, imageSize1);
                SubImages(FirstImage.GreenChannel, secondImage.GreenChannel, imageSize1);
                var outputBuffersObject = ImageHelper.CreateNewImageBuffersObject(FirstImage.RedChannel, FirstImage.GreenChannel, FirstImage.BlueChannel, width1, height1);
                this.outputImage_pictureBox.Image = ImageHelper.GetImageFromBuffers(outputBuffersObject).BitmapObject;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var FirstImage = BuffersFirstImage;
            int width1 = FirstImage.Width;
            int height1 = FirstImage.Height;
            int imageSize = width1 * height1;
            ImageToGray(FirstImage.RedChannel,FirstImage.GreenChannel, FirstImage.BlueChannel, imageSize);
        
            int[] channel1 = Padding(FirstImage.RedChannel, height1, width1);
           
            Sobel(channel1, width1+2, height1+2,1);
            int img = (width1 + 2) * (height1 + 2);
           
            textBox1.Text = channel1.Count().ToString();
          
            var outputBuffersObject = ImageHelper.CreateNewImageBuffersObject(channel1, channel1,channel1, width1+2, height1+2);
            this.outputImage_pictureBox.Image = ImageHelper.GetImageFromBuffers(outputBuffersObject).BitmapObject;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            var FirstImage = BuffersFirstImage;
            int width1 = FirstImage.Width;
            int height1 = FirstImage.Height;
            int imageSize = width1 * height1;
            ImageToGray(FirstImage.RedChannel, FirstImage.GreenChannel, FirstImage.BlueChannel, imageSize);
           
            int[] channel1 = Padding(FirstImage.RedChannel, height1, width1);
            Sobel(channel1, width1 + 2, height1 + 2,0);
            int img = (width1 + 2) * (height1 + 2);
            textBox1.Text = channel1.Count().ToString();
           
            var outputBuffersObject = ImageHelper.CreateNewImageBuffersObject(channel1, channel1, channel1, width1 + 2, height1 + 2);
            this.outputImage_pictureBox.Image = ImageHelper.GetImageFromBuffers(outputBuffersObject).BitmapObject;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            inputImage_pictureBox.Image = null;
            inputImage_pictureBox.Refresh();

            inputImage2_pictureBox1.Image = null;
            inputImage2_pictureBox1.Refresh();
            
            outputImage_pictureBox.Image = null;
            outputImage_pictureBox.Refresh();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var FirstImage = BuffersFirstImage;
            int width1 = FirstImage.Width;
            int height1 = FirstImage.Height;
            int imageSize = width1 * height1;
            ImageToGray(FirstImage.RedChannel, FirstImage.GreenChannel, FirstImage.BlueChannel, imageSize);

            int[] channel1 = Padding(FirstImage.RedChannel, height1, width1);
            int [] channel3 = Padding(FirstImage.RedChannel, height1, width1);
            Sobel(channel1, width1 + 2, height1 + 2, 0);
            Sobel(channel3, width1 + 2, height1 + 2, 1);
            int img_size = (width1 + 2) * (height1 + 2);
            AddImages(channel1, channel3, img_size);
            for(int i = 0; i < img_size; i++)
            {
               double a = Math.Sqrt(channel1[i]);
                channel1[i] = Convert.ToInt32(a);  
            }
            var outputBuffersObject = ImageHelper.CreateNewImageBuffersObject(channel1, channel1, channel1, width1 + 2, height1 + 2);
            this.outputImage_pictureBox.Image = ImageHelper.GetImageFromBuffers(outputBuffersObject).BitmapObject;
        }

        private void button8_Click(object sender, EventArgs e)
        {  
              
            Image x = outputImage_pictureBox.Image;
            if(x==null)
            {
                return; 
            }
            SaveFileDialog sfd =  new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                x.Save(sfd.FileName);
            }
           /*
            
cmp ebx , 0 
jl LM1
cmp ebx , 255 
jg LM2
jmp LM3
LM1 : 
  neg ebx
cmp ebx , 255 
;mov ebx , 0 
jna LM3 
 LM2 : 
 mov ebx , 255
 LM3: 
  cmp ebx , 70 
  ja LMM
  mov ebx , 0 
  LMM:
  mov acc ,ebx 
  */


        }

        private void outputImage_pictureBox_Click(object sender, EventArgs e)
        {

        }
    }
}
