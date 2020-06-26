using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actor.Player
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SkillAttacher : MonoBehaviour
    {
        [SerializeField]
        private eSkill skill_;

        [SerializeField]
        private Player player_;

        private BoxCollider2D hit_box_;

        private bool in_player_;

        private float left_, right_, bottom_, top_;

        private string message_;

        [SerializeField]
        private TMPro.TextMeshPro otameshi_text_;
        [SerializeField]
        private TMPro.TextMeshPro buy_text_;

        [SerializeField]
        private SkillItem.SkillUIManager ui_manager_;

        [SerializeField]
        private SkillItem.SkillPurchaseGage purchase_manager_;

        private bool uped_ = false;

        private bool is_level_max_ = false;
        private bool is_has_point_ = false;

        private int prev_level_;

        private void Start()
        {
            hit_box_ = GetComponent<BoxCollider2D>();

            in_player_ = false;
            Vector2 scale = transform.localScale;
            left_ = transform.position.x - hit_box_.size.x * scale.x / 2f;
            right_ = transform.position.x + hit_box_.size.x * scale.x / 2f;
            bottom_ = transform.position.y - hit_box_.size.y * scale.y / 2f;
            top_ = transform.position.y + hit_box_.size.y * scale.y / 2f;

            message_ = "<color=red>";
            var skills = SkillManager.Instance.Skills;
            message_ += skills[(int)skill_].Name + '\n';
            message_ += "レベル " + (skills[(int)skill_].Level + 1).ToString() + '\n';
            message_ += "必要ポイント " + skills[(int)skill_].NeedPoint().ToString() + "SP\n";
            message_ += skills[(int)skill_].Explonation + '\n';
            message_ += "</color>";
        }

        private void Update()
        {
            is_level_max_ = SkillManager.Instance.Skills[(int)skill_].ReachLevelLimit;
            is_has_point_ = SkillManager.Instance.Skills[(int)skill_].NeedPoint() <= SkillManager.Instance.SkillPoint;

            // 範囲内か   
            if (IsInner() && !in_player_)
            {
                in_player_ = true;
                OnEnter();
            }
            else if(!IsInner() && in_player_)
            {
                in_player_ = false;
                OnExit();
            }

            if (is_level_max_ || !is_has_point_) return; 

            if(!uped_ && IsInner() && ActionInput.GetButton(ButtonCode.Up))
            {
                uped_ = true;
                purchase_manager_.RequestPurchase(skill_);
            }
            if (uped_ && !ActionInput.GetButton(ButtonCode.Up))
            {
                uped_ = false;
                purchase_manager_.DismissPurchase();
            }
        }

        private void OnEnter()
        {
            ui_manager_.ChangeExplonation(skill_, transform.position);

            if (is_level_max_) return;
            prev_level_ = SkillManager.Instance.Skills[(int)skill_].Level;
            player_.AquireTemporarySkill(skill_);
            //MessageManager.OpenKanbanWindow(message_);
            otameshi_text_.gameObject.SetActive(true);
            //if (is_has_point_)
                buy_text_.gameObject.SetActive(true);
        }

        private void OnExit()
        {
            otameshi_text_.gameObject.SetActive(false);
            //if (!is_has_point_)
                buy_text_.gameObject.SetActive(false);
            ui_manager_.DeleteExplonation();
            purchase_manager_.DismissPurchase();

            // スキルを試していない
            if (is_level_max_) return;
            // レベルが上がったなら戻さない
            if (prev_level_ != SkillManager.Instance.Skills[(int)skill_].Level) return;

            player_.ReleaseTemporarySkill(skill_);
            //MessageManager.CloseKanbanWindow();
        }

        private bool IsInner()
        {
            return player_.transform.position.x >= left_ && player_.transform.position.x <= right_ &&
                player_.transform.position.y >= bottom_ && player_.transform.position.y <= top_;
        }
    }
}