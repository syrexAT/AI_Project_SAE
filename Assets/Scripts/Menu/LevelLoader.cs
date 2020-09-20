using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;
    //public TextMeshProUGUI progressText;

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
        loadingScreen.SetActive(true);
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);


        while (!operation.isDone)
        {
            //isDone geht nur durch den Loading state (danach kommt aber noch der Activation state)
            //um statt von 0-0.9 von 0-1 zu gehen : 
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("AsyncProgress: " + operation.progress);
            slider.value = progress;
            //progressText.text = progress * 100f + "%";

            yield return null;
        }
    }

}
