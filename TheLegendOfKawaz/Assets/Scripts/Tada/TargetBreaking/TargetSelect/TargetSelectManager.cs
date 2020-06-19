using Actor.Player;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace TargetBreaking
{
    [System.Serializable]
    public abstract class BaseItem : MonoBehaviour
    {
        [field:SerializeField]
        public string Name { private set; get; }

        [System.NonSerialized]
        public RectTransform rectTransform;

        // 初期化
        public abstract void Init(TargetSelectManager parent);

        // 始まった時
        public abstract void OnStart();

        // 状態を更新する
        public abstract void Proc();

        // 終わった時
        public abstract void OnEnd();
    }


    public class TargetSelectManager : MonoBehaviour
    {
        // 現在のステージを得る
        public static RewardData CurStageData = null;
        // 前回に選んだ難易度のインデックス
        private static int[] prev_game_index_ = new int[3] { 0, 0, 0 };

        public static GhostData PrevGameGhost = null;
        public static bool GhostEnabled = true;

        [SerializeField]
        private int game_index_ = 0;

        [SerializeField]
        private List<BaseItem> items_;

        [SerializeField]
        private Vector3 item_top_pos_;

        [SerializeField]
        private float item_distance_ = 80f;

        [SerializeField]
        private float item_move_cursor_dist_ = 20f;
        [SerializeField]
        private float item_move_selected_dist_ = 20f;

        [SerializeField]
        private float item_default_scale_;
        [SerializeField]
        private float item_cursor_scale_;
        [SerializeField]
        private float item_selected_scale_;

        [SerializeField]
        private float item_move_duration_ = 0.2f;
        [SerializeField]
        private Ease ease_;

        public TextMeshProUGUI explonation_text_;

        public TextMeshProUGUI header_text_;

        public RectTransform box_;
        [SerializeField]
        private Vector3 box_default_pos_;
        [SerializeField]
        private Vector3 box_destination_pos_;

        [SerializeField]
        private TextMeshProUGUI coin_text_;

        private BaseItem cur_item_;

        private int item_num_;

        private int index_;

        private void Start()
        {
            item_num_ = items_.Count;
            index_ = 0;
            // もしすでにこのゲームを遊んだことがあったら，前回の選択肢に合わせる
            if (game_index_ < prev_game_index_.Length) index_ = prev_game_index_[game_index_];

            foreach (var item in items_) item.Init(this);

            cur_item_ = null;
            ChangeIndex();

            box_.localPosition = box_default_pos_;

            coin_text_.text = SkillManager.Instance.SkillPoint.ToString();
        }

        private void Update()
        {
            if (cur_item_ != null)
            {
                cur_item_.Proc();
                return;
            }

            if (ActionInput.GetButtonDown(ActionCode.Decide))
            {
                GoNext();
                return;
            }
            if (ActionInput.GetButtonDown(ActionCode.Back)){
                GoBack();
                return;
            }

            int prev_index = index_;
            if (ActionInput.GetButtonDown(ButtonCode.Down))
            {
                index_ = (index_ + 1) % item_num_;
            }
            else if (ActionInput.GetButtonDown(ButtonCode.Up))
            {
                index_ = (index_ - 1 + item_num_) % item_num_;
            }

            if (index_ != prev_index) ChangeIndex();
        }

        private void GoNext()
        {
            SelectItem();
            cur_item_ = items_[index_];
            // やめるの場合は保存しない　よくない
            if(index_ != item_num_ - 1) prev_game_index_[game_index_] = index_;
            cur_item_.OnStart();
        }

        private void GoBack()
        {
            index_ = item_num_ - 1;
            ChangeIndex();
        }

        // 選択肢を戻す
        public void BackState()
        {
            cur_item_.OnEnd();
            cur_item_ = null;
            ChangeIndex();
            box_.DOKill();
            box_.DOLocalMove(box_default_pos_, item_move_duration_).SetEase(ease_);
        }

        private void ChangeIndex()
        {
            // 選択肢のアイコンを少し移動させる
            for(int i = 0, n = items_.Count; i < n; ++i)
            {
                BaseItem item = items_[i];

                Vector3 pos = item_top_pos_;
                pos.y -= i * item_distance_;

                item.rectTransform.DOKill();

                float target_scale = item_default_scale_;
                if (index_ == i)
                {
                    target_scale = item_cursor_scale_;
                }
                else if(index_ < i)
                {
                    pos.y -= item_move_cursor_dist_;
                }
                else if(index_ > i)
                {
                    pos.y += item_move_cursor_dist_;
                }

                item.rectTransform.DOScale(target_scale, item_move_duration_).SetEase(ease_);
                item.rectTransform.DOLocalMove(pos, item_move_duration_).SetEase(ease_);
                //item.rectTransform.DOMove(pos, item_move_duration_).SetEase(ease_);
            }
        }

        private void SelectItem()
        {
            // 選択肢のアイコンを少し移動させる
            for (int i = 0, n = items_.Count; i < n; ++i)
            {
                BaseItem item = items_[i];

                Vector3 pos = item_top_pos_;
                pos.y -= i * item_distance_;

                item.rectTransform.DOKill();

                float target_scale = item_default_scale_;
                if (index_ == i)
                {
                    target_scale = item_selected_scale_;
                }
                else if (index_ < i)
                {
                    pos.y -= item_move_selected_dist_;
                }
                else if (index_ > i)
                {
                    pos.y += item_move_selected_dist_;
                }

                item.rectTransform.DOScale(target_scale, item_move_duration_).SetEase(ease_);
                item.rectTransform.DOLocalMove(pos, item_move_duration_).SetEase(ease_);
            }
            box_.DOKill();
            box_.DOLocalMove(box_destination_pos_, item_move_duration_).SetEase(ease_);
        }
    }
}