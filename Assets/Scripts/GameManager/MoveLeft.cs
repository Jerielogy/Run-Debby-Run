using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private GameManager gameManager;
    private float currentSpeed;
   
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        currentSpeed = gameManager.worldSpeed;
    }

    void Update()
    {
        currentSpeed = gameManager.worldSpeed;
        transform.Translate(Vector2.left * currentSpeed * Time.deltaTime);
    }
}
