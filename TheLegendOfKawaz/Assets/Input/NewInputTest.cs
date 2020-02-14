using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputTest : MonoBehaviour, PlayerAct.IPlatformActionActions
{
    PlayerAct.PlatformActionActions input;

    float horizontal;

    private void Awake()
    {
        // インプットを生成して，自身をコールバックとして登録
        input = new PlayerAct.PlatformActionActions(new PlayerAct());
        input.SetCallbacks(this);
    }

    // インプットの有効・無効化
    void OnDestroy() => input.Disable();
    void OnEnable() => input.Enable();

    void OnDisable() => input.Disable();
    //void Update() => Debug.Log(horizontal);

    //Dictionary<string, bool> flags;

    // コールバック
    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log(context.action.name);
        Debug.Log(context.startTime + " : jump");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }
}
