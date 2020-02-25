using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 複数のターゲットを指定できる
/// カメラコントロールクラス
/// </summary>

namespace CameraSpace
{
    [RequireComponent(typeof(Camera))]
    public class MultipleTargetCamera : MonoBehaviour
    {
        private Camera cam_;
        [SerializeField]
        private BoxCollider2D boader_;
        [SerializeField]
        private List<Transform> targets_;

        [SerializeField]
        private Vector3 offset_;
        [SerializeField]
        private float smooth_time_ = 0.5f;

        [SerializeField]
        private float min_zoom_ = 40;
        [SerializeField]
        private float max_zoom_ = 10;
        [SerializeField]
        private float zoom_limiter_ = 50;

        private Vector3 velocity;

        private void Start()
        {
            cam_ = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (targets_.Count == 0) return;

            Zoom();
            Move();
        }

        private void Zoom()
        {
            var new_zoom = Mathf.Lerp(max_zoom_, min_zoom_, GetGreatestDistance() / zoom_limiter_);
            cam_.fieldOfView = Mathf.Lerp(cam_.fieldOfView, new_zoom, Time.deltaTime);
        }

        private void Move()
        {
            var center_point = GetCenterPoint();
            var new_position = center_point + offset_;
            new_position.z = -10f;
            // 指定ボーダー内に収める
            if (boader_ != null)
            {
                // 画面の幅と高さ
                float height = 10f * Mathf.Tan(cam_.fieldOfView * 0.5f * Mathf.Deg2Rad);
                float width = height * cam_.aspect;

                Vector2 origin = (Vector2)boader_.transform.position + boader_.offset * boader_.transform.localScale;
                Vector2 size = boader_.size * boader_.transform.localScale / 2.0f;

                // ボーダーの左,下,右,上の順
                float left_limit = origin.x - size.x;
                float bottom_limit = origin.y - size.y;
                float right_limit = origin.x + size.x;
                float top_limit = origin.y + size.y;

                // 現在の左，下，右，上
                float left_c = new_position.x - width;
                float bottom_c = new_position.y - height;
                float right_c = new_position.x + width;
                float top_c = new_position.y + height;

                if (left_c < left_limit) new_position.x -= left_c - left_limit;
                else if(right_c > right_limit) new_position.x -= right_c - right_limit;

                if(bottom_c < bottom_limit) new_position.y -= bottom_c - bottom_limit;
                else if(top_c > top_limit) new_position.y -= top_c - top_limit;
            }
            transform.position = Vector3.SmoothDamp(transform.position, new_position, ref velocity, smooth_time_);
        }

        private float GetGreatestDistance()
        {
            int x = 0;
            var bounds = new Bounds(targets_[0].position, Vector3.zero);
            for (int i = 0; i < targets_.Count; i++)
            {
                bounds.Encapsulate(targets_[i].position);
            }
            // x軸方向とy軸方向、差の大きい方を基準に全体のサイズを変更する
            return (bounds.size.x >= bounds.size.y) ? bounds.size.x : bounds.size.y; // ここ自分で変えた　神！！！！！

        }

        private Vector3 GetCenterPoint()
        {
            if (targets_.Count == 1) return targets_[0].position;
            var bounds = new Bounds(targets_[0].position, Vector3.zero);
            for (int i = 0; i < targets_.Count; i++)
            {
                bounds.Encapsulate(targets_[i].position);
            }
            return bounds.center;
        }
    }
}