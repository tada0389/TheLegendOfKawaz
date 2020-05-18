using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TargetBreaking;
using Actor.Player;
using System.Threading;

namespace Target
{
    public class TargetManager : MonoBehaviour
    {
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


        private void Start()
        {
            timer_ = 0.0f;
            started_ = false;

            KoitanLib.ObjectPoolManager.Init(target_break_eff_, this, 5);
        }

        private void Update()
        {
            if (!started_)
            {
                ready_duration_ -= Time.deltaTime;
                if(ready_duration_ <= 0f)
                {
                    started_ = true;
                    player_.GetComponent<Actor.Player.Player>().enabled = true;
                }
            }

            if (!started_ || finished_) return;
            timer_ += Time.deltaTime;
            timer_text_.text = timer_.ToString("f1");

            if (player_.transform.position.y < bottom_boader_) Finish(false);
        }

        public void RegisterTarget()
        {
            ++target_num_;
        }

        public void BreakTarget(Vector3 pos)
        {
            --target_num_;
            TadaLib.EffectPlayer.Play(target_break_eff_, pos, Vector3.zero);
            if (target_num_ == 0) Finish(true);
        }

        private void Finish(bool clear)
        {
            finished_ = true;
            StartCoroutine(FinishFlow(clear));
        }

        private IEnumerator FinishFlow(bool clear)
        {
            if(clear) ui_animator_.Play("clear");
            else ui_animator_.Play("failed");

            Time.timeScale = 0.1f;

            if (TargetSelectManager.CurStageData != null)
            {
                if (clear)
                {
                    Time.timeScale = 0.06f;
                    var data = TargetSelectManager.CurStageData;
                    int reward = data.OtherReward;
                    string grade_name = "";
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
                    yield return new WaitForSeconds(0.06f);
                    SkillManager.Instance.GainSkillPoint(reward, spawn_pos, 0.02f);
                }
                else SkillManager.Instance.SpendSkillPoint(-TargetSelectManager.CurStageData.OtherReward, 0.05f);
            }

            //clear_text_.rectTransform.DOPunchScale(Vector3.one, 3.0f * Time.timeScale);

            yield return new WaitForSeconds(3.0f * Time.timeScale);

            Time.timeScale = 1.0f;
            // もどったゆ「
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            KoitanLib.FadeManager.FadeIn(0.5f, "TargetMediator");
        }
    }
}