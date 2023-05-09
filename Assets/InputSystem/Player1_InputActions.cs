//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.0
//     from Assets/InputSystem/Player1_InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Player1_InputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Player1_InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player1_InputActions"",
    ""maps"": [
        {
            ""name"": ""PlayerActions"",
            ""id"": ""caf5164b-7abb-470a-b498-34ae958f8603"",
            ""actions"": [
                {
                    ""name"": ""TargetNavigation - UP"",
                    ""type"": ""Button"",
                    ""id"": ""7c9349fe-1ced-4fdd-b945-5985d4b6d59f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TargetNavigation - DOWN"",
                    ""type"": ""Button"",
                    ""id"": ""90fb1b5d-8327-439e-ae5a-d960eb38606a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Menus_Confirm"",
                    ""type"": ""Button"",
                    ""id"": ""f6e8cbaf-d49b-4bf3-810f-920f4ad26d4e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Menus_Back"",
                    ""type"": ""Button"",
                    ""id"": ""9be45c6c-6f39-4159-91ff-b288738e9a93"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwapActiveCharacter"",
                    ""type"": ""Button"",
                    ""id"": ""8c21546b-a912-4ac7-a018-66be7f06d91f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PauseGame"",
                    ""type"": ""Button"",
                    ""id"": ""7673323c-c2b9-4818-ba57-91d447739c1f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""984f5b1b-5324-4a72-9d26-2c861752e5d0"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""Menus_Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3676baff-9cae-4cb0-9c7d-89551074c544"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""Menus_Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5193bb67-e9dc-48ec-a522-b1d6ecba51ca"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard"",
                    ""action"": ""Menus_Confirm"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a5e4d760-31a8-4907-ac8c-4af5a23f43ff"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""Menus_Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b8a3f4a-c93e-4d47-a59b-366c2671fde0"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""Menus_Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa4edc58-8ec6-43b7-8507-414b23339fa9"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""Menus_Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54b58090-cd0c-4708-b40c-838a3943f956"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard"",
                    ""action"": ""Menus_Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e4c223e1-1911-43c4-8d37-48e408ed1d2b"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""SwapActiveCharacter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ed91cf5-7ec6-4361-a14d-2af54a3329f6"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""SwapActiveCharacter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""30ad86e3-dd4d-4b70-a586-c628b989fe90"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""SwapActiveCharacter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cf3ffd64-1733-49ec-98cc-79800387d9ab"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""SwapActiveCharacter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dadd1310-c782-4b48-912b-3349a723d001"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""SwapActiveCharacter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""063c71ec-79da-41cd-a68a-94cf084775ad"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard"",
                    ""action"": ""SwapActiveCharacter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0de976ff-1d85-4bf3-8102-fdb387a0e140"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard"",
                    ""action"": ""SwapActiveCharacter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e57da5d6-d411-458e-bfa2-75ddfb0346f8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""TargetNavigation - UP"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7aeceeb4-b9fb-46f6-8373-87e7b93be8d6"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""TargetNavigation - UP"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""744bbef1-11ec-4171-a5ec-afe8709df5c3"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard"",
                    ""action"": ""TargetNavigation - UP"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""18e40f23-b4a1-4436-b509-3fe06e043bc0"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""TargetNavigation - DOWN"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c05462c-7a68-448a-ad2d-ff8550e4d404"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""TargetNavigation - DOWN"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e3ff0527-6f1f-4e07-b41b-0f8a6dad4556"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard"",
                    ""action"": ""TargetNavigation - DOWN"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d251ce24-bf8e-453c-9080-050adc2fc454"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""PauseGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eeff4ac7-4500-4d29-b1c5-1d3f871536f6"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard"",
                    ""action"": ""PauseGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""StartMenuActions"",
            ""id"": ""b7f9686f-45fe-4744-8951-1420a4c3fb07"",
            ""actions"": [
                {
                    ""name"": ""Menus_Back"",
                    ""type"": ""Button"",
                    ""id"": ""df9ef59c-ff43-4612-a23b-0411003d2b9f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""QuitToDesktop"",
                    ""type"": ""Button"",
                    ""id"": ""26334429-60e0-4358-a622-a5322019b8f8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AnyPositiveKey"",
                    ""type"": ""Button"",
                    ""id"": ""88016c5f-5d11-4702-9482-56cf55ae72cd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d54395a8-6d5b-40ae-9839-653c035654b3"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""Menus_Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""94849eba-1f5b-48f6-b388-ec2e380c1a1b"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard"",
                    ""action"": ""Menus_Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e8cf89b8-0eaf-4735-86db-e6c36c32c805"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""QuitToDesktop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""accc93ca-67f7-4b84-bd13-3d6ef360a2cf"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""QuitToDesktop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9d55eb4d-8815-4c82-a89f-5046a03819a9"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""AnyPositiveKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff2f0db4-fafb-411c-b635-6b39e8a01a04"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""AnyPositiveKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e24a736a-9867-4f0e-b5c5-fca291cee211"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""AnyPositiveKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb682453-078d-4bc6-a2df-785ebb3abfd8"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""AnyPositiveKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""677cf920-bfcd-47cd-bfc5-c1550891e170"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard;Gamepad"",
                    ""action"": ""AnyPositiveKey"",
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
        // PlayerActions
        m_PlayerActions = asset.FindActionMap("PlayerActions", throwIfNotFound: true);
        m_PlayerActions_TargetNavigationUP = m_PlayerActions.FindAction("TargetNavigation - UP", throwIfNotFound: true);
        m_PlayerActions_TargetNavigationDOWN = m_PlayerActions.FindAction("TargetNavigation - DOWN", throwIfNotFound: true);
        m_PlayerActions_Menus_Confirm = m_PlayerActions.FindAction("Menus_Confirm", throwIfNotFound: true);
        m_PlayerActions_Menus_Back = m_PlayerActions.FindAction("Menus_Back", throwIfNotFound: true);
        m_PlayerActions_SwapActiveCharacter = m_PlayerActions.FindAction("SwapActiveCharacter", throwIfNotFound: true);
        m_PlayerActions_PauseGame = m_PlayerActions.FindAction("PauseGame", throwIfNotFound: true);
        // StartMenuActions
        m_StartMenuActions = asset.FindActionMap("StartMenuActions", throwIfNotFound: true);
        m_StartMenuActions_Menus_Back = m_StartMenuActions.FindAction("Menus_Back", throwIfNotFound: true);
        m_StartMenuActions_QuitToDesktop = m_StartMenuActions.FindAction("QuitToDesktop", throwIfNotFound: true);
        m_StartMenuActions_AnyPositiveKey = m_StartMenuActions.FindAction("AnyPositiveKey", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerActions
    private readonly InputActionMap m_PlayerActions;
    private List<IPlayerActionsActions> m_PlayerActionsActionsCallbackInterfaces = new List<IPlayerActionsActions>();
    private readonly InputAction m_PlayerActions_TargetNavigationUP;
    private readonly InputAction m_PlayerActions_TargetNavigationDOWN;
    private readonly InputAction m_PlayerActions_Menus_Confirm;
    private readonly InputAction m_PlayerActions_Menus_Back;
    private readonly InputAction m_PlayerActions_SwapActiveCharacter;
    private readonly InputAction m_PlayerActions_PauseGame;
    public struct PlayerActionsActions
    {
        private @Player1_InputActions m_Wrapper;
        public PlayerActionsActions(@Player1_InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @TargetNavigationUP => m_Wrapper.m_PlayerActions_TargetNavigationUP;
        public InputAction @TargetNavigationDOWN => m_Wrapper.m_PlayerActions_TargetNavigationDOWN;
        public InputAction @Menus_Confirm => m_Wrapper.m_PlayerActions_Menus_Confirm;
        public InputAction @Menus_Back => m_Wrapper.m_PlayerActions_Menus_Back;
        public InputAction @SwapActiveCharacter => m_Wrapper.m_PlayerActions_SwapActiveCharacter;
        public InputAction @PauseGame => m_Wrapper.m_PlayerActions_PauseGame;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionsActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Add(instance);
            @TargetNavigationUP.started += instance.OnTargetNavigationUP;
            @TargetNavigationUP.performed += instance.OnTargetNavigationUP;
            @TargetNavigationUP.canceled += instance.OnTargetNavigationUP;
            @TargetNavigationDOWN.started += instance.OnTargetNavigationDOWN;
            @TargetNavigationDOWN.performed += instance.OnTargetNavigationDOWN;
            @TargetNavigationDOWN.canceled += instance.OnTargetNavigationDOWN;
            @Menus_Confirm.started += instance.OnMenus_Confirm;
            @Menus_Confirm.performed += instance.OnMenus_Confirm;
            @Menus_Confirm.canceled += instance.OnMenus_Confirm;
            @Menus_Back.started += instance.OnMenus_Back;
            @Menus_Back.performed += instance.OnMenus_Back;
            @Menus_Back.canceled += instance.OnMenus_Back;
            @SwapActiveCharacter.started += instance.OnSwapActiveCharacter;
            @SwapActiveCharacter.performed += instance.OnSwapActiveCharacter;
            @SwapActiveCharacter.canceled += instance.OnSwapActiveCharacter;
            @PauseGame.started += instance.OnPauseGame;
            @PauseGame.performed += instance.OnPauseGame;
            @PauseGame.canceled += instance.OnPauseGame;
        }

        private void UnregisterCallbacks(IPlayerActionsActions instance)
        {
            @TargetNavigationUP.started -= instance.OnTargetNavigationUP;
            @TargetNavigationUP.performed -= instance.OnTargetNavigationUP;
            @TargetNavigationUP.canceled -= instance.OnTargetNavigationUP;
            @TargetNavigationDOWN.started -= instance.OnTargetNavigationDOWN;
            @TargetNavigationDOWN.performed -= instance.OnTargetNavigationDOWN;
            @TargetNavigationDOWN.canceled -= instance.OnTargetNavigationDOWN;
            @Menus_Confirm.started -= instance.OnMenus_Confirm;
            @Menus_Confirm.performed -= instance.OnMenus_Confirm;
            @Menus_Confirm.canceled -= instance.OnMenus_Confirm;
            @Menus_Back.started -= instance.OnMenus_Back;
            @Menus_Back.performed -= instance.OnMenus_Back;
            @Menus_Back.canceled -= instance.OnMenus_Back;
            @SwapActiveCharacter.started -= instance.OnSwapActiveCharacter;
            @SwapActiveCharacter.performed -= instance.OnSwapActiveCharacter;
            @SwapActiveCharacter.canceled -= instance.OnSwapActiveCharacter;
            @PauseGame.started -= instance.OnPauseGame;
            @PauseGame.performed -= instance.OnPauseGame;
            @PauseGame.canceled -= instance.OnPauseGame;
        }

        public void RemoveCallbacks(IPlayerActionsActions instance)
        {
            if (m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActionsActions @PlayerActions => new PlayerActionsActions(this);

    // StartMenuActions
    private readonly InputActionMap m_StartMenuActions;
    private List<IStartMenuActionsActions> m_StartMenuActionsActionsCallbackInterfaces = new List<IStartMenuActionsActions>();
    private readonly InputAction m_StartMenuActions_Menus_Back;
    private readonly InputAction m_StartMenuActions_QuitToDesktop;
    private readonly InputAction m_StartMenuActions_AnyPositiveKey;
    public struct StartMenuActionsActions
    {
        private @Player1_InputActions m_Wrapper;
        public StartMenuActionsActions(@Player1_InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Menus_Back => m_Wrapper.m_StartMenuActions_Menus_Back;
        public InputAction @QuitToDesktop => m_Wrapper.m_StartMenuActions_QuitToDesktop;
        public InputAction @AnyPositiveKey => m_Wrapper.m_StartMenuActions_AnyPositiveKey;
        public InputActionMap Get() { return m_Wrapper.m_StartMenuActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(StartMenuActionsActions set) { return set.Get(); }
        public void AddCallbacks(IStartMenuActionsActions instance)
        {
            if (instance == null || m_Wrapper.m_StartMenuActionsActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_StartMenuActionsActionsCallbackInterfaces.Add(instance);
            @Menus_Back.started += instance.OnMenus_Back;
            @Menus_Back.performed += instance.OnMenus_Back;
            @Menus_Back.canceled += instance.OnMenus_Back;
            @QuitToDesktop.started += instance.OnQuitToDesktop;
            @QuitToDesktop.performed += instance.OnQuitToDesktop;
            @QuitToDesktop.canceled += instance.OnQuitToDesktop;
            @AnyPositiveKey.started += instance.OnAnyPositiveKey;
            @AnyPositiveKey.performed += instance.OnAnyPositiveKey;
            @AnyPositiveKey.canceled += instance.OnAnyPositiveKey;
        }

        private void UnregisterCallbacks(IStartMenuActionsActions instance)
        {
            @Menus_Back.started -= instance.OnMenus_Back;
            @Menus_Back.performed -= instance.OnMenus_Back;
            @Menus_Back.canceled -= instance.OnMenus_Back;
            @QuitToDesktop.started -= instance.OnQuitToDesktop;
            @QuitToDesktop.performed -= instance.OnQuitToDesktop;
            @QuitToDesktop.canceled -= instance.OnQuitToDesktop;
            @AnyPositiveKey.started -= instance.OnAnyPositiveKey;
            @AnyPositiveKey.performed -= instance.OnAnyPositiveKey;
            @AnyPositiveKey.canceled -= instance.OnAnyPositiveKey;
        }

        public void RemoveCallbacks(IStartMenuActionsActions instance)
        {
            if (m_Wrapper.m_StartMenuActionsActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IStartMenuActionsActions instance)
        {
            foreach (var item in m_Wrapper.m_StartMenuActionsActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_StartMenuActionsActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public StartMenuActionsActions @StartMenuActions => new StartMenuActionsActions(this);
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
    public interface IPlayerActionsActions
    {
        void OnTargetNavigationUP(InputAction.CallbackContext context);
        void OnTargetNavigationDOWN(InputAction.CallbackContext context);
        void OnMenus_Confirm(InputAction.CallbackContext context);
        void OnMenus_Back(InputAction.CallbackContext context);
        void OnSwapActiveCharacter(InputAction.CallbackContext context);
        void OnPauseGame(InputAction.CallbackContext context);
    }
    public interface IStartMenuActionsActions
    {
        void OnMenus_Back(InputAction.CallbackContext context);
        void OnQuitToDesktop(InputAction.CallbackContext context);
        void OnAnyPositiveKey(InputAction.CallbackContext context);
    }
}
