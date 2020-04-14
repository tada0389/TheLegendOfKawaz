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
        }

        private void OnEnter()
        {
            player_.AquireTemporarySkill(skill_);
            MessageManager.OpenKanbanWindow(message_);
            otameshi_text_.gameObject.SetActive(true);
        }

        private void OnExit()
        {
            player_.ReleaseTemporarySkill(skill_);
            MessageManager.CloseKanbanWindow();
            otameshi_text_.gameObject.SetActive(false);
        }

        private bool IsInner()
        {
            return player_.transform.position.x >= left_ && player_.transform.position.x <= right_ &&
                player_.transform.position.y >= bottom_ && player_.transform.position.y <= top_;
        }
    }
}