using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bullet;
using KoitanLib;

public class PoolTest : MonoBehaviour
{
    [SerializeField]
    int maxNum;
    [SerializeField]
    GravityBullet bullet;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //オブジェクト登録
        if (Input.GetKeyDown(KeyCode.I))
        {
            ObjectPoolManager.Init(bullet, this, maxNum);
        }

        //オブジェクト表示
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GravityBullet g = ObjectPoolManager.GetInstance<GravityBullet>(bullet);
            g.transform.position = transform.position;
            Debug.Log(g.gameObject.name);
        }

        //オブジェクト破棄
        if (Input.GetKeyDown(KeyCode.R))
        {
            ObjectPoolManager.Release(bullet);
        }
    }
}
