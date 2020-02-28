using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace TadaLib
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TadaRigidbody : MonoBehaviour
    {
        // 速度
        [System.NonSerialized]
        public Vector2 Velocity = Vector2.zero;
        // 速度にかける倍率
        public float InitSpeed = 1.0f;

        // すり抜ける床の判定を受けるかどうか
        [System.NonSerialized]
        public bool IsThrough = false;

        // 最大何度までの角度を登れるか
        [SerializeField]
        private float MaxClimbDegree = 45f;

        // 左でぶつかっている
        public bool LeftCollide { private set; get; }
        // 右でぶつかっている
        public bool RightCollide { private set; get; }
        // 上でぶつかっている
        public bool TopCollide { private set; get; }
        // 下でぶつかっている
        public bool BottomCollide { private set; get; }

        private BoxCollider2D hit_box_;

        private const float kEpsilon = 0.001f;

        private void Start()
        {
            // 衝突の初期化
            LeftCollide = false;
            RightCollide = false;
            TopCollide = false;
            BottomCollide = false;

            hit_box_ = GetComponent<BoxCollider2D>();
        }

        private void LateUpdate()
        {
            Move();
        }

        private void Move()
        {
            // 衝突の初期化
            LeftCollide = false;
            RightCollide = false;
            TopCollide = false;
            BottomCollide = false;

            Vector2 scale = transform.localScale;

            // 当たり判定(矩形)のサイズと中心
            Vector2 offset = hit_box_.offset * scale;
            Vector2 half_size = hit_box_.size * scale * 0.5f;

            // 移動量
            Vector2 d = InitSpeed * Velocity * Time.deltaTime * 60f;

            // レイキャストを飛ばす中心
            Vector2 origin = (Vector2)transform.position + offset;
            // レイの長さ
            float length_x = half_size.x / 2f;
            // y軸は少し余分に取る
            float length_y = half_size.y + half_size.x * 1.5f;

            // 名前思いつかなかった・・・ すり抜ける床を含めないのはmask_1
            int mask_0 = (IsThrough || d.y > 0f)? 1 << 8 : 1 << 8 | 1 << 9;
            int mask_1 = 1 << 8;

            // 始めにy軸方向 3本の線を出す 坂道チェックもする
            {
                // 左端，中央，右端の順に確かめる
                Vector2 origin_left = origin + new Vector2(-half_size.x * 0.8f, 0f);
                Vector2 origin_right = origin + new Vector2(half_size.x * 0.8f, 0f);
                RaycastHit2D hit_down_left = LinecastWithGizmos(origin_left, origin_left + new Vector2(0f, -length_y + d.y), mask_0);
                RaycastHit2D hit_down_center = LinecastWithGizmos(origin, origin + new Vector2(0f, -length_y + d.y), mask_0);
                RaycastHit2D hit_down_right = LinecastWithGizmos(origin_right, origin_right + new Vector2(0f, -length_y + d.y), mask_0);

                // めり込んでいる分は上に持ち上げる
                float length = half_size.y; // 地面までの通常の距離
                float tmp_d_y = -100f;
                if (hit_down_left) tmp_d_y = Mathf.Max(tmp_d_y, length - hit_down_left.distance);
                if (hit_down_center) tmp_d_y = Mathf.Max(tmp_d_y, length - hit_down_center.distance);
                if (hit_down_right) tmp_d_y = Mathf.Max(tmp_d_y, length - hit_down_right.distance);
                d.y = Mathf.Max(d.y, tmp_d_y);
                BottomCollide |= (tmp_d_y >= (d.y - kEpsilon));

                // ヒットしているなら，それぞれの法線ベクトルを取得 坂道対応
                if (Mathf.Abs(d.x) > kEpsilon)
                {
                    if (hit_down_left && d.x > kEpsilon)
                    {
                        float theta = Mathf.Atan2(hit_down_left.normal.y, hit_down_left.normal.x) - Mathf.PI / 2f;

                        if (theta < Mathf.Deg2Rad * MaxClimbDegree)
                        {
                            d.y += d.x * Mathf.Sin(theta);
                            d.x *= Mathf.Cos(theta);
                        }
                    }
                    else if (hit_down_right && d.x < kEpsilon)
                    {
                        float theta = Mathf.Atan2(hit_down_right.normal.y, hit_down_right.normal.x) - Mathf.PI / 2f;
                        if (theta < Mathf.Deg2Rad * MaxClimbDegree)
                        {
                            d.y += d.x * Mathf.Sin(theta);
                            d.x *= Mathf.Cos(theta);
                        }
                    }
                    else if (hit_down_center)
                    {
                        float theta = Mathf.Atan2(hit_down_center.normal.y, hit_down_center.normal.x) - Mathf.PI / 2f;

                        if (theta < Mathf.Deg2Rad * MaxClimbDegree)
                        {
                            d.y += d.x * Mathf.Sin(theta);
                            d.x *= Mathf.Cos(theta);
                        }
                    }
                }
            }

            // 次に上方向 下向き方向にヒットしたならば，上向き方向はやらない
            if(!BottomCollide){
                // 左端，中央，右端の順に確かめる
                Vector2 origin_left = origin + new Vector2(-half_size.x * 0.8f, 0f);
                Vector2 origin_right = origin + new Vector2(half_size.x * 0.8f, 0f);
                float length = half_size.y;
                RaycastHit2D hit_down_left = LinecastWithGizmos(origin_left, origin_left + new Vector2(0f, length + d.y), mask_1);
                RaycastHit2D hit_down_center = LinecastWithGizmos(origin, origin + new Vector2(0f, length + d.y), mask_1);
                RaycastHit2D hit_down_right = LinecastWithGizmos(origin_right, origin_right + new Vector2(0f, length + d.y), mask_1);

                // めり込んでいる分は下に下げる
                float len = half_size.y; // 地面までの通常の距離
                float tmp_d_y = 100f;
                if (hit_down_left) tmp_d_y = Mathf.Min(tmp_d_y, hit_down_left.distance - len);
                if (hit_down_center) tmp_d_y = Mathf.Min(tmp_d_y, hit_down_center.distance - len);
                if (hit_down_right) tmp_d_y = Mathf.Min(tmp_d_y, hit_down_right.distance - len);
                d.y = Mathf.Min(d.y, tmp_d_y);
                TopCollide |= (tmp_d_y <= (d.y + kEpsilon));
            }

            // y軸移動を考慮する
            origin.y += d.y;

            // 次に左右
            {
                // まずはx軸方向
                if (d.x < 0)
                {
                    RaycastHit2D hit_left = Physics2D.BoxCast(origin, new Vector2(half_size.x, half_size.y * 0.6f), 0f, Vector2.left,
                        -d.x + half_size.x / 2f, mask_1);
                    Debug.DrawLine(origin, origin - new Vector2(half_size.x - d.x, 0f), Color.blue);
                    if (hit_left)
                    {
                        d.x = -hit_left.distance + half_size.x / 2f;
                    }
                    if (hit_left) LeftCollide = true;

                }
                else if (d.x > 0)
                {
                    RaycastHit2D hit_right = Physics2D.BoxCast(origin, new Vector2(half_size.x, half_size.y * 0.6f), 0f, Vector2.right,
                        d.x + half_size.x / 2f, mask_1);
                    Debug.DrawLine(origin, origin + new Vector2(half_size.x + d.x, 0f), Color.blue);
                    if (hit_right)
                    {
                        d.x = hit_right.distance - half_size.x / 2f;
                    }
                    if (hit_right) RightCollide = true; ;
                }
            }

            transform.position += (Vector3)d;
        }

        // レイキャストを飛ばす(+ Debugの線を引く)
        RaycastHit2D LinecastWithGizmos(Vector2 from, Vector2 to, int layer_mask)
        {
            RaycastHit2D hit = Physics2D.Linecast(from, to, layer_mask);
            Debug.DrawLine(from, (hit) ? to : to);
            return hit;
        }
    }
}