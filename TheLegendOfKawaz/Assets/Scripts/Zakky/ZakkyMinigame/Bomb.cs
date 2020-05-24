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
            if (Game.instance.state == Game.STATE.MOVE) col.gameObject.SetActive(false);
            //else if (Game.instance.state == Game.STATE.FEVER) m_scoreText.ScoreAdder(30);
            //爆破もする
            Explotion();
        }
        else if (col.tag == "KawazWall" || col.tag == "Player") //プレイヤーか井戸に当たったらまけ
        {
            //ダメージ
            if (Game.instance.state == Game.STATE.MOVE) GameObject.Find("NewKawazTan").GetComponent<Actor.Player.Player>().Damage(5);
            else if (Game.instance.state == Game.STATE.FEVER)
            {
                int score = 60;
                m_scoreText.ScoreAdder(score);
                //テキスト呼び出す
                GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), score);
            }
            //爆弾ばくはつ
            Explotion();
        }
        else if (col.tag == "KawaztanShot")
        {
            //ゲージふやす
            m_gage.bombNumAdder(1f);
            int score = 30;
            if (Game.instance.state == Game.STATE.FEVER) score = 60;

            //スコア増やす
            m_scoreText.ScoreAdder(score);

            {
                //テキスト呼び出す
                GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), score);
            }
            Explotion();
        }
        else if (col.tag == "ToumeiStage" || col.tag == "Enemy")
        {

        }
        else
        {
            //速度反転
            Vector3 vec = m_rigidbody2D.velocity;
            vec.y = bounce;
            m_rigidbody2D.velocity = vec;
        }
    }

    void Explotion()
    {
        //bloom関数呼ぶ
        m_miniPostProcessing.ExplotionLight();
        //爆破エフェクト
        TadaLib.EffectPlayer.Play(bomFX, transform.position, Vector3.zero);
        gameObject.SetActive(false);
    }
}
