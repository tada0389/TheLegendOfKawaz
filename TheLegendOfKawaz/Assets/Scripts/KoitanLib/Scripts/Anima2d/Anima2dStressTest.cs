﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anima2dStressTest : MonoBehaviour
{
    [SerializeField]
    private GameObject leftObj;
    [SerializeField]
    private GameObject rightObj;
    private RaycastHit2D hit;
    private List<GameObject> objList = new List<GameObject>();    

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //マウスのポジションを取得してRayに代入
        pos.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(leftObj, pos, Quaternion.identity);
            obj.SetActive(true);
            objList.Add(obj);
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject obj = Instantiate(rightObj, pos, Quaternion.identity);
            obj.SetActive(true);
            objList.Add(obj);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach(GameObject o in objList)
            {
                Destroy(o);                
            }
            objList.Clear();
        }        
    }
}