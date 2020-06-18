using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;

/// <summary>
/// ターゲットを壊せでゴーストデータを生成する
/// フォートマットは，毎回フレーム プレイヤーの座標データを保存，
/// 加えて，アニメーションのコール時間を保存する
/// </summary>

namespace TargetBreaking
{ 
    [Serializable]
    public class GhostData
    {
        [SerializeField]
        [HideInInspector]
        public List<Tuple<float, float, float>> Pos;

        // 呼ばれた時間とアニメーションの名前
        [SerializeField]
        [HideInInspector]
        public List<Tuple<float, string>> Anim;

        [SerializeField]
        [HideInInspector]
        public List<Tuple<float, int>> Shot;

        [SerializeField]
        [HideInInspector]
        public bool RecordFailed;

        public GhostData()
        {
            Pos = new List<Tuple<float, float, float>>(100 * 10); // とりあえず10秒分確保
            Anim = new List<Tuple<float, string>>();
            Shot = new List<Tuple<float, int>>();
            RecordFailed = true;
        }

        public void Reset()
        {
            // 全てのデータを破棄
            Pos.Clear();
            Anim.Clear();
            Shot.Clear();
            RecordFailed = true;
        }
    }

    public class GhostSaver : MonoBehaviour
    {
        private GhostData data_;

        [SerializeField]
        private Player target_;

        [SerializeField]
        private float record_max_time_ = 30f;

        private float start_time_;
        private bool running_;

        private void Awake()
        {
            data_ = new GhostData();
        }

        private void FixedUpdate()
        {
            if (running_)
            {
                if (target_ == null)
                {
                    running_ = false;
                    data_.Reset();
                    data_.RecordFailed = true;
                    return;
                }

                float time = Time.time - start_time_;

                if(time >= record_max_time_)
                {
                    running_ = false;
                    data_.RecordFailed = true;
                    data_.Reset();
                }

                Vector3 pos = target_.transform.position;
                data_.Pos.Add(Tuple.Create(time, pos.x, pos.y));
                if (target_.AnimCalled != "") data_.Anim.Add(Tuple.Create(time, target_.AnimCalled));
                if (target_.ShotCalled != ShotType.None) data_.Shot.Add(Tuple.Create(time, (int)target_.ShotCalled));
            }
        }

        // ゴーストの取得スタート
        public void RecordStart()
        {
            data_.Reset();
            start_time_ = Time.time;
            running_ = true;
            data_.RecordFailed = false;
        }

        public void RecordEnd()
        {
            running_ = false;
        }
    }
}