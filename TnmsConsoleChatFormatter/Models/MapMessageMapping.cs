namespace TnmsConsoleChatFormatter.Models;

public class MapMessageMapping(string mapName, Dictionary<string, Dictionary<string, string>> messagesToReplace)
{
    public string MapName { get; } = mapName;

    // Dic["lang"]["replace text"] = "replaced text"
    public Dictionary<string, Dictionary<string, string>> MessagesToReplace { get; } = messagesToReplace;
}