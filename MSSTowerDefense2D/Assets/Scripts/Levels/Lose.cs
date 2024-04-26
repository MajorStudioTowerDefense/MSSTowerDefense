using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("NewStartScene");
    }
}
