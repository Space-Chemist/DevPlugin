using Torch;
using ProtoBuf; 

namespace KothPlugin
{
    public class KothPluginConfig : ViewModel
    {
        [ProtoMember(1)]

        private bool _webhookenabled = true;
        private bool _webserverenabled = true;
        private bool _embedenabled = true;
        private bool _apienabled = true;
        private bool _webpageenabled = true;
        private string _webhookUrl = "";
        private string _host = "localhost";
        private string _messageprefix = "";
        private string _color = "#8b1f5e";
        private string _embedtitle = "";
        private int _port = 8888;

        public bool WebHookEnabled
        {
            get => _webhookenabled; 
            set => SetValue(ref _webhookenabled, value);
        }
        
        public bool WebServerEnabled
        {
            get => _webserverenabled;
            set => SetValue(ref _webserverenabled, value);
            
        }
        
        public bool EmbedEnabled
        {
            get => _embedenabled;
            set => SetValue(ref _embedenabled, value);
        }
        
        public bool ApiEnabled
        {
            get => _apienabled;
            set => SetValue(ref _apienabled, value);
        }
        
        public bool WebPageEnabled
        {
            get => _webpageenabled;
            set => SetValue(ref _webpageenabled, value);
        }
        
        public string WebHookUrl
        {
            get => _webhookUrl; 
            set => SetValue(ref _webhookUrl, value);
        }
        
        public string Host
        {
            get => _host;
            set => SetValue(ref _host, value);
        }
        
        public string MessagePrefix
        {
            get => _messageprefix;
            set => SetValue(ref _messageprefix, value);
        }
        
        public string Color
        {
            get => _color;
            set => SetValue(ref _color, value);
        }

        public string EmbedTitle
        {
            get => _embedtitle;
            set => SetValue(ref _embedtitle, value);
        }

        public int Port
        {
            get => _port;
            set => SetValue(ref _port, value);
        }

    }
}