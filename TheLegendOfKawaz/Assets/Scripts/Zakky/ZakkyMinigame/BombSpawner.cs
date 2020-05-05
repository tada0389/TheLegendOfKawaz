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
    IEnumerator Start()
    {
        yield return new WaitForSeconds(interval);
        while (true)
        {
            //処理
            float randomX = Random.Range(-20f, 20f);
            GameObject obj = Instantiate(bomb,
                new Vector2(randomX, 15f),
                Quaternion.Euler(Vector2.zero));
            obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-Mathf.Sign(randomX), 0) * Random.Range(2f, 5f);
            obj.GetComponent<Bomb>().bounce = Random.Range(5f, 15f);

            yield return new WaitForSeconds(interval);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
