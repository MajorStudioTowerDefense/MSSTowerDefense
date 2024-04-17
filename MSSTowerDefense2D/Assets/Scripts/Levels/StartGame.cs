using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{

    public void StartGameButton()
    {
        ES3.Save<bool>("isNewGame", true);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        
    }

    public void loadGameButton()
    {
        ES3.Save<bool>("isNewGame", false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        
    }

}
