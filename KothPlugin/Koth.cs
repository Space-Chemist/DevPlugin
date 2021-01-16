using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using Discord;
using Discord.Webhook;
using KothPlugin.ModNetworkAPI;
using Nest;
using NLog;
using Sandbox;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Session;
using Torch.Managers.PatchManager;
using Torch.Session;
using VRage.Game;

namespace KothPlugin
{
    public class Koth : TorchPluginBase
    {
        public static bool _init;

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public const string Keyword = "/koth";
        public const string DisplayName = "KotH";
        public const ushort ComId = 42511;
        public static bool IsInitilaized = false;
        public string BotMessage = "";
        private Network Network => Network.Instance;
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
            var kothScoreName = MySandboxGame.ConfigDedicated.LoadWorld;
            KothScorePath = Path.Combine(kothScoreName, @"Storage\2002161364.sbm_NewKoth\Scores.data");
            if (!File.Exists(KothScorePath)) Log.Error("KOTH PLUGIN: NO SOCRE DATA, PLUGIN WILL FAIL");
            _sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            _sessionManager.SessionStateChanged += SessionManagerOnSessionStateChanged;
            SetupNetwork();
        }

        private void SetupNetwork()
        {
            if (!IsInitilaized)
            {
                Network.Init(ComId, DisplayName, Keyword);
            }
            
            if (Network.NetworkType == NetworkTypes.Client)
            {
                //MyVisualScriptLogicProvider.SendChatMessage("does not know it is a server");
                Network.RegisterNetworkCommand("Wipe", ServerCallBack_Wipe);
                Network.RegisterNetworkCommand("BotMessage", ServerCallback_BotMessage);
            }
            else
            {
                //MyVisualScriptLogicProvider.SendChatMessage("knows it is a server");
                Network.RegisterNetworkCommand("Wipe", ServerCallBack_Wipe);
                Network.RegisterNetworkCommand("BotMessage", ServerCallback_BotMessage);
                IsInitilaized = true;
            }
        }


        private void SessionManagerOnSessionStateChanged(ITorchSession session, TorchSessionState newstate)
        {
            switch (newstate)
            {
                case TorchSessionState.Loading:
                    break;
                case TorchSessionState.Loaded:
                    WebService.StartWebServer();
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
            const string webhookUrl = "https://discordapp.com/api/webhooks/";
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

        private void ServerCallBack_Wipe(ulong steamId, string commandString, byte[] data)
        {
            // Clearscore();
        }

        public void ServerCallback_BotMessage(ulong steamId, string commandString, byte[] data)
        {
            BotMessage = ASCIIEncoding.ASCII.GetString(data);
            SendDiscordWebHook("FuckReload", BotMessage);

            //MyVisualScriptLogicProvider.SendChatMessage("servercallback update");
        }


        public override void Update()
        {
            //try{
            if (MyAPIGateway.Session == null) return;

            if (!_init) Initialize();
        }

        private void Initialize()
        {
            _init = true;
        }


        public override void Dispose()
        {
            //Network.Dispose();
        }
    }
}