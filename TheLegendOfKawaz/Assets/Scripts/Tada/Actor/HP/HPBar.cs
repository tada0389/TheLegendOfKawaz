using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Actor
{

    public class HPBar : MonoBehaviour
    {
        [SerializeField]
        private RectTransform target_;

        [SerializeField]
        private float default_height = 12f;

        [SerializeField]
        private float init_size_ = 32f;

        [SerializeField]
        private float change_duration_ = 0.075f;

        // HPバーが赤く点滅する閾値
        [SerializeField]
        private float red_heat_thr_ = 0.26f;

        private Image image_;

        // 前回のHP
        private int prev_hp_;
        private int max_hp_;

        // ターゲットのHP
        private int target_hp_;

        private bool damage_running_;
        private bool redheat_running_;

        public void Init(int init_hp) {
            RectTransform rectTransform = GetComponent<RectTransform>();
            image_ = target_.GetComponent<Image>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, default_height + init_hp * init_size_);
            prev_hp_ = init_hp;
            target_hp_ = prev_hp_;
            max_hp_ = init_hp;
            target_.sizeDelta = new Vector2(target_.sizeDelta.x, prev_hp_ * init_size_);
            damage_running_ = false;
            redheat_running_ = false;
            ChangeValue(init_hp);
        }

        public void ChangeValue(int hp)
        {
            target_hp_ = hp;
            if(!damage_running_) StartCoroutine(ChangeValue());
            if (target_hp_ <= max_hp_ * red_heat_thr_) DoRedHeat();
            else if(redheat_running_) CancelRedHeat();
        }

        private IEnumerator ChangeValue()
        {
            yield return new WaitForSeconds(0.05f);

            damage_running_ = true;
            while(prev_hp_ != target_hp_)
            {
                if (prev_hp_ > target_hp_) --prev_hp_;
                else ++prev_hp_;

                target_.sizeDelta = new Vector2(target_.sizeDelta.x, prev_hp_ * init_size_); // Mathf.Min(max_hp_, hp * init_size_));

                yield return new WaitForSeconds(change_duration_ * Time.timeScale);
            }
            damage_running_ = false;
        }

        private void DoRedHeat()
        {
            if (redheat_running_) return;
            // 赤く点滅させる
            image_.DOColor(Color.red, 0.5f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
            redheat_running_ = true;
        }

        private void CancelRedHeat()
        {
            image_.DOKill();
            image_.color = Color.white;
            redheat_running_ = false;
        }
    }
}