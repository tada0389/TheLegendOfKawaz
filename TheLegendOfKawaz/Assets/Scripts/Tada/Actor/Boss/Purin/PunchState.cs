using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Purin;
using TadaLib;

/// <summary>
/// プリンボスのサクランボでパンチするときののステート
/// </summary>

namespace Actor.Enemy.Purin
{
    partial class PurinBossController
    {
        // ステージ中央に移動するときのステート
        [System.Serializable]
        private class PunchState1 : StateMachine<PurinBossController>.StateBase
        {
            [SerializeField]
            private Vector2 speed_ = new Vector2(0.16f, 0.16f);

            [SerializeField]
            private float period_ = Mathf.PI;

            [SerializeField]
            private float stage_center_x = 2.5f;

            private float dir_;

            // ステートの初期化
            public override void OnInit()
            {

            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                dir_ = Mathf.Sign(stage_center_x - Parent.transform.position.x);
                Parent.SetDirection((dir_ < 0f) ? eDir.Left : eDir.Right);

                Parent.trb_.Velocity = new Vector2(0f, 0f);
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                // 真ん中に移動できた，もしくは壁に衝突したなら次へ
                float dir = Mathf.Sign(stage_center_x - Parent.transform.position.x);
                if (dir * dir_ < 0f || (dir_ < 0f && Parent.trb_.LeftCollide) || (dir_ > 0f && Parent.trb_.RightCollide) || Parent.trb_.TopCollide)
                {
                    ChangeState((int)eState.Punch2);
                    return;
                }

                Parent.trb_.Velocity.y = Mathf.Sin(Mathf.PI * (Timer / period_)) * speed_.y;

                ActorUtils.ProcSpeed(ref Parent.trb_.Velocity, new Vector2(dir_, 0f) * Accel, MaxAbsSpeed);
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.trb_.Velocity = Vector2.zero;
            }
        }

        // サクランボでパンチするときののステート
        [System.Serializable]
        private class PunchState2 : StateMachine<PurinBossController>.StateBase
        {
            [SerializeField]
            private PunchMarkController punch_mark_;

            private List<PunchMarkController> marks_;

            [SerializeField]
            private float punch_interval_ = 3.0f;
            [SerializeField]
            private int punch_num_ = 3;

            [SerializeField]
            private float charge_time_ = 1.0f;
            [SerializeField]
            private Vector2 punch_charge_time_range_ = new Vector2(0.5f, 1f);
            [SerializeField]
            private float punch_tenmetu_time_ = 0.2f;
            [SerializeField]
            private float punch_lock_time_ = 0.2f;

            [SerializeField]
            private Transform ik_;
            [SerializeField]
            private GameObject ik_parent_;

            [SerializeField]
            private float punch_time_scale_ = 0.75f;

            // ステートの初期化
            public override void OnInit()
            {
                KoitanLib.ObjectPoolManager.Init(punch_mark_, Parent, 5);
                marks_ = new List<PunchMarkController>();
            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                Parent.StartCoroutine(PunchFlow());
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {

            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {
                Parent.StopCoroutine(PunchFlow());
            }

            private IEnumerator PunchFlow()
            {
                yield return new WaitForSeconds(charge_time_);

                for(int i = 0; i < punch_num_; ++i)
                {
                    if (state_machine_.CurrentStateId == (int)eState.Dead) break;

                    // マークを出す
                    PunchMarkController mark = KoitanLib.ObjectPoolManager.GetInstance<PunchMarkController>(punch_mark_);

                    if (state_machine_.CurrentStateId != (int)eState.Dead)
                        Parent.animator_.Play("cherry_attack", 0, 0.25f);

                    if (mark != null)
                    {
                        mark.Init(Parent.player_);

                        yield return new WaitForSeconds(Random.Range(punch_charge_time_range_.x, punch_charge_time_range_.y));

                        mark.TenmetuStart();
                        // 時間を少しゆっくりにする
                        TadaLib.TimeScaler.Instance.RequestChange(punch_time_scale_, punch_tenmetu_time_ * 1.2f);

                        yield return new WaitForSeconds(punch_tenmetu_time_);

                        //TadaLib.TimeScaler.Instance.DismissRequest(punch_time_scale_);

                        mark.LockPosition();
                        ik_parent_.SetActive(true);

                        yield return new WaitForSeconds(punch_lock_time_);

                        float dir = Mathf.Sign(Parent.player_.position.x - Parent.transform.position.x);
                        Parent.SetDirection((dir < 0f) ? eDir.Left : eDir.Right);


                        ik_.position = mark.transform.position;

                        mark.Punch();
                        mark.TenmetsuEnd();
                    }

                    yield return new WaitForSeconds(punch_interval_);

                    ik_parent_.SetActive(false);
                    ik_.position = new Vector3(1f, 3f);
                }

                if(state_machine_.CurrentStateId != (int)eState.Dead)
                    ChangeState((int)eState.Think);
            }
        }
    }
} // namespace Actor.Enemy.Purin