using System.Collections.Frozen;

namespace TnmsConsoleChatFormatter.Models;

public class MapMessageMapping(string mapName, FrozenDictionary<string, FrozenDictionary<string, string>> messagesToReplace)
{
    public string MapName { get; } = mapName;

    // Dic["replace text"]["lang"] = "replaced text"
    public FrozenDictionary<string, FrozenDictionary<string, string>> MessagesToReplace { get; } = messagesToReplace;
}