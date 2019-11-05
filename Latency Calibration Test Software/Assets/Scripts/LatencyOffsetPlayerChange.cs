//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        05.11.19
// Date last edited:    05.11.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// A script which allows the player to increase/decrease the latency offset value of the music player using the 'W' and 'S' keys.
[RequireComponent(typeof(MusicPlayer))]
public class LatencyOffsetPlayerChange : MonoBehaviour
{
    public bool DestroyAllPromptsOnOffsetValueChange;
    public float AdjustmentMagnitude = 0.05f; // The amount the latency offset value is adjusted by each time one of the increase/decrease keys is pressed.
    public float UpperBound = 0.5f;
    public float LowerBound = -0.5f;


    private MusicPlayer musicPlayer;

    private void Start()
    {
        musicPlayer = GetComponent<MusicPlayer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && RoundToTwoDecimalPlaces(musicPlayer.LatencyOffset + AdjustmentMagnitude) <= UpperBound)
        {
            musicPlayer.LatencyOffset = RoundToTwoDecimalPlaces(musicPlayer.LatencyOffset + AdjustmentMagnitude);

            if (DestroyAllPromptsOnOffsetValueChange)
            {
                foreach (GameObject prompt in GameObject.FindGameObjectsWithTag("InputPrompt"))
                    Destroy(prompt);
            }
        }
        if (Input.GetKeyDown(KeyCode.S) && RoundToTwoDecimalPlaces(musicPlayer.LatencyOffset - AdjustmentMagnitude) >= LowerBound)
        {
            musicPlayer.LatencyOffset = RoundToTwoDecimalPlaces(musicPlayer.LatencyOffset - AdjustmentMagnitude);

            if (DestroyAllPromptsOnOffsetValueChange)
            {
                foreach (GameObject prompt in GameObject.FindGameObjectsWithTag("InputPrompt"))
                    Destroy(prompt);
            }
        }
    }

    private float RoundToTwoDecimalPlaces(float number)
    {
        return Mathf.Round(number * 100.0f) / 100.0f;
    }
}