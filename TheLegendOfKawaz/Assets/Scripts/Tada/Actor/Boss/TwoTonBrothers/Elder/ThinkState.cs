using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// ツートンボス(兄)の次のアクションを決めるときのステート
/// </summary>

namespace Actor.Enemy.TwoTon.Elder
{
    partial class TwoTonElderBossController
    {
        // 次のアクションを決めるときのステート
        [System.Serializable]
        private class ThinkState : StateMachine<TwoTonElderBossController>.StateBase
        {
            [SerializeField]
            private float think_time_ = 0.5f;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                //Parent.animator_.Play("Idle");
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > think_time_)
                {
                    Decide();
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }

            private void Decide()
            {
                float value = Random.value;
                //Debug.Log(value);
                ChangeState((int)eState.SpinAttack1);
                //else if (value < 1.01f) ChangeState((int)eState.Walk);
            }
        }
    }
} // namespace Actor.Enemy.TwoTon.Elder