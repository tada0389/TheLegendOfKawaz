using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// プレイヤーが上にいたら落ちる床
/// マリオで言うちくわブロック
/// </summary>

namespace Stage
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DropBlock : TadaLib.Mover
    {
        // 乗っても大丈夫な時間
        [SerializeField]
        private float stay_duration_ = 1.0f;

        [SerializeField]
        private float drop_speed_ = 0.1f;

        // 落ちてからの生存時間
        [SerializeField]
        private float life_time_ = 5.0f;

        // 色を変えるか
        [SerializeField]
        private bool change_color_ = true;

        private bool rided_ = false;

        private Timer stay_timer_;
        private Timer drop_timer_;

        private bool is_dropping_ = false;

        private Vector3 default_position_;
        private Color default_color_;

        private SpriteRenderer sp_renderer_;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            stay_timer_ = new Timer(stay_duration_);
            drop_timer_ = new Timer(life_time_);
            default_position_ = transform.position;

            sp_renderer_ = GetComponent<SpriteRenderer>();
            if(change_color_) default_color_ = sp_renderer_.color;
        }

        protected override void FixedUpdate()
        {
            if (is_dropping_)
            {
                // そのまま落下
                transform.position += new Vector3(0f, -drop_speed_ * Time.deltaTime * 60f);

                if (drop_timer_.IsTimeout())
                {
                    drop_timer_.TimeReset();
                    Reset();
                }

            }
            else
            {
                if (rided_)
                {
                    if (stay_timer_.IsTimeout())
                    {
                        is_dropping_ = true;
                        drop_timer_.TimeReset();
                    }
                }
                else stay_timer_.TimeReverse(Time.deltaTime * 2f);
                // 色を変更
                float t = stay_timer_.GetTime() / stay_duration_;
                if(change_color_) sp_renderer_.color = new Color(1f, 1f - t, 1f - t, 1f);
            }
            rided_ = false;
            base.FixedUpdate();
        }

        public override void Rided()
        {
            rided_ = true;
        }

        private void Reset()
        {
            is_dropping_ = false;
            transform.position = default_position_;
            if(change_color_) sp_renderer_.color = default_color_;
            stay_timer_.TimeReset();
        }
    }
} // namespace Stage