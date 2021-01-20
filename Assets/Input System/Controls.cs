// GENERATED AUTOMATICALLY FROM 'Assets/Input System/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Master"",
            ""id"": ""fe6537ac-7067-45ec-9ecd-18f23f135d24"",
            ""actions"": [
                {
                    ""name"": ""LMB"",
                    ""type"": ""Button"",
                    ""id"": ""2b33c84b-11c8-4445-8f33-024d8771e93d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RMB"",
                    ""type"": ""Button"",
                    ""id"": ""77365dda-ee6e-4d1a-9bc4-c96bbbe09d14"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shift LMB"",
                    ""type"": ""Button"",
                    ""id"": ""8235a6ea-b79d-4993-9db5-17c2abd056cc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shift RMB"",
                    ""type"": ""Button"",
                    ""id"": ""1822b38b-cfb5-4875-8226-21d29d505d7e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ctrl LMB"",
                    ""type"": ""Button"",
                    ""id"": ""c5b6d656-41a5-4cdc-b29b-e43982495654"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ctrl RMB"",
                    ""type"": ""Button"",
                    ""id"": ""dc7ebe05-3d8f-4f5f-9ebf-8c62fab270b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Floor Up"",
                    ""type"": ""Button"",
                    ""id"": ""968763e4-0606-4865-b387-a5746d12c85d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Floor Down"",
                    ""type"": ""Button"",
                    ""id"": ""2de6d8d2-31a1-49d0-8e32-7b60311d7396"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a6da5980-139d-4c6b-8368-783ffdf647b2"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""LMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46ac3df0-e287-40c9-9897-5964612e564e"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""RMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""78fa70b8-6f19-4cc5-8bf4-6b46538b8e0e"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""Floor Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da4b7523-adf5-4e7b-8869-8f7e066591c3"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""Floor Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""3916c2e7-ac2a-4bb1-ab6b-ba17855846ac"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift LMB"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""648c7841-c451-40c1-ba3c-efa2a6fcbcf3"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift LMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""c406ccc7-2ba0-42be-887f-bfb73744cb87"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift LMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""e59494a4-1dc1-411d-b4aa-a6d9c685ac05"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift RMB"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""36dd71d4-9c43-4627-b295-08362005c3b0"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift RMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""f5a996b7-e75e-4d97-afc7-bc391f452c53"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shift RMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""e9960491-ce25-4931-a1be-9e6f0196ebbd"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl LMB"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""67138e63-d517-4bf7-a591-30f3bcabe69b"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl LMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""c5c18ef8-86ff-4a4c-aaa8-8a47b7590cd4"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl LMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""f732fdfa-2311-4dce-bf57-934c9ee857d8"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl RMB"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""caa2a249-ca59-4349-ad92-c08a0965b29d"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl RMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""47dffaa7-52c4-4713-898d-7b2e61443c0b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ctrl RMB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Survivor"",
            ""id"": ""5099cd33-2d13-4876-92a2-34fd78f0c0a5"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""72938724-e4c3-4702-aa21-5240b7a6aa7c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera"",
                    ""type"": ""PassThrough"",
                    ""id"": ""61c3a70e-68ea-4ce3-b203-4fd128c11e2d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""f411bafe-6630-4353-a27a-980dcbc3d809"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f24f344d-813b-41aa-8679-667289bff570"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ad7e8338-5a4e-4c5f-a72e-032881837790"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8a004cef-c01e-4173-98cc-6d46acc17b89"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f962f614-406d-4bea-a58c-c41b6a529031"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""055e9154-8cc6-491f-a516-1d5b134ea694"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a11fc30e-a95f-46d8-93e4-1802c33f2066"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9d7a9463-9386-4f22-9ff6-a0ae13895675"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Game Actions"",
            ""bindingGroup"": ""Game Actions"",
            ""devices"": []
        }
    ]
}");
        // Master
        m_Master = asset.FindActionMap("Master", throwIfNotFound: true);
        m_Master_LMB = m_Master.FindAction("LMB", throwIfNotFound: true);
        m_Master_RMB = m_Master.FindAction("RMB", throwIfNotFound: true);
        m_Master_ShiftLMB = m_Master.FindAction("Shift LMB", throwIfNotFound: true);
        m_Master_ShiftRMB = m_Master.FindAction("Shift RMB", throwIfNotFound: true);
        m_Master_CtrlLMB = m_Master.FindAction("Ctrl LMB", throwIfNotFound: true);
        m_Master_CtrlRMB = m_Master.FindAction("Ctrl RMB", throwIfNotFound: true);
        m_Master_FloorUp = m_Master.FindAction("Floor Up", throwIfNotFound: true);
        m_Master_FloorDown = m_Master.FindAction("Floor Down", throwIfNotFound: true);
        // Survivor
        m_Survivor = asset.FindActionMap("Survivor", throwIfNotFound: true);
        m_Survivor_Movement = m_Survivor.FindAction("Movement", throwIfNotFound: true);
        m_Survivor_Camera = m_Survivor.FindAction("Camera", throwIfNotFound: true);
        m_Survivor_Interact = m_Survivor.FindAction("Interact", throwIfNotFound: true);
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

    // Master
    private readonly InputActionMap m_Master;
    private IMasterActions m_MasterActionsCallbackInterface;
    private readonly InputAction m_Master_LMB;
    private readonly InputAction m_Master_RMB;
    private readonly InputAction m_Master_ShiftLMB;
    private readonly InputAction m_Master_ShiftRMB;
    private readonly InputAction m_Master_CtrlLMB;
    private readonly InputAction m_Master_CtrlRMB;
    private readonly InputAction m_Master_FloorUp;
    private readonly InputAction m_Master_FloorDown;
    public struct MasterActions
    {
        private @Controls m_Wrapper;
        public MasterActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LMB => m_Wrapper.m_Master_LMB;
        public InputAction @RMB => m_Wrapper.m_Master_RMB;
        public InputAction @ShiftLMB => m_Wrapper.m_Master_ShiftLMB;
        public InputAction @ShiftRMB => m_Wrapper.m_Master_ShiftRMB;
        public InputAction @CtrlLMB => m_Wrapper.m_Master_CtrlLMB;
        public InputAction @CtrlRMB => m_Wrapper.m_Master_CtrlRMB;
        public InputAction @FloorUp => m_Wrapper.m_Master_FloorUp;
        public InputAction @FloorDown => m_Wrapper.m_Master_FloorDown;
        public InputActionMap Get() { return m_Wrapper.m_Master; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MasterActions set) { return set.Get(); }
        public void SetCallbacks(IMasterActions instance)
        {
            if (m_Wrapper.m_MasterActionsCallbackInterface != null)
            {
                @LMB.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnLMB;
                @LMB.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnLMB;
                @LMB.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnLMB;
                @RMB.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnRMB;
                @RMB.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnRMB;
                @RMB.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnRMB;
                @ShiftLMB.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftLMB;
                @ShiftLMB.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftLMB;
                @ShiftLMB.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftLMB;
                @ShiftRMB.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftRMB;
                @ShiftRMB.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftRMB;
                @ShiftRMB.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnShiftRMB;
                @CtrlLMB.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlLMB;
                @CtrlLMB.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlLMB;
                @CtrlLMB.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlLMB;
                @CtrlRMB.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlRMB;
                @CtrlRMB.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlRMB;
                @CtrlRMB.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnCtrlRMB;
                @FloorUp.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorUp;
                @FloorUp.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorUp;
                @FloorUp.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorUp;
                @FloorDown.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorDown;
                @FloorDown.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorDown;
                @FloorDown.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnFloorDown;
            }
            m_Wrapper.m_MasterActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LMB.started += instance.OnLMB;
                @LMB.performed += instance.OnLMB;
                @LMB.canceled += instance.OnLMB;
                @RMB.started += instance.OnRMB;
                @RMB.performed += instance.OnRMB;
                @RMB.canceled += instance.OnRMB;
                @ShiftLMB.started += instance.OnShiftLMB;
                @ShiftLMB.performed += instance.OnShiftLMB;
                @ShiftLMB.canceled += instance.OnShiftLMB;
                @ShiftRMB.started += instance.OnShiftRMB;
                @ShiftRMB.performed += instance.OnShiftRMB;
                @ShiftRMB.canceled += instance.OnShiftRMB;
                @CtrlLMB.started += instance.OnCtrlLMB;
                @CtrlLMB.performed += instance.OnCtrlLMB;
                @CtrlLMB.canceled += instance.OnCtrlLMB;
                @CtrlRMB.started += instance.OnCtrlRMB;
                @CtrlRMB.performed += instance.OnCtrlRMB;
                @CtrlRMB.canceled += instance.OnCtrlRMB;
                @FloorUp.started += instance.OnFloorUp;
                @FloorUp.performed += instance.OnFloorUp;
                @FloorUp.canceled += instance.OnFloorUp;
                @FloorDown.started += instance.OnFloorDown;
                @FloorDown.performed += instance.OnFloorDown;
                @FloorDown.canceled += instance.OnFloorDown;
            }
        }
    }
    public MasterActions @Master => new MasterActions(this);

    // Survivor
    private readonly InputActionMap m_Survivor;
    private ISurvivorActions m_SurvivorActionsCallbackInterface;
    private readonly InputAction m_Survivor_Movement;
    private readonly InputAction m_Survivor_Camera;
    private readonly InputAction m_Survivor_Interact;
    public struct SurvivorActions
    {
        private @Controls m_Wrapper;
        public SurvivorActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Survivor_Movement;
        public InputAction @Camera => m_Wrapper.m_Survivor_Camera;
        public InputAction @Interact => m_Wrapper.m_Survivor_Interact;
        public InputActionMap Get() { return m_Wrapper.m_Survivor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SurvivorActions set) { return set.Get(); }
        public void SetCallbacks(ISurvivorActions instance)
        {
            if (m_Wrapper.m_SurvivorActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnMovement;
                @Camera.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnCamera;
                @Interact.started -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_SurvivorActionsCallbackInterface.OnInteract;
            }
            m_Wrapper.m_SurvivorActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
            }
        }
    }
    public SurvivorActions @Survivor => new SurvivorActions(this);
    private int m_GameActionsSchemeIndex = -1;
    public InputControlScheme GameActionsScheme
    {
        get
        {
            if (m_GameActionsSchemeIndex == -1) m_GameActionsSchemeIndex = asset.FindControlSchemeIndex("Game Actions");
            return asset.controlSchemes[m_GameActionsSchemeIndex];
        }
    }
    public interface IMasterActions
    {
        void OnLMB(InputAction.CallbackContext context);
        void OnRMB(InputAction.CallbackContext context);
        void OnShiftLMB(InputAction.CallbackContext context);
        void OnShiftRMB(InputAction.CallbackContext context);
        void OnCtrlLMB(InputAction.CallbackContext context);
        void OnCtrlRMB(InputAction.CallbackContext context);
        void OnFloorUp(InputAction.CallbackContext context);
        void OnFloorDown(InputAction.CallbackContext context);
    }
    public interface ISurvivorActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
    }
}
