using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[]
        {
            new Vector3(-5, 0, -5),
            new Vector3(-5, 0, 5),
            new Vector3(5, 0, 5),
            new Vector3(5,0,-5),
        };

        triangles = new int[]
        {
            0,1,2,
            2,3,0
        };
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        Debug.Log(mesh.vertices);
        
        mesh.RecalculateNormals();
    }
    
}
