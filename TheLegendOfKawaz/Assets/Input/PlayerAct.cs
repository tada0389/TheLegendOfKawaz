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
                    ""groups"": ""Keyboard"",
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
                    ""groups"": ""Keyboard"",
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
                    ""groups"": ""Keyboard"",
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
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7a31b020-6350-4ce1-afac-37cd1dc49b71"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4bc3213f-00de-4112-a67e-4be59e055821"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""34c1afd5-9349-491d-9e16-1a54cd7403e3"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""59f25310-0ca2-46a0-a72b-ac62cd3c7dae"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Decide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""64c189d0-df95-4cf6-a702-02ece79334bf"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9a6b53eb-82b1-4c6f-868e-03210125e662"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
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
                    ""groups"": ""Keyboard"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlatformActionGamepadPlus"",
            ""id"": ""ef3d48e7-113d-4c54-bd3e-3e19424ca093"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""185aeb91-b1dd-42ad-a1e5-5badb947af48"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Value"",
                    ""id"": ""469deb22-bc8b-4bde-bb54-645b229b0a16"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shot"",
                    ""type"": ""Button"",
                    ""id"": ""63dcb165-2d00-4e03-ada3-ecfa0c60c655"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""fe08bffd-cd3f-4a7b-9ed2-7223db961eae"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Decide"",
                    ""type"": ""Button"",
                    ""id"": ""4b3b1ef9-95cb-4173-be9d-1ed07e4c81fc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""ff225500-7097-4982-b58b-16fb0ad1c837"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""7da71444-836b-4149-9a42-dea8b63d00bb"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ffc8b973-719d-4c0b-93d9-8623d2ccbc81"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4786bbff-6089-4b1a-9304-429f5dbd5e97"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""b31cba57-defb-41c5-b7cb-58971aa736b6"",
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
                    ""id"": ""6b6731e2-4cd1-4da8-ad14-37a223349fa8"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f73e37a9-8f0a-437b-8b2f-57328db9782c"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""33531ada-0325-47b2-8d39-4c1d338b6416"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""580815cb-4b08-4c2a-bc59-201d65524e0c"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f8fc85fd-fa5f-4e86-8f37-50056440c8c5"",
                    ""path"": ""<Joystick>/stick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62f815ec-c908-47d4-bdb0-e7a9f5f24c47"",
                    ""path"": ""<HID::My-Power CO.,LTD. JC-P301U>/hat"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d9463900-b1d8-49dc-a1d0-407635b70f83"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e331510c-1415-4018-b47b-947b774df543"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5d6a8823-8a08-4f54-ae62-ac222a7823e5"",
                    ""path"": ""<HID::HORI CO.,LTD  PAD A>/button2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bf03d9d8-14fa-47e6-861d-dec783839b6a"",
                    ""path"": ""<HID::My-Power CO.,LTD. JC-P301U>/button3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c016e2f-5d1a-40da-b695-7ebb12c33140"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bee9ad0b-0da3-432d-bab3-46c108cf5b16"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ed9e1b7e-f611-4c46-80ed-90a75a71cd0b"",
                    ""path"": ""<HID::My-Power CO.,LTD. JC-P301U>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Shot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc617528-5452-4db9-936c-e518279b4bfe"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ffcf28ea-511d-4c40-a398-c3ca3db1e01e"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6bde1fcf-404e-4c13-be76-c78c88572588"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e1125e78-041b-4a5d-83ff-6d260abbc8bb"",
                    ""path"": ""<HID::My-Power CO.,LTD. JC-P301U>/button5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""61443852-b057-41c3-9793-7e9af3edfbd9"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Decide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ac1363c6-6c95-4ac1-980e-38dc3a9174a9"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Decide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2be77fce-036a-47db-978c-3ab9d7929b3e"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f2a88df-29b0-47a6-9a99-22c463377d91"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d2956047-3fdb-4b3c-a396-f10188287103"",
                    ""path"": ""<DualShockGamepad>/touchpadButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3970d5d3-206c-41df-8df2-b8c84433aefb"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""64263d54-d207-4a02-a9ef-b625db4b85f3"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c07294a-6fb0-4915-8904-be61945a6d93"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
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
        // PlatformActionGamepadPlus
        m_PlatformActionGamepadPlus = asset.FindActionMap("PlatformActionGamepadPlus", throwIfNotFound: true);
        m_PlatformActionGamepadPlus_Move = m_PlatformActionGamepadPlus.FindAction("Move", throwIfNotFound: true);
        m_PlatformActionGamepadPlus_Jump = m_PlatformActionGamepadPlus.FindAction("Jump", throwIfNotFound: true);
        m_PlatformActionGamepadPlus_Shot = m_PlatformActionGamepadPlus.FindAction("Shot", throwIfNotFound: true);
        m_PlatformActionGamepadPlus_Dash = m_PlatformActionGamepadPlus.FindAction("Dash", throwIfNotFound: true);
        m_PlatformActionGamepadPlus_Decide = m_PlatformActionGamepadPlus.FindAction("Decide", throwIfNotFound: true);
        m_PlatformActionGamepadPlus_Back = m_PlatformActionGamepadPlus.FindAction("Back", throwIfNotFound: true);
        m_PlatformActionGamepadPlus_Pause = m_PlatformActionGamepadPlus.FindAction("Pause", throwIfNotFound: true);
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

    // PlatformActionGamepadPlus
    private readonly InputActionMap m_PlatformActionGamepadPlus;
    private IPlatformActionGamepadPlusActions m_PlatformActionGamepadPlusActionsCallbackInterface;
    private readonly InputAction m_PlatformActionGamepadPlus_Move;
    private readonly InputAction m_PlatformActionGamepadPlus_Jump;
    private readonly InputAction m_PlatformActionGamepadPlus_Shot;
    private readonly InputAction m_PlatformActionGamepadPlus_Dash;
    private readonly InputAction m_PlatformActionGamepadPlus_Decide;
    private readonly InputAction m_PlatformActionGamepadPlus_Back;
    private readonly InputAction m_PlatformActionGamepadPlus_Pause;
    public struct PlatformActionGamepadPlusActions
    {
        private @PlayerAct m_Wrapper;
        public PlatformActionGamepadPlusActions(@PlayerAct wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlatformActionGamepadPlus_Move;
        public InputAction @Jump => m_Wrapper.m_PlatformActionGamepadPlus_Jump;
        public InputAction @Shot => m_Wrapper.m_PlatformActionGamepadPlus_Shot;
        public InputAction @Dash => m_Wrapper.m_PlatformActionGamepadPlus_Dash;
        public InputAction @Decide => m_Wrapper.m_PlatformActionGamepadPlus_Decide;
        public InputAction @Back => m_Wrapper.m_PlatformActionGamepadPlus_Back;
        public InputAction @Pause => m_Wrapper.m_PlatformActionGamepadPlus_Pause;
        public InputActionMap Get() { return m_Wrapper.m_PlatformActionGamepadPlus; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlatformActionGamepadPlusActions set) { return set.Get(); }
        public void SetCallbacks(IPlatformActionGamepadPlusActions instance)
        {
            if (m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnJump;
                @Shot.started -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnShot;
                @Shot.performed -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnShot;
                @Shot.canceled -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnShot;
                @Dash.started -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnDash;
                @Decide.started -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnDecide;
                @Decide.performed -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnDecide;
                @Decide.canceled -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnDecide;
                @Back.started -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnBack;
                @Pause.started -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_PlatformActionGamepadPlusActionsCallbackInterface = instance;
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
    public PlatformActionGamepadPlusActions @PlatformActionGamepadPlus => new PlatformActionGamepadPlusActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
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
    public interface IPlatformActionGamepadPlusActions
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
