using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFadeout : MonoBehaviour
{
    public float fadeTime = 1f;

    private float currentRemainTime;
    private SpriteRenderer spRenderer;

    // Use this for initialization
    void Start()
    {
        
    }
    private void OnEnable()
    {
        spRenderer = GetComponent<SpriteRenderer>();
    }
    public void ReStart()
    {
        // 初期化
        currentRemainTime = fadeTime;
        var color = spRenderer.color;
        color.a = 1f;
        spRenderer.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        // 残り時間を更新
        currentRemainTime -= Time.deltaTime;

        if (currentRemainTime <= 0f)
        {
            // 残り時間が無くなったら自分自身を消滅
            gameObject.SetActive(false);
            return;
        }

        // フェードアウト
        float alpha = currentRemainTime / fadeTime;
        var color = spRenderer.color;
        color.a = alpha;
        spRenderer.color = color;
    }
}
