using TnmsPluginFoundation.Models.Plugin;
using System.Text.Json;
using TnmsConsoleChatFormatter.Models;

namespace TnmsConsoleChatFormatter.Modules;

public class MapConfigurationParser(string configFilePath)
{
    public static Dictionary<string, MapMessageMapping> LoadAllMapConfigurations(string mapsDirectory)
    {
        var mapMessageMappings = new Dictionary<string, MapMessageMapping>();
        
        if (!Directory.Exists(mapsDirectory))
        {
            return mapMessageMappings;
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
        
        return mapMessageMappings;
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

        var mapName = Path.GetFileNameWithoutExtension(configFilePath);
        return new MapMessageMapping(mapName, configData);
    }
}