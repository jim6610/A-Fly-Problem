using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelMenu : MonoBehaviour
{
    [SerializeField] private GameObject levelOver;

    public void MainMenu()
    {
        Cursor.lockState = CursorLockMode.Confined;

        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }
}
