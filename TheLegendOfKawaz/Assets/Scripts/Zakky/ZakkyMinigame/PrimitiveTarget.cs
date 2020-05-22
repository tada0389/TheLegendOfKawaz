using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // コレ重要
using TMPro;

public class PrimitiveTarget : MonoBehaviour
{
    //y方向の最大の速さ
    [SerializeField]
    private float maxVelo;
    //ボム爆発エフェクト
    [SerializeField]
    protected BaseParticle bomFX;
    //ボム発生エフェクト
    [SerializeField]
    protected BaseParticle bomStartFX;
    //ボム爆発時ゲットポイントテキスト
    [SerializeField]
    protected TextMeshPro gotPoint;

    //ポストプロセシング
    protected MiniGamePostProcessing m_miniPostProcessing;
    //フィーバーゲージ
    protected Gage m_gage;
    //スコアテキスト
    protected ScoreText m_scoreText;
    //rigidbody2D
    protected Rigidbody2D m_rigidbody2D;

    //跳ねやすさ
    [HideInInspector]
    public float bounce;
    //[HideInInspector]
    //public BombSpawner m_bombSpawner;
    // Start is called before the first frame update
    protected void Start()
    {
        GetComponenter();

        //最初のまがまがしい登場エフェクト
        TadaLib.EffectPlayer.Play(bomStartFX, transform.position, Vector3.zero);

    }

    // Update is called once per frame
    protected void Update()
    {
        TeritoryCheck();
    }

    /*
    private void OnTriggerEnter2D(Collider2D col)
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
            m_gage.bombNumIncrimenter();
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
                GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero));
                //txt.transform.position = transform.position;
                //txt.transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            Debug.Log("false");
            gameObject.SetActive(false);
        }
        else if (col.tag == "ToumeiStage")
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
    */

    protected virtual void ColTriggerAction(Collider2D col)
    {

    }

    void GetComponenter()
    {
        //Rigidbody2D、MiniGamePostProcessing、Gage、ScoreTextとる
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_miniPostProcessing = GameObject.Find("PostProcessVolume").GetComponent<MiniGamePostProcessing>();
        m_gage = GameObject.Find("Gage").GetComponent<Gage>();
        m_scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();
    }

    void TeritoryCheck()
    {
        //一定以上落ちてたら消す
        if (transform.position.y < -10f) gameObject.SetActive(false);
        {
            Vector2 vec = m_rigidbody2D.velocity;
            if (vec.y < -maxVelo) vec.y = -maxVelo;
            m_rigidbody2D.velocity = vec;
        }
    }
}
