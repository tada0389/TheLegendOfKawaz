using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // コレ重要
using WallDefence;

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
        if (col.tag == "Player") //プレイヤーに当たったら吸収
        {
            //ゲージふやす
            m_gage.bombNumAdder(0.3f);
            //スコア増やす
            int score = 50;
            if (Game.instance.state == Game.STATE.FEVER) score = 100;

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
            int score = -50;
            if (Game.instance.state == Game.STATE.MOVE)
            {
                //ゲージ減らす
                m_gage.bombNumAdder(-2f);
            }
            else if (Game.instance.state == Game.STATE.FEVER)
            {
                score = 100;
            }
            
            m_scoreText.ScoreAdder(score);
            GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), score);
            m_miniPostProcessing.ExplotionLight();
            TadaLib.EffectPlayer.Play(bomFX, transform.position, Vector3.zero);

            gameObject.SetActive(false);
        }
        else if (col.tag == "ToumeiStage" || col.tag == "Bomb" || col.tag == "Coin")
        {

        }
        else
        {
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
