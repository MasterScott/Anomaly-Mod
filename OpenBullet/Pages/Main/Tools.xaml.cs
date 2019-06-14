using OpenBullet.Pages.Main.Tools;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per Tools.xaml
    /// </summary>
    public partial class Tools : Page
    {
        private ToolsListGenerator ListGenerator;
        private ToolsSeleniumTools SeleniumTools;
        private ComboSuite ComboSuiteTools;
        private LolixDecrypt LolixTools;

        public Tools()
        {
            InitializeComponent();

            ListGenerator = new ToolsListGenerator();
            SeleniumTools = new ToolsSeleniumTools();
            ComboSuiteTools = new ComboSuite();
            LolixTools = new LolixDecrypt();

            menuOptionListGenerator_MouseDown(this, null);
        }

        #region Menu Options

        private void menuOptionListGenerator_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = ListGenerator;
            menuOptionSelected(menuOptionListGenerator);
        }

        private void menuOptionSeleniumUtilities_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = SeleniumTools;
            menuOptionSelected(menuOptionSeleniumTools);
        }

        private void menuOptionComboSuite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = ComboSuiteTools;
            menuOptionSelected(menuOptionComboSuite);
        }

        private void menuOptionLolixDecrypt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = LolixTools;
            menuOptionSelected(menuOptionLolixDecrypt);
        }

        private void menuOptionSelected(object sender)
        {
            foreach (var child in topMenu.Children)
            {
                try
                {
                    var c = (Label)child;
                    c.Foreground = Globals.GetBrush("ForegroundMain");
                }
                catch { }
            }
            ((Label)sender).Foreground = Globals.GetBrush("ForegroundCustom");
        }

        #endregion Menu Options
    }
}