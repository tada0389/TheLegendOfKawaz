using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actor.Enemy.Purin;
using TadaLib;

/// <summary>
/// プリンボスの死亡時のステート
/// </summary>

namespace Actor.Enemy.Purin
{
    partial class PurinBossController
    {
        // 死亡したときの状態
        [System.Serializable]
        private class DeadState : StateMachine<PurinBossController>.StateBase
        {
            // 死亡時の爆発のエフェクト
            [SerializeField]
            private ParticleSystem explosion_effect_;

            private bool get = false;

            // ステートの初期化
            public override void OnInit()
            {
                
            }

            // 開始時に呼ばれる
            public override void OnStart()
            {
                TadaLib.TimeScaler.Instance.RequestChange(0.3f, 2.0f);
                explosion_effect_.gameObject.SetActive(true);
                Global.GlobalPlayerInfo.IsMuteki = true;
            }

            // 毎フレーム呼ばれる
            public override void Proc()
            {
                if (Timer > 2.0f && !get)
                {
                    get = true;
                    Actor.Player.SkillManager.Instance.GainSkillPoint(500, Parent.transform.position, 0.8f);
                    //実績解除
                    AchievementManager.FireAchievement("Purin");
                    if (Parent.player_.GetComponent<Actor.Player.Player>().IsNoDamage())
                        AchievementManager.FireAchievement("Purin_nodamage");
                }

                if(Timer > 7.0f)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("ZakkyScene");
                }
            }

            // 終了時に呼ばれる
            public override void OnEnd()
            {

            }
        }
    }
} // namespace Actor.Enemy.Purin