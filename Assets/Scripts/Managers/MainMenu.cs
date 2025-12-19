using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        // —бросить сохранени€ (PlayerPrefs.DeleteAll)
        SceneManager.LoadScene("GameScene");
    }

    public void Continue()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}