using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Purin;
using TadaLib;
using Bullet;

/// <summary>
/// プリンボスの射撃ステート
/// </summary>

namespace Actor.Enemy.Purin
{
    partial class PurinBossController
    {
        // 射撃ステート
        [System.Serializable]
        private class ShotState : StateMachine<PurinBossController>.StateBase
        {
            // 弾
            [SerializeField]
            private BaseBulletController bullet_;

            [SerializeField]
            private int shot_num_ = 3;

            [SerializeField]
            private float shot_delay_ = 0.5f;

            [SerializeField]
            private float shot_dir_y_ = 0.3f;

            // 硬直時間
            [SerializeField]
            private float rigity_time_ = 1.5f;

            // ステートの初期化
            public override void OnInit()
            {
                Parent.bullet_spawner_.RegisterBullet(bullet_, 6);
            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.StartCoroutine(ShotCoroutine());
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }

            // 弾を打つコルーチン バグのもとになるかなぁ
            private IEnumerator ShotCoroutine()
            {
                for (int i = 0; i < shot_num_; ++i)
                {
                    yield return new WaitForSeconds(shot_delay_);

                    // 敵の方向をむく
                    float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                    Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);

                    Parent.bullet_spawner_.Shot(bullet_, Parent.transform.position, new Vector2(dir, shot_dir_y_).normalized, "Player");
                }

                yield return new WaitForSeconds(rigity_time_);

                if (state_machine_.CurrentStateId != (int)eState.Dead)
                    ChangeState((int)eState.Think);
            }
        }
    }
} // namespace Actor.Enemy.Purin