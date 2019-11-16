//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        05.11.19
// Date last edited:    16.11.19
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
    public float backgroundColorChangeIntervalMultiplier = 1.0f; // The multiplier for amount the background color is changed each time the latency offset is altered.


    private Color initialBackgroundColor;
    private MusicPlayer musicPlayer;

    private void Start()
    {
        initialBackgroundColor = Camera.main.backgroundColor;
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

        Camera.main.backgroundColor = new Color(initialBackgroundColor.r + musicPlayer.LatencyOffset * backgroundColorChangeIntervalMultiplier, 
            initialBackgroundColor.g + musicPlayer.LatencyOffset * backgroundColorChangeIntervalMultiplier,
            initialBackgroundColor.b + musicPlayer.LatencyOffset * backgroundColorChangeIntervalMultiplier);
    }

    private float RoundToTwoDecimalPlaces(float number)
    {
        return Mathf.Round(number * 100.0f) / 100.0f;
    }
}