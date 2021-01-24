using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField]private GameObject projectilePrefab;
    [SerializeField] private float forceMult = 1f;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            FireProjectile();
        }
    }


    public void FireProjectile()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            //if (hit.collider && hit.collider.gameObject.name != "Projectile")
            {
                Vector3 point = hit.point;
                Debug.DrawLine(Camera.main.transform.position, point);

                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                var projScript = projectile.GetComponentInChildren<Projectile>();
                projScript.targetPoint = point;
                projScript.forceMult = forceMult;
            }
        }
    }
}
