using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private GameObject playerObject;

    private void Start()
    {
        playerObject = FindObjectOfType<Player>().gameObject;
    }

    // Update camera position in LateUpdate so it updates after everything else each frame.
    // This is the recommended method by Unity.
    private void LateUpdate()
    {
        // Make the camera follow the player
        // Add a z-axis offset to the position so the camera's near clip plane doesn't devour the whole scene
        transform.position = playerObject.transform.position + Vector3.back;
    }
}
