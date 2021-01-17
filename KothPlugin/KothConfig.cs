using System.Windows.Controls;
using Torch;
using ProtoBuf; 

namespace KothPlugin
{
    public class KothPluginConfig : ViewModel
    {
        [ProtoMember(1)]
        public string FuckReload { get; set; }
        private string _Username;
        private string _Password;
        private int _AuthToken;
        private bool _PreferBulkChanges;

        private bool _enabled = true;
        private string _webserverUrl = "";
        private string _host = "";
        private bool _apienabled = true;
        
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

        public string Username
        {
            get
            {
                return this._Username;
            }
            set => SetValue(ref _Username, value);
        }

        public string Password
        {
            get
            {
                return this._Password;
            }
            set => SetValue(ref _Password, value);
        }

        public int AuthToken
        {
            get
            {
                return this._AuthToken;
            }
            set => SetValue(ref _AuthToken, value);
        }

        public bool PreferBulkChanges
        {
            get
            {
                return this._PreferBulkChanges;
            }
            set => SetValue(ref _PreferBulkChanges, value);
        }
    }
}