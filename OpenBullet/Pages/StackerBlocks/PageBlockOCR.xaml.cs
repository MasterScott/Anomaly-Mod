using RuriLib;
using System;
using System.Windows.Controls;

namespace OpenBullet.Pages.StackerBlocks
{
    /// <summary>
    /// Interaction logic for PageBlockOCR.xaml
    /// </summary>
    public partial class PageBlockOCR : Page
    {
        private BlockOCR block;

        public PageBlockOCR(BlockOCR block)
        {
            InitializeComponent();
            this.block = block;
            DataContext = this.block;
        }
    }
}