//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        30.10.19
// Date last edited:    16.11.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// The script for the object which persists between scenes and is used to implement the order of the test, calcalute latency offset values, and record gameplay data.
public class TestManager : MonoBehaviour
{
    private AutoCalibration currentSceneAutoCalibrator; // The latency auto-calibrator gameobject existing in the current scene - null if there isn't one present.   
    private MusicPlayer currentSceneMusicPlayer; // The music player gameobject existing in the current scene - null if there isn't one present.    
    private static TestManager instance; // Used to ensure that if a scene with another TestManager is loaded, the instance that existed in the previous scene persists while the newer version is destroyed.
    private bool readyToLoadNextScene;
    private bool calibrationSceneLoadedOnce; // Used to tell if a calibration scene has already been loaded once, in which case the test manager latency offset value will be carried over.
    private float latencyOffset; // The current latency offset value to be carried between scenes.]
    private float calibrationDurationTimer; // The timer used to record the duration of each calibration process.

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
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (!readyToLoadNextScene)
        {
            if (!currentSceneMusicPlayer && !currentSceneAutoCalibrator || currentSceneMusicPlayer && currentSceneMusicPlayer.IsSongCompleted || currentSceneAutoCalibrator && currentSceneAutoCalibrator.CalibrationCompleted)
                readyToLoadNextScene = true;

            // Allows the player to proceed to the next scene at any time if the following scenes are active.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (SceneManager.GetActiveScene().name == "Calibration - Beat Matching" ||
                    SceneManager.GetActiveScene().name == "Calibration - Gameplay")
                    LoadNextSceneInOrder();
            }
        }

        if (readyToLoadNextScene)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                LoadNextSceneInOrder();
            else if (Input.GetKeyDown(KeyCode.R) && currentSceneMusicPlayer && currentSceneMusicPlayer.CanReplay)
                ReloadCurrentScene();
        }

        if (SceneManager.GetActiveScene().name == "Calibration - Gameplay" || SceneManager.GetActiveScene().name == "Calibration - Beat Matching" ||
            SceneManager.GetActiveScene().name == "Calibration - Auto-Calculated")
            calibrationDurationTimer += Time.deltaTime;
    }

    private void LoadNextSceneInOrder()
    {
        if (readyToLoadNextScene)
            readyToLoadNextScene = false;

        if (currentSceneMusicPlayer)
            latencyOffset = currentSceneMusicPlayer.LatencyOffset;
        else if (currentSceneAutoCalibrator)
            latencyOffset = currentSceneAutoCalibrator.CalculatedLatencyOffset;

        UpdateTestData();

        if (SceneManager.GetActiveScene().name == "Calibration - Gameplay" || SceneManager.GetActiveScene().name == "Calibration - Beat Matching" ||
            SceneManager.GetActiveScene().name == "Calibration - Auto-Calculated")
            calibrationDurationTimer = 0.0f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ReloadCurrentScene()
    {
        if (readyToLoadNextScene && currentSceneMusicPlayer.CanReplay)
        {
            readyToLoadNextScene = false;

            if (currentSceneMusicPlayer)
                latencyOffset = currentSceneMusicPlayer.LatencyOffset;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Updates the test data struct values according to the currently active scene.
    private void UpdateTestData()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "No Latency Offset":
                {
                    TestData.NoLatencyOffset.NotePromptsHitPercentage = (float)currentSceneMusicPlayer.TotalPromptsHitCount / (float)currentSceneMusicPlayer.TotalPromptsCount;
                    TestData.NoLatencyOffset.AverageNotePromptInputOffset = currentSceneMusicPlayer.AverageClosestInputOffset;
                    break;
                }
            case "Calibration - Gameplay":
                {
                    TestData.Gameplay.CalibrationDuration = calibrationDurationTimer;
                    TestData.Gameplay.LatencyOffset = latencyOffset;
                    break;
                }
            case "Gameplay - Gameplay":
                {
                    TestData.Gameplay.NotePromptsHitPercentage = (float)currentSceneMusicPlayer.TotalPromptsHitCount / (float)currentSceneMusicPlayer.TotalPromptsCount;
                    TestData.Gameplay.AverageNotePromptInputOffset = currentSceneMusicPlayer.AverageClosestInputOffset;
                    break;
                }
            case "Calibration - Beat Matching":
                {
                    TestData.BeatMatching.CalibrationDuration = calibrationDurationTimer;
                    TestData.BeatMatching.LatencyOffset = latencyOffset;
                    break;
                }
            case "Gameplay - Beat Matching":
                {
                    TestData.BeatMatching.NotePromptsHitPercentage = (float)currentSceneMusicPlayer.TotalPromptsHitCount / (float)currentSceneMusicPlayer.TotalPromptsCount;
                    TestData.BeatMatching.AverageNotePromptInputOffset = currentSceneMusicPlayer.AverageClosestInputOffset;
                    break;
                }
            case "Calibration - Auto-Calculated":
                {
                    TestData.AutoCalculated.CalibrationDuration = calibrationDurationTimer;
                    TestData.AutoCalculated.LatencyOffset = latencyOffset;
                    break;
                }
            case "Gameplay - Auto-Calculated":
                {
                    TestData.AutoCalculated.NotePromptsHitPercentage = (float)currentSceneMusicPlayer.TotalPromptsHitCount / (float)currentSceneMusicPlayer.TotalPromptsCount;
                    TestData.AutoCalculated.AverageNotePromptInputOffset = currentSceneMusicPlayer.AverageClosestInputOffset;
                    break;
                }
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
                SceneManager.GetActiveScene().name == "Gameplay - Beat Matching" ||
                SceneManager.GetActiveScene().name == "Gameplay - Auto-Calculated")
            {
                currentSceneMusicPlayer.LatencyOffset = latencyOffset;

                if (SceneManager.GetActiveScene().name == "Gameplay - Gameplay" ||
                    SceneManager.GetActiveScene().name == "Gameplay - Beat Matching" ||
                    SceneManager.GetActiveScene().name == "Gameplay - Auto-Calculated")
                    calibrationSceneLoadedOnce = false;
            }

            // If either calibration scene has already been loaded once, toggles the flag to ensure that the current latency offset value is carried over if the scene is reloaded.
            if (SceneManager.GetActiveScene().name == "Calibration - Gameplay" && !calibrationSceneLoadedOnce ||
                SceneManager.GetActiveScene().name == "Calibration - Beat Matching" && !calibrationSceneLoadedOnce)
                calibrationSceneLoadedOnce = true;
        }
        else
            currentSceneMusicPlayer = null;

        if (GameObject.FindGameObjectWithTag("AutoCalibrator"))
            currentSceneAutoCalibrator = GameObject.FindGameObjectWithTag("AutoCalibrator").GetComponent<AutoCalibration>();
    }
}