using CounterStrikeSharp.API.Core;
using PugPlugin.Config;

namespace PugPlugin.Menu.Setup;

public class Captain1ChoiceMenu : Menu<int>
{
    public Captain1ChoiceMenu(BasePlugin basePlugin) : base(basePlugin)
    {
        SetTitle($"{PugConfig.ChatPrefix} As first Captain you can choose one of the following preferences:");
        AddMenuItem("Pick First Side", 0);
        AddMenuItem("Pick First Teammate", 1);
    }
}