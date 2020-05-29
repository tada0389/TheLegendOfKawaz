using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;

/// <summary>
/// 千手観音ボスの次のアクションを決めるときのステート
/// </summary>

namespace Actor.Enemy.Thousand
{
    partial class ThousandBossController
    {
        // 次のアクションを決めるときのステート
        [System.Serializable]
        private class ThinkState : StateMachine<ThousandBossController>.StateBase
        {
            [SerializeField]
            private float think_time_ = 0.5f;

            private bool first_action_ = true;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.animator_.Play("Idle");
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
                if (value < 0.28f) ChangeState((int)eState.Walk);
                else if (value < 0.56f) ChangeState((int)eState.ArmStretch);
                else if (value < 0.84f) ChangeState((int)eState.ArmThrow);
                else
                {
                    // お経を唱えた後はすぐにお経を唱えない
                    if (PrevStateId == (int)eState.Sutras || first_action_) Decide();
                    else ChangeState((int)eState.Sutras);
                }

                first_action_ = false;
            }
        }
    }
} // namespace Actor.Enemy.Purin