using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Thousand;
using TadaLib;
using DG.Tweening;

/// <summary>
/// ツートンボスの死亡時のステート
/// </summary>

namespace Actor.Enemy.TwoTon
{
    partial class TwoTonBossController
    {
        [System.Serializable]
        private class DeadState : StateMachine<TwoTonBossController>.StateBase
        {
            [SerializeField]
            private List<TalkContest> talk_contests;

            [SerializeField]
            private float new_time_scale_ = 0.3f;
            [SerializeField]
            private float time_change_duration_ = 1.0f;

            private float talk_end_timer_ = 0.0f;
            private bool talk_end_ = false;
            [SerializeField]
            private float scene_transition_time_ = 4.0f;

            // 開始時に呼ばれる
            public override void OnStart()
            {
                // ボスを死亡させる
                Parent.younger_boss_.Damage(1000);
                Parent.elder_boss_.Damage(1000);

                // 時間をゆっくりにする
                TadaLib.TimeScaler.Instance.RequestChange(new_time_scale_, time_change_duration_);
                Global.GlobalPlayerInfo.IsMuteki = true;
                Global.GlobalPlayerInfo.BossDefeated = true;

                // ボスが死んだ回数を加算する
                //Global.GlobalDataManager.AddBossDefeatCnt(Global.eBossType.TwoTon);

                Parent.StartCoroutine(Parent.Talk(talk_contests, TalkEnd));
            }

            public override void Proc()
            {
                if (!talk_end_) return;
                talk_end_timer_ += Time.fixedDeltaTime;
                if(talk_end_timer_ > scene_transition_time_)
                {
                    talk_end_timer_ = -100000f;
                    KoitanLib.FadeManager.FadeIn(0.5f, "ZakkyScene");
                }
            }


            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }


            private void TalkEnd()
            {
                Actor.Player.SkillManager.Instance.GainSkillPoint(2000, Parent.transform.position, 0.7f);
                talk_end_ = true;
                ////実績解除
                //AchievementManager.FireAchievement("Senju");
                //if (Parent.player_.GetComponent<Actor.Player.Player>().IsNoDamage())
                //    AchievementManager.FireAchievement("Senju_nodamage");
            }
        }
    }
} // namespace Actor.Enemy.TwoTon