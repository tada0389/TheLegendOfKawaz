using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private float max_hp_;

        public void Init(int init_hp) {
            max_hp_ = init_hp * init_size_;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, default_height + init_hp * init_size_);
            ChangeValue(init_hp);
        }

        public void ChangeValue(int hp)
        {
            target_.sizeDelta = new Vector2(target_.sizeDelta.x, hp * init_size_); // Mathf.Min(max_hp_, hp * init_size_));
        }
    }
}