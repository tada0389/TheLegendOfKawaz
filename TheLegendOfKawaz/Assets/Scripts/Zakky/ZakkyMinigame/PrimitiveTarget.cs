﻿using System.Collections;
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

    // 既に必要なコンポーネントを取得しているか
    bool IsInited = false;

    //跳ねやすさ
    [HideInInspector]
    public float bounce;

    // ミニマップに登録されているか by tada めちゃくちゃ強引でごめん
    public bool IsRegisteredToMiniMap = false;

    // 初期化で必要なオブジェクトを取得する by tada
    public void Init(MiniGamePostProcessing mini_post, Gage gage, ScoreText text)
    {
        if (IsInited) return;
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_miniPostProcessing = mini_post;
        m_gage = gage;
        m_scoreText = text;

        IsInited = true;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        //最初のまがまがしい登場エフェクト
        TadaLib.EffectPlayer.Play(bomStartFX, transform.position, Vector3.zero);

    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        TeritoryCheck();
    }

    //接触時に呼び出す関数
    protected virtual void ColTriggerAction(Collider2D col)
    {

    }

    //void GetComponenter()
    //{
    //    //Rigidbody2D、MiniGamePostProcessing、Gage、ScoreTextとる
    //    m_rigidbody2D = GetComponent<Rigidbody2D>();
    //    m_miniPostProcessing = GameObject.Find("PostProcessVolumeIdo").GetComponent<MiniGamePostProcessing>();
    //    m_gage = GameObject.Find("Gage").GetComponent<Gage>();
    //    m_scoreText = GameObject.Find("ScoreText").GetComponent<ScoreText>();
    //}

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
