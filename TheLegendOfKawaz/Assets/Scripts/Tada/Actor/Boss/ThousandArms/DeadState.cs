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
        private class DeadState : StateMachine<ThousandBossController>.StateBase
        {
            // 死亡時の爆発のエフェクト
            [SerializeField]
            private ParticleSystem explosion_effect_;

            [SerializeField]
            private float new_time_scale_ = 0.45f;
            [SerializeField]
            private float time_change_duration_ = 1.5f;
            [SerializeField]
            private float talk_wait_time_ = 2.0f;

            // 死亡時のセリフ
            [SerializeField]
            Sprite im;
            [SerializeField, Multiline(3)]
            private string[] message;
            private int index = 0;
            private bool talk_end_;

            private bool open_window_ = false;

            private float talk_end_timer_ = 0.0f;
            [SerializeField]
            private float scene_transition_time_ = 4.0f;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.trb_.Velocity = Vector2.zero;
                // 時間をゆっくりにする
                TadaLib.TimeScaler.Instance.RequestChange(new_time_scale_, time_change_duration_);
                explosion_effect_.gameObject.SetActive(true);
                Global.GlobalPlayerInfo.IsMuteki = true;
                Parent.animator_.Play("Dead2");

                // 手を全部吹き飛ばす
                foreach (var arm in Parent.arms_)
                {
                    arm.Burst();
                }

                // ボスが死んだ回数を加算する
                Global.GlobalDataManager.AddBossDefeatCnt();
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                // セリフを出してすぐにスキップされるのを防ぐ (ジャンプとかダッシュキーを押すと進んでしまうため)
                if (Timer > talk_wait_time_ * 2f / 3f)
                {
                    if (!open_window_)
                    {
                        MessageManager.OpenMessageWindow(message[0], im);
                        open_window_ = true;
                    }
                }

                // 死亡アニメーションが終わるまでまつ
                if (Timer < talk_wait_time_) return;

                if (!talk_end_)
                {
                    if (!open_window_)
                    {
                        MessageManager.OpenMessageWindow(message[0], im);
                        open_window_ = true;
                    }

                    if (ActionInput.GetButtonDown(ActionCode.Decide))
                    {
                        index++;
                        if (index < message.Length)
                        {
                            MessageManager.InitMessage(message[index]);
                        }
                        else
                        {
                            EndSeq();
                        }
                    }

                    if (ActionInput.GetButtonDown(ActionCode.Dash))
                    {
                        EndSeq();
                    }

                    return;
                }

                talk_end_timer_ += Time.deltaTime;

                if (talk_end_timer_ > scene_transition_time_)
                {
                    KoitanLib.FadeManager.ChangeFadeColor(Color.white);
                    KoitanLib.FadeManager.FadeIn(2f, "Epilogue", 1);
                    talk_end_timer_ = -10000f;
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }

            private void EndSeq()
            {
                talk_end_ = true;
                MessageManager.CloseMessageWindow();

                Actor.Player.SkillManager.Instance.GainSkillPoint(1500, Parent.transform.position, 0.7f);
                //実績解除
                AchievementManager.FireAchievement("Senju");
                if (Parent.player_.GetComponent<Actor.Player.Player>().IsNoDamage())
                    AchievementManager.FireAchievement("Senju_nodamage");
            }
        }
    }
} // namespace Actor.Enemy.Thousand