using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIPlayManager : SingletonMonobehaviour<AIPlayManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
