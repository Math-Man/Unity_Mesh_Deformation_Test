using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformer : MonoBehaviour
{

    [SerializeField] public bool useMeshedCollider = false;
    [SerializeField] public bool enableSpring = true;
    [SerializeField] public bool CompletlyDisableMeshChanging = false;

    [Header("Velocity Rectifier")]
    [SerializeField] [Range(0f, 1f)] public float VelocityRectifierMult = 0.334f;

    [Header("Spring Properties")]
    [SerializeField] public float springForce = 20f;
    [SerializeField] public float damping = 5f;
    [SerializeField] public float uniformScale = 1f;

    private Mesh deformingMesh;
    private Vector3[] originalVertices, displacedVertices;
    private Vector3[] vertexVelocities;
    private MeshCollider meshCollider;
    private Mesh OriginalMesh;

    private void Awake()
    {
        if (!CompletlyDisableMeshChanging) 
        {
            var collider = GetComponent<Collider>();
            if (!(typeof(MeshCollider).IsInstanceOfType(collider)))
            {
                DestroyImmediate(collider);
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }
            else 
            {
                meshCollider = (MeshCollider)collider;
            }
            OriginalMesh = meshCollider.sharedMesh;
        }
    }

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
        vertexVelocities = new Vector3[originalVertices.Length];
    }

    void FixedUpdate()
    {
        uniformScale = transform.localScale.x;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();


        if (!CompletlyDisableMeshChanging) 
        {
            if (useMeshedCollider)
            {
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = deformingMesh;
            }
            else
            {
                if (OriginalMesh != meshCollider.sharedMesh) 
                {
                    meshCollider.sharedMesh = OriginalMesh;
                }
            }
        }


    }


    public void AddDeformingForce(Vector3 point, float force)
    {
        //Debug.DrawLine(Camera.main.transform.position, point);
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    private void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    private void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];

        displacement *= uniformScale;
        if (enableSpring)
        {
            velocity -= displacement * springForce * Time.deltaTime;
            velocity *= 1f - damping * Time.deltaTime;
        }
        else
        {
            velocity *= VelocityRectifierMult;
        }

        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }
}
