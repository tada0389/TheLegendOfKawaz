using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    //はねかた、横速度、位置がランダム
    [SerializeField]
    private float interval;
    [SerializeField]
    private GameObject bomb;
    // Start is called before the first frame update
    private float overTime;
    IEnumerator Start()
    {
        overTime = 0f;
        yield return new WaitForSeconds(interval);
        while (true)
        {
            overTime += Time.deltaTime;
            //処理
            float randomX = Random.Range(-50f, 50f);
            GameObject obj = Instantiate(bomb,
                new Vector2(randomX, 15f),
                Quaternion.Euler(Vector2.zero));
            obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-Mathf.Sign(randomX), 0) * Random.Range(2f, 10f);
            obj.GetComponent<Bomb>().bounce = Random.Range(5f, 15f);

            yield return new WaitForSeconds(Mathf.Max(interval - 0.05f * overTime, 1f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
