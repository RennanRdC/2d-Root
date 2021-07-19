using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public static Fade Instance { get; private set; }
    public Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void FadeOut()
    {
        anim.SetTrigger("Fade");
    }
}
