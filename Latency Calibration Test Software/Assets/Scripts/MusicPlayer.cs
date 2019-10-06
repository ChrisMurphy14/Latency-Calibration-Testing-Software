//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        30.09.19
// Date last edited:    30.09.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
// The class used to play a song and display the associated rhythm gameplay input prompts.
public class MusicPlayer : MonoBehaviour
{
    public Song SongToPlay;
    public float InputWindowDuration;
    public float DebugLatencyOffset; // DEBUG 
    // public float VideoLatencyOffset;
    // public float AudioLatencyOffset;

        
    private AudioSource audioSource;       
    private float songPosInSeconds; // The current play position of the song in seconds.    
    private float songPosInBeats; // The current play position of the song in beats.   
    private float songStartTime;  // The time value when the song started playing.    
    private int prevBeatCount; // DEBUG

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = SongToPlay.AudioTrack;
        songStartTime = (float)AudioSettings.dspTime;
    }

    private void Start()
    {              
        audioSource.Play();
    }

    private void Update()
    {
        songPosInSeconds = (float)(AudioSettings.dspTime - songStartTime) - DebugLatencyOffset;
        songPosInBeats = songPosInSeconds / SongToPlay.SecondsPerBeat;

        if ((int)songPosInBeats > prevBeatCount)
            Debug.Log("Beat: " + (int)songPosInBeats);
        //    Camera.main.GetComponent<Camera>().backgroundColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        prevBeatCount = (int)songPosInBeats;
    }
}
