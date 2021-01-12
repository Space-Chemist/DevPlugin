using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
using NLog;
using Sandbox;
using Sandbox.ModAPI;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Session;
using Torch.Session;

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

        private Persistent<KothPluginConfig> _config;
        private KothPluginControl _control;
        private TorchSessionManager _sessionManager;
        public string KothScorePath = "";
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
        }


        private void SessionManagerOnSessionStateChanged(ITorchSession session, TorchSessionState newstate)
        {
            switch (newstate)
            {
                case TorchSessionState.Loading:
                    break;
                case TorchSessionState.Loaded:
                    var KothScoreName = MySandboxGame.ConfigDedicated.LoadWorld;
                    KothScorePath = Path.Combine(KothScoreName,
                        $@"Storage\2002161364.sbm_NewKoth\Scores.data");
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

        public static Koth ScoresFromStorage()
        {
            Log.Warn("got passed function first step");
            var settings = new Koth();
            try
            {
                Log.Warn("got passed function try");
                if (MyAPIGateway.Utilities.FileExistsInWorldStorage(Filename, typeof(Koth)))
                {
                    Log.Warn("Found Koth Scores in Storage");
                    var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage(Filename, typeof(Koth));
                    var text = reader.ReadToEnd();
                    reader.Close();

                    settings = MyAPIGateway.Utilities.SerializeFromXML<Koth>(text);
                    Log.Info(text.ToString);
                }
            }
            catch (Exception e)
            {
                Log.Warn("Failed to recover Koth Scores from storage");
            }

            return settings;
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

            Communication.RegisterHandlers();
        }


        public override void Dispose()
        {
            base.Dispose();
            Communication.UnregisterHandlers();
        }
    }
}