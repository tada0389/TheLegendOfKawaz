using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 風船の動きをするスクリプト
/// </summary>

namespace Stage
{
    public class BalloonMove : MonoBehaviour
    {
        [SerializeField]
        private Vector3 d = new Vector3(0.2f, 0.3f);

        [SerializeField]
        private float period = 0.5f;

        private float time_;
        private Vector3 default_position_;

        // Start is called before the first frame update
        void Start()
        {
            time_ = 0.0f;
            default_position_ = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            transform.localPosition = default_position_ + d * Mathf.Sin(time_ * 2.0f / period);
            time_ += Time.deltaTime;
        }
    }
}