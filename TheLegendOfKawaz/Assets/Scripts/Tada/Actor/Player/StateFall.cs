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


            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                // 落下アニメーション開始
                Parent.PlayAnim("isFall", eAnimType.SetBoolTrue);

            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
                Parent.PlayAnim("isFall", eAnimType.SetBoolFalse);
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 水中に落ちた
                if (data.IsUnderWater)
                {
                    ChangeState((int)eState.WaterWalk);
                    return;
                }

                // 壁キック
                if (data.CanWallKick && (data.IsLeft || data.IsRight))
                {
                    if (Parent.input_.GetButtonDown(ActionCode.Jump, false))
                    {
                        ChangeState((int)eState.Wall);
                        return;
                    }
                }

                // 空中ジャンプ
                if (data.RequestArialJump())
                {
                    if (Parent.input_.GetButtonDown(ActionCode.Jump))
                    {
                        data.ArialJumpCalled();
                        ChangeState((int)eState.Jump);
                        return;
                    }
                }

                // ダッシュステート
                if (data.CanAirDash() && Parent.input_.GetButtonDown(ActionCode.Dash))
                {
                    data.AirDashCalled();
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
                if (PrevStateId == (int)eState.Walk && Timer < can_jump_time && (Parent.input_.GetButtonDown(ActionCode.Jump)))
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // 壁に沿っている 
                if (data.CanWallKick &&
                    ((data.IsLeft && Parent.input_.GetButton(ButtonCode.Left)) || (data.IsRight && Parent.input_.GetButton(ButtonCode.Right))))
                {
                    // 前回Wallだった & Fallになったばっかの時は遷移できない (壁張り付きでカクカクになるのを防止)
                    int prev_id = PrevStateId;
                    if (Timer > 0.10f || (prev_id != (int)eState.Wall && prev_id != (int)eState.Walk))
                    {
                        ChangeState((int)eState.Wall);
                        return;
                    }
                }

                // 空中ジャンプ
                if (data.RequestArialJump() && Parent.input_.GetButtonDown(ActionCode.Jump))
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // 壁に当たってるなら速度ゼロ
                if ((data.velocity.x > 0f && data.IsRight) || (data.velocity.x < 0f && data.IsLeft)) data.velocity.x = 0f;

                // 移動している方向に速度を加える
                float dir = Parent.input_.GetAxis(AxisCode.Horizontal);
                if (Mathf.Abs(dir) < 0.2f) dir = 0f;
                if (dir < -0f) data.ChangeDirection(eDir.Left);
                if (dir > 0f) data.ChangeDirection(eDir.Right);

                ActorUtils.ProcSpeed(ref data.velocity, new Vector2(dir, 1f) * Accel, MaxAbsSpeed);
            }
        }
    }
}