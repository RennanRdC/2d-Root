using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerControl : Actor
{
	[Header("Components")]
	public AudioSource footStepsSource;
	public AudioSource SFXsource;
	private Coroutine reload;
	[Header("Particles")]
	public ParticleSystem doubleJumpParticle;
	public ParticleSystem wallSlide;
	public ParticleSystem dashParticle;
	public GameObject trailPrefab;

	[Header("Parameters")]
	[ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)] public Color dashTrailColor;
	public Color doubleJumpTrailColor;
	public float velocity = 2f;
	public float jumpForce = 180;
	public float dashForce = 150;
	public Vector2 attack1Force;
	public Vector2 attackCriticalForce;
	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;
	public bool doubleJump;
	public bool airDash;
	private float XInput;
	private Coroutine trailCoroutine;
	private Coroutine doubleJumpCoroutine;
	private Coroutine wallFlipJumpCoroutine;
	[Header("WallJump")]
	[Range(0.00f, 1.00f)]
	public float wallSlideSpeed;
	public float wallJumpXForce;
	public float wallJumpYForce;
	public float wallJumpTime;
	public bool doingWallJump;
	public bool wallJumpSafeTime;
	public bool wallSliding;
	private bool wallSlidingCheck;
	private bool wallSlideCanJump;
	[Header("Updates")]
	public float jumpPressRemember = 0f;
	public float jumpPressRememberTime = 0.2f;
	public bool checkerGround;
	public bool canCoyote;
	public float coyoteTime = 0.2f;
	private Coroutine coyoteCorou;
	[Header("Audio")]
	public ScriptableGroundSounds groundSounds;
	public AudioClip[] clips;








	// Start is called before the first frame update
	new void Start()
    {
		base.Start();
		doubleJump = false;
    }

    // Update is called once per frame
    protected new void Update()
    {
		base.Update();



		XInput = Input.GetAxisRaw("Horizontal");
		wallSliding = touchingWall  && XInput == Mathf.Sign(transform.localScale.x) && !anim.GetBool("OnAttackAnimation");

		if (!wallSliding && wallSlidingCheck && !doingWallJump)
		{
			if(wallFlipJumpCoroutine != null)
			{
				StopCoroutine(wallFlipJumpCoroutine);
			}

			wallFlipJumpCoroutine = StartCoroutine(WallFlipJump());
		}

		if (anim.GetBool("NoFall"))
		{
			rigid.velocity = new Vector2(rigid.velocity.x, 0);
		}

		if (!anim.GetBool("IsDashing"))
		{
			if(trailCoroutine != null)
			{
				StopCoroutine(trailCoroutine);
			}

			dashParticle.Stop();
			invencibleFrame = false;
			
		}

		//Setar variavel para falso em caso de input ou grounded
		if ((grounded || XInput != 0) && wallJumpSafeTime == false)
		{
			doingWallJump = false;
		}

		//Controle do player
		if (!anim.GetBool("CantMove") && !anim.GetBool("Damaged"))
		{
			if(!(grounded && anim.GetBool("CantMoveGround"))) { 
				if (!doingWallJump && !touchingWall && !anim.GetBool("OnDamageAnimation"))
				{
					rigid.velocity = new Vector2(velocity * XInput, rigid.velocity.y);
				}

				//Adicionar força em caso de input apos o wall jump
				if (wallJumpSafeTime && XInput != 0)
				{
					if (Mathf.Sign(rigid.velocity.x) == -XInput)
					{
						rigid.AddForce(new Vector2(XInput * 2.5f, 0));
					}
				}

				//Pular mais alto com input
				if (rigid.velocity.y < 0)
				{
					rigid.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
				}
				else if (rigid.velocity.y > 0 && !Input.GetButton("Jump"))
				{
					rigid.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
				}
			}
		}

		//Reset air Jump double jump
		if (grounded || wallSliding)
		{
			airDash = true;
			doubleJump = false;
		}

		//Controle de flip
		if(facingRight && XInput < 0)
		{
			Flip();

		}else if (!facingRight && XInput > 0)
		{
			Flip();
		}

		//Controle do wallSlide
		if (wallSliding)
		{
			rigid.velocity = new Vector2(rigid.velocity.x,wallSlideSpeed);
			if (!wallSlide.isPlaying && !grounded)
			{
				wallSlide.Play();
			}

			if (grounded){
				wallSlide.Stop();
			}
		}
		else
		{
			if (wallSlide.isPlaying)
			{
				wallSlide.Stop();
			}
		}

		jumpPressRemember -= Time.deltaTime;
		if (Input.GetButtonDown("Jump") && !anim.GetBool("Damaged") && !anim.GetBool("OnDamageAnimation"))
		{
			jumpPressRemember = jumpPressRememberTime;
		}

		if(!grounded && checkerGround && rigid.velocity.y < 0)
		{
			if(coyoteCorou != null)
			{
				StopCoroutine(coyoteCorou);
			}
			coyoteCorou = StartCoroutine(Coyote());
		}

		//Input de jump
		if (jumpPressRemember > 0 /*&& !anim.GetBool("IsDashing")*/)
		{
			if ((grounded || canCoyote) && !wallSliding)
			{
				Jump();
				PlayDust();
				jumpPressRemember = 0;
				
			}
			else if(wallSliding){
				WallJump();
				jumpPressRemember = 0;
			}else if (wallSlideCanJump)
			{
				WallSlideJump();
				jumpPressRemember = 0;
			}
			else
			{
				if (doubleJump)
				{
					Jump();
					doubleJumpParticle.Play();
					doubleJumpCoroutine = StartCoroutine(TrailShadow(doubleJumpTrailColor, 5, 0.15f,doubleJumpCoroutine));
					doubleJump = false;
					jumpPressRemember = 0;
				}
			}	
		}

		if (Input.GetButtonDown("Fire1") && anim.GetBool("CanDash"))
		{
			if (grounded)
			{
				anim.SetTrigger("DashTrigger");
			}
			else
			{
				if (airDash)
				{
					anim.SetTrigger("DashTrigger");
				}
			}
		}

		if(Input.GetButtonDown("Fire2") && anim.GetBool("CanAttack"))
		{
			anim.SetTrigger("Attack");
		}

		if(transform.position.y < -4f && reload == null)
		{
			
			Death();
		}

		//Update do animator
		anim.SetBool("Walk", XInput != 0);
		anim.SetBool("Grounded", grounded);
		anim.SetBool("Dead", currentLife <= 0);
		anim.SetBool("WallSlide", wallSliding);
		anim.SetFloat("VelocityY", rigid.velocity.y);
		checkerGround = grounded;
		wallSlidingCheck = wallSliding;

	}


	void Jump()
	{
		anim.SetBool("CantMove", false);
		anim.SetBool("AirAnim", false);
		anim.SetBool("NoFall", false);
		anim.SetBool("CantFlip", false);
		rigid.velocity = Vector2.zero;
		rigid.Sleep();
		rigid.WakeUp();
		rigid.AddForce(new Vector2(0, jumpForce), ForceMode2D.Force);
		SFXsource.PlayOneShot(clips[0]);
	}

	void WallJump()
	{
		doingWallJump = true;
		wallJumpSafeTime = true;
		rigid.velocity = Vector2.zero;
		rigid.Sleep();
		rigid.WakeUp();
		rigid.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x)* wallJumpXForce, jumpForce*wallJumpYForce), ForceMode2D.Force);
		Flip();
		SFXsource.PlayOneShot(clips[1]);
		Invoke("ResetWallJump", wallJumpTime);
	}


	void WallSlideJump()
	{
		doingWallJump = true;
		wallJumpSafeTime = true;
		rigid.velocity = Vector2.zero;
		rigid.Sleep();
		rigid.WakeUp();
		rigid.AddForce(new Vector2(Mathf.Sign(transform.localScale.x) * wallJumpXForce, jumpForce * wallJumpYForce), ForceMode2D.Force);
		Flip();
		SFXsource.PlayOneShot(clips[1]);
		Invoke("ResetWallJump", wallJumpTime);
	}


	void ResetWallJump()
	{
		wallJumpSafeTime = false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "Item")
		{
			Item item = col.GetComponent<Item>();
			switch (item.itemType)
			{
				case ItemType.JumpGem:
					doubleJump = true;
					item.Collect();
					break;
				case ItemType.DashGem:
					airDash = true;
					item.Collect();
					break;
			}
		}
	}

	public void Footstep()
	{
		Vector2 position = transform.position;
		Vector2 direction = Vector2.down;
		float distance = 1.0f;

		RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, WhatIsGround);
		if (hit.collider != null)
		{
			Ground ground = hit.collider.GetComponent<Ground>();
			if(ground != null)
			{
				PlayFootstepSound(ground.groundType);
			}

		}
	}

	void PlayFootstepSound(GroundType type)
	{
		footStepsSource.PlayOneShot(groundSounds.playSound(type));
	}



	public void Dash()
	{
		if (!grounded)
		{
			airDash = false;
		}
		dashParticle.Play();
		CinemachineShake.Instance.ShakeCamera(0.5f, 0.05f);
		rigid.velocity = Vector2.zero;
		rigid.Sleep();
		rigid.WakeUp();
		rigid.AddForce(new Vector2(dashForce*Mathf.Sign(transform.localScale.x), 0),ForceMode2D.Force);
		trailCoroutine = StartCoroutine(TrailShadow(dashTrailColor, 10, 0.25f,trailCoroutine));
		SFXsource.PlayOneShot(clips[2]);
	}

	IEnumerator TrailShadow(Color color, int trailCount,float time,Coroutine corou)
	{
		for (int i = 0; i < trailCount; i++)
		{
			SpriteRenderer trail = GameObject.Instantiate(trailPrefab, transform.position, transform.rotation).GetComponent<SpriteRenderer>();
			trail.transform.localScale = transform.localScale;
			trail.color = color;
			yield return new WaitForSeconds(time / trailCount);
		}

		corou = null;
	}

	IEnumerator Reload()
	{
		yield return new WaitForSeconds(1.5f);
		Fade.Instance.FadeOut();
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(0);
	}

	public override void Death()
	{
		reload = StartCoroutine(Reload());
	}

	IEnumerator Coyote()
	{
		canCoyote = true;
		yield return new WaitForSeconds(coyoteTime);
		canCoyote = false;
		coyoteCorou = null;
	}

	public void Attack1()
	{
		rigid.velocity = Vector2.zero;
		rigid.Sleep();
		rigid.WakeUp();
		if (grounded) { 
			rigid.AddForce(new Vector2(attack1Force.x * Mathf.Sign(transform.localScale.x), attack1Force.y), ForceMode2D.Force);
		}
	}

	public void AttackCritical()
	{
		rigid.velocity = Vector2.zero;
		rigid.Sleep();
		rigid.WakeUp();
		if (grounded)
		{
			rigid.AddForce(new Vector2(attackCriticalForce.x * Mathf.Sign(transform.localScale.x), attackCriticalForce.y), ForceMode2D.Force);
		}
	}


	public override void GetDamage(int damage, Vector2 pushDistance, AttackType atkType)
	{
		if (currentLife <= 0)
		{
			return;
		}

		if (invencible)
		{
			return;
		}
		if (invencibleFrame)
		{
			FloatingText("Dodge!",Color.yellow, atkType);
			anim.SetBool("Critical", true);
			Invoke("CriticalReset", 1f);
			return;
		}

		FloatingText(damage.ToString(), DamageColor, atkType);
		CinemachineShake.Instance.ChromaticCamera(1, 20f);

		base.GetDamage(damage, pushDistance,atkType);
	}

	void CriticalReset()
	{
		anim.SetBool("Critical", false);
	}





	IEnumerator WallFlipJump()
	{
		wallSlideCanJump = true;
		yield return new WaitForSeconds(0.2f);
		wallSlideCanJump = false;

		wallFlipJumpCoroutine = null;
	}
}
