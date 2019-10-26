//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        09.10.19
// Date last edited:    26.10.19
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

    // Spawns an input prompt which will move towards the hitbox and reach it at the specified time.
    public void SpawnInputPrompt(float timeToReachHitbox)
    {
        Transform inputPrompt = Instantiate(InputPromptPrefab, new Vector3(transform.position.x + InputPromptSpawnPosOffset.x, transform.position.y + InputPromptSpawnPosOffset.y, 0.0f), InputPromptPrefab.rotation);

        NoteInputPrompt inputPromptScript = inputPrompt.GetComponent<NoteInputPrompt>();
        inputPromptScript.InputHitboxPosition = transform.position;
        inputPromptScript.ReachInputHitboxTime = timeToReachHitbox;
    }


    private Collider2D hitboxCollider;
    private SpriteRenderer spriteRenderer;
    private float stateChangeTimer; // A timer which disabled the hitbox from toggling between active and inactive while counting down.

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.isTrigger = true;
        hitboxCollider.enabled = false;

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
