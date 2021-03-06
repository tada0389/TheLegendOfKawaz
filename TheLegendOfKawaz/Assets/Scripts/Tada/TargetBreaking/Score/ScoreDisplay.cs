﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TadaLib.Save;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Result
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text_;

        private ScoreManager score_manager_;

        [SerializeField]
        private bool IsTarget = true;

        // Start is called before the first frame update
        void Start()
        {
            score_manager_ = ScoreManager.Instance;
            //Display(score_manager_.LatestGameScene, score_manager_.LatestGame, score_manager_.LatestRank);
        }

        public void Display(string game_scene_name, string game_name, int rank = -1)
        {
            if (IsTarget)
            {
                string res = "";
                res += game_name;
                res += "\nスコアランキング\n";
                Score score = score_manager_.GetScoreData(game_scene_name);
                for (int i = 0, n = score.Scores.Count; i < n; ++i)
                {
                    if (i == rank - 1) res += "<color=red>";
                    res += (i + 1).ToString() + "位";
                    res += String.Format("{0, 6}", (-score.Scores[i] / 100.0f).ToString("F1"));
                    if (i == rank - 1) res += "</color>";
                    res += "\n";
                }
                text_.text = res;
            }
            else
            {
                string res = "";
                res += game_name;
                res += "\nスコアランキング\n";
                Score score = score_manager_.GetScoreData(game_scene_name);
                for (int i = 0, n = score.Scores.Count; i < n; ++i)
                {
                    if (i == rank - 1) res += "<color=red>";
                    res += (i + 1).ToString() + "位";
                    res += String.Format("{0, 8}", (score.Scores[i]).ToString());
                    if (i == rank - 1) res += "</color>";
                    res += "\n";
                }
                text_.text = res;
            }

            // 順位の表示量に応じてテキストのフォントサイズを変更する
            //text_.fontSize = 50 - 3 * score.Scores.Count;
        }

        public string GetScoreRanking(string game_scene_name, string game_name, int rank = -1)
        {
            string res = "";
            //res += game_name + "\n";
            res += "<size=26>【スコアランキング】</size>\n";
            res += "<size=2>\n</size>";
            Score score = score_manager_.GetScoreData(game_scene_name);
            for (int i = 0, n = score.Scores.Count; i < n; ++i)
            {
                if (i == rank - 1) res += "<color=red>";
                res += "  " + (i + 1).ToString() + "位";
                if(IsTarget) res += String.Format("{0, 6}", (-score.Scores[i] / 100.0f).ToString("F2")) + " (s)";
                else res += String.Format("{0, 8}", (score.Scores[i]).ToString()) + " (pt)";
                if (i == rank - 1) res += "</color>";
                res += "\n";
            }

            return res;
        }
    }
} // namespace Result