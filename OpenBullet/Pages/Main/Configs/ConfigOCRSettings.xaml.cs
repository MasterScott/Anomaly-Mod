using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tesseract;

namespace OpenBullet
{
    /// <summary>
    /// Interaction logic for ConfigOCRSettings.xaml
    /// </summary>
    public partial class ConfigOCRSettings : System.Windows.Controls.Page
    {
        RuriLib.ConfigSettings vm = Globals.mainWindow.ConfigsPage.CurrentConfig.Config.Settings;
        public ConfigOCRSettings()
        {
            InitializeComponent();
            DataContext = Globals.mainWindow.ConfigsPage.CurrentConfig.Config.Settings;
        }
        //int correct = 0;
        //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public void DoOCR()
        {
            var OCRTess = new TesseractEngine(@".\tessdata", "eng", EngineMode.Default);

            var result = OCRTess.Process(GetOCRImage()).GetText();
            //if (result.ToLower().Contains("c") && result.ToLower().Contains("h") && result.ToLower().Contains("3") && result.ToLower().Contains("n"))
                //correct++;
            OCRResult.Content = result;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Pix GetOCRImage()
        {
            Pix OCR;
            Bitmap captcha;
            Bitmap appliedCaptcha;
            var request = WebRequest.Create(OCRUrl.Text);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                captcha = (Bitmap)Bitmap.FromStream(stream);
                appliedCaptcha = captcha;
            }
            
            OcrImage.Source = ImageSourceFromBitmap(captcha);

            if (vm.ContrastGamma)
                appliedCaptcha = SetContrastGamma(appliedCaptcha);

            if (vm.Transparent)
                appliedCaptcha = SetTransparent(appliedCaptcha);

            if(vm.RemoveLines)
                appliedCaptcha = RemoveImageLines(appliedCaptcha);

            if (vm.RemoveNoise)
                appliedCaptcha = RemoveNoise(appliedCaptcha);

            if (vm.Grayscale)
                appliedCaptcha = ToGrayScale(appliedCaptcha);

            AppliedImage.Source = ImageSourceFromBitmap(appliedCaptcha);
            OCR = PixConverter.ToPix(appliedCaptcha);

            return OCR;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                //for (int i = 0; i < 1000; i++)
                //{
                    DoOCR();
                //}
                //System.Windows.Forms.MessageBox.Show("Correct amount: " + correct + "\nOut of: 1000");
            }
            catch(Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
        }

        public Bitmap SetContrastGamma(Bitmap original)
        {
            float contrastAmt = float.Parse(ContrastAmt.Text);
            float gammaAmt = float.Parse(GammaAmt.Text);
            float brightnessAmt = float.Parse(BrightnessAmt.Text);

            Bitmap adjustedImage = original;
            float brightness = brightnessAmt; // no change in brightness
            float contrast = contrastAmt; // twice the contrast
            float gamma = gammaAmt; // no change in gamma

            float adjustedBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            float[][] ptsArray ={
        new float[] {contrast, 0, 0, 0, 0}, // scale red
        new float[] {0, contrast, 0, 0, 0}, // scale green
        new float[] {0, 0, contrast, 0, 0}, // scale blue
        new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
        new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(adjustedImage);
            g.DrawImage(original, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height)
                , 0, 0, original.Width, original.Height,
                GraphicsUnit.Pixel, imageAttributes);
            return adjustedImage;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Bitmap ToGrayScale(Bitmap Bmp)
        {
            int rgb;
            System.Drawing.Color c;

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)Math.Round(.299 * c.R + .587 * c.G + .114 * c.B);
                    Bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(rgb, rgb, rgb));
                }
            return Bmp;
        }

        

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Bitmap RemoveImageLines(Bitmap Bmp)
        {
            int amtMin = Int32.Parse(PixelAmtMin.Text);
            int amtMax = Int32.Parse(PixelAmtMax.Text);
            System.Drawing.Color c;
            System.Drawing.Color compare1;
            System.Drawing.Color compare2;
            for(int x = 0; x < Bmp.Width; x++)
                for(int y = 0; y < Bmp.Height; y++)
                {
                    c = Bmp.GetPixel(x, y);

                    if (x - amtMax > 0 && y - amtMax > 0)
                    {
                        compare1 = Bmp.GetPixel(x - amtMin, y - amtMin);
                        compare2 = Bmp.GetPixel(x - amtMax, y - amtMax);
                        if (compare1 == compare2)
                            if (c != compare1) //Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                                if (x + amtMax < Bmp.Width && y + amtMax < Bmp.Height)
                                {
                                    compare1 = Bmp.GetPixel(x + amtMin, y + amtMin);
                                    compare2 = Bmp.GetPixel(x + amtMax, y + amtMax);
                                    if (compare1 == compare2)
                                        if (c != compare1)
                                            Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                                }
                    }



                    //////////if (x - 1 > 0 && y - 1 > 0)
                    //////////{
                    //////////    compare = Bmp.GetPixel(x - 1, y - 1);
                    //////////    if (c != compare)
                    //////////    {
                    //////////        if (x + 2 < Bmp.Width && y + 2 < Bmp.Height)
                    //////////        {
                    //////////            compare = Bmp.GetPixel(x + 2, y + 2);
                    //////////            if( c != compare)
                    //////////                Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);

                        //////////        }
                        //////////    }
                        //////////}

                        //////////if (x + 1 < Bmp.Width && y + 1 < Bmp.Height)
                        //////////{
                        //////////    compare = Bmp.GetPixel(x + 1, y + 1);
                        //////////    if (c != compare)
                        //////////    {
                        //////////        if (x - 2 > 0 && y - 2 > 0)
                        //////////        {
                        //////////            compare = Bmp.GetPixel(x - 2, y - 2);
                        //////////            if (c != compare)
                        //////////                Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);

                        //////////        }
                        //////////    }
                        //////////}




                        //if (x - 1 > 0 && y - 1 > 0)
                        //{
                        //    compare = Bmp.GetPixel(x - 1, y - 1);
                        //    if (c == compare)
                        //        Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                        //}
                        //if (x + 1 < Bmp.Width && y + 1 < Bmp.Height)
                        //{
                        //    compare = Bmp.GetPixel(x + 1, y + 1);
                        //    if (c == compare)
                        //        Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                        //}
                }

            return Bmp;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Bitmap RemoveNoise(Bitmap Bmp)
        {

            return Bmp;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Bitmap SetTransparent(Bitmap Bmp)
        {
            Bmp.MakeTransparent(System.Drawing.Color.White);
            return Bmp;
        }
    }
}
