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

        private void Start()
        {
            hit_box_ = GetComponent<BoxCollider2D>();

            in_player_ = false;
            Vector2 scale = transform.localScale;
            left_ = transform.position.x - hit_box_.size.x * scale.x / 2f;
            right_ = transform.position.x + hit_box_.size.x * scale.x / 2f;
            bottom_ = transform.position.y - hit_box_.size.y * scale.y / 2f;
            top_ = transform.position.y + hit_box_.size.y * scale.y / 2f;
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
        }

        private void OnExit()
        {
            player_.ReleaseTemporarySkill(skill_);
        }

        private bool IsInner()
        {
            return player_.transform.position.x >= left_ && player_.transform.position.x <= right_ &&
                player_.transform.position.y >= bottom_ && player_.transform.position.y <= top_;
        }
    }
}