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

        private string opponent_tag_;
        private int damage_;

        private void Update()
        {
            Move();
        }

        private void Awake()
        {
            timer_ = new Timer(life_time_);
        }

        public override void Init(Vector2 pos, Vector2 dir, int damage, string opponent_tag = "Player")
        {
            transform.position = (Vector3)pos;
            move_body_.transform.position = (Vector3)pos;
            dir_ = dir;
            damage_ = damage;
            opponent_tag_ = opponent_tag;
            timer_.TimeReset();
            CreateEffect(shot_effect_, transform.position);
        }

        protected override void Move()
        {
            move_body_.transform.position += (Vector3)dir_ * speed_ * 60f * Time.deltaTime;
            if (timer_.IsTimeout()) Dead();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == "Stage" || collider.tag == opponent_tag_)
            {
                if (collider.tag == opponent_tag_) collider.GetComponent<Actor.BaseActorController>().Damage(damage_);
                Dead();
            }
        }

        private void Dead()
        {
            CreateEffect(hit_effect_, move_body_.transform.position);
            gameObject.SetActive(false);
        }

        // エフェクトを生成する オブジェクトプール使いたいので仮
        private void CreateEffect(ParticleSystem effect, Vector3 pos)
        {
            var eff = Instantiate(effect, pos, Quaternion.identity);
            eff.transform.localEulerAngles = new Vector3(0f, Mathf.Sign(dir_.x) * 90f - 90f, 0f);
            eff.gameObject.SetActive(true);
            eff.Play();
            Destroy(eff.gameObject, 2.0f);
        }
    }
}