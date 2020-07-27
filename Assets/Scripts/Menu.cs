using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void SinglePlayerButton()
    {
        Debug.Log(SceneManager.sceneCount);
        Debug.Log("Build index " + SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(1);
    }
}
