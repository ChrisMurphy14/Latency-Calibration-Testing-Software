//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        14.11.19
// Date last edited:    15.11.19
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
        DisplayAllStats();
    }

    private void DisplayAllStats()
    {
        uiText.text += "Calibration type: NO LATENCY OFFSET \n";
        uiText.text += "Note prompts hit percentage: " + RoundToTwoDecimalPlaces(TestData.NoLatencyOffset.NotePromptsHitPercentage).ToString() + '\n';
        uiText.text += "Average input offset from prompts: " + RoundToTwoDecimalPlaces(TestData.NoLatencyOffset.AverageNotePromptInputOffset).ToString() + '\n';
        uiText.text += '\n';

        DisplayIndividualTestStats(TestData.Gameplay, "GAMEPLAY");
        DisplayIndividualTestStats(TestData.BeatMatching, "BEAT MATCHING");
        DisplayIndividualTestStats(TestData.AutoCalculated, "AUTO-CALCULATED");
    }

    private void DisplayIndividualTestStats(CalibrationTechniqueTestData testData, string calibrationTecnique)
    {
        uiText.text += "Calibration type: " + calibrationTecnique + '\n';
        uiText.text += "Calibration duration: " + RoundToTwoDecimalPlaces(testData.CalibrationDuration).ToString() + '\n';
        uiText.text += "Latency offset: " + RoundToTwoDecimalPlaces(testData.LatencyOffset).ToString() + '\n';
        uiText.text += "Note prompts hit percentage: " + RoundToTwoDecimalPlaces(testData.NotePromptsHitPercentage).ToString() + '\n';
        uiText.text += "Average input offset from prompts: " + RoundToTwoDecimalPlaces(testData.AverageNotePromptInputOffset).ToString() + '\n';
        uiText.text += '\n';
    }

    private float RoundToTwoDecimalPlaces(float number)
    {
        return Mathf.Round(number * 100.0f) / 100.0f;
    }
}