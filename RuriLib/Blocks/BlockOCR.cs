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
            var OCRTess = new TesseractEngine(@".\tessdata", "eng", EngineMode.Default);

            output.Add(OCRTess.Process(GetOCRImage()).GetText());

            return output;
        }

        [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
        public Pix GetOCRImage()
        {
            Pix OCR;
            var request = WebRequest.Create(Url);

            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                Bitmap captcha = (Bitmap)Bitmap.FromStream(stream);
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
    }
}