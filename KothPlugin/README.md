# Koth Plugin 
> Adds Webpage, API, and Discord Webhook support for King of the Hill mod

![](https://cdn.discordapp.com/attachments/377619690513498133/406183455123177481/OpenSauce.svg)
![](https://cdn.discordapp.com/attachments/330777295952543744/478325842188042241/license.svg)
![](https://forthebadge.com/images/badges/60-percent-of-the-time-works-every-time.svg) ![](https://forthebadge.com/images/badges/built-with-love.svg)
[![Patreon](https://img.shields.io/badge/patreon-donate-green.svg)](https://www.patreon.com/bePatron?u=847269)

This is a Torch plugin for community servers using the the King Of the Hill Mod. It is mean to add functionality to the mod and the ability 
to share score information with the community as well as being able to reset the the scores.

![](header.png)

## Installation

Torch Plugin 

## Development setup

1. place the kothplugin zip in the Plugins folder
2. get the guid from the manifest in the Kothplugin zip. (also can be found at top of readme)
3. In the Torch.cfg place the guid inbetween the <plugins></plugins> tag.
4. launch torch and the plugin can be found in the plugins tab of torch.

##Use instructions

Using the plugin
* webserver enabled turns on the ability to use the API, Webhook, or webpage.
* The different features such as API, Webhook or webpage can each then be enabled seperately via their own checkboxes.
* Host is the IP you wish to host the API or webpage at.
* Port is the Port for the host.
* Webpage Url example "http://localhost:8888/koth"
* API Url example "http://localhost:8888/json"
* WebHook Url is for the discord Webhook Url.
* Message Prefix is the Prefix on the Discord message.
* Color is a hex code color for the webhook.
* EmbedTitle is the Title for the embed in Discord webhook.
* Clear Scores button clears save scores in scores.data 
* Test webhook generates a test message for your webhook set up.
* Refresh Path changes the storage Path without restarting torch.
* update config saves the updates to the configuration of the plugin. 

## Commands 

* !koth clearscores - This clears koth scores
* !koth refreshpath - Used when changing Saved files but not relaunching torch
* !koth testwebhook - Sends a test message to Webhook


## Release History

* 1.0
    * Initial Release
* 1.1.0
    * Fix:`API Support` (Thanks @twistedgrim!)
* 0.1.2
    * FIX: `WebPage Support`
    * ADD: `README`

## Prerequisites

* Requires [Torch](https://torchapi.net/)


## License

This project is licensed under the AGPL-3.0 License - see the [LICENSE](LICENSE) file for details