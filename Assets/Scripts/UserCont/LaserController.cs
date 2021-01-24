using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{

    private LineRenderer lr;
    private Collider coll;

    void Start()
    {
        coll = GetComponent<Collider>();
        lr = gameObject.GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position);
    }

    void Update()
    {
        lr.SetPosition(0, transform.position);

        Vector3 v = transform.forward;
        v.x *= 100;
        v.y *= 100;
        v.z *= 100;

        RaycastHit hit;
        if (Physics.Linecast(transform.position, transform.position + v, out hit))
        {
            Debug.Log("blocked " + hit.point);
            lr.SetPosition(1, hit.point);
        }
        else 
        {
            lr.SetPosition(1, transform.position + v);
        }

    }

}
