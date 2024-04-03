using UnityEngine;

public class Cell : MonoBehaviour
{
    public Sprite[] sprites;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
