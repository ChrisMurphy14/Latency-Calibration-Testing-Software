//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        06.11.19
// Date last edited:    06.11.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AutoCalibration : MonoBehaviour
{
    public Text TestStartText;
    public Text TestEndText;
    public Text LatencyOffsetText;
    public float BPM;
    public float LatencyOffset;
    public float LatencyAdjustmentMagnitude = 0.05f; // The amount the latency offset value is adjusted by each time one of the increase/decrease keys is pressed.
    public float LatencyUpperBound = 0.5f;
    public float LatencyLowerBound = -0.5f;
    public uint TestLengthInBeats = 64;


    private AudioSource audioSource;
    private bool testRunning;
    private float testPosInSeconds; // The current play position of the test in seconds.    
    private float testPosInBeats; // The current play position of the test in beats.   
    private float testStartTime;  // The time value when the test started.    
    private int prevBeatCount; // The integer value of the most recent beat.

    private void Awake()
    {
        TestStartText.enabled = true;
        TestEndText.enabled = false;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
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
    }

    private void Update()
    {
        if (!testRunning)
        {
            if (Input.GetKeyDown(KeyCode.R))
                StartTest();
        }
        else
        {
            testPosInSeconds = (float)(AudioSettings.dspTime - testStartTime) - LatencyOffset;
            testPosInBeats = testPosInSeconds / (60.0f / BPM);

            if ((int)testPosInBeats > prevBeatCount)
                audioSource.PlayOneShot(audioSource.clip);
            prevBeatCount = (int)testPosInBeats;

            if (testPosInBeats >= TestLengthInBeats)
                EndTest();

            UpdateLatencyOffset();
        }
    }    

    private void UpdateLatencyOffset()
    {
        if (Input.GetKeyDown(KeyCode.W) && RoundToTwoDecimalPlaces(LatencyOffset + LatencyAdjustmentMagnitude) <= LatencyUpperBound)
            LatencyOffset = RoundToTwoDecimalPlaces(LatencyOffset + LatencyAdjustmentMagnitude);

        if (Input.GetKeyDown(KeyCode.S) && RoundToTwoDecimalPlaces(LatencyOffset - LatencyAdjustmentMagnitude) >= LatencyLowerBound)
            LatencyOffset = RoundToTwoDecimalPlaces(LatencyOffset - LatencyAdjustmentMagnitude);
    }

    private float RoundToTwoDecimalPlaces(float number)
    {
        return Mathf.Round(number * 100.0f) / 100.0f;
    }

    private void OnGUI()
    {
        UpdateDisplayedGUITextValue(LatencyOffsetText, LatencyOffset);
    }

    // Updates the float value appended to the end of one of the UI text elements used to display information to the player.
    private void UpdateDisplayedGUITextValue(Text displayedText, float value)
    {
        if (displayedText)
        {
            displayedText.text = displayedText.text.Remove(displayedText.text.LastIndexOf(' '));
            displayedText.text += " " + value.ToString();
        }
    }
}
