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
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""cec807d6-14ef-413e-8688-f10f0f551991"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Decide"",
                    ""type"": ""Button"",
                    ""id"": ""5ab6348d-1468-4856-935d-5a800acb5a45"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""c2a5d623-bd5f-415f-a5ce-5904551d750d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""43056d35-0e9f-47b3-89aa-edb34b35dec2"",
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
                    ""name"": """",
                    ""id"": ""23d6e559-d6f5-4496-a5d7-cba3768caaa9"",
                    ""path"": ""<Gamepad>/leftStick"",
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
                    ""path"": ""<Keyboard>/upArrow"",
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
                    ""path"": ""<Keyboard>/downArrow"",
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
                    ""path"": ""<Keyboard>/leftArrow"",
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
                    ""path"": ""<Keyboard>/rightArrow"",
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
                    ""path"": ""<Gamepad>/buttonSouth"",
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
                    ""path"": ""<Keyboard>/z"",
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
                    ""path"": ""<Keyboard>/x"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""fccc9c69-a5cb-439f-a168-e60cb35b5e45"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ee38dd9-6922-46f3-a45a-1309d16b4cd9"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""34c1afd5-9349-491d-9e16-1a54cd7403e3"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d10bb15f-18e2-43b9-8105-697b10ebc333"",
                    ""path"": ""<HID::My-Power CO.,LTD. JC-P301U>/button5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1dec7ab-cbfc-40c2-9ba2-4d2d98f90250"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Decide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59f25310-0ca2-46a0-a72b-ac62cd3c7dae"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Decide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""370f712d-2668-4484-99a0-3268f35e4bcd"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""64c189d0-df95-4cf6-a702-02ece79334bf"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb2399a5-938d-4feb-945b-bdad51d72682"",
                    ""path"": ""<DualShockGamepad>/touchpadButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c16cdb9-c561-4051-a2f7-f1bdff3a29c1"",
                    ""path"": ""<DualShockGamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9a6b53eb-82b1-4c6f-868e-03210125e662"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b95c8172-07ce-4032-a977-22cadd8c1829"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
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
        m_PlatformAction_Dash = m_PlatformAction.FindAction("Dash", throwIfNotFound: true);
        m_PlatformAction_Decide = m_PlatformAction.FindAction("Decide", throwIfNotFound: true);
        m_PlatformAction_Back = m_PlatformAction.FindAction("Back", throwIfNotFound: true);
        m_PlatformAction_Pause = m_PlatformAction.FindAction("Pause", throwIfNotFound: true);
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
    private readonly InputAction m_PlatformAction_Dash;
    private readonly InputAction m_PlatformAction_Decide;
    private readonly InputAction m_PlatformAction_Back;
    private readonly InputAction m_PlatformAction_Pause;
    public struct PlatformActionActions
    {
        private @PlayerAct m_Wrapper;
        public PlatformActionActions(@PlayerAct wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlatformAction_Move;
        public InputAction @Jump => m_Wrapper.m_PlatformAction_Jump;
        public InputAction @Shot => m_Wrapper.m_PlatformAction_Shot;
        public InputAction @Dash => m_Wrapper.m_PlatformAction_Dash;
        public InputAction @Decide => m_Wrapper.m_PlatformAction_Decide;
        public InputAction @Back => m_Wrapper.m_PlatformAction_Back;
        public InputAction @Pause => m_Wrapper.m_PlatformAction_Pause;
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
                @Dash.started -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnDash;
                @Decide.started -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnDecide;
                @Decide.performed -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnDecide;
                @Decide.canceled -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnDecide;
                @Back.started -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnBack;
                @Pause.started -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PlatformActionActionsCallbackInterface.OnPause;
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
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Decide.started += instance.OnDecide;
                @Decide.performed += instance.OnDecide;
                @Decide.canceled += instance.OnDecide;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public PlatformActionActions @PlatformAction => new PlatformActionActions(this);
    public interface IPlatformActionActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnShot(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnDecide(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
}
