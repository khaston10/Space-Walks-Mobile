using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMover : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Vector2 spriteSize;
    private Vector2 moveDir = Vector2.down;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteSize = spriteRenderer.sprite.bounds.size;
        spriteSize[0] = (spriteSize[0] / 2) - 10;
        spriteSize[1] = (spriteSize[1] / 2) - 10;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(moveDir.x * Time.deltaTime, moveDir.y * Time.deltaTime, 0);

        if (!CheckIfInbounds())
        {
            moveDir = PickNewDirection();
        }
    }

    public Vector2 PickNewDirection()
    {
        var newDirection = Vector2.zero;

        if (gameObject.transform.position.x > spriteSize[0])
        {
            if (gameObject.transform.position.y >= 0)
            {
                newDirection = new Vector2(-1, Random.Range(-1f, 0f));
            }

            else newDirection = new Vector2(-1, Random.Range(0f, 1f));

        }

        else if(gameObject.transform.position.x < -spriteSize[0])
        {
            if (gameObject.transform.position.y >= 0)
            {
                newDirection = new Vector2(1, Random.Range(-1f, 0f));
            }

            else newDirection = new Vector2(1, Random.Range(0f, 1f));
        }

        else if (gameObject.transform.position.y > spriteSize[1])
        {
            if(gameObject.transform.position.x >= 0)
            {
                newDirection = new Vector2(Random.Range(-1f, 0f), -1);
            }

            else newDirection = new Vector2(Random.Range(0f, 1f), -1);
        }

        else if (gameObject.transform.position.y < -spriteSize[1])
        {
            if (gameObject.transform.position.x >= 0)
            {
                newDirection = new Vector2(Random.Range(-1f, 0f), 1);
            }

            else newDirection = new Vector2(Random.Range(0f, 1f), 1);
        }

        return newDirection;
    }

    public bool CheckIfInbounds()
    {
        if (gameObject.transform.position.x > spriteSize[0] && moveDir.x > 0)
        {
            return false;
        }

        else if (gameObject.transform.position.x < -spriteSize[0] && moveDir.x < 0)
        {
            return false;
        }

        else if (gameObject.transform.position.y > spriteSize[1] && moveDir.y > 0)
        {
            return false;
        }

        else if (gameObject.transform.position.y < -spriteSize[1] && moveDir.y < 0)
        {
            return false;
        }

        else
        {
            return true;
        }
        
    }
}
