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


    // Start is called before the first frame update
    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MessageStart();
        }
    }

    public void WindowInit()
    {
        messageTextMesh.maxVisibleCharacters = 0;
        currentFrame = 0;
        isSending = false;
        windowImage.rectTransform.sizeDelta = new Vector2(0, 400);
        //windowImage.color = new Color(1, 1, 1, 0);
        //narratorImage.color = new Color(1, 1, 1, 0);
       
        messageTextMesh.enabled = false;
        narratorImage.enabled = false;
    }

    public void MessageStart()
    {
        WindowInit();
        Sequence seq = DOTween.Sequence()
            //.Append(windowImage.DOFade(1,duration))
            //.Join(narratorImage.DOFade(1, duration))
            .Append(windowImage.rectTransform.DOSizeDelta(new Vector2(1800, 400), duration)).SetEase(ease)
            //.Join(windowImage.rectTransform.DORotate(new Vector3(0, 0, 360), duration).SetRelative())
            .AppendCallback(() =>
            {
                messageTextMesh.enabled = true;
                narratorImage.enabled = true;
                isSending = true;
            });


    }

    public void MessageStart(string textStr, float pitch = 1.0f)
    {
        text = textStr;
        messageTextMesh.text = textStr;
        messageTextMesh.maxVisibleCharacters = 0;
        audioSource.pitch = pitch;
        currentFrame = 0;
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
