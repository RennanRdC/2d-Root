using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : Interactable
{
    private Coroutine crank;
    public Transform plataform;
    public Transform A;
    public Transform B;
    public float timeOpen;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        plataform.gameObject.SetActive(false);
    }


    public override void Interact()
    {
        if(crank == null) 
        {
            crank = StartCoroutine(CrankOn());
        }
    }


    public IEnumerator CrankOn()
	{
        plataform.gameObject.SetActive(true);
        plataform.localPosition = A.localPosition;
        source.Play();
        Instantiate(interactPrefab,transform.position,interactPrefab.transform.rotation);
        anim.SetBool("Open", true);

        yield return new WaitForSeconds(1f);

        while (Vector2.Distance(plataform.localPosition, B.localPosition) > 0.1f)
        {
            plataform.transform.localPosition = Vector3.Lerp(plataform.transform.localPosition, B.localPosition, Time.deltaTime * 1f);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(timeOpen);

        while (Vector2.Distance(plataform.localPosition, A.localPosition) > 0.1f)
        {
            plataform.transform.localPosition = Vector3.MoveTowards(plataform.transform.localPosition, A.localPosition, Time.deltaTime * 2.5f);
            yield return new WaitForEndOfFrame();
        }
        source.Play();
        plataform.gameObject.SetActive(false);

        anim.SetBool("Open", false);
        crank = null;
	}
}
