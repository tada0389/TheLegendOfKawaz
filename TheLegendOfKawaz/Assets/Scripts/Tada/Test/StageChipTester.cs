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

        private SpriteRenderer renderer_;

        // Start is called before the first frame update
        void Start()
        {
            renderer_ = GetComponent<SpriteRenderer>();

            StartCoroutine(Flow());
        }



        private IEnumerator Flow()
        {
            renderer_.color = new Color(0f, 1f, 0f, alpha_value_);

            while (true)
            {
                renderer_.DOKill();
                renderer_.DOColor(new Color(1f, 1f, 0f, alpha_value_), period_ / 6f);

                yield return new WaitForSeconds(period_ / 6f);

                renderer_.DOKill();
                renderer_.DOColor(new Color(1f, 0f, 0f, alpha_value_), period_ / 6f);

                yield return new WaitForSeconds(period_ / 6f);

                renderer_.DOKill();
                renderer_.DOColor(new Color(1f, 0f, 1f, alpha_value_), period_ / 6f);

                yield return new WaitForSeconds(period_ / 6f);

                renderer_.DOKill();
                renderer_.DOColor(new Color(0f, 0f, 1f, alpha_value_), period_ / 6f);

                yield return new WaitForSeconds(period_ / 6f);

                renderer_.DOKill();
                renderer_.DOColor(new Color(0f, 1f, 1f, alpha_value_), period_ / 6f);

                yield return new WaitForSeconds(period_ / 6f);

                renderer_.DOKill();
                renderer_.DOColor(new Color(0f, 1f, 0f, alpha_value_), period_ / 6f);

                yield return new WaitForSeconds(period_ / 6f);
            }
        }
    }
}