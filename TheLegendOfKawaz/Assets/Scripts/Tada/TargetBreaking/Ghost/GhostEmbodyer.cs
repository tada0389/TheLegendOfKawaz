using Actor.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴーストを具現化するクラス
/// </summary>

namespace TargetBreaking
{
    public class GhostEmbodyer : MonoBehaviour
    {
        private GhostData data_;

        [SerializeField]
        private GhostPlayer ghost_;

        private float start_time_;
        private bool running_;

        int pos_index_;
        int anim_index_;

        int pos_count_;
        int anim_count_;

        private float next_pos_time_;
        private float next_pos_x_;
        private float next_pos_y_;
        private float next_pos_dir_;
        private bool pos_decoded_;

        private float next_anim_time_;
        private string next_anim_name_;
        private int next_anim_type_;
        private bool anim_decoded_;

        private float pos_timer_;

        private Vector3 prev_ghost_pos_;

        public void LoadGhost(GhostData data)
        {
            data_ = data;
            pos_count_ = data.P.Count;
            anim_count_ = data.A.Count;
        }

        private void FixedUpdate()
        {
            if (running_)
            {
                ghost_.gameObject.SetActive(true);
                float time = Time.time - start_time_;

                bool pos_changed = false;

                // 座標の更新
                while (true)
                {
                    if (pos_index_ >= pos_count_) break;
                    if (!pos_decoded_) DecodePosData(data_.P[pos_index_]);
                    if (pos_timer_ + next_pos_time_ > time) break;

                    pos_changed = true;
                    pos_decoded_ = false;
                    pos_timer_ += next_pos_time_;

                    ghost_.transform.position = prev_ghost_pos_ + new Vector3(next_pos_x_, next_pos_y_);
                    ghost_.transform.localScale = new Vector3(next_pos_dir_, ghost_.transform.localScale.y, ghost_.transform.localScale.z);
                    prev_ghost_pos_ = ghost_.transform.position;
                    ++pos_index_;
                }

                // 線形補間する
                if (!pos_changed)
                {
                    float t = (time - pos_timer_) / next_pos_time_;
                    ghost_.transform.position = prev_ghost_pos_ + new Vector3(next_pos_x_ * t, next_pos_y_ * t);
                }

                // アニメーションの更新
                while (true)
                {
                    if (anim_index_ >= anim_count_) break;
                    if (!anim_decoded_) DecodeAnimData(data_.A[anim_index_]);
                    if (next_anim_time_ > time) break;

                    anim_decoded_ = false;

                    ghost_.PlayAnim(next_anim_name_, next_anim_type_);
                    ++anim_index_;
                }

                bool finish = (pos_index_ == pos_count_ && anim_index_ == anim_count_);
                if (finish)
                {
                    EmbodyFinish();
                }
            }
        }

        private void Reset()
        {
            pos_index_ = 0;
            anim_index_ = 0;
            pos_timer_ = 0.0f;
        }

        // ゴーストの具現化を開始する
        public void EmbodyStart()
        {
            Reset();
            start_time_ = Time.time;
            running_ = true;
            ghost_.gameObject.SetActive(true);
            pos_decoded_ = false;
            prev_ghost_pos_ = ghost_.transform.position;
        }

        // ゴーストの具現化を終了する
        public void EmbodyFinish()
        {
            running_ = false;
            ghost_.gameObject.SetActive(false);
        }

        // 座標データを解凍する
        private void DecodePosData(string target)
        {
            if (pos_decoded_) return;

            // 64進数から10進数に変換
            long num = GhostUtils.Ary64ToNum(target);
            // 左寄せで8文字にする
            target = num.ToString().PadLeft(8, '0');

            int time = int.Parse(target.Substring(0, 3));
            int sign = int.Parse(target.Substring(3, 1));
            int x = int.Parse(target.Substring(4, 2));
            int y = int.Parse(target.Substring(6, 2));

            next_pos_time_ = time / 1000.0f;
            next_pos_x_ = x / 100.0f;
            next_pos_y_ = y / 100.0f;
            if((sign & 1) >= 1) next_pos_x_ *= -1f;
            if((sign & 2) >= 1) next_pos_y_ *= -1f;
            if ((sign & 4) >= 1) next_pos_dir_ = 1; // 1で右
            else next_pos_dir_ = -1;

            pos_decoded_ = true;
        }

        // アニメーションデータを解凍する
        private void DecodeAnimData(string target)
        {
            if (anim_decoded_) return;

            // 64進数から10進数に変換
            long num = GhostUtils.Ary64ToNum(target);
            // 左寄せで7文字にする
            target = num.ToString().PadLeft(7, '0');

            int time = int.Parse(target.Substring(0, 5));
            int anim = int.Parse(target.Substring(5, 2));

            next_anim_time_ = time / 1000.0f;
            next_anim_name_ = GhostUtils.Index2Name(anim);
            next_anim_type_ = anim % 4;

            anim_decoded_ = true;
        }
    }
}