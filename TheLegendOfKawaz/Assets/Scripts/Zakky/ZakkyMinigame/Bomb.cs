using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // コレ重要

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float maxVelo;
    [SerializeField]
    BaseParticle bomFX;
    [SerializeField]
    BaseParticle bomStartFX;

    MiniGamePostProcessing m_miniPostProcessing;
    Gage m_gage;
    ScoreText m_scoreText;

    private Rigidbody2D m_rigidbody2D;
    [HideInInspector]
    public float bounce;
    [HideInInspector]
    public BombSpawner m_bombSpawner;
    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_miniPostProcessing = GameObject.Find("PostProcessVolume").GetComponent<MiniGamePostProcessing>();
        m_gage = GameObject.Find("Gage").GetComponent<Gage>();
        m_scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();
        TadaLib.EffectPlayer.Play(bomStartFX, transform.position, Vector3.zero);

    }

    // Update is called once per frame
    void Update()
    {
        {
            Vector2 vec = m_rigidbody2D.velocity;
            if (vec.y < -maxVelo) vec.y = -maxVelo;
            m_rigidbody2D.velocity = vec;
        }
    }

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
}
