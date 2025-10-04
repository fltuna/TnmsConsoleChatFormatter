using System.Globalization;
using Microsoft.Extensions.Logging;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using TnmsConsoleChatFormatter.Models;
using TnmsPluginFoundation.Extensions.Client;
using TnmsPluginFoundation.Models.Plugin;
using TnmsPluginFoundation.Utils.UI.Chat;

namespace TnmsConsoleChatFormatter.Modules;

public class LocalizedConsoleChatFormatter(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider), IGameListener
{
    public override string PluginModuleName => "LocalizedConsoleChatFormatter";
    public override string ModuleChatPrefix => "TNMSCCF";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    public int ListenerVersion => 1;
    public int ListenerPriority => 1;
    
    private CultureInfo _defaultCulture = CultureInfo.GetCultureInfo("en-US");
    
    private Dictionary<string, MapMessageMapping> _mapMessageMappings = new(StringComparer.OrdinalIgnoreCase);

    protected override void OnInitialize()
    {
        LoadMapMessageMappings();
    }

    private void LoadMapMessageMappings()
    {
        try
        {
            var mapsDirectory = Path.Combine(Plugin.ModuleDirectory, "maps");
            _mapMessageMappings = MapConfigurationParser.LoadAllMapConfigurations(mapsDirectory);
            Logger.LogInformation("Loaded {count} map configurations", _mapMessageMappings.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to load map configurations: {error}", ex.Message);
        }
    }

    public ECommandAction ConsoleSay(string message)
    {
        if (_mapMessageMappings.TryGetValue(Plugin.SharedSystem.GetModSharp().GetMapName() ?? string.Empty , out var msgMapping))
        {
            foreach (var gameClient in SharedSystem.GetModSharp().GetIServer().GetGameClients())
            {
                if (gameClient.IsFakeClient || gameClient.IsHltv)
                    continue;

                var controller = gameClient.GetPlayerController();
            
                if (controller == null)
                    continue;

                var clientLang = Plugin.Localizer.GetClientCulture(gameClient);

                
                if (msgMapping.MessagesToReplace.TryGetValue(message, out var messagesToReplace))
                {
                    if (messagesToReplace.TryGetValue(clientLang.TwoLetterISOLanguageName, out var translatedMsg) && !string.IsNullOrEmpty(translatedMsg))
                    {
                        controller.PrintToChat($"{LocalizeString(gameClient, "Console.ChatPrefix")} {ChatColorUtil.FormatChatMessage(translatedMsg)}");
                        continue;
                    }
                }
                controller.PrintToChat($"{LocalizeString(gameClient, "Console.ChatPrefix")} {message}");
            }
            return ECommandAction.Stopped;
        }

        foreach (var gameClient in SharedSystem.GetModSharp().GetIServer().GetGameClients())
        {
            if (gameClient.IsFakeClient || gameClient.IsHltv)
                continue;

            var controller = gameClient.GetPlayerController();
        
            if (controller == null)
                continue;
        
            controller.PrintToChat($"{LocalizeString(gameClient, "Console.ChatPrefix")} {message}");
        } 

        return ECommandAction.Stopped;
    }
}