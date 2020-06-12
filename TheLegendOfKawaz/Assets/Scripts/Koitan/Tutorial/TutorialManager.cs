﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public TutorialPage[] pages;
    public List<TutorialPage> openedList;

    private int pageIndex;
    private GameObject pageObj;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenedPageUpdate()
    {
        openedList.Clear();
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i].isOpen) openedList.Add(pages[i]);
        }
    }

    public void PageOpen(int index)
    {
        pageIndex = index;
        pageObj = openedList[index].pageParent;
        pageObj.SetActive(true);
    }

    public void PageOpenFirst(int index)
    {
        pageIndex = index;
        pageObj = pages[index].pageParent;
        pageObj.SetActive(true);
    }

    public void PageClose()
    {
        pageObj.SetActive(false);
    }

    public void PageNext()
    {
        pageObj.SetActive(false);
        pageIndex++;
        pageObj = openedList[pageIndex].pageParent;
        pageObj.SetActive(true);
    }

    public void PagePrev()
    {
        pageObj.SetActive(false);
        pageIndex--;
        pageObj = openedList[pageIndex].pageParent;
        pageObj.SetActive(true);
    }

    public bool PageTop()
    {
        return pageIndex == 0;
    }

    public bool PageTail()
    {
        return pageIndex == pages.Length - 1;
    }

    [Serializable]
    public class TutorialPage
    {
        public string pageTitle;
        public GameObject pageParent;
        public bool isOpen = false;
    }
}
