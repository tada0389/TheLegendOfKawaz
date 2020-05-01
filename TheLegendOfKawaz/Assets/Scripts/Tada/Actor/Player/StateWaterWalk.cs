using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーの歩き状態を管理するステート
/// 接地している時のみ
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateWaterWalk : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            // 前回がジャンプステートでこの時間以内に水中外にいたらジャンプする
            [SerializeField]
            private float add_jump_time_ = 0.05f;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // 歩きアニメーション開始
                data.animator.SetBool("isWalk", true);
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
                data.animator.SetBool("isWalk", false);
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // 水中から離れたならほかのステートへ
                if (!data.IsUnderWater)
                {
                    if (PrevStateId == (int)eState.WaterJump && Timer < add_jump_time_) ChangeState((int)eState.Jump);
                    else ChangeState((int)eState.Fall);
                    return;
                }

                // ジャンプ入力ならジャンプステートへ
                if (ActionInput.GetButtonDown(ActionCode.Jump))
                {
                    ChangeState((int)eState.WaterJump);
                    return;
                }

                // 移動している方向に速度を加える
                float dir = ActionInput.GetAxis(AxisCode.Horizontal);
                if (Mathf.Abs(dir) < 0.2f) dir = 0f;
                if (dir == 0.0f)
                { // 良くない
                    // 何も押していないならWait状態に
                    ChangeState((int)eState.WaterIdle);
                    return;
                }
                if (dir < -0f) data.ChangeDirection(eDir.Left);
                if (dir > 0f) data.ChangeDirection(eDir.Right);

                float gravity_dir = 1f;
                if (data.velocity.y + Accel.y < -MaxAbsSpeed.y) gravity_dir = -1f;

                ActorUtils.ProcSpeed(ref data.velocity, new Vector2(dir, gravity_dir) * Accel, MaxAbsSpeed);
            }
        }
    }
}