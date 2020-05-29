using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace KoitanLib
{
    public class MovieManager : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI upperTextMesh;
        [SerializeField]
        TextMeshProUGUI bottomTextMesh;
        [SerializeField]
        Image upperCurtain;
        [SerializeField]
        Image bottomCurtain;
        [SerializeField]
        MsElem[] messages;

        [SerializeField]
        string nextScene;

        Sequence seq;
        int msIndex = 0;
        bool isLoaded = false;

        private void Start()
        {
            upperTextMesh.color = new Color(1, 1, 1, 0);
            bottomTextMesh.maxVisibleCharacters = 0;
            upperCurtain.rectTransform.sizeDelta = new Vector2(1920, 0);
            bottomCurtain.rectTransform.sizeDelta = new Vector2(1920, 0);
            seq = DOTween.Sequence()
                .Append(upperCurtain.rectTransform.DOSizeDelta(new Vector2(1920, 240), 0.5f))
                .Join(bottomCurtain.rectTransform.DOSizeDelta(new Vector2(1920, 240), 0.5f))
                .Append(upperTextMesh.DOFade(1, 0.5f))
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    ShowMessage();
                });
        }

        private void Update()
        {
            if (ActionInput.GetButtonDown(ActionCode.Dash) && !isLoaded)
            {
                seq.Kill();
                LoadScene();
            }
        }

        private void ShowMessage()
        {
            seq = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    bottomTextMesh.maxVisibleCharacters = 0;
                    bottomTextMesh.color = new Color(1, 1, 1, 1);
                    bottomTextMesh.text = messages[msIndex].bodyText;
                    //Debug.Log(messages[msIndex].bodyText);
                    /*
                    tweener = DOTween.To(
                        () => bottomTextMesh.maxVisibleCharacters,          // 何を対象にするのか
                        num => bottomTextMesh.maxVisibleCharacters = num,   // 値の更新
                        100,                  // 最終的な値
                        10f                  // アニメーション時間
                    );
                    */
                })
                //.AppendInterval(messages[msIndex].waitTime)
                .Append(DOTween.To(
                        () => bottomTextMesh.maxVisibleCharacters,          // 何を対象にするのか
                        num => bottomTextMesh.maxVisibleCharacters = num,   // 値の更新
                        messages[msIndex].bodyText.Length,           // 最終的な値
                        messages[msIndex].bodyText.Length / 15f                // アニメーション時間
                ).SetEase(Ease.Linear))
                .AppendInterval(messages[msIndex].waitTime)
                .Append(bottomTextMesh.DOFade(0, 0.5f))
                .AppendCallback(() =>
                {
                    msIndex++;
                    if (msIndex < messages.Length)
                    {
                        ShowMessage();
                    }
                    else
                    {
                        LoadScene();
                    }
                });
        }

        public void LoadScene()
        {
            if(!isLoaded)
            {
                isLoaded = true;
                FadeManager.FadeIn(1f, nextScene);
            }
        }

        [System.Serializable]
        public class MsElem
        {
            public float waitTime = 1;
            [Multiline(2)]
            public string bodyText = "ここにメッセージ\n二行目";
        }
    }
}
