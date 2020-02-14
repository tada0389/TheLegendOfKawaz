using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputTest2 : MonoBehaviour
{
    // インプットの登録と破棄
    private PlayerAct input;
    void Awake() => input = new PlayerAct();
    void OnDisable() => input.Disable();

    // インプットの有効・無効化
    void OnDestroy() => input.Disable();
    void OnEnable() => input.Enable();

    private void Update()
    {
        Debug.Log(input.controlSchemes.Count);
        // スティックの移動を取得して動かす
        var velocity = input.PlatformAction.Move.ReadValue<Vector2>();
        //Debug.Log(velocity.x);

        // ジャンプが押されているか判定して動かす
        //Debug.Log(input.PlatformAction.Jump.ReadValue<float>());
        var isJumpButtonPressed = (input.PlatformAction.Jump.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint);
        if (isJumpButtonPressed) Debug.Log("jump");
    }
}
