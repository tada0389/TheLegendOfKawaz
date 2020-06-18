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
        int shot_index_;

        int pos_count_;
        int anim_count_;
        int shot_count_;

        public void LoadGhost(GhostData data)
        {
            data_ = data;
            pos_count_ = data.Pos.Count;
            anim_count_ = data.Anim.Count;
            shot_count_ = data.Shot.Count;
        }

        private void FixedUpdate()
        {
            if (running_)
            {

                float time = Time.time - start_time_;

                // 座標の更新
                while (true)
                {
                    if (pos_index_ + 1 >= pos_count_) break;
                    if (data_.Pos[pos_index_ + 1].Item1 > time) break;

                    ++pos_index_;
                    ghost_.transform.position = new Vector3(data_.Pos[pos_index_].Item2, data_.Pos[pos_index_].Item3);
                }

                // アニメーションの更新
                while (true)
                {
                    break;
                }

                // ショットの更新
                while (true)
                {
                    if (shot_index_ + 1 >= shot_count_) break;
                    if (data_.Shot[shot_index_ + 1].Item1 > time) break;

                    ++shot_index_;
                    ghost_.Shot(data_.Shot[shot_index_].Item2);
                }
            }
        }

        private void Reset()
        {
            pos_index_ = 0;
            anim_index_ = 0;
            shot_index_ = 0;
        }

        // ゴーストの具現化を開始する
        public void EmbodyStart()
        {
            start_time_ = Time.time;
            running_ = true;
            Reset();
        }

        // ゴーストの具現化を終了する
        public void EmbodyEnd()
        {
            running_ = false;
        }
    }
}