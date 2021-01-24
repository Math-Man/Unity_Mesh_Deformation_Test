using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float forceMult { get; set; }


    [SerializeField] float deformationForce = 100f;
    [SerializeField] float deformationOffset = 0.1f;

    private Rigidbody rbody;
    [SerializeField]private ParticleSystem deathParticleSystem;


    public Vector3 targetPoint { get; set; }

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rbody.AddForce( (-this.transform.position + targetPoint).normalized * 800f * forceMult);
    }


    private void OnCollisionEnter(Collision collision)
    {
        ApplyDeformation(collision);
        ExplosiveEffect();
        Destroy(this.gameObject,0.1f);
        Destroy(this.transform.parent.gameObject, 2);
    }

    public void ApplyDeformation(Collision collision) 
    {
        MeshDeformer deformer = collision.collider.GetComponent<MeshDeformer>();
        if (deformer != null && collision.contacts.Length > 0)
        {
            Vector3 point = collision.contacts[0].point;
            point += collision.contacts[0].normal * deformationOffset;
            deformer.AddDeformingForce(point, deformationForce * rbody.velocity.magnitude);

            deathParticleSystem.transform.position = point;
            deathParticleSystem.Play();

        }
    }

    public void ExplosiveEffect() 
    {
        Collider[] objects = UnityEngine.Physics.OverlapSphere(this.transform.position, 10 * forceMult);
        foreach (Collider h in objects)
        {
            Rigidbody r = h.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(forceMult * 100f, this.transform.position, 10 * forceMult);
            }
        }


    }
}
