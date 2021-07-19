using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
		if (target)
		{
            transform.position = target.position + offset;
		}
    }

    public void SetTarget(Transform p_target,Vector3 p_offset)
	{
        target = p_target;
        offset = p_offset;
    }
}
