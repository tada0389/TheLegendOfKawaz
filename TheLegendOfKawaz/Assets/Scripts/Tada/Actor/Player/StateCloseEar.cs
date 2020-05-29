using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// プレイヤーが咆哮で耳をふさぐステート
/// </summary>

namespace Actor.Player
{
    public partial class Player
    {
        [System.Serializable]
        private class StateCloseEar : StateMachine<Player>.StateBase
        {
            // ステート間で共有するデータのコピーインスタンス
            private Data data = null;

            [SerializeField]
            private float close_duration_ = 3.0f;

            [SerializeField]
            private float start_animation_time_ = 1.0f;

            private bool flag_;

            // ステートが始まった時に呼ばれるメソッド
            public override void OnStart()
            {
                if (data == null) data = Parent.data_;

                flag_ = false;
            }

            // ステートが終了したときに呼ばれるメソッド
            public override void OnEnd()
            {
                data.velocity = Vector2.zero;
            }

            // 毎フレーム呼ばれる関数
            public override void Proc()
            {
                if(!flag_ && Timer > start_animation_time_)
                {
                    flag_ = true;
                    data.animator.Play("CloseEar");
                }
                if(Timer > close_duration_)
                {
                    ChangeState((int)eState.Wait);
                }
            }
        }
    }
}