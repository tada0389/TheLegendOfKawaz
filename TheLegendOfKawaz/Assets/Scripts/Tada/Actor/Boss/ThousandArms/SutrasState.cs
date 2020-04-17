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

            [SerializeField]
            private float knock_down_duration_ = 2.0f;

            private bool is_knock_down_;
            private float knock_timer_;
            private bool is_burst_;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                is_knock_down_ = false;
                is_burst_ = false;

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
                if(!is_burst_ && !is_knock_down_ && Timer > charge_duration_)
                {
                    is_burst_ = true;
                    knock_timer_ = 0.0f;
                    Parent.player_.GetComponent<BaseActorController>().Damage(5);
                    burst_eff_.gameObject.SetActive(true);

                    kekkai_eff_.gameObject.SetActive(false);
                    charge_eff_.gameObject.SetActive(false);

                    background_prev_.DOKill();
                    background_okyo_.DOKill();
                    background_prev_.DOFade(1.0f, 1.0f);
                    background_okyo_.DOFade(0.0f, 1.0f);
                }

                if(!is_burst_ && !is_knock_down_ && prev_hp_ - Parent.HP >= knock_down_damage_)
                {
                    is_knock_down_ = true;
                    knock_timer_ = 0.0f;
                    knock_down_eff_.gameObject.SetActive(true);

                    kekkai_eff_.gameObject.SetActive(false);
                    charge_eff_.gameObject.SetActive(false);

                    background_prev_.DOKill();
                    background_okyo_.DOKill();
                    background_prev_.DOFade(1.0f, 1.0f);
                    background_okyo_.DOFade(0.0f, 1.0f);
                }

                if (is_knock_down_)
                {
                    knock_timer_ += Time.deltaTime;
                    Parent.transform.localEulerAngles = new Vector3(0f, 0f, knock_timer_ * 360f);
                    if (knock_timer_ > knock_down_duration_)
                    {
                        Parent.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        ChangeState((int)eState.Think);
                        return;
                    }
                }
                if (is_burst_)
                {
                    knock_timer_ += Time.deltaTime;
                    if (knock_timer_ > knock_down_duration_ / 2f)
                    {
                        ChangeState((int)eState.Think);
                        return;
                    }
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
            }
        }
    }
} // namespace Actor.Enemy.Thousand