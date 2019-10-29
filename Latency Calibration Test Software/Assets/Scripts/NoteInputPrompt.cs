//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        09.10.19
// Date last edited:    29.10.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The class for an input prompt for a note which moves towards an note input hitbox.
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class NoteInputPrompt : MonoBehaviour
{
    public AudioClip SuccessfulInputSound;
    public NoteInputHitbox InputHitbox;
    public Vector2 InputHitboxPosition; // The posititon to which the prompt will travel.
    public NotePitch Pitch;
    public bool SuccessfulInputStopPromptMovement;
    public float ReachInputHitboxTime; // The time at which the prompt will reach the input hitbox position.
    public float ExistAfterReachingInputHitboxDuration; // The duration which the prompt exists after passing the hitbox if no input was entered at the same time.
    public float ExistAfterSuccessfuInputDuration; // The duration which the prompt exists after passing the hitbox if the correct input was entered at the same time.
    public float SuccessfulInputScaleMultiplier; // The amount the prompt sprite is scaled after a successful input.
    public float SuccessfulInputScreenShakeDuration;
    public float SuccessfulInputScreenShakeMagnitude;

    private SpriteRenderer spriteRenderer;
    private Vector2 spawnPos;
    private bool wasInputHitboxActivePreviousFrame;
    private bool isSuccessfulInputDestroyCoroutineRunning;
    private float spawnTime; // The time value when the input prompt is activated.
    private float destroyTimer; // The self-destruct timer which starts after the prompt reaches the hitbox.
    private float closestInputOffset = 999.99f; // The travel time offset of the prompt in seconds when it's nearest to the activated input hitbox (i.e. how far off was the player input from being perfect) - has a value of 999.99f if the hitbox hasn't been activated while the prompt has been active. 

    private bool hasReachedHitbox
    {
        get { return (((float)AudioSettings.dspTime - spawnTime) / (ReachInputHitboxTime - spawnTime)) >= 1.0f; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spawnPos = transform.position;
        spawnTime = (float)AudioSettings.dspTime;
    }

    private void Update()
    {
        if (!(isSuccessfulInputDestroyCoroutineRunning && SuccessfulInputStopPromptMovement))
            transform.position = Vector2.LerpUnclamped(spawnPos, InputHitboxPosition, ((float)AudioSettings.dspTime - spawnTime) / (ReachInputHitboxTime - spawnTime)); // Lerps between the spawn position and the hitbox position so that it will arrive at the hitbox at the specified time, then continues travelling in the same direction.

        if (!wasInputHitboxActivePreviousFrame && InputHitbox.GetComponent<Collider2D>().enabled)
        {
            float inputOffset = (float)AudioSettings.dspTime - ReachInputHitboxTime;
            if (Mathf.Abs(inputOffset) < Mathf.Abs(closestInputOffset))
                closestInputOffset = inputOffset;
        }

        if (hasReachedHitbox && !isSuccessfulInputDestroyCoroutineRunning)
        {
            destroyTimer += Time.deltaTime;
            if (destroyTimer >= ExistAfterReachingInputHitboxDuration)
            {
                InputHitbox.RegisterPromptDestroyed(false, closestInputOffset);
                Destroy(gameObject);
            }
        }
    }

    private void LateUpdate()
    {
        wasInputHitboxActivePreviousFrame = InputHitbox.GetComponent<Collider2D>().enabled;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<NoteInputHitbox>() == InputHitbox)
            StartCoroutine("SuccessfulInputDestroyCoroutine");
    }

    private IEnumerator SuccessfulInputDestroyCoroutine()
    {
        InputHitbox.RegisterPromptDestroyed(true, closestInputOffset);

        GetComponent<Collider2D>().enabled = false;

        Camera.main.GetComponent<Shake2D>().Shake(SuccessfulInputScreenShakeDuration, SuccessfulInputScreenShakeMagnitude);
        if (SuccessfulInputSound)
            GetComponent<AudioSource>().PlayOneShot(SuccessfulInputSound);

        if (ExistAfterSuccessfuInputDuration > 0.0f)
        {
            isSuccessfulInputDestroyCoroutineRunning = true;

            Vector3 initialScale = transform.localScale;
            Color initialColor = spriteRenderer.color;
            float timer = 0.0f;
            while (timer < ExistAfterSuccessfuInputDuration)
            {
                float lerpVal = timer / ExistAfterSuccessfuInputDuration;
                transform.localScale = Vector3.Lerp(initialScale, initialScale * SuccessfulInputScaleMultiplier, lerpVal);
                spriteRenderer.color = Color.Lerp(initialColor, new Color(initialColor.r, initialColor.g, initialColor.b, 0.0f), lerpVal);

                timer += Time.deltaTime;
                yield return null;
            }

            isSuccessfulInputDestroyCoroutineRunning = false;
        }

        Destroy(gameObject);
    }
}
