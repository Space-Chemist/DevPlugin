using NLog;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace KothPlugin
{
    [Category("koth")]
    public class KothCommands : CommandModule
    {
        private static readonly Logger Log = LogManager.GetLogger("koth");

        public KothPlugin.Koth Plugin
        {
            get
            {
                return (KothPlugin.Koth)this.Context.Plugin;
            }
        }

        [Command("clearscores", "This clears koth scores", "clears koth scores")]
        [Permission(MyPromoteLevel.Admin)]
        public void ClearScores()
        {
            this.Context.Respond("Scores have been Cleared");
            NetworkService.SendPacket("clear");
        }
        
        [Command("refreshpath", "This clears koth scores", "Used when changing Saved files but not relaunching torch")]
        [Permission(MyPromoteLevel.Admin)]
        public void PathRefresher()
        {
            this.Context.Respond("Save Path Refreshed");
            Koth.SetPath();
        }
        
        [Command("testwebhook", "This clears koth scores", "Sends a test message to Webhook")]
        [Permission(MyPromoteLevel.Admin)]
        public void WebHookTest()
        {
            this.Context.Respond("test message sent");
            DiscordService.SendDiscordWebHook("Successful WebHook Test");
        }
    }
}