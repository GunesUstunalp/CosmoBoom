using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    public void GoToLevelByNumber(int num)
    {
        SceneManager.LoadScene("Level " + num);
    }
}
