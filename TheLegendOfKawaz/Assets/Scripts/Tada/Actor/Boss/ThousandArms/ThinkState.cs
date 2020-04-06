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
                ChangeState((int)eState.Walk);
            }
        }
    }
} // namespace Actor.Enemy.Purin