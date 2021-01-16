using NLog;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using KothPlugin.ModNetworkAPI;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game;
using VRage.Game.ModAPI;
using VRageMath;

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
            
            //Koth.Wiped();
            this.Context.Respond("this needs fixed");
        }

        // [Command("score", "output scores", null)]
        // [Permission(MyPromoteLevel.Admin)]
        // public void Score()
        // {
        //     Koth.ScoresFromStorage();
        //     this.Context.Respond("command ran function");
        // }
    }
}