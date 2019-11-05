//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        05.11.19
// Date last edited:    05.11.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BeatMatchCalibrationMusicPlayer : MusicPlayer
{
    public float PauseBeforeStartSpawningPrompts = 4.0f; // The duration in seconds between the song starting and beats starting to be spawned.


    protected override void Update()
    {
        if (!audioSource.isPlaying && Input.GetKeyDown(KeyCode.R))
            StartSong();

        if (audioSource.isPlaying)
        {
            songPosInSeconds = (float)(AudioSettings.dspTime - songStartTime) - LatencyOffset;
            songPosInBeats = songPosInSeconds / SongToPlay.SecondsPerBeat;

            UpdatePromptSpawning();

            if (songPosInBeats >= SongToPlay.SongDurationInBeats && !IsSongCompleted)
                EndSong();

            prevBeatCount = (int)songPosInBeats;
        }
    }

    protected override void UpdatePromptSpawning()
    {
        float arrivalPosInSeconds = (nextPromptSpawnBeatCounter) * SongToPlay.SecondsPerBeat; // The song position in seconds at which the next note should arrive at the associated hitbox.
        float spawnPosInSeconds = arrivalPosInSeconds - NotePromptTravelDuration; // The song position in seconds at which the note should be spawned.

        if (songPosInSeconds >= spawnPosInSeconds)
        {
            foreach (NoteInputHitbox hitbox in InputHitboxes)
            {
                if (songPosInSeconds >= PauseBeforeStartSpawningPrompts)
                    hitbox.SpawnInputPrompt((float)AudioSettings.dspTime + NotePromptTravelDuration);
            }

            nextPromptSpawnBeatCounter++;
        }
    }

    private uint nextPromptSpawnBeatCounter; // The incremental beat counter used to spawn a prompt for each hitbox every beat according to the BPM of the song.
}
