using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnChangePosition : MonoBehaviour
{
    public PolygonCollider2D hole2DColider;
    public PolygonCollider2D ground2DColider;
    public MeshCollider GeneratedMeshColider;
    public Collider GroundColider;
    public float initialScale = 0.5f;
    Mesh GeneratedMesh;

    public void Move(BaseEventData myEvent)
    {
        if(((PointerEventData)myEvent).pointerCurrentRaycast.isValid)
        {
            transform.position = ((PointerEventData)myEvent).pointerCurrentRaycast.worldPosition;
        }
    }

    public IEnumerator ScaleHole()
    {
        Vector3 StartScale = transform.localScale;
        Vector3 EndScale = StartScale * 2;

        float t = 0;
        while(t<=0.4f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(StartScale, EndScale, t);
            yield return null;
        }
    }

    private void Start()
    {
        GameObject[] AllGOs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (var go in AllGOs)
        {
            if (go.layer == LayerMask.NameToLayer("Obstacles"))
            {
                Physics.IgnoreCollision(go.GetComponent<Collider>(), GeneratedMeshColider, true);
            }
        }
    }
   

    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreCollision(other, GroundColider, true);
        Physics.IgnoreCollision(other, GeneratedMeshColider, false);
    }

    private void OnTriggerExit(Collider other)
    {
        Physics.IgnoreCollision(other, GroundColider, false);
        Physics.IgnoreCollision(other, GeneratedMeshColider, true);
    }


    private void FixedUpdate()
    {
        if(transform.hasChanged == true)
        {
            transform.hasChanged = false;
            hole2DColider.transform.position = new Vector2(transform.position.x, transform.position.z);
            hole2DColider.transform.localScale = transform.localScale * initialScale; 
            MakeHole2D();
            Make3DMeshCollider();
        }
    }

    private void MakeHole2D()
    {
        Vector2[] PointPositions = hole2DColider.GetPath(0);

        for (int i = 0; i < PointPositions.Length; i++)
        {
            PointPositions[i] = hole2DColider.transform.TransformPoint(PointPositions[i]);
        }

        ground2DColider.pathCount = 2;
        ground2DColider.SetPath(1, PointPositions);
    }

    private void Make3DMeshCollider()
    {
        if (GeneratedMesh != null) Destroy(GeneratedMesh);
        GeneratedMesh = ground2DColider.CreateMesh(true, true);
        GeneratedMeshColider.sharedMesh = GeneratedMesh;

    }
}
