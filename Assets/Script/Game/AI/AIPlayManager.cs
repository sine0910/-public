using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIPlayManager : SingletonMonobehaviour<AIPlayManager>
{
    void Start()
    {
        AISendManager.instance.on_awake();
        UIManager.instance.ui_start();
    }

    public void GameQuit()
    {
        SceneManager.LoadScene("HomeScene");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            FirebaseManager.instance.offline();
        }
        else
        {
            FirebaseManager.instance.online();
        }
    }

    private void OnApplicationQuit()
    {
        FirebaseManager.instance.offline();
    }
}
