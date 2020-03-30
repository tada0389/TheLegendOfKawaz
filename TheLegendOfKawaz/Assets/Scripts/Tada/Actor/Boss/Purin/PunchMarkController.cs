using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Actor.Enemy.Purin
{
    public class PunchMarkController : MonoBehaviour
    {
        [SerializeField]
        private float scale_from_ = 2.0f;

        [SerializeField]
        private float scale_to_ = 0.4f;

        [SerializeField]
        private float scale_change_duration_ = 0.5f;

        [SerializeField]
        private Ease ease_;

        [SerializeField]
        private float tenmetu_interval_ = 0.05f;

        [SerializeField]
        private GameObject hit_box_;
        [SerializeField]
        private int damage_ = 3;

        private TadaLib.Timer timer_;

        private bool is_tenmetu_ = false;
        private SpriteRenderer renderer_;
        private int cnt_ = 0;

        private Transform player_;

        private bool lock_ = false;

        private void Start()
        {
            timer_ = new TadaLib.Timer(tenmetu_interval_);
            renderer_ = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (player_ && !lock_)
            {
                transform.position = player_.position;
            }
            if (is_tenmetu_)
            {
                if (timer_.IsTimeout())
                {
                    timer_.TimeReset();
                    if (cnt_ % 2 == 0)
                        renderer_.color = new Color(1f, 1f, 1f, 0f);
                    else
                        renderer_.color = new Color(1f, 1f, 1f, 1f);
                    ++cnt_;
                }
            }
        }

        public void Init(Transform player)
        {
            player_ = player;
            is_tenmetu_ = false;
            lock_ = false;
            transform.DOKill();
            transform.localScale = Vector3.one * scale_from_;
            transform.DOScale(scale_to_, scale_change_duration_).SetEase(ease_);
            transform.localEulerAngles = Vector3.zero;
            transform.DORotate(new Vector3(0f, 0f, 360f), scale_change_duration_, RotateMode.FastBeyond360).SetEase(ease_);

            hit_box_.gameObject.SetActive(false);
        }

        public void TenmetuStart()
        {
            is_tenmetu_ = true;
        }

        public void TenmetsuEnd()
        {
            transform.DOScale(scale_to_ * 1.5f, scale_change_duration_ / 2f).SetEase(ease_).OnComplete(() => 
            { renderer_.color = new Color(1f, 1f, 1f, 1f); gameObject.SetActive(false); });
        }

        // 攻撃する場所を固定する
        public void LockPosition()
        {
            lock_ = true;
        }

        // 実際に攻撃する 本当は別でやったほうがいい
        public void Punch()
        {
            hit_box_.gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.tag == "Player")
            {
                collider.GetComponent<BaseActorController>().Damage(damage_);
            }
        }
    }
}