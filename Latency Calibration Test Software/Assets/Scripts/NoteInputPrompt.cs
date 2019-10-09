//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        09.10.19
// Date last edited:    09.10.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The class for an input prompt for a note which moves towards an note input hitbox.
[RequireComponent(typeof(Collider2D))]
public class NoteInputPrompt : MonoBehaviour
{
    public Vector2 InputHitboxPosition; // The posititon to which the prompt will travel.
    public float ReachInputHitboxTime; // The time at which the prompt will reach the input hitbox position.
    public float ExistAfterReachingInputHitboxDuration;


    private Vector2 spawnPos;
    private float spawnTime; // The time value when the input prompt is activated.
    private float destroyTimer; // The self-destruct timer which starts after the prompt reaches the hitbox.

    private bool hasReachedHitbox
    {
        get { return (((float)AudioSettings.dspTime - spawnTime) / (ReachInputHitboxTime - spawnTime)) >= 1.0f; }
    }

    private void Start()
    {
        spawnPos = transform.position;
        spawnTime = (float)AudioSettings.dspTime;
    }

    private void Update()
    {
        transform.position = Vector2.LerpUnclamped(spawnPos, InputHitboxPosition, ((float)AudioSettings.dspTime - spawnTime) / (ReachInputHitboxTime - spawnTime)); // Lerps between the spawn position and the hitbox position so that it will arrive at the hitbox at the specified time, then continues travelling in the same direction.

        if(hasReachedHitbox)
        {
            destroyTimer += Time.deltaTime;
            if (destroyTimer >= ExistAfterReachingInputHitboxDuration)
                Destroy(gameObject);
        }
    }
}
