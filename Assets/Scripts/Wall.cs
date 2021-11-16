using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public int hp = 1;

    private SpriteRenderer spriteRenderer;



    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    public void DamageWall(int loss)
    {
        hp -= loss;

        if (hp <=0)
        {
            gameObject.SetActive(false);
        }
    }
}
