using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Actor.Player;
using UnityEngine.UI;
using DG.Tweening;

namespace SkillItem
{
    public enum eOpenType
    {
        Left, // 左から
        Right, // 右から
        RightDown, // 右下
        None,
    }

    // スキルを表示するボックス
    [System.Serializable]
    public class SkillUI
    {
        [SerializeField]
        private Image frame_;

        [SerializeField]
        private TextMeshProUGUI SkillName;
        [SerializeField]
        private TextMeshProUGUI SkillBody;
        [SerializeField]
        private TextMeshProUGUI SkillPoint;
        [SerializeField]
        private TextMeshProUGUI HavePoint;

        [SerializeField]
        private Vector3 open_left_pos_;
        [SerializeField]
        private Vector3 open_right_pos_;
        [SerializeField]
        private Vector3 open_rightdown_pos_;

        [SerializeField]
        private float max_scale_ = 1.0f;

        [SerializeField]
        private float open_duration_ = 0.5f;

        [SerializeField]
        private Ease open_ease_;

        private float open_start_time_;

        private CanvasGroup group_;

        public void Init()
        {
            group_ = frame_.GetComponent<CanvasGroup>();
            group_.alpha = 0.0f;
            frame_.rectTransform.localScale = Vector3.one * 0.5f;
            open_start_time_ = Time.time - open_duration_;
        }

        // 開く
        public void Open(eOpenType type, Skill skill)
        {
            frame_.rectTransform.localScale = Vector3.one * 0.5f;
            Vector3 target_pos = frame_.rectTransform.localPosition;

            if (type == eOpenType.Right) target_pos = open_right_pos_;
            else if (type == eOpenType.RightDown) target_pos = open_rightdown_pos_;
            else if(type == eOpenType.Left) target_pos = open_left_pos_;

            frame_.rectTransform.localPosition = target_pos;

            frame_.rectTransform.DOKill();
            group_.DOKill();

            frame_.rectTransform.localScale = Vector3.one * 0.5f;
            float duration = open_duration_; // Mathf.Min(open_duration_, Time.time - open_start_time_);
            frame_.rectTransform.DOScale(max_scale_, duration).SetEase(open_ease_);
            group_.DOFade(1f, duration);

            if (skill.ReachLevelLimit)
            {
                SkillName.text = skill.Name.ToString() + " LvMax";
                SkillBody.text = skill.Explonation;
                // 値段の横文字を所持コインの数と合わせる
                string price = "";
                int len = (int)(SkillManager.Instance.SkillPoint.ToString().Length * 1.5f);
                for (int i = 0; i < len; ++i) price += "-";
                SkillPoint.text = "<size=36>値段</size> <sprite index=0> " + price;
                HavePoint.text = "<size=36>所持</size> <sprite index=0> " + SkillManager.Instance.SkillPoint.ToString();
            }
            else
            {
                SkillName.text = skill.Name.ToString() + " Lv" + (skill.Level + 1).ToString();
                SkillBody.text = skill.Explonation;
                SkillPoint.text = "<size=36>値段</size> <sprite index=0> " + skill.NeedPoint().ToString();
                HavePoint.text = "<size=36>所持</size> <sprite index=0> " + SkillManager.Instance.SkillPoint.ToString();
            }

            open_start_time_ = Time.time;
        }

        // 閉じる
        public void Close()
        {
            // 移動する
            frame_.rectTransform.DOKill();
            group_.DOKill();

            float duration = Mathf.Min(open_duration_, Time.time - open_start_time_);
            frame_.rectTransform.DOScale(0.5f, duration).SetEase(open_ease_);
            group_.DOFade(0f, duration);

            open_start_time_ = Time.time;
        }
    }

    public class SkillUIManager : MonoBehaviour
    {
        [SerializeField]
        private SkillUI skill_ui_;

        private Queue<TadaLib.Pair<eSkill, eOpenType>> skill_queue_;
        private int delete_requests_;

        // Start is called before the first frame update
        void Start()
        {
            skill_ui_.Init();
            skill_queue_ = new Queue<TadaLib.Pair<eSkill, eOpenType>>();
            delete_requests_ = 0;
        }

        private void Update()
        {
            if(skill_queue_.Count != 0)
            {
                while (skill_queue_.Count != 1) skill_queue_.Dequeue();
                var item = skill_queue_.Dequeue();
                eSkill skill_item = item.first;
                eOpenType type = item.second;  

                Skill skill = SkillManager.Instance.GetSkill((int)skill_item);

                skill_ui_.Open(type, skill);
                delete_requests_ = 0;
            }
            else if(delete_requests_ >= 1)
            {
                delete_requests_ = 0;
                skill_ui_.Close();
            }
        }

        public void ChangeExplonation(eSkill skill, eOpenType type)
        {
            skill_queue_.Enqueue(new TadaLib.Pair<eSkill, eOpenType>(skill, type));
        }

        public void DeleteExplonation()
        {
            ++delete_requests_;
        }
    }
}