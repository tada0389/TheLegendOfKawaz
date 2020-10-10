using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GamingComponent : MonoBehaviour
    {
        [SerializeField]
        private float color_speed_ = 1.0f;

        private SpriteRenderer renderer_;

        private float hue_;

        // Start is called before the first frame update
        void Start()
        {
            renderer_ = GetComponent<SpriteRenderer>();
            hue_ = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            renderer_.color = Color.HSVToRGB(hue_, 1.0f, 1.0f);
            hue_ += color_speed_ * Time.deltaTime;
            while (hue_ >= 1.0f) hue_ -= 1.0f;
        }
    }
}