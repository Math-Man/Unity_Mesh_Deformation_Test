using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGenericLimb : MonoBehaviour
{
    [SerializeField] private GameObject LimbChunks;
    [SerializeField] private UnitGeneric body;
    [SerializeField] private HingeJoint joint;
    [SerializeField] private Rigidbody limbRigidBody;

    private void Awake()
    {
        LimbChunks = GameObject.Find("LimbChunks");
        body.limbs.Add(this);
    }

    private void OnJointBreak(float breakForce)
    {
        limbRigidBody.mass *= 5;
        body.limbs.Remove(this);
        Destroy(joint);
        Destroy(this, 0.1f);

        this.transform.SetParent(LimbChunks.transform, true);

    }
}
