//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        09.10.19
// Date last edited:    30.10.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The class for a hitbox which note input prompts move towards for the player to press the input at the correct time.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NoteInputHitbox : MonoBehaviour
{
    public Sprite ActiveSprite;
    public Sprite InactiveSprite;
    public Transform InputPromptPrefab; // The prefab used to spawn input prompts which move towards and interact with the hitbox.
    public Vector2 InputPromptSpawnPosOffset; // The position offset from the hitbox at which to spawn input prompts.
    public KeyCode ActivateKeycode;
    public float ActivationDuration; // The duration for which the hitbox is active after the input key is pressed.
    public float ActiveRefractoryDuration; // The duration after the hitbox is activated for which the player cannot re-activate it - prevents the player from repeatedly tapping the key instead of timing the input correctly.    

    // The property used to get the average closest offset of each input from the associated prompt arriving at the hitbox (i.e. the average distance in seconds that the player misses getting a 'perfect' input timing) - returns 999.99f if the list of closest input offsets is currently empty.
    public float AverageClosestInputOffset
    {
        get
        {
            if (closestInputOffsets.Count == 0)
                return 999.99f;
            else
            {
                float total = 0.0f;
                foreach (float offset in closestInputOffsets)
                    total += offset;
                return total / closestInputOffsets.Count;
            }
        }
    }

    public uint PromptsHitCount
    {
        get { return promptsHitCount; }
    }

    public uint PromptsMissedCount
    {
        get { return promptsMissedCount; }
    }

    // Spawns an input prompt which will move towards the hitbox and reach it at the specified time.
    public void SpawnInputPrompt(float timeToReachHitbox)
    {
        Transform inputPrompt = Instantiate(InputPromptPrefab, new Vector3(transform.position.x + InputPromptSpawnPosOffset.x, transform.position.y + InputPromptSpawnPosOffset.y, 0.0f), InputPromptPrefab.rotation);

        NoteInputPrompt inputPromptScript = inputPrompt.GetComponent<NoteInputPrompt>();
        inputPromptScript.InputHitbox = this;
        inputPromptScript.InputHitboxPosition = transform.position;
        inputPromptScript.ReachInputHitboxTime = timeToReachHitbox;
    }

    // Call to record that a prompt was destroyed, how, and its closest input offset value.
    public void RegisterPromptDestroyed(bool collidedWithHitbox, float closestInputOffset = 999.99f)
    {
        if (collidedWithHitbox)
            promptsHitCount++;
        else
            promptsMissedCount++;

        if (closestInputOffset != 999.99f)        
            closestInputOffsets.Add(closestInputOffset);        
    }


    private Collider2D hitboxCollider;
    private List<float> closestInputOffsets; // The travel time offset of each prompt in seconds when it's nearest to the activated input hitbox (i.e. how far off was the player input from being perfect). 
    private SpriteRenderer spriteRenderer; private float stateChangeTimer; // A timer which disabled the hitbox from toggling between active and inactive while counting down.    
    private uint promptsHitCount; // The number of note prompts spawned by this hitbox which have been hit by the activated collider.
    private uint promptsMissedCount; // The number of note prompts spawned by this hitbox which have been missed by the collider.

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.isTrigger = true;
        hitboxCollider.enabled = false;

        closestInputOffsets = new List<float>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = InactiveSprite;
    }

    private void Update()
    {
        if (stateChangeTimer > 0.0f)
        {
            stateChangeTimer -= Time.deltaTime;
            if (stateChangeTimer <= 0.0f)
                stateChangeTimer = 0.0f;
        }
        else
        {
            if (!hitboxCollider.enabled && Input.GetKeyDown(ActivateKeycode))
                ActivateHitbox();
            else if (hitboxCollider.enabled)
                DeactivateHitbox();
        }
    }

    private void ActivateHitbox()
    {
        hitboxCollider.enabled = true;
        spriteRenderer.sprite = ActiveSprite;

        stateChangeTimer = ActivationDuration;
    }

    private void DeactivateHitbox()
    {
        hitboxCollider.enabled = false;
        spriteRenderer.sprite = InactiveSprite;

        stateChangeTimer = ActiveRefractoryDuration;
    }
}
