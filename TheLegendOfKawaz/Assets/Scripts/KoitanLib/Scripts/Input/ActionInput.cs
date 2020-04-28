using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class ActionInput : MonoBehaviour
{
    //シングルトン
    public static ActionInput Instance;
    private Dictionary<ButtonCode, bool> oldButtonValue = new Dictionary<ButtonCode, bool>();
    private Dictionary<ButtonCode, bool> buttonValue = new Dictionary<ButtonCode, bool>();
    private Dictionary<AxisCode, float> oldAxisValue = new Dictionary<AxisCode, float>();
    private Dictionary<AxisCode, float> axisValue = new Dictionary<AxisCode, float>();
    private Dictionary<ActionCode, bool> oldActionValue = new Dictionary<ActionCode, bool>();
    private Dictionary<ActionCode, bool> actionValue = new Dictionary<ActionCode, bool>();
    private Dictionary<ActionCode, bool> actionFlag = new Dictionary<ActionCode, bool>();

    // インプットの登録と破棄
    PlayerAct input;

    //スティックのしきい値
    float deadline = 0.25f;

    //ゲームパッド
    Gamepad gamepad;
    Joystick joystick;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            input = new PlayerAct();

            //初期化
            foreach (ButtonCode code in Enum.GetValues(typeof(ButtonCode)))
            {
                buttonValue.Add(code, false);
                oldButtonValue.Add(code, false);
            }
            foreach (AxisCode code in Enum.GetValues(typeof(AxisCode)))
            {
                axisValue.Add(code, 0);
                oldAxisValue.Add(code, 0);
            }
            foreach (ActionCode code in Enum.GetValues(typeof(ActionCode)))
            {
                actionValue.Add(code, false);
                oldActionValue.Add(code, false);
                actionFlag.Add(code, false);
            }

        }
        else
        {
            Destroy(gameObject);
        }
    }


    void OnDisable() => input.Disable();

    // インプットの有効・無効化
    void OnDestroy() => input.Disable();
    void OnEnable() => input.Enable();

    void Start()
    {
        gamepad = Gamepad.current;
        joystick = Joystick.current;
        /*
        DebugTextManager.Display(() => "leftTrigger:" + gamepad.leftTrigger.ReadValue().ToString() + "\n");
        DebugTextManager.Display(() => "leftShoulder:" + gamepad.leftShoulder.ReadValue().ToString() + "\n");
        DebugTextManager.Display(() => "ButtonEast:" + gamepad.buttonEast.ReadValue().ToString() + "\n");
        */
        //input.PlatformAction.Dash.ApplyBindingOverride("<Gamepad>/buttonNorth", input.GamepadScheme.bindingGroup);
        //joystick.

        /*
        DebugTextManager.Display(() => "ReadValue:" + input.PlatformAction.Jump.ReadValue<float>().ToString() + "\n");
        DebugTextManager.Display(() => "JumpFlag:" + Instance.actionFlag[ActionCode.Jump].ToString() + "\n");
        DebugTextManager.Display(() => "oldJumpValue:" + Instance.oldActionValue[ActionCode.Jump].ToString() + "\n");
        DebugTextManager.Display(() => "JumpValue:" + Instance.actionValue[ActionCode.Jump].ToString() + "\n");
        
        DebugTextManager.Display(() => "Up:" + Instance.buttonValue[ButtonCode.Up].ToString() + "\n");
        DebugTextManager.Display(() => "UpDown:" + GetButtonDown(ButtonCode.Up).ToString() + "\n");
        DebugTextManager.Display(() => "UpUp:" + GetButtonUp(ButtonCode.Up).ToString() + "\n");
        */
    }

    private void Update()
    {
        gamepad = Gamepad.current;
        joystick = Joystick.current;
        //デバッグ用
        /*
        foreach (var i in Keyboard.current.allKeys)
        {
            if (i.ReadValue() > 0)
            {
                Debug.Log("押されたキーのpath:" + i.path + "\n");
            }
        }
        */

        //更新
        foreach (ButtonCode code in Enum.GetValues(typeof(ButtonCode)))
        {
            oldButtonValue[code] = buttonValue[code];
            buttonValue[code] = currentButtonValue(code);
        }

        foreach (AxisCode code in Enum.GetValues(typeof(AxisCode)))
        {
            oldAxisValue[code] = axisValue[code];
            axisValue[code] = currentAxisValue(code);
        }

        foreach (ActionCode code in Enum.GetValues(typeof(ActionCode)))
        {
            oldActionValue[code] = actionValue[code];
            actionValue[code] = currentButtonValue(code);
            actionFlag[code] = false;
        }
    }


    //オーバーロード
    public static bool GetButton(ButtonCode code)
    {
        return Instance.buttonValue[code];
    }

    public static bool GetButton(ActionCode code)
    {
        return Instance.actionValue[code];
    }

    public static bool GetButtonDown(ButtonCode code)
    {
        return (!Instance.oldButtonValue[code] && Instance.buttonValue[code]);
    }

    public static bool GetButtonDown(ActionCode code)
    {
        return (!Instance.oldActionValue[code] && Instance.actionValue[code]);
    }

    public static bool GetButtonUp(ButtonCode code)
    {
        return (Instance.oldButtonValue[code] && !Instance.buttonValue[code]);
    }

    public static bool GetButtonUp(ActionCode code)
    {
        return (Instance.oldActionValue[code] && !Instance.actionValue[code]);
    }

    private bool currentButtonValue(ButtonCode code)
    {
        switch (code)
        {
            case ButtonCode.Up:
                return Instance.input.PlatformAction.Move.ReadValue<Vector2>().y > deadline;
            case ButtonCode.Down:
                return Instance.input.PlatformAction.Move.ReadValue<Vector2>().y < -deadline;
            case ButtonCode.Left:
                return Instance.input.PlatformAction.Move.ReadValue<Vector2>().x < -deadline;
            case ButtonCode.Right:
                return Instance.input.PlatformAction.Move.ReadValue<Vector2>().x > deadline;
        }
        return false;
    }

    private bool currentButtonValue(ActionCode code)
    {
        switch (code)
        {
            case ActionCode.Jump:
                return input.PlatformAction.Jump.ReadValue<float>() > 0;
            case ActionCode.Shot:
                return input.PlatformAction.Shot.ReadValue<float>() > 0;
            case ActionCode.Dash:
                return input.PlatformAction.Dash.ReadValue<float>() > 0;
            case ActionCode.Decide:
                return input.PlatformAction.Decide.ReadValue<float>() > 0;
            case ActionCode.Back:
                return input.PlatformAction.Back.ReadValue<float>() > 0;
            case ActionCode.Pause:
                return input.PlatformAction.Pause.ReadValue<float>() > 0;
        }
        return false;
    }

    private float currentAxisValue(AxisCode code)
    {
        switch (code)
        {
            case AxisCode.Horizontal:
                return Instance.input.PlatformAction.Move.ReadValue<Vector2>().x;
            case AxisCode.Vertical:
                return Instance.input.PlatformAction.Move.ReadValue<Vector2>().y;
        }
        return 0;
    }

    public static float GetAxis(AxisCode code)
    {
        return Instance.axisValue[code];
    }

    public static string GetSpriteCode(ActionCode code)
    {
        var gamePad = Gamepad.current;
        var joyStick = Joystick.current;
        if (gamePad == null && joyStick == null)
        {
            switch (code)
            {
                case ActionCode.Jump:
                    return "Zキー";
                case ActionCode.Shot:
                    return "Xキー";
                case ActionCode.Dash:
                    return "Cキー";
                case ActionCode.Decide:
                    return "Zキー";
                case ActionCode.Back:
                    return "Xキー";
                case ActionCode.Pause:
                    return "Shiftキー";
            }
        }
        else
        {
            switch (code)
            {
                case ActionCode.Jump:
                    return "<sprite=\"ps4\" index=26>";
                case ActionCode.Shot:
                    return "<sprite=\"ps4\" index=23>";
                case ActionCode.Dash:
                    return "<sprite=\"ps4\" index=15><sprite=\"ps4\" index=16>";
                case ActionCode.Decide:
                    return "<sprite=\"ps4\" index=25>";
                case ActionCode.Back:
                    return "<sprite=\"ps4\" index=26>";
                case ActionCode.Pause:
                    return "<sprite=\"ps4\" index=21>";
            }
        }
        return "None";
    }

    public static string GetSpriteCode(AxisCode code)
    {
        var gamePad = Gamepad.current;
        var joyStick = Joystick.current;
        if (gamePad == null && joyStick == null)
        {
            switch (code)
            {
                case AxisCode.Horizontal:
                    return "←→";
                case AxisCode.Vertical:
                    return "↑↓";
            }
        }
        else
        {
            switch (code)
            {
                case AxisCode.Horizontal:
                    return "<sprite=\"ps4\" index=6>";
                case AxisCode.Vertical:
                    return "<sprite=\"ps4\" index=5>";
            }
        }
        return "None";
    }

    public static string GetSpriteCode(ButtonCode code)
    {
        var gamePad = Gamepad.current;
        var joyStick = Joystick.current;
        if (gamePad == null && joyStick == null)
        {
            switch (code)
            {
                case ButtonCode.Up:
                    return "↑";
                case ButtonCode.Down:
                    return "↓";
                case ButtonCode.Left:
                    return "→";
                case ButtonCode.Right:
                    return "←";
            }
        }
        else
        {
            switch (code)
            {
                case ButtonCode.Up:
                    return "<sprite=\"ps4\" index=2>";
                case ButtonCode.Down:
                    return "<sprite=\"ps4\" index=4>";
                case ButtonCode.Left:
                    return "<sprite=\"ps4\" index=1>";
                case ButtonCode.Right:
                    return "<sprite=\"ps4\" index=3>";
            }
        }
        return "None";
    }

    /*
    public void OnMove(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        actionFlag[ActionCode.Jump] = true;
        Debug.Log("OnJump");
    }
    */

}

public enum ButtonCode
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
}

public enum AxisCode
{
    Horizontal = 0,
    Vertical = 1,
}

public enum ActionCode
{
    Jump = 0,
    Shot = 1,
    Dash = 2,
    Decide,
    Back,
    Pause
}
