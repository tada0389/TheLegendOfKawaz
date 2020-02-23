using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;

public class Anima2dBoneCreater : MonoBehaviour
{
    public Transform rootBone;

    [ContextMenu("CreateBone")]
    public void CreateBoneRoot()
    {
        CreateBone(rootBone);
        SetBoneChild(rootBone);
    }

    void CreateBone(Transform parent)
    {
        parent.gameObject.AddComponent<Bone2D>();
        foreach (Transform child in parent)
        {
            //Debug.Log(child.name);            
            CreateBone(child);
        }
    }

    void SetBoneChild(Transform parent)
    {
        Bone2D bone = parent.GetComponent<Bone2D>();
        if (parent.childCount > 0)
        {
            bone.child = parent.GetChild(0).GetComponent<Bone2D>();
        }
        foreach (Transform child in parent)
        {                        
            SetBoneChild(child);
        }
    }
}
