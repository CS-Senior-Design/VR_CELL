using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayOutside ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);   // LoadScene() could be swapped for LoadSceneAsync() to avoid pausing while loading
    }

    public void PlayInside ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);   // LoadScene() could be swapped for LoadSceneAsync() to avoid pausing while loading
    }
    public void QuitGame ()
    {
        Application.Quit();
    }
}