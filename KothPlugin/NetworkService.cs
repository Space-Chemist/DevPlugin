using System;
using System.Text;
using NLog;
using Sandbox.ModAPI;

namespace KothPlugin
{
    public class NetworkService
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void NetworkInit()
        {
            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(8008, HandleIncomingPacket);
        }

        private static void HandleIncomingPacket(ushort comId, byte[] msg, ulong id, bool relible)
        {
            try
            {
                if (!msg.IsNullOrEmpty())
                {
                    var message = Encoding.ASCII.GetString(msg);
                    if (message.Equals("clear")) return;
                    DiscordService.SendDiscordWebHook(message);
                }
            }
            catch (Exception error)
            {
                Log.Error(error, "Network error");
            }
        }


        public static void SendPacket(string data)
        {
            try
            {
                var bytes = Encoding.ASCII.GetBytes(data);
                MyAPIGateway.Multiplayer.SendMessageToServer(8008, bytes);
            }
            catch (Exception error)
            {
                Log.Error(error, "Network error");
            }
        }
    }
}