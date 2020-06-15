using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

/// <summary>
/// 複数のターゲットを指定できる
/// カメラコントロールクラス
/// </summary>

namespace CameraSpace
{
    [System.Serializable]
    public class CameraData
    {
        // カメラが進むことのできる境界線
        [SerializeField]
        private BoxCollider2D boader_;
        public BoxCollider2D Boader => boader_;

        // 追従する対象
        [SerializeField]
        private List<Transform> targets_;
        public List<Transform> Targets { set { targets_ = value; } get { return targets_; } }

        [SerializeField]
        private Vector3 offset_;
        public Vector3 Offset => offset_;
        [SerializeField]
        private float smooth_time_ = 0.3f;
        public float SmoothTime => smooth_time_;

        [SerializeField]
        private float min_zoom_ = 85f;
        public float MinZoom => min_zoom_;
        [SerializeField]
        private float max_zoom_ = 75f;
        public float MaxZoom => max_zoom_;
        [SerializeField]
        private float zoom_limiter_ = 60f;
        public float ZoomLimiter => zoom_limiter_;
    }

    [RequireComponent(typeof(Camera))]
    public class MultipleTargetCamera : MonoBehaviour
    {
        private Camera cam_;

        [SerializeField]
        private CameraData data_;

        private Vector3 velocity;

        // カメラの座標 カメラを揺らすために実際の座標と分離している
        public Vector3 Position { private set; get; }

        // 揺らしている大きさ
        private Vector3 shake_dif;

        // 揺れの大きさの集合 大きい順
        TadaLib.MultiSet<float> shake_powers_;

        private void Start()
        {
            cam_ = GetComponent<Camera>();

            Position = transform.position;
            shake_dif = Vector3.zero;

            shake_powers_ = new TadaLib.MultiSet<float>();
        }

        private void FixedUpdate()
        {
            CheckTargetsDestroyed();
            if (data_.Targets.Count == 0) return;

            Zoom();
            Move();

            // カメラの座標と揺れを合わせる
            transform.position = Position + shake_dif;
        }

        // カメラを揺らす
        public void Shake(float power = 1.0f, float duration = 1.0f, float shake_interval = 0.05f)
        {
            StartCoroutine(DOShake(power, duration, shake_interval));
        }

        private IEnumerator DOShake(float power = 1.0f, float duration = 1.0f, float shake_interval = 0.05f)
        {
            shake_powers_.Insert(-power);
            float start = Time.time;

            while((Time.time - start) < duration)
            {
                // もしより震度の大きいものが回っていたらそっちを優先
                if (power >= -shake_powers_.ElementAt(0) - 1e-6)
                {
                    shake_dif.x = Random.Range(-power, power);
                    shake_dif.y = Random.Range(-power, power);
                }

                yield return new WaitForSeconds(shake_interval);
            }

            shake_dif = Vector3.zero;
            shake_powers_.Remove(-power);
        }

        // 削除されたターゲットをリストから削除する
        private void CheckTargetsDestroyed()
        {
            for(int i = data_.Targets.Count - 1; i >= 0; --i)
            {
                if(data_.Targets[i] == null)
                {
                    data_.Targets.RemoveAt(i);
                }
            }
        }

        // カメラのデータを変更する
        public void ChangeData(CameraData data)
        {
            data_ = data;
        }

        private void Zoom()
        {
            var new_zoom = Mathf.Lerp(data_.MaxZoom, data_.MinZoom, GetGreatestDistance() / data_.ZoomLimiter);
            cam_.fieldOfView = Mathf.Lerp(cam_.fieldOfView, new_zoom, Time.fixedDeltaTime);
        }

        private void Move()
        {
            var center_point = GetCenterPoint();
            var new_position = center_point + data_.Offset;
            new_position.z = -10f;
            // 指定ボーダー内に収める
            if (data_.Boader != null)
            {
                // 画面の幅と高さ
                float height = 10f * Mathf.Tan(cam_.fieldOfView * 0.5f * Mathf.Deg2Rad);
                float width = height * cam_.aspect;

                Vector2 origin = (Vector2)data_.Boader.transform.position + data_.Boader.offset * data_.Boader.transform.localScale;
                Vector2 size = data_.Boader.size * data_.Boader.transform.localScale / 2.0f;

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
            Position = Vector3.SmoothDamp(Position, new_position, ref velocity, data_.SmoothTime, Mathf.Infinity, Time.fixedDeltaTime);
        }

        // 縦横ともに考慮して最も長い方を返す
        private float GetGreatestDistance()
        {
            int x = 0;
            var bounds = new Bounds(data_.Targets[0].position, Vector3.zero);
            for (int i = 0; i < data_.Targets.Count; i++)
            {
                bounds.Encapsulate(data_.Targets[i].position);
            }
            // x軸方向とy軸方向、差の大きい方を基準に全体のサイズを変更する
            return (bounds.size.x >= bounds.size.y) ? bounds.size.x : bounds.size.y;

        }

        private Vector3 GetCenterPoint()
        {
            if (data_.Targets.Count == 1) return data_.Targets[0].position;
            var bounds = new Bounds(data_.Targets[0].position, Vector3.zero);
            for (int i = 0; i < data_.Targets.Count; i++)
            {
                bounds.Encapsulate(data_.Targets[i].position);
            }
            return bounds.center;
        }
    }
}