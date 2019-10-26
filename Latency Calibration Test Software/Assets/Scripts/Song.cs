//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        05.10.19
// Date last edited:    26.10.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// The range of pitch values for each note input.
public enum NotePitch
{
    Low,
    Mid,
    High
}


// A note for the player to input as part of a song.  
[System.Serializable]
public struct SongNote
{   
    public NotePitch Pitch; // The pitch of the note, used to determine the appropriate input key and visual prompt.    
    public float BeatPosInSong; // The position of the note in the song in beats.
}


// A song class containing both the audio track and additional rhythm gameplay data.
public class Song : MonoBehaviour
{   
    public AudioClip AudioTrack;
    public List<SongNote> Notes; // The list containing all of the notes to be input throughout the playtime of the song.
    public float BPM;
    public float SongDurationInBeats; 
    
    public float SecondsPerBeat
    {
        get { return 60.0f / BPM; }
    }
    

    private void Awake()
    {        
        Notes = Notes.OrderBy(x => x.BeatPosInSong).ToList(); // Re-orders the notes according to play position.
    }
}
