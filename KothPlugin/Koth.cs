using System;
using System.IO;
using System.Windows.Controls;
using System.Xml.Serialization;
using NLog;
using Sandbox;
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
        public static Koth Instance { get; private set; }

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static string KothScorePath = "";
        private Persistent<KothPluginConfig> _config;
        private KothPluginControl _control;
        public static TorchSessionManager SessionManager;
        public KothPluginConfig Config => _config?.Data;

        public UserControl GetControl()
        {
            return _control ?? (_control = new KothPluginControl(this));
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);
            SetupConfig();
            Instance = this;
            SessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            SessionManager.SessionStateChanged += SessionManagerOnSessionStateChanged;
            
        }

        private static void SessionManagerOnSessionStateChanged(ITorchSession session, TorchSessionState newstate)
        {
            switch (newstate)
            {
                case TorchSessionState.Loading:
                    break;
                case TorchSessionState.Loaded:
                    SetPath();
                    NetworkService.NetworkInit();
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


        public static void SetPath()
        {
            if (SessionManager.CurrentSession !=null)
            {
                var kothScoreName = MySandboxGame.ConfigDedicated.LoadWorld;
                KothScorePath = Path.Combine(kothScoreName, @"Storage\2388326362.sbm_koth\Scores.data");
                if (!File.Exists(KothScorePath)) Log.Error("KOTH PLUGIN: NO SOCRE DATA, PLUGIN WILL FAIL");
            }
            else
            {
                Log.Error("No Current session Loaded, cannot set koth score data path");
            }
            
        }

        public static session ScoresFromStorage()
        {
            var serializer = new XmlSerializer(typeof(session));
            using (var reader = new StreamReader(KothScorePath))
            {
                return (session) serializer.Deserialize(reader);
            }
        }
        
        
    }
}