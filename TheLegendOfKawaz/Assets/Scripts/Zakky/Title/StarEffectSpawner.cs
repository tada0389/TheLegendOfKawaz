using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarEffectSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] star;
    List<SpriteFadeout> starSpriteFadeout = new List<SpriteFadeout>();
    List<Rigidbody2D> starRigidbody2D = new List<Rigidbody2D>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < star.Length; i++)
        {
            //star[i] = chileTransform.gameObject;
            starSpriteFadeout.Add(star[i].GetComponent<SpriteFadeout>());
            starRigidbody2D.Add(star[i].GetComponent<Rigidbody2D>());
            star[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StarEffect(Vector2 pos)
    {
        //星エフェクト出す
        //なんかparticle生成
        //角度とスピードランダムで取得
        // 子オブジェクトを全て取得する

        for (int i = 0; i < star.Length; i++)
        {
            star[i].SetActive(true);
            starSpriteFadeout[i].ReStart();
            star[i].transform.position = pos;
            float dir = Random.Range(0, 359);
            float spd = Random.Range(1f, 5.0f);
            //それらを代入
            Vector2 vec2 = new Vector2(Mathf.Cos(dir * Mathf.Deg2Rad), Mathf.Sin(dir * Mathf.Deg2Rad)) * spd;
            starRigidbody2D[i].velocity = vec2;
        }
    }
}
