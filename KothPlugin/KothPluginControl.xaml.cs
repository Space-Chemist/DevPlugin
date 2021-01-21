using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Discord.Webhook;
using NLog;
using NLog.Fluent;

namespace KothPlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class KothPluginControl : UserControl
    {
        
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public KothPluginControl()
        {
            InitializeComponent();
        }
        
        public KothPluginControl(Koth plugin) : this() {
            Plugin = plugin;
            DataContext = plugin.Config;
        }

        private KothPlugin.Koth Plugin { get; }
        
        private void RefreshPathButton_OnClick(object sender, RoutedEventArgs e)
        {
            Log.Info("Path Refresh");
            Koth.SetPath();
        }

        private void TestWebHookButton_OnClick(object sender, RoutedEventArgs e)
        {
            Log.Info("Test Webhook Sent");
            DiscordService.SendDiscordWebHook("Successful WebHook Test");
            
        }
        
        private void ClearScoresButton_OnClick(object sender, RoutedEventArgs e)
        {
            Log.Info("Scores Cleared");
            NetworkService.SendPacket("clear");
        }
        
        private void UpdateConfigButton_OnClick(object sender, RoutedEventArgs e)
        {
            Log.Info("Config updated ");
            Plugin.Save();
        }
    }
}
