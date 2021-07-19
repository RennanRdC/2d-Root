using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [Header("Components")]
    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Animator anim;
    private GUIManager guiManager;
    public Transform groundCheck;
    public Transform wallCheck;
    public SpriteRenderer rend;
    public LayerMask WhatIsGround;
    public LayerMask WhatIsWall;
    [Header("Particles")]
    public ParticleSystem dust;
    public GameObject sparkPrefab;
    [Header("Parameters")]
    public Team team;
    public Color DamageColor;
    public int spriteFace = -1;
    public Vector2 wallCheckSize;
    public Vector2 groundCheckSize;
    public int maxlife = 10;
    public int currentLife;
    public bool facingRight;
    public bool grounded;
    public bool invencible;
    public bool invencibleFrame;
    public float invencibleTime = 1;
    public bool noMove;
    public Vector3 sparkOffset;
    private Coroutine invencibleCorou;
    private Coroutine invencibleFrameCorou;
    [Header("WallJump")]
    public bool touchingWall;


    // Start is called before the first frame update
    protected void Start()
    {
        rigid = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        rend = this.GetComponent<SpriteRenderer>();
        currentLife = maxlife;
        guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
    }

    // Update is called once per frame
    protected void Update()
    {
        grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, WhatIsGround);
        touchingWall = Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, WhatIsWall);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(wallCheck.position, wallCheckSize);
    }

    protected void Flip()
    {



        if (anim.GetBool("CantFlip"))
        {
            return;
        }
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        if(dust != null)
		{
            dust.transform.localScale = theScale;
        }
        
    }

    public void FloatingText(string p_text, Color textColor, AttackType atkType)
    {
        string finalText = p_text;
        if(atkType == AttackType.Critical)
		{
            finalText += "!";
		}
        guiManager.FloatingText(groundCheck, finalText, textColor);
    }

    protected IEnumerator PushDamage(Vector2 pushDistance)
    {
        rigid.velocity = Vector2.zero;
        rigid.Sleep();
        rigid.WakeUp();
        anim.SetBool("Damaged", true);
        noMove = true;
        rigid.velocity = Vector2.zero;
        rigid.Sleep();
        rigid.WakeUp();
        rigid.AddForce(new Vector2(pushDistance.x, pushDistance.y), ForceMode2D.Force);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Damaged", false);
    }

    protected IEnumerator Invencible()
    {
        if(invencibleTime == 0)
		{
            invencibleCorou = null;
            yield break;

		}


        invencible = true;
        rend.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 5; i++)
        {
            rend.color = Color.clear;
            yield return new WaitForSeconds((invencibleTime - 0.05f) / 10);
            rend.color = Color.white;
            yield return new WaitForSeconds((invencibleTime - 0.05f) / 10);
        }
        invencible = false;
        invencibleCorou = null;
    }

    public virtual void Death()
    {

    }



    public void PlayDust()
    {
        dust.Play();
    }

    public virtual void GetDamage(int damage, Vector2 pushDistance,AttackType atkType)
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
            return;
        }

        currentLife -= damage;
        StartCoroutine(PushDamage(pushDistance));

        CinemachineShake.Instance.ShakeCamera(0.1f + 0.1f*damage, 0.1f);
        
        
        GameObject.Instantiate(sparkPrefab, transform.position + sparkOffset, transform.rotation);
        if (currentLife <= 0)
        {
            Death();
        }
        else
        {
            if(invencibleCorou != null)
			{
                StopCoroutine(invencibleCorou);
			}
            invencibleCorou = StartCoroutine(Invencible());
        }
    }

    public void InvencibleFrame(float time)
	{
        if (invencibleFrameCorou != null)
        {
            StopCoroutine(invencibleFrameCorou);
            invencibleFrame = false;
        }
        invencibleFrameCorou = StartCoroutine(InvencibleFrameCorou(time));
    }



    protected IEnumerator InvencibleFrameCorou(float time)
    {
        invencibleFrame = true;
        yield return new WaitForSeconds(time);
        invencibleFrame = false;

        invencibleFrameCorou = null;
    }

}
