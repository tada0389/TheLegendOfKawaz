using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TadaScene;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Test
{
    public class CRTController : MonoBehaviour
    {
        [SerializeField]
        private CRT crt_;

        [SerializeField]
        private AudioSource audio_;

        private IEnumerator Start()
        {

            yield return new WaitForSeconds(10.0f);

            DOTween.To(
                () => crt_.noiseX, 
                num => crt_.noiseX = num,
                0.04f, 
                1.0f);

            yield return new WaitForSeconds(10.0f);

            DOTween.To(
               () => crt_.sinNoiseScale,
               num => crt_.sinNoiseScale = num,
               0.6f,
               4.0f).SetEase(Ease.InQuart);

            DOTween.To(
              () => crt_.sinNoiseWidth,
              num => crt_.sinNoiseWidth = num,
              8.0f,
              4.0f).SetEase(Ease.InQuart);

            yield return new WaitForSeconds(8.0f);

            DOTween.To(
              () => crt_.noiseX,
              num => crt_.noiseX = num,
              0.5f,
              3.0f);

            DOTween.To(
              () => audio_.volume,
              num => audio_.volume = num,
              audio_.volume / 5f,
              3.0f);

            yield return new WaitForSeconds(5.0f);

            DontDestroyOnLoad(this);
            TadaSceneManager.LoadScene("ZakkyScene", 0.5f, Vector3.zero, true);

            yield return new WaitForSeconds(0.6f);
            AchievementManager.FireAchievement("Hidden");

            TadaLib.Save.SaveManager.Instance.Save();
            Destroy(this);
        }
    }
}