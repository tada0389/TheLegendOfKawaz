using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 千手観音が最初にダメージを受けるときのエフェクトたち
/// </summary>

namespace Actor.Enemy.Thousand
{
    public class InitialThousandEffect : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem purin_effect_;

        [SerializeField]
        private ParticleSystem venom_effect_;

        [SerializeField]
        private ParticleSystem beta_effect_;

        [SerializeField]
        private float effect_interval_ = 0.2f;

        private IEnumerator Start()
        {
            purin_effect_.gameObject.SetActive(false);
            venom_effect_.gameObject.SetActive(false);
            beta_effect_.gameObject.SetActive(false);

            // 少し待つ
            yield return null;
            yield return null;

            yield return new WaitForSeconds(0.2f * Time.timeScale);

            if (Global.GlobalDataManager.EachBossDefeatCnt(Global.eBossType.Purin) >= 1)
            {
                purin_effect_.gameObject.SetActive(true);
                purin_effect_.Play();
                yield return new WaitForSeconds(effect_interval_ * Time.timeScale);
            }
            if (Global.GlobalDataManager.EachBossDefeatCnt(Global.eBossType.VernmDrake) >= 1)
            {
                venom_effect_.gameObject.SetActive(true);
                venom_effect_.Play();
                yield return new WaitForSeconds(effect_interval_ * Time.timeScale);
            }
            if (Global.GlobalDataManager.EachBossDefeatCnt(Global.eBossType.KawazTanBeta) >= 1)
            {
                beta_effect_.gameObject.SetActive(true);
                beta_effect_.Play();
                yield return new WaitForSeconds(effect_interval_ * Time.timeScale);
            }
            if (!purin_effect_.gameObject.activeSelf) Destroy(purin_effect_.gameObject);
            if (!venom_effect_.gameObject.activeSelf) Destroy(venom_effect_.gameObject);
            if (!beta_effect_.gameObject.activeSelf) Destroy(beta_effect_.gameObject);
        }
    }
}