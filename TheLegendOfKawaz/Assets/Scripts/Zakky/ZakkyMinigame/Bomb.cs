﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // コレ重要
using WallDefence;
//using TMPro;

public class Bomb : PrimitiveTarget
{
    // Start is called before the first frame update
    //void Start()
    //{
    //    base.Start();
    //}

    // Update is called once per frame
    //void Update()
    //{
    //    base.Update();
    //}

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
            // 他よりも軽くする
            CameraSpace.CameraShaker.Shake(0.3f, 0.2f);
        }
        else if (col.tag == "KawazWall" || col.tag == "Player") //プレイヤーか井戸に当たったらまけ
        {
            //ダメージ
            if (Game.instance.state == Game.STATE.MOVE)
            {
                Actor.Player.Player ply = GameObject.Find("NewKawazTan").GetComponent<Actor.Player.Player>();
                {
                    ply.Damage(5);
                    if (ply.IsDead())
                    {
                        Game.instance.state = Game.STATE.GAMEOVER;
                    }
                }
            }
            else if (Game.instance.state == Game.STATE.FEVER)
            {
                int score = 60;
                m_scoreText.ScoreAdder(score);
                //テキスト呼び出す
                GotPoint.PointInit(gotPoint, transform.position, Quaternion.Euler(Vector3.zero), score);
            }
            //爆弾ばくはつ
            Explotion();

            CameraSpace.CameraShaker.Shake(0.7f, 0.3f);
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

            // 他よりも軽くする
            CameraSpace.CameraShaker.Shake(0.3f, 0.2f);
        }
        else if (col.tag == "ToumeiStage" || col.tag == "Bomb" || col.tag == "Coin")
        {

        }
        else if(col.tag == "DropZone") // by tada
        {
            // 消滅 
            gameObject.SetActive(false);
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
