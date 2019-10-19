//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        09.10.19
// Date last edited:    19.10.19
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
    private bool isSuccessfulInputDestroyCoroutineRunning;
    private float spawnTime; // The time value when the input prompt is activated.
    private float destroyTimer; // The self-destruct timer which starts after the prompt reaches the hitbox.

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

        if (hasReachedHitbox && !isSuccessfulInputDestroyCoroutineRunning)
        {
            destroyTimer += Time.deltaTime;
            if (destroyTimer >= ExistAfterReachingInputHitboxDuration)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "InputHitbox")
            StartCoroutine("SuccessfulInputDestroyCoroutine");
    }

    private IEnumerator SuccessfulInputDestroyCoroutine()
    {
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
