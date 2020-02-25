using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DebugTextManager.Display(() => "InputDevice:\n" + GetInputDevicesName());
        //DebugTextManager.Display(() => "GamePad:\n" + GetGamePadName());
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;        
        //　ゲームパッドが接続されていなければこれ以降
        if (gamepad == null)
        {
            return;
        }       
    }

    string GetGamePadName()
    {
        string str = string.Empty;
        var gamePadArray = Gamepad.all;
        for (int i = 0; i < gamePadArray.Count; i++)
        {
            str += i.ToString() + ":" + gamePadArray[i].name + "\n";
        }
        return str;
    }

    string GetInputDevicesName()
    {
        string str = string.Empty;
        var devicesArray = InputSystem.devices;
        for (int i = 0; i < devicesArray.Count; i++)
        {
            str += i.ToString() + ":" + devicesArray[i].name + "\n";
        }
        foreach (InputDevice device in devicesArray)
        {
            str += device.deviceId + ":" + device.displayName + "\n";
        }
        return str;
    }
}
