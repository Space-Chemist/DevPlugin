using System;
using Discord;
using Discord.Webhook;
using NLog;

namespace KothPlugin
{
    public class DiscordService
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static async void SendDiscordWebHook(string msg)
        {
            if (!string.IsNullOrEmpty(Koth.Instance.Config.WebHookUrl) && Koth.Instance.Config.WebHookEnabled)
            {
                if (!string.IsNullOrEmpty(Koth.Instance.Config.MessagePrefix))
                {
                    msg = $"{Koth.Instance.Config.MessagePrefix} {msg}";
                }

                if (Koth.Instance.Config.EmbedEnabled)
                {
                    
                    var embed = new EmbedBuilder
                    {
                        Title = Koth.Instance.Config.EmbedTitle,
                        Color = new Color(Convert.ToUInt32(Koth.Instance.Config.Color.Replace("#", ""), 16)),
                        Description = msg
                    };
                
                    using (var client = new DiscordWebhookClient(Koth.Instance.Config.WebHookUrl))
                    {
                        await client.SendMessageAsync("", embeds: new[] {embed.Build()});
                    }
                }
                else
                {
                    using (var client = new DiscordWebhookClient(Koth.Instance.Config.WebHookUrl))
                    {
                        await client.SendMessageAsync(msg);
                    }
                }
            }
            else
            {
                Log.Warn("WebHook Url empty and is required");
            }
            
        }
    }
}