using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BombSpawner : MonoBehaviour
{
    //はねかた、横速度、位置がランダム
    [SerializeField]
    float interval;
    [SerializeField]
    float overTimeCoe;
    [SerializeField]
    float finalInterval;
    [SerializeField]
    PrimitiveTarget bomb;
    [SerializeField]
    BaseParticle bomFX;
    [SerializeField]
    BaseParticle bomStartFX;
    [SerializeField]
    TextMeshPro gotPoint;
    [SerializeField]
    Gage m_gage;

    //public int brokenBombsSum;
    // Start is called before the first frame update
    private float overTime;
    IEnumerator Start()
    {
        //最初にリソースを開放
        KoitanLib.ObjectPoolManager.Release(bomb);
        KoitanLib.ObjectPoolManager.Release(bomFX);
        KoitanLib.ObjectPoolManager.Release(bomStartFX);
        KoitanLib.ObjectPoolManager.Release(gotPoint);

        KoitanLib.ObjectPoolManager.Init(bomb, this, 30);
        KoitanLib.ObjectPoolManager.Init(bomFX, this, 10);
        KoitanLib.ObjectPoolManager.Init(bomStartFX, this, 10);
        KoitanLib.ObjectPoolManager.Init(gotPoint, this, 5);

        overTime = 0f;
        //brokenBombsSum = 0;
        yield return new WaitForSeconds(interval);
        while (true)
        {
            overTime += interval;
            //処理
            float randomX = Random.Range(-50f, 50f);

            PrimitiveTarget obj = KoitanLib.ObjectPoolManager.GetInstance<PrimitiveTarget>(bomb);
            if (obj != null)
            {
                obj.transform.position = new Vector2(randomX, 35f);
                //もし井戸の真ん中らへんなら速度ゼロ
                if (Mathf.Abs(obj.transform.position.x) < 10)
                {
                    obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
                else
                {
                    //違うなら左右に応じて速度入れる
                    obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-Mathf.Sign(randomX), 0) * Random.Range(2f, 10f);
                }
                obj.bounce = Random.Range(5f, 15f);
                //obj.m_bombSpawner = this;
            }

            //Debug.Log(overTime.ToString());
            yield return new WaitForSeconds(Mathf.Max(interval - overTimeCoe * overTime, finalInterval));
        }
    }

    // Update is called once per frame
    //void Update()
    //{
        //ゲージを上げる
        //m_gage.bombNumSetter(brokenBombsSum);
    //}
}
