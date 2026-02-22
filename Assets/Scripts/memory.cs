using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public static GameConfig Instance;

    public int pegCount = 3;
    public int diskCount = 3;

    public GameMode gameMode = GameMode.Normal;

    public enum GameMode
    {
        Normal,
        Weighted,
        Cyclic
    }

    private void Start()
    {
        Debug.Log("GameConfig alive in scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}