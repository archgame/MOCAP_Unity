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
    [SerializeField]
    private int maxTurn=15;
    private int boomNum;

    private GameObject[] lineHolder;

    public bool isDrawActive;
    private int activeAvartarIndex;
    public int starNum=5;
    private DataSubscription.Avatar[] avatars;


    private LineRenderer[] lineRenderers;
    public Material lineMaterial;
    public Material starMaterial;


    public VisualEffect Sparkle;

    private GameObject[][] constellation;
    private GameObject allConstellation;

    private int activeStarIndex;

    private bool isCoroutine;

    [SerializeField]
    private DataSubscription data;

    [SerializeField] private Transform endPointPart;

    [SerializeField] private SkinnedMeshRenderer[] avatarRenders;

    [SerializeField] private GameObject avatarManager;

    [SerializeField] private Color controllingAvatarColour;

    [SerializeField] private GameObject currantConstlation;

    private int currantAvatarCollorIndex;

    // Start is called before the first frame update
    void Start()
    {
        isCoroutine = false;
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
        activeAvartarIndex = 0;
        Sparkle.enabled = false;
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
        //gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawActive && activeStarIndex != constellation[turnNum].Length) {
            lineRenderers[turnNum].enabled = true;
            lineRenderers[turnNum].material.SetFloat("_alpha", 1f);

            if(Sparkle.enabled == false) { Sparkle.enabled = true; Sparkle.Play(); }


            Sparkle.SetTexture("texture", trans);
            Vector3 currentPos = lineRenderers[turnNum].GetPosition(lineRenderers[turnNum].positionCount - 1);
            Sparkle.SetVector3("spawnPosition", currentPos);
            Vector3 nextStarPos = constellation[turnNum][activeStarIndex].transform.position;
            Vector3 currentPosToStar = nextStarPos - currentPos;

            Vector3 moveDirection = avatars[activeAvartarIndex].leftHand.velocity;
            //if(moveDirection == Vector3.zero) { moveDirection = currentPosToStar; }
            
            Vector3 connectDir = ConnectDir(currentPosToStar, moveDirection);
            float magnifier = Unity.Mathematics.math.remap(0, 53, 0f, 2f, Unity.Mathematics.math.min(avatars[activeAvartarIndex].leftHand.speed, 53));
            magnifier = Mathf.Max(magnifier, 0f);
            Vector3 drawVector = connectDir * magnifier;
            //magnifier;
            //Debug.Log(string.Format("{0},{1},{2}", drawVector.x, drawVector.y, drawVector.z));
            drawVector = new Vector3 (drawVector.x, 0, drawVector.z);
            DrawLine(lineRenderers[turnNum], drawVector, constellation[turnNum][0].transform.position);
            IncreIndex(currentPos, nextStarPos);
            
        }
        if(!isDrawActive && activeStarIndex == constellation[turnNum].Length) {
            /*foreach (var vfx in galaxy) {
                vfx.playRate = 1f;
            }*/
            if (boomNum == turnNum && activeStarIndex!= 1) { starBoom.SendEvent("Wave"); boomNum++; Debug.Log("boomNum is now " + boomNum); }
            alp = alp<=0.1f?  0.1f : alp - (Time.deltaTime/5f);
            lineRenderers[turnNum].material.SetFloat("_alpha", alp);
            //lineMaterial.SetFloat("_alpha", alp);
            if (!isCoroutine) {
                StartCoroutine(AutoAnotherRounds());
            }
        }

        if (isDrawActive == true)
            endPointPart.gameObject.SetActive(true);
        else
            endPointPart.gameObject.SetActive(false);


        if (activeAvartarIndex == 0)
            currantAvatarCollorIndex = avatarManager.GetComponent<CharacterManager>().Char0ColorIndex;
        else
            currantAvatarCollorIndex = avatarManager.GetComponent<CharacterManager>().Char1ColorIndex;

        if (avatarManager.GetComponent<CharacterManager>().Char0ColorIndex == 0)
            controllingAvatarColour = avatarManager.GetComponent<CharacterManager>().colors[avatarManager.GetComponent<CharacterManager>().colors.Length -1];
        else
            controllingAvatarColour = avatarManager.GetComponent<CharacterManager>().colors[currantAvatarCollorIndex - 1];
            
        currantConstlation = constellation[turnNum][activeStarIndex].GetComponent<twinkle>().star;
        currantConstlation.GetComponent<MeshRenderer>().material.SetVector("_Color", controllingAvatarColour);
    }

    private Vector3 ConnectDir(Vector3 posToNextStar, Vector3 drawDriection)
    {
        if (Vector3.Angle(posToNextStar, drawDriection) <= 15f && Vector3.Angle(posToNextStar, drawDriection) >= -15f) {
            //Debug.Log(Vector3.Angle(posToNextStar, drawDriection));
            return drawDriection.normalized * 3f;
        }
        else
            return drawDriection.normalized * 1.2f;
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
        foreach (Transform child in allConstellation.transform) {
            GameObject.Destroy(child.gameObject);
        }
        Vector3[][] starList = new Vector3[maxTurn][];
        for (int i = 0; i < maxTurn; i++) {
            starList[i] = new Vector3[numStar];
        }
        constellation = new GameObject[maxTurn][];
        for (int i =0; i<maxTurn; i++) {
            constellation[i] = new GameObject[numStar];
        }
        for (int j = 0; j < maxTurn; j++) {
            GameObject ConStars = new GameObject();
            ConStars.name = "Constellation" + j;
            ConStars.transform.parent = allConstellation.transform;

            for (int i = 0; i < numStar; i++) {              
                starList[j][i] = new Vector3(Random.Range(-8f, 8f), 0f, Random.Range(-4f, 4f));
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
        currentP.y = 0f;
        endP.y = 0f;
        endPointPart.position = endP;
        if (Vector3.Distance(currentP, endP) <= 0.6f || Input.GetKeyDown(KeyCode.N)) {
            currantConstlation.GetComponent<MeshRenderer>().material.SetVector("_Color", Color.white);
            constellation[turnNum][activeStarIndex].GetComponent<twinkle>().period = 0.3f;
            constellation[turnNum][activeStarIndex].GetComponent<twinkle>()._scale = true;
            Debug.Log("Twinkle force switched");
            activeStarIndex++;
            Debug.Log("activeStarIndex is now " + activeStarIndex + "while ConstellationLength is" + constellation[turnNum].Length);
            if (activeStarIndex == constellation[turnNum].Length) {
                
                //constellationDrawer.SetTexture("texture", particleTex);
                foreach(var stars in constellation[turnNum]) {
                    stars.GetComponent<twinkle>().spike.SetActive(false);
                }
                Debug.Log("Turn" + turnNum +" Finished");
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

    public void toggleDraw(bool bo)
    {
        isDrawActive = bo;
    }

    public void anotherRound()
    {
        isDrawActive = true;
        activeStarIndex = 1;
        turnNum++;
        Debug.Log("ActiveStarIndex reset to" + activeStarIndex + " and turn numer is now"  + turnNum);
    }

    public void resetDrawing()
    {
        Sparkle.enabled = false;
        turnNum = 0;
        boomNum = 0;
        activeStarIndex = 1;
        foreach (var rend in lineRenderers) {
            rend.material.SetFloat("_alpha", 1);
            rend.enabled = false;
            rend.positionCount = 2;
            rend.SetPosition(0, Vector3.zero);
            rend.SetPosition(1, Vector3.zero);
        }
        GenerateConstellation(starNum);
        for (int i = 0; i < constellation.Length; i++) {
            for (int j = 0; j < constellation[i].Length; j++) {
                constellation[i][j].SetActive(false);
            }
        }
        isCoroutine = false;
        isDrawActive = false;

    }

    public void stopDrawing()
    {
        isDrawActive = false;
        foreach (var rend in lineRenderers) {
            rend.material.SetFloat("_alpha", 0.1f);
            Sparkle.enabled = false;
        }
        foreach (var stars in constellation[turnNum]) {
            stars.GetComponent<twinkle>().spike.SetActive(false);
        }
    }


    IEnumerator AutoAnotherRounds()
    {
        isCoroutine = true;
        yield return new WaitForSeconds(5);
        anotherRound();
        isCoroutine = false;
    }
}



