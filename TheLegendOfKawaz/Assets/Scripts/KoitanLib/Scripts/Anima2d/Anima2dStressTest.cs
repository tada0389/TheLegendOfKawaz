using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

public class Anima2dStressTest : MonoBehaviour
{
    [SerializeField]
    private GameObject leftObj;
    [SerializeField]
    private GameObject rightObj;
    //[SerializeField]
    //private TimeLimitObject eff;
    private RaycastHit2D hit;
    private List<GameObject> objList = new List<GameObject>();

    private void Start()
    {
        //ObjectPoolManager.Init(eff, this, 10);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos); //マウスのポジションを取得してRayに代入        

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
            foreach (GameObject o in objList)
            {
                Destroy(o);
            }
            objList.Clear();
        }
    }
}
