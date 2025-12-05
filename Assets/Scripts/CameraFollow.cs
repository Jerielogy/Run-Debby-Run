using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public Transform playerToFollow;
    public float xOffset = 6.5f;

    private void LateUpdate()
    {
        if (playerToFollow != null)
        {
            Vector3 newPosition = transform.position;

            newPosition.x = playerToFollow.position.x + xOffset;

            transform.position = newPosition;
        }
    }

}
