using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillItem;
using Actor.Player;
using TMPro;
using TadaLib;
using DG.Tweening;

/// <summary>
/// スキル購入シーンを管理するクラス
/// </summary>

namespace SkillItem
{
    public class SkillItemManager : MonoBehaviour
    {
        // ステートパターンにする
        StateMachine<SkillItemManager> state_machine_;

        private enum eState
        {
            Select,
            Decide,
        }

        // キャッシュ
        private SkillManager skill_;

        [SerializeField]
        private Canvas canvas_;

        [SerializeField]
        private TextMeshProUGUI skill_point_text_;

        [SerializeField]
        private TextMeshProUGUI explonation_text_;
        [SerializeField]
        private TextMeshProUGUI price_text_;

        // スキルアイテムのプレハブ
        [SerializeField]
        private SkillItemController skill_prefab_;
        // スキルアイコンの画像たち
        [SerializeField]
        private Sprite[] skill_icons_; 

        // スキルアイテム
        private List<SkillItemController> skills_;

        // アイテムの初期座標
        [SerializeField]
        private Vector3 skill_initial_pos_;
        // アイテムの間隔幅
        [SerializeField]
        private float width_intervel_ = 200f;
        // アイテムの間隔高さ
        [SerializeField]
        private float height_interval_ = 200f;

        // 現在選択しているインデックス
        private int select_index_;

        [SerializeField]
        private SelectState select_state_;
        [SerializeField]
        private DecideState decide_state_;

        // オーディオソース
        private AudioSource audio_;
        // 決定音
        [SerializeField]
        private AudioClip decide_se_;
        // キャンセル音
        [SerializeField]
        private AudioClip cancel_se_;
        // 購入音
        [SerializeField]
        private AudioClip buy_se_;

        // Start is called before the first frame update
        void Start()
        {
            audio_ = GetComponent<AudioSource>();
            skill_ = SkillManager.Instance;

            skills_ = new List<SkillItemController>();

            select_index_ = 0;
            skill_point_text_.text = SkillManager.Instance.SkillPoint.ToString() + "SP";

            CreateSkillItem();

            // ステートマシンのメモリ確保 自分自身を渡す
            state_machine_ = new StateMachine<SkillItemManager>(this);

            state_machine_.AddState((int)eState.Select, select_state_);
            state_machine_.AddState((int)eState.Decide, decide_state_);

            state_machine_.SetInitialState((int)eState.Select);
        }

        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.B].wasPressedThisFrame)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("TestSceneCopy");
            }
            state_machine_.Proc();
        }

        // スキルアイテムを並べる
        private void CreateSkillItem()
        {
            int skill_num = skill_.Skills.Count;
            int half_num = (skill_num + 1) / 2;
            // 並べる
            for(int i = 0; i < half_num; ++i)
            {
                SkillItemController item = Instantiate(skill_prefab_, canvas_.transform);
                item.transform.localPosition = skill_initial_pos_ + new Vector3(i * width_intervel_, 0f, 0f);
                item.transform.parent = transform;
                item.Init(i, skill_icons_[i], canvas_);
                skills_.Add(item);
            }
            for (int i = half_num; i < skill_num; ++i)
            {
                SkillItemController item = Instantiate(skill_prefab_, canvas_.transform);
                item.transform.localPosition = skill_initial_pos_ + new Vector3((i - half_num) * width_intervel_, -height_interval_, 0f);
                item.transform.parent = transform;
                item.Init(i, skill_icons_[i], canvas_);
                skills_.Add(item);
            }
        }


        // スキル選択ステート
        [System.Serializable]
        private class SelectState : StateMachine<SkillItemManager>.StateBase
        {
            private int skill_num_;
            private int half_num_;

            public override void OnStart()
            {
                skill_num_ = Parent.skill_.Skills.Count;

                // 折り返しの添え字
                half_num_ = (skill_num_ + 1) / 2;

                ChangeSelectSkill(Parent.select_index_);
            }

            public override void Proc()
            {
                if (ActionInput.GetButtonDown(ActionCode.Shot))
                {
                    Parent.audio_.PlayOneShot(Parent.decide_se_);
                    ChangeState((int)eState.Decide);
                    return;
                }

                int before_index = Parent.select_index_;
                if (ActionInput.GetButtonDown(ButtonCode.Down) || ActionInput.GetButtonDown(ButtonCode.Up))
                {
                    if (Parent.select_index_ >= half_num_ || Parent.select_index_ + half_num_ < skill_num_)
                    {
                        if (Parent.select_index_ < half_num_) Parent.select_index_ += half_num_;
                        else Parent.select_index_ -= half_num_;
                    }
                }
                if (ActionInput.GetButtonDown(ButtonCode.Right))
                {
                    Parent.select_index_ = (Parent.select_index_ + 1) % skill_num_;
                }
                if (ActionInput.GetButtonDown(ButtonCode.Left))
                {
                    Parent.select_index_ = (Parent.select_index_ - 1 + skill_num_) % skill_num_;
                }

                ChangeSelectSkill(Parent.select_index_, before_index);
            }

            public override void OnEnd()
            {

            }

            // スキルアイテムの選択対象を変更する
            private void ChangeSelectSkill(int after_index, int before_index = -1)
            {
                if (before_index == after_index) return;
                Parent.skills_[after_index].ChangeFrameColor(Color.red);
                if (before_index != -1)
                    Parent.skills_[before_index].ChangeFrameColor(Color.black);

                Parent.explonation_text_.text = Parent.skills_[after_index].Explonation;
                if (Parent.skills_[after_index].ReachSkillLimit) Parent.price_text_.text = "<color=red>最大レベル</color>";
                else Parent.price_text_.text = Parent.skills_[after_index].NeedPoint.ToString() + "SP";
            }
        }

        // スキル購入確認ステート
        [System.Serializable]
        private class DecideState : StateMachine<SkillItemManager>.StateBase
        {
            // スキル購入確認のポップアップ
            [SerializeField]
            private GameObject pop_up_;

            // いーじんぐ
            [SerializeField]
            private Ease popup_ease_;
            [SerializeField]
            private float appear_time_ = 1.0f;

            // 説明
            [SerializeField]
            private TextMeshProUGUI explonation_;
            [SerializeField]
            private Image skill_icon_;

            // 値段
            [SerializeField]
            private TextMeshProUGUI price_;

            // 選択のはい，いいえ
            [SerializeField]
            private TextMeshProUGUI yes_text_;
            [SerializeField]
            private TextMeshProUGUI no_text_;

            private bool watch_yes_;

            // 購入のメッセージ
            [SerializeField]
            private RectTransform levelup_message_;
            [SerializeField]
            private Image levelup_icon_;
            [SerializeField]
            private Image icon_back_;
            [SerializeField]
            private Ease levelup_ease_;
            [SerializeField]
            private float levelup_message_time_ = 0.5f;

            private bool buyed = false;

            public override void OnStart()
            {
                buyed = false;
                // 購入できるか確かめる
                if(Parent.skills_[Parent.select_index_].ReachSkillLimit || SkillManager.Instance.SkillPoint < Parent.skills_[Parent.select_index_].NeedPoint)
                {
                    // SEを鳴らす
                    Parent.audio_.PlayOneShot(Parent.cancel_se_);
                    ChangeState((int)eState.Select);
                    return;
                }

                // デフォルトではいを赤く，いいえを白くする
                watch_yes_ = true;
                yes_text_.color = Color.red;
                no_text_.color = Color.white;

                // 説明を埋める
                explonation_.text = Parent.skills_[Parent.select_index_].Name;
                price_.text = Parent.skills_[Parent.select_index_].NeedPoint.ToString() + "SP";

                skill_icon_.sprite = Parent.skills_[Parent.select_index_].Image;

                // ポップアップを表示
                pop_up_.transform.DOScale(new Vector3(1f, 1f, 1f), appear_time_).SetEase(popup_ease_);
            }

            public override void Proc()
            {
                // ポップアップの表示が完了するまで行動できない
                if (Timer <= appear_time_) return;

                // 購入する
                if (ActionInput.GetButtonDown(ActionCode.Shot))
                {
                    Decide();
                    return;
                }
                if (ActionInput.GetButtonDown(ActionCode.Back))
                {
                    ChangeState((int)eState.Select);
                    return;
                }

                if(ActionInput.GetButtonDown(ButtonCode.Left) || ActionInput.GetButtonDown(ButtonCode.Right))
                {
                    SwitchSelect();
                }
            }

            public override void OnEnd()
            {
                pop_up_.transform.localScale = Vector3.zero;
            }

            // はい，いいえの選択状態を変更する
            private void SwitchSelect()
            {
                watch_yes_ = !watch_yes_;
                yes_text_.color = (watch_yes_) ? Color.red : Color.white;
                no_text_.color = (!watch_yes_) ? Color.red : Color.white;
            }

            // スキルを決定する
            private void Decide()
            {
                if (buyed) return;
                if (watch_yes_)
                {
                    Parent.audio_.PlayOneShot(Parent.buy_se_);
                    buyed = true;
                    levelup_message_.localScale = Vector3.zero;
                    levelup_icon_.sprite = skill_icon_.sprite;
                    //icon_back_.sprite = skill_icon_.sprite;
                    levelup_message_.DOScale(Vector3.one, levelup_message_time_).SetEase(levelup_ease_).OnComplete(
                        () => { levelup_message_.DOScale(Vector3.zero, levelup_message_time_ / 2f); ChangeState((int)eState.Select); });
                    SkillManager.Instance.SpendSkillPoint(Parent.skills_[Parent.select_index_].NeedPoint);
                    Parent.skills_[Parent.select_index_].LevelUp();
                    if(SkillManager.Instance.SkillPoint == 0) Parent.skill_point_text_.text = "<color=red>" + SkillManager.Instance.SkillPoint.ToString() + "SP</color>";
                    else Parent.skill_point_text_.text = SkillManager.Instance.SkillPoint.ToString() + "SP";
                }
                else
                {
                    Parent.audio_.PlayOneShot(Parent.cancel_se_);
                    ChangeState((int)eState.Select);
                }
                //ChangeState((int)eState.Select);
            }
        }
    }
}