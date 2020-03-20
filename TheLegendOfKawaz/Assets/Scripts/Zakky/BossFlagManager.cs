using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TadaLib;

public class BossFlagManager : SingletonMonoBehaviour<BossFlagManager>
{
    //SingletonMonoBehavourを継承しているので実質全部staticになる
    [SerializeField]
    private int bossRoomSum = 6;
    [System.NonSerialized]
    public int bossRoomNum;

    public List<bool> BossFlag { private set; get; }

    protected override void Awake()
    {
        base.Awake();

        BossFlag = new List<bool>();
        for (int i = 0; i < bossRoomSum; ++i) BossFlag.Add(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // デバッグ
        if (UnityEngine.InputSystem.Keyboard.current[UnityEngine.InputSystem.Key.B].wasPressedThisFrame)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SkillGetScene");
        }
    }
}
