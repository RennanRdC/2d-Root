using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlataform : MonoBehaviour
{
    public Transform plataform;
    public Transform A;
    public Transform B;
    public bool toB;
    public float velocity;
    public Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
		if (toB){
            plataform.transform.localPosition = A.localPosition;
            target = B.localPosition;
		} else
		{
            plataform.transform.localPosition = B.localPosition;
            target = A.localPosition;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (toB)
		{
            plataform.transform.localPosition = Vector3.MoveTowards(plataform.transform.localPosition,  target, Time.deltaTime * velocity);
            if(Vector3.Distance(plataform.transform.localPosition, target) < 0.01f)
			{
                toB = false;
                target = A.localPosition;
            }
		}
		else
		{
            plataform.transform.localPosition = Vector3.MoveTowards(plataform.transform.localPosition, target,Time.deltaTime* velocity);
            if (Vector3.Distance(plataform.transform.localPosition, target) < 0.01f)
            {
                toB = true;
                target = B.localPosition;
            }
        }
    }


    void OnTriggerStay2D(Collider2D col)
    {
		if (col.gameObject.CompareTag("Player"))
		{
            col.transform.SetParent(plataform.transform);
		}
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.transform.SetParent(null);
        }
    }
}
