using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class CharacterTurning : MonoBehaviour
{
    public Sprite front;
    public Sprite back;
    public Sprite forward;
    public IAstarAI path;
    private SpriteRenderer thisRenderer;

    private Vector3 left;
    private Vector3 right;

    void Start()
    {
        thisRenderer = GetComponent<SpriteRenderer>();
        path = GetComponentInParent<IAstarAI>();
        right = new Vector3(-1, 1, 1);
        left = Vector3.one;
    }

    void Update()
    {
        if (path.desiredVelocity.y > 0 && path.desiredVelocity.x > 0)
        {
           thisRenderer.sprite = back;
           transform.localScale = right;
        }
        else if (path.desiredVelocity.y > 0 && path.desiredVelocity.x < 0)
        {
            thisRenderer.sprite = back;
            transform.localScale = left;
        }
        else if (path.desiredVelocity.y < 0 && path.desiredVelocity.x > 0)
        {
            thisRenderer.sprite = forward;
            transform.localScale = right;
        }
        else if (path.desiredVelocity.y < 0 && path.desiredVelocity.x < 0)
        {
            thisRenderer.sprite = forward;
            transform.localScale = left;
        }
        else if (path.desiredVelocity.y < 0 && path.desiredVelocity.x == 0)
        {
            thisRenderer.sprite = front;
            transform.localScale = left;
        }
    }
}
