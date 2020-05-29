using System.IO;
using UnityEngine;
using Steamworks;
using ImGuiNET;

public class SteamScript : MonoBehaviour
{
    public static AppId_t RECEIVER1_APP_ID = new AppId_t(234190);

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
        if (uploadingItem != null && uploadingItem.waiting_for_create) {
            uploadingItem.DrawItemWindow();
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
                        uploadingItem.waiting_for_create = true;
                    }
                }
            }
        }

        ImGui.End();
    }
}


public class SteamworksUGCItem {
    public bool waiting_for_create;

    private PublishedFileId_t steamworks_id;
    private ERemoteStoragePublishedFileVisibility visibility;
    private UGCUpdateHandle_t update_handle;

    private Mod mod;
    private string title;
    private char[] description;
    private char[] previewImagePath;

    private CallResult<CreateItemResult_t> m_CreateItemResult;
    private CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdateResult;


    private void OnCreateItemResult(CreateItemResult_t pResult, bool failed) {
        if (failed == false) {
            if (pResult.m_eResult != EResult.k_EResultOK) {
                Debug.LogError("Steam CreateItem error " + pResult.m_eResult.ToString());
            }
            steamworks_id = pResult.m_nPublishedFileId;

            if (pResult.m_bUserNeedsToAcceptWorkshopLegalAgreement) {
                Debug.LogWarning("User needs to accept workshop legal agreement");
                Application.OpenURL("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
            }
        } else {
            Debug.LogError("Error creating Steam Workshop item");
        }

        RequestUpload("Initial Upload");
    }


    private void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pResult, bool failed) {
        if (failed == false) {
            if (pResult.m_eResult != EResult.k_EResultOK) {
                Debug.LogError("Steam SubmitItemUpdate error " + pResult.m_eResult.ToString());
            }

            if (pResult.m_bUserNeedsToAcceptWorkshopLegalAgreement) {
                Debug.LogWarning("User needs to accept workshop legal agreement");
            }

            string itemPath = "steam://url/CommunityFilePage/" + steamworks_id.ToString();
            SteamFriends.ActivateGameOverlayToWebPage(itemPath);
        } else {
            Debug.LogError("Error on Steam Workshop item update");
        }

        waiting_for_create = false;
    }


    public SteamworksUGCItem(Mod _mod) {
        waiting_for_create = false;
        description = new char[1024]; description[0] = '\0';
        previewImagePath = new char[512]; previewImagePath[0] = '\0';
        visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate;
        mod = _mod;
        if (SteamManager.Initialized) {
            m_CreateItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
            m_SubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
        }
    }


    private void RequestCreation() {
        if (steamworks_id == PublishedFileId_t.Invalid) {
            SteamAPICall_t hSteamAPICall = SteamUGC.CreateItem(SteamScript.RECEIVER1_APP_ID, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
            m_CreateItemResult.Set(hSteamAPICall);
        } else {
            Debug.LogError("Requested creation of an already created Steamworks item");
        }
    }


    private void RequestUpload(string update_message) {
        // TODO: verify for upload

        Debug.Log("Doing Steam Workshop upload");
        title = mod.name;
                
        update_handle = SteamUGC.StartItemUpdate(SteamScript.RECEIVER1_APP_ID, steamworks_id);

        SteamUGC.SetItemTitle(update_handle, title);

        SteamUGC.SetItemDescription(update_handle, new string(description));

        SteamUGC.SetItemUpdateLanguage(update_handle, "english");

        //SteamUGC.SetItemMetadata(update_handle, metadata);

        SteamUGC.SetItemVisibility(update_handle, visibility);

        SteamUGC.SetItemPreview(update_handle, new string(previewImagePath));

        string modpath = Path.GetDirectoryName(mod.path);
        if (Directory.Exists(modpath)) {
            SteamUGC.SetItemContent(update_handle, modpath);
        } else {
            Debug.LogError("Invalid path for mod, unable to upload " + modpath);
            return;
        }

        SteamAPICall_t hSteamAPICall = SteamUGC.SubmitItemUpdate(update_handle, update_message);
        m_SubmitItemUpdateResult.Set(hSteamAPICall);
    }


    public void DrawItemWindow() {
        ImGui.Begin("Steam Workshop item " + title);

        ImGui.InputText("Description", description);
        ImGui.InputText("Preview Image", previewImagePath);

        ImGui.Dummy(new Vector2(0.0f, 10.0f));
        
        if (ImGui.Button("Submit")) {
            RequestCreation();
        }
        if (ImGui.Button("Cancel")) {
            waiting_for_create = false;
        }

        ImGui.End();
    }
}
