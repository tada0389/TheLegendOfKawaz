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

        private void Awake()
        {
            animator_ = GetComponent<Animator>();
        }

        public void PlayAnim(string anim, int type)
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

        public void Shot(int shot_type)
        {

        }
    }
}