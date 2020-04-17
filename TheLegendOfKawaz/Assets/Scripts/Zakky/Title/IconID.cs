using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconID : MonoBehaviour
{

    public GameObject titleIDManager;
    public GameObject titleSceneManager;
    public StarEffectSpawner starEffectSpawner;
    //public GameObject titleState;
    public GameObject m_particle;

    //MyIDで番号を決める
    public int MyID;
    private float θ;
    private float ω;

    // Use this for initialization
    void Start()
    {
        //titleIDManager = GameObject.Find("TitleScript");   //スクリプト管理するEmptyObjectを取得

        θ = 0f;
        ω = 1.5f;
    }
    
    // Update is called once per frame
    void Update()
    {


        if (titleIDManager.GetComponent<TitleIDManager>().ChoiceID == MyID)
        {    //今選ばれてるButtonIDが自分のと一致するなら
            θ += ω * Time.deltaTime;

            this.transform.localScale = new Vector2(2f, 2f);
        }
        else
        {    //IDが一致しないなら
            θ = 0f;
            this.transform.localScale = new Vector2(1.5f, 1.5f);

        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 10f * Mathf.Sin(θ)));
    }

    void OnMouseEnter()
    {
        //のったときMyIDからChoiceIDを変える
        titleIDManager.GetComponent<TitleIDManager>().ChoiceID = MyID;
    }

    void OnMouseDown()
    {
        //starEffectSpawner.StarEffect(transform.position);
        //ロードする関数を呼び出す
        titleSceneManager.GetComponent<ZakkyTitleSceneManager>().IsLoadScene();
    }

    //public void StarEffect()
    //{
    //    //星エフェクト出す
    //    //なんかparticle生成
    //    for (int i = 0; i < 16; i++)
    //    {
    //        GameObject obj = Instantiate(m_particle, transform.position, Quaternion.Euler(Vector3.zero));
    //        //角度とスピードランダムで取得
    //        float dir = Random.Range(0, 359);
    //        float spd = Random.Range(1f, 5.0f);
    //        //それらを代入
    //        Vector2 vec2 = new Vector2(Mathf.Cos(dir * Mathf.Deg2Rad), Mathf.Sin(dir * Mathf.Deg2Rad)) * spd;
    //        obj.GetComponent<Rigidbody2D>().velocity = vec2;
    //    }
    //}
}
