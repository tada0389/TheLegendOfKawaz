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
    public class PurinBullet : BaseBulletController
    {
        [SerializeField]
        private Vector2 stage_range_x = new Vector2(-10f, 10f);

        [SerializeField]
        private float gravity_ = -0.98f;

        // 弾性力
        [SerializeField]
        private float bounceness_ = 0.8f;

        [SerializeField]
        private float init_speed_ = 20f;
        [SerializeField]
        private int init_damage_ = 2;
        // 寿命
        [SerializeField]
        private float life_time_ = 3.0f;

        [SerializeField]
        private BaseParticle shot_effect_;
        [SerializeField]
        private BaseParticle hit_effect_;

        private Vector2 dir_;

        private Timer timer_;

        private string opponent_tag_;
        private int damage_;
        private Vector2 velocity_;

        private Transform owner_;

        private float hit_radius_;

        private void Start()
        {
            hit_radius_ = move_body_.GetComponent<CircleCollider2D>().radius * transform.localScale.x;
        }

        private void Update()
        {
            Move();
        }

        public override void Init(Vector2 pos, Vector2 dir, string opponent_tag = "Player", Transform owner = null, float init_speed = 1.0f, float life_time = -1.0f, float damage_rate = 1f)
        {
            transform.position = (Vector3)pos;
            move_body_.transform.position = (Vector3)pos;
            dir_ = dir;
            damage_ = (int)(init_damage_ * damage_rate);
            opponent_tag_ = opponent_tag;
            velocity_ = dir * init_speed_ * init_speed;
            if (life_time > 0f) life_time_ = life_time;
            timer_ = new Timer(life_time_);
            timer_.TimeReset();
            owner_ = owner;
            // 発射エフェクト
            EffectPlayer.Play(shot_effect_, move_body_.transform.position, new Vector3(0f, dir_.x, 0f), owner);
        }

        protected override void Move()
        {
            velocity_ += new Vector2(0f, gravity_);
            move_body_.transform.position += (Vector3)velocity_ * Time.deltaTime;
            if (timer_.IsTimeout()) Dead();
        }

        private void Bound()
        {
            if(move_body_.transform.position.x - hit_radius_ < stage_range_x.x ||
                move_body_.transform.position.x + hit_radius_ > stage_range_x.y)
            {
                // 壁反射
                velocity_.x *= -bounceness_;
            }
            else
            {
                // 床反射
                velocity_.y *= -bounceness_;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.tag == opponent_tag_)
            {
                collider.GetComponent<Actor.BaseActorController>().Damage(damage_);
            }

            // バウンドする
            if (collider.tag == "Stage")
            {
                Bound();
            }
        }

        private void Dead()
        {
            EffectPlayer.Play(hit_effect_, move_body_.transform.position, new Vector3(0f, dir_.x, 0f));
            gameObject.SetActive(false);
        }
    }
}