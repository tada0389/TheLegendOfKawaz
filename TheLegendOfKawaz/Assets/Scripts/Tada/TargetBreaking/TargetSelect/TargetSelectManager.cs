﻿using Actor.Player;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TargetBreaking
{

    public enum eGrade
    {
        Gold,
        Silver,
        Bronze,
    }

    [System.Serializable]
    public class StageData
    {
        [SerializeField]
        private string rank_name_;
        public string RankName => rank_name_;

        [SerializeField]
        private int need_point_;
        public int NeedPoint => need_point_;

        [SerializeField]
        private float gold_boader_time_;
        public float GoldBoaderTime => gold_boader_time_;

        [SerializeField]
        private int gold_reward_;
        public int GoldReward => gold_reward_;

        [SerializeField]
        private float silver_boader_time_;
        public float SilverBoaderTime => silver_boader_time_;

        [SerializeField]
        private int silver_reward_;
        public int SilverReward => silver_reward_;

        [SerializeField]
        private float bronze_boader_time_;
        public float BronzeBoaderTime => bronze_boader_time_;

        [SerializeField]
        private int bronze_reward_;
        public int BronzeReward => bronze_reward_;

        [SerializeField]
        private int other_reward_;
        public int OtherReward => other_reward_;

        [SerializeField]
        private float developer_time_;
        public float DeveloperTime => developer_time_;

        [SerializeField]
        private string next_scene_;
        public string NextScene => next_scene_;
    }

    // ステージ説明欄を開くときのイージング情報
    [Serializable]
    public class ExplonationBoxData
    {
        public Ease ease_ = Ease.OutQuart;
        public float open_duration_ = 0.5f;
        public float close_duration_ = 0.3f;
        public float inverse_duration_ = 0.5f;
        public Ease inverse_ease_ = Ease.InOutBack;
    }

    public class TargetSelectManager : MonoBehaviour
    {
        // 現在のステージを得る
        public static StageData CurStageData = null;

        [SerializeField]
        private StageData[] stages_;

        // 今見てるインデックス // firstがグレード選び secondが始めるかどうか
        private TadaLib.Pair<int, int> index_;

        private static int prev_stage_index_;

        // 難易度選択中か
        private bool selecting_grade_;

        private bool is_feeding_ = false;

        [SerializeField]
        private Image explonation_box_;

        [SerializeField]
        private TextMeshProUGUI[] grades_;

        [SerializeField]
        private TextMeshProUGUI[] go_back_texts_;

        [SerializeField]
        private TextMeshProUGUI grade_text_;
        [SerializeField]
        private TextMeshProUGUI needpoint_text_;
        [SerializeField]
        private TextMeshProUGUI gold_text_;
        [SerializeField]
        private TextMeshProUGUI silver_text_;
        [SerializeField]
        private TextMeshProUGUI bronze_text_;
        [SerializeField]
        private TextMeshProUGUI other_text_;
        [SerializeField]
        private TextMeshProUGUI developer_text_;

        [SerializeField]
        private TextMeshProUGUI skill_point_text_;

        [SerializeField]
        private ExplonationBoxData box_data_;

        // スコアデータ
        [SerializeField]
        private Result.ScoreDisplay score_displayer_;

        [SerializeField]
        private GameObject stage_texts_;
        [SerializeField]
        private GameObject score_texts_;

        // Start is called before the first frame update
        private void Awake()
        {
            index_ = new TadaLib.Pair<int, int>(prev_stage_index_, 0);
            selecting_grade_ = true;
            grades_[prev_stage_index_].color = Color.red;
        }

        private void Start()
        {
            skill_point_text_.text = SkillManager.Instance.SkillPoint.ToString();
        }

        // Update is called once per frame
        void Update()
        {
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

            if (selecting_grade_)
            {
                int prev = index_.first;
                if (ActionInput.GetButtonDown(ButtonCode.Up)) index_.first = (index_.first + 3) % grades_.Length;
                else if (ActionInput.GetButtonDown(ButtonCode.Down)) index_.first = (index_.first + 1) % grades_.Length;
                if (prev != index_.first)
                {
                    grades_[index_.first].color = Color.red;
                    grades_[prev].color = Color.white;
                }
            }
            else
            {
                // スコアを表示
                if (ActionInput.GetButtonDown(ActionCode.Dash))
                {
                    ChangeStageDataDisplay();
                    return;
                }
                if (!score_texts_.activeSelf)
                {
                    int prev = index_.second;
                    if (ActionInput.GetButtonDown(ButtonCode.Left) || ActionInput.GetButtonDown(ButtonCode.Right)) index_.second = 1 - index_.second;
                    if (prev != index_.second)
                    {
                        go_back_texts_[index_.second].color = Color.red;
                        go_back_texts_[prev].color = Color.white;
                    }
                }
            }

            CheckStageData();
        }

        // 現在のインデックスで決定する
        private void GoNext()
        {
            if (selecting_grade_)
            {
                if(index_.first == 3 && !is_feeding_)
                {
                    is_feeding_ = true;
                    // 前のシーンに戻る
                    KoitanLib.FadeManager.FadeIn(0.5f, "ZakkyScene");
                }
                else
                {
                    OpenExplonation(stages_[index_.first]);
                }
            }
            else
            {
                if (score_texts_.activeSelf) return; // ランキング表示

                if(index_.second == 0 && !is_feeding_)
                {
                    is_feeding_ = true;
                    // 実際に遊ぶ
                    CurStageData = stages_[index_.first];
                    prev_stage_index_ = index_.first;
                    KoitanLib.FadeManager.FadeIn(0.5f, stages_[index_.first].NextScene);

                }
                else
                {
                    // 戻る
                    GoBack();
                }
            }
        }

        // ひとつ前に戻る
        private void GoBack()
        {
            if (selecting_grade_)
            {
                // やめるにカーソルを合わせる
                grades_[index_.first].color = Color.white;
                index_.first = grades_.Length - 1;
                grades_[index_.first].color = Color.red;
            }
            else
            {
                CloseExplonation();
            }
        }

        // 説明欄を更新する
        private void OpenExplonation(StageData data)
        {
            selecting_grade_ = false;
            go_back_texts_[0].color = Color.red;
            go_back_texts_[1].color = Color.white;
            index_.second = 0;
            //explonation_box_.gameObject.SetActive(true);

            // 中身を変更する
            grade_text_.text = data.RankName;
            needpoint_text_.text = "必要SP : " + data.NeedPoint.ToString();
            gold_text_.text =   "Gold      " + data.GoldBoaderTime.ToString("F1") + "s以内   -> " + data.GoldReward + "SP";
            silver_text_.text = "Silver    " + data.SilverBoaderTime.ToString("F1") + "s以内   -> " + data.SilverReward + "SP";
            bronze_text_.text = "Bronze   " + data.BronzeBoaderTime.ToString("F1") + "s以内   -> " + data.BronzeReward + "SP";
            other_text_.text =  "Other     " + data.OtherReward + "SP";
            developer_text_.text = "Developer Time  :  " + data.DeveloperTime.ToString("F1") + "s";

            explonation_box_.rectTransform.DOKill();
            explonation_box_.rectTransform.DOLocalRotate(Vector3.zero, box_data_.open_duration_).SetEase(box_data_.ease_);
        }

        // 説明欄を閉じる
        private void CloseExplonation()
        {
            selecting_grade_ = true;
            //explonation_box_.gameObject.SetActive(false);
            explonation_box_.rectTransform.DOKill();
            explonation_box_.rectTransform.DOLocalRotate(new Vector3(0f, -90f, 0f), box_data_.close_duration_).SetEase(box_data_.ease_);

            //// ランキング表示からステージ詳細に戻す
            //score_texts_.SetActive(false);
            //stage_texts_.SetActive(true);
        }

        // ステージの詳細を変更する
        private void ChangeStageDataDisplay()
        {
            explonation_box_.rectTransform.DOKill();
            if(stage_texts_.activeSelf) explonation_box_.rectTransform.DOLocalRotate(new Vector3(0f, -180f, 0f), box_data_.inverse_duration_).SetEase(box_data_.inverse_ease_);
            else explonation_box_.rectTransform.DOLocalRotate(new Vector3(0f,    0f, 0f), box_data_.inverse_duration_).SetEase(box_data_.inverse_ease_);
        }
        
        // ステージの詳細を表示するか，スコア表を表示するか確認する
        private void CheckStageData()
        {
            if (selecting_grade_) return;
            float r_y = explonation_box_.rectTransform.localEulerAngles.y;
            //Debug.Log(r_y);
            if (stage_texts_.activeSelf && (r_y > 90f && r_y < 270f))
            {
                //Debug.Log("ランク");
                score_displayer_.Display(stages_[index_.first].NextScene, ScoreManager.Instance.GetGameName(stages_[index_.first].NextScene));
                score_texts_.SetActive(true);
                stage_texts_.SetActive(false);
            }
            else if(score_texts_.activeSelf && (r_y < 90f || r_y > 270f))
            {
                //Debug.Log("データ");
                score_texts_.SetActive(false);
                stage_texts_.SetActive(true);
            }
        }
    }
}