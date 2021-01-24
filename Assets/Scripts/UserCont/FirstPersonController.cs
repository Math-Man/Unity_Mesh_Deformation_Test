using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{

    [SerializeField] private Camera viewCamera;
    private Rigidbody rigidbody;
    private Collider collider;

    private float distToGround;
    private bool leap;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
    }

    void Start()
    {
        distToGround = collider.bounds.extents.y;
    }

    void Update()
    {
        rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        //Lateral
        Vector3 motionVector = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            motionVector += viewCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            motionVector += -viewCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            motionVector += viewCamera.transform.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            motionVector += -viewCamera.transform.right;
        }


        bool grounded = isGrounded();
        if (grounded)
        {
            leap = false;
        }
        else if (rigidbody.velocity.y > 0) 
        {
            leap = true;
        }

        //Vertical
        Vector3 upMotionVector = new Vector3();
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            upMotionVector.y += 500f;
        }
        else if (Input.GetKeyUp(KeyCode.Space) && !grounded && leap) 
        {
            upMotionVector.y -= 145f;
        }

        motionVector.x *= 0.06f;
        motionVector.y *= 0f;
        motionVector.z *= 0.06f;


        transform.position += motionVector;
        rigidbody.AddForce(upMotionVector);
        rigidbody.AddForce(motionVector);

    }

    private bool isGrounded() 
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }


    
}
