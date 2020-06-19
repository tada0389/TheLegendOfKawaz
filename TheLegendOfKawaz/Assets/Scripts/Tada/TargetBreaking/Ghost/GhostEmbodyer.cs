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

        string prev_anim_ = "";

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
                ghost_.gameObject.SetActive(true);
                float time = Time.time - start_time_;

                // 座標の更新
                while (true)
                {
                    if (pos_index_ >= pos_count_) break;
                    if (data_.Pos[pos_index_].Item1 > time) break;

                    ghost_.transform.position = new Vector3(data_.Pos[pos_index_].Item2, data_.Pos[pos_index_].Item3);
                    ++pos_index_;
                }

                // アニメーションの更新
                while (true)
                {
                    if (anim_index_ >= anim_count_) break;
                    if (data_.Anim[anim_index_].Item1 > time) break;

                    bool same = prev_anim_ == data_.Anim[anim_index_].Item2;
                    ghost_.PlayAnim(data_.Anim[anim_index_].Item2, data_.Anim[anim_index_].Item3);
                    prev_anim_ = data_.Anim[anim_index_].Item2;

                    ++anim_index_;
                }

                // ショットの更新
                while (true)
                {
                    if (shot_index_ >= shot_count_) break;
                    if (data_.Shot[shot_index_].Item1 > time) break;

                    ghost_.Shot(data_.Shot[shot_index_].Item2);
                    ++shot_index_;
                }

                bool finish = (pos_index_ == pos_count_ && anim_index_ == anim_count_ && shot_index_ == shot_count_);
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
            shot_index_ = 0;
        }

        // ゴーストの具現化を開始する
        public void EmbodyStart()
        {
            Reset();
            start_time_ = Time.time;
            running_ = true;
            ghost_.gameObject.SetActive(true);
        }

        // ゴーストの具現化を終了する
        public void EmbodyFinish()
        {
            running_ = false;
            ghost_.gameObject.SetActive(false);
        }
    }
}