using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDist : MonoBehaviour
{
    public Material matDefault;
    public Material matTouched;

    List<GameObject> chunks = new List<GameObject>();
    List<Renderer> rends = new List<Renderer>();
    List<Material> prevMats = new List<Material>();
    List<float> startTimes = new List<float>();

    float t;
    // float startTime;
    float stayOnDuration = 7;
    float fadeDuration = 3;

    Material prevMat;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
        foreach(GameObject go in allObjects) {

            if(go.tag != gameObject.tag && go.tag != "Untagged") {      // Identify objects with a tag and different than current dancer

                Component[] allComponents = go.GetComponents<Component>();
                if (allComponents.Length != 1) { // Contains only Transform? -> Is an empty game object
                    chunks.Add(go);
                    rends.Add(go.GetComponent<Renderer>() );
                    startTimes.Add(Time.time - stayOnDuration - fadeDuration*2); // Initiate start time with value out of bounds   
                } 
            }
        }

        foreach(Renderer r in rends) {
            r.material = matDefault;
            prevMats.Add(r.material);
        } 

    }

    // Update is called once per frame
    void Update()
    {
        t = Time.time;

        int i=0;
        foreach(GameObject go in chunks) {
            // rends[i].updateWhenOffscreen = true;

            if(rends[i].material != prevMats[i]) {
                // startTimes[i] = Time.time;
            }

            float dist = Vector3.Distance(go.transform.position, transform.position);
            // float dist = Vector3.Distance(rends[i].bounds.center, transform.position);
                // Mesh mesh = go.GetComponent<SkinnedMeshRenderer>();
            // Bounds bounds = go.GetComponent<SkinnedMeshRenderer>().bounds;

//  SkinnedMeshRenderer mesh = go.GetComponent<SkinnedMeshRenderer> ();
//  mesh.updateWhenOffscreen = true;
//  Bounds bounds = new Bounds();
//  Vector3 center = mesh.localBounds.center;
//  Vector3 extents = mesh.localBounds.extents;
//  bounds.center = center;
//  bounds.extents = extents;
//  mesh.updateWhenOffscreen = false;
//  mesh.localBounds = bounds;

// Bounds boundingBox = new Bounds();
// boundingBox.Encapsulate (rend[i].bounds);

        //  Mesh mesh = go.GetComponent<SkinnedMeshRenderer> ().sharedMesh;
        // mesh.RecalculateBounds();

        // Mesh mesh = new Mesh();
        // SkinnedMeshRenderer sMesh = go.GetComponent<SkinnedMeshRenderer> ();
        // sMesh.BakeMesh(mesh);
        // Bounds b = mesh.bounds;

        //     float dist = Vector3.Distance(b.center, transform.position);

        //     if(i==31) Debug.Log(dist);

            if(dist <= 0.1) {
                rends[i].material = matTouched;
                prevMats[i] = rends[i].material;
                Debug.Log(go);
            }

            // if(t >= startTimes[i] + stayOnDuration) {
            //     if( t < startTimes[i] + stayOnDuration + fadeDuration) {
            //         float lerp = map(t, startTimes[i]+stayOnDuration, startTimes[i]+stayOnDuration+fadeDuration, 0, 1);
            //         rends[i].material.Lerp(matTouched, matDefault, lerp); 
            //     } else {
            //         rends[i].material = matDefault;
            //         prevMats[i] = rends[i].material;
            //     }
            // }


            i++;
        }
        

        
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }
}
