using UnityEngine;
using TMPro;

public class VersionDisplay : MonoBehaviour
{
    private TextMeshProUGUI m_Text;

    private void Awake()
    {
        //Display the game name and version
        m_Text = GetComponent<TextMeshProUGUI>();
        m_Text.text = GameManager.Instance.info.GameName + "\n" + GameManager.Instance.info.Version;
    }
}
