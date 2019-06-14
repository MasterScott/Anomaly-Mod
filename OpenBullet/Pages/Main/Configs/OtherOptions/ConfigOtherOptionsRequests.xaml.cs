using System.Windows.Controls;

namespace OpenBullet
{
    /// <summary>
    /// Logica di interazione per ConfigOtherOptionsRequests.xaml
    /// </summary>
    public partial class ConfigOtherOptionsRequests : Page
    {
        public ConfigOtherOptionsRequests()
        {
            InitializeComponent();
            DataContext = Globals.mainWindow.ConfigsPage.CurrentConfig.Config.Settings;
        }
    }
}