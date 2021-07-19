using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum Team {Player,Enemy,Trap}
public enum AttackType {Normal,Critical}

public class Attack : MonoBehaviour
{
    public Team team;
    public AttackType atkType;
    public Actor actor;
    public int Damage;
    public Vector2 PushForce;
    // Start is called before the first frame update
    void Start()
    {
        actor = GetComponentInParent<Actor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        Actor actor2 = collision.GetComponent<Actor>();
        if (actor2 != null)
        {
			if (actor.team != actor2.team)
			{
                actor2.GetDamage(Damage, new Vector2(Mathf.Sign(actor.transform.localScale.x) * actor.spriteFace * PushForce.x, PushForce.y), atkType);
            }
            
        }

        Interactable interact = collision.GetComponent<Interactable>();
        if(interact != null)
		{
            interact.Interact();
		}
    }
}
