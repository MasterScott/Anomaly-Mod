using RuriLib;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace OpenBullet.Pages.StackerBlocks
{
    /// <summary>
    /// Interaction logic for PageBlockOCR.xaml
    /// </summary>
    /// 
    [Obfuscation(Exclude = false, Feature = "+koi;-ctrl flow")]
    public partial class PageBlockOCR : Page
    {
        private BlockOCR block;

        public PageBlockOCR(BlockOCR block)
        {
            InitializeComponent();
            this.block = block;
            DataContext = this.block;
        }

        private void LanguageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            block.OcrLang = (string)LanguageList.SelectedItem;//.ToString().Split('.')[0];
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(@".\tessdata");

            foreach (var file in d.GetFiles("."))
            {
                if (!LanguageList.Items.Contains(file.Name))
                    LanguageList.Items.Add(file.Name.Split('.')[0]);
            }
        }
    }
}