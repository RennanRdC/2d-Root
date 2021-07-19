using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected Animator anim;
    protected AudioSource source;
    public GameObject interactPrefab;

    // Start is called before the first frame update
    public void Start()
    {
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interact()
	{

	}
}
