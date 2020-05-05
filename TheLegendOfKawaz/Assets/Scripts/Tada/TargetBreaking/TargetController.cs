using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor;
using DG.Tweening;

namespace Target
{
    public class TargetController : BaseActorController
    {
        [SerializeField]
        private TargetManager manager_;

        // 移動情報
        [SerializeField]
        private float duration_ = 3.0f;

        [SerializeField]
        private Vector3 move_distance_;

        [SerializeField]
        private Ease ease_ = Ease.Linear;

        private Vector3 to_ = Vector3.zero;

        [SerializeField]
        private bool isPath_ = false;

        [SerializeField]
        private LoopType loop_type_ = LoopType.Incremental;

        [SerializeField]
        private Vector3[] path_;

        // Start is called before the first frame update
        void Start()
        {
            manager_.RegisterTarget();

            if (!isPath_)
            {
                to_ = transform.position + move_distance_;
                transform.DOMove(to_, duration_).SetLoops(-1, LoopType.Yoyo).SetEase(ease_);
            }
            else
            {
                Vector3[] path = new Vector3[path_.Length + 1];
                //for (int i = 0; i < path_.Length; ++i) path[i] = path_[i];
                path_.CopyTo(path, 0);
                path[path_.Length] = transform.position;
                transform.DOPath(path, duration_, PathType.CatmullRom).SetLoops(-1, loop_type_).SetEase(ease_);
            }
        }

        public override void Damage(int damage)
        {
            manager_.BreakTarget(transform.position);
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            if (!isPath_)
            {
                if (to_ == Vector3.zero)
                    Gizmos.DrawLine(transform.position, transform.position + move_distance_);
                else Gizmos.DrawLine(transform.position, to_);
            }
            else
            {
                Vector3 from = transform.position;
                foreach(var pos in path_)
                {
                    Vector3 to = pos + transform.position;
                    Gizmos.DrawLine(from, to);
                    from = to;
                }
                if (loop_type_ == LoopType.Incremental) Gizmos.DrawLine(from, transform.position);
            }
        }
    }
}