using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using ImGuiNET;

public class SteamScript : MonoBehaviour
{
    public static AppId_t RECEIVER2_APP_ID = new AppId_t(234190);

    string steamName;
    SteamworksUGCItem uploadingItem;


    // Start is called before the first frame update
    void Start() {
        uploadingItem = null;

        if (SteamManager.Initialized) {
            steamName = SteamFriends.GetPersonaName();
        }
    }


    // Update is called once per frame
    void Update() {
        if (optionsmenuscript.show_steam_ui) {
            DrawSteamWindow();
        }
    }


    void DrawSteamWindow() {
        ImGui.Begin("Steam Test Window");

        ImGui.Text("Steam user name: " + steamName);
        ImGui.Dummy(new Vector2(0.0f, 10.0f));

        ImGui.Text("Available mods:");
        if (PlayerPrefs.GetInt("mods_enabled", 0) == 1) {
            foreach (Mod mod in ModManager.availableMods) {
                ImGui.Text(mod.name);
                ImGui.SameLine(120);
                if (ImGui.Button("Upload to Steam Workshop")) {
                    if (uploadingItem == null) {
                        uploadingItem = new SteamworksUGCItem(mod);
                        uploadingItem.RequestCreation();
                    }
                }
            }
        }

        ImGui.End();
    }
}


public class SteamworksUGCItem {
    PublishedFileId_t steamworks_id;
    Mod mod;

    private CallResult<CreateItemResult_t> m_CreateItemResult;


    private void OnCreateItemResult(CreateItemResult_t pResult, bool failed) {
        if (failed == false) {
            if (pResult.m_eResult != EResult.k_EResultOK) {
                Debug.LogError("Steam CreateItem error " + pResult.m_eResult.ToString());
            }
            steamworks_id = pResult.m_nPublishedFileId;

            if (pResult.m_bUserNeedsToAcceptWorkshopLegalAgreement) {
                Debug.LogWarning("User needs to accept workshop legal agreement");
            }
        } else {
            Debug.LogError("Error creating Steam Workshop item");
        }

        //RequestUpload("Initial Upload", visibility, k_UploadDataControl_UploadAll);
    }


    public SteamworksUGCItem(Mod _mod) {
        mod = _mod;
        if (SteamManager.Initialized) {
            m_CreateItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
        }
    }


    public void RequestCreation() {
        if (steamworks_id == PublishedFileId_t.Invalid) {
            SteamAPICall_t hSteamAPICall = SteamUGC.CreateItem(SteamScript.RECEIVER2_APP_ID, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
            m_CreateItemResult.Set(hSteamAPICall);
        } else {
            Debug.LogError("Requested creation of an already created Steamworks item");
        }
    }
}
