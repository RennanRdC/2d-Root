using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { JumpGem,DashGem };
public class Item : MonoBehaviour
{
    private SpriteRenderer rend;
    private Collider2D col;
    private Color myColor;
	public ItemType itemType;
    public bool respawn;
    public float respawnTime;
    public float fadeVelocity = 10f;
    public ParticleSystem particles;
    // Start is called before the first frame update
    void Start()
    {
        
        rend = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        myColor = rend.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Collect()
	{
        StartCoroutine(CollectCorou());
	}
    
    IEnumerator CollectCorou()
	{
        particles.Play();
        col.enabled = false;
        rend.enabled = false;

        if (respawn)
		{
            yield return new WaitForSeconds(respawnTime);
            col.enabled = true;
            rend.enabled = true;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * fadeVelocity)
            {
                Color newColor = new Color(myColor.r, myColor.g, myColor.b, Mathf.Lerp(0, 1, t));
                rend.color = newColor;
                yield return null;
            }
        }
        
	}
}
