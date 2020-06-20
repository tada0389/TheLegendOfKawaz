using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Player;
using TadaLib;

/// <summary>
/// ターゲットを壊せでゴーストデータを生成する
/// フォートマットは，毎回フレーム プレイヤーの座標データを保存，
/// 加えて，アニメーションのコール時間を保存する
/// 
/// 入力情報をセーブするのと悩んだが，ズレるのを防ぐために上記の内容にした
/// </summary>

namespace TargetBreaking
{ 
    // List<tuple>をシリアライズできないので強引に
    [System.Serializable]
    public class ShotData
    {
        public float t;
        public short v;

        public ShotData(float time, short xpos)
        {
            t = time;
            v = xpos;
        }
    }

    [Serializable]
    public class GhostData : TadaLib.Save.BaseSaver<GhostData>
    {
        // 座標情報　セーブのために変数名を短くしている
        // 時間情報 + 座標の符号情報 + x座標情報 + y座標情報 を14文字の文字列で表す
        [SerializeField]
        public List<string> P;

        //[SerializeField]
        //[HideInInspector]
        //public List<Tuple<float, int>> Dir;

        // 呼ばれた時間とアニメーションの名前
        // 時間情報 + アニメーション情報
        [SerializeField]
        public List<string> A;

        [SerializeField]
        public List<ShotData> S;

        [System.NonSerialized]
        public bool RecordFailed;

        public GhostData()
        {
            RecordFailed = true;
        }

        public bool LoadObj(string kFileName)
        {
            var data = base.Load(kFileName);

            if(data == null)
            {
                return false;
            }

            P = data.P;
            A = data.A;
            S = data.S;

            return true;
        }

        public void SaveRequest(string kFileName)
        {
            if (save_completed_)
            {
                save_completed_ = false;
                TadaLib.Save.SaveManager.Instance.RequestSave(() => { Save(kFileName); save_completed_ = true; });
            }
        }

        public void Init()
        {
            P = new List<string>(100 * 10); // とりあえず10秒分確保
            A = new List<string>();
            S = new List<ShotData>();
        }

        public void AddPosData(int dt, int dx, int dy, int dir)
        {
            // データ圧縮
            int sign = 0;
            if (dx < 0) sign += 1;
            if (dy < 0) sign += 2;

            // dir 0で左 1で右
            if (dir == 1) sign += 4;

            dx = Mathf.Abs(dx);
            dy = Mathf.Abs(dy);

            string add = dt.ToString("D3") + sign.ToString() + dx.ToString("D2") + dy.ToString("D2");

            // これを64進数に変換して保存
            string num = GhostUtils.NumTo64Ary(int.Parse(add));

            P.Add(num);
        }

        public void AddAnimData(float t, string anim, eAnimType type)
        {
            int id = GhostUtils.Name2Index(anim);
            id += (int)type;
            int it = (int)(t * 1000.0f + 0.1f);
            string add = it.ToString("D5") + id.ToString("D2");

            // これを64進数に変換して保存
            string num = GhostUtils.NumTo64Ary(int.Parse(add));

            A.Add(num);
        }

        public void ResetData()
        {
            // 全てのデータを破棄
            P.Clear();
            A.Clear();
            S.Clear();
            RecordFailed = true;
        }
    }

    public class GhostRecorder : MonoBehaviour
    {
        private GhostData data_;

        [SerializeField]
        private Player target_;

        [SerializeField]
        private float record_max_time_ = 30f;

        [SerializeField]
        private float record_interval_ = 0.0199f;

        private float start_time_;
        private bool running_;
        private float elapsed_time_;
        private float prev_pos_time_;
        private float prev_pos_x_;
        private float prev_pos_y_;

        private void Awake()
        {
            data_ = new GhostData();
            data_.Init();
        }

        private void FixedUpdate()
        {
            if (running_)
            {
                if (target_ == null)
                {
                    running_ = false;
                    data_.ResetData();
                    data_.RecordFailed = true;
                    return;
                }

                float time = Time.time - start_time_;

                if (time >= record_max_time_)
                {
                    running_ = false;
                    data_.RecordFailed = true;
                    data_.ResetData();
                }

                for (int i = 0, n = target_.AnimCalled.Count; i < n; ++i)
                {
                    data_.AddAnimData(time, target_.AnimCalled[i], target_.AnimType[i]);
                }
                //if (target_.ShotCalled != eShotType.None) data_.S.Add(new ShotData(time, (short)target_.ShotCalled));

                // 座標データは一定間隔ごとに

                elapsed_time_ += Time.fixedDeltaTime;

                if (elapsed_time_ >= record_interval_)
                {
                    // 誤差をなくすためにいったん保存する値に変換してから再度座標計算
                    Vector3 pos = target_.transform.position;
                    float dx = pos.x - prev_pos_x_;
                    int i_dx = (int)(dx * 100.0f + 0.1f);
                    prev_pos_x_ += i_dx / 100f;
                    float dy = pos.y - prev_pos_y_;
                    int i_dy = (int)(dy * 100.0f + 0.1f);
                    prev_pos_y_ += i_dy / 100f;
                    float dt = Time.time - prev_pos_time_;
                    int i_dt = (int)(dt * 1000.0f + 0.1f);
                    prev_pos_time_ += i_dt / 1000f;

                    data_.AddPosData(i_dt, i_dx, i_dy, (int)target_.Dir);

                    while (elapsed_time_ >= record_interval_) elapsed_time_ -= record_interval_;
                }
            }
        }

        // ゴーストの取得スタート
        public void RecordStart()
        {
            data_.ResetData();
            start_time_ = Time.time;
            elapsed_time_ = 0.0f;
            running_ = true;
            data_.RecordFailed = false;
            prev_pos_time_ = Time.time;
            prev_pos_x_ = target_.transform.position.x;
            prev_pos_y_ = target_.transform.position.y;
        }

        public void RecordFinish(bool success)
        {
            running_ = false;
            if (!success) RecordFailed();
            else TargetSelectManager.PrevGameGhost = data_;
        }

        public void RecordFailed()
        {
            running_ = false;
            data_.RecordFailed = true;
            data_.ResetData();
        }
    }
}