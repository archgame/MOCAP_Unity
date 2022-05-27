using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cone : MonoBehaviour
{

//Mesh
Mesh mesh;
MeshRenderer meshRenderer;

//Vertices
List<Vector3> vertices;
List<int> triangles;

public Material material;

//Cone parameters
public float height = 3.0f;
public float radius = 5.0f;
public int segments = 7;

Vector3 pos;

float angle = 0.0f;
float angleAmount = 0.0f;


    // Start is called before the first frame update
    void Start()
    {

        //Initialise mesh and material
        gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //Initialise vertices
        vertices = new List<Vector3>();
        pos = new Vector3();

        angleAmount = 2 * Mathf.PI / segments;
        angle = 0.0f;

        //Cone centre top
        pos.x = 0.0f;
        pos.y = height;
        pos.z = 0.0f;
        vertices.Add(new Vector3(pos.x, pos.y, pos.z));

        //Cone centre base
        pos.y = 0.0f;
        vertices.Add(new Vector3(pos.x, pos.y, pos.z));

        for (int i=0; i < segments; i++)
        {
            //Agregate vertices around
            pos.x = radius * Mathf.Sin(angle);
            pos.z = radius * Mathf.Cos(angle);

            vertices.Add(new Vector3(pos.x, pos.y, pos.z));

            angle-= angleAmount;
        }

        //Assign vertices to mesh
        mesh.vertices = vertices.ToArray();

        //Initialise triangle list
        triangles = new List<int>();

        for(int i=2; i<segments+1; i++) {
            triangles.Add(0);
            triangles.Add(i+1);
            triangles.Add(i);
        }

        //Close
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(segments+1);

        //Bottom
        for(int i=2; i<segments+1; i++) {
            triangles.Add(1);
            triangles.Add(i);
            triangles.Add(i+1);
        }

        //Close bottom
        triangles.Add(1);
        triangles.Add(segments+1);
        triangles.Add(2);


        mesh.triangles = triangles.ToArray();



        
    }

}
