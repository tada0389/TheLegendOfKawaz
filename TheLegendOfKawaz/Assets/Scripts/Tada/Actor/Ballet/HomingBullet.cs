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
    public class HomingBullet : BaseBulletController
    {
        [SerializeField]
        private float init_speed_ = 0.3f;
        // 寿命
        [SerializeField]
        private float life_time_ = 3.0f;

        [SerializeField]
        private ParticleSystem shot_effect_;
        [SerializeField]
        private ParticleSystem hit_effect_;

        [SerializeField, Range(0, 1)]
        private float homing_power_ = 0.5f;

        private Transform target_;

        private Vector2 dir_;

        private Timer timer_;

        private string opponent_tag_;
        private int damage_;
        private float speed_;
        private float prev_target_angle_;

        private void Update()
        {
            Move();
        }

        private void Awake()
        {
            timer_ = new Timer(life_time_);
        }

        public override void Init(Vector2 pos, Vector2 dir, int damage, string opponent_tag = "Player", Transform target = null, float init_speed = 1.0f, float life_time = -1.0f, float damage_rate = 1f)
        {
            transform.position = (Vector3)pos;
            move_body_.transform.position = (Vector3)pos;
            dir_ = dir;
            damage_ = damage;
            opponent_tag_ = opponent_tag;
            target_ = target;
            speed_ = init_speed_ * init_speed;
            prev_target_angle_ = Mathf.Atan2(dir.y, dir.x);
            if (life_time > 0f) life_time_ = life_time;
            timer_.TimeReset();
            CreateEffect(shot_effect_, transform.position);
        }

        protected override void Move()
        {
            if (timer_.IsTimeout()) Dead();

            if (target_ == null)
            {
                move_body_.transform.position += (Vector3)dir_ * speed_ * 60f * Time.deltaTime;
            }
            else
            {
                Vector3 next = target_.position;
                Vector3 now = transform.position;
                // 目的となる角度を取得する
                Vector3 d = next - now;
                float angle = Mathf.Atan2(d.y, d.x);
                // アングルを前の移動アングルとの補間にする
                angle = angle * homing_power_ + prev_target_angle_ * (1f - homing_power_);
                prev_target_angle_ = angle;

                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                move_body_.transform.position += (Vector3)dir * speed_ * 60f * Time.deltaTime;
            }
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