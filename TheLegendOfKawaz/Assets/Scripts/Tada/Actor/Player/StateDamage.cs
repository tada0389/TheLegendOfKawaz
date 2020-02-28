using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーのダメージ状態を管理するステート
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateDamage : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private Vector2 power_ = new Vector2(0.5f, 1.0f);

            // 硬直時間
            [SerializeField]
            private float rigidity_time_ = 1.0f;


            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                // 待機アニメーション開始
                data.animator.Play("Damage");

                // 速度ダメージを受けた方向に飛ぶ いまは左だけ
                data.velocity.x = power_.x;
                data.velocity.y = power_.y;
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {

            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                if (Timer >= rigidity_time_)
                {
                    // 接地してたらステート変更
                    if (data.IsGround)
                    {
                        ChangeState((int)eState.Wait);
                    }
                    else
                    {
                        ChangeState((int)eState.Fall);
                    }
                    return;
                }
                ActorUtils.ProcSpeed(ref data.velocity, Accel, MaxAbsSpeed);
            }
        }
    }
}