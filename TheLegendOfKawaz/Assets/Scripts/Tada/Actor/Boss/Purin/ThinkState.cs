using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Purin;
using TadaLib;

/// <summary>
/// プリンボスの次のアクションを決めるときのステート
/// </summary>

namespace Actor.Enemy.Purin
{
    partial class PurinBossController
    {
        // 次のアクションを決めるときのステート
        [System.Serializable]
        private class ThinkState : StateMachine<PurinBossController>.StateBase
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
                if(Timer > think_time_)
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
                Debug.Log(value);
                if (value < 0.28f) ChangeState((int)eState.Shot);
                else if (value < 0.56f) ChangeState((int)eState.Punch1);
                else if (value < 0.84f) ChangeState((int)eState.Drop1);
                else ChangeState((int)eState.Walk);
            }
        }
    }
} // namespace Actor.Enemy.Purin