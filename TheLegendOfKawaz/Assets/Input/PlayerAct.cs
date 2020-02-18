// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerAct.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerAct : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerAct()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerAct"",
    ""maps"": [
        {
            ""name"": ""PlatformAction"",
            ""id"": ""d2debad1-d707-4f12-911b-2877aadb8837"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""b7467e71-f346-4655-becc-904b260ec376"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Value"",
                    ""id"": ""1e3aea51-fd1d-4c00-8f5d-1584e288d8ad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shot"",
                    ""type"": ""Button"",
                    ""id"": ""a8962f47-112f-496d-a3a4-a0ec0bed5b5b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""94bb74b2-878b-43af-8ff6-a8084e45a468"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""dab47c0e-0000-46d5-8a6f-09b9f18a4202"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b86da34e-7f8f-4703-9262-6796751de0fb"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f28c6301-7166-4abc-9cc8-1ddbcbb1fe83"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bdf91ce6-672d-4a42-a7e6-86683342a9ae"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d2bf3064-db50-440e-a244-60e6818dc098"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""6e36a397-3a71-42aa-a21b-ab4cad767f9f"",
                    ""path"": ""<Joystick>/stick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""58e26e4f-e5a9-4f2e-af1d-436d833958c9"",
                    ""path"": ""<HID::My-Power CO.,LTD. JC-P301U>/hat"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b879028-424d-48d9-8b33-0b7938362b52"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a31b020-6350-4ce1-afac-37cd1dc49b71"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""81568e41-5067-4686-8d49-58bfacd1885f"",
                    ""path"": ""<HID::HORI CO.,LTD  PAD A>/button2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f23b940e-0831-49ab-a1ef-c0842dc094c1"",
                    ""path"": ""<HID::My-Power CO.,LTD. JC-P301U>/button3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92f77ac2-1f45-49a4-be8f-09a281ce923d"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4bc3213f-00de-4112-a67e-4be59e055821"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""25a6ebfd-d772-4146-a809-a3285d018ea6"",
                    ""path"": ""<HID::My-Power CO.,LTD. JC-P301U>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlatformAction
        m_PlatformAction = asset.FindActionMap("PlatformAction", throwIfNotFound: true);
        m_PlatformAction_Move = m_PlatformAction.FindAction("Move", throwIfNotFound: true);
        m_PlatformAction_Jump = m_PlatformAction.FindAction("Jump", throwIfNotFound: true);
        m_PlatformAction_Shot = m_PlatformAction.FindAction("Shot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // PlatformAction
    private readonly InputActionMap m_PlatformAction;
    private IPlatformActionActions m_PlatformActionActionsCallbackInterface;
    private readonly InputAction m_PlatformAction_Move;
    private readonly InputAction m_PlatformAction_Jump;
    private readonly InputAction m_PlatformAction_Shot;
    public struct PlatformActionActions
    {
        private @PlayerAct m_Wrapper;
        public PlatformActionActions(@PlayerAct wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlatformAction_Move;
        public InputAction @Jump => m_Wrapper.m_PlatformAction_Jump;
        public InputAction @Shot => m_Wrapper.m_PlatformAction_Shot;
        public InputActionMap Get() { return m_Wrapper.m_PlatformAction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlatformActionActions set) { return set.Get(); }
        public void SetCallbacks(IPlatformActionActions instance)
        {
            if (m_Wrapper.m_PlatformActionActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnJump;
                @Shot.started -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnShot;
                @Shot.performed -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnShot;
                @Shot.canceled -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnShot;
            }
            m_Wrapper.m_PlatformActionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Shot.started += instance.OnShot;
                @Shot.performed += instance.OnShot;
                @Shot.canceled += instance.OnShot;
            }
        }
    }
    public PlatformActionActions @PlatformAction => new PlatformActionActions(this);
    public interface IPlatformActionActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnShot(InputAction.CallbackContext context);
    }
}
