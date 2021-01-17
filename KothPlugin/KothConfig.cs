using System.Windows.Controls;
using Torch;
using ProtoBuf; 

namespace KothPlugin
{
    public class KothPluginConfig : ViewModel
    {
        [ProtoMember(1)]

        private bool _enabled = true;
        private string _webserverUrl = "";
        private string _host = "";
        private bool _apienabled = true;
        private int _port = 8888;
        
        public string Host
        {
            get => _host;
            set => SetValue(ref _host, value);
        }

        public bool Enabled { get => _enabled; set => SetValue(ref _enabled, value); }
        public bool ApiEnabled
        {
            get => _apienabled;
            set => SetValue(ref _apienabled, value);
        }

        public string WebserverUrl { get => _webserverUrl; set => SetValue(ref _webserverUrl, value); }

        public int Port
        {
            get => _port;
            set => SetValue(ref _port, value);
        }
    }
}