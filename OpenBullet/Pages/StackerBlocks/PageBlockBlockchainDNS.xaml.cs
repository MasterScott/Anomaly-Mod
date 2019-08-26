using RuriLib;
using System.IO;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace OpenBullet.Pages.StackerBlocks
{
    /// <summary>
    /// Logica di interazione per PageBlockBlockchainDNS.xaml
    /// </summary>
    public partial class PageBlockBlockchainDNS : Page
    {
        private BlockBlockchainDNS block;

        public PageBlockBlockchainDNS(BlockBlockchainDNS block)
        {
            InitializeComponent();
            this.block = block;
            DataContext = this.block;
        }
    }
}