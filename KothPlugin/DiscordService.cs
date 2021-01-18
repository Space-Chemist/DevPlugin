using System;
using Discord;
using Discord.Webhook;

namespace KothPlugin
{
    public class DiscordService
    {
        public static async void SendDiscordWebHook(string msg)
        {
            const string webhookUrl = "https://discord.com/api/webhooks/800156920270815253/U05QvvZqSUm5iTLmEtLiIFJyGg19JR6rLOb16v6L05qraMypR6kpcQZSeD1NHegLb5Ip"; //TODO Config //Done
            const bool embedEnabled = true; //TODO Config //Done
            const string embedTitle = ""; //TODO Config   //Done
            const string color = "#8b1f5e"; //TODO Config  //Done
            const string messagePrefix = ""; //TODO Config //Done

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