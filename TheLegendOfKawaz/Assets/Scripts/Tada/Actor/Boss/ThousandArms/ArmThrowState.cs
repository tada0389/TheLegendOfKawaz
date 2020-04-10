using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;
using DG.Tweening;

/// <summary>
/// 千手観音ボスの死亡時のステート
/// </summary>

namespace Actor.Enemy.Thousand
{
    partial class ThousandBossController
    {
        // 最初の会話
        [System.Serializable]
        private class ArmThrowState : StateMachine<ThousandBossController>.StateBase
        {
            [SerializeField]
            private float delay_ = 5.0f;
            // 開始時に呼ばれる
            public override void OnStart()
            { 
                foreach(var arm in Parent.arms_)
                {
                    arm.Throw();
                }
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > delay_)
                {
                    ChangeState((int)eState.Think);
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
    }
} // namespace Actor.Enemy.Thousand