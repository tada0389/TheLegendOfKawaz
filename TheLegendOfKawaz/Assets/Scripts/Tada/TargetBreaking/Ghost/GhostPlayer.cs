using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴーストとなる本体
/// </summary>

namespace TargetBreaking
{
    [RequireComponent(typeof(Animator))]
    public class GhostPlayer : MonoBehaviour
    {
        private Animator animator_;

        [SerializeField]
        private GameObject body_;

        [SerializeField]
        private bool IsTenmetsu = true;

        private TadaLib.Timer shot_timer_;

        private void Awake()
        {
            animator_ = GetComponent<Animator>();
            shot_timer_ = new TadaLib.Timer(0.3f);
            if(IsTenmetsu) StartCoroutine(Tenmetu());
        }

        private void FixedUpdate()
        {
            // ショット後のアニメーション変更
            if (animator_.GetLayerWeight(1) == 1 && shot_timer_.IsTimeout()) animator_.SetLayerWeight(1, 0);
        }

        public void PlayAnim(string anim, int type)
        {
            //Debug.Log(anim);
            //Debug.Log(type);

            // ショットだけ特別
            if (anim == "ChargeShot" || anim == "Shot")
            {
                if (animator_.GetLayerWeight(1) == 0)
                {
                    shot_timer_.TimeReset();
                    animator_.SetLayerWeight(1, 1);
                }
                else
                {
                    animator_.SetLayerWeight(1, 0);
                }
                animator_.Play(anim, 1, 0);
            }
            else
            {
                switch (type)
                {
                    case 0: // Start
                        animator_.Play(anim);
                        break;
                    case 1: // SetBoolTrue
                        animator_.SetBool(anim, true);
                        break;
                    case 2:
                        animator_.SetBool(anim, false);
                        break; // SetBoolFalse
                    case 3:
                        animator_.Play(anim, 0, 0.0f);
                        break; // Restart
                }
            }
        }

        //点滅
        private IEnumerator Tenmetu()
        {
            while (true)
            {
                body_.SetActive(false);
                yield return new WaitForSeconds(0.01f);
                body_.SetActive(true);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}