using System.Windows;
using System.Windows.Controls;

namespace KothPlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class KothPluginControl : UserControl
    {
        public KothPluginControl()
        {
            InitializeComponent();
        }
        
        public KothPluginControl(Koth plugin) : this() {
            Plugin = plugin;
            DataContext = plugin.Config;
        }

        private KothPlugin.Koth Plugin { get; }
        
        private void SaveButton_OnClick(object sender, RoutedEventArgs e) {
            Plugin.Save();
        }

        private void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}
