//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        30.10.19
// Date last edited:    30.10.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// The script for the object which persists between scenes and is used to implement the order of the test, calcalute latency offset values, and record gameplay data.
public class TestManager : MonoBehaviour
{
    public List<int> SceneOrder; // The order of the testing scenes to be loaded between the start and finish scene of the test process.


    private static TestManager instance; // Used to ensure that if a scene with another TestManager is loaded, the instance that existed in the previous scene persists while the newer version is destroyed.
    private bool readyToLoadNextScene = false;

    private void Awake()
    {
        if (instance)
            GameObject.Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (!readyToLoadNextScene)
        {
            if (SceneManager.GetActiveScene().name == "Start")
                readyToLoadNextScene = true;
            else if (SceneManager.GetActiveScene().name == "Acclimation" && GameObject.FindWithTag("MusicPlayer").GetComponent<MusicPlayer>().IsSongCompleted)
                readyToLoadNextScene = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && readyToLoadNextScene)
            LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (readyToLoadNextScene)
        {
            readyToLoadNextScene = false;

            if (SceneOrder.Count > 0)
            {
                int nextScene = SceneOrder[0];
                SceneOrder.RemoveAt(0);
                SceneManager.LoadScene(nextScene);
            }
            else
                SceneManager.LoadScene("Finish");
        }
    }
}