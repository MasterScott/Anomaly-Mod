﻿using RuriLib.LS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection;
using Tesseract;

namespace RuriLib
{
    /// <summary>
    /// A block that can perform Image recognition.
    /// </summary>
    ///
    [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
    public class BlockOCR : BlockBase
    {
        #region Variables

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

        private bool isBase64 = false;

        /// <summary>If the image needs converted from Base64.</summary>
        public bool IsBase64 { get { return isBase64; } set { isBase64 = value; OnPropertyChanged(); } }

        private float contrast = 1;

        /// <summary>OCR contrast value.</summary>
        public float Contrast { get { return contrast; } set { contrast = value; OnPropertyChanged(); } }

        private float gamma = 1;

        /// <summary>OCR gamma value.</summary>
        public float Gamma { get { return gamma; } set { gamma = value; OnPropertyChanged(); } }

        private float brightness = 1;

        /// <summary>OCR brightness value.</summary>
        public float Brightness { get { return brightness; } set { brightness = value; OnPropertyChanged(); } }

        private int linesMin = 0;

        /// <summary>OCR minimum lines to remove value.</summary>
        public int LinesMin { get { return linesMin; } set { linesMin = value; OnPropertyChanged(); } }

        private int linesMax = 0;

        /// <summary>OCR maximum lines to remove value.</summary>
        public int LinesMax { get { return linesMax; } set { linesMax = value; OnPropertyChanged(); } }

        private bool conGamBri = false;

        /// <summary>If the image needs Contrast, Gamma or Brightness adjusted.</summary>
        public bool ConGamBri { get { return conGamBri; } set { conGamBri = value; OnPropertyChanged(); } }

        private bool grayScale = false;

        /// <summary>If the image needs converted to grayscale.</summary>
        public bool GrayScale { get { return grayScale; } set { grayScale = value; OnPropertyChanged(); } }

        private bool removeLines = false;

        /// <summary>If the image needs lines or pixels removed.</summary>
        public bool RemoveLines { get { return removeLines; } set { removeLines = value; OnPropertyChanged(); } }

        private bool removeNoise = false;

        /// <summary>If the image needs noise removed.</summary>
        public bool RemoveNoise { get { return removeNoise; } set { removeNoise = value; OnPropertyChanged(); } }

        private bool dilate = false;

        /// <summary>If the image needs noise removed.</summary>
        public bool Dilate { get { return dilate; } set { dilate = value; OnPropertyChanged(); } }

        private float threshold = 1.0f;

        /// <summary>If the image needs noise removed.</summary>
        public float Threshold { get { return threshold; } set { threshold = value; OnPropertyChanged(); } }

        private string ocrLang = "eng";

        /// <summary>Language the Tesseract uses to read the Image.</summary>
        public string OcrLang { get { return ocrLang; } set { ocrLang = value; OnPropertyChanged(); } }

        #endregion Variables

        /// <summary>
        /// Creates a OCR block.
        /// </summary>
        public BlockOCR()
        {
            Label = "OCR";
        }

        public override void Process(BotData data)
        {
            base.Process(data);

            InsertVariables(data, IsCapture, false, GetOCR(data), VariableName, "", "", false, true);
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public List<string> GetOCR(BotData data)
        {
            var output = new List<string>();
            var OCRTess = new TesseractEngine(@".\tessdata", OcrLang, EngineMode.Default);

            output.Add(OCRTess.Process(GetOCRImage(data)).GetText());

            return output;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Pix GetOCRImage(BotData data)
        {
            Pix OCR;
            Bitmap captcha;

            var inputs = ReplaceValues(Url, data);

            //if (inputs.Contains("."))
            if (IsBase64)
            {
                byte[] imageBytes = Convert.FromBase64String(inputs);
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    captcha = (Bitmap)Bitmap.FromStream(ms);
                    var appliedCaptcha = captcha;

                    if (ConGamBri)
                        appliedCaptcha = SetContrastGamma(appliedCaptcha);

                    ////if (vm.Transparent)
                    ////    captcha = SetTransparent(captcha);

                    if (GrayScale)
                        appliedCaptcha = ToGrayScale(appliedCaptcha);

                    if (RemoveNoise)
                        appliedCaptcha = RemoveNoiseThreshold(appliedCaptcha, Threshold);

                    if (RemoveLines)
                        appliedCaptcha = RemoveImageLines(appliedCaptcha);

                    if (Dilate)
                        appliedCaptcha = DilateImage(appliedCaptcha);

                    OCR = PixConverter.ToPix(appliedCaptcha);
                }
            }
            else
            {
                var request = WebRequest.Create(inputs);

                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    captcha = (Bitmap)Bitmap.FromStream(stream);
                    var appliedCaptcha = captcha;

                    if (ConGamBri)
                        appliedCaptcha = SetContrastGamma(appliedCaptcha);

                    ////if (vm.Transparent)
                    ////    captcha = SetTransparent(captcha);

                    if (RemoveNoise)
                        appliedCaptcha = RemoveNoiseThreshold(appliedCaptcha, Threshold);

                    if (GrayScale)
                        appliedCaptcha = ToGrayScale(appliedCaptcha);

                    if (RemoveLines)
                        appliedCaptcha = RemoveImageLines(appliedCaptcha);

                    if(Dilate)
                        appliedCaptcha = DilateImage(appliedCaptcha);

                    OCR = PixConverter.ToPix(appliedCaptcha);
                }
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
                    .Literal(VariableName)
                    .Token(OcrLang)
                    .Indent();

            writer
                .Boolean(IsBase64, "IsBase64")
                .Boolean(ConGamBri, "ConGamBri")
                .Boolean(GrayScale, "GrayScale")
                .Boolean(RemoveLines, "RemoveLines")
                .Boolean(RemoveNoise, "RemoveNoise")
                .Boolean(Dilate, "Dilate")
                .Indent();

            if (ConGamBri)
                writer
                    .Float(Contrast)
                    .Float(Gamma)
                    .Float(Brightness)
                    .Indent();

            if (RemoveLines)
                writer
                    .Integer(LinesMin)
                    .Integer(LinesMax)
                    .Indent();

            if (RemoveNoise)
                writer
                    .Float(Threshold)
                    .Return();

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

            OcrLang = LineParser.ParseToken(ref input, TokenType.Parameter, false);

            while (LineParser.Lookahead(ref input) == TokenType.Boolean)
                LineParser.SetBool(ref input, this);

            try
            {
                Contrast = LineParser.ParseFloat(ref input, "Contrast");
                Gamma = LineParser.ParseFloat(ref input, "Gamma");
                Brightness = LineParser.ParseFloat(ref input, "Brightness");

                LinesMin = LineParser.ParseInt(ref input, "LinesMin");
                LinesMax = LineParser.ParseInt(ref input, "LinesMax");

                Threshold = LineParser.ParseFloat(ref input, "Threshold");
            }
            catch { }

            return this;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Bitmap SetContrastGamma(Bitmap original)
        {
            //float contrastAmt = contrast;//configSettings.Contrast;
            //float gammaAmt = 1;//configSettings.Gamma;
            //float brightnessAmt = 1;//configSettings.Brightness;

            Bitmap adjustedImage = original;
            //float brightness = brightnessAmt; // no change in brightness
            //float contrast = contrastAmt; // twice the contrast
            //float gamma = gammaAmt; // no change in gamma

            float adjustedBrightness = brightness - 1.0f;
            // create matrix that will brighten and contrast the image
            float[][] ptsArray ={
        new float[] {Contrast, 0, 0, 0, 0}, // scale red
        new float[] {0, Contrast, 0, 0, 0}, // scale green
        new float[] {0, 0, Contrast, 0, 0}, // scale blue
        new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
        new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(Gamma, ColorAdjustType.Bitmap);
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
            //int amtMin = 0;//configData.ConfigSettings.RemoveLinesMin;
            //int amtMax = 0;//configData.ConfigSettings.RemoveLinesMax;
            System.Drawing.Color c;
            System.Drawing.Color compare1;
            System.Drawing.Color compare2;
            System.Drawing.Color compare11;
            System.Drawing.Color compare22;
            for (int x = 0; x < Bmp.Width; x++)
                for (int y = 0; y < Bmp.Height; y++)
                {
                    c = Bmp.GetPixel(x, y);


                    //if (x - (LinesMax + 1) > 0 && y - (LinesMax + 1) > 0)
                    //{
                    //    compare1 = Bmp.GetPixel(x - LinesMin, y - LinesMin);
                    //    compare11 = Bmp.GetPixel(x - (LinesMin - 1), y - (LinesMin - 1));
                    //    compare2 = Bmp.GetPixel(x - LinesMax, y - LinesMax);
                    //    compare22 = Bmp.GetPixel(x - (LinesMax - 1), y - (LinesMax - 1));

                    //    if (compare1 != compare2)
                    //    {
                    //        if (compare1 != c && compare11 != c)
                    //            Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                    //        if (compare2 != c && compare22 != c)
                    //            Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                    //    }
                    //}
                    //if (x + (LinesMax + 1) < Bmp.Width && y + (LinesMax + 1) < Bmp.Height)
                    //{
                    //    compare1 = Bmp.GetPixel(x + LinesMin, y + LinesMin);
                    //    compare11 = Bmp.GetPixel(x + (LinesMin - 1), y + (LinesMin - 1));
                    //    compare2 = Bmp.GetPixel(x + LinesMax, y + LinesMax);
                    //    compare22 = Bmp.GetPixel(x + (LinesMax - 1), y + (LinesMax - 1));

                    //    if (compare1 != compare2)
                    //    {
                    //        if (compare1 != c && compare11 != c)
                    //            Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                    //        if (compare2 != c && compare22 != c)
                    //            Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                    //    }
                    //}


                    //for (int x = 0; x < Bmp.Width; x++)
                    //    for (int y = 0; y < Bmp.Height; y++)
                    //    {
                    //        c = Bmp.GetPixel(x, y);

                            if (x - (LinesMax + 1) > 0 && y - (LinesMax + 1) > 0)
                            {
                                compare1 = Bmp.GetPixel(x - LinesMin, y - LinesMin);
                                compare2 = Bmp.GetPixel(x - LinesMax, y - LinesMax);
                                if (compare1 == compare2)
                                    if (c != compare1)
                                    {
                                        if (compare1 != Bmp.GetPixel(x - (LinesMin - 1), y - (LinesMin - 1)))
                                            Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                                        if (compare2 != Bmp.GetPixel(x - (LinesMax + 1), y - (LinesMax + 1)))
                                            Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                                    }
                            }
                            if (x + (LinesMax + 1) < Bmp.Width && y + (LinesMax + 1) < Bmp.Height)
                            {
                                compare1 = Bmp.GetPixel(x + LinesMin, y + LinesMin);
                                compare2 = Bmp.GetPixel(x + LinesMax, y + LinesMax);
                                if (compare1 == compare2)
                                    if (c != compare1)
                                    {
                                        if (compare1 != Bmp.GetPixel(x + (LinesMin - 1), y + (LinesMin - 1)))
                                            Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                                        if (compare2 != Bmp.GetPixel(x + (LinesMax + 1), y + (LinesMax + 1)))
                                            Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                                    }
                            }



                            //if (x - (LinesMax + 1) > 0 && y - (LinesMax + 1) > 0)
                            //{
                            //    compare1 = Bmp.GetPixel(x - LinesMin, y - LinesMin);
                            //    compare2 = Bmp.GetPixel(x - LinesMax, y - LinesMax);
                            //    if (compare1 == compare2)
                            //        if (c != compare1)
                            //        {
                            //            if (compare1 != Bmp.GetPixel(x - (LinesMin - 1), y - (LinesMin - 1)))
                            //                Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                            //            if (compare2 != Bmp.GetPixel(x - (LinesMax + 1), y - (LinesMax + 1)))
                            //                Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                            //        }
                            //}
                            //if (x + (LinesMax + 1) < Bmp.Width && y + (LinesMax + 1) < Bmp.Height)
                            //{
                            //    compare1 = Bmp.GetPixel(x + LinesMin, y + LinesMin);
                            //    compare2 = Bmp.GetPixel(x + LinesMax, y + LinesMax);
                            //    if (compare1 == compare2)
                            //        if (c != compare1)
                            //        {
                            //            if (compare1 != Bmp.GetPixel(x + (LinesMin - 1), y + (LinesMin - 1)))
                            //                Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                            //            if (compare2 != Bmp.GetPixel(x + (LinesMax + 1), y + (LinesMax + 1)))
                            //                Bmp.SetPixel(x, y, System.Drawing.Color.Transparent);
                            //        }
                            //}

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
        public Bitmap RemoveNoiseThreshold(Bitmap Bmp, float threshold)
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

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Bitmap DilateImage(Bitmap SrcImage)
        {
            // Create Destination bitmap.
            Bitmap tempbmp = new Bitmap(SrcImage.Width, SrcImage.Height);

            // Take source bitmap data.
            BitmapData SrcData = SrcImage.LockBits(new Rectangle(0, 0,
                SrcImage.Width, SrcImage.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            // Take destination bitmap data.
            BitmapData DestData = tempbmp.LockBits(new Rectangle(0, 0, tempbmp.Width,
                tempbmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Element array to used to dilate.
            byte[,] sElement = new byte[5, 5] {
        {0,0,1,0,0},
        {0,1,1,1,0},
        {1,1,1,1,1},
        {0,1,1,1,0},
        {0,0,1,0,0}
    };

            // Element array size.
            int size = 5;
            byte max, clrValue;
            int radius = size / 2;
            int ir, jr;

            unsafe
            {

                // Loop for Columns.
                for (int colm = radius; colm < DestData.Height - radius; colm++)
                {
                    // Initialise pointers to at row start.
                    byte* ptr = (byte*)SrcData.Scan0 + (colm * SrcData.Stride);
                    byte* dstPtr = (byte*)DestData.Scan0 + (colm * SrcData.Stride);

                    // Loop for Row item.
                    for (int row = radius; row < DestData.Width - radius; row++)
                    {
                        max = 0;
                        clrValue = 0;

                        // Loops for element array.
                        for (int eleColm = 0; eleColm < 5; eleColm++)
                        {
                            ir = eleColm - radius;
                            byte* tempPtr = (byte*)SrcData.Scan0 +
                                ((colm + ir) * SrcData.Stride);

                            for (int eleRow = 0; eleRow < 5; eleRow++)
                            {
                                jr = eleRow - radius;

                                // Get neightbour element color value.
                                clrValue = (byte)((tempPtr[row * 3 + jr] +
                                    tempPtr[row * 3 + jr + 1] + tempPtr[row * 3 + jr + 2]) / 3);

                                if (max < clrValue)
                                {
                                    if (sElement[eleColm, eleRow] != 0)
                                        max = clrValue;
                                }
                            }
                        }

                        dstPtr[0] = dstPtr[1] = dstPtr[2] = max;

                        ptr += 3;
                        dstPtr += 3;
                    }
                }
            }

            // Dispose all Bitmap data.
            SrcImage.UnlockBits(SrcData);
            tempbmp.UnlockBits(DestData);

            // return dilated bitmap.
            return tempbmp;
        }

    }
}