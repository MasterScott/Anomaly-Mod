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
        public ConfigOCRSettings()
        {
            InitializeComponent();
            DataContext = Globals.mainWindow.ConfigsPage.CurrentConfig.Config.Settings;
        }

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

        //   BELOW IS FOR IRONOCR WHICH I WILL NOT BE USING IT IS SLOW AND HAS LITTLE CONFIGURATIONS   \\

        //var Ocr = new AdvancedOcr()
        //{
        //    CleanBackgroundNoise = true,
        //    EnhanceContrast = true,
        //    EnhanceResolution = true,
        //    Language = IronOcr.Languages.English.OcrLanguagePack,
        //    Strategy = IronOcr.AdvancedOcr.OcrStrategy.Advanced,
        //    ColorSpace = AdvancedOcr.OcrColorSpace.Color,
        //    DetectWhiteTextOnDarkBackgrounds = true,
        //    InputImageType = AdvancedOcr.InputTypes.AutoDetect,
        //    RotateAndStraighten = true,
        //    ReadBarCodes = true,
        //    ColorDepth = 48
        //};
            var result = OCRTess.Process(GetOCRImage()).GetText();
            //var Result = Ocr.Read(GetOCRImage());
            //OcrImage = GetOCRImage();
            OCRResult.Content = result;
            //System.Windows.Forms.MessageBox.Show(Result);
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Pix GetOCRImage()
        {
            Pix OCR;
            var request = WebRequest.Create(OCRUrl.Text);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                System.Drawing.Image newImage = System.Drawing.Image.FromStream(stream);
                newImage = ConvertToGrayscale(newImage);
                Bitmap newImageB = (Bitmap)newImage;
                var captcha = (Bitmap)Bitmap.FromStream(stream);
                OCR = PixConverter.ToPix(captcha);
                OcrImage.Source = ImageSourceFromBitmap(captcha);
            }
            return OCR;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                DoOCR();
            }
            catch(Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
        }



        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public System.Drawing.Image ConvertToGrayscale(System.Drawing.Image image)
        {
            System.Drawing.Image grayscaleImage = new Bitmap(image.Width, image.Height, image.PixelFormat);

            // Create the ImageAttributes object and apply the ColorMatrix
            ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
            ColorMatrix grayscaleMatrix = new ColorMatrix(new float[][]{
        new float[] {0.299f, 0.299f, 0.299f, 0, 0},
        new float[] {0.587f, 0.587f, 0.587f, 0, 0},
        new float[] {0.114f, 0.114f, 0.114f, 0, 0},
        new float[] {     0,      0,      0, 1, 0},
        new float[] {     0,      0,      0, 0, 1}
        });
            attributes.SetColorMatrix(grayscaleMatrix);

            // Use a new Graphics object from the new image.
            using (Graphics g = Graphics.FromImage(grayscaleImage))
            {
                // Draw the original image using the ImageAttributes created above.
                g.DrawImage(image,
                            new Rectangle(0, 0, grayscaleImage.Width, grayscaleImage.Height),
                            0, 0, grayscaleImage.Width, grayscaleImage.Height,
                            GraphicsUnit.Pixel,
                            attributes);
            }

            return grayscaleImage;
        }
    }
}
