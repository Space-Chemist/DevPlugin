﻿using System.Windows.Controls;
using Torch;
using ProtoBuf; 

namespace KothPlugin
{
    public class KothPluginConfig : ViewModel
    {
        [ProtoMember(1)]

        private bool _enabled = true;
        private string _webhookUrl = "";
        private string _host = "localhost";
        private bool _apienabled = true;
        private int _port = 8888;
        private string _messageprefix = "";
        private string _color = "#8b1f5e";
        private bool _embedenabled = true;
        private string _embedtitle = "";
        private bool _webserverenabled = true;
        public bool WebServerEnabled
        {
            get => _webserverenabled;
            set => SetValue(ref _webserverenabled, value);
        }

        public string EmbedTitle
        {
            get => _embedtitle;
            set => SetValue(ref _embedtitle, value);
        }

        public bool EmbedEnabled
        {
            get => _embedenabled;
            set => SetValue(ref _embedenabled, value);
        }


        public string Color
        {
            get => _color;
            set => SetValue(ref _color, value);
        }

        public string MessagePrefix
        {
            get => _messageprefix;
            set => SetValue(ref _messageprefix, value);
        }
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

        public string WebHookUrl { get => _webhookUrl; set => SetValue(ref _webhookUrl, value); }

        public int Port
        {
            get => _port;
            set => SetValue(ref _port, value);
        }

    }
}