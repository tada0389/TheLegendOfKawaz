﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// 全てのボスの基底クラス
/// ボスに共通する関数をまとめている
/// </summary>

namespace Actor.Enemy
{
    [RequireComponent(typeof(TadaRigidbody))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BaseBossController : BaseActorController
    { 
        // 向いている方向
        protected enum eDir
        {
            Left,
            Right,
        }

        // 現在見ている方向
        protected float dir_ = 1f;

        // ボス本体に当たった時のダメージ
        [SerializeField]
        private int body_damage_ = 3;

        // プレイヤーの座標
        [SerializeField]
        protected Transform player_;

        [SerializeField]
        private float muteki_time_ = 1.0f;
        private Timer muteki_timer_;
        private bool timer_inited_ = false;
        [SerializeField]
        protected GameObject mesh_;

        [SerializeField]
        protected Transform not_reverse_;

        // 向いている方向を変更する
        protected void SetDirection(eDir dir)
        {
            // 浮動小数点型で==はあんまよくないけど・・・
            if (dir == eDir.Left && transform.localEulerAngles.y != 180f)
            {
                dir_ = -1f;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);
                if (not_reverse_) not_reverse_.localEulerAngles = new Vector3(not_reverse_.localEulerAngles.x, 180f, not_reverse_.localEulerAngles.z);
            }
            // else if(data_.velocity.x > 0f)
            else if (dir == eDir.Right && transform.localEulerAngles.y != 0f)
            {
                dir_ = 1f;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
                if (not_reverse_) not_reverse_.localEulerAngles = new Vector3(not_reverse_.localEulerAngles.x, 0f, not_reverse_.localEulerAngles.z);
            }
        }

        // ダメージを受ける
        public override void Damage(int damage)
        {
            if (!timer_inited_) // タイマーが起動してないとき
            {
                muteki_timer_ = new Timer(muteki_time_);
                timer_inited_ = true;
            }
            else if (!muteki_timer_.IsTimeout()) return;

            DamageDisplayer.eDamageType type = DamageDisplayer.eDamageType.Mini;
            if (damage >= 3) type = DamageDisplayer.eDamageType.Big;
            else if (damage >= 2) type = DamageDisplayer.eDamageType.Normal;
            DamageDisplayer.Instance.ShowDamage(damage, transform.position, type);

            HP = Mathf.Max(0, HP - damage);
            muteki_timer_.TimeReset();
            StartCoroutine(Tenmetu());
            if (HP == 0) Dead();
        }

        protected virtual void Dead()
        {
            // 派生クラスで読んでね
            Debug.Log("Defeated");
        }

        // このボスにぶつかるとダメージを受ける
        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.tag == "Player")
            {
                collider.GetComponent<BaseActorController>().Damage(body_damage_);
            }
        }

        //点滅
        private IEnumerator Tenmetu()
        {
            if (muteki_timer_.GetTime() < muteki_time_ / 2f)
            {
                mesh_.SetActive(false);
                yield return new WaitForEndOfFrame();
                mesh_.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
            else
            {
                mesh_.SetActive(false);
                yield return new WaitForSeconds(0.05f);
                mesh_.SetActive(true);
                yield return new WaitForSeconds(0.05f);
            }
            if (!muteki_timer_.IsTimeout())
            {
                StartCoroutine(Tenmetu());
            }
        }

        // 死んでいるか
        public virtual bool IsDead()
        {
            // 派生クラスで読んでね
            UnityEngine.Assertions.Assert.IsTrue(false, "これは派生クラスから読んでください");
            return true;
        }
    }
} // namespace Actor.Enemy