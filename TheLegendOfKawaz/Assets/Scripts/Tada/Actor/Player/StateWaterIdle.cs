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
        private class StateWaterIdle : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

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
                    ChangeState((int)eState.Fall);
                    return;
                }

                // ジャンプ入力ならジャンプステートへ
                if (Parent.input_.GetButtonDown(ActionCode.Jump))
                {
                    ChangeState((int)eState.WaterJump);
                    return;
                }

                // 左右に押したら歩くステートに変更
                if (Mathf.Abs(Parent.input_.GetAxis(AxisCode.Horizontal)) > 0.2f)
                {
                    ChangeState((int)eState.WaterWalk);
                    return;
                }

                float gravity_dir = 1f;
                if (data.velocity.y + Accel.y < -MaxAbsSpeed.y) gravity_dir = -1f;

                ActorUtils.ProcSpeed(ref data.velocity, new Vector2(1f, gravity_dir) * Accel, MaxAbsSpeed);
            }
        }
    }
}