using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public TMP_Dropdown pegDropdown;
    public TMP_Dropdown diskDropdown;
    public TMP_Dropdown modeDropdown;

    public void StartGame()
    {
        if (GameConfig.Instance == null)
        {
            Debug.LogError("GameConfig not found! Make sure it exists in the scene.");
            return;
        }

        // Parse numbers safely
        int pegCount = 3; // default
        int diskCount = 3; // default

        int.TryParse(pegDropdown.options[pegDropdown.value].text, out pegCount);
        int.TryParse(diskDropdown.options[diskDropdown.value].text, out diskCount);

        GameConfig.Instance.pegCount = pegCount;
        GameConfig.Instance.diskCount = diskCount;
        GameConfig.Instance.gameMode = (GameConfig.GameMode)modeDropdown.value;

        SceneManager.LoadScene("GameScene");
    }
}