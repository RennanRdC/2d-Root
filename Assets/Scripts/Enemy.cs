using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : Actor
{
	public int cu;
	public Coroutine corou;
	public Transform player;
	public Vector2 leapDistance;
	public float attackDistance;
	public float attackDistanceRage;
	public float attackCD;
	public float attackCDRage;
	public float attackCounter;
	[Header("Rage")]
	public Color RageColor;
	public int rageLife;
	public bool inRage;
	public ParticleSystem rageParticle;
	[Header("Audio")]
	public AudioSource source;
	public AudioClip[] clips;




	// Start is called before the first frame update
	new void Start()
    {
		base.Start();
		source = GetComponent<AudioSource>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		attackCounter = attackCD;
    }

    // Update is called once per frame
    new void Update()
    {
		base.Update();
		attackCounter += Time.deltaTime;

		if(Vector2.Distance(transform.position,player.position) < attackDistance && attackCounter >= attackCD)
		{
			anim.SetTrigger("Attack");
			attackCounter = 0;
		}




		//RAGE
		if (!inRage && currentLife <= rageLife && currentLife > 0)
		{
			inRage = true;
			attackCounter = 0f;
		}

		if (inRage)
		{
			if (!rageParticle.isPlaying)
			{
				rageParticle.Play();
			}
			
			rend.color = RageColor;
			attackCD = attackCDRage;
			attackDistance = attackDistanceRage;
		}






		if (anim.GetBool("FlipEnabled"))
		{
			if(player.transform.position.x < this.transform.position.x && facingRight)
			{
				Flip();
			} else if (player.transform.position.x > this.transform.position.x && !facingRight)
			{
				Flip();
			}
		}


		anim.SetBool("Grounded", grounded);
    }

	public override void GetDamage(int damage, Vector2 pushDistance, AttackType atkType)
	{
		if (invencible)
		{
			return;
		}

		if (currentLife <= 0)
		{
			return;
		}

		if (corou != null)
		{
			StopCoroutine(corou);
		}
		PlaySound(1);
		FloatingText(damage.ToString(), DamageColor, atkType);
		corou = StartCoroutine(FadeTo(5.5f));

		base.GetDamage(damage, pushDistance,atkType);
	}

	public override void Death()
	{
		inRage = false;
		rend.color = Color.white;
		rageParticle.Stop();

		anim.SetBool("Death",true);

	}

	IEnumerator FadeTo(float aTime)
	{

		float ElapsedTime = 0.0f;
		float TotalTime = aTime;
		rend.color = Color.red;

		while (ElapsedTime < TotalTime)
		{
			ElapsedTime += Time.deltaTime;
			rend.color = Color.Lerp(rend.color, Color.white, (ElapsedTime / TotalTime));
			yield return new WaitForEndOfFrame();
		}
		corou = null;
	}


	public void Leap()
	{
		PlaySound(2);
		if (inRage)
		{
			PlaySound(0);
		}

		rigid.velocity = Vector2.zero;
		rigid.AddForce(new Vector2(leapDistance.x * spriteFace * Mathf.Sign(transform.localScale.x), leapDistance.y));
	}

	public void PlaySound(int i)
	{
		source.PlayOneShot(clips[i]);
	}
}
