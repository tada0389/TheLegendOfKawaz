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
        private class SutrasState : StateMachine<ThousandBossController>.StateBase
        {
            [SerializeField]
            private int knock_down_damage_ = 1;

            [SerializeField]
            private float charge_duration_ = 5f;

            private int prev_hp_;

            [SerializeField]
            private ParticleSystem kekkai_eff_;

            [SerializeField]
            private ParticleSystem charge_eff_;

            [SerializeField]
            private SpriteRenderer background_prev_;
            [SerializeField]
            private SpriteRenderer background_okyo_;

            [SerializeField]
            private BaseParticle burst_eff_;

            [SerializeField]
            private BaseParticle knock_down_eff_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                prev_hp_ = Parent.HP;

                kekkai_eff_.gameObject.SetActive(true);
                kekkai_eff_.Play();

                charge_eff_.gameObject.SetActive(true);
                charge_eff_.Play();

                background_prev_.DOFade(0.0f, 1.0f);
                background_okyo_.DOFade(1.0f, 1.0f);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if(Timer > charge_duration_)
                {
                    Parent.player_.GetComponent<BaseActorController>().Damage(5);
                    burst_eff_.gameObject.SetActive(true);
                    ChangeState((int)eState.Think);
                    return;
                }

                if(prev_hp_ - Parent.HP >= knock_down_damage_)
                {
                    knock_down_eff_.gameObject.SetActive(true);
                    ChangeState((int)eState.Think);
                    return;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                kekkai_eff_.gameObject.SetActive(false);
                charge_eff_.gameObject.SetActive(false);

                background_prev_.DOKill();
                background_okyo_.DOKill();
                background_prev_.DOFade(1.0f, 1.0f);
                background_okyo_.DOFade(0.0f, 1.0f);
            }
        }
    }
} // namespace Actor.Enemy.Thousand