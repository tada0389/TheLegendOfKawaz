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
                // 左,下,右,上の順
                float left = boader_.transform.position.x - boader_.offset.x;
                float bottom = boader_.transform.position.y - boader_.offset.y;
                float right = boader_.transform.position.x + boader_.offset.x;
                float top = boader_.transform.position.y + boader_.offset.y;

                new_position.x = Mathf.Clamp(new_position.x, left, right);
                new_position.y = Mathf.Clamp(new_position.y, bottom, top);
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