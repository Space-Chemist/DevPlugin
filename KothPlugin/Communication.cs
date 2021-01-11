using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI;
using VRage.Game;
using NLog;
using NLog.Fluent;

namespace KothPlugin
{
    public class Communication
    {
        public const ushort ComId = 42511;
        public enum MessageType : byte
        {
            DeleteD = 0,
            runF = 1,
            Message = 2
        }

        

        public static void RegisterHandlers()
        {
            MyAPIGateway.Multiplayer.RegisterMessageHandler(ComId, MessageHandler);
        }

        public static void UnregisterHandlers()
        {
            MyAPIGateway.Multiplayer.UnregisterMessageHandler(ComId, MessageHandler);
        }

        private static void MessageHandler(byte[] bytes)
        {
            try
            {
                var type = (MessageType) bytes[0];

                Log.Info($"Recieved message: {bytes[0]}: {type}");

                var data = new byte[bytes.Length - 1];
                Array.Copy(bytes, 1, data, 0, data.Length);

                switch (type)
                {
                    case MessageType.runF:
                        runF();
                        break;
                    case MessageType.Message:
                        Message();
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                Log.Info($"Error during message handle! {ex}");
            }
        }
        
        [Serializable]
        public struct Package
        {
            public string Message;
        }

        public static void Message()
        {
            string message = "fuck you reload";
            var package = new Package
            {
                Message = message, 
            };
            byte[] data = Encoding.UTF8.GetBytes(MyAPIGateway.Utilities.SerializeToXML(package));
            SendToServer(Communication.MessageType.DeleteD, data, ComId);
        }
        
        public static void SendToServer(MessageType type, byte[] data, ushort comId)
        {
            var newData = new byte[data.Length + 1];
            newData[0] = (byte)type;
            data.CopyTo(newData, 1);
            MyAPIGateway.Utilities.InvokeOnGameThread(() => { MyAPIGateway.Multiplayer.SendMessageToServer(ComId, newData); });
        }
        
        private static void runF()
        {
            
        }
    }
}