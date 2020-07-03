using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイ時間などのゲームのプレイ状況を管理するクラス
/// </summary>

namespace Global
{
    public enum eBossType
    {
        Purin = 0,
        VernmDrake = 1,
        KawazTanBeta = 2,
        Senju = 3,
        ExPurin = 4,
        ExVernmDrake = 5,
        ExKawazTanBeta = 6,
        ExSenju = 7,
    }

    [System.Serializable]
    public class GlobalGameData : TadaLib.Save.BaseSaver<GlobalGameData>
    {
        // ゲーム全体のプレイ時間 (s)
        [SerializeField]
        private double eternal_timer_ = 0.0;
        public double EternalTimer => eternal_timer_ + (Time.time - prev_time_);

        // 現在のストーリーでのゲームのプレイ時間 (s)
        [SerializeField]
        private double story_timer_ = 0.0;
        public double StoryTimer => story_timer_ + ((is_story_timer_stoped_)? 0f : (Time.time - prev_time_));

        // 死亡回数
        [SerializeField]
        private int death_cnt_ = 0;
        public int DeathCnt => death_cnt_;
        // ボスごとの死亡回数
        [SerializeField]
        private List<int> death_cnts_;
        public List<int> DeathCnts => death_cnts_;

        // ボスを倒した回数
        [SerializeField]
        private int boss_defeat_cnt_ = 0;
        public int BossDefeatCnt => boss_defeat_cnt_;

        // ボスを倒した回数　ボスごとに
        [SerializeField]
        private List<int> each_boss_defeat_cnt_;
        public List<int> EachBossDefeatCnt => each_boss_defeat_cnt_;

        // エンディングを迎えた回数
        [SerializeField]
        private int ending_cnt_ = 0;
        public int EndingCnt => ending_cnt_;

        private const string kFileName = "GlobalData";

        // 前回セーブが完了したときのプレイ時間
        private float prev_time_ = 0.0f;

        private bool is_story_timer_stoped_ = false;

        // ロードする
        public bool Load()
        {
            // 常にセーブリクエストを送るようにする
            TadaLib.Save.SaveManager.Instance.RequestSaveAlways(() => 
            {
                eternal_timer_ += Time.time - prev_time_;
                if(!is_story_timer_stoped_) story_timer_ += Time.time - prev_time_;
                prev_time_ = Time.time;
                Save(kFileName); 
            });

            prev_time_ = Time.time;

            var data = Load(kFileName);

            if (data == null)
            {
                // 初期化
                each_boss_defeat_cnt_ = new List<int>(8);
                each_boss_defeat_cnt_.AddRange(System.Linq.Enumerable.Repeat(0, 8));
                death_cnts_ = new List<int>(8);
                death_cnts_.AddRange(System.Linq.Enumerable.Repeat(0, 8));
                return false;
            }

            eternal_timer_ = data.eternal_timer_;
            story_timer_ = data.story_timer_;
            death_cnt_ = data.death_cnt_;
            boss_defeat_cnt_ = data.boss_defeat_cnt_;
            ending_cnt_ = data.ending_cnt_;

            // ちゃんとロードできなかったら一から生成する
            if (data.death_cnts_ != null && data.death_cnts_.Count == 8) death_cnts_ = data.death_cnts_;
            else
            {
                death_cnts_ = new List<int>(8);
                death_cnts_.AddRange(System.Linq.Enumerable.Repeat(0, 8));
            }
            if (data.each_boss_defeat_cnt_ != null && data.each_boss_defeat_cnt_.Count == 8) each_boss_defeat_cnt_ = data.each_boss_defeat_cnt_;
            else
            {
                each_boss_defeat_cnt_ = new List<int>(8);
                each_boss_defeat_cnt_.AddRange(System.Linq.Enumerable.Repeat(0, 8));
            }

            return true;
        }

        // セーブする ここでは使わない (常にセーブ申請を送るようにしたいから)
        public void Save()
        {
            if (save_completed_)
            {
                save_completed_ = false;
                TadaLib.Save.SaveManager.Instance.RequestSave(() => { Save(kFileName); save_completed_ = true; });
            }
        }

        // データを削除する
        public void DeleteSaveData()
        {
            TadaLib.Save.SaveManager.Instance.DeleteData(kFileName);
            // データを初期化
            eternal_timer_ = 0.0;
            story_timer_ = 0.0;
            prev_time_ = Time.time;
            death_cnt_ = 0;
            boss_defeat_cnt_ = 0;
            ending_cnt_ = 0;
            for(int i = 0, n = each_boss_defeat_cnt_.Count; i < n; ++i)
            {
                each_boss_defeat_cnt_[i] = 0;
                death_cnts_[i] = 0;
            }
        }

        // 死亡回数を加算する
        public void AddDeathCnt(eBossType id)
        {
            ++death_cnt_;
            ++death_cnts_[(int)id];
        }

        public void AddBossDefeatCnt(eBossType id)
        {
            ++boss_defeat_cnt_;
            ++each_boss_defeat_cnt_[(int)id];
        }

        public void AddEndingCnt()
        {
            ++ending_cnt_;
        }

        public void RestartStoryTimer()
        {
            story_timer_ = 0.0;
            eternal_timer_ += Time.time - prev_time_;
            prev_time_ = Time.time;
            is_story_timer_stoped_ = false;

            // ついでにボスのカウントも
            for (int i = 0, n = each_boss_defeat_cnt_.Count; i < n; ++i)
            {
                each_boss_defeat_cnt_[i] = 0;
                death_cnts_[i] = 0;
            }
        }

        public void StopStoryTimer()
        {
            is_story_timer_stoped_ = true;
        }

        public void StartStoryTimer()
        {
            is_story_timer_stoped_ = false;
            eternal_timer_ += Time.time - prev_time_;
            prev_time_ = Time.time;
        }
    }

    public class GlobalDataManager : TadaLib.SingletonMonoBehaviour<GlobalDataManager>
    {
        // セーブデータ
        private GlobalGameData data_;

        protected override void Awake()
        {
            base.Awake();
            data_ = new GlobalGameData();
            data_.Load();
        }

        // セーブデータを削除する
        public static void DeleteSaveData()
        {
            Instance.data_.DeleteSaveData();
        }

        // 現在のゲーム全体のプレイ時間を取得する
        public static double EternalTimer => Instance.data_.EternalTimer;

        // 現在のストーリーでのプレイ時間を取得する
        public static double StoryTimer => Instance.data_.StoryTimer;

        // 現在のプレイヤーの死亡回数を取得する
        public static int DeathCnt => Instance.data_.DeathCnt;

        // 現在のボスの撃破回数を取得する
        public static int BossDefeatCnt => Instance.data_.BossDefeatCnt;

        // 現在のエンディングを迎えた回数を取得する
        public static int EndingCnt => Instance.data_.EndingCnt;

        // ボスを倒した回数を取得する
        public static int EachBossDefeatCnt(eBossType id)
        {
            return Instance.data_.EachBossDefeatCnt[(int)id];
        }

        // プレイヤーの死亡回数を加算する
        public static void AddDeathCnt(eBossType id)
        {
            Instance.data_.AddDeathCnt(id);
        }

        public static int DeathCnts(eBossType id)
        {
            return Instance.data_.DeathCnts[(int)id];
        }

        // ボスを撃破した回数を加算する
        public static void AddBossDefeatCnt(eBossType id)
        {
            Instance.data_.AddBossDefeatCnt(id);
        }

        // エンディングを迎えた回数を加算する
        public static void AddEndingCnt()
        {
            Instance.data_.AddEndingCnt();
        }

        // 現在のストーリー内でのタイマーをリセットする
        public static void RestartStoryTimer()
        {
            Instance.data_.RestartStoryTimer();
        }
        // 現在のストーリー内でのタイマーをストップする
        public static void StopStoryTimer()
        {
            Instance.data_.StopStoryTimer();
        }
        // 現在のストーリー内でのタイマーを再開する
        public static void StartStoryTimer()
        {
            Instance.data_.StartStoryTimer();
        }
    }
} // namespace Global