using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// すべてのパーティクルにアタッチする基底クラス
/// (ParticleSystemはMonobehaviorを継承していないため作成した) ←なぜアタッチできるの！？
/// </summary>

[RequireComponent(typeof(ParticleSystem))]
public class BaseParticle : MonoBehaviour
{
    [SerializeField]
    private float life_time_ = 2.0f;

    private Timer timer_;

    private ParticleSystem particle_;

    private void Awake()
    {
        timer_ = new Timer(life_time_);
        particle_ = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        timer_.TimeReset();
        particle_.Play();
    }

    private void Update()
    {
        if (timer_.IsTimeout())
        {
            gameObject.SetActive(false);
        }
    }
}
