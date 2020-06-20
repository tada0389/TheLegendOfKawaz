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

        private void Awake()
        {
            animator_ = GetComponent<Animator>();
            StartCoroutine(Tenmetu());
        }

        public void PlayAnim(string anim, int type)
        {
            //Debug.Log(anim);
            //Debug.Log(type);
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

        public void Shot(int shot_type)
        {

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