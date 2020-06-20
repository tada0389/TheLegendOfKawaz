using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace TargetBreaking
{
    [System.Serializable]
    public class RuleItem : BaseItem
    {
        // ルールの中身
        [SerializeField]
        private List<string> OverViewText;

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
        private int explonation_font_size_ = 16;
        [SerializeField]
        private int ranking_font_size_ = 20;

        private int index_;

        private List<float> tag_timer_;
        private float tag_position_y_;

        private float timer_;

        TargetSelectManager parent_;

        public override void Init(TargetSelectManager parent, int index)
        {
            parent_ = parent;
            ItemIndex = index;
            rectTransform = GetComponent<RectTransform>();

            tag_timer_ = new List<float>();
            for (int i = 0, n = tag_.Count; i < n; ++i) tag_timer_.Add(tag_jump_duration_);

            tag_position_y_ = tag_[0].localPosition.y;
        }

        public override void OnStart()
        {
            parent_.explonation_text_.text = "";
            parent_.header_text_.text = Name;
            foreach (var str in OverViewText)
            {
                parent_.explonation_text_.text += str + "\n";
            }

            foreach (var tag in tag_)
            {
                tag.gameObject.SetActive(true);
            }

            for (int i = 0, n = tag_.Count; i < n; ++i)
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

            index_ = 0;
            ShowExplonation(index_);

            timer_ = 0.0f;
        }

        public override void Proc()
        {
            for (int i = 0, n = tag_.Count; i < n; ++i)
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

            if (ActionInput.GetButtonDown(ActionCode.Decide) || ActionInput.GetButtonDown(ActionCode.Back))
            {
                parent_.BackState();
                return;
            }

            int prev = index_;
            if (ActionInput.GetButtonDown(ButtonCode.Right))
            {
                index_ = (index_ + 1) % tag_.Count;
            }
            else if (ActionInput.GetButtonDown(ButtonCode.Left))
            {
                index_ = (index_ - 1 + tag_.Count) % tag_.Count;
            }

            timer_ += Time.deltaTime;
            if (prev != index_) ShowExplonation(index_, prev);

        }

        private void ShowExplonation(int index, int prev = -1)
        {
            if (prev == -1) for (int i = 0, n = tag_.Count; i < n; ++i) tag_timer_[i] = tag_jump_duration_ - tag_timer_[i];
            else if (timer_ > tag_jump_duration_ / 2f) // バグ防止 
            {
                tag_timer_[prev] = tag_jump_duration_ - tag_timer_[prev];
                tag_timer_[index] = tag_jump_duration_ - tag_timer_[index];
            }

            string text = OverViewText[index];
            parent_.explonation_text_.text = text;
        }

        public override void OnEnd()
        {
            foreach (var tag in tag_)
            {
                tag.gameObject.SetActive(false);
            }
        }
    }
}