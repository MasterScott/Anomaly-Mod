using Cloudflare;
using Cloudflare.CaptchaProviders;
using RuriLib.LS;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Windows.Media;
using System.Globalization;
using Tesseract;
using System.Drawing;
using System.Reflection;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RuriLib
{
    /// <summary>
    /// A block that can perform Image recognition.
    /// </summary>
    public class BlockOCR : BlockBase
    {
        private string variableName = "";

        /// <summary>The name of the output variable where the OCR response will be stored.</summary>
        public string VariableName { get { return variableName; } set { variableName = value; OnPropertyChanged(); } }

        private bool isCapture = false;

        /// <summary>Whether the output variable should be marked for Capture.</summary>
        public bool IsCapture { get { return isCapture; } set { isCapture = value; OnPropertyChanged(); } }

        private string url = "";

        /// <summary>The URL of the image.</summary>
        public string Url { get { return url; } set { url = value; OnPropertyChanged(); } }

        private string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

        /// <summary>The User-Agent header to use when grabbing the image.</summary>
        public string UserAgent { get { return userAgent; } set { userAgent = value; OnPropertyChanged(); } }

        private string ocrString = "";

        /// <summary>The URL of the image.</summary>
        public string OCRString { get { return ocrString; } set { ocrString = value; OnPropertyChanged(); } }

        private bool base64 = false;

        /// <summary>If the image needs converted from Base64.</summary>
        public bool Base64 { get { return base64; } set { base64 = value; OnPropertyChanged(); } }

        /// <summary>
        /// Creates a OCR block.
        /// </summary>
        public BlockOCR()
        {
            Label = "OCR";
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public override void Process(BotData data)
        {
            base.Process(data);

            InsertVariables(data, IsCapture, false, GetOCR(data), VariableName, "", "", false, true);
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public List<string> GetOCR(BotData data)
        {
            var output = new List<string>();
            var OCRTess = new TesseractEngine(@".\tessdata", "rus", EngineMode.Default);

            output.Add(OCRTess.Process(GetOCRImage()).GetText());

            return output;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Pix GetOCRImage()
        {
            Pix OCR;
            Bitmap captcha;
            var request = WebRequest.Create(Url);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                captcha = (Bitmap)Bitmap.FromStream(stream);

                //if (configSettings.ContrastGamma)
                //{
                //    captcha = SetContrastGamma(captcha);
                //}

                ////if (vm.Transparent)
                ////    captcha = SetTransparent(captcha);

                //if (configData.ConfigSettings.RemoveLines)
                //    captcha = RemoveImageLines(captcha);

                //if (configData.ConfigSettings.RemoveNoise)
                //    captcha = RemoveNoise(captcha);

                //if (configData.ConfigSettings.Grayscale)
                //    captcha = ToGrayScale(captcha);

                OCR = PixConverter.ToPix(captcha);
            }
            return OCR;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public override string ToLS(bool indent = true)
        {
            var writer = new BlockWriter(GetType(), indent, Disabled);
            writer
                .Label(Label)
                .Token("OCR")
                .Literal(Url);

            if (!writer.CheckDefault(VariableName, "VariableName"))
                writer
                    .Arrow()
                    .Token(IsCapture ? "CAP" : "VAR")
                    .Literal(VariableName);

            return writer.ToString();
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public override BlockBase FromLS(string line)
        {
            // Trim the line
            var input = line.Trim();

            // Parse the label
            if (input.StartsWith("#"))
                Label = LineParser.ParseLabel(ref input);

            Url = LineParser.ParseLiteral(ref input, "Url");

            if (LineParser.ParseToken(ref input, TokenType.Arrow, false) == "")
                return this;

            //Parse the VAR / CAP
            try
            {
                var varType = LineParser.ParseToken(ref input, TokenType.Parameter, true);
                if (varType.ToUpper() == "VAR" || varType.ToUpper() == "CAP")
                    IsCapture = varType.ToUpper() == "CAP";
            }
            catch { throw new ArgumentException("Invalid or missing variable type"); }

            // Parse the variable/capture name
            try { VariableName = LineParser.ParseToken(ref input, TokenType.Literal, true); }
            catch { throw new ArgumentException("Variable name not specified"); }

            return this;
        }




        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Bitmap SetContrastGamma(Bitmap original)
        {
            float contrastAmt = 1;//configSettings.Contrast;
            float gammaAmt = 1;//configSettings.Gamma;
            float brightnessAmt = 1;//configSettings.Brightness;

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
            int amtMin = 0;//configData.ConfigSettings.RemoveLinesMin;
            int amtMax = 0;//configData.ConfigSettings.RemoveLinesMax;
            System.Drawing.Color c;
            System.Drawing.Color compare1;
            System.Drawing.Color compare2;
            for (int x = 0; x < Bmp.Width; x++)
                for (int y = 0; y < Bmp.Height; y++)
                {
                    c = Bmp.GetPixel(x, y);

                    if (x - (amtMax + 1) > 0 && y - (amtMax + 1) > 0)
                    {
                        compare1 = Bmp.GetPixel(x - amtMin, y - amtMin);
                        compare2 = Bmp.GetPixel(x - amtMax, y - amtMax);
                        if (compare1 == compare2)
                            if (c != compare1)
                            {
                                if (compare1 != Bmp.GetPixel(x - (amtMin - 1), y - (amtMin - 1)))
                                    Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                                if (compare2 != Bmp.GetPixel(x - (amtMax + 1), y - (amtMax + 1)))
                                    Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                            }
                    }
                    if (x + (amtMax + 1) < Bmp.Width && y + (amtMax + 1) < Bmp.Height)
                    {
                        compare1 = Bmp.GetPixel(x + amtMin, y + amtMin);
                        compare2 = Bmp.GetPixel(x + amtMax, y + amtMax);
                        if (compare1 == compare2)
                            if (c != compare1)
                            {
                                if (compare1 != Bmp.GetPixel(x + (amtMin - 1), y + (amtMin - 1)))
                                    Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                                if (compare2 != Bmp.GetPixel(x + (amtMax + 1), y + (amtMax + 1)))
                                    Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                            }
                    }



                    //if (x - amtMax > 0 && y - amtMax > 0)
                    //{
                    //    compare1 = Bmp.GetPixel(x - amtMin, y - amtMin);
                    //    compare2 = Bmp.GetPixel(x - amtMax, y - amtMax);
                    //    if (compare1 == compare2)
                    //        if (c != compare1) //Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                    //            if (x + amtMax < Bmp.Width && y + amtMax < Bmp.Height)
                    //            {
                    //                compare1 = Bmp.GetPixel(x + amtMin, y + amtMin);
                    //                compare2 = Bmp.GetPixel(x + amtMax, y + amtMax);
                    //                if (compare1 == compare2)
                    //                    if (c != compare1)
                    //                        Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                    //            }
                    //}
                }

            return Bmp;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Bitmap RemoveNoise(Bitmap Bmp)
        {

            return Bmp;
        }

        //[Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        //public Bitmap SetTransparent(Bitmap Bmp)
        //{
        //    foreach (System.Drawing.Color i in ColorsList.Items)
        //        Bmp.MakeTransparent(i);
        //    return Bmp;
        //}





    }
}