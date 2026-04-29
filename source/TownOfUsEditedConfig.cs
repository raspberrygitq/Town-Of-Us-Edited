using BepInEx.Configuration;

namespace TownOfUsEdited;

public static class TownOfUsEditedConfig
{
    public static ConfigEntry<bool> DeadSeeGhosts { get; set; }
    public static ConfigEntry<bool> SeeSettingNotifier { get; set; }
    public static ConfigEntry<bool> DisableLobbyMusic { get; set; }
    public static ConfigEntry<bool> Force4Columns { get; set; }
    public static ConfigEntry<bool> ShowWelcomeMessage { get; set; }
    public static ConfigEntry<bool> HideDevStatus { get; set; }
    public static ConfigEntry<bool> DontShowCosmeticsInGame { get; set; }

    public static void Bind(ConfigFile config) 
    {
        DeadSeeGhosts = config.Bind("Settings", "Dead See Other Ghosts", true, "Whether you see other dead players ghosts while your dead");
        SeeSettingNotifier = config.Bind("Settings", "See Setting Notifier", true, "Whether you see setting changes in lobby at bottom left");
        DisableLobbyMusic = config.Bind("Settings", "Disable Lobby Music", false, "Whether you want to disable the lobby Music in-game (can be changed in-game with settings)");
        Force4Columns = config.Bind("Settings", "Force 4 Columns", false, "Always display 4 columns in meeting, vitals & shapeshifter menu.");
        ShowWelcomeMessage = config.Bind("Settings", "Show Welcome Message", true, "Show welcome message after creating a lobby.");
        HideDevStatus = config.Bind("Settings", "Hide Special Status", false, "Toggle this to hide your special status when launching if you have one. You will still have access to your special perks.");
        DontShowCosmeticsInGame = config.Bind("Settings", "Dont Show Cosmetics In Game (Client Only)", false, "Don't show any cosmetics in-game (only client-side)");
    }
}
