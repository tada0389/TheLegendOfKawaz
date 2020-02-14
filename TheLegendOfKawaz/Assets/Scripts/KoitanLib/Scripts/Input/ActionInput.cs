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
        
        DebugTextManager.Display(() => "ReadValue:" + input.PlatformAction.Jump.ReadValue<float>().ToString() + "\n");
        DebugTextManager.Display(() => "JumpFlag:" + Instance.actionFlag[ActionCode.Jump].ToString() + "\n");
        DebugTextManager.Display(() => "oldJumpValue:" + Instance.oldActionValue[ActionCode.Jump].ToString() + "\n");
        DebugTextManager.Display(() => "JumpValue:" + Instance.actionValue[ActionCode.Jump].ToString() + "\n");
        
    }

    private void Update()
    {
        //更新
        foreach (ButtonCode code in Enum.GetValues(typeof(ButtonCode)))
        {
            oldButtonValue[code] = buttonValue[code];
            buttonValue[code] = currentButtonValue(code);
        }
        /*
        foreach (AxisCode code in Enum.GetValues(typeof(AxisCode)))
        {
            axisValue.Add(code, 0);
            oldAxisValue.Add(code, 0);
        }
        */
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
                break;
            case ButtonCode.Down:
                break;
            case ButtonCode.Left:
                break;
            case ButtonCode.Right:
                break;
        }
        return false;
    }

    private bool currentButtonValue(ActionCode code)
    {
        return input.PlatformAction.Jump.ReadValue<float>() > 0;
    }

    public static float GetAxis(AxisCode code)
    {
        switch(code)
        {
            case AxisCode.Horizontal:
                return Instance.input.PlatformAction.Move.ReadValue<Vector2>().x;
            case AxisCode.Vertical:
                return Instance.input.PlatformAction.Move.ReadValue<Vector2>().y;
        }
        return 0;
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
    Shot = 1
}
