using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;
using DG.Tweening;

/// <summary>
/// 千手観音ボスの腕を伸ばすステート
/// </summary>

namespace Actor.Enemy.Thousand
{
    partial class ThousandBossController
    {
        // 最初の会話
        [System.Serializable]
        private class ArmStretchState : StateMachine<ThousandBossController>.StateBase
        {
            // 腕が静止するまでの時間
            [SerializeField]
            private float arm_stop_duration_ = 1.0f; 

            [SerializeField]
            private float warning_duration_ = 1.0f;

            private int cnt = 0;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                cnt = 1 - cnt;
                Parent.StartCoroutine(Flow());
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.StopCoroutine(Flow());
            }

            private IEnumerator Flow()
            {
                // 腕の動きを止める これはすべて
                for(int i = 0; i < Parent.arms_.Count; ++i)
                {
                    Parent.arms_[i].Stop(arm_stop_duration_);
                }

                yield return new WaitForSeconds(arm_stop_duration_);

                // 腕を赤くする 半分の腕だけ
                for (int i = cnt; i < Parent.arms_.Count; i += 2)
                {
                    Parent.arms_[i].ChargeStart();
                }

                yield return new WaitForSeconds(warning_duration_);

                // 腕を伸ばす
                for(int i = cnt; i < Parent.arms_.Count; i += 2)
                {
                    Parent.arms_[i].Stretch();
                }

                yield return new WaitForSeconds(2.0f);

                for(int i = 0; i < Parent.arms_.Count; ++i)
                {
                    Parent.arms_[i].Move();
                }
                ChangeState((int)eState.Think);
            }
        }
    }
} // namespace Actor.Enemy.Thousand