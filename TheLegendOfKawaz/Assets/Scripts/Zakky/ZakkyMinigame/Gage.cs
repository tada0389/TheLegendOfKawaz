using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gage : MonoBehaviour
{
    [SerializeField]
    LineRenderer BHPBar;
    [SerializeField]
    Ciel m_ciel;
    [SerializeField]
    ParticleSystem m_kamihubuki;

    float bombNum;
    float gageNum = 0f;

    // Start is called before the first frame update
    void Start()
    {
        bombNum = 0f;
        gageNum = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        GageMaxCheck();

        GagePict();
    }

    public void bombNumAdder(float num)
    {
        //フィーバー状態のときはゲージ増やさない
        if (Game.instance.state == Game.STATE.FEVER) return;

        bombNum += num;
        if (bombNum < 0) bombNum = 0;
    }

    void GageMaxCheck()
    {
        if (bombNum >= 20f)
        {
            bombNum = 0f;
            gageNum = 20f;
            Game.instance.state = Game.STATE.FEVER;
            Color col = new Color(1f, 1f, 0f);
            BHPBar.startColor = col;
            BHPBar.endColor = col;
            m_ciel.CielRespawner();
            //紙吹雪降らせる
            m_kamihubuki.Play();
        }
    }

    void GagePict()
    {
        if (Game.instance.state == Game.STATE.FEVER)
        {
            gageNum -= 20f * (Time.deltaTime / 8f);
            //2秒前になったら紙吹雪止める
            if (gageNum < 20f * 2f / 8f) m_kamihubuki.Stop();
            if (gageNum <= 0f)
            {
                gageNum = 0f;
                Color col = new Color(0.6f, 0.6f, 1f);
                BHPBar.startColor = col;
                BHPBar.endColor = col;
                Game.instance.state = Game.STATE.MOVE;
            }
        }
        else if (Game.instance.state == Game.STATE.MOVE)
        {
            //ゲージをぬるっとふやす
            DOTween.To(
            () => gageNum,          // 何を対象にするのか
            num => gageNum = num,   // 値の更新
            bombNum,                  // 最終的な値
            0.5f                  // アニメーション時間
            );
            Color col = new Color(0.6f, 0.6f, 1f);
            if (bombNum - gageNum > 0.01f) col = Color.green * 0.8f;
            else if (bombNum - gageNum < -0.01f) col = Color.red * 0.8f;
            BHPBar.startColor = col;
            BHPBar.endColor = col;
        }
        BHPBar.SetPosition(0, new Vector3(10f * gageNum / 20, 0.0f, 0.0f));
    }
}
