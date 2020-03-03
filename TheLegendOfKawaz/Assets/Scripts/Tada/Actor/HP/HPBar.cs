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
        private float init_size_ = 32f;

        public void ChangeValue(int hp)
        {
            target_.sizeDelta = new Vector2(target_.sizeDelta.x, Mathf.Min(640f, hp * init_size_));
        }
    }
}