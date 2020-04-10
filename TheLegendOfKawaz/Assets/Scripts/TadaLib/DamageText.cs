using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace TadaLib
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DamageText : MonoBehaviour
    {
        private Camera cam_;

        private TadaLib.Timer timer_;

        private TextMeshProUGUI text_;

        [SerializeField]
        private Ease ease = Ease.OutQuart;

        [SerializeField]
        private float move_duration_ = 0.25f;

        [SerializeField]
        private float up_dist_ = 100.0f;

        private float duration_;

        private bool damaged_ = false;

        public void Display(int damage, int size, float duration, Vector3 pos, Canvas canvas)
        {
            text_ = GetComponent<TextMeshProUGUI>();
            cam_ = Camera.main;
            text_.rectTransform.SetParent(canvas.transform);
            duration_ = duration;

            text_.text = damage.ToString();
            text_.fontSize = size;
            timer_ = new TadaLib.Timer(duration);
            gameObject.SetActive(true);

            Vector3 new_pos = cam_.WorldToScreenPoint(pos);
            text_.rectTransform.position = new_pos;
            text_.rectTransform.DOMoveY(new_pos.y + up_dist_, move_duration_).SetEase(ease);
            text_.color = new Color(text_.color.r, text_.color.g, text_.color.b, 0.0f);
            text_.DOFade(1.0f, move_duration_).SetEase(ease);

            damaged_ = true;
        }

        private void Update()
        {
            if (!damaged_) return;
            if(timer_.GetTime() > duration_ / 2f)
            {
                text_.DOFade(0.0f, move_duration_).SetEase(ease);
                duration_ = 1000000000f;
            }

            if (timer_.IsTimeout())
            {
                damaged_ = true;
                gameObject.SetActive(false);
            }
        }
    }
}