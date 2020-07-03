using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

namespace KoitanLib
{
    public class LastMovieManager : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI upperTextMesh;
        [SerializeField]
        TextMeshProUGUI bottomTextMesh;
        [SerializeField]
        Image upperCurtain;
        [SerializeField]
        Image bottomCurtain;
        MsElem[] messages;
        [SerializeField]
        AnimElem[] anims;
        Queue<AnimElem> movies = new Queue<AnimElem>();
        Animator animator;

        [SerializeField]
        string nextScene;

        Sequence seq;
        int msIndex = 0;
        bool isLoaded = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
            //必要なムービー要素を読み込む
            //仮
            for (int i = 0; i < anims.Length; i++)
            {
                //条件分岐で含めるか決める
                movies.Enqueue(anims[i]);
            }
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
                    LoadAnim();
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
                })
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
                    else if (movies.Count > 0)
                    {
                        LoadAnim();
                        ShowMessage();
                    }
                    else
                    {
                        LoadScene();
                    }
                });
        }

        private void LoadAnim()
        {
            AnimElem a = movies.Dequeue();
            animator.Play(a.animName);
            messages = a.messages;
            msIndex = 0;
        }

        public void LoadScene()
        {
            if (!isLoaded)
            {
                isLoaded = true;
                FadeManager.ChangeFadeColor(Color.black);
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

        [System.Serializable]
        public class AnimElem
        {
            public string animName;
            public MsElem[] messages;
        }
    }
}
