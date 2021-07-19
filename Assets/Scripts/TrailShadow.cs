using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailShadow : MonoBehaviour
{
    public SpriteRenderer rend;
    public float AlphaVelocity=1f;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rend.color.a > 0)
		{
            var tempColor = rend.color;
            tempColor.a -= Time.deltaTime* AlphaVelocity;
            rend.color = tempColor;
        }
		else
		{
            Destroy(this.gameObject);
		}
    }
}
