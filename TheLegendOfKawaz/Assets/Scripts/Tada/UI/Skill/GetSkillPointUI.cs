using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using TadaLib;

namespace SkillUI
{
    public class GetSkillPointUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas_;
        //[SerializeField]
        private Camera cam_;

        [SerializeField]
        private Image point_icon_;

        [SerializeField]
        private TextMeshProUGUI point_text_;

        // ポイントごとのアイコン数
        [SerializeField]
        private int icon_per_point_ = 10;

        // スキルアイコンの取得時間
        [SerializeField]
        private float icon_get_duration_ = 3.0f;
        // アイコンがはじける距離
        [SerializeField]
        private float icon_appear_power_ = 1f;
        // アイコンがはじける時間
        [SerializeField]
        private float icon_appear_duration = 0.25f;
        // アイコンが移動する時間
        [SerializeField]
        private float icon_move_duration_ = 0.5f;

        // アイコンの移動が終了してからの待ち時間
        [SerializeField]
        private float feed_wait_time_ = 1.0f;
        [SerializeField]
        private float feed_duration_ = 1.0f;

        [SerializeField]
        private Ease icon_appear_ease_ = Ease.InOutQuart;
        [SerializeField]
        private Ease icon_move_ease_ = Ease.InOutQuad;

        private Timer feed_timer_;
        private Timer point_get_timer_;

        // 生成したアイコンのリスト
        private List<Image> icons_;

        private int point_;
        private int added_point_;

        private void Update()
        {
            //// デバッグ
            //if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.B].wasPressedThisFrame)
            //{
            //    GainSkillPoint(500, Vector3.one * 100f);
            //}
            //if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.C].wasPressedThisFrame)
            //{
            //    GainSkillPoint(1000, Vector3.zero);
            //}
            // "し"ょ"う"が"な"い"!
            AddPointToText(added_point_);
            added_point_ = 0;
        }

        private void Start()
        {
            icons_ = new List<Image>();
            feed_timer_ = new Timer(feed_duration_);
            point_get_timer_ = new Timer(icon_get_duration_);

            point_ = Actor.Player.SkillManager.Instance.SkillPoint;
            point_text_.text = point_.ToString();
            added_point_ = 0;
        }

        // ポイントの取得を開始する
        public void GainSkillPoint(int point, Vector3 point_spawner_pos)
        {
            cam_ = Camera.main;
            point_ = Actor.Player.SkillManager.Instance.SkillPoint;
            point_text_.text = point.ToString();
            Vector3 pos = cam_.WorldToScreenPoint(point_spawner_pos);
            StartCoroutine(GetFlow(point, pos));
        }

        // コルーチンで処理
        private IEnumerator GetFlow(int point, Vector3 point_spawner_pos)
        {
            feed_timer_.TimeReset();
            point_get_timer_.TimeReset();

            // UIを出現
            FeedUI(true);

            // いくつのアイコンを生成させるか
            int icon_num = point / icon_per_point_;
            // 一フレームで生成するアイコンの数
            float icon_num_per_sec = icon_num / feed_duration_;

            int icon_cnt = 0;
            // フェードの時間にポイント分のオブジェクトをプーリングしておく
            while (!feed_timer_.IsTimeout())
            {
                int new_icon_num = (int)(feed_timer_.GetTime() * icon_num_per_sec) - icon_cnt;
                PoolingIcon(new_icon_num);
                icon_cnt += new_icon_num;
                yield return null;
            }

            // 足りない分のアイコンをプーリングする
            PoolingIcon(icon_num - icon_cnt);

            // アイコンを出現させる
            // 徐々に出現
            icon_num_per_sec = icon_num / icon_get_duration_;
            icon_cnt = 0;
            // フェードの時間にポイント分のオブジェクトを生成しておく
            while (!point_get_timer_.IsTimeout())
            {
                int new_icon_num = (int)(point_get_timer_.GetTime() * icon_num_per_sec) - icon_cnt;
                CreateIcon(new_icon_num, point_spawner_pos, icon_cnt);
                icon_cnt += new_icon_num;
                yield return null;
            }
            // 足りない分のアイコンを生成する
            CreateIcon(icon_num - icon_cnt, point_spawner_pos, icon_cnt);

            yield return new WaitForSeconds(icon_move_duration_ + feed_wait_time_);

            // UIを消す
            FeedUI(false);

            // 後処理
            icons_.Clear();
        }

        // アイコンをプーリングする
        private void PoolingIcon(int num)
        {
            for (int i = 0; i < num; ++i)
            {
                Image icon = Instantiate(point_icon_, canvas_.transform);
                icon.gameObject.SetActive(false);
                icons_.Add(icon);
            }
        }

        // アイコンを出現させる
        private void CreateIcon(int num, Vector3 point_spawner_pos, int start_index)
        {
            for (int i = 0; i < num; ++i)
            {
                float angle = Random.Range(0f, 2f * Mathf.PI);
                Vector2 target = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * icon_appear_power_;
                Image icon = icons_[start_index + i];
                icon.gameObject.SetActive(true);
                icon.rectTransform.position = point_spawner_pos;
                // 終了のコールバックで再度DOMoveして，その終了のコールバックで破棄するごり押し
                icon.rectTransform.DOMove(icon.rectTransform.position + (Vector3)target, icon_appear_duration).SetEase(icon_appear_ease_).OnComplete(() =>
                {
                    icon.rectTransform.DOMove(point_icon_.rectTransform.position, icon_move_duration_).SetEase(icon_move_ease_).OnComplete(() =>
                    {
                        ++added_point_;
                        Destroy(icon.gameObject);
                    });
                });
            }
        }

        // UIをフェードで出現させる
        private void FeedUI(bool feedin)
        {
            if (feedin)
            {
                point_icon_.DOFade(1.0f, feed_duration_);
                point_text_.DOFade(1.0f, feed_duration_);
            }
            else
            {
                point_icon_.DOFade(0.0f, feed_duration_);
                point_text_.DOFade(0.0f, feed_duration_);
            }
        }

        // ポイントを更新する
        private void AddPointToText(int add_point)
        {
            if (add_point == 0) return;
            point_ += add_point * icon_per_point_;
            point_text_.text = point_.ToString();
        }
    }
}