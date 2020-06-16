using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーのダッシュ状態を管理するステート
/// 地上ダッシュと空中ダッシュでステートわければよかった
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateDush : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private float dush_time_ = 1.0f;

            [SerializeField]
            private float dush_speed_ = 0.4f;

            private bool is_air_dash_;

            [SerializeField]
            private BaseParticle dash_effect_;

            // ダッシュ後，わずかな時間だけ方向転換が可能
            [SerializeField]
            private float reverse_enable_time_ = 0.04f;
            private bool reversed_;

            private BaseParticle tmp_eff_;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // 待機アニメーション開始
                data.animator.Play("Dash");
                data.animator.SetBool("isDash", true);

                is_air_dash_ = !data.IsGround;

                reversed_ = false;

                // どちらを向いているか
                float dir = Parent.input_.GetAxis(AxisCode.Horizontal);
                if (Mathf.Abs(dir) < 0.2f) dir = 0f;
                if (dir < -0.1f) data.ChangeDirection(eDir.Left);
                if (dir > 0.1f) data.ChangeDirection(eDir.Right);

                data.velocity.x = (data.Dir == eDir.Left) ? -dush_speed_ : dush_speed_;
                data.velocity.y = 0f;

                tmp_eff_ = EffectPlayer.Play(dash_effect_, data.transform.position, new Vector2((data.Dir == eDir.Left) ? -1.0f : 1.0f, 0f));

                // カメラを揺らす
                CameraSpace.CameraShaker.Shake(0.05f, 0.1f);
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
                data.animator.SetBool("isDash", false);
                // 急に落ちないように少し上昇する
                if (!data.IsGround) data.velocity.y += 0.1f;
                else data.velocity.y -= 0.02f;
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 壁に当たったら終了
                if(data.IsLeft && data.Dir == eDir.Left)
                {
                    if (data.IsGround) ChangeState((int)eState.Walk);
                    else ChangeState((int)eState.Fall);
                    return;
                }
                if(data.IsRight && data.Dir == eDir.Right)
                {
                    if (data.IsGround) ChangeState((int)eState.Walk);
                    else ChangeState((int)eState.Fall);
                    return;
                }

                // 時間経過でステート以降
                if (Timer > dush_time_)
                {
                    if (data.IsGround) ChangeState((int)eState.Walk);
                    else ChangeState((int)eState.Fall);
                    return;
                }

                // ダッシュ後，わずかな時間だけ反転が可能
                if (Timer < reverse_enable_time_ && !reversed_)
                {
                    float dir = Parent.input_.GetAxis(AxisCode.Horizontal);
                    if (Mathf.Abs(dir) < 0.2f) dir = 0f;
                    if (dir < -0.1f && data.Dir == eDir.Right)
                    {
                        data.ChangeDirection(eDir.Left);
                        reversed_ = true;
                        // エフェクトも反転
                        tmp_eff_.transform.localEulerAngles = new Vector3(0f, Mathf.Sign(dir) * 90f - 90f, 0f);
                        data.velocity.x = -dush_speed_;
                    }
                    else if (dir > 0.1f && data.Dir == eDir.Left)
                    {
                        data.ChangeDirection(eDir.Right);
                        reversed_ = true;
                        tmp_eff_.transform.localEulerAngles = new Vector3(0f, Mathf.Sign(dir) * 90f - 90f, 0f);
                        data.velocity.x = dush_speed_;
                    }
                }

                // ジャンプ
                if ((data.IsGround || data.RequestArialJump()) && Parent.input_.GetButtonDown(ActionCode.Jump))
                {
                    Parent.dash_remain_time_ = 1 - Timer / dush_time_;
                    if(!data.IsGround) data.ArialJumpCalled();
                    ChangeState((int)eState.DashJump);
                    return;
                }


                // 壁に当たってるなら速度ゼロ
                if ((data.velocity.x > 0f && data.IsRight) || (data.velocity.x < 0f && data.IsLeft)) data.velocity.x = 0f;

                if (!is_air_dash_) ActorUtils.ProcSpeed(ref data.velocity, new Vector2(0f, Accel.y), MaxAbsSpeed);
                data.velocity.x = (data.Dir == eDir.Left) ? -dush_speed_ : dush_speed_;
                //ActorUtils.ProcSpeed(ref data.velocity, new Vector2(Mathf.Sign(-data.velocity.x), 1f) * Accel, MaxAbsSpeed);
            }
        }
    }
}