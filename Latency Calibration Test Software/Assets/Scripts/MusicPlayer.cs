//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        30.09.19
// Date last edited:    05.11.19
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
    public Text NotesHitText; // The UI text which shows the number of note prompts that the player has hit.
    public Text NotesMissedText; // The UI text which shows the number of note prompts that the player has missed.
    public Text LatencyOffsetText; // The UI text which shows the current latency offset.
    public Text AverageInputOffsetText; // The UI text which shows the average offset of each player input from the associated prompts.
    public Text SongStartText; // The UI text which is displayed until the player begins the song.
    public Text SongCompletedText; // The UI text which appears to give the player instructions once the song is over.       
    public bool CanReplay = true;
    public float InputWindowDuration;
    public float InputRefractoryDuration; // The duration after the input window is activated for which it cannot be reactivated.
    public float NotePromptTravelDuration; // The duration between each prompt appearing and reaching the associated hitbox.
    public float LatencyOffset; // The offset in seconds between the audio and the gameplay associated with the audio.

    public bool IsSongCompleted
    {
        get { return SongCompletedText.enabled; }
    }

    // The property used to get the average closest offset of each input from the associated prompt arriving at the hitbox (i.e. the average distance in seconds that the player misses getting a 'perfect' input timing) - returns 999.99f if the list of closest input offsets for all input hitboxes are currently empty.
    public float AverageClosestInputOffset
    {
        get
        {
            bool hitboxClosestInputOffsetsAreEmpty = true;
            foreach (NoteInputHitbox inputHitbox in InputHitboxes)
            {
                if (inputHitbox.AverageClosestInputOffset != 999.99f)
                {
                    hitboxClosestInputOffsetsAreEmpty = false;
                    break;
                }
            }

            if (hitboxClosestInputOffsetsAreEmpty)
                return 999.99f;
            else
            {
                float total = 0;
                foreach (NoteInputHitbox inputHitbox in InputHitboxes)
                    total += inputHitbox.AverageClosestInputOffset;
                return total / InputHitboxes.Count;
            }
        }
    }

    // The property used to get the total number of note prompts that have been hit by all of the input hitboxes.
    public uint TotalPromptsHitCount
    {
        get
        {
            uint count = 0;
            foreach (NoteInputHitbox inputHitbox in InputHitboxes)
                count += inputHitbox.PromptsHitCount;

            return count;
        }
    }

    // The property used to get the total number of note prompts that have been missed by all of the input hitboxes.
    public uint TotalPromptsMissedCount
    {
        get
        {
            uint count = 0;
            foreach (NoteInputHitbox inputHitbox in InputHitboxes)
                count += inputHitbox.PromptsMissedCount;

            return count;
        }
    }


    protected AudioSource audioSource;
    protected float songPosInSeconds; // The current play position of the song in seconds.    
    protected float songPosInBeats; // The current play position of the song in beats.   
    protected float songStartTime;  // The time value when the song started playing.    
    protected int prevBeatCount; // DEBUG
    protected uint totalNotesCount; // The total number of note prompts in the assigned song. 

    protected void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.clip = SongToPlay.AudioTrack;        
    }

    protected void Start()
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

        totalNotesCount = (uint)SongToPlay.Notes.Count;

        SongStartText.enabled = true;
        SongCompletedText.enabled = false;
    }

    protected virtual void Update()
    {
        if (!audioSource.isPlaying && Input.GetKeyDown(KeyCode.R))
            StartSong(); 

        if(audioSource.isPlaying)
        {
            songPosInSeconds = (float)(AudioSettings.dspTime - songStartTime) - LatencyOffset;
            songPosInBeats = songPosInSeconds / SongToPlay.SecondsPerBeat;

            if (SongToPlay.Notes.Count > 0)
                UpdatePromptSpawning();

            if (songPosInBeats >= SongToPlay.SongDurationInBeats && !IsSongCompleted)
                EndSong();            

            //if ((int)songPosInBeats > prevBeatCount)
            //    Debug.Log("Beat: " + (int)songPosInBeats); // DEBUG        
            prevBeatCount = (int)songPosInBeats;
        }        
    }

    protected void OnGUI()
    {
        UpdateDisplayedGUITextValue(NotesHitText, TotalPromptsHitCount);
        UpdateDisplayedGUITextValue(NotesMissedText, TotalPromptsMissedCount);
        UpdateDisplayedGUITextValue(LatencyOffsetText, LatencyOffset);
        UpdateDisplayedGUITextValue(AverageInputOffsetText, AverageClosestInputOffset);
    }

    protected void StartSong()
    {
        SongStartText.enabled = false;

        songStartTime = (float)AudioSettings.dspTime;
        audioSource.Play();
    }

    protected virtual void UpdatePromptSpawning()
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

    protected void EndSong()
    {
        SongCompletedText.enabled = true;
    }

    // Updates the float value appended to the end of one of the UI text elements used to display information to the player.
    protected void UpdateDisplayedGUITextValue(Text displayedText, float value)
    {
        if (displayedText)
        {
            displayedText.text = displayedText.text.Remove(displayedText.text.LastIndexOf(' '));
            displayedText.text += " " + value.ToString();
        }
    }
}
