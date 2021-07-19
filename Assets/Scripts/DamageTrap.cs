using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrap : MonoBehaviour
{
    public int Damage;
    public Vector2 PushForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        Actor actor = collision.GetComponent<Actor>();
		if(actor != null)
		{
            if (actor.team == Team.Player)
            {
                actor.GetDamage(Damage, new Vector2(Mathf.Sign(actor.transform.position.x - transform.position.x) * PushForce.x, PushForce.y),AttackType.Normal);
            }
		}
	}


}
