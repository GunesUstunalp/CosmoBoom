using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPopUpWindow : MonoBehaviour
{
    public void SwitchActive()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToNextLevel()
    {
        string currLevelNo = SceneManager.GetActiveScene().name.Replace("Level ", "");
        int nextLevelNo = int.Parse(currLevelNo) + 1;

        SceneManager.LoadScene("Level " + nextLevelNo);
    }
}
