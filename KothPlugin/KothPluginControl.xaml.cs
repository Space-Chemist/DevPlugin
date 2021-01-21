using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Discord.Webhook;
using NLog.Fluent;

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
        
        private void RefreshPathButton_OnClick(object sender, RoutedEventArgs e)
        {
            Koth.SetPath();
        }

        private void TestWebHookButton_OnClick(object sender, RoutedEventArgs e)
        {
            using (var client = new DiscordWebhookClient(Koth.Instance.Config.WebHookUrl))
            { 
                client.SendMessageAsync("Successful WebHook Test");
            }
        }
        
        private void ClearScoresButton_OnClick(object sender, RoutedEventArgs e)
        {
            Log.Info("Scores have been Cleared");
            NetworkService.SendPacket("clear");
        }
        
        private void UpdateConfigButton_OnClick(object sender, RoutedEventArgs e)
        {
            Plugin.Save();
        }
    }
}
