using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //DebugTextManager.Display(() => "Decide:" + ActionInput.GetSpriteCode(ActionCode.Decide) + "\n");
        //DebugTextManager.Display(() => "Back:" + ActionInput.GetSpriteCode(ActionCode.Back) + "\n");
        DebugTextManager.Display(() => "Jump:" + ActionInput.GetSpriteCode(ActionCode.Jump) + "\n");
        DebugTextManager.Display(() => "Shot:" + ActionInput.GetSpriteCode(ActionCode.Shot) + "\n");
        DebugTextManager.Display(() => "Dash:" + ActionInput.GetSpriteCode(ActionCode.Dash) + "\n");
        //DebugTextManager.Display(() => "Horizontal:" + ActionInput.GetSpriteCode(AxisCode.Horizontal) + "\n");
        //DebugTextManager.Display(() => "Down:" + ActionInput.GetSpriteCode(ButtonCode.Down) + "\n");
        DebugTextManager.Display(() => "GamePad:\n" + GetInputDevicesName());
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
        //var gamePadChildren = Gamepad.current.allControls;
        
        for (int i = 0; i < devicesArray.Count; i++)
        {
            str += i.ToString() + ":" + devicesArray[i].name + "\n";
        }
        
        /*
        foreach (InputDevice device in devicesArray)
        {
            str += device.deviceId + ":" + device.displayName + "\n";
        }
        */
        /*
        for (int i = 0; i < gamePadChildren.Count; i++)
        {
            str += i.ToString() + ":" + gamePadChildren[i].name + "\n";
        }
        */
        return str;
    }
}
