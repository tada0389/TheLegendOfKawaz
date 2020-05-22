using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // コレ重要
//using TMPro;

public class Bomb : PrimitiveTarget
{
    /*
    [SerializeField]
    private float maxVelo;
    [SerializeField]
    BaseParticle bomFX;
    [SerializeField]
    BaseParticle bomStartFX;
    [SerializeField]
    TextMeshPro gotPoint;

    MiniGamePostProcessing m_miniPostProcessing;
    Gage m_gage;
    ScoreText m_scoreText;

    private Rigidbody2D m_rigidbody2D;
    [HideInInspector]
    public float bounce;
    [HideInInspector]
    public BombSpawner m_bombSpawner;
    */
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        /*
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_miniPostProcessing = GameObject.Find("PostProcessVolume").GetComponent<MiniGamePostProcessing>();
        m_gage = GameObject.Find("Gage").GetComponent<Gage>();
        m_scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();

        //最初のまがまがしい登場エフェクト
        TadaLib.EffectPlayer.Play(bomStartFX, transform.position, Vector3.zero);
        */
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        /*
        //一定以上落ちてたら消す
        if (transform.position.y < -10f) gameObject.SetActive(false);
        {
            Vector2 vec = m_rigidbody2D.velocity;
            if (vec.y < -maxVelo) vec.y = -maxVelo;
            m_rigidbody2D.velocity = vec;
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        ColTriggerAction(col);
    }

    protected override void ColTriggerAction(Collider2D col)
    {
        //床に当たったら床ぶっ壊す
        if (col.tag == "KawazCeil")
        {
            col.gameObject.SetActive(false);
            //爆破もする
            //bloom関数呼ぶ
            m_miniPostProcessing.ExplotionLight();
            //爆破エフェクト
            TadaLib.EffectPlayer.Play(bomFX, transform.position, Vector3.zero);

            //TadaLib.EffectPlayer.Play(gotPoint, transform.position, Vector3.zero);
            //used = falseをする
            gameObject.SetActive(false);
        }
        else if (col.tag == "KawazWall" || col.tag == "Player") //プレイヤーか井戸に当たったらまけ
        {
            //まけ
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (col.tag == "KawaztanShot")
        {
            //ゲージふやす
            m_gage.bombNumAdder(1f);
            //スコア増やす
            m_scoreText.ScoreAdder(30);

            //bloom関数呼ぶ
            m_miniPostProcessing.ExplotionLight();
            TadaLib.EffectPlayer.Play(bomFX, transform.position, Vector3.zero);

            {
                //テキスト呼び出す

                //Color colo = txt.color;
                //colo.a = 1f;
                //txt.color = colo;
                GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), 30);
                //txt.transform.position = transform.position;
                //txt.transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            Debug.Log("false");
            gameObject.SetActive(false);
        }
        else if (col.tag == "ToumeiStage" || col.tag == "Enemy")
        {

        }
        else
        {
            //速度反転
            //col.gameObject.GetComponent<Rigidbody2D>().velocity = -col.gameObject.GetComponent<Rigidbody2D>().velocity;
            Vector3 vec = m_rigidbody2D.velocity;
            vec.y = bounce;
            m_rigidbody2D.velocity = vec;
        }
    }
}
