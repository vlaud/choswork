using Commands.Menu;
using System.Collections.Generic;

public class MenuCommandProfile
{
    public Dictionary<MainMenuKeyType, iCommandType<iMainmenuFunctionality>> Commands = new();

    public void Set(MainMenuKeyType key, iCommandType<iMainmenuFunctionality> command)
    {
        Commands[key] = command;
    }

    public iCommandType<iMainmenuFunctionality> Get(MainMenuKeyType key)
    {
        Commands.TryGetValue(key, out var command);
        return command;
    }

    public void Clear()
    {
        Commands.Clear();
    }
}

public static class MenuCommandProfileFactory
{
    public static MenuCommandProfile Create(string sceneName)
    {
        Debug.Log("scd "+sceneName);
        return sceneName switch
        {
            "Title" => CreateTitleProfile(),
            "GameStage" => CreateGameplayProfile(),
            "ClearMessage" => CreateClearMessageProfile(),
            _ => CreateDefaultProfile()

        };
    }
    public static MenuCommandProfile CreateTitleProfile()
    {
        var profile = new MenuCommandProfile();
        profile.Set(MainMenuKeyType.PauseAction, null);
        profile.Set(MainMenuKeyType.UnPauseAction, null);
        profile.Set(MainMenuKeyType.BackToMenu, new BackToMain());
        profile.Set(MainMenuKeyType.BackToOptions, new BackToOptions());
        return profile;
    }

    private static MenuCommandProfile CreateGameplayProfile()
    {
        var profile = new MenuCommandProfile();
        profile.Set(MainMenuKeyType.PauseAction, new Pause());
        profile.Set(MainMenuKeyType.UnPauseAction, new UnPause());
        profile.Set(MainMenuKeyType.BackToMenu, new BackToMain());
        profile.Set(MainMenuKeyType.BackToOptions, new BackToOptions());
        return profile;
    }

    private static MenuCommandProfile CreateClearMessageProfile()
    {
        var profile = new MenuCommandProfile();
        profile.Clear();
        return profile;
    }

    private static MenuCommandProfile CreateDefaultProfile()
    {
        var profile = new MenuCommandProfile();
        // 최소 기본값만 넣거나 로그 출력용
        return profile;
    }
}
