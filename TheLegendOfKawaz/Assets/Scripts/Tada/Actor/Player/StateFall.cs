using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーの落下状態(ジャンプでの落下は別)を管理するステート
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateFall : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private float can_jump_time = 0.15f;

            // 壁に沿っている時の落下加速度
            [SerializeField]
            private float fall_accel_with_wall_ = -0.01f;
            // 壁に沿っている時の落下最大速度
            [SerializeField]
            private float fall_max_abs_speed_with_wall_ = -0.15f;


            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                // 落下アニメーション開始
                data.animator.Play("Fall");

            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // ジャンプ
                if (ActionInput.GetButtonDown(ActionCode.Jump))
                {
                    // 壁キック
                    if(data.CanWallKick && (data.IsLeft || data.IsRight))
                    {
                        ChangeState((int)eState.Wall);
                        return;
                    }

                    // 空中ジャンプ
                    if (data.RequestArialJump())
                    {
                        ChangeState((int)eState.Jump);
                        return;
                    }
                }

                // ダッシュステート
                if (ActionInput.GetButtonDown(ActionCode.Dash) && data.CanDash())
                {
                    ChangeState((int)eState.Dush);
                    return;
                }

                // 接地したらステート変更
                if (data.IsGround)
                {
                    ChangeState((int)eState.Walk);
                    return;
                }

                // 落下開始の始めはジャンプができる (イライラ防止のため)
                if (PrevStateId == (int)eState.Walk && Timer < can_jump_time && ActionInput.GetButtonDown(ActionCode.Jump))
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // 壁に沿っている 
                if (data.CanWallKick &&
                    ((data.IsLeft && ActionInput.GetButton(ButtonCode.Left)) || (data.IsRight && ActionInput.GetButton(ButtonCode.Right))))
                {
                    ChangeState((int)eState.Wall);
                }

                // 空中ジャンプ
                if (ActionInput.GetButtonDown(ActionCode.Jump) && data.RequestArialJump())
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // 移動している方向に速度を加える
                float dir = ActionInput.GetAxis(AxisCode.Horizontal);
                if (Mathf.Abs(dir) < 0.2f) dir = 0f;
                if (dir < -0f) data.ChangeDirection(eDir.Left);
                if (dir > 0f) data.ChangeDirection(eDir.Right);

                ActorUtils.ProcSpeed(ref data.velocity, new Vector2(dir, 1f) * Accel, MaxAbsSpeed);
            }
        }
    }
}