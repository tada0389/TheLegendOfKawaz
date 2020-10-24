using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// ツートンボス(兄)のスピン攻撃時のステート
/// </summary>

namespace Actor.Enemy.TwoTon.Elder
{
    partial class TwoTonElderBossController
    {
        // 画面はじに移動する
        [System.Serializable]
        private class SpinAttackState1 : StateMachine<TwoTonElderBossController>.StateBase
        {
            [SerializeField]
            private float move_vel_x_ = 1.0f;

            [SerializeField]
            private Vector3 target_pos_left_;

            [SerializeField]
            private Vector3 target_pos_right_;

            private float duration_;

            private float rotate_vel_;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 左と右，近いほうを目的地とする
                bool target_is_left = (Vector2.Distance(Parent.transform.position, target_pos_left_) <= Vector2.Distance(Parent.transform.position, target_pos_right_));
                Vector3 target_pos = (target_is_left) ? target_pos_left_ : target_pos_right_;

                Vector3 d = target_pos - Parent.transform.position;
                duration_ = Mathf.Abs(d.x) / (move_vel_x_ * 60f) + 0.001f;
                float t = duration_ * 60f;
                float jump_power = (d.y) / t - 0.5f * Accel.y * t;
                Parent.trb_.Velocity = new Vector2(move_vel_x_ * Mathf.Sign(target_pos.x - Parent.transform.position.x), jump_power);

                if (target_is_left) rotate_vel_ = 360f - Parent.transform.localEulerAngles.z;
                else rotate_vel_ = -360f - Parent.transform.localEulerAngles.z;
                rotate_vel_ /= t;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(0f, Accel.y), Vector2.one * 100f);
                Parent.transform.localEulerAngles += new Vector3(0f, 0f, rotate_vel_);

                if (Timer >= duration_)
                {
                    ChangeState((int)eState.SpinAttack2);
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity = Vector2.zero;
                Parent.transform.localEulerAngles = Vector3.zero;
            }
        }

        // スピンタックルする前動作 壁キックする
        [System.Serializable]
        private class SpinAttackState2 : StateMachine<TwoTonElderBossController>.StateBase
        {
            [SerializeField]
            private float move_vel_x_ = 1.0f;

            [SerializeField]
            private Vector3 target_pos_left_;

            [SerializeField]
            private Vector3 target_pos_right_;

            [SerializeField]
            private float delay_ = 0.5f;

            private float duration_;

            private float target_rotate_;

            private Vector3 vel_;

            private bool is_move_started_;


            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 左と右，近いほうを目的地とする
                bool target_is_left = (Vector2.Distance(Parent.transform.position, target_pos_left_) <= Vector2.Distance(Parent.transform.position, target_pos_right_));
                Vector3 target_pos = (target_is_left) ? target_pos_left_ : target_pos_right_;

                Vector3 d = target_pos - Parent.transform.position;
                duration_ = Mathf.Abs(d.x) / (move_vel_x_ * 60f) + 0.001f;
                float t = duration_ * 60f;
                float jump_power = (d.y) / t - 0.5f * Accel.y * t;
                vel_ = new Vector2(move_vel_x_ * Mathf.Sign(target_pos.x - Parent.transform.position.x), jump_power);

                Parent.trb_.Velocity = Vector3.zero;

                if (target_is_left) target_rotate_ = -90f;
                else target_rotate_ = 90f;

                is_move_started_ = false;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer < delay_) return;

                if(!is_move_started_) Parent.trb_.Velocity = vel_;
                is_move_started_ = true;

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(0f, Accel.y), Vector2.one * 100f);
                Parent.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(Parent.transform.localEulerAngles.z, target_rotate_, (Timer - delay_) / duration_));

                if (Timer >= duration_ + delay_)
                {
                    ChangeState((int)eState.SpinAttack3);
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity = Vector3.zero;
            }
        }

        // スピンタックル
        [System.Serializable]
        private class SpinAttackState3 : StateMachine<TwoTonElderBossController>.StateBase
        {
            //[SerializeField]
            //private float spin_accel_ = 1.0f;
            //[SerializeField]
            //private float max_spin_vel_ = 720f;
            //private float spin_vel_;

            [SerializeField]
            private float delay_ = 0.25f;

            [SerializeField]
            private float kick_power_ = 0.5f;

            [SerializeField]
            private float accel_ = 0.01f;

            private float target_dir_;
            private Vector3 target_dir_vec_;

            private bool is_move_started_;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                //spin_vel_ = 0.0f;
                Vector3 d = Parent.player_.position - Parent.transform.position;
                Debug.Log(d);
                target_dir_ = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg -90f;
                int cnt = 0;
                float az = Parent.transform.localEulerAngles.z;
                while(Mathf.Abs(az - target_dir_) > 180f)
                {
                    target_dir_ += Mathf.Sign(az - target_dir_) * 360f;
                    ++cnt;
                    if (cnt >= 2) break;
                }
                // ターゲット方向をクランプする
                target_dir_ = Mathf.Clamp(target_dir_, az - 60f, az + 60f);

                float rad = (target_dir_ + 90f) * Mathf.Deg2Rad;
                target_dir_vec_ = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

                is_move_started_ = false;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                //spin_vel_ = Mathf.Min(spin_vel_ + spin_accel_ * Time.deltaTime, max_spin_vel_);
                //Parent.transform.localEulerAngles += new Vector3(0f, spin_vel_ * Time.deltaTime, 0f);

                if(Timer < delay_)
                {
                    Parent.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(Parent.transform.localEulerAngles.z, target_dir_, Timer / delay_));
                }
                else
                {
                    if (!is_move_started_)
                    {
                        Parent.trb_.Velocity = target_dir_vec_ * kick_power_;
                    }
                    is_move_started_ = true;


                    ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, target_dir_vec_ * accel_, Vector2.one * 100f);
                }

                // 壁にぶつかったら次のステートへ
                if ((Parent.trb_.LeftCollide && Parent.dir_ < 0f) || (Parent.trb_.RightCollide && Parent.dir_ > 0f) || Parent.trb_.ButtomCollide || Parent.trb_.TopCollide)
                {
                    // カメラを揺らす
                    CameraSpace.CameraShaker.Shake(0.1f, 0.3f);
                    //EffectPlayer.Play(wall_hit_eff_, Parent.transform.position + new Vector3(Parent.dir_, 0f, 0f), new Vector3(Parent.dir_, 0f, 0f));
                    ChangeState((int)eState.SpinAttack4);
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }

        // 壁にぶつかった反動で後退する
        [System.Serializable]
        private class SpinAttackState4 : StateMachine<TwoTonElderBossController>.StateBase
        {

            [SerializeField]
            private Vector2 power_ = new Vector2(-0.2f, 0.2f);

            [SerializeField]
            private float rigidy_time_ = 1.0f;

            private float target_rotate_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // dirの反対側に飛ぶ
                Parent.trb_.Velocity = power_ * new Vector2(0f, 1f);

                target_rotate_ = 0f;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > rigidy_time_ && Parent.trb_.ButtomCollide)
                {
                    ChangeState((int)eState.Think);
                    return;
                }

                Parent.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(Parent.transform.localEulerAngles.z, target_rotate_, Mathf.Min(1.0f, Timer / 0.25f)));

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(Parent.dir_, 1f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                // 速度をリセット
                Parent.trb_.Velocity = Vector2.zero;
                Parent.transform.localEulerAngles = Vector3.zero;
            }
        }
    }
} // namespace Actor.Enemy.TwoTon.Elder