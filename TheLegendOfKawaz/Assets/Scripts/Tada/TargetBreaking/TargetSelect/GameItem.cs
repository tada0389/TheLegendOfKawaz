using DG.Tweening;
using Result;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TargetBreaking
{
    [System.Serializable]
    public class RewardData
    {
        [field: SerializeField]
        public float GoldBoaderTime { private set; get; }

        [field: SerializeField]
        public int GoldReward { private set; get; }

        [field: SerializeField]
        public float SilverBoaderTime { private set; get; }

        [field: SerializeField]
        public int SilverReward { private set; get; }

        [field: SerializeField]
        public float BronzeBoaderTime { private set; get; }

        [field: SerializeField]
        public int BronzeReward { private set; get; }

        [field: SerializeField]
        public int OtherReward { private set; get; }

        [field:SerializeField]
        public float DeveloperTime { private set; get; }
        [field: SerializeField]
        public int DeveloperScore { private set; get; }

        [field:SerializeField]
        public string AchievementKey { private set; get; }
    }

    [System.Serializable]
    public class GameItem : BaseItem
    {
        [SerializeField]
        private bool is_target_mode_ = true;

        [SerializeField]
        private RewardData reward_;

        [field: SerializeField]
        public string NextScene { private set; get; }


        [SerializeField]
        private string PopOutText;
        [SerializeField]
        private List<string> OverViewText;

        // 使用できるスキルたち
        [SerializeField]
        private List<Image> skill_icons_;

        [SerializeField]
        private float icon_distance_ = 50f;

        [SerializeField]
        private List<RectTransform> tag_;
        [SerializeField]
        private List<TextMeshProUGUI> tag_text_;
        [SerializeField]
        private List<int> tag_font_size_;
        [SerializeField]
        private List<string> tag_comments_;
        [SerializeField]
        private List<Image> tag_shadow_;
        [SerializeField]
        private float tag_jump_power_ = 100f;
        [SerializeField]
        private float tag_jump_duration_ = 0.5f;

        [SerializeField]
        private float tag_distance_ = 100f;

        [SerializeField]
        private ScoreDisplay score_displayer_;

        [SerializeField]
        private int explonation_font_size_ = 16;
        [SerializeField]
        private int ranking_font_size_ = 20;

        private int index_;
        private bool is_fading_;

        private List<float> tag_timer_;
        private float tag_position_y_;

        private float timer_;

        TargetSelectManager parent_;

        public override void Init(TargetSelectManager parent)
        {
            parent_ = parent;
            rectTransform = GetComponent<RectTransform>();
            tag_timer_ = new List<float>();
            for (int i = 0, n = tag_.Count; i < n; ++i) tag_timer_.Add(tag_jump_duration_);

            tag_position_y_ = tag_[0].localPosition.y;
        }

        public override void OnStart()
        {
            index_ = 0;
            is_fading_ = false;
            parent_.header_text_.text = Name;
            foreach (var tag in tag_)
            {
                tag.gameObject.SetActive(true);
            }

            for(int i = 0, n = tag_.Count; i < n; ++i)
            {
                tag_text_[i].text = tag_comments_[i];
                tag_text_[i].fontSize = tag_font_size_[i];
                tag_timer_[i] = tag_jump_duration_;
                if (i == 0) tag_[i].transform.SetAsLastSibling();
                else tag_[i].transform.SetAsFirstSibling();
                Color color = tag_shadow_[i].color;
                color.a = 0.3f;
                tag_shadow_[i].color = color;
            }
            
            ShowExplonation(index_);

            timer_ = 0.0f;
        }

        public override void Proc()
        {
            for(int i = 0, n = tag_.Count; i < n; ++i)
            {
                float time = tag_timer_[i];
                tag_timer_[i] = Mathf.Min(tag_timer_[i] + Time.deltaTime, tag_jump_duration_);
                if (time < tag_jump_duration_ / 2f && tag_timer_[i] > tag_jump_duration_ / 2f)
                {
                    // 半分を過ぎた 表裏を変更する
                    // タグの描画準を変更する
                    tag_shadow_[i].DOKill();
                    if (index_ == i)
                    {
                        tag_[i].transform.SetAsLastSibling();
                        tag_shadow_[i].DOFade(0.0f, tag_jump_duration_ / 4f);
                    }
                    else
                    {
                        tag_[i].transform.SetAsFirstSibling();
                        tag_shadow_[i].DOFade(0.3f, tag_jump_duration_ / 4f);
                    }
                }

                time = tag_timer_[i];
                //if(tag_timer_[i] < tag_jump_duration_)
                {
                    Vector3 pos = tag_[i].localPosition;
                    float theta = time / tag_jump_duration_ * Mathf.PI;
                    pos.y = tag_position_y_ - Mathf.Sin(theta) * tag_jump_power_;
                    tag_[i].localPosition = pos;
                }
            }

            if (is_fading_) return;

            if (ActionInput.GetButtonDown(ActionCode.Decide))
            {
                GoNext();
                return;
            }

            if (ActionInput.GetButtonDown(ActionCode.Back))
            {
                GoBack();
                return;
            }

            timer_ += Time.deltaTime;
            if (ActionInput.GetButtonDown(ButtonCode.Left) || ActionInput.GetButtonDown(ButtonCode.Right))
            {
                index_ = 1 - index_;
                ShowExplonation(index_, 1 - index_);
            }
        }

        public override void OnEnd()
        {
            foreach (var icon in skill_icons_)
            {
                icon.gameObject.SetActive(false);
            }

            foreach (var tag in tag_)
            {
                tag.gameObject.SetActive(false);
            }
        }

        private void ShowExplonation(int index, int prev = -1)
        {
            if (prev == -1) for (int i = 0, n = tag_.Count; i < n; ++i) tag_timer_[i] = tag_jump_duration_ - tag_timer_[i];
            else if(timer_ > tag_jump_duration_ / 2f) // バグ防止 
            {
                tag_timer_[prev] = tag_jump_duration_ - tag_timer_[prev];
                tag_timer_[index] = tag_jump_duration_ - tag_timer_[index];
            }

            if (index == 0)
            {
                string text = "<size=20>【" + PopOutText + "】</size>\n";
                text += "・概要\n";
                foreach (var str in OverViewText)
                    text += "   " + str + "\n";
                text += "・報酬\n";
                if (is_target_mode_)
                {
                    text += "   Gold : " + reward_.GoldReward.ToString() + "SP\n";
                    text += "   Silver : " + reward_.SilverReward.ToString() + "SP\n";
                    text += "   Bronze : " + reward_.BronzeReward.ToString() + "SP\n";
                }
                else
                {
                    text += "   完全歩合制です\n";
                    text += "   頑張ればその分だけ貰えます\n";
                }
                text += "・所持スキル\n";
                parent_.explonation_text_.text = text;
                parent_.explonation_text_.fontSize = explonation_font_size_;

                // スキルアイコンを表示
                float x = -150f;
                float y = -110f;
                foreach (var icon in skill_icons_)
                {
                    icon.gameObject.SetActive(true);
                    icon.rectTransform.localPosition = new Vector3(x, y, 0f);
                    x += icon_distance_;
                }
            }
            else
            {
                foreach (var icon in skill_icons_)
                {
                    icon.gameObject.SetActive(false);
                }
                string text = score_displayer_.GetScoreRanking(NextScene, ScoreManager.Instance.GetGameName(NextScene));
                if (is_target_mode_)
                {
                    text += "<color=#666666> 開発者タイム ";
                    if (AchievementManager.IsUnlocked(reward_.AchievementKey + "_Gold")) text += System.String.Format("{0, 6}", reward_.DeveloperTime.ToString("F2"));
                    else text += "??????";
                    text += " (s)</color>";
                }
                else
                {
                    text += "<color=#666666> 開発者スコア ";
                    if (true || AchievementManager.IsUnlocked(reward_.AchievementKey + "_Gold")) text += System.String.Format("{0, 8}", reward_.DeveloperScore.ToString());
                    else text += "??????";
                    text += " (pt)</color>";
                }
                parent_.explonation_text_.text = text;
                parent_.explonation_text_.fontSize = ranking_font_size_;
            }
        }

        private void GoNext()
        {
            if(index_ == 1)
            {
                index_ = 0;
                ShowExplonation(index_);
                return;
            }

            is_fading_ = true;
            // 実際に遊ぶ
            //CurStageData = stages_[index_.first];
            //if (IsTargetMode) prev_stage_index_ = index_.first;
            KoitanLib.FadeManager.FadeIn(0.5f, NextScene);
            TargetSelectManager.CurStageData = reward_;
        }

        private void GoBack()
        {
            // 戻る
            parent_.BackState();
        }
    }
}
