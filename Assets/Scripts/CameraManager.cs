using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private GameObject playerObject;

    // Shake
    bool isShaking;
    float shakeTimer;
    float shakeStrength;

    private void Start()
    {
        playerObject = FindObjectOfType<Player>().gameObject;
    }

    // Update camera position in LateUpdate so it updates after everything else each frame.
    // This is the recommended method by Unity.
    private void LateUpdate()
    {
        // Calculate shake
        Vector2 shake = Vector2.zero;
        if (isShaking)
        {
            shake = Random.insideUnitCircle * shakeStrength;
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0) isShaking = false;
        }

        // Make the camera follow the player
        // Add a z-axis offset to the position so the camera's near clip plane doesn't devour the whole scene
        // Add shake
        transform.position = playerObject.transform.position + Vector3.back + (Vector3)shake;
    }

    public void Shake(float duration, float strength)
    {
        shakeTimer = duration;
        shakeStrength = strength;
        isShaking = true;
    }
}
