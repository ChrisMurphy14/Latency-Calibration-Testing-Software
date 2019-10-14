//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        14.10.19
// Date last edited:    14.10.19
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shake2D : MonoBehaviour
{
    public void Shake(float duration, float magnitude)
    {
        if (isCoroutineRunning || duration <= 0.0f || magnitude <= 0.0f)
            return;

        this.duration = duration;
        this.magnitude = magnitude;
        StartCoroutine("ShakeCoroutine");        
    }


    private bool isCoroutineRunning;
    private float duration;
    private float magnitude;

    private IEnumerator ShakeCoroutine()
    {
        isCoroutineRunning = true;

        Vector3 initialPosition = transform.position;
        float timer = 0.0f;
        while (timer < duration)
        {
            if (transform.position == initialPosition)
            {
                Vector3 direction = Vector3.zero;
                while (direction == Vector3.zero)
                {
                    direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
                }
                transform.position += direction.normalized * magnitude;           
            }
            else
                transform.position = initialPosition;

            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = initialPosition;

        isCoroutineRunning = false;
    }
}