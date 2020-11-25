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
            ""name"": ""Gun"",
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
                    ""name"": ""Slide Lock"",
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
                    ""name"": ""Fire Selector"",
                    ""type"": ""Button"",
                    ""id"": ""7954041c-84b9-4619-a054-921b355af3f6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pull Slide"",
                    ""type"": ""Button"",
                    ""id"": ""00ca8563-88b4-4e22-9af0-554bd06b0819"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Swing Out Cylinder"",
                    ""type"": ""Button"",
                    ""id"": ""7effeeeb-a628-424c-9233-898d68c7965d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Close Cylinder"",
                    ""type"": ""Button"",
                    ""id"": ""d1030576-ddb8-48a6-9dce-4e39445bc980"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Extractor Rod"",
                    ""type"": ""Button"",
                    ""id"": ""e01e1e85-0e34-4f91-ae3e-bec813a10f0f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Spin Cylinder"",
                    ""type"": ""Value"",
                    ""id"": ""049796db-3e74-4709-8825-d24e1df81950"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Toggle Stance"",
                    ""type"": ""Button"",
                    ""id"": ""6d35e334-2108-49d0-ab5a-8e16368a0655"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Insert Round"",
                    ""type"": ""Button"",
                    ""id"": ""73837fd4-67b8-4c6d-a914-a5344f0cb47a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Bolt Lock"",
                    ""type"": ""Button"",
                    ""id"": ""10830232-f342-42d3-a619-85c34e1f12cd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Eject"",
                    ""type"": ""Button"",
                    ""id"": ""485c4d1d-5351-4437-af2e-c40a9f675c0c"",
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
                    ""action"": ""Slide Lock"",
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
                    ""action"": ""Fire Selector"",
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
                    ""action"": ""Pull Slide"",
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
                    ""action"": ""Swing Out Cylinder"",
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
                    ""action"": ""Close Cylinder"",
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
                    ""action"": ""Extractor Rod"",
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
                    ""action"": ""Spin Cylinder"",
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
                    ""action"": ""Toggle Stance"",
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
                    ""action"": ""Bolt Lock"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""04175357-ad5b-4c6b-8466-d753c6696663"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Eject"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ada938ba-9a57-40f3-89e7-7b757b31fc72"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse & Keyboard"",
                    ""action"": ""Insert Round"",
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
        // Gun
        m_Gun = asset.FindActionMap("Gun", throwIfNotFound: true);
        m_Gun_Trigger = m_Gun.FindAction("Trigger", throwIfNotFound: true);
        m_Gun_Hammer = m_Gun.FindAction("Hammer", throwIfNotFound: true);
        m_Gun_SlideLock = m_Gun.FindAction("Slide Lock", throwIfNotFound: true);
        m_Gun_Safety = m_Gun.FindAction("Safety", throwIfNotFound: true);
        m_Gun_FireSelector = m_Gun.FindAction("Fire Selector", throwIfNotFound: true);
        m_Gun_PullSlide = m_Gun.FindAction("Pull Slide", throwIfNotFound: true);
        m_Gun_SwingOutCylinder = m_Gun.FindAction("Swing Out Cylinder", throwIfNotFound: true);
        m_Gun_CloseCylinder = m_Gun.FindAction("Close Cylinder", throwIfNotFound: true);
        m_Gun_ExtractorRod = m_Gun.FindAction("Extractor Rod", throwIfNotFound: true);
        m_Gun_SpinCylinder = m_Gun.FindAction("Spin Cylinder", throwIfNotFound: true);
        m_Gun_ToggleStance = m_Gun.FindAction("Toggle Stance", throwIfNotFound: true);
        m_Gun_InsertRound = m_Gun.FindAction("Insert Round", throwIfNotFound: true);
        m_Gun_BoltLock = m_Gun.FindAction("Bolt Lock", throwIfNotFound: true);
        m_Gun_Eject = m_Gun.FindAction("Eject", throwIfNotFound: true);
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

    // Gun
    private readonly InputActionMap m_Gun;
    private IGunActions m_GunActionsCallbackInterface;
    private readonly InputAction m_Gun_Trigger;
    private readonly InputAction m_Gun_Hammer;
    private readonly InputAction m_Gun_SlideLock;
    private readonly InputAction m_Gun_Safety;
    private readonly InputAction m_Gun_FireSelector;
    private readonly InputAction m_Gun_PullSlide;
    private readonly InputAction m_Gun_SwingOutCylinder;
    private readonly InputAction m_Gun_CloseCylinder;
    private readonly InputAction m_Gun_ExtractorRod;
    private readonly InputAction m_Gun_SpinCylinder;
    private readonly InputAction m_Gun_ToggleStance;
    private readonly InputAction m_Gun_InsertRound;
    private readonly InputAction m_Gun_BoltLock;
    private readonly InputAction m_Gun_Eject;
    public struct GunActions
    {
        private @GunInputs m_Wrapper;
        public GunActions(@GunInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Trigger => m_Wrapper.m_Gun_Trigger;
        public InputAction @Hammer => m_Wrapper.m_Gun_Hammer;
        public InputAction @SlideLock => m_Wrapper.m_Gun_SlideLock;
        public InputAction @Safety => m_Wrapper.m_Gun_Safety;
        public InputAction @FireSelector => m_Wrapper.m_Gun_FireSelector;
        public InputAction @PullSlide => m_Wrapper.m_Gun_PullSlide;
        public InputAction @SwingOutCylinder => m_Wrapper.m_Gun_SwingOutCylinder;
        public InputAction @CloseCylinder => m_Wrapper.m_Gun_CloseCylinder;
        public InputAction @ExtractorRod => m_Wrapper.m_Gun_ExtractorRod;
        public InputAction @SpinCylinder => m_Wrapper.m_Gun_SpinCylinder;
        public InputAction @ToggleStance => m_Wrapper.m_Gun_ToggleStance;
        public InputAction @InsertRound => m_Wrapper.m_Gun_InsertRound;
        public InputAction @BoltLock => m_Wrapper.m_Gun_BoltLock;
        public InputAction @Eject => m_Wrapper.m_Gun_Eject;
        public InputActionMap Get() { return m_Wrapper.m_Gun; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GunActions set) { return set.Get(); }
        public void SetCallbacks(IGunActions instance)
        {
            if (m_Wrapper.m_GunActionsCallbackInterface != null)
            {
                @Trigger.started -= m_Wrapper.m_GunActionsCallbackInterface.OnTrigger;
                @Trigger.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnTrigger;
                @Trigger.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnTrigger;
                @Hammer.started -= m_Wrapper.m_GunActionsCallbackInterface.OnHammer;
                @Hammer.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnHammer;
                @Hammer.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnHammer;
                @SlideLock.started -= m_Wrapper.m_GunActionsCallbackInterface.OnSlideLock;
                @SlideLock.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnSlideLock;
                @SlideLock.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnSlideLock;
                @Safety.started -= m_Wrapper.m_GunActionsCallbackInterface.OnSafety;
                @Safety.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnSafety;
                @Safety.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnSafety;
                @FireSelector.started -= m_Wrapper.m_GunActionsCallbackInterface.OnFireSelector;
                @FireSelector.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnFireSelector;
                @FireSelector.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnFireSelector;
                @PullSlide.started -= m_Wrapper.m_GunActionsCallbackInterface.OnPullSlide;
                @PullSlide.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnPullSlide;
                @PullSlide.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnPullSlide;
                @SwingOutCylinder.started -= m_Wrapper.m_GunActionsCallbackInterface.OnSwingOutCylinder;
                @SwingOutCylinder.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnSwingOutCylinder;
                @SwingOutCylinder.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnSwingOutCylinder;
                @CloseCylinder.started -= m_Wrapper.m_GunActionsCallbackInterface.OnCloseCylinder;
                @CloseCylinder.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnCloseCylinder;
                @CloseCylinder.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnCloseCylinder;
                @ExtractorRod.started -= m_Wrapper.m_GunActionsCallbackInterface.OnExtractorRod;
                @ExtractorRod.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnExtractorRod;
                @ExtractorRod.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnExtractorRod;
                @SpinCylinder.started -= m_Wrapper.m_GunActionsCallbackInterface.OnSpinCylinder;
                @SpinCylinder.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnSpinCylinder;
                @SpinCylinder.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnSpinCylinder;
                @ToggleStance.started -= m_Wrapper.m_GunActionsCallbackInterface.OnToggleStance;
                @ToggleStance.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnToggleStance;
                @ToggleStance.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnToggleStance;
                @InsertRound.started -= m_Wrapper.m_GunActionsCallbackInterface.OnInsertRound;
                @InsertRound.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnInsertRound;
                @InsertRound.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnInsertRound;
                @BoltLock.started -= m_Wrapper.m_GunActionsCallbackInterface.OnBoltLock;
                @BoltLock.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnBoltLock;
                @BoltLock.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnBoltLock;
                @Eject.started -= m_Wrapper.m_GunActionsCallbackInterface.OnEject;
                @Eject.performed -= m_Wrapper.m_GunActionsCallbackInterface.OnEject;
                @Eject.canceled -= m_Wrapper.m_GunActionsCallbackInterface.OnEject;
            }
            m_Wrapper.m_GunActionsCallbackInterface = instance;
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
                @InsertRound.started += instance.OnInsertRound;
                @InsertRound.performed += instance.OnInsertRound;
                @InsertRound.canceled += instance.OnInsertRound;
                @BoltLock.started += instance.OnBoltLock;
                @BoltLock.performed += instance.OnBoltLock;
                @BoltLock.canceled += instance.OnBoltLock;
                @Eject.started += instance.OnEject;
                @Eject.performed += instance.OnEject;
                @Eject.canceled += instance.OnEject;
            }
        }
    }
    public GunActions @Gun => new GunActions(this);
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse & Keyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    public interface IGunActions
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
        void OnInsertRound(InputAction.CallbackContext context);
        void OnBoltLock(InputAction.CallbackContext context);
        void OnEject(InputAction.CallbackContext context);
    }
}
