using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image LoadScreen;

    public Image loadScreenBar;

    public bool startLoading = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayGame()
    {
        //startLoading = true;
        //SceneManager.LoadSceneAsync(1);
        //var async = SceneManager.LoadSceneAsync(1);
        //Debug.Log("ASYNC PROGRESS: " + async.progress);

    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
