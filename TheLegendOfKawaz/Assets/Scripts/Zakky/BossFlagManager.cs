using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

public class BossFlagManager : SingletonMonoBehaviour<BossFlagManager>
{
    [SerializeField]
    private int bossRoomNum = 6;

    public List<bool> BossFlag { private set; get; }

    protected override void Awake()
    {
        base.Awake();

        BossFlag = new List<bool>();
        for (int i = 0; i < bossRoomNum; ++i) BossFlag.Add(false);
    }

    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
