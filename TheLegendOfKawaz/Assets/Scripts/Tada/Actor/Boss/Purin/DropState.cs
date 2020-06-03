using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Purin;
using TadaLib;

/// <summary>
/// プリンボスの落下攻撃のステート
///
/// プレイヤーの真上に移動
/// ↓
/// 落下
/// ↓
/// 空中に戻る
/// 
/// </summary>

namespace Actor.Enemy.Purin
{
    partial class PurinBossController
    {
        // 落下攻撃のためにプレイヤーの頭上に向かうステート
        [System.Serializable]
        private class DropState1 : StateMachine<PurinBossController>.StateBase
        {
            [SerializeField]
            private Vector2 speed_ = new Vector2(0.16f, 0.16f);

            [SerializeField]
            private float period_ = Mathf.PI;

            private float dir_;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // 敵の方向へ行く
                dir_ = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                Parent.SetDirection((dir_ < 0f) ? eDir.Left : eDir.Right);

                Parent.trb_.Velocity = new Vector2(0f, 0f);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                // プレイヤーが下にいるもしくは，壁に衝突したなら終了
                float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);

                if (dir * dir_ < 0f || (dir_ < 0f && Parent.trb_.LeftCollide) || (dir_ > 0f && Parent.trb_.RightCollide) || Parent.trb_.TopCollide)
                {
                    ChangeState((int)eState.Drop2);
                    return;
                }

                Parent.trb_.Velocity.y = Mathf.Sin(Mathf.PI * (Timer / period_)) * speed_.y;

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(dir_, 0f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity = Vector2.zero;
            }
        }

        // 落下攻撃のステート
        [System.Serializable]
        private class DropState2 : StateMachine<PurinBossController>.StateBase
        {
            [SerializeField]
            private float charge_time_ = 1.0f;

            [SerializeField]
            private float vib_power_ = 0.2f;

            private float next_;

            [SerializeField]
            private BaseParticle drop_eff_;
            [SerializeField]
            private BaseParticle ground_eff_;

            private bool effect_appeared_ = false;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                next_ = 1f;
                effect_appeared_ = false;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Parent.trb_.ButtomCollide)
                {
                    EffectPlayer.Play(ground_eff_, Parent.transform.position + new Vector3(0f, -3.7f, 0f), Vector3.zero);
                    ChangeState((int)eState.Drop3);
                    return;
                }


                if(Timer > charge_time_)
                {
                    if (!effect_appeared_)
                    {
                        effect_appeared_ = true;
                        EffectPlayer.Play(drop_eff_, Parent.transform.position + new Vector3(0f, -3.7f, 0f), Vector3.zero, Parent.transform);
                    }
                    ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, Accel, MaxAbsSpeed);
                    Parent.trb_.Velocity.x = 0f;
                }
                else
                {
                    // 震動する
                    Parent.trb_.Velocity.x = vib_power_ * next_;
                    next_ *= -1f;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity = Vector2.zero;
            }
        }

        // 落下攻撃のステート
        [System.Serializable]
        private class DropState3 : StateMachine<PurinBossController>.StateBase
        {
            [SerializeField]
            private float duration_ = 1.0f;

            [SerializeField]
            private float charge_time_ = 1.0f;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {

            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > duration_ + charge_time_)
                {
                    ChangeState((int)eState.Think);
                    return;
                }

                if (Timer > charge_time_)
                {
                    ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, Accel, MaxAbsSpeed);
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity = Vector2.zero;
            }
        }
    }
} // namespace Actor.Enemy.Purin