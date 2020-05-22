using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // コレ重要

public class KawaCoin : PrimitiveTarget
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        ColTriggerAction(col);
    }

    protected override void ColTriggerAction(Collider2D col)
    {
        /*
        //床に当たったら床にとどまる
        if (col.tag == "KawazCeil")
        {
            //速度反転
            //Vector3 vec = m_rigidbody2D.velocity;
            //vec.y = bounce;
            //m_rigidbody2D.velocity = vec;

            //col.gameObject.SetActive(false);
            //爆破もする
            //bloom関数呼ぶ
            m_miniPostProcessing.ExplotionLight();
            //消滅エフェクト
            TadaLib.EffectPlayer.Play(bomStartFX, transform.position, Vector3.zero);

            int score = 0;
            GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), score);
            //used = falseをする
            gameObject.SetActive(false);
        }
        */
        if (col.tag == "KawazWall" || col.tag == "Player") //プレイヤーか井戸に当たったら吸収
        {
            //スコア増やす
            int score = 50;
            m_scoreText.ScoreAdder(score);
            //ゲットしたポイントをその場に表示
            GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), score);
            //bloom関数呼ぶ
            m_miniPostProcessing.ExplotionLight();
            //消滅
            gameObject.SetActive(false);
        }
        else if (col.tag == "KawaztanShot")
        {
            //スコア減らす
            int score = -50;
            m_scoreText.ScoreAdder(score);
            GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), score);
            m_miniPostProcessing.ExplotionLight();
            TadaLib.EffectPlayer.Play(bomFX, transform.position, Vector3.zero);

            gameObject.SetActive(false);
            /*
            //ゲージふやす
            m_gage.bombNumIncrimenter();
            //スコア増やす
            m_scoreText.ScoreAdder(150);

            //bloom関数呼ぶ
            m_miniPostProcessing.ExplotionLight();
            TadaLib.EffectPlayer.Play(bomFX, transform.position, Vector3.zero);

            {
                //テキスト呼び出す

                //Color colo = txt.color;
                //colo.a = 1f;
                //txt.color = colo;
                GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), 150);
                //txt.transform.position = transform.position;
                //txt.transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            //Debug.Log("false");
            */
        }
        else if (col.tag == "ToumeiStage")
        {

        }
        else
        {
            //速度反転
            //col.gameObject.GetComponent<Rigidbody2D>().velocity = -col.gameObject.GetComponent<Rigidbody2D>().velocity;
            //Vector3 vec = m_rigidbody2D.velocity;
            //vec.y = base.bounce;
            //m_rigidbody2D.velocity = vec;

            //bloom関数呼ぶ
            m_miniPostProcessing.ExplotionLight();
            //消滅エフェクト
            TadaLib.EffectPlayer.Play(bomStartFX, transform.position, Vector3.zero);

            int score = 0;
            GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), score);
            //used = falseをする
            gameObject.SetActive(false);
        }
    }
}
