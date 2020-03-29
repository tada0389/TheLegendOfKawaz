using System.Collections;
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
        // Bossのステート一覧
        private enum eState
        {
            Think, // 次の行動を考えるステート
            Idle, // 待機中のステート アイドリング
            Fall, // 落下中のステート
            Damage, // ダメージを受けたときのステート
            Dead, // 死亡したときのステート

            // 以下，任意の行動 それぞれのボスに合わせて実装する
            Action1,
            Action2,
            Action3,
            Action4,
            Action5,
        }

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
        private GameObject mesh_;

        // 向いている方向を変更する
        protected void SetDirection(eDir dir)
        {
            // 浮動小数点型で==はあんまよくないけど・・・
            if (dir == eDir.Left && transform.localEulerAngles.y != 180f)
            {
                dir_ = -1f;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180f, transform.localEulerAngles.z);
            }
            // else if(data_.velocity.x > 0f)
            else if (dir == eDir.Right && transform.localEulerAngles.y != 0f)
            {
                dir_ = 1f;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);
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
            HP = Mathf.Max(0, HP - damage);
            muteki_timer_.TimeReset();
            StartCoroutine(Tenmetu());
            if (HP == 0)
            {
                Debug.Log("Defeated");
            }
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
    }
} // namespace Actor.Enemy