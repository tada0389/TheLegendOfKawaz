using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

/// <summary>
/// エフェクトを生成するグローバルクラス
/// みんな使おう
/// </summary>

namespace TadaLib
{
    public class EffectPlayer : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="effect">どのエフェクトを発生させるか</param>
        /// <param name="pos">発生させる場所</param>
        /// <param name="dir">どの方向に発生させるか</param>
        /// <param name="owner">親となるオブジェクト</param>
        public static void Play(BaseParticle effect, Vector3 pos, Vector3 dir, Transform owner = null)
        {
            var eff = KoitanLib.ObjectPoolManager.GetInstance<BaseParticle>(effect);
            if (eff == null) return;
            eff.transform.position = pos;
            eff.transform.parent = owner;
            eff.transform.localEulerAngles = new Vector3(0f, Mathf.Sign(dir.x) * 90f - 90f, 0f);
            eff.gameObject.SetActive(true);
        }
    }
}