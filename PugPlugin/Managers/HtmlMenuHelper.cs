namespace PugPlugin.Managers
{
	using CounterStrikeSharp.API;
	using CounterStrikeSharp.API.Core;
	using CounterStrikeSharp.API.Modules.Menu;

	namespace PugPlugin.Managers
	{
		public static class HtmlMenuHelper
		{
			public static void PrintHtmlMessagesToAll(BasePlugin basePlugin, IEnumerable<string> messages, Action<CCSPlayerController, ChatMenuOption>? onSelect, bool disabled = false, string? title = null)
			{
				onSelect ??= static (_, _) => { };
				var menu = new CenterHtmlMenu(title ?? messages.First() ?? "", basePlugin);
				menu.MenuOptions.Clear();
				foreach (var message in messages)
				{
					menu.AddMenuOption(message, onSelect: onSelect, disabled);
				}
				foreach (var player in Utilities.GetPlayers())
				{
					//player.PrintToCenterHtml($"<font color='yellow'>{option}</font> <font color='{color}'>{message}</font>");
					MenuManager.OpenCenterHtmlMenu(basePlugin, player, menu);
				}
			}

			public static void PrintHtmlMessageToAll(BasePlugin basePlugin, string message, Action<CCSPlayerController, ChatMenuOption>? onSelect, bool disabled = false, string? title = null)
			{
				onSelect ??= static (_, _) => { };
				var menu = new CenterHtmlMenu(title ?? message, basePlugin);
				menu.MenuOptions.Clear();
				foreach (var player in Utilities.GetPlayers())
				{
					menu.AddMenuOption(message, onSelect: onSelect, disabled);
					//player.PrintToCenterHtml($"<font color='yellow'>{option}</font> <font color='{color}'>{message}</font>");
					MenuManager.OpenCenterHtmlMenu(basePlugin, player, menu);
				}
			}

			public static void PrintHtmlMessageToPlayer(BasePlugin basePlugin, CCSPlayerController player, string message, Action<CCSPlayerController, ChatMenuOption>? onSelect, bool disabled = false, string? title = null)
			{
				onSelect ??= static (_, _) => { };
				var menu = new CenterHtmlMenu(title ?? message, basePlugin);
				menu.MenuOptions.Clear();
				menu.AddMenuOption(message, onSelect: onSelect, disabled);
				MenuManager.OpenCenterHtmlMenu(basePlugin, player, menu);
				//player.PrintToCenterHtml($"<font color='yellow'>{option}</font> <font color='{color}'>{message}</font>");
			}

			public static void PrintHtmlMessagesToPlayer(BasePlugin basePlugin, CCSPlayerController player, IEnumerable<string> messages, Action<CCSPlayerController, ChatMenuOption>? onSelect, bool disabled = false, string? title = null)
			{
				onSelect ??= static (_, _) => { };
				var menu = new CenterHtmlMenu(title ?? messages.First() ?? "", basePlugin);
				menu.MenuOptions.Clear();
				foreach (var message in messages)
				{
					menu.AddMenuOption(message, onSelect: onSelect, disabled);
				}
				MenuManager.OpenCenterHtmlMenu(basePlugin, player, menu);
			}
		}
	}

}
