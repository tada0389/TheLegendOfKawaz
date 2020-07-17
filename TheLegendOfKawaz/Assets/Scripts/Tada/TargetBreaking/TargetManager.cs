using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TargetBreaking;
using Actor.Player;
using System.Threading;
using TadaLib;

namespace Target
{
    public class TargetManager : MonoBehaviour
    {
        // 実績のキー
        //[SerializeField]
        //private string achievement_key_ = "Target_Clear_syokyu";

        private int target_num_ = 0;

        private float timer_;

        [SerializeField]
        private TextMeshProUGUI timer_text_;
        //[SerializeField]
        //private TextMeshProUGUI clear_text_;

        private bool started_ = false;
        private bool finished_ = false;

        [SerializeField]
        private Transform player_;
        [SerializeField]
        private float bottom_boader_ = -15f;

        [SerializeField]
        private BaseParticle target_break_eff_;

        [SerializeField]
        private Animator ui_animator_;
        [SerializeField]
        private Animator grade_ui_animator_;

        [SerializeField]
        private Transform coin_spwan_pos_;

        [SerializeField]
        private float ready_duration_ = 1.5f;

        [SerializeField]
        private Canvas canvas_;
        [SerializeField]
        private UnityEngine.UI.Image target_icon_;
        [SerializeField]
        private Vector3 icon_initial_pos_ = new Vector3(-800f, 400f, 0f);
        [SerializeField]
        private float icon_distance_ = 50f;
        [SerializeField]
        private int icon_space_width_ = 5;
        [SerializeField]
        private Vector3 icon_default_scale_ = new Vector3(0.2f, 0.2f, 1.0f);

        private List<UnityEngine.UI.Image> target_icons_ = new List<UnityEngine.UI.Image>();

        // ゴーストの行動を保存するインスタンス
        [SerializeField]
        private GhostRecorder ghost_saver_;

        // ゴーストを呼び出すインスタンス
        [SerializeField]
        private GhostEmbodyer ghost_embodyer_;

        private bool ghost_invoked_ = false;

        private void Start()
        {
            timer_ = 0.0f;
            started_ = false;

            KoitanLib.ObjectPoolManager.Init(target_break_eff_, this, 5);

            Global.GlobalPlayerInfo.ActionEnabled = false;

            if(TargetSelectManager.GhostEnabled)
            {
                ghost_embodyer_.LoadGhost(TargetSelectManager.LoadGameGhost);
                ghost_invoked_ = true;
            }
        }

        private void Update()
        {
            if (!started_)
            {
                ready_duration_ -= Time.deltaTime;
                if(ready_duration_ <= 0f)
                {
                    started_ = true;
                    if(ghost_saver_) ghost_saver_.RecordStart();
                    if (ghost_invoked_) ghost_embodyer_.EmbodyStart();
                    Global.GlobalPlayerInfo.ActionEnabled = true;
                }
            }

            if (!started_ || finished_) return;
            timer_ += Time.deltaTime;
            timer_text_.text = timer_.ToString("f2");

            if (player_.transform.position.y < bottom_boader_) Finish(false);
        }

        public void RegisterTarget()
        {
            ++target_num_;
            // アイコンを生成する
            UnityEngine.UI.Image image = Instantiate(target_icon_, canvas_.transform);
            image.rectTransform.localPosition = new Vector3(icon_initial_pos_.x + (target_icons_.Count % icon_space_width_) * icon_distance_,
                icon_initial_pos_.y - target_icons_.Count / icon_space_width_ * icon_distance_, 0f);
            image.rectTransform.localScale = icon_default_scale_;
            target_icons_.Add(image);
        }

        public void BreakTarget(Vector3 pos)
        {
            --target_num_;
            TadaLib.EffectPlayer.Play(target_break_eff_, pos, Vector3.zero);
            Destroy(target_icons_[target_num_].gameObject);
            if (target_num_ == 0) Finish(true);
        }

        private void Finish(bool clear)
        {
            finished_ = true;
            if(ghost_saver_) ghost_saver_.RecordFinish(clear, (int)((timer_ + 0.005f) * 100.0f));
            if (ghost_invoked_) ghost_embodyer_.EmbodyFinish();
            StartCoroutine(FinishFlow(clear));
        }

        private IEnumerator FinishFlow(bool clear)
        {
            // シーンフェード中とかだと実行しない
            if (!KoitanLib.FadeManager.is_fading)
            {

                if (clear) ui_animator_.Play("clear");
                else ui_animator_.Play("failed");

                float new_time_scale = 0.1f;

                bool get_point = false;
                if (TargetSelectManager.CurStageData != null)
                {
                    if (clear)
                    {
                        get_point = true;
                        ScoreManager.Instance.RegisterScore((int)(-(timer_ + 0.005f) * 100.0f), SceneManager.GetActiveScene().name);
                        new_time_scale = 0.06f;
                        var data = TargetSelectManager.CurStageData;
                        int reward = data.OtherReward;
                        string grade_name = "Other";
                        if (timer_ <= data.GoldBoaderTime)
                        {
                            reward = data.GoldReward;
                            grade_name = "Gold";
                        }
                        else if (timer_ <= data.SilverBoaderTime)
                        {
                            reward = data.SilverReward;
                            grade_name = "Silver";
                        }
                        else if (timer_ <= data.BronzeBoaderTime)
                        {
                            reward = data.BronzeReward;
                            grade_name = "Bronze";
                        }
                        Vector3 spawn_pos = (coin_spwan_pos_ != null) ? coin_spwan_pos_.position : Camera.main.transform.position;
                        if (grade_ui_animator_ != null) grade_ui_animator_.Play(grade_name);
                        if (grade_name != "Other") SkillManager.Instance.GainSkillPoint(reward, spawn_pos, 0.02f);
                        else SkillManager.Instance.SpendSkillPoint(-reward, 0.03f);
                        // 実績解除
                        AchievementManager.FireAchievement(data.AchievementKey);
                        if (grade_name == "Gold") AchievementManager.FireAchievement(data.AchievementKey + "_Gold");
                    }
                    else SkillManager.Instance.SpendSkillPoint(-TargetSelectManager.CurStageData.OtherReward, 0.05f);
                }

                float time_change_duration = 1.5f;
                // もしポイントを獲得していたならもうちょい伸ばす
                if (get_point) time_change_duration += 1.5f;
                time_change_duration *= new_time_scale;
                TimeScaler.Instance.RequestChange(new_time_scale, time_change_duration);

                //clear_text_.rectTransform.DOPunchScale(Vector3.one, 3.0f * Time.timeScale);

                yield return new WaitForSeconds(time_change_duration);

                // お試し
                TadaLib.Save.SaveManager.Instance.Save();

                // もどったゆ「
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                KoitanLib.FadeManager.FadeIn(0.5f, "TargetMediator");
            }
        }
    }
}