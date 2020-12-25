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
                    ""name"": ""Main"",
                    ""type"": ""Button"",
                    ""id"": ""2b33c84b-11c8-4445-8f33-024d8771e93d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Alt"",
                    ""type"": ""Button"",
                    ""id"": ""77365dda-ee6e-4d1a-9bc4-c96bbbe09d14"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Change Floor Up"",
                    ""type"": ""Button"",
                    ""id"": ""968763e4-0606-4865-b387-a5746d12c85d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Change Floor Down"",
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
                    ""action"": ""Main"",
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
                    ""action"": ""Alt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""78fa70b8-6f19-4cc5-8bf4-6b46538b8e0e"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""Change Floor Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da4b7523-adf5-4e7b-8869-8f7e066591c3"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Game Actions"",
                    ""action"": ""Change Floor Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
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
        m_Master_Main = m_Master.FindAction("Main", throwIfNotFound: true);
        m_Master_Alt = m_Master.FindAction("Alt", throwIfNotFound: true);
        m_Master_ChangeFloorUp = m_Master.FindAction("Change Floor Up", throwIfNotFound: true);
        m_Master_ChangeFloorDown = m_Master.FindAction("Change Floor Down", throwIfNotFound: true);
        // Survivor
        m_Survivor = asset.FindActionMap("Survivor", throwIfNotFound: true);
        m_Survivor_Movement = m_Survivor.FindAction("Movement", throwIfNotFound: true);
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
    private readonly InputAction m_Master_Main;
    private readonly InputAction m_Master_Alt;
    private readonly InputAction m_Master_ChangeFloorUp;
    private readonly InputAction m_Master_ChangeFloorDown;
    public struct MasterActions
    {
        private @Controls m_Wrapper;
        public MasterActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Main => m_Wrapper.m_Master_Main;
        public InputAction @Alt => m_Wrapper.m_Master_Alt;
        public InputAction @ChangeFloorUp => m_Wrapper.m_Master_ChangeFloorUp;
        public InputAction @ChangeFloorDown => m_Wrapper.m_Master_ChangeFloorDown;
        public InputActionMap Get() { return m_Wrapper.m_Master; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MasterActions set) { return set.Get(); }
        public void SetCallbacks(IMasterActions instance)
        {
            if (m_Wrapper.m_MasterActionsCallbackInterface != null)
            {
                @Main.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnMain;
                @Main.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnMain;
                @Main.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnMain;
                @Alt.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnAlt;
                @Alt.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnAlt;
                @Alt.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnAlt;
                @ChangeFloorUp.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnChangeFloorUp;
                @ChangeFloorUp.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnChangeFloorUp;
                @ChangeFloorUp.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnChangeFloorUp;
                @ChangeFloorDown.started -= m_Wrapper.m_MasterActionsCallbackInterface.OnChangeFloorDown;
                @ChangeFloorDown.performed -= m_Wrapper.m_MasterActionsCallbackInterface.OnChangeFloorDown;
                @ChangeFloorDown.canceled -= m_Wrapper.m_MasterActionsCallbackInterface.OnChangeFloorDown;
            }
            m_Wrapper.m_MasterActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Main.started += instance.OnMain;
                @Main.performed += instance.OnMain;
                @Main.canceled += instance.OnMain;
                @Alt.started += instance.OnAlt;
                @Alt.performed += instance.OnAlt;
                @Alt.canceled += instance.OnAlt;
                @ChangeFloorUp.started += instance.OnChangeFloorUp;
                @ChangeFloorUp.performed += instance.OnChangeFloorUp;
                @ChangeFloorUp.canceled += instance.OnChangeFloorUp;
                @ChangeFloorDown.started += instance.OnChangeFloorDown;
                @ChangeFloorDown.performed += instance.OnChangeFloorDown;
                @ChangeFloorDown.canceled += instance.OnChangeFloorDown;
            }
        }
    }
    public MasterActions @Master => new MasterActions(this);

    // Survivor
    private readonly InputActionMap m_Survivor;
    private ISurvivorActions m_SurvivorActionsCallbackInterface;
    private readonly InputAction m_Survivor_Movement;
    public struct SurvivorActions
    {
        private @Controls m_Wrapper;
        public SurvivorActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Survivor_Movement;
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
            }
            m_Wrapper.m_SurvivorActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
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
        void OnMain(InputAction.CallbackContext context);
        void OnAlt(InputAction.CallbackContext context);
        void OnChangeFloorUp(InputAction.CallbackContext context);
        void OnChangeFloorDown(InputAction.CallbackContext context);
    }
    public interface ISurvivorActions
    {
        void OnMovement(InputAction.CallbackContext context);
    }
}
