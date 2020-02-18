using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bullet;
using TadaLib;

/// <summary>
/// プレイヤーが放つ通常弾
/// </summary>

namespace Bullet
{
    public class NormalBullet : BaseBulletController
    {
        [SerializeField]
        private float speed_ = 1.0f;
        // 寿命
        [SerializeField]
        private float life_time_ = 3.0f;

        [SerializeField]
        private ParticleSystem shot_effect_;
        [SerializeField]
        private ParticleSystem hit_effect_;

        private Vector2 dir_;

        private Timer timer_;

        private void Update()
        {
            Move();
        }

        private void Awake()
        {
            timer_ = new Timer(life_time_);
        }

        public override void Init(Vector2 pos, Vector2 dir)
        {
            transform.position = (Vector3)pos;
            dir_ = dir;
            timer_.TimeReset();
            //shot_effect_.Play();
        }

        protected override void Move()
        {
            transform.position += (Vector3)dir_ * speed_ * 60f * Time.deltaTime;
            if (timer_.IsTimeout())
            {
                //hit_effect_.Play();
                gameObject.SetActive(false);
            }
        }
    }
}