using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetX : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;
    public float xOffset = 12f;

    void Update()
    {
        if (target != null)
        {
            Vector3 newPosition = transform.position;

            newPosition.x = target.position.x + xOffset;

            transform.position = newPosition;
        }
    }
}
