using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using NLog;


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
        
        private void WebServerUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (Koth.Instance.Config.WebServerEnabled)
            {
                if (Koth.SessionManager.CurrentSession !=null){
                    WebService.StartWebServer();
                }
            }
            else
            {
                if (Koth.SessionManager.CurrentSession !=null){
                    WebService.StopWebServer();
                }
            }
        }

    }
}
