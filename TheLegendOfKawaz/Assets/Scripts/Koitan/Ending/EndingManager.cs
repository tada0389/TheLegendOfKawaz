﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using KoitanLib;
using System;

public class EndingManager : MonoBehaviour
{
    private Sequence seq;
    [SerializeField]
    private GameObject startObj;
    [SerializeField]
    private GameObject lastObj;
    [SerializeField]
    private GameObject exObj;
    [SerializeField]
    private SpriteRenderer makuSpr;
    [SerializeField]
    private TextMeshPro timeMesh;
    private bool canSkip;
    private bool isLoaded;

    // 隠しのエンディングシーンじゃないかどうか by tada
    [SerializeField]
    private bool IsTrueEnding = true;

    // Start is called before the first frame update
    void Start()
    {
        //TimeRecoder.Stop();
        Global.GlobalDataManager.StopStoryTimer();
        Global.GlobalDataManager.AddEndingCnt();

        // クリアタイムを別の方法で取得 by tada
        double clear_time = Global.GlobalDataManager.StoryTimer;
        if (IsTrueEnding)
        {
            AchievementManager.FireAchievement("GameClear");
            if (clear_time <= 3600)
            {
                AchievementManager.FireAchievement("Kagawa");
            }
            if (clear_time <= 300)
            {
                AchievementManager.FireAchievement("RTA");
            }

            CheckNoSkillClear();
        }
        TimeSpan ts = new TimeSpan(0, 0, (int)clear_time);
        timeMesh.text = ts.ToString() + "\n" + AchievementManager.GetAchieveNowUnlockedNum().ToString() + "/" + AchievementManager.GetAchieveMax();
        lastObj.SetActive(false);
        exObj.SetActive(false);
        startObj.SetActive(true);
        makuSpr.color = Color.black;

        seq = DOTween.Sequence()
            .Append(makuSpr.DOFade(0f, 1f))
            .AppendInterval(4f)
            .Append(makuSpr.DOFade(1f, 1f))
            .AppendCallback(() => startObj.SetActive(false))
            .AppendInterval(71f)
            .AppendCallback(() => lastObj.SetActive(true))
            .Append(makuSpr.DOFade(0f, 2f))
            .AppendInterval(1f)
            /*
            .Append(makuSpr.DOFade(1f, 1f))            
            .AppendCallback(() =>
            {
                makuSpr.sortingOrder = 30;
                lastObj.SetActive(false);
                exObj.SetActive(true);
            })
            .Append(makuSpr.DOFade(0f, 1f))
            */
            .AppendCallback(() =>
            {
                canSkip = true;
            });


        //.AppendCallback(() => canSkip = true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsTrueEnding) return;

        if (canSkip & ActionInput.GetButtonDown(ActionCode.Decide) && !isLoaded)
        {
            LoadTitle();            
        }

        if (ActionInput.GetButtonDown(ActionCode.Dash) && !isLoaded)
        {
            LoadTitle();
        }
    }

    private void LoadTitle()
    {
        // セーブもする
        TadaLib.Save.SaveManager.Instance.Save();
        isLoaded = true;
        FadeManager.FadeIn(2f, "Gojitudan");
    }

    // ノースキルでクリアしたかをチェックする by tada
    private void CheckNoSkillClear()
    {
        bool no_skill = Actor.Player.SkillManager.Instance.IsNoSkill();
        if (no_skill) AchievementManager.FireAchievement("Hadaka");
    }
}
