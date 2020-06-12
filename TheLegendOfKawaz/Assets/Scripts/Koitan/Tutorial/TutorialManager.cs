using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField]
    private TutorialPage[] pages;

    [SerializeField]
    private Transform uiParent;

    private int pageIndex;
    private int headIndex;
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

    public static void PageOpen(int index)
    {
        Instance.headIndex = index;
        Instance.pageIndex = 0;
        Instance.pageObj =  Instantiate(Instance.pages[index].pageParetns[0], Instance.uiParent);
    }

    public static void PageClose()
    {
        Destroy(Instance.pageObj);
    }

    public static void PageNext()
    {
        Destroy(Instance.pageObj);        
        Instance.pageIndex++;
        Instance.pageObj = Instantiate(Instance.pages[Instance.headIndex].pageParetns[Instance.pageIndex], Instance.uiParent);
    }

    public static void PagePrev()
    {
        Destroy(Instance.pageObj);
        Instance.pageIndex--;
        Instance.pageObj = Instantiate(Instance.pages[Instance.headIndex].pageParetns[Instance.pageIndex], Instance.uiParent);
    }

    public static bool PageTop()
    {
        return Instance.pageIndex == 0;
    }

    public static bool PageTail()
    {
        return Instance.pageIndex == Instance.pages[Instance.headIndex].pageWidth - 1;
    }

    [Serializable]
    public class TutorialPage
    {
        public string pageTitle;
        public int pageWidth;
        public GameObject[] pageParetns;
    }
}
