using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecoder : MonoBehaviour
{
    public static TimeRecoder Instance;
    public static float GlobalTime { get; private set; }
    public static bool isRun { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //デバッグ用
        DebugTextManager.Display(() => "GlobalTime = " + GlobalTime.ToString() + "\n");
        Run();
    }

    // Update is called once per frame
    void Update()
    {
        if(isRun)
        {
            GlobalTime += Time.unscaledDeltaTime;
        }
    }

    public static void Reset()
    {
        GlobalTime = 0;
    }

    public static void Stop()
    {
        isRun = false;
    }

    public static void Run()
    {
        isRun = true;
    }
}
