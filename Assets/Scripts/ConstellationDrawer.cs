using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ConstellationDrawer : MonoBehaviour
{
    public bool isDrawActive;
    private int activeAvartarIndex;
    public int starNum=5;
    private DataSubscription.Avatar[] avatars;

    private LineRenderer lineRenderer;
    public Material lineMaterial;
    public Material starMaterial;

    public GameObject radarPulse;
    private GameObject[] radarPulses;

    public VisualEffect constellationDrawer;

    private GameObject[] constellation;

    private int activeStarIndex;

    [SerializeField]
    private DataSubscription data;
    // Start is called before the first frame update
    void Start()
    {
        isDrawActive = false;
        avatars = new DataSubscription.Avatar[2] { data.avatar0, data.avatar1 };
        radarPulses = new GameObject[starNum];
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.material = lineMaterial;
        activeStarIndex = 1;
        activeAvartarIndex = 1;
        constellationDrawer.Stop();
        //lineRenderer.material = lineMaterial;
        //StartCoroutine(DrawWithTime());
        GenerateConstellation(starNum);
        //renderConstellation(stars);
        //StartCoroutine(reportStarEndPos(lineRenderer));

    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawActive) {
            lineRenderer.enabled = true;
            constellationDrawer.Play();
            //Vector3 newPos = new Vector3(Time.time, 0, Time.time );
            Vector3 currentPos = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            constellationDrawer.SetVector3("spawnPosition", currentPos);
            Vector3 nextStarPos = constellation[activeStarIndex].transform.position;
            Vector3 currentPosToStar = nextStarPos - currentPos;

            Vector3 moveDirection = avatars[activeAvartarIndex].leftHand.velocity;
            //if(moveDirection == Vector3.zero) { moveDirection = currentPosToStar; }
            
            Vector3 connectDir = ConnectDir(currentPosToStar, moveDirection);
            float magnifier = Unity.Mathematics.math.remap(0, 25, 0.1f, 0.3f, Unity.Mathematics.math.min(data.avatar0.leftHand.speed, 25));
            Vector3 drawVector = connectDir * magnifier;
            //Debug.Log(string.Format("{0},{1},{2}", drawVector.x, drawVector.y, drawVector.z));
            DrawLine(lineRenderer, drawVector, constellation[0].transform.position);
            radarPulses[activeStarIndex].GetComponent<RadarPulse>().maxScale = 6f;
            IncreIndex(currentPos, nextStarPos);
            
        }

    }

    private Vector3 ConnectDir(Vector3 posToNextStar, Vector3 drawDriection)
    {
        if (Vector3.Angle(posToNextStar, drawDriection) <= 15f && Vector3.Angle(posToNextStar, drawDriection) >= -15f) {
            //Debug.Log(Vector3.Angle(starLine, drawDriection));
            return drawDriection.normalized * 2f;
        }
        else
            return drawDriection.normalized * 0.1f;
    }
    private void DrawLine(LineRenderer lineRend, Vector3 drawVector, Vector3 startPos)
    {
        if (lineRend.positionCount == 2) {

            lineRend.SetPosition(0, startPos);
            lineRend.SetPosition(1, startPos);
            constellation[0].SetActive(true);
            instanceRadarWithScale(0, 3f);
            constellation[1].SetActive(true);
            instanceRadarWithScale(1, 3f);
            //Debug.Log("start position set as " + lineRend.GetPosition(0) + " and " + lineRend.GetPosition(1));
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
        Vector3[] starList = new Vector3[numStar];
        constellation = new GameObject[numStar];
        for (int i = 0; i < numStar; i++) {
            starList[i] = new Vector3(Random.Range(-6f, 6f), 0f, Random.Range(-6f, 6f));

            constellation[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            constellation[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            constellation[i].GetComponent<MeshRenderer>().material = starMaterial;
            constellation[i].transform.position = starList[i];
            constellation[i].name = "Star" + i;
            constellation[i].SetActive(false);
        }
    }

    private void IncreIndex(Vector3 currentP, Vector3 endP)
    {
        if (Vector3.Distance(currentP, endP) <= 0.1f) {
            //if (activeStarIndex == maxNum) {tigger, 4seconds, disppear; }
            radarPulses[activeStarIndex ].GetComponent<RadarPulse>().maxScale = 3f;
            activeStarIndex++;
            if (activeStarIndex == constellation.Length) { isDrawActive = false; return; }
            instanceRadarWithScale(activeStarIndex, 6f);
            activeAvartarIndex = activeAvartarIndex == 0 ? 1 : 0;
            Debug.Log("activeAvatarIndex is now " + activeAvartarIndex);
            Debug.Log("activeStarIndex is now " + activeStarIndex);
            constellation[activeStarIndex].SetActive(true);
        }
    }
    /*private void renderConstellation(Vector3[] starList)
    {
        for (int i = 0; i < starList.Length; i++) {
            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            star.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            star.GetComponent<MeshRenderer>().material = starMaterial;
            star.transform.position = starList[i];
        }
    }*/

    private void instanceRadarWithScale(int insNum,  float maxScale)
    {
        radarPulses[insNum] = Instantiate(radarPulse);
        radarPulses[insNum].GetComponent<RadarPulse>().maxScale = maxScale;
        radarPulses[insNum].GetComponent<RadarPulse>().trans = constellation[insNum].transform;
    }



    public IEnumerator reportStarEndPos (LineRenderer lRend )
    {
        while (true) {
            yield return new WaitForSeconds(5f);
            Vector3 endLoc = lRend.GetPosition(lRend.positionCount - 1);
            float dist = Vector3.Distance(endLoc, constellation[activeStarIndex].transform.position);
            Debug.Log("The current location of the end point is " + endLoc);
            Debug.Log("The current distance to star#" + (activeStarIndex)  + "is" + dist);
        }
    }

}



