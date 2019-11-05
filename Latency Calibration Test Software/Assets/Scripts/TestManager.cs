﻿//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        30.10.19
// Date last edited:    05.11.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// The script for the object which persists between scenes and is used to implement the order of the test, calcalute latency offset values, and record gameplay data.
public class TestManager : MonoBehaviour
{
    //public List<int> SceneOrder; // The order of the testing scenes to be loaded between the start and finish scene of the test process.


    private MusicPlayer currentSceneMusicPlayer;
    private static TestManager instance; // Used to ensure that if a scene with another TestManager is loaded, the instance that existed in the previous scene persists while the newer version is destroyed.
    private bool readyToLoadScene = false;
    private bool calibrationSceneLoadedOnce;
    private float latencyOffset; // The current latency offset value to be carried between scenes.

    private void Awake()
    {
        if (instance)
            GameObject.Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void Update()
    {
        if (!readyToLoadScene)
        {
            if (!currentSceneMusicPlayer || currentSceneMusicPlayer && currentSceneMusicPlayer.IsSongCompleted)
                readyToLoadScene = true;
        }

        if (readyToLoadScene)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                LoadNextSceneInOrder();
            else if (Input.GetKeyDown(KeyCode.R) && currentSceneMusicPlayer.CanReplay)
                ReloadCurrentScene();
        }
    }

    private void LoadNextSceneInOrder()
    {
        if (readyToLoadScene)
        {
            readyToLoadScene = false;

            if (currentSceneMusicPlayer)
            {
                latencyOffset = currentSceneMusicPlayer.LatencyOffset;
                Debug.Log("Latency offset set to: " + latencyOffset);
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void ReloadCurrentScene()
    {
        if (readyToLoadScene && currentSceneMusicPlayer.CanReplay)
        {
            readyToLoadScene = false;

            if (currentSceneMusicPlayer)
                latencyOffset = currentSceneMusicPlayer.LatencyOffset;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameObject.FindGameObjectWithTag("MusicPlayer"))
        {
            currentSceneMusicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<MusicPlayer>();

            

            if (SceneManager.GetActiveScene().name == "Calibration - Gameplay" && calibrationSceneLoadedOnce ||
                SceneManager.GetActiveScene().name == "Gameplay - Gameplay" ||
                SceneManager.GetActiveScene().name == "Calibration - Beat Matching" && calibrationSceneLoadedOnce ||
                SceneManager.GetActiveScene().name == "Gameplay - Beat Matching")
            {
                currentSceneMusicPlayer.LatencyOffset = latencyOffset;

                if (SceneManager.GetActiveScene().name == "Gameplay - Gameplay" ||
                    SceneManager.GetActiveScene().name == "Gameplay - Beat Matching")
                    calibrationSceneLoadedOnce = false;
            }

            if (SceneManager.GetActiveScene().name == "Calibration - Gameplay" && !calibrationSceneLoadedOnce ||
                SceneManager.GetActiveScene().name == "Calibration - Beat Matching" && !calibrationSceneLoadedOnce)
                calibrationSceneLoadedOnce = true;
        }
        else
            currentSceneMusicPlayer = null;
    }
}