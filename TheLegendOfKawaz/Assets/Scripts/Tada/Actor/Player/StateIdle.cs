using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーのアイドリング状態を管理するステート
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateIdle : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if(data == null) data = Parent.data_;

                // 待機アニメーション開始
                data.animator.Play("Idle");

                // 速度をゼロにする
                //data.velocity = Vector2.zero;
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                // ジャンプ入力ならジャンプステートへ
                if (ActionInput.GetButtonDown(ActionCode.Jump))
                {
                    ChangeState((int)eState.Jump);
                    return;
                }

                // ダッシュステート
                if (ActionInput.GetButtonDown(ActionCode.Dash) && data.CanGroundDash())
                {
                    ChangeState((int)eState.Dush);
                    return;
                }

                // 地面から離れたらふぉるステートへ
                if (!data.IsGround)
                {
                    ChangeState((int)eState.Fall);
                }

                // 左右に押したら歩くステートに変更
                if (Mathf.Abs(ActionInput.GetAxis(AxisCode.Horizontal)) > 0.2f)
                {
                    ChangeState((int)eState.Walk);
                    return;
                }

                ActorUtils.ProcSpeed(ref data.velocity, Vector2.zero, MaxAbsSpeed);
                ActorUtils.ProcSpeed(ref data.velocity, Vector2.zero, MaxAbsSpeed);
                ActorUtils.ProcSpeed(ref data.velocity, Vector2.zero, MaxAbsSpeed);
            }
        }
    }
}