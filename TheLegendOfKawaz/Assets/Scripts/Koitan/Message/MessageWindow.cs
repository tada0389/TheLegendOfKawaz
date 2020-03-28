using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MessageWindow : MonoBehaviour
{
    public Image windowImage;
    public Image narratorImage;
    public TextMeshProUGUI messageTextMesh;
    public string text;
    private AudioSource audioSource;
    public int intervalFrame = 1;
    private int currentFrame = 0;
    public string NarratorStr;
    public Ease ease;
    public float duration;
    public bool isSending { private set; get; }
    public bool isOpening { private set; get; }
    public Vector2 initDeltaSize;
    private Vector2 targetDeltaSize;

    private Sequence seq;


    // Start is called before the first frame update
    void Awake()
    {
        seq = DOTween.Sequence();
        targetDeltaSize = windowImage.rectTransform.sizeDelta;
        audioSource = GetComponent<AudioSource>();
        WindowInit();
    }

    // Update is called once per frame
    void Update()
    {
        if (messageTextMesh.textInfo.characterCount > messageTextMesh.maxVisibleCharacters && isSending)
        {
            MessageUpdate();
        }
        else
        {
            isSending = false;
        }

        /*
        if (Input.GetKeyDown(KeyCode.O))
        {
            WindowOpen();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            WindowClose();
        }
        */
    }

    public void WindowInit()
    {
        messageTextMesh.maxVisibleCharacters = 0;
        currentFrame = 0;
        isSending = false;
        windowImage.enabled = true;
        windowImage.rectTransform.sizeDelta = initDeltaSize;

        messageTextMesh.enabled = false;
        if (narratorImage != null) narratorImage.enabled = false;
    }

    public void WindowOpen()
    {
        WindowInit();
        isOpening = true;
        seq.Kill();
        seq = DOTween.Sequence()
            //.Append(windowImage.DOFade(1,duration))
            //.Join(narratorImage.DOFade(1, duration))
            .Append(windowImage.rectTransform.DOSizeDelta(targetDeltaSize, duration)).SetEase(ease)
            //.Join(windowImage.rectTransform.DORotate(new Vector3(0, 0, 360), duration).SetRelative())
            .AppendCallback(() =>
            {
                messageTextMesh.enabled = true;
                if (narratorImage != null) narratorImage.enabled = true;
                isSending = true;
            });
    }

    public void WindowOpen(string textStr)
    {
        messageTextMesh.text = textStr;
        WindowOpen();
    }

    public void WindowOpen(string textStr, Sprite sprite)
    {
        narratorImage.sprite = sprite;
        WindowOpen(textStr);
    }

    public void WindowClose()
    {
        messageTextMesh.enabled = false;
        if (narratorImage != null) narratorImage.enabled = false;
        seq.Kill();
        seq = DOTween.Sequence()
            //.Append(windowImage.DOFade(1,duration))
            //.Join(narratorImage.DOFade(1, duration))
            .Append(windowImage.rectTransform.DOSizeDelta(initDeltaSize, duration)).SetEase(ease)
            //.Join(windowImage.rectTransform.DORotate(new Vector3(0, 0, 360), duration).SetRelative())
            .AppendCallback(() =>
            {
                windowImage.enabled = false;
                isOpening = false;
            });


    }

    public void MessageInit(string textStr)
    {
        messageTextMesh.text = textStr;
        messageTextMesh.maxVisibleCharacters = 0;
        currentFrame = 0;
        isSending = true;
    }

    private void MessageUpdate()
    {
        if (currentFrame >= intervalFrame)
        {
            currentFrame = 0;
            messageTextMesh.maxVisibleCharacters++;
            audioSource.PlayOneShot(audioSource.clip);
        }
        else
        {
            currentFrame++;
        }
    }
}
