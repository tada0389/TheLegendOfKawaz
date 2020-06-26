﻿using System.Collections;
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
        
        }

        // 開く
        public void Open(eOpenType type, Skill skill)
        {
            frame_.rectTransform.localScale = Vector3.one * 0.5f;
            Vector3 target_pos;

            if (type == eOpenType.Right) target_pos = open_right_pos_;
            else if (type == eOpenType.RightDown) target_pos = open_rightdown_pos_;
            else target_pos = open_left_pos_;

            frame_.rectTransform.localPosition = target_pos;

            frame_.rectTransform.DOKill();
            group_.DOKill();

            frame_.rectTransform.DOScale(max_scale_, open_duration_).SetEase(open_ease_);
            group_.DOFade(1f, open_duration_);

            if (skill.ReachLevelLimit)
            {
                SkillName.text = skill.Name.ToString() + " LvMax";
                SkillBody.text = skill.Explonation;
                SkillPoint.text = "<size=36>値段</size> <sprite index=0> ∞";
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
        }
    }

    public class SkillUIManager : MonoBehaviour
    {
        [SerializeField]
        private SkillUI skill_ui_;

        private Queue<TadaLib.Pair<eSkill, Vector3>> skill_queue_;
        private int delete_requests_;

        // Start is called before the first frame update
        void Start()
        {
            skill_ui_.Init();
            skill_queue_ = new Queue<TadaLib.Pair<eSkill, Vector3>>();
            delete_requests_ = 0;
        }

        private void Update()
        {
            if(skill_queue_.Count != 0)
            {
                while (skill_queue_.Count != 1) skill_queue_.Dequeue();
                var item = skill_queue_.Dequeue();
                eSkill skill_item = item.first;
                Vector3 pos = item.second;  

                Skill skill = SkillManager.Instance.GetSkill((int)skill_item);

                eOpenType type = eOpenType.Left;
                if (pos.x < 0f && pos.y < 0f) type = eOpenType.None;
                else if (pos.x < 480f) type = eOpenType.Right;
                if (skill_item == eSkill.Attack) type = eOpenType.RightDown;
                skill_ui_.Open(type, skill);
                delete_requests_ = 0;
            }
            else if(delete_requests_ >= 1)
            {
                delete_requests_ = 0;
                skill_ui_.Close();
            }
        }

        public void ChangeExplonation(eSkill skill, Vector3 spawn_pos)
        {
            Vector3 ui_pos = Camera.main.WorldToScreenPoint(spawn_pos);
            skill_queue_.Enqueue(new TadaLib.Pair<eSkill, Vector3>(skill, ui_pos));
        }

        public void DeleteExplonation()
        {
            ++delete_requests_;
        }
    }
}