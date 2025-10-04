using Microsoft.Extensions.Configuration;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using Sharp.Shared.Objects;
using TnmsConsoleChatFormatter.Modules;
using TnmsPluginFoundation;

namespace TnmsConsoleChatFormatter;

public class TnmsConsoleChatFormatter(
    ISharedSystem sharedSystem,
    string dllPath,
    string sharpPath,
    Version? version,
    IConfiguration coreConfiguration,
    bool hotReload)
    : TnmsPlugin(sharedSystem, dllPath, sharpPath, version, coreConfiguration, hotReload)
{
    public override string DisplayName => "TnmsConsoleChatFormatter";
    public override string DisplayAuthor => "faketuna";
    public override string BaseCfgDirectoryPath => "unused";
    public override string ConVarConfigPath => "";
    public override string PluginPrefix => "[TNMSCCF]";
    public override bool UseTranslationKeyInPluginPrefix => false;


    protected override void TnmsOnPluginLoad(bool hotReload)
    {
        var registered = RegisterModule<LocalizedConsoleChatFormatter>();
        SharedSystem.GetModSharp().InstallGameListener(registered);
    }
}
