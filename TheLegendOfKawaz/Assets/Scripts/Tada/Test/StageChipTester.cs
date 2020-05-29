using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Test
{
    public class StageChipTester : MonoBehaviour
    {
        [SerializeField]
        private float period_ = 3.0f;

        [SerializeField]
        private float alpha_value_ = 0.6f;

        [SerializeField]
        private int order_ = 0;
        [SerializeField]
        private int sum_ = 28;

        private SpriteRenderer renderer_;

        // private float tmp_each_duration_;

        // Start is called before the first frame update
        void Start()
        {
            renderer_ = GetComponent<SpriteRenderer>();

            StartCoroutine(Flow());
        }



        private IEnumerator Flow()
        {
            renderer_.color = new Color(0f, 1f, 0f, alpha_value_);

            yield return new WaitForSeconds(period_ * order_ / sum_);

            float each_duration = period_ / 6f;

            float prev_time = Time.time - each_duration;

            float new_duration;

            while (true)
            {
                new_duration = 2f * each_duration - (Time.time - prev_time);
                prev_time = Time.time;
                renderer_.DOKill();
                renderer_.DOColor(new Color(1f, 1f, 0f, alpha_value_), new_duration);

                yield return new WaitForSeconds(new_duration);

                new_duration = 2f * each_duration - (Time.time - prev_time);
                prev_time = Time.time;
                renderer_.DOKill();
                renderer_.DOColor(new Color(1f, 0f, 0f, alpha_value_), new_duration);

                yield return new WaitForSeconds(new_duration);

                new_duration = 2f * each_duration - (Time.time - prev_time);
                prev_time = Time.time;
                renderer_.DOKill();
                renderer_.DOColor(new Color(1f, 0f, 1f, alpha_value_), new_duration);

                yield return new WaitForSeconds(new_duration);

                new_duration = 2f * each_duration - (Time.time - prev_time);
                prev_time = Time.time;
                renderer_.DOKill();
                renderer_.DOColor(new Color(0f, 0f, 1f, alpha_value_), new_duration);

                yield return new WaitForSeconds(new_duration);

                new_duration = 2f * each_duration - (Time.time - prev_time);
                prev_time = Time.time;
                renderer_.DOKill();
                renderer_.DOColor(new Color(0f, 1f, 1f, alpha_value_), new_duration);

                yield return new WaitForSeconds(new_duration);

                new_duration = 2f * each_duration - (Time.time - prev_time);
                prev_time = Time.time;
                renderer_.DOKill();
                renderer_.DOColor(new Color(0f, 1f, 0f, alpha_value_), new_duration);

                yield return new WaitForSeconds(new_duration);
            }
        }
    }
}