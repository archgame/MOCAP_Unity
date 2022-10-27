using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using HSVPicker;

public class CharacterManager : MonoBehaviour
{
    public GameObject[] Char0Avatars;
    public int Char0Index = 0;
    public int Char0ColorIndex = 0;
    public Material Char0Mat;
    public GameObject[] Char1Avatars;
    public int Char1Index = 0;
    public int Char1ColorIndex = 0;
    public Material Char1Mat;
    public Color[] colors;
    public MeshRenderer Renderer;

    // Start is called before the first frame update
    private void Start()
    {
        NextChar(0);
        NextChar(1);

        NextColor(0);
        NextColor(1);

        /*/ Wahei Color Picker Set Color Example
        ColorPicker[] pickers = GameObject.FindObjectsOfType<ColorPicker>(); Debug.Log("pickers: " + pickers.Length);
        foreach (var picker in pickers)
        {
            picker.AssignColor(Color.black);
        }
        //*/
    }

    public void NextChar(int avatarIndex)
    {
        //get the character to act on
        GameObject[] avatars = Char0Avatars;
        int index = Char0Index;
        if (avatarIndex == 1)
        {
            avatars = Char1Avatars;
            index = Char1Index;
        }

        //update all the avatars
        for (int i = 0; i < avatars.Length; i++)
        {
            var go = avatars[i];
            if (i == index)
            {
                SetSkinnedMeshChildren(go, true);
                continue;
            }
            SetSkinnedMeshChildren(go, false);
        }

        //update global variable
        index++; //get the next index for the next avatar
        if (index >= avatars.Length) { index = 0; } //reset index if we are at the last avatar
        if (avatarIndex == 1)
        {
            Char1Index = index;
        }
        else
        {
            Char0Index = index;
        }
    }

    public void NextColor(int avatarIndex)
    {
        //get the character to act on
        Material mat = Char0Mat;
        int index = Char0ColorIndex;
        if (avatarIndex == 1)
        {
            mat = Char1Mat;
            index = Char1ColorIndex;
        }

        //update material colors
        Color color = colors[index];
        mat.color = color;
        mat.SetColor("_EmissionColor", color);

        //update ring colors
        if (Renderer != null)
        {
            Material ring = Renderer.material;
            string ava = "_ava0GridColor";
            if (avatarIndex == 1) { ava = "_ava1GridColor"; }
            ring.SetColor(ava, color);
        }

        //update global variable
        index++; //get the next index for the next avatar
        if (index >= colors.Length) { index = 0; } //reset index if we are at the last avatar
        if (avatarIndex == 1) { Char1ColorIndex = index; }
        else { Char0ColorIndex = index; }
    }

    public void SetSkinnedMeshChildren(GameObject go, bool set)
    {
        foreach (Transform child in go.transform)
        {
            var skins = child.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var skin in skins)
            {
                skin.enabled = set;
            }
        }
    }
}