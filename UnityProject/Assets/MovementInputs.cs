// GENERATED AUTOMATICALLY FROM 'Assets/MovementInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MovementInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MovementInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MovementInputs"",
    ""maps"": [
        {
            ""name"": ""main"",
            ""id"": ""84a2c5b6-195c-4b1f-8f83-b249b1b80f12"",
            ""actions"": [
                {
                    ""name"": ""Vertical"",
                    ""type"": ""Value"",
                    ""id"": ""22c2df65-860b-4a0a-a266-e9b7d85cb8ad"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Horizontal"",
                    ""type"": ""Value"",
                    ""id"": ""81ab46d9-2d27-45b6-97ef-7fdd207d34e2"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""f77db03f-40dd-4427-9888-f79ce68461f9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pickup"",
                    ""type"": ""Button"",
                    ""id"": ""458c6a04-1818-4080-bc41-4fa9d66bdf33"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim Delta"",
                    ""type"": ""Value"",
                    ""id"": ""95810dea-689d-4c03-aad7-35ce381f2a82"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Tape Player"",
                    ""type"": ""Button"",
                    ""id"": ""b0c5ba89-d29c-451c-8d6f-922d96f5bddb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""5ef6f0e8-d352-46a0-b092-53c10451d695"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim Hold"",
                    ""type"": ""Button"",
                    ""id"": ""9a7de4c1-f7c9-45e5-acef-b6e54a1c4add"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""b88067ee-b020-4ad5-a225-c7856ec185ba"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Slomo Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""41cff4ba-5256-4830-9eac-237eaa075508"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Level Reset"",
                    ""type"": ""Button"",
                    ""id"": ""42cc37f9-4c1d-436a-aef0-0a6469830c75"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Flashlight Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""31b390c4-0328-4ab1-9f75-7419ec2264b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Help Button"",
                    ""type"": ""Button"",
                    ""id"": ""3cc6393c-983a-468f-a3d6-0badd5c377de"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""41e3553d-49f3-4aab-8f6f-2e3527cf2be5"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""e335a5fc-b7ce-4938-a1ad-487cbb3bc6ef"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""06a85789-c451-4829-80f5-24242f4bd255"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""7a2f3c4d-a3aa-48d1-9bb3-38d328ec29b8"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""41f3c1c5-46bb-47a5-9dc3-fd07b48d1741"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""1c299a54-daeb-455f-8da6-185ce876897e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Horizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4b0ab3d2-6d28-4d85-a1ac-1c4a29b8a723"",
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
                    ""id"": ""e71e2f0c-5cfe-4a85-b810-4236da9f29c9"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pickup"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b344c3f2-bded-46d0-9849-71148b723d77"",
                    ""path"": ""<Pointer>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.05,y=0.05)"",
                    ""groups"": """",
                    ""action"": ""Aim Delta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""39bb189b-0f5f-416e-8901-c8953161871e"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tape Player"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""51fac04e-909d-4bd8-8b98-25afa39a259d"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c0cec2f-18ed-4c0f-9944-51ff8a247992"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim Hold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3985fadf-3944-478f-b8a3-0641408bb6f1"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c151ab43-6a28-4fd7-88ab-4b99d2c93f80"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slomo Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""24552e1e-110e-4bed-ada7-2be50ced4493"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Level Reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c5b07a1a-449c-4b23-883c-8a9674336d59"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Flashlight Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""30b6f181-ef46-4202-a57e-b644a18a834d"",
                    ""path"": ""<Keyboard>/slash"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Help Button"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Inventory"",
            ""id"": ""e6489541-2696-4e39-80be-570f392320fb"",
            ""actions"": [
                {
                    ""name"": ""Inventory1"",
                    ""type"": ""Button"",
                    ""id"": ""ae36eb54-050f-48b7-ac1f-4ca122695f41"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory2"",
                    ""type"": ""Button"",
                    ""id"": ""f1160d00-2952-412d-88a5-93b43640beac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory3"",
                    ""type"": ""Button"",
                    ""id"": ""e118eacc-b617-4bdf-954c-64715be11ee3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory4"",
                    ""type"": ""Button"",
                    ""id"": ""2836aab7-f8d2-4605-bb5e-32611a524c62"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory5"",
                    ""type"": ""Button"",
                    ""id"": ""dfee9fb9-cde2-4cd6-9aba-d2ac4eaab6e5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory6"",
                    ""type"": ""Button"",
                    ""id"": ""5538aef0-9c2f-42ad-b187-8855f4be86ea"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory7"",
                    ""type"": ""Button"",
                    ""id"": ""0b93472f-3fff-4281-97c8-45cb12ed0f40"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory8"",
                    ""type"": ""Button"",
                    ""id"": ""db188fbd-acbe-45e7-9189-20b67d2bec97"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory9"",
                    ""type"": ""Button"",
                    ""id"": ""d78bc6a9-a8bf-40d4-a47e-b63487a8171e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory10"",
                    ""type"": ""Button"",
                    ""id"": ""0facd9b8-9e49-47ec-8972-f7d88b8b7d75"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Holster"",
                    ""type"": ""Button"",
                    ""id"": ""06be512c-afe0-48bc-ac6c-10ed47aa6326"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b54cca0a-2afa-4144-8b03-7f6bfc97ca89"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""943beb8b-7828-4d90-8891-411fa979c729"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""879e4f43-6d9b-4886-9c9d-7334a249fae8"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e14598d4-7cbc-4bd1-b971-1f7c96ead592"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""774a7fcb-472e-4071-80fc-01c83bab14b8"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4afc81cf-c1f4-49cc-b2c1-44c20b7b67ba"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb9d84c1-7258-4238-8f11-b1a5a6aebfc1"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68863695-6d8d-4486-95bc-cf917419a561"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc0ddaf0-db1d-4de7-9ab1-620b4325d940"",
                    ""path"": ""<Keyboard>/9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory9"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9a50227d-f639-418a-8802-5568974b4296"",
                    ""path"": ""<Keyboard>/0"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory10"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b99fb4f6-ce1a-4a63-9c63-d5756ec54fc6"",
                    ""path"": ""<Keyboard>/semicolon"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Holster"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Magazine"",
            ""id"": ""fef7530b-8765-4afa-b31f-11f179d17f6e"",
            ""actions"": [
                {
                    ""name"": ""Insert Round"",
                    ""type"": ""Button"",
                    ""id"": ""d373bd51-1c3d-46a6-9282-538ccf00c5ab"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f958cded-a188-4c75-97dd-e27118c1808b"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Insert Round"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // main
        m_main = asset.FindActionMap("main", throwIfNotFound: true);
        m_main_Vertical = m_main.FindAction("Vertical", throwIfNotFound: true);
        m_main_Horizontal = m_main.FindAction("Horizontal", throwIfNotFound: true);
        m_main_Jump = m_main.FindAction("Jump", throwIfNotFound: true);
        m_main_Pickup = m_main.FindAction("Pickup", throwIfNotFound: true);
        m_main_AimDelta = m_main.FindAction("Aim Delta", throwIfNotFound: true);
        m_main_TapePlayer = m_main.FindAction("Tape Player", throwIfNotFound: true);
        m_main_Crouch = m_main.FindAction("Crouch", throwIfNotFound: true);
        m_main_AimHold = m_main.FindAction("Aim Hold", throwIfNotFound: true);
        m_main_AimToggle = m_main.FindAction("Aim Toggle", throwIfNotFound: true);
        m_main_SlomoToggle = m_main.FindAction("Slomo Toggle", throwIfNotFound: true);
        m_main_LevelReset = m_main.FindAction("Level Reset", throwIfNotFound: true);
        m_main_FlashlightToggle = m_main.FindAction("Flashlight Toggle", throwIfNotFound: true);
        m_main_HelpButton = m_main.FindAction("Help Button", throwIfNotFound: true);
        // Inventory
        m_Inventory = asset.FindActionMap("Inventory", throwIfNotFound: true);
        m_Inventory_Inventory1 = m_Inventory.FindAction("Inventory1", throwIfNotFound: true);
        m_Inventory_Inventory2 = m_Inventory.FindAction("Inventory2", throwIfNotFound: true);
        m_Inventory_Inventory3 = m_Inventory.FindAction("Inventory3", throwIfNotFound: true);
        m_Inventory_Inventory4 = m_Inventory.FindAction("Inventory4", throwIfNotFound: true);
        m_Inventory_Inventory5 = m_Inventory.FindAction("Inventory5", throwIfNotFound: true);
        m_Inventory_Inventory6 = m_Inventory.FindAction("Inventory6", throwIfNotFound: true);
        m_Inventory_Inventory7 = m_Inventory.FindAction("Inventory7", throwIfNotFound: true);
        m_Inventory_Inventory8 = m_Inventory.FindAction("Inventory8", throwIfNotFound: true);
        m_Inventory_Inventory9 = m_Inventory.FindAction("Inventory9", throwIfNotFound: true);
        m_Inventory_Inventory10 = m_Inventory.FindAction("Inventory10", throwIfNotFound: true);
        m_Inventory_Holster = m_Inventory.FindAction("Holster", throwIfNotFound: true);
        // Magazine
        m_Magazine = asset.FindActionMap("Magazine", throwIfNotFound: true);
        m_Magazine_InsertRound = m_Magazine.FindAction("Insert Round", throwIfNotFound: true);
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
    private readonly InputAction m_main_Vertical;
    private readonly InputAction m_main_Horizontal;
    private readonly InputAction m_main_Jump;
    private readonly InputAction m_main_Pickup;
    private readonly InputAction m_main_AimDelta;
    private readonly InputAction m_main_TapePlayer;
    private readonly InputAction m_main_Crouch;
    private readonly InputAction m_main_AimHold;
    private readonly InputAction m_main_AimToggle;
    private readonly InputAction m_main_SlomoToggle;
    private readonly InputAction m_main_LevelReset;
    private readonly InputAction m_main_FlashlightToggle;
    private readonly InputAction m_main_HelpButton;
    public struct MainActions
    {
        private @MovementInputs m_Wrapper;
        public MainActions(@MovementInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Vertical => m_Wrapper.m_main_Vertical;
        public InputAction @Horizontal => m_Wrapper.m_main_Horizontal;
        public InputAction @Jump => m_Wrapper.m_main_Jump;
        public InputAction @Pickup => m_Wrapper.m_main_Pickup;
        public InputAction @AimDelta => m_Wrapper.m_main_AimDelta;
        public InputAction @TapePlayer => m_Wrapper.m_main_TapePlayer;
        public InputAction @Crouch => m_Wrapper.m_main_Crouch;
        public InputAction @AimHold => m_Wrapper.m_main_AimHold;
        public InputAction @AimToggle => m_Wrapper.m_main_AimToggle;
        public InputAction @SlomoToggle => m_Wrapper.m_main_SlomoToggle;
        public InputAction @LevelReset => m_Wrapper.m_main_LevelReset;
        public InputAction @FlashlightToggle => m_Wrapper.m_main_FlashlightToggle;
        public InputAction @HelpButton => m_Wrapper.m_main_HelpButton;
        public InputActionMap Get() { return m_Wrapper.m_main; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MainActions set) { return set.Get(); }
        public void SetCallbacks(IMainActions instance)
        {
            if (m_Wrapper.m_MainActionsCallbackInterface != null)
            {
                @Vertical.started -= m_Wrapper.m_MainActionsCallbackInterface.OnVertical;
                @Vertical.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnVertical;
                @Vertical.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnVertical;
                @Horizontal.started -= m_Wrapper.m_MainActionsCallbackInterface.OnHorizontal;
                @Horizontal.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnHorizontal;
                @Horizontal.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnHorizontal;
                @Jump.started -= m_Wrapper.m_MainActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnJump;
                @Pickup.started -= m_Wrapper.m_MainActionsCallbackInterface.OnPickup;
                @Pickup.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnPickup;
                @Pickup.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnPickup;
                @AimDelta.started -= m_Wrapper.m_MainActionsCallbackInterface.OnAimDelta;
                @AimDelta.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnAimDelta;
                @AimDelta.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnAimDelta;
                @TapePlayer.started -= m_Wrapper.m_MainActionsCallbackInterface.OnTapePlayer;
                @TapePlayer.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnTapePlayer;
                @TapePlayer.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnTapePlayer;
                @Crouch.started -= m_Wrapper.m_MainActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnCrouch;
                @AimHold.started -= m_Wrapper.m_MainActionsCallbackInterface.OnAimHold;
                @AimHold.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnAimHold;
                @AimHold.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnAimHold;
                @AimToggle.started -= m_Wrapper.m_MainActionsCallbackInterface.OnAimToggle;
                @AimToggle.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnAimToggle;
                @AimToggle.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnAimToggle;
                @SlomoToggle.started -= m_Wrapper.m_MainActionsCallbackInterface.OnSlomoToggle;
                @SlomoToggle.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnSlomoToggle;
                @SlomoToggle.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnSlomoToggle;
                @LevelReset.started -= m_Wrapper.m_MainActionsCallbackInterface.OnLevelReset;
                @LevelReset.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnLevelReset;
                @LevelReset.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnLevelReset;
                @FlashlightToggle.started -= m_Wrapper.m_MainActionsCallbackInterface.OnFlashlightToggle;
                @FlashlightToggle.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnFlashlightToggle;
                @FlashlightToggle.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnFlashlightToggle;
                @HelpButton.started -= m_Wrapper.m_MainActionsCallbackInterface.OnHelpButton;
                @HelpButton.performed -= m_Wrapper.m_MainActionsCallbackInterface.OnHelpButton;
                @HelpButton.canceled -= m_Wrapper.m_MainActionsCallbackInterface.OnHelpButton;
            }
            m_Wrapper.m_MainActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Vertical.started += instance.OnVertical;
                @Vertical.performed += instance.OnVertical;
                @Vertical.canceled += instance.OnVertical;
                @Horizontal.started += instance.OnHorizontal;
                @Horizontal.performed += instance.OnHorizontal;
                @Horizontal.canceled += instance.OnHorizontal;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Pickup.started += instance.OnPickup;
                @Pickup.performed += instance.OnPickup;
                @Pickup.canceled += instance.OnPickup;
                @AimDelta.started += instance.OnAimDelta;
                @AimDelta.performed += instance.OnAimDelta;
                @AimDelta.canceled += instance.OnAimDelta;
                @TapePlayer.started += instance.OnTapePlayer;
                @TapePlayer.performed += instance.OnTapePlayer;
                @TapePlayer.canceled += instance.OnTapePlayer;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @AimHold.started += instance.OnAimHold;
                @AimHold.performed += instance.OnAimHold;
                @AimHold.canceled += instance.OnAimHold;
                @AimToggle.started += instance.OnAimToggle;
                @AimToggle.performed += instance.OnAimToggle;
                @AimToggle.canceled += instance.OnAimToggle;
                @SlomoToggle.started += instance.OnSlomoToggle;
                @SlomoToggle.performed += instance.OnSlomoToggle;
                @SlomoToggle.canceled += instance.OnSlomoToggle;
                @LevelReset.started += instance.OnLevelReset;
                @LevelReset.performed += instance.OnLevelReset;
                @LevelReset.canceled += instance.OnLevelReset;
                @FlashlightToggle.started += instance.OnFlashlightToggle;
                @FlashlightToggle.performed += instance.OnFlashlightToggle;
                @FlashlightToggle.canceled += instance.OnFlashlightToggle;
                @HelpButton.started += instance.OnHelpButton;
                @HelpButton.performed += instance.OnHelpButton;
                @HelpButton.canceled += instance.OnHelpButton;
            }
        }
    }
    public MainActions @main => new MainActions(this);

    // Inventory
    private readonly InputActionMap m_Inventory;
    private IInventoryActions m_InventoryActionsCallbackInterface;
    private readonly InputAction m_Inventory_Inventory1;
    private readonly InputAction m_Inventory_Inventory2;
    private readonly InputAction m_Inventory_Inventory3;
    private readonly InputAction m_Inventory_Inventory4;
    private readonly InputAction m_Inventory_Inventory5;
    private readonly InputAction m_Inventory_Inventory6;
    private readonly InputAction m_Inventory_Inventory7;
    private readonly InputAction m_Inventory_Inventory8;
    private readonly InputAction m_Inventory_Inventory9;
    private readonly InputAction m_Inventory_Inventory10;
    private readonly InputAction m_Inventory_Holster;
    public struct InventoryActions
    {
        private @MovementInputs m_Wrapper;
        public InventoryActions(@MovementInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Inventory1 => m_Wrapper.m_Inventory_Inventory1;
        public InputAction @Inventory2 => m_Wrapper.m_Inventory_Inventory2;
        public InputAction @Inventory3 => m_Wrapper.m_Inventory_Inventory3;
        public InputAction @Inventory4 => m_Wrapper.m_Inventory_Inventory4;
        public InputAction @Inventory5 => m_Wrapper.m_Inventory_Inventory5;
        public InputAction @Inventory6 => m_Wrapper.m_Inventory_Inventory6;
        public InputAction @Inventory7 => m_Wrapper.m_Inventory_Inventory7;
        public InputAction @Inventory8 => m_Wrapper.m_Inventory_Inventory8;
        public InputAction @Inventory9 => m_Wrapper.m_Inventory_Inventory9;
        public InputAction @Inventory10 => m_Wrapper.m_Inventory_Inventory10;
        public InputAction @Holster => m_Wrapper.m_Inventory_Holster;
        public InputActionMap Get() { return m_Wrapper.m_Inventory; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InventoryActions set) { return set.Get(); }
        public void SetCallbacks(IInventoryActions instance)
        {
            if (m_Wrapper.m_InventoryActionsCallbackInterface != null)
            {
                @Inventory1.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory1;
                @Inventory1.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory1;
                @Inventory1.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory1;
                @Inventory2.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory2;
                @Inventory2.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory2;
                @Inventory2.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory2;
                @Inventory3.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory3;
                @Inventory3.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory3;
                @Inventory3.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory3;
                @Inventory4.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory4;
                @Inventory4.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory4;
                @Inventory4.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory4;
                @Inventory5.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory5;
                @Inventory5.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory5;
                @Inventory5.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory5;
                @Inventory6.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory6;
                @Inventory6.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory6;
                @Inventory6.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory6;
                @Inventory7.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory7;
                @Inventory7.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory7;
                @Inventory7.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory7;
                @Inventory8.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory8;
                @Inventory8.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory8;
                @Inventory8.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory8;
                @Inventory9.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory9;
                @Inventory9.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory9;
                @Inventory9.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory9;
                @Inventory10.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory10;
                @Inventory10.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory10;
                @Inventory10.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnInventory10;
                @Holster.started -= m_Wrapper.m_InventoryActionsCallbackInterface.OnHolster;
                @Holster.performed -= m_Wrapper.m_InventoryActionsCallbackInterface.OnHolster;
                @Holster.canceled -= m_Wrapper.m_InventoryActionsCallbackInterface.OnHolster;
            }
            m_Wrapper.m_InventoryActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Inventory1.started += instance.OnInventory1;
                @Inventory1.performed += instance.OnInventory1;
                @Inventory1.canceled += instance.OnInventory1;
                @Inventory2.started += instance.OnInventory2;
                @Inventory2.performed += instance.OnInventory2;
                @Inventory2.canceled += instance.OnInventory2;
                @Inventory3.started += instance.OnInventory3;
                @Inventory3.performed += instance.OnInventory3;
                @Inventory3.canceled += instance.OnInventory3;
                @Inventory4.started += instance.OnInventory4;
                @Inventory4.performed += instance.OnInventory4;
                @Inventory4.canceled += instance.OnInventory4;
                @Inventory5.started += instance.OnInventory5;
                @Inventory5.performed += instance.OnInventory5;
                @Inventory5.canceled += instance.OnInventory5;
                @Inventory6.started += instance.OnInventory6;
                @Inventory6.performed += instance.OnInventory6;
                @Inventory6.canceled += instance.OnInventory6;
                @Inventory7.started += instance.OnInventory7;
                @Inventory7.performed += instance.OnInventory7;
                @Inventory7.canceled += instance.OnInventory7;
                @Inventory8.started += instance.OnInventory8;
                @Inventory8.performed += instance.OnInventory8;
                @Inventory8.canceled += instance.OnInventory8;
                @Inventory9.started += instance.OnInventory9;
                @Inventory9.performed += instance.OnInventory9;
                @Inventory9.canceled += instance.OnInventory9;
                @Inventory10.started += instance.OnInventory10;
                @Inventory10.performed += instance.OnInventory10;
                @Inventory10.canceled += instance.OnInventory10;
                @Holster.started += instance.OnHolster;
                @Holster.performed += instance.OnHolster;
                @Holster.canceled += instance.OnHolster;
            }
        }
    }
    public InventoryActions @Inventory => new InventoryActions(this);

    // Magazine
    private readonly InputActionMap m_Magazine;
    private IMagazineActions m_MagazineActionsCallbackInterface;
    private readonly InputAction m_Magazine_InsertRound;
    public struct MagazineActions
    {
        private @MovementInputs m_Wrapper;
        public MagazineActions(@MovementInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @InsertRound => m_Wrapper.m_Magazine_InsertRound;
        public InputActionMap Get() { return m_Wrapper.m_Magazine; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MagazineActions set) { return set.Get(); }
        public void SetCallbacks(IMagazineActions instance)
        {
            if (m_Wrapper.m_MagazineActionsCallbackInterface != null)
            {
                @InsertRound.started -= m_Wrapper.m_MagazineActionsCallbackInterface.OnInsertRound;
                @InsertRound.performed -= m_Wrapper.m_MagazineActionsCallbackInterface.OnInsertRound;
                @InsertRound.canceled -= m_Wrapper.m_MagazineActionsCallbackInterface.OnInsertRound;
            }
            m_Wrapper.m_MagazineActionsCallbackInterface = instance;
            if (instance != null)
            {
                @InsertRound.started += instance.OnInsertRound;
                @InsertRound.performed += instance.OnInsertRound;
                @InsertRound.canceled += instance.OnInsertRound;
            }
        }
    }
    public MagazineActions @Magazine => new MagazineActions(this);
    public interface IMainActions
    {
        void OnVertical(InputAction.CallbackContext context);
        void OnHorizontal(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnPickup(InputAction.CallbackContext context);
        void OnAimDelta(InputAction.CallbackContext context);
        void OnTapePlayer(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnAimHold(InputAction.CallbackContext context);
        void OnAimToggle(InputAction.CallbackContext context);
        void OnSlomoToggle(InputAction.CallbackContext context);
        void OnLevelReset(InputAction.CallbackContext context);
        void OnFlashlightToggle(InputAction.CallbackContext context);
        void OnHelpButton(InputAction.CallbackContext context);
    }
    public interface IInventoryActions
    {
        void OnInventory1(InputAction.CallbackContext context);
        void OnInventory2(InputAction.CallbackContext context);
        void OnInventory3(InputAction.CallbackContext context);
        void OnInventory4(InputAction.CallbackContext context);
        void OnInventory5(InputAction.CallbackContext context);
        void OnInventory6(InputAction.CallbackContext context);
        void OnInventory7(InputAction.CallbackContext context);
        void OnInventory8(InputAction.CallbackContext context);
        void OnInventory9(InputAction.CallbackContext context);
        void OnInventory10(InputAction.CallbackContext context);
        void OnHolster(InputAction.CallbackContext context);
    }
    public interface IMagazineActions
    {
        void OnInsertRound(InputAction.CallbackContext context);
    }
}
