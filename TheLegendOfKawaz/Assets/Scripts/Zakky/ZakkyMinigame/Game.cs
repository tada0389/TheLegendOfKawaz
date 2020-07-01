using Actor.Player;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace WallDefence
{
    public class Game : MonoBehaviour
    {
        //Gameクラスの唯一のインスタンス
        private static Game mInstance;
        //Gameのインスタンスを渡すパブリックな関数
        public static Game instance
        {
            get
            {
                //インスタンスが格納されているか
                if (mInstance == null)
                {
                    //インスタンスを探し、参照
                    mInstance = FindObjectOfType<Game>();
                }
                //インスタンスを返す
                return mInstance;
            }
        }

        //ゲームの状態
        public enum STATE
        {
            NONE,
            START,
            MOVE,
            FEVER,
            GAMEOVER
        };
        //ゲームの状態
        public STATE state
        {
            get;
            set;
        }

        // ゲーム終了処理が呼ばれたか
        private bool is_game_finish_called = false;

        [SerializeField]
        private Animator ui_animator_;
        [SerializeField]
        private Animator grade_animator_;

        [SerializeField]
        private ScoreText score_;

        [SerializeField]
        private int GoldBoader = 5000;
        [SerializeField]
        private int SilverBoader = 1000;

        [SerializeField]
        private Transform coin_spawer_pos_;


        // Start is called before the first frame update
        private IEnumerator Start()
        {
            //ゲームの状態をスタートにする
            state = STATE.MOVE;
            is_game_finish_called = false;

            // プレイヤーを動けないようにする
            Global.GlobalPlayerInfo.ActionEnabled = false;

            // Ready to Go の待機時間
            yield return new WaitForSeconds(1.5f);

            // プレイヤーを動けるようにする
            Global.GlobalPlayerInfo.ActionEnabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (state == STATE.GAMEOVER && !is_game_finish_called)
            {
                is_game_finish_called = true;
                StartCoroutine(FinishFlow());
            }
        }

        // ゲーム終了処理
        private IEnumerator FinishFlow()
        {
            if(ui_animator_) ui_animator_.Play("clear");

            float new_time_scale = 0.06f;

            // 時間を遅くする
            TadaLib.TimeScaler.Instance.RequestChange(new_time_scale);

            // 少し待つ
            yield return new WaitForSeconds(0.5f * new_time_scale);

            int score = score_.m_score;

            // スコアランキングに登録
            ScoreManager.Instance.RegisterScore(score, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            string grade = "Bronze";
            if (score >= GoldBoader) grade = "Gold";
            else if (score >= SilverBoader) grade = "Silver";
            if (grade_animator_) grade_animator_.Play(grade);
            // コイン出現
            Vector3 spawn_pos = (coin_spawer_pos_ != null) ? coin_spawer_pos_.position : Camera.main.transform.position;
            int point = score / 5;
            if(point > 0 && point % 5 != 0) point += 5 - point % 5;
            if (point < 0 && (-point) % 5 != 0) point += (-point) % 5;
            if (point > 0) SkillManager.Instance.GainSkillPoint(point, spawn_pos, 0.02f);
            else if (point < 0) SkillManager.Instance.SpendSkillPoint(-point, 0.02f);
            // 実績解除
            //AchievementManager.FireAchievement("key");
            //if (grade == "Gold") AchievementManager.FireAchievement("key");

            float time_duration = 3.5f * new_time_scale;

            yield return new WaitForSeconds(time_duration);

            TadaLib.Save.SaveManager.Instance.Save();

            // 時間をもとに戻す
            TadaLib.TimeScaler.Instance.DismissRequest(new_time_scale);

            KoitanLib.FadeManager.FadeIn(0.5f, "WellDefenceMediator");
        }
    }
}