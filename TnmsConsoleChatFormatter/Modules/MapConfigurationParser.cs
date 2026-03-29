using System.Collections.Frozen;
using System.Text.Json;
using TnmsConsoleChatFormatter.Models;
using TnmsPluginFoundation.Utils.UI.Chat;

namespace TnmsConsoleChatFormatter.Modules;

public class MapConfigurationParser(string configFilePath)
{
    public static FrozenDictionary<string, MapMessageMapping> LoadAllMapConfigurations(string mapsDirectory)
    {
        var mapMessageMappings = new Dictionary<string, MapMessageMapping>();

        if (!Directory.Exists(mapsDirectory))
        {
            return mapMessageMappings.ToFrozenDictionary();
        }

        var jsonFiles = Directory.GetFiles(mapsDirectory, "*.json");

        foreach (var jsonFile in jsonFiles)
        {
            try
            {
                var parser = new MapConfigurationParser(jsonFile);
                var mapping = parser.Parse();
                mapMessageMappings[mapping.MapName] = mapping;
            }
            catch (Exception)
            {
                // Ignore invalid files
            }
        }

        return mapMessageMappings.ToFrozenDictionary();
    }

    private MapMessageMapping Parse()
    {
        if (!File.Exists(configFilePath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configFilePath}");
        }

        var jsonContent = File.ReadAllText(configFilePath);
        var configData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);

        if (configData == null)
        {
            throw new InvalidOperationException("Failed to parse configuration file");
        }

        foreach (var (key, dictionary) in configData)
        {
            foreach (var (s, value) in dictionary)
            {
                dictionary[s] = ChatColorUtil.FormatChatMessage(value);
            }
        }

        var mapName = Path.GetFileNameWithoutExtension(configFilePath);
        var frozenMessages = configData.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenDictionary()
        );
        return new MapMessageMapping(mapName, frozenMessages);
    }
}