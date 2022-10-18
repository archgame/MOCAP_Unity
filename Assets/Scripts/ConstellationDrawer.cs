using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.VFX;

public class ConstellationDrawer : MonoBehaviour
{
    //Collect all VFX 
    private VisualEffect[] galaxy;
    public GameObject vfxParent;
    //effects during drawing

    [SerializeField]
    private Texture2D particleTex;
    [SerializeField]
    private Texture2D trans;
    private float alp;


    [SerializeField]
    private GameObject starSpike;

    [SerializeField]
    private VisualEffect starBoom;
    private int turnNum;
    private int maxTurn=5;
    private int boomNum;

    private GameObject[] lineHolder;

    public bool isDrawActive;
    private int activeAvartarIndex;
    public int starNum=5;
    private DataSubscription.Avatar[] avatars;


    private LineRenderer[] lineRenderers;
    public Material lineMaterial;
    public Material starMaterial;


    public VisualEffect constellationDrawer;

    private GameObject[][] constellation;
    private GameObject allConstellation;

    private int activeStarIndex;

    [SerializeField]
    private DataSubscription data;
    // Start is called before the first frame update
    void Start()
    {
        isDrawActive = false;
        avatars = new DataSubscription.Avatar[2] { data.avatar0, data.avatar1 };
        allConstellation = new GameObject();
        allConstellation.name = "All Constellations";
        allConstellation.transform.parent = gameObject.transform;
        lineHolder = new GameObject[maxTurn];
        lineRenderers = new LineRenderer[maxTurn];
        for (int i = 0; i < maxTurn; i++) {
            lineHolder[i] = new GameObject();
            lineHolder[i].name = "LineHolder" + i;
            lineHolder[i].transform.parent = gameObject.transform;
            lineRenderers[i] = lineHolder[i].AddComponent<LineRenderer>();
            lineRenderers[i].SetPosition(0, Vector3.zero);
            lineRenderers[i].SetPosition(1, Vector3.zero);
            lineRenderers[i].startWidth = 0.25f;
            lineRenderers[i].endWidth = 0.5f;
            lineRenderers[i].material = lineMaterial;
            lineRenderers[i].textureMode = LineTextureMode.Tile;
            lineRenderers[i].enabled = false;
        }


        /*lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.startWidth = 0.25f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.material = lineMaterial;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.material.mainTextureScale = new Vector2(1f / 0.01f, 1f);
        lineRenderer.material.texturemode*/
        activeStarIndex = 1;
        Debug.Log("Activestar Index is " + activeStarIndex);
        activeAvartarIndex = 1;
        constellationDrawer.enabled = false;
        alp = 1f;
        
        GenerateConstellation(starNum);



        //boom
        turnNum = 0;
        boomNum = 0;

        //initialize galaxy VFX
        vfxParent.SetActive(true);
        galaxy = FindObjectsOfType<VisualEffect>();
        foreach(Transform child in vfxParent.transform) {
            child.gameObject.SetActive(false);
        }

        Debug.Log("there are " + galaxy.Length +" in the list");

    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawActive && activeStarIndex != constellation[turnNum].Length) {
            lineRenderers[turnNum].enabled = true;
            lineRenderers[turnNum].material.SetFloat("_alpha", 1f);
            /*foreach (var vfx in galaxy) {
                if(vfx != constellationDrawer) { vfx.playRate = 0.1f; }
            }*/
            if(constellationDrawer.enabled == false) { constellationDrawer.enabled = true; constellationDrawer.Play(); }

            //Color.Lerp(Color.white, Color.black, colorVar);=
            //Vector3 newPos = new Vector3(Time.time, 0, Time.time );
            constellationDrawer.SetTexture("texture", trans);
            Vector3 currentPos = lineRenderers[turnNum].GetPosition(lineRenderers[turnNum].positionCount - 1);
            constellationDrawer.SetVector3("spawnPosition", currentPos);
            Vector3 nextStarPos = constellation[turnNum][activeStarIndex].transform.position;
            Vector3 currentPosToStar = nextStarPos - currentPos;

            Vector3 moveDirection = avatars[activeAvartarIndex].leftHand.velocity;
            //if(moveDirection == Vector3.zero) { moveDirection = currentPosToStar; }
            
            Vector3 connectDir = ConnectDir(currentPosToStar, moveDirection);
            float magnifier = Unity.Mathematics.math.remap(0, 25, 0.1f, 0.3f, Unity.Mathematics.math.min(data.avatar0.leftHand.speed, 25));
            Vector3 drawVector = connectDir * magnifier;
            //Debug.Log(string.Format("{0},{1},{2}", drawVector.x, drawVector.y, drawVector.z));
            DrawLine(lineRenderers[turnNum], drawVector, constellation[turnNum][0].transform.position);
            IncreIndex(currentPos, nextStarPos);
            
        }
        if(!isDrawActive && activeStarIndex == constellation[turnNum].Length) {
            /*foreach (var vfx in galaxy) {
                vfx.playRate = 1f;
            }*/
            if (boomNum < turnNum && activeStarIndex!= 1) { starBoom.SendEvent("ShootStar"); boomNum++; }
            alp = alp<=0.1f?  0.1f : alp - (Time.deltaTime/5f);
            lineRenderers[turnNum-1].material.SetFloat("_alpha", alp);
            //lineMaterial.SetFloat("_alpha", alp);
        }

    }

    private Vector3 ConnectDir(Vector3 posToNextStar, Vector3 drawDriection)
    {
        if (Vector3.Angle(posToNextStar, drawDriection) <= 15f && Vector3.Angle(posToNextStar, drawDriection) >= -15f) {
            //Debug.Log(Vector3.Angle(posToNextStar, drawDriection));
            return drawDriection.normalized * 2f;
        }
        else
            return drawDriection.normalized * 0.35f;
    }
    private void DrawLine(LineRenderer lineRend, Vector3 drawVector, Vector3 startPos)
    {
        if (lineRend.positionCount == 2) {

            lineRend.SetPosition(0, startPos);
            lineRend.SetPosition(1, startPos);
            constellation[turnNum][0].SetActive(true);
            //instanceRadarWithScale(0, 3f);
            constellation[turnNum][1].SetActive(true);
            //instanceRadarWithScale(1, 3f);
            Debug.Log("start position set as " + lineRend.GetPosition(0) + " and " + lineRend.GetPosition(1));
        }
        Vector3[] lastPositions = new Vector3[lineRend.positionCount];
        Vector3[] updatedPosition = new Vector3[lineRend.positionCount + 1];
        //Debug.Log(string.Format("lastPositionsLength is {0}, updatedPositions length is {1}", lineRend.positionCount, updatedPosition.Length));
        lineRend.GetPositions(lastPositions);
        for (int i = 0; i < lastPositions.Length; i++) {
            updatedPosition[i] = lastPositions[i];
        }
        updatedPosition[updatedPosition.Length - 1] = lastPositions[lastPositions.Length - 1] + drawVector;
        lineRend.positionCount += 1;
        lineRend.SetPositions(updatedPosition);
    }

    private void GenerateConstellation(int numStar)
    {
        Vector3[][] starList = new Vector3[maxTurn][];
        for (int i = 0; i < maxTurn; i++) {
            starList[i] = new Vector3[numStar];
        }
        constellation = new GameObject[maxTurn][];
        for (int i =0; i<maxTurn; i++) {
            constellation[i] = new GameObject[numStar];
        }
        for (int j = 0; j < 5; j++) {
            GameObject ConStars = new GameObject();
            ConStars.name = "Constellation" + j;
            ConStars.transform.parent = allConstellation.transform;
            for (int i = 0; i < numStar; i++) {
                starList[j][i] = new Vector3(Random.Range(-8f, 8f), 0f, Random.Range(-4f, 4f));

                //constellation[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                constellation[j][i] = Instantiate(starSpike);
                constellation[j][i].transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                constellation[j][i].GetComponent<twinkle>().period = 1f;
                //constellation[i].GetComponent<MeshRenderer>().material = starMaterial;
                constellation[j][i].transform.position = starList[j][i];
                constellation[j][i].name = "Star" + j + "," + i;
                constellation[j][i].transform.parent = ConStars.transform;
                constellation[j][i].SetActive(false);
            }
            constellation[j][0].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            constellation[j][0].GetComponent<twinkle>().period = 0.3f;
            constellation[j][1].transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            constellation[j][1].GetComponent<twinkle>().period = 1f;
        }


    }

    private void IncreIndex(Vector3 currentP, Vector3 endP)
    {
        if (Vector3.Distance(currentP, endP) <= 0.4f) {
            constellation[turnNum][activeStarIndex].GetComponent<twinkle>().period = 0.3f;
            constellation[turnNum][activeStarIndex].GetComponent<twinkle>()._scale = true;
            activeStarIndex++;
            Debug.Log("activeStarIndex is now " + activeStarIndex + "while ConstellationLength is" + constellation[turnNum].Length);
            if (activeStarIndex == constellation[turnNum].Length) {
                
                //constellationDrawer.SetTexture("texture", particleTex);
                foreach(var stars in constellation[turnNum]) {
                    stars.GetComponent<twinkle>().spike.SetActive(false);
                }
                turnNum++;
                Debug.Log("activeStarIndex is now " + activeStarIndex);
                Debug.Log("turn numer is now" + turnNum);
                isDrawActive = false;
                return; }
            //constellation[activeStarIndex].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            //instanceRadarWithScale(activeStarIndex, 6f);
            activeAvartarIndex = activeAvartarIndex == 0 ? 1 : 0;
            Debug.Log("activeAvatarIndex is now " + activeAvartarIndex);
            //Debug.Log("activeStarIndex is now " + activeStarIndex);
            constellation[turnNum][activeStarIndex].SetActive(true);
           
        }
    }

    public void toggleDraw()
    {
        isDrawActive = !isDrawActive;
    }

    public void anotherRound()
    {
        isDrawActive = true;
        activeStarIndex = 1;
        Debug.Log("ActiveStarIndex is" + activeStarIndex);
    }

}



