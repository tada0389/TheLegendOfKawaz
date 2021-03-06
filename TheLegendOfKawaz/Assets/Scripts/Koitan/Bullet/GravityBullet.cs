﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bullet;
using TadaLib;

/// <summary>
/// プレイヤーが放つ通常弾
/// </summary>

namespace Bullet
{
    public class GravityBullet : BaseBulletController
    {
        [SerializeField]
        private float init_speed_ = 0.3f;
        // 寿命
        [SerializeField]
        private float life_time_ = 3.0f;

        //重力
        [SerializeField]
        private float gravity = -1.0f;

        //速度
        private Vector3 velocity = Vector3.zero;

        [SerializeField]
        private BaseParticle shot_effect_;
        [SerializeField]
        private BaseParticle hit_effect_;

        private Vector2 dir_;

        private Timer timer_;

        private string opponent_tag_;
        [SerializeField]
        private int damage_ = 1;
        private float speed_;

        private Transform owner_;

        private void Update()
        {
            Move();
        }

        public override void Init(Vector2 pos, Vector2 dir, string opponent_tag = "Player", Transform owner = null, float init_speed = 1.0f, float life_time = -1.0f, float damage_rate = 1f)
        {
            transform.position = (Vector3)pos;
            move_body_.transform.position = (Vector3)pos;
            dir_ = dir;            
            opponent_tag_ = opponent_tag;
            speed_ = init_speed_ * init_speed;
            velocity = (Vector3)dir_ * speed_;
            if (life_time > 0f) life_time_ = life_time;
            timer_ = new Timer(life_time_);
            timer_.TimeReset();
            owner_ = owner;
            // 発射エフェクト
            EffectPlayer.Play(shot_effect_, move_body_.transform.position, new Vector3(0f, dir_.x, 0f), owner);
        }

        protected override void Move()
        {
            velocity.y += gravity * Time.deltaTime;
            move_body_.transform.position += velocity * Time.deltaTime;
            if (timer_.IsTimeout()) Dead();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == "Stage" || collider.tag == opponent_tag_)
            {
                if (collider.tag == opponent_tag_) collider.GetComponent<Actor.BaseActorController>().Damage(damage_);
                else Dead();
            }
        }

        private void Dead()
        {
            EffectPlayer.Play(hit_effect_, move_body_.transform.position, new Vector3(0f, dir_.x, 0f));
            gameObject.SetActive(false);
        }
    }
}