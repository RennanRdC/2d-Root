using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Parallax : MonoBehaviour
{
    public Transform cam;
    public float relativeMoveX = .3f;
    public float relativeMoveY = .3f;
    public Vector3 offset;
    public bool lockY;
    public float lockOffset;



	// Update is called once per frame
	void Update()
    {



		if (lockY)
		{
            transform.position = new Vector3(cam.position.x * relativeMoveX, transform.position.y+ lockOffset,transform.position.z);
		}
		else
		{
            transform.position = new Vector3(cam.position.x * relativeMoveX, cam.position.y * relativeMoveY,transform.position.z) + offset;
        }
        
    }
}
