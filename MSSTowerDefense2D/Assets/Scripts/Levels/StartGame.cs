using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{

    public void StartGameButton()
    {
        saveLoadSystem.instance.isLoadingGame = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        
    }

    public void loadGameButton()
    {
        saveLoadSystem.instance.isLoadingGame = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        
    }

}
