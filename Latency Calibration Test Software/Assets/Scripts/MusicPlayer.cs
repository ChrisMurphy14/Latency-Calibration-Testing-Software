//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        30.09.19
// Date last edited:    26.10.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// The class used to play a song while displaying the associated rhythm gameplay input prompts.
[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public List<NoteInputHitbox> InputHitboxes; // A list containing the hitboxes for each associated note pitch/player input.   
    public Song SongToPlay;
    public Text SongCompletedText; // The UI text which appears to give the player instructions once the song is over.    
    public float InputWindowDuration;
    public float InputRefractoryDuration; // The duration after the input window is activated for which it cannot be reactivated.
    public float NotePromptTravelDuration; // The duration between each prompt appearing and reaching the associated hitbox.
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
        if (InputHitboxes.Count < 1)
            throw new System.Exception("Error: There must be at least one hitbox assigned to the InputHitboxes list.");
        for (int i = 0; i < InputHitboxes.Count; ++i)
        {
            for (int j = 0; j < InputHitboxes.Count; ++j)
            {
                if (InputHitboxes[j].transform != InputHitboxes[i].transform &&
                    InputHitboxes[j].InputPromptPrefab.GetComponent<NoteInputPrompt>().Pitch == InputHitboxes[i].InputPromptPrefab.GetComponent<NoteInputPrompt>().Pitch)
                    throw new System.Exception("Error: There is more than one hitbox in the InputHitboxes list that spawn prompts with the same pitch.");
            }

            InputHitboxes[i].ActivationDuration = InputWindowDuration;
            InputHitboxes[i].ActiveRefractoryDuration = InputRefractoryDuration;
        }

        SongCompletedText.enabled = false;

        audioSource.Play();
    }

    private void Update()
    {
        songPosInSeconds = (float)(AudioSettings.dspTime - songStartTime) - DebugLatencyOffset;
        songPosInBeats = songPosInSeconds / SongToPlay.SecondsPerBeat;

        if (SongToPlay.Notes.Count > 0)
            UpdatePromptSpawning();

        if (songPosInBeats >= SongToPlay.SongDurationInBeats && !SongCompletedText.enabled)
            EndSong();          
        else if(SongCompletedText.enabled && Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);

        if ((int)songPosInBeats > prevBeatCount)
            Debug.Log("Beat: " + (int)songPosInBeats); // DEBUG        
        prevBeatCount = (int)songPosInBeats;           
    }

    private void UpdatePromptSpawning()
    {
        float arrivalPosInSeconds = (SongToPlay.Notes[0].BeatPosInSong) * SongToPlay.SecondsPerBeat; // The song position in seconds at which the next note should arrive at the associated hitbox.
        float spawnPosInSeconds = arrivalPosInSeconds - NotePromptTravelDuration; // The song position in seconds at which the note should be spawned.

        if (songPosInSeconds >= spawnPosInSeconds)
        {
            NoteInputHitbox spawnHitbox = null; // The hitbox which the prompt will travel towards according to the pitch of the prompts that the hitbox spawns.
            foreach (NoteInputHitbox hitbox in InputHitboxes)
            {
                if (hitbox.InputPromptPrefab.GetComponent<NoteInputPrompt>().Pitch == SongToPlay.Notes[0].Pitch)
                {
                    spawnHitbox = hitbox;
                    break;
                }
            }
            spawnHitbox.SpawnInputPrompt((float)AudioSettings.dspTime + NotePromptTravelDuration);

            SongToPlay.Notes.RemoveAt(0);
        }
    }

    private void EndSong()
    {
        SongCompletedText.enabled = true;
    }
}
