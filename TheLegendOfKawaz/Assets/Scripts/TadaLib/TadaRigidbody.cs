using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

/// <summary>
/// 2D用の自作Rigidbody
/// 壁，地面の埋め込みを防いだり，
/// 坂道を歩くこと，すり抜ける床，移動する床
/// に対応できる
/// </summary>

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
        private float MaxClimbDegree = 90f;

        // 左でぶつかっている
        public bool LeftCollide { private set; get; }
        // 右でぶつかっている
        public bool RightCollide { private set; get; }
        // 上でぶつかっている
        public bool TopCollide { private set; get; }
        // 下でぶつかっている
        public bool ButtomCollide { private set; get; }

        // 地面の摩擦係数
        public float GroundFriction { private set; get; }

        // 水中の中にいるかどうか
        public bool IsUnderWater { private set; get; }

        private BoxCollider2D hit_box_;

        private const float kEpsilon = 0.001f;

        // 現在載っている移動する床 一つだけ持つ
        private Mover riding_mover_;

        private void Start()
        {
            // 衝突の初期化
            LeftCollide = false;
            RightCollide = false;
            TopCollide = false;
            ButtomCollide = false;

            GroundFriction = 1.0f;

            hit_box_ = GetComponent<BoxCollider2D>();

            riding_mover_ = null;
        }

        private void LateUpdate()
        {
            Move();

            // 水中にいるか
            IsUnderWater = hit_box_.IsTouchingLayers(1 << 14);
        }

        private void Move()
        {
            // 衝突の初期化
            LeftCollide = false;
            RightCollide = false;
            TopCollide = false;
            ButtomCollide = false;

            Vector2 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            scale.y = Mathf.Abs(scale.y);

            // 当たり判定(矩形)のサイズと中心
            Vector2 offset = hit_box_.offset * scale;
            Vector2 half_size = hit_box_.size * scale * 0.5f;

            // 移動量
            Vector2 d = InitSpeed * Velocity * Time.deltaTime * 60f;

            // 名前思いつかなかった・・・ すり抜ける床を含めないのはmask_1
            bool through = IsThrough || d.y > 0f; // 床をすり抜けるかどうか 上から来た場合はfalse 下から来た場合はtrue
            int mask_0 = (through) ? (1 << 8 | 1 << 10) : (1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);
            int mask_1 = 1 << 8 | 1 << 10;

            // 移動する床の移動量
            if (riding_mover_)
            {
                d += riding_mover_.Diff;
                //transform.position += (Vector3)riding_mover_.Diff;
                //Debug.Log(riding_mover_.Diff.x + " , " + riding_mover_.Diff.y + " 追加");
            }
            // レイキャストを飛ばす中心
            Vector2 origin = (Vector2)transform.position + offset;
            // レイの長さ
            float length_x = half_size.x / 2f;
            // y軸は少し余分に取る
            float length_y = half_size.y + half_size.x * 1.5f;

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
                float tmp_d_y = d.y - kEpsilon;
                bool hit = false;
                RaycastHit2D most_top_hit = hit_down_left;
                //if (hit_down_left && tmp_d_y <= length - hit_down_left.distance) { tmp_d_y = length - hit_down_left.distance; hit = true; }
                //if (hit_down_center && tmp_d_y <= length - hit_down_center.distance) { tmp_d_y = length - hit_down_center.distance; most_top_hit = hit_down_center; hit = true; }
                //if (hit_down_right && tmp_d_y <= length - hit_down_right.distance) { tmp_d_y = length - hit_down_right.distance; most_top_hit = hit_down_right; hit = true; }

                hit |= CheckButtonCollide(ref tmp_d_y, ref most_top_hit, hit_down_left, length, through);
                hit |= CheckButtonCollide(ref tmp_d_y, ref most_top_hit, hit_down_center, length, through);
                hit |= CheckButtonCollide(ref tmp_d_y, ref most_top_hit, hit_down_right, length, through);

                d.y = Mathf.Max(d.y, tmp_d_y);
                ButtomCollide = hit;

                // 移動する床に載っているか確かめる
                if (ButtomCollide && (most_top_hit.collider.gameObject.layer == 10 || most_top_hit.collider.gameObject.layer == 11))
                    riding_mover_ = most_top_hit.collider.gameObject.GetComponent<Mover>();
                else riding_mover_ = null;

                // 床の摩擦係数を取得する
                if (ButtomCollide)
                {
                    var material = most_top_hit.collider.gameObject.GetComponent<Road.RoadMaterial2D>();
                    if (material) GroundFriction = material.Friction;
                    else GroundFriction = 1.0f;
                }

                // ヒットしているなら，それぞれの法線ベクトルを取得 坂道対応
                {
                    if (hit_down_left && d.x < kEpsilon)
                    {
                        float theta = Mathf.Atan2(hit_down_left.normal.y, hit_down_left.normal.x) - Mathf.PI / 2f;

                        if (Mathf.Abs(theta) < MaxClimbDegree / 90f)
                        {
                            float friction_power = (1f - GroundFriction) * 0.2f;
                            float rate = 1f - Mathf.Sign(d.x) * Mathf.Sin(theta) * 0.75f;

                            //Velocity.x -= Mathf.Sin(theta) * friction_power * Mathf.Abs(1f - rate) / 60f;
                            Velocity.x -= Mathf.Sin(theta) * friction_power / 60f;

                            if (d.x * Mathf.Sin(theta) < 0) // 坂道下り坂
                            {
                                d.y += d.x * Mathf.Sin(theta) * rate;
                                d.x *= Mathf.Cos(theta) * rate;
                            }
                            else
                            {
                                d.y += d.x * Mathf.Sin(theta);
                                d.x *= Mathf.Cos(theta);
                            }
                        }
                    }
                    else if (hit_down_right && d.x > kEpsilon)
                    {
                        float theta = Mathf.Atan2(hit_down_right.normal.y, hit_down_right.normal.x) - Mathf.PI / 2f;

                        if (Mathf.Abs(theta) < MaxClimbDegree / 90f)
                        {
                            float friction_power = (1f - GroundFriction) * 0.2f;
                            float rate = 1f - Mathf.Sign(d.x) * Mathf.Sin(theta) * 0.75f;

                            //Velocity.x -= Mathf.Sin(theta) * friction_power * Mathf.Abs(1f - rate) / 60f;
                            Velocity.x -= Mathf.Sin(theta) * friction_power / 60f;

                            if (d.x * Mathf.Sin(theta) < 0) // 坂道下り坂
                            {
                                d.y += d.x * Mathf.Sin(theta) * rate;
                                d.x *= Mathf.Cos(theta) * rate;
                            }
                            else
                            {
                                d.y += d.x * Mathf.Sin(theta);
                                d.x *= Mathf.Cos(theta);
                            }
                        }
                    }
                    else if (hit_down_center)
                    {
                        float theta = Mathf.Atan2(hit_down_center.normal.y, hit_down_center.normal.x) - Mathf.PI / 2f;

                        if (Mathf.Abs(theta) < MaxClimbDegree / 90f)
                        {
                            float friction_power = (1f - GroundFriction) * 0.2f;
                            float rate = 1f - Mathf.Sign(d.x) * Mathf.Sin(theta) * 0.75f;

                            //Velocity.x -= Mathf.Sin(theta) * friction_power * Mathf.Abs(1f - rate) / 60f;
                            Velocity.x -= Mathf.Sin(theta) * friction_power / 60f;

                            if (d.x * Mathf.Sin(theta) < 0) // 坂道下り坂
                            {
                                d.y += d.x * Mathf.Sin(theta) * rate;
                                d.x *= Mathf.Cos(theta) * rate;
                            }
                            else
                            {
                                d.y += d.x * Mathf.Sin(theta);
                                d.x *= Mathf.Cos(theta);
                            }
                        }
                    }
                }
            }
            // 次に上方向 下向き方向にヒットしたならば，上向き方向はやらない
            if (!ButtomCollide)
            {
                // 左端，中央，右端の順に確かめる
                Vector2 origin_left = origin + new Vector2(-half_size.x * 0.8f, 0f);
                Vector2 origin_right = origin + new Vector2(half_size.x * 0.8f, 0f);
                RaycastHit2D hit_up_left = LinecastWithGizmos(origin_left, origin_left + new Vector2(0f, length_y + d.y), mask_1);
                RaycastHit2D hit_up_center = LinecastWithGizmos(origin, origin + new Vector2(0f, length_y + d.y), mask_1);
                RaycastHit2D hit_up_right = LinecastWithGizmos(origin_right, origin_right + new Vector2(0f, length_y + d.y), mask_1);


                // めり込んでいる分は下に下げる
                float len = half_size.y; // 地面までの通常の距離
                float tmp_d_y = 100f;
                if (hit_up_left) tmp_d_y = Mathf.Min(tmp_d_y, hit_up_left.distance - len);
                if (hit_up_center) tmp_d_y = Mathf.Min(tmp_d_y, hit_up_center.distance - len);
                if (hit_up_right) tmp_d_y = Mathf.Min(tmp_d_y, hit_up_right.distance - len);
                d.y = Mathf.Min(d.y, tmp_d_y);
                TopCollide |= (tmp_d_y <= (d.y + kEpsilon));

                // ヒットしているなら，それぞれの法線ベクトルを取得 坂崖対応
                // 移動の関心を崖の角度方向にさせる
                {
                    if (hit_up_left && d.x < kEpsilon)
                    {
                        float theta = Mathf.Atan2(hit_up_left.normal.y, hit_up_left.normal.x) + Mathf.PI / 2f;
                        d.y += Mathf.Abs(d.x) * Mathf.Sin(theta);
                        d.x *= Mathf.Cos(theta);
                    }
                    else if (hit_up_right && d.x > kEpsilon)
                    {
                        float theta = Mathf.Atan2(hit_up_right.normal.y, hit_up_right.normal.x) + Mathf.PI / 2f;
                        d.y += Mathf.Abs(d.x) * Mathf.Sin(theta);
                        d.x *= Mathf.Cos(theta);
                    }
                    else if (hit_up_center)
                    {
                        float theta = Mathf.Atan2(hit_up_center.normal.y, hit_up_center.normal.x) + Mathf.PI / 2f;
                        d.y += Mathf.Abs(d.x) * Mathf.Sin(theta);
                        d.x *= Mathf.Cos(theta);
                    }
                }
            }

            // y軸移動を考慮する
            origin.y += d.y;

            // 次に左右
            {
                // まずはx軸方向
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

                {
                    RaycastHit2D hit_right = Physics2D.BoxCast(origin, new Vector2(half_size.x, half_size.y * 0.6f), 0f, Vector2.right,
                        d.x + half_size.x / 2f, mask_1);
                    Debug.DrawLine(origin, origin + new Vector2(half_size.x + d.x, 0f), Color.blue);
                    if (hit_right)
                    {
                        d.x = hit_right.distance - half_size.x / 2f;
                    }

                    if (hit_right) RightCollide = true;
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

        // 下向き方向に飛ばしたレイの結果を反映する 接地しているときはtrueを返す
        private bool CheckButtonCollide(ref float new_d_y, ref RaycastHit2D most_top_ray, RaycastHit2D ray, float length, bool through)
        {
            if (!ray) return false;
            if (new_d_y > length - ray.distance) return false;

            if (!through) // すり抜ける床の可能性があるときはカウントしないときもある
            {
                int layer = ray.collider.gameObject.layer;
                // 対象のオブジェクトより下にいるときはカウントしない
                // 移動しないすり抜ける床の時
                if (layer == 9 && transform.position.y - length < ray.point.y - kEpsilon) return false;
                if(layer == 11) // 移動するすり抜ける床の時
                {
                    float added_y = ray.collider.gameObject.GetComponent<Mover>().Diff.y;
                    if (transform.position.y - length + added_y < ray.point.y - kEpsilon) return false;
                }
            }

            // 更新する
            new_d_y = length - ray.distance;
            most_top_ray = ray;

            return true;
        }
    }
}