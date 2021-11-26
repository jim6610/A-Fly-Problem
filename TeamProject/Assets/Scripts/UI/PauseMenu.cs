using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    
    public static bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        pauseUI.SetActive(isPaused);
        
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.Confined;
        isPaused = true;
        pauseUI.SetActive(isPaused);

        Time.timeScale = 0;
    }

    public void MainMenu()
    {
        Cursor.lockState = CursorLockMode.Confined;
        
        Time.timeScale = 1f;
        
        isPaused = false;
        
        SceneManager.LoadScene(0);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
