using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public GameObject[] Char0Avatars;
    public int Char0Index = 0;
    public GameObject[] Char1Avatars;
    public int Char1Index = 0;

    // Start is called before the first frame update
    private void Start()
    {
        NextChar(0);
        NextChar(1);
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