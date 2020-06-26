using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;
using UnityEngine.UI;
using Actor.Player;
using DG.Tweening;

// スキルを購入するときに出てくる円状のゲージ
// これが溜まったら購入したことになる

namespace SkillItem
{
    public class SkillPurchaseGage : MonoBehaviour
    {
        // 購入までにかかる時間
        [SerializeField]
        private float purchase_wait_time_ = 3.0f;

        private Timer timer_;

        // プレイヤーの座標
        [SerializeField]
        private Transform player_;

        // 表示する場所からプレイヤーの座標までの差異
        [SerializeField]
        private Vector2 offset_ = new Vector2(0f, 4f);

        private Camera cam_;

        [SerializeField]
        private Image body_;

        private eSkill cur_skill_;
        private bool displayed_ = false;

        // スキル購入のリクエストをキューで保存する
        private Queue<eSkill> skill_queue_;
        // スキル購入をやめた数
        private int delete_requests_;

        [SerializeField]
        private SkillUIManager ui_manager_;

        [SerializeField]
        private TMPro.TextMeshPro levelup_text_;

        // Start is called before the first frame update
        void Start()
        {
            cam_ = Camera.main;
            timer_ = new Timer(purchase_wait_time_);
            body_.fillAmount = 0f;

            skill_queue_ = new Queue<eSkill>();
            delete_requests_ = 0;
        }

        // Update is called once per frame
        void Update()
        {
            // スキル購入リクエストが来ているか確認する
            CheckPurchaseRequest();

            if (!displayed_) return;

            SetPosition();
            ProcGage();
        }

        // 購入要求をする
        public void RequestPurchase(eSkill skill)
        {
            skill_queue_.Enqueue(skill);
        }

        // 購入を解除する
        public void DismissPurchase()
        {
            ++delete_requests_;
        }

        // スキル購入リクエストが来ているか確認する
        private void CheckPurchaseRequest()
        {
            if (skill_queue_.Count != 0)
            {
                while (skill_queue_.Count != 1) skill_queue_.Dequeue();
                eSkill skill_item = skill_queue_.Dequeue();

                cur_skill_ = skill_item;
                delete_requests_ = 0;

                // スキル購入開始
                displayed_ = true;
                timer_.TimeReset();
            }
            else if (delete_requests_ >= 1)
            {
                delete_requests_ = 0;
                displayed_ = false;
                body_.fillAmount = 0f;
            }
        }

        // プレイヤーの頭の上に設置
        private void SetPosition()
        {
            Vector3 pos = player_.position + (Vector3)offset_;
            Vector3 new_pos = cam_.WorldToScreenPoint(pos);
            body_.rectTransform.position = new_pos;
        }

        // ゲージの更新
        private void ProcGage()
        {
            if (timer_.IsTimeout()) // 購入完了
            {
                body_.fillAmount = 1.0f;
                SkillManager.Instance.SpendSkillPoint(SkillManager.Instance.Skills[(int)cur_skill_].NeedPoint(), 0.5f);
                SkillManager.Instance.LevelUp((int)cur_skill_);
                delete_requests_ = 0;
                displayed_ = false;
                //body_.rectTransform.DOScale(2.0f, 0.5f).OnComplete(() =>
                //{
                //    body_.rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
                //    body_.fillAmount = 0.0f;
                //});
                body_.fillAmount = 0.0f;
                ui_manager_.ChangeExplonation(cur_skill_, eOpenType.None);

                levelup_text_.rectTransform.position = player_.transform.position + Vector3.up * 2.5f;

                levelup_text_.rectTransform.localScale = new Vector3(0f, 0f, 0f);
                levelup_text_.rectTransform.DOScale(0.7f, 0.75f).SetEase(Ease.OutBack);
                levelup_text_.DOFade(1.0f, 0.2f);
                DOTween.To(
                    () => levelup_text_.characterSpacing,
                    num => levelup_text_.characterSpacing = num,
                    20f,
                    0.75f).SetEase(Ease.OutBack).OnComplete(() => levelup_text_.DOFade(0f, 0.2f));

                return;
            }
            body_.fillAmount = timer_.GetTime() / purchase_wait_time_;

        }
    }
}