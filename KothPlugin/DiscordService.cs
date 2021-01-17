using System;
using Discord;
using Discord.Webhook;

namespace KothPlugin
{
    public class DiscordService
    {
        public static async void SendDiscordWebHook(string msg)
        {
            const string webhookUrl = "https://discordapp.com/api/"; //TODO Config
            const bool embedEnabled = true; //TODO Config
            const string embedTitle = ""; //TODO Config
            const string color = "#8b1f5e"; //TODO Config
            const string messagePrefix = ""; //TODO Config

            if (!string.IsNullOrEmpty(messagePrefix))
            {
                msg = $"{messagePrefix} {msg}";
            }
            
            if (embedEnabled)
            {
                var embed = new EmbedBuilder
                {
                    Title = embedTitle,
                    Color = new Color(Convert.ToUInt32(color.Replace("#", ""), 16)),
                    Description = msg
                };
                
                using (var client = new DiscordWebhookClient(webhookUrl))
                {
                    await client.SendMessageAsync("", embeds: new[] {embed.Build()});
                }
            }
            else
            {
                using (var client = new DiscordWebhookClient(webhookUrl))
                {
                    await client.SendMessageAsync(msg);
                }
            }
        }
    }
}