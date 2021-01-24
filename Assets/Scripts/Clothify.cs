using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Clothify : MonoBehaviour
{
    [SerializeField] private float Gravity = 0.3f;


    private Mesh ClothMesh;
    public Cloth clothObject;

    private void Awake()
    {
        ClothMesh = GetComponent<MeshFilter>().mesh;
        clothObject = new Cloth(ClothMesh, 10, 10, transform);
    }


    private void OnDrawGizmos()
    {
        if (clothObject != null) 
        {
            for (int i = 0; i < clothObject.WidthVerticeCount; i++) 
            {
                for (int j = 0; j < clothObject.HeightVerticeCount; j++) 
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(clothObject.points[i, j].cPos, 0.5f);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(clothObject.points[i, j].pPos, 0.5f);

                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(clothObject.points[i, j].cPos, clothObject.points[i, j].acc);
                }
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (clothObject != null)
            clothObject.update(Time.deltaTime);
    }


  
}

public class Cloth
{
    //int pDist = 15;
    //int pCount = 20;

    //int startX = 250 - (pCount * pDist) / 2;
    //int startY = 50;

    public int WidthVerticeCount;
    public int HeightVerticeCount;

    public Particle[,] points;

    public Mesh clothMesh;

    public List<Constraint> lines;

    public Transform transform;

    public Cloth(Mesh clothMesh, int wSegments, int hSegments, Transform transform)
    {
        this.transform = transform;
        this.clothMesh = clothMesh;
        WidthVerticeCount = wSegments+1;
        HeightVerticeCount = hSegments+1;

        points = new Particle[WidthVerticeCount, HeightVerticeCount];
        lines = new List<Constraint>();

        initPoints(); //Initialise points on the cloth
        initConstrains(); //Initialises the constrainst
    }

    public void update(float dT)
    {
        // this.deform();
        foreach (Constraint c in this.lines)
        {
            for (int i = 0; i < 20; i++)
            {
                c.solve();
            }
        }

        //this.deform();

        //Updates the position of all points
        for (int i = 0; i < WidthVerticeCount; i++)
        {
            for (int j = 0; j < HeightVerticeCount; j++)
            {
                if (this.points[i,j].movable == true)
                    this.points[i,j].updatePos(dT);
            }
        }

        Vector3[] displacedVertices = clothMesh.vertices;

        int k = 0;
        for (int i = 0; i < WidthVerticeCount; i++)
        {
            for (int j = 0; j < HeightVerticeCount; j++)
            {
                displacedVertices[k] = this.points[i, j].cPos;
                k++;
            }
        }


        clothMesh.vertices = displacedVertices;
        clothMesh.RecalculateNormals();


    }


    //Create the points in grid formation
    private void initPoints()
    {
        Vector3[] vertices = clothMesh.vertices;


        //Expand to 2d array
        Vector3[,] vertices2d = new Vector3[WidthVerticeCount, HeightVerticeCount];
        for (int i = 0; i < WidthVerticeCount; i++)
        {
            for (int j = 0; j < HeightVerticeCount; j++)
            {
                vertices2d[i, j] = vertices[i * HeightVerticeCount + j];
            }
        }


        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    //currentVerticePosition[i] = originalVericePositions[i];


        //}

        for (int i = 0; i < WidthVerticeCount; i++)
        {
            for (int j = 0; j < HeightVerticeCount; j++)
            {
                Vector3 worldPt = transform.TransformPoint(vertices2d[i, j]);
                Particle verticeParticle = new Particle(worldPt , true, i, j);
                points[i, j] = verticeParticle;
            }
        }

                //for (int i = 0; i < pCount; i++)
                //{
                //    for (int j = 0; j < pCount; j++)
                //    {
                //        this.points[i,j] = new Particle(0 + (pDist * j), 0 + (pDist * i),
                //                i == 0 && (j == 0 || j == 1 || j == 9 || j == 10 || j == 18 || j == 19) ? false : true);
                //    }
                //}
            }

    //Initializes all the constrains between points
    //Uses right and down tranversal at the same time
    private void initConstrains()
    {
        for (int i = 0; i < WidthVerticeCount; i++)
        {
            for (int j = 0; j < HeightVerticeCount; j++)
            {
                Particle currParticle = points[i, j];

                if (j != 10) lines.Add(new Constraint(currParticle, points[i, j + 1]));
                if (i != 10) lines.Add(new Constraint(currParticle, points[i + 1, j]));

            }

        }
    }

    //Extra function for a bit of extra fun
    //Displaces the particles by a random margin within 10 pixels
    public void deform()
    {
        //for (int i = 1; i < WidthVerticeCount; i++)
        //{
        //    for (int j = 0; j < HeightVerticeCount; j++)
        //    {
        //        float randomNum = Random.Range(-10, 10 + 1);
        //        this.points[i, j].cPos.x = 0 + (0.1f * j) + randomNum;
        //        this.points[i, j].cPos.y = 0 + (0.1f * i) + randomNum;
        //        this.points[i, j].cPos.z = 0 + (0.1f * i) + randomNum;
        //    }
        //}
    }
}


public class Particle
{
    int indexX, indexY;

    public Vector3 cPos;  //The position of the particle in this time-st    ep
    public Vector3 pPos; //The position of the particle in the previous time-step
    public Vector3 acc; //The acceleration of the particle
    public bool movable; //Is the particle movable
    public static float GRAVITY = 0.3f;

    public Particle(Vector3 pos, bool mov, int indexx, int indexy)
    {
        this.cPos = pos;
        this.pPos = pos;
        acc = Vector3.zero;
        acc.y = GRAVITY;
        this.movable = mov;

        indexX = indexx;
        indexY = indexy;
    }

    public void updatePos(float dT)
    {
        //Calculate future positions
        if (this.movable == true)
        {
            Vector3 fPos = this.cPos; //Future positions

            //Verlet integration that calculates the future position of a particle
            //Based on a variable time step, past position and acceleration
            fPos += (this.cPos - this.pPos) + 0.5f * this.acc * dT * dT;

            //Update the position values
            this.pPos = this.cPos;
            this.cPos = fPos;

            //Box
            //if (this.cPosY > 500) this.cPosY = 500;
        }
    }
}

public class Constraint
{
    Particle stP, enP;
    float length;
    float normL;

    public Constraint(Particle start, Particle end)
    {
        this.stP = start;
        this.enP = end;
        this.normL = (15.0f);
    }

    //Calculates the length between the start point and end point
    private float getLength()
    {
        this.length = Mathf.Pow(Mathf.Pow(enP.cPos.x - stP.cPos.x, 2) + Mathf.Pow(enP.cPos.y - stP.cPos.y, 2) + Mathf.Pow(enP.cPos.z - stP.cPos.z, 2), 0.5f);
        return length;
    }

    public void solve()
    {
        float diffX = stP.cPos.x - enP.cPos.x;
        float diffY = stP.cPos.y - enP.cPos.y;
        float diffZ = stP.cPos.z - enP.cPos.z;

        float d = this.getLength();
        if (d != 0)
        {
            float diffS = (normL - d) / d;

            float transX = diffX * 0.5f * diffS * 0.09f;
            float transY = diffY * 0.5f * diffS * 0.09f;
            float transZ = diffZ * 0.5f * diffS * 0.09f;

            if (stP.movable)
            {
                this.stP.cPos += new Vector3(transX, transY, transZ);
            }

            if (enP.movable)
            {
                this.enP.cPos -= new Vector3(transX, transY, transZ);
            }
        }
    }
}