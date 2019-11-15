//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        15.11.19
// Date last edited:    15.11.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;


// A struct used to hold the data recorded for one of the latency calibration techniques used.
public struct CalibrationTechniqueTestData
{
    public float CalibrationDuration;
    public float LatencyOffset;
    public float NotePromptsHitPercentage;
    public float AverageNotePromptInputOffset;
}


// The static class used to hold all of the recorded test data.
public static class TestData
{
    public static CalibrationTechniqueTestData NoLatencyOffset;
    public static CalibrationTechniqueTestData Gameplay;
    public static CalibrationTechniqueTestData BeatMatching;
    public static CalibrationTechniqueTestData AutoCalculated;
}