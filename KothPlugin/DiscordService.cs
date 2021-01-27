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
            if (!Koth.Instance.Config.WebHookEnabled)
            {
                return;
            }


            if (string.IsNullOrEmpty(Koth.Instance.Config.WebHookUrl))
            {
                Log.Error("discord Webhook is enabled but the Webhook url is empty? you should fix that!");
                return;
            }

            if (!string.IsNullOrEmpty(Koth.Instance.Config.MessagePrefix))
            {
                msg = $"{Koth.Instance.Config.MessagePrefix} {msg}";
            }

            try
            {
                using (var client = new DiscordWebhookClient(Koth.Instance.Config.WebHookUrl))
                {
                    if (Koth.Instance.Config.EmbedEnabled)
                    {
                        var embed = new EmbedBuilder
                        {
                            Title = Koth.Instance.Config.EmbedTitle,
                            Color = new Color(Convert.ToUInt32(Koth.Instance.Config.Color.Replace("#", ""), 16)),
                            Description = msg
                        };

                        await client.SendMessageAsync("", embeds: new[] {embed.Build()});
                    }
                    
                    else
                    {
                        await client.SendMessageAsync(msg);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "discord Webhook is most likely bad or discord is down");
            }
        }
    }
}