//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        06.11.19
// Date last edited:    14.11.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// A script used to run the latency auto-calibration procedure in which the player presses a key in time to a constant beat and the latency offset value is calculated from the results. 
[RequireComponent(typeof(AudioSource))]
public class AutoCalibration : MonoBehaviour
{
    public AudioClip IntroBeat; // The sound effect to play for each intro beat.
    public AudioClip TestBeat; // The sound effect to play for each test beat.
    public Text TestStartText;
    public Text TestCountdownText;
    public Text TestEndText;    
    public KeyCode InputKey; // The input key for the player to press in time to the beat.
    public float BPM;
    public uint IntroBeatCount = 9; // The number of beats to play before the player inputs begin being recorded.
    public uint TestLengthInBeats = 64; // The number of beats the player must tap along to in order to calculate the appropriate latency offset.

    public bool CalibrationCompleted
    {
        get { return calibrationCompleted; }
    }

    public float CalculatedLatencyOffset
    {
        get { return CalculateAverageOffset(); }
    }


    private AudioSource audioSource;
    private List<float> beatTimes; // Stores the time of every beat throughout the test.
    private List<float> inputTimes; // Stores the time of every input of the specified keycode so each can be compared against the associated beat time to get the offset.
    private bool testRunning;
    private bool calibrationCompleted;
    private float testPosInSeconds; // The current play position of the test in seconds.    
    private float testPosInBeats; // The current play position of the test in beats.   
    private float testStartTime;  // The time value when the test started.    
    private int prevBeatCount; // The integer value of the most recent beat.

    // Once the test has finished, returns the average offset between each player input time and the associated beat time.
    private float CalculateAverageOffset()
    {
        if (inputTimes.Count == beatTimes.Count - 1) 
        {
            float combinedOffsets = 0.0f;
            for (int i = 0; i < inputTimes.Count; ++i)
                combinedOffsets += inputTimes[i] - beatTimes[i];

            return combinedOffsets / inputTimes.Count;
        }
        else
            return 0.0f;
    }

    private void Awake()
    {
        TestStartText.enabled = true;
        TestEndText.enabled = false;

        audioSource = GetComponent<AudioSource>();
        if (IntroBeatCount > 0)
            audioSource.clip = IntroBeat;
        audioSource.loop = false;

        beatTimes = new List<float>();
        inputTimes = new List<float>();
    }

    private void StartTest()
    {
        TestStartText.enabled = false;
        testStartTime = (float)AudioSettings.dspTime;
        testRunning = true;
    }

    private void EndTest()
    {
        TestEndText.enabled = true;
        testRunning = false;

        if (inputTimes.Count != beatTimes.Count - 1) // beatTimes.Count - 1 is used because the final beat is disregarded due to the player input likely being afterwards when the test is already 'completed'.
        {
            TestEndText.text = "Error - the number of inputs (" + inputTimes.Count.ToString() + ") must be equal to the number of beats (" + (beatTimes.Count - 1).ToString() + ") in order for the latency offset to be calculated accurately. \n Press R to restart the calibration process.";
        }
        else
        {
            TestEndText.text = "Calibration complete - the calculated latency offset value is " + CalculatedLatencyOffset + "\n" +
                "Press Spacebar to continue \n" +
                "Press R to restart the calibration process";

            calibrationCompleted = true;
        }
    }

    private void Update()
    {
        if (!testRunning)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (TestStartText.enabled)
                    StartTest();
                else if (TestEndText.enabled)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else        
            UpdateTestProcess();        
    }

    private void UpdateTestProcess()
    {
        testPosInSeconds = (float)(AudioSettings.dspTime - testStartTime);
        testPosInBeats = testPosInSeconds / (60.0f / BPM);

        if (testPosInBeats >= IntroBeatCount)
        {
            if (audioSource.clip != TestBeat)
                audioSource.clip = TestBeat;

            if (Input.GetKeyDown(InputKey))
                inputTimes.Add((float)AudioSettings.dspTime);
        }

        if ((int)testPosInBeats > prevBeatCount)
        {
            audioSource.PlayOneShot(audioSource.clip);

            if (audioSource.clip == TestBeat)
                beatTimes.Add((float)AudioSettings.dspTime);
        }
        prevBeatCount = (int)testPosInBeats;

        if (testPosInBeats >= IntroBeatCount + TestLengthInBeats)
            EndTest();
    }

    private void OnGUI()
    {
        if (testRunning && (int)testPosInBeats < IntroBeatCount)
            TestCountdownText.text = ((int)testPosInBeats).ToString();
        else
            TestCountdownText.text = "";
    }
}
