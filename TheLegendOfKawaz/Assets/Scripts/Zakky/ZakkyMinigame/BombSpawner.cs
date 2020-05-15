using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    //はねかた、横速度、位置がランダム
    [SerializeField]
    private float interval;
    [SerializeField]
    private Bomb bomb;
    [SerializeField]
    BaseParticle bomFX;
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
        KoitanLib.ObjectPoolManager.Init(bomb, this, 50);
        KoitanLib.ObjectPoolManager.Init(bomFX, this, 20);

        overTime = 0f;
        //brokenBombsSum = 0;
        yield return new WaitForSeconds(interval);
        while (true)
        {
            overTime += interval;
            //処理
            float randomX = Random.Range(-50f, 50f);

            Bomb obj = KoitanLib.ObjectPoolManager.GetInstance<Bomb>(bomb);
                //Instantiate(bomb,
                //new Vector2(randomX, 35f),
                //Quaternion.Euler(Vector2.zero));
            if (obj != null)
            {
                obj.transform.position = new Vector2(randomX, 35f);
                obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-Mathf.Sign(randomX), 0) * Random.Range(2f, 10f);
                obj.bounce = Random.Range(5f, 15f);
                obj.m_bombSpawner = this;
            }

            //Debug.Log(overTime.ToString());
            yield return new WaitForSeconds(Mathf.Max(interval - 0.05f * overTime, 1f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //ゲージを上げる
        //m_gage.bombNumSetter(brokenBombsSum);
    }
}
