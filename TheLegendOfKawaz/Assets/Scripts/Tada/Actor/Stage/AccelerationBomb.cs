using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触れると爆発して触れたものを加速させるギミック
/// celesteのフグの衝撃波に似てる
/// </summary>

namespace Stage
{
    public class AccelerationBomb : MonoBehaviour
    {
        [SerializeField]
        private float explonation_power_ = 0.5f;

        [SerializeField]
        private Vector2 MaxTargetVelocity = new Vector2(0.5f, 0.5f);

        // 追加でほか方向に速度を加える
        [SerializeField]
        private Vector2 add_power_ = new Vector2(0f, 0.2f);

        [SerializeField]
        private float respawn_duration_ = 3.0f;

        private TadaLib.Timer timer_;

        private SpriteRenderer renderer_;
        private CircleCollider2D hit_box_;

        private void Start()
        {
            timer_ = new TadaLib.Timer(respawn_duration_);
            renderer_ = GetComponent<SpriteRenderer>();
            hit_box_ = GetComponent<CircleCollider2D>();
        }

        private void Update()
        {
            if (!renderer_.enabled && timer_.IsTimeout())
            {
                Respawn();
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            var trb = collision.GetComponent<TadaLib.TadaRigidbody>();

            if (trb != null) Explosion(trb);
        }

        private void Explosion(TadaLib.TadaRigidbody trb)
        {
            float dir = Mathf.Sign(trb.transform.position.x - transform.position.x);
            trb.AddForce(add_power_ * dir, MaxTargetVelocity, TadaLib.eForceMode.VelocityChange);
            renderer_.enabled = false;
            hit_box_.enabled = false;
            timer_.TimeReset();
        }

        private void Respawn()
        {
            renderer_.enabled = true;
            hit_box_.enabled = true;
        }
    }
} // namespace Stage