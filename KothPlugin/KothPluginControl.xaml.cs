using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        public class KothPluginControlViewModel : INotifyDataErrorInfo
        {
            private readonly Dictionary<string, string> _validationErrors = new Dictionary<string, string>();

            public void Validate()
            {
                bool isValid = !string.IsNullOrEmpty(_text);
                bool contains = _validationErrors.ContainsKey(nameof(Text));
                if (!isValid && !contains)
                    _validationErrors.Add(nameof(Text), "Mandatory field!");
                else if (isValid && contains)
                    _validationErrors.Remove(nameof(Text));

                if (ErrorsChanged != null)
                    ErrorsChanged(this, new DataErrorsChangedEventArgs(nameof(Text)));
            }

            public bool HasErrors => _validationErrors.Count > 0;

            public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

            public IEnumerable GetErrors(string propertyName)
            {
                string message;
                if (_validationErrors.TryGetValue(propertyName, out message))
                    return new List<string> { message };

                return null;
            }

            private string _text;
            public string Text
            {
                get { return _text; }
                set
                {
                    _text = value; 
                }
            }
        }

    }
}
