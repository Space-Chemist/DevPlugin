using NLog;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Nest;

using Sandbox.ModAPI;
using Torch.Session;
using VRage.Game.Entity;
using VRage.ModAPI;

namespace KothPlugin
{
    public class Koth : TorchPluginBase
    {
        private KothPluginControl _control;
        public static bool _init;
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private Persistent<KothPluginConfig> _config;
        public KothPluginConfig Config => _config?.Data;
        public UserControl GetControl() => _control ?? (_control = new KothPluginControl(this));
        public const string Filename = "Scores.data";
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static int pageViews = 0;
        public static int requestCount = 0;
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
        
        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                // Write the response info
                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);
            this.SetupConfig();
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
        
        public void Save() => _config.Save();

        public static Koth ScoresFromStorage()
        {
            Koth settings = new Koth();
            try
            {
                if (MyAPIGateway.Utilities.FileExistsInWorldStorage(Filename, typeof(Koth)))
                {
                    Log.Warn("Found Koth Scores in Storage");
                    TextReader reader = MyAPIGateway.Utilities.ReadFileInWorldStorage(Filename, typeof(Koth));
                    string text = reader.ReadToEnd();
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