using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    int cur = 0;
    public Transform[] waypoints;
    public bool direc = false;
    public float speed = 0.05f;

    public Sprite[] upSprites;
    public Sprite[] downSprites;
    public Sprite[] leftSprites;
    public Sprite[] rightSprites;

    private SpriteRenderer spriteRenderer;
    private bool isMoving = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = leftSprites[0];
    }

    void Update()
    {
        if (!isMoving && transform.position == waypoints[cur].position)
        {
            isMoving = true;
            StartCoroutine(UpdateMovementAndAnimation());
        }
    }

    IEnumerator UpdateMovementAndAnimation()
    {
        while (true)
        {
            if (transform.position == waypoints[cur].position)
            {
                if (direc == false)
                {
                    cur = (cur + 1) % waypoints.Length;
                }
                else
                {
                    cur = (cur + waypoints.Length - 1) % waypoints.Length;
                }
                isMoving = false;
                yield break;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, waypoints[cur].position, speed * Time.deltaTime);
                UpdateDirectionSprite();
                yield return null;
            }
        }
    }

    void UpdateDirectionSprite()
    {
        Vector2 direction = waypoints[(cur + 1) % waypoints.Length].position - waypoints[cur].position;

        if (direction.y > 0)
        {
            spriteRenderer.sprite = upSprites[(int)(Time.time * speed) % upSprites.Length];
        }
        else if (direction.y < 0)
        {
            spriteRenderer.sprite = downSprites[(int)(Time.time * speed) % downSprites.Length];
        }
        else if (direction.x < 0)
        {
            spriteRenderer.sprite = leftSprites[(int)(Time.time * speed) % leftSprites.Length];
        }
        else if (direction.x > 0)
        {
            spriteRenderer.sprite = rightSprites[(int)(Time.time * speed) % rightSprites.Length];
        }
    }
}
