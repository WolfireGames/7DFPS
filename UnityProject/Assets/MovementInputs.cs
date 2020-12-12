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
            ""name"": ""Player"",
            ""id"": ""84a2c5b6-195c-4b1f-8f83-b249b1b80f12"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""0706fb0b-54bb-443b-8bb1-5031e5196c04"",
                    ""expectedControlType"": ""Vector2"",
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
                },
                {
                    ""name"": ""Drop"",
                    ""type"": ""Button"",
                    ""id"": ""0a353ead-4743-4645-bce0-d82f62004375"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
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
                },
                {
                    ""name"": """",
                    ""id"": ""dbf9c423-ea3e-42a7-b02e-0d3489fed915"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""d7ef9a2a-52a5-436b-9b3a-b0cff3a7b47b"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""b1e5439e-0f24-4697-b103-434a06d15050"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""7a859548-81cc-4d0c-9042-fc0b68a38286"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""3bc1283b-b0c0-4f4b-ad1f-da65a3096d5c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""a8af1eff-c12d-429c-88df-7cc918f126ad"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Inventory"",
            ""id"": ""e6489541-2696-4e39-80be-570f392320fb"",
            ""actions"": [
                {
                    ""name"": ""Inventory 1"",
                    ""type"": ""Button"",
                    ""id"": ""ae36eb54-050f-48b7-ac1f-4ca122695f41"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory 2"",
                    ""type"": ""Button"",
                    ""id"": ""f1160d00-2952-412d-88a5-93b43640beac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory 3"",
                    ""type"": ""Button"",
                    ""id"": ""e118eacc-b617-4bdf-954c-64715be11ee3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory 4"",
                    ""type"": ""Button"",
                    ""id"": ""2836aab7-f8d2-4605-bb5e-32611a524c62"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory 5"",
                    ""type"": ""Button"",
                    ""id"": ""dfee9fb9-cde2-4cd6-9aba-d2ac4eaab6e5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory 6"",
                    ""type"": ""Button"",
                    ""id"": ""5538aef0-9c2f-42ad-b187-8855f4be86ea"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory 7"",
                    ""type"": ""Button"",
                    ""id"": ""0b93472f-3fff-4281-97c8-45cb12ed0f40"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory 8"",
                    ""type"": ""Button"",
                    ""id"": ""db188fbd-acbe-45e7-9189-20b67d2bec97"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory 9"",
                    ""type"": ""Button"",
                    ""id"": ""d78bc6a9-a8bf-40d4-a47e-b63487a8171e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory 10"",
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
                    ""action"": ""Inventory 1"",
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
                    ""action"": ""Inventory 2"",
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
                    ""action"": ""Inventory 3"",
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
                    ""action"": ""Inventory 4"",
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
                    ""action"": ""Inventory 5"",
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
                    ""action"": ""Inventory 6"",
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
                    ""action"": ""Inventory 7"",
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
                    ""action"": ""Inventory 8"",
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
                    ""action"": ""Inventory 9"",
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
                    ""action"": ""Inventory 10"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b99fb4f6-ce1a-4a63-9c63-d5756ec54fc6"",
                    ""path"": ""<Keyboard>/backquote"",
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
                },
                {
                    ""name"": ""Remove Round"",
                    ""type"": ""Button"",
                    ""id"": ""755a0173-3d3a-413e-9c2c-78fcd72908aa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Insert Magazine"",
                    ""type"": ""Button"",
                    ""id"": ""8038eef4-d9e6-4209-b520-2ede50bbb091"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""7362d795-81e4-4e8e-b8b2-608736babf60"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Insert Magazine"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c47934ab-4e60-4121-8c51-9fcaed484c33"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Remove Round"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Pickup = m_Player.FindAction("Pickup", throwIfNotFound: true);
        m_Player_AimDelta = m_Player.FindAction("Aim Delta", throwIfNotFound: true);
        m_Player_TapePlayer = m_Player.FindAction("Tape Player", throwIfNotFound: true);
        m_Player_Crouch = m_Player.FindAction("Crouch", throwIfNotFound: true);
        m_Player_AimHold = m_Player.FindAction("Aim Hold", throwIfNotFound: true);
        m_Player_AimToggle = m_Player.FindAction("Aim Toggle", throwIfNotFound: true);
        m_Player_SlomoToggle = m_Player.FindAction("Slomo Toggle", throwIfNotFound: true);
        m_Player_LevelReset = m_Player.FindAction("Level Reset", throwIfNotFound: true);
        m_Player_FlashlightToggle = m_Player.FindAction("Flashlight Toggle", throwIfNotFound: true);
        m_Player_HelpButton = m_Player.FindAction("Help Button", throwIfNotFound: true);
        m_Player_Drop = m_Player.FindAction("Drop", throwIfNotFound: true);
        // Inventory
        m_Inventory = asset.FindActionMap("Inventory", throwIfNotFound: true);
        m_Inventory_Inventory1 = m_Inventory.FindAction("Inventory 1", throwIfNotFound: true);
        m_Inventory_Inventory2 = m_Inventory.FindAction("Inventory 2", throwIfNotFound: true);
        m_Inventory_Inventory3 = m_Inventory.FindAction("Inventory 3", throwIfNotFound: true);
        m_Inventory_Inventory4 = m_Inventory.FindAction("Inventory 4", throwIfNotFound: true);
        m_Inventory_Inventory5 = m_Inventory.FindAction("Inventory 5", throwIfNotFound: true);
        m_Inventory_Inventory6 = m_Inventory.FindAction("Inventory 6", throwIfNotFound: true);
        m_Inventory_Inventory7 = m_Inventory.FindAction("Inventory 7", throwIfNotFound: true);
        m_Inventory_Inventory8 = m_Inventory.FindAction("Inventory 8", throwIfNotFound: true);
        m_Inventory_Inventory9 = m_Inventory.FindAction("Inventory 9", throwIfNotFound: true);
        m_Inventory_Inventory10 = m_Inventory.FindAction("Inventory 10", throwIfNotFound: true);
        m_Inventory_Holster = m_Inventory.FindAction("Holster", throwIfNotFound: true);
        // Magazine
        m_Magazine = asset.FindActionMap("Magazine", throwIfNotFound: true);
        m_Magazine_InsertRound = m_Magazine.FindAction("Insert Round", throwIfNotFound: true);
        m_Magazine_RemoveRound = m_Magazine.FindAction("Remove Round", throwIfNotFound: true);
        m_Magazine_InsertMagazine = m_Magazine.FindAction("Insert Magazine", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Pickup;
    private readonly InputAction m_Player_AimDelta;
    private readonly InputAction m_Player_TapePlayer;
    private readonly InputAction m_Player_Crouch;
    private readonly InputAction m_Player_AimHold;
    private readonly InputAction m_Player_AimToggle;
    private readonly InputAction m_Player_SlomoToggle;
    private readonly InputAction m_Player_LevelReset;
    private readonly InputAction m_Player_FlashlightToggle;
    private readonly InputAction m_Player_HelpButton;
    private readonly InputAction m_Player_Drop;
    public struct PlayerActions
    {
        private @MovementInputs m_Wrapper;
        public PlayerActions(@MovementInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Pickup => m_Wrapper.m_Player_Pickup;
        public InputAction @AimDelta => m_Wrapper.m_Player_AimDelta;
        public InputAction @TapePlayer => m_Wrapper.m_Player_TapePlayer;
        public InputAction @Crouch => m_Wrapper.m_Player_Crouch;
        public InputAction @AimHold => m_Wrapper.m_Player_AimHold;
        public InputAction @AimToggle => m_Wrapper.m_Player_AimToggle;
        public InputAction @SlomoToggle => m_Wrapper.m_Player_SlomoToggle;
        public InputAction @LevelReset => m_Wrapper.m_Player_LevelReset;
        public InputAction @FlashlightToggle => m_Wrapper.m_Player_FlashlightToggle;
        public InputAction @HelpButton => m_Wrapper.m_Player_HelpButton;
        public InputAction @Drop => m_Wrapper.m_Player_Drop;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Pickup.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPickup;
                @Pickup.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPickup;
                @Pickup.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPickup;
                @AimDelta.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimDelta;
                @AimDelta.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimDelta;
                @AimDelta.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimDelta;
                @TapePlayer.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTapePlayer;
                @TapePlayer.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTapePlayer;
                @TapePlayer.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTapePlayer;
                @Crouch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @AimHold.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimHold;
                @AimHold.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimHold;
                @AimHold.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimHold;
                @AimToggle.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimToggle;
                @AimToggle.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimToggle;
                @AimToggle.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAimToggle;
                @SlomoToggle.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSlomoToggle;
                @SlomoToggle.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSlomoToggle;
                @SlomoToggle.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSlomoToggle;
                @LevelReset.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLevelReset;
                @LevelReset.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLevelReset;
                @LevelReset.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLevelReset;
                @FlashlightToggle.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFlashlightToggle;
                @FlashlightToggle.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFlashlightToggle;
                @FlashlightToggle.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFlashlightToggle;
                @HelpButton.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHelpButton;
                @HelpButton.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHelpButton;
                @HelpButton.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHelpButton;
                @Drop.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDrop;
                @Drop.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDrop;
                @Drop.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDrop;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
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
                @Drop.started += instance.OnDrop;
                @Drop.performed += instance.OnDrop;
                @Drop.canceled += instance.OnDrop;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

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
    private readonly InputAction m_Magazine_RemoveRound;
    private readonly InputAction m_Magazine_InsertMagazine;
    public struct MagazineActions
    {
        private @MovementInputs m_Wrapper;
        public MagazineActions(@MovementInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @InsertRound => m_Wrapper.m_Magazine_InsertRound;
        public InputAction @RemoveRound => m_Wrapper.m_Magazine_RemoveRound;
        public InputAction @InsertMagazine => m_Wrapper.m_Magazine_InsertMagazine;
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
                @RemoveRound.started -= m_Wrapper.m_MagazineActionsCallbackInterface.OnRemoveRound;
                @RemoveRound.performed -= m_Wrapper.m_MagazineActionsCallbackInterface.OnRemoveRound;
                @RemoveRound.canceled -= m_Wrapper.m_MagazineActionsCallbackInterface.OnRemoveRound;
                @InsertMagazine.started -= m_Wrapper.m_MagazineActionsCallbackInterface.OnInsertMagazine;
                @InsertMagazine.performed -= m_Wrapper.m_MagazineActionsCallbackInterface.OnInsertMagazine;
                @InsertMagazine.canceled -= m_Wrapper.m_MagazineActionsCallbackInterface.OnInsertMagazine;
            }
            m_Wrapper.m_MagazineActionsCallbackInterface = instance;
            if (instance != null)
            {
                @InsertRound.started += instance.OnInsertRound;
                @InsertRound.performed += instance.OnInsertRound;
                @InsertRound.canceled += instance.OnInsertRound;
                @RemoveRound.started += instance.OnRemoveRound;
                @RemoveRound.performed += instance.OnRemoveRound;
                @RemoveRound.canceled += instance.OnRemoveRound;
                @InsertMagazine.started += instance.OnInsertMagazine;
                @InsertMagazine.performed += instance.OnInsertMagazine;
                @InsertMagazine.canceled += instance.OnInsertMagazine;
            }
        }
    }
    public MagazineActions @Magazine => new MagazineActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
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
        void OnDrop(InputAction.CallbackContext context);
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
        void OnRemoveRound(InputAction.CallbackContext context);
        void OnInsertMagazine(InputAction.CallbackContext context);
    }
}
