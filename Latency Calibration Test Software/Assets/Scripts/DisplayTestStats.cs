//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        14.11.19
// Date last edited:    14.11.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// A script used to fill a UI text element with the recorded test stats take from the Test Manager.
[RequireComponent(typeof(Text))]
public class DisplayTestStats : MonoBehaviour
{
    private Text uiText;

    private void Awake()
    {
        uiText = GetComponent<Text>();
        uiText.text = "";
    }

    private void Start()
    {
        TestManager testManager = GameObject.FindGameObjectWithTag("TestManager").GetComponent<TestManager>();

        uiText.text += "NO LATENCY OFFSET \n";       
        uiText.text += "Note prompts hit percentage: " + RoundToTwoDecimalPlaces(testManager.TestData.NoLatencyOffset.NotePromptsHitPercentage) + '\n';
        uiText.text += "Average input offset from prompts: " + RoundToTwoDecimalPlaces(testManager.TestData.NoLatencyOffset.AverageNotePromptInputOffset) + '\n';
        uiText.text += '\n';

        DisplayStatsForCalibrationTechnique(testManager.TestData.Gameplay, "GAMEPLAY");
        DisplayStatsForCalibrationTechnique(testManager.TestData.BeatMatching, "BEAT MATCHING");
        DisplayStatsForCalibrationTechnique(testManager.TestData.Gameplay, "AUTO CALCULATED");
    }

    private void DisplayStatsForCalibrationTechnique(CalibrationTechniqueTestData testData, string calibrationTechniqueName)
    {
        uiText.text += "Calibration technique: " + calibrationTechniqueName + '\n';
        uiText.text += "Calibration duration: " + RoundToTwoDecimalPlaces(testData.CalibrationDuration) + '\n';
        uiText.text += "Latency offset: " + RoundToTwoDecimalPlaces(testData.LatencyOffset) + '\n';
        uiText.text += "Note prompts hit percentage: " + RoundToTwoDecimalPlaces(testData.NotePromptsHitPercentage) + '\n';
        uiText.text += "Average input offset from prompts: " + RoundToTwoDecimalPlaces(testData.AverageNotePromptInputOffset) + '\n';
        uiText.text += '\n';
    }

    private float RoundToTwoDecimalPlaces(float number)
    {
        return Mathf.Round(number * 100.0f) / 100.0f;
    }
}