using UnityEngine;

public class GameManager
{
    //A basic singleton instance
    public static GameManager Instance { get; } = new GameManager();

    //Store basic information about the application
    public struct ApplicationInfo
    {
        public string GameName;
        public string Version;
        public string CompanyName;
    }

    public ApplicationInfo info;

    GameManager()
    {
        //Set up vsync
        Application.targetFrameRate = Screen.currentResolution.refreshRate;

        //Set up Application Info
        info.GameName = Application.productName;
        info.Version = Application.version;
        info.CompanyName = Application.companyName;
    }
}
