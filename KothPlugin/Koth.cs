using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Xml.Serialization;
using Discord;
using Discord.Webhook;
using NLog;
using Sandbox;
using Sandbox.ModAPI;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Session;


namespace KothPlugin
{
    public class Koth : TorchPluginBase, IWpfPlugin
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static string KothScorePath = "";
        private Persistent<KothPluginConfig> _config;
        private KothPluginControl _control;
        private TorchSessionManager _sessionManager;
        public KothPluginConfig Config => _config?.Data;

        public UserControl GetControl()
        {
            return _control ?? (_control = new KothPluginControl(this));
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);
            SetupConfig();
            
            _sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            _sessionManager.SessionStateChanged += SessionManagerOnSessionStateChanged;
            
        }

        private void SessionManagerOnSessionStateChanged(ITorchSession session, TorchSessionState newstate)
        {
            switch (newstate)
            {
                case TorchSessionState.Loading:
                    break;
                case TorchSessionState.Loaded:
                    SetPath();
                    WebService.StartWebServer();
                    //SetupNetwork();
                    MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(8008, HandleIncomingPacket);
                    break;
                case TorchSessionState.Unloading:
                    break;
                case TorchSessionState.Unloaded:
                    WebService.StopWebServer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newstate), newstate, null);
            }
        }
        
        
        
        private void HandleIncomingPacket(ushort comId, byte[] msg ,ulong id, bool relible)
        {
            try
            {
                string message = Encoding.ASCII.GetString(msg);
                SendDiscordWebHook("</reloadisgay>", message);
                
            }
            catch (Exception error)
            {
                Log.Error(error, "server error");
            }
        }
        
        private void SetupConfig()
        {
            var configFile = Path.Combine(StoragePath, "KothPluginConfig.cfg");
            try
            {
                _config = Persistent<KothPluginConfig>.Load(configFile);
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }

            if (_config?.Data != null) return;
            Log.Info("Creating Default Config");
            _config = new Persistent<KothPluginConfig>(configFile, new KothPluginConfig());
            _config.Save();
        }

        public void Save()
        {
            _config.Save();
        }


        private void SetPath()
        {
            var kothScoreName = MySandboxGame.ConfigDedicated.LoadWorld;
            KothScorePath = Path.Combine(kothScoreName, @"Storage\2002161364.sbm_NewKoth\Scores.data");
            if (!File.Exists(KothScorePath)) Log.Error("KOTH PLUGIN: NO SOCRE DATA, PLUGIN WILL FAIL");
        }
        
        public static session ScoresFromStorage()

        {
            var serializer = new XmlSerializer(typeof(session));
            using (var reader = new StreamReader(KothScorePath))
            {
                return (session) serializer.Deserialize(reader);
            }
        }
        public async void SendDiscordWebHook(string title, string msg)
        {
            const string webhookUrl = "https://discordapp.com/api/webhooks/800156920270815253/U05QvvZqSUm5iTLmEtLiIFJyGg19JR6rLOb16v6L05qraMypR6kpcQZSeD1NHegLb5Ip"; //TODO Config
            using (var client = new DiscordWebhookClient(webhookUrl))
            {
                var embed = new EmbedBuilder
                {
                    Title = title,
                    Description = msg
                };
                await client.SendMessageAsync("", embeds: new[] {embed.Build()});
            }
        }
    }
}