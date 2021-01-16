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
        public const string Filename = "Scores.data";
        public static bool _init;
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static int pageViews;
        public static int requestCount;
        //public const string Keyword = "/koth";
        //public const string DisplayName = "KotH";
        //public const ushort ComId = 42511;
        //public static bool IsInitilaized = false;
        //public string BotMessage = "";
        //private Network Network => Network.Instance;

        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";

        public static string KothScorePath = "";

        private Persistent<KothPluginConfig> _config;
        private KothPluginControl _control;
        private TorchSessionManager _sessionManager;
        public KothPluginConfig Config => _config?.Data;

        public UserControl GetControl()
        {
            return _control ?? (_control = new KothPluginControl(this));
        }

        public static async Task HandleIncomingConnections()
        {
            var runServer = true;

            while (runServer)
            {
                var ctx = await listener.GetContextAsync();
                var req = ctx.Request;
                var resp = ctx.Response;
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                if (req.HttpMethod == "POST" && req.Url.AbsolutePath == "/shutdown")
                {
                    Log.Info("Shutdown requested");
                    runServer = false;
                }

                if (req.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                var disableSubmit = !runServer ? "disabled" : "";
                var data = Encoding.UTF8.GetBytes(string.Format(pageData, pageViews, disableSubmit));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);
            SetupConfig();
            _sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            _sessionManager.SessionStateChanged += SessionManagerOnSessionStateChanged;
            //SetupNetwork();
        }

        /*private void SetupNetwork()
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
        }*/
        
        [PatchShim]
        public static class MyModAdditionPatch {

            public static readonly Logger Log = LogManager.GetCurrentClassLogger();

            internal static readonly MethodInfo update =
                typeof(MySession).GetMethod("GetWorld", BindingFlags.Instance | BindingFlags.Public) ??
                throw new Exception("Failed to find MySession.GetWorld method to patch");

            internal static readonly MethodInfo updatePatch =
                typeof(MyModAdditionPatch).GetMethod(nameof(SuffixGetWorld), BindingFlags.Static | BindingFlags.Public) ??
                throw new Exception("Failed to find patch method");

            public static void Patch(PatchContext ctx) {

                try {

                    ctx.GetPattern(update).Suffixes.Add(updatePatch);

                    Log.Info("Patching Successful MySessionPatch!");

                } catch (Exception e) {
                    Log.Error(e, "Patching failed!");
                }
            }

            public static void SuffixGetWorld(ref MyObjectBuilder_World __result) {
                __result.Checkpoint.Mods = __result.Checkpoint.Mods.ToList();
                __result.Checkpoint.Mods.Add(new MyObjectBuilder_Checkpoint.ModItem(2183079146));
            }
        }

        /*public void Wiped()
        {
            Network.SendCommand("wiped");
        }*/

        private void SessionManagerOnSessionStateChanged(ITorchSession session, TorchSessionState newstate)
        {
            switch (newstate)
            {
                case TorchSessionState.Loading:
                    break;
                case TorchSessionState.Loaded:
                    var KothScoreName = MySandboxGame.ConfigDedicated.LoadWorld;
                    KothScorePath = Path.Combine(KothScoreName,
                        @"Storage\2002161364.sbm_NewKoth\Scores.data");
                    Log.Warn(KothScorePath.ToString);
                    if (!File.Exists(KothScorePath)) Log.Info("No scores");

                    listener = new HttpListener();
                    listener.Prefixes.Add(url);
                    listener.Start();
                    Log.Info("Listening for connections on {0}", url);
                    Task.Run(async () => await HandleIncomingConnections());
                    
                    WebService.StartWebServer();
                    break;
                case TorchSessionState.Unloading:
                    break;
                case TorchSessionState.Unloaded:
                    listener.Close();
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
            var ser = new XmlSerializer(typeof(session));

            using (var sr = new StreamReader(KothScorePath))
            {
                return (session) ser.Deserialize(sr);
            }
        }

        public async void SendWebHook(string title, string msg)
        {
            using (var client = new DiscordWebhookClient("https://discordapp.com/api/webhooks/798454863452831784/IVawsrVD46QFbLVxwFarrEGUTZBTScQcQYUbYTapbPKDvVU9TnodMkwYyrLunZRDkzRL"))
            {
                var embed = new EmbedBuilder
                {
                    Title = title,
                    Description = msg
                };
                await client.SendMessageAsync(msg, embeds: new[] {embed.Build()});
            }
        }
        
        //private void ServerCallBack_Wipe(ulong steamId, string commandString, byte[] data)
        //{
           // Clearscore();
        //}
        
        /*public void ServerCallback_BotMessage(ulong steamId, string commandString, byte[] data)
        {
            BotMessage = ASCIIEncoding.ASCII.GetString(data);
            SendWebHook("FuckReload", BotMessage);

            //MyVisualScriptLogicProvider.SendChatMessage("servercallback update");
        }*/
        

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