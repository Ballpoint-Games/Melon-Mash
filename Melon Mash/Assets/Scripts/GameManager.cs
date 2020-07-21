using UnityEngine;

public class GameManager
{
    public static GameManager Instance { get; } = new GameManager();

    public struct ApplicationInfo
    {
        public string GameName;
        public string Version;
        public string CompanyName;
    }

    public ApplicationInfo info;

    GameManager()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;

        info.GameName = Application.productName;
        info.Version = Application.version;
        info.CompanyName = Application.companyName;
    }
}
