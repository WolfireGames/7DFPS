// GENERATED AUTOMATICALLY FROM 'Assets/GunInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GunInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GunInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GunInputs"",
    ""maps"": [
        {
            ""name"": ""main"",
            ""id"": ""3a569327-08a2-4d14-9ae4-bc8dfadb4b06"",
            ""actions"": [
                {
                    ""name"": ""Trigger"",
                    ""type"": ""Button"",
                    ""id"": ""79979f87-f351-46d8-b383-854942e05124"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Hammer"",
                    ""type"": ""Button"",
                    ""id"": ""7f505a03-cf55-4e98-ae1f-5d87cf12d5ed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SlideLock"",
                    ""type"": ""Button"",
                    ""id"": ""46921d76-33df-4293-a1ec-9c393cc3bdb2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Safety"",
                    ""type"": ""Button"",
                    ""id"": ""afb8de5c-5c19-4e78-bc28-393b856c4765"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""FireSelector"",
                    ""type"": ""Button"",
                    ""id"": ""7954041c-84b9-4619-a054-921b355af3f6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PullSlide"",
                    ""type"": ""Button"",
                    ""id"": ""00ca8563-88b4-4e22-9af0-554bd06b0819"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwingOutCylinder"",
                    ""type"": ""Button"",
                    ""id"": ""7effeeeb-a628-424c-9233-898d68c7965d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CloseCylinder"",
                    ""type"": ""Button"",
                    ""id"": ""d1030576-ddb8-48a6-9dce-4e39445bc980"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ExtractorRod"",
                    ""type"": ""Button"",
                    ""id"": ""e01e1e85-0e34-4f91-ae3e-bec813a10f0f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpinCylinder"",
                    ""type"": ""Value"",
                    ""id"": ""049796db-3e74-4709-8825-d24e1df81950"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleStance"",
                    ""type"": ""Button"",
                    ""id"": ""6d35e334-2108-49d0-ab5a-8e16368a0655"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BoltLock"",
                    ""type"": ""Button"",
                    ""id"": ""10830232-f342-42d3-a619-85c34e1f12cd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7f18fcfd-980c-41d7-8415-faefa919a168"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""Trigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b3b8cae0-5cf7-4365-9528-57c7ea36764a"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""Hammer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c78b326-8c7b-4324-a328-a7a5dafebce9"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""SlideLock"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""838e8a5b-6bc4-41cd-9ccf-5afb2b944e4c"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""Safety"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""60f1a7c9-8fdd-42ba-b413-5284a7b1f888"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""FireSelector"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46839fc6-e415-42ae-aa03-4defa1d3d332"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""PullSlide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""909c70cb-94f5-4b8d-9fcf-78d2bd5d863d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""SwingOutCylinder"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ead95101-2248-4400-b02e-0ae707476305"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""CloseCylinder"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""954734cd-0fbd-4478-ac53-ae7d6f54c1f7"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""ExtractorRod"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""31f57787-46d6-4850-95cd-00ab7f327f8b"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""SpinCylinder"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de1ab855-ae98-4559-8fcd-8711f4ea7b70"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""ToggleStance"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fada1905-fb25-40ef-92e3-0626bb8ae3d3"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""BoltLock"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse & Keyboard"",
            ""bindingGroup"": ""Mouse & Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // main
        m_main = asset.FindActionMap("main", throwIfNotFound: true);
        m_main_Trigger = m_main.FindAction("Trigger", throwIfNotFound: true);
        m_main_Hammer = m_main.FindAction("Hammer", throwIfNotFound: true);
        m_main_SlideLock = m_main.FindAction("SlideLock", throwIfNotFound: true);
        m_main_Safety = m_main.FindAction("Safety", throwIfNotFound: true);
        m_main_FireSelector = m_main.FindAction("FireSelector", throwIfNotFound: true);
        m_main_PullSlide = m_main.FindAction("PullSlide", throwIfNotFound: true);
        m_main_SwingOutCylinder = m_main.FindAction("SwingOutCylinder", throwIfNotFound: true);
        m_main_CloseCylinder = m_main.FindAction("CloseCylinder", throwIfNotFound: true);
        m_main_ExtractorRod = m_main.FindAction("ExtractorRod", throwIfNotFound: true);
        m_main_SpinCylinder = m_main.FindAction("SpinCylinder", throwIfNotFound: true);
        m_main_ToggleStance = m_main.FindAction("ToggleStance", throwIfNotFound: true);
        m_main_BoltLock = m_main.FindAction("BoltLock", throwIfNotFound: true);
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

    // main
    private readonly InputActionMap m_main;
    private IMainActions m_MainActionsCallbackInterface;
    private readonly InputAction m_main_Trigger;
    private readonly InputAction m_main_Hammer;
    private readonly InputAction m_main_SlideLock;
    private readonly InputAction m_main_Safety;
    private readonly InputAction m_main_FireSelector;
    private readonly InputAction m_main_PullSlide;
    private readonly InputAction m_main_SwingOutCylinder;
    private readonly InputAction m_main_CloseCylinder;
    private readonly InputAction m_main_ExtractorRod;
    private readonly InputAction m_main_SpinCylinder;
    private readonly InputAction m_main_ToggleStance;
    private readonly InputAction m_main_BoltLock;
    public struct MainActions
    {
        private @GunInputs m_Wrapper;
        public MainActions(@GunInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Trigger => m_Wrapper.m_main_Trigger;
        public InputAction @Hammer => m_Wrapper.m_main_Hammer;
        public InputAction @SlideLock => m_Wrapper.m_main_SlideLock;
        public InputAction @Safety => m_Wrapper.m_main_Safety;
        public InputAction @FireSelector => m_Wrapper.m_main_FireSelector;
        public InputAction @PullSlide => m_Wrapper.m_main_PullSlide;
        public InputAction @SwingOutCylinder => m_Wrapper.m_main_SwingOutCylinder;
        public InputAction @CloseCylinder => m_Wrapper.m_main_CloseCylinder;
        public InputAction @ExtractorRod => m_Wrapper.m_main_ExtractorRod;
        public InputAction @SpinCylinder => m_Wrapper.m_main_SpinCylinder;
        public InputAction @ToggleStance => m_Wrapper.m_main_ToggleStance;
        public InputAction @BoltLock => m_Wrapper.m_main_BoltLock;
        public InputActionMap Get() { return m_Wrapper.m_main; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MainActions set) { return set.Get(); }
        public void SetCallbacks(IMainActions instance)
        {
            if (m_Wrapper.m_MainActionsCallbackInterface != null)
            {
                @Trigger.started -= m_Wrapper.m_MainActionsCallbackInterface.OnTrigger;
                @Trigger.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnTrigger;
                @Trigger.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnTrigger;
                @Hammer.started -= m_Wrapper.m_MainActionsCallbackInterface.OnHammer;
                @Hammer.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnHammer;
                @Hammer.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnHammer;
                @SlideLock.started -= m_Wrapper.m_MainActionsCallbackInterface.OnSlideLock;
                @SlideLock.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnSlideLock;
                @SlideLock.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnSlideLock;
                @Safety.started -= m_Wrapper.m_MainActionsCallbackInterface.OnSafety;
                @Safety.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnSafety;
                @Safety.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnSafety;
                @FireSelector.started -= m_Wrapper.m_MainActionsCallbackInterface.OnFireSelector;
                @FireSelector.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnFireSelector;
                @FireSelector.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnFireSelector;
                @PullSlide.started -= m_Wrapper.m_MainActionsCallbackInterface.OnPullSlide;
                @PullSlide.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnPullSlide;
                @PullSlide.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnPullSlide;
                @SwingOutCylinder.started -= m_Wrapper.m_MainActionsCallbackInterface.OnSwingOutCylinder;
                @SwingOutCylinder.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnSwingOutCylinder;
                @SwingOutCylinder.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnSwingOutCylinder;
                @CloseCylinder.started -= m_Wrapper.m_MainActionsCallbackInterface.OnCloseCylinder;
                @CloseCylinder.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnCloseCylinder;
                @CloseCylinder.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnCloseCylinder;
                @ExtractorRod.started -= m_Wrapper.m_MainActionsCallbackInterface.OnExtractorRod;
                @ExtractorRod.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnExtractorRod;
                @ExtractorRod.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnExtractorRod;
                @SpinCylinder.started -= m_Wrapper.m_MainActionsCallbackInterface.OnSpinCylinder;
                @SpinCylinder.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnSpinCylinder;
                @SpinCylinder.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnSpinCylinder;
                @ToggleStance.started -= m_Wrapper.m_MainActionsCallbackInterface.OnToggleStance;
                @ToggleStance.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnToggleStance;
                @ToggleStance.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnToggleStance;
                @BoltLock.started -= m_Wrapper.m_MainActionsCallbackInterface.OnBoltLock;
                @BoltLock.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnBoltLock;
                @BoltLock.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnBoltLock;
            }
            m_Wrapper.m_MainActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Trigger.started += instance.OnTrigger;
                @Trigger.performed += instance.OnTrigger;
                @Trigger.canceled += instance.OnTrigger;
                @Hammer.started += instance.OnHammer;
                @Hammer.performed += instance.OnHammer;
                @Hammer.canceled += instance.OnHammer;
                @SlideLock.started += instance.OnSlideLock;
                @SlideLock.performed += instance.OnSlideLock;
                @SlideLock.canceled += instance.OnSlideLock;
                @Safety.started += instance.OnSafety;
                @Safety.performed += instance.OnSafety;
                @Safety.canceled += instance.OnSafety;
                @FireSelector.started += instance.OnFireSelector;
                @FireSelector.performed += instance.OnFireSelector;
                @FireSelector.canceled += instance.OnFireSelector;
                @PullSlide.started += instance.OnPullSlide;
                @PullSlide.performed += instance.OnPullSlide;
                @PullSlide.canceled += instance.OnPullSlide;
                @SwingOutCylinder.started += instance.OnSwingOutCylinder;
                @SwingOutCylinder.performed += instance.OnSwingOutCylinder;
                @SwingOutCylinder.canceled += instance.OnSwingOutCylinder;
                @CloseCylinder.started += instance.OnCloseCylinder;
                @CloseCylinder.performed += instance.OnCloseCylinder;
                @CloseCylinder.canceled += instance.OnCloseCylinder;
                @ExtractorRod.started += instance.OnExtractorRod;
                @ExtractorRod.performed += instance.OnExtractorRod;
                @ExtractorRod.canceled += instance.OnExtractorRod;
                @SpinCylinder.started += instance.OnSpinCylinder;
                @SpinCylinder.performed += instance.OnSpinCylinder;
                @SpinCylinder.canceled += instance.OnSpinCylinder;
                @ToggleStance.started += instance.OnToggleStance;
                @ToggleStance.performed += instance.OnToggleStance;
                @ToggleStance.canceled += instance.OnToggleStance;
                @BoltLock.started += instance.OnBoltLock;
                @BoltLock.performed += instance.OnBoltLock;
                @BoltLock.canceled += instance.OnBoltLock;
            }
        }
    }
    public MainActions @main => new MainActions(this);
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse & Keyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    public interface IMainActions
    {
        void OnTrigger(InputAction.CallbackContext context);
        void OnHammer(InputAction.CallbackContext context);
        void OnSlideLock(InputAction.CallbackContext context);
        void OnSafety(InputAction.CallbackContext context);
        void OnFireSelector(InputAction.CallbackContext context);
        void OnPullSlide(InputAction.CallbackContext context);
        void OnSwingOutCylinder(InputAction.CallbackContext context);
        void OnCloseCylinder(InputAction.CallbackContext context);
        void OnExtractorRod(InputAction.CallbackContext context);
        void OnSpinCylinder(InputAction.CallbackContext context);
        void OnToggleStance(InputAction.CallbackContext context);
        void OnBoltLock(InputAction.CallbackContext context);
    }
}
