using NLog;
using Sandbox.ModAPI;
using System.Text;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace KothPlugin
{
    [Category("Koth")]
    public class KothCommands : CommandModule
    {
        private static readonly Logger Log = LogManager.GetLogger("Koth");

        public KothPlugin.Koth Plugin
        {
            get
            {
                return (KothPlugin.Koth)this.Context.Plugin;
            }
        }

        [Command("clearscores", "This clears koth scores", null)]
        [Permission(MyPromoteLevel.Admin)]
        public void ClearScores()
        {
            this.Context.Respond("this has been fixed");
            NetworkService.SendPacket("clear");
        }

    }
}