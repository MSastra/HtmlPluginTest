using System.Numerics;
using System.Text;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;

namespace ReportSystem;

public class ReportSystem : BasePlugin
{
	public override string ModuleAuthor => "phantom";
	public override string ModuleName => "[Discord] ReportSystem";
	public override string ModuleVersion => "v1.0.1";

	private Config _config = null!;
	//private readonly PersonTargetData?[] _selectedReason = new PersonTargetData?[65];

	public override void Load(bool hotReload)
	{
		_config = LoadConfig();

		var mapsFilePath = Path.Combine(ModuleDirectory, "reasons.txt");
		if (!File.Exists(mapsFilePath))
			File.WriteAllText(mapsFilePath, "");

		//RegisterListener<Listeners.OnClientConnected>(slot => _selectedReason[slot + 1] = new PersonTargetData());
		//RegisterListener<Listeners.OnClientDisconnectPost>(slot => _selectedReason[slot + 1] = null);

		AddCommand("css_report", "", (controller, info) =>
		{
			if (controller == null) return;

			foreach (CCSPlayerController player in Utilities.GetPlayers().Where(player => player is { IsValid: true, Connected: PlayerConnectedState.PlayerConnected }))
			{
				if (player != null && player.IsValid)
				{
					if (AdminManager.PlayerHasPermissions(player, "@css/generic"))
					{
						controller.PrintToChat($" {ChatColors.DarkBlue}Reports {ChatColors.White} | {ChatColors.White} Ver daareporteb roca Admini serverzea!");
						return;
					}
				}
			}

			var reportMenu = new ChatMenu("Airchiet Motamashe");
			reportMenu.MenuOptions.Clear();
			foreach (var player in Utilities.GetPlayers())
			{
				if (player.IsBot || player.PlayerName == controller.PlayerName) continue;

				reportMenu.AddMenuOption($"{player.PlayerName} [{player.Index}]", HandleMenu);
			}

			//MenuManager.OpenChatMenu(controller, reportMenu);
		});

		//AddCommandListener("say", Listener_Say);
		//AddCommandListener("say_team", Listener_Say);
	}

	/*private HookResult Listener_Say(CCSPlayerController? player, CommandInfo commandinfo)
    {
        if (player == null) return HookResult.Continue;

        if (_selectedReason[player.Index] != null && _selectedReason[player.Index]!.IsSelectedReason)
        {
            var msg = GetTextInsideQuotes(commandinfo.ArgString);
            var target = Utilities.GetPlayerFromIndex(_selectedReason[player.Index]!.Target);
            switch (msg)
            {
                case "cancel":
                    _selectedReason[player.Index]!.IsSelectedReason = false;
                    return HookResult.Handled;
                default:
                    Task.Run(() => SendMessageToDiscord(player.PlayerName, player.SteamID.ToString(), target.PlayerName,
                        target.SteamID.ToString(), commandinfo.ArgString));
                    _selectedReason[player.Index]!.IsSelectedReason = false;
                    player.PrintToChat($"[\x0C ReportSystem\x01 ] You have successfully filed a report on player `{target.PlayerName}`");
                    return HookResult.Handled;
            }
        }
        return HookResult.Continue;
    }*/

	private void HandleMenu(CCSPlayerController controller, ChatMenuOption option)
	{
		var parts = option.Text.Split('[', ']');
		var lastPart = parts[^2];
		var numbersOnly = string.Join("", lastPart.Where(char.IsDigit));

		var index = int.Parse(numbersOnly.Trim());
		var reason = File.ReadAllLines(Path.Combine(ModuleDirectory, "reasons.txt"));
		var reasonMenu = new ChatMenu("Airchiet Mizezi");
		reasonMenu.MenuOptions.Clear();

		/*reasonMenu.AddMenuOption($"My reason [{index}]", (playerController, menuOption) =>
        {
            if (_selectedReason[playerController.Index] == null) return;

            _selectedReason[playerController.Index]!.IsSelectedReason = true;
            _selectedReason[playerController.Index]!.Target = index;
            playerController.PrintToChat($"[\x0C ReportSystem\x01 ] Write a reason, or write\x02 cancel\x01 to abort the submission.");
        });*/
		foreach (var a in reason)
		{
			reasonMenu.AddMenuOption($"{a} [{index}]", HandleMenu2);
		}

		MenuManager.OpenChatMenu(controller, reasonMenu);
	}

	private void HandleMenu2(CCSPlayerController controller, ChatMenuOption option)
	{
		var parts = option.Text.Split('[', ']');
		var lastPart = parts[^2];
		var numbersOnly = string.Join("", lastPart.Where(char.IsDigit));

		var target = Utilities.GetPlayerFromIndex(int.Parse(numbersOnly.Trim()));

		Server.NextFrame(() => SendMessageToDiscord(controller.PlayerName, controller.SteamID.ToString(), target.PlayerName,
			target.SteamID.ToString(), parts[0]));

		controller.PrintToChat($" {ChatColors.DarkBlue}Reports {ChatColors.White} | {ChatColors.White} Reporti Gaigzavna!");
	}

	private async void SendMessageToDiscord(string clientName, string clientSteamId, string targetName,
		string targetSteamId, string msg)
	{
		try
		{
			var webhookUrl = GetWebhook();

			if (string.IsNullOrEmpty(webhookUrl)) return;

			var httpClient = new HttpClient();

			if (msg == "") return;

			var payload = new
			{
				embeds = new[]
				{
					new
					{
						title = "",
						description = $"**სერვერი**\n**```fix\n{ConVar.Find("hostname")!.StringValue}```**",
						color = 16711680, //255204255
                        fields = new[]
						{
							new
							{
							   name = "",
								value =
									$"**გამომგზავნი**\n**```{clientName}```**\n**SteamID**\n**```{new SteamID(ulong.Parse(clientSteamId)).SteamId2}```**\n[**`პროფილი`**](https://steamcommunity.com/profiles/{clientSteamId}/)",
								inline = true
							},
							new
							{
								name = "",
								value =
									$"**დამრღვევი**\n**```{targetName}```**\n**SteamID**\n**```{new SteamID(ulong.Parse(targetSteamId)).SteamId2}```**\n[**`პროფილი`**](https://steamcommunity.com/profiles/{targetSteamId}/)",
								inline = true
							},
							new
							{
								name = "",
                                //value = msg,
                                value = $"**მიზეზი**\n**```{msg}```**",
								inline = false
							}
						}
					}
				}
			};

			var json = JsonSerializer.Serialize(payload);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync(webhookUrl, content);

			/*Console.ForegroundColor = response.IsSuccessStatusCode ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(response.IsSuccessStatusCode
                ? "Success"
                : $"Error: {response.StatusCode}");*/
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}

	}
	private Config LoadConfig()
	{
		var configPath = Path.Combine(ModuleDirectory, "settings.json");

		if (!File.Exists(configPath)) return CreateConfig(configPath);

		var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath))!;

		return config;
	}

	private Config CreateConfig(string configPath)
	{
		var config = new Config
		{
			WebhookUrl = ""
		};

		File.WriteAllText(configPath,
			JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

		Console.ForegroundColor = ConsoleColor.DarkGreen;
		Console.WriteLine("[ReportSystem] The configuration was successfully saved to a file: " + configPath);
		Console.ResetColor();

		return config;
	}
	private string GetWebhook()
	{
		return _config.WebhookUrl;
	}

	/*private string GetTextInsideQuotes(string input)
    {
        var startIndex = input.IndexOf('"');
        var endIndex = input.LastIndexOf('"');

        if (startIndex != -1 && endIndex != -1 && startIndex < endIndex)
        {
            return input.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        return string.Empty;
    }*/
}
public class Config
{
	public required string WebhookUrl { get; set; }
}

/*public class PersonTargetData
{
    public int Target { get; set; }
    public bool IsSelectedReason { get; set; }
}*/