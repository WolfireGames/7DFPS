using System;
using System.IO;
using System.Text;
using UnityEngine;
using Steamworks;
using ImGuiNET;

public class SteamScript : MonoBehaviour
{
    public static AppId_t RECEIVER1_APP_ID = new AppId_t(234190);

    public ModManager modManager;

    private SteamworksUGCItem uploadingItem;

    protected Callback<ItemInstalled_t> m_ItemInstalled;
    protected Callback<DownloadItemResult_t> m_DownloadItemResult;
    private CallResult<SteamUGCQueryCompleted_t> m_callSteamUGCQueryCompleted;


    private void OnItemInstalled(ItemInstalled_t pCallback) {
        if (pCallback.m_unAppID != RECEIVER1_APP_ID) {
            // Not our game
            return;
        }

        LoadModIntoGame(pCallback.m_nPublishedFileId);
    }


    private void OnItemDownloaded(DownloadItemResult_t pCallback) {
        if (pCallback.m_unAppID != RECEIVER1_APP_ID) {
            // Not our game
            return;
        }

        LoadModIntoGame(pCallback.m_nPublishedFileId);
    }


    private void OnUGCSteamUGCQueryCompleted(SteamUGCQueryCompleted_t pResult, bool failed) {
        Debug.Log("OnUGCSteamUGCQueryCompleted() " + pResult.m_eResult);

        if (failed == false) {
            for (uint i = 0; i < pResult.m_unNumResultsReturned; i++) {
                SteamUGCDetails_t details;
                SteamUGC.GetQueryUGCResult(pResult.m_handle, i, out details);
                SteamUGC.DownloadItem(details.m_nPublishedFileId, false);
            }
        } else {
            Debug.LogError("OnUGCSteamUGCQueryCompleted() error " + pResult.m_eResult);
        }

        SteamUGC.ReleaseQueryUGCRequest(pResult.m_handle);
    }


    private void LoadModIntoGame(PublishedFileId_t publishedFileId) {
        ulong sizeOnDisk = 0;
        string folder;
        uint folderSize = 0;
        uint timeStamp = 0;

        uint retval = SteamUGC.GetItemState(publishedFileId);
        EItemState state = (EItemState)retval;
        Debug.Log("Item state: " + state.ToString());

        if (SteamUGC.GetItemInstallInfo(publishedFileId, out sizeOnDisk, out folder, folderSize, out timeStamp)) {
            try {
                modManager.LoadSteamItem(folder);
            } catch (System.Exception e) {
                Debug.LogWarning($"Failed to import {folder}: {e.Message}");
            }
            // Store Steamworks ID?
        } else {
            Debug.LogWarning("Got Steam ItemInstalled message for non-installed Workshop item");
        }
    }


    private void OnEnable() {
        if (SteamManager.Initialized) {
            m_ItemInstalled = Callback<ItemInstalled_t>.Create(OnItemInstalled);
            m_DownloadItemResult = Callback<DownloadItemResult_t>.Create(OnItemDownloaded);
            m_callSteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnUGCSteamUGCQueryCompleted);
        }
    }


    // Start is called before the first frame update
    void Start() {
        uploadingItem = null;

        if (SteamManager.Initialized) {
            QueryPersonalWorkshopItems();
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


    void QueryPersonalWorkshopItems() {
        CSteamID userid = SteamUser.GetSteamID();

        UGCQueryHandle_t query_handle = SteamUGC.CreateQueryUserUGCRequest(
            userid.GetAccountID(),
            EUserUGCList.k_EUserUGCList_Published,
            EUGCMatchingUGCType.k_EUGCMatchingUGCType_All,
            EUserUGCListSortOrder.k_EUserUGCListSortOrder_TitleAsc,
            RECEIVER1_APP_ID,
            RECEIVER1_APP_ID,
            1
        );

        SteamAPICall_t call = SteamUGC.SendQueryUGCRequest(query_handle);
        m_callSteamUGCQueryCompleted.Set(call);
    }


    void DrawSteamWindow() {
        ImGui.Begin("Steam Workshop upload");
        ImGui.Text("Local installed mods");
        if (PlayerPrefs.GetInt("mods_enabled", 0) == 1) {
            int i = 0;
            foreach (Mod mod in ModManager.availableMods) {
                ImGui.Text(mod.name);
                ImGui.SameLine(120);
                if (ImGui.Button("Upload to Steam Workshop##" + i++)) {
                    if (uploadingItem == null || !uploadingItem.waiting_for_create) {
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
            } else {
                steamworks_id = pResult.m_nPublishedFileId;

                // Store Steamworks ID in mod folder for future use
                string idPath = Path.GetDirectoryName(mod.path) + "/steamworks_id.txt";
                try {
                    File.Create(idPath).Close();
                    File.WriteAllText(idPath, steamworks_id.ToString());
                } catch (Exception e) {
                    Debug.LogError("Failed to write Steam Workshop file ID for mod: " + e);
                }

                if (pResult.m_bUserNeedsToAcceptWorkshopLegalAgreement) {
                    Debug.LogWarning("User needs to accept workshop legal agreement");
                    Application.OpenURL("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
                }
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
            } else {
                if (pResult.m_bUserNeedsToAcceptWorkshopLegalAgreement) {
                    Debug.LogWarning("User needs to accept workshop legal agreement");
                }

                string itemPath = "steam://url/CommunityFilePage/" + steamworks_id.ToString();
                SteamFriends.ActivateGameOverlayToWebPage(itemPath);
            }
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

        string idPath = Path.GetDirectoryName(mod.path) + "/steamworks_id.txt";
        if (File.Exists(idPath)) {
            try {
                string idText = File.ReadAllText(idPath);
                ulong id = 0;
                if (ulong.TryParse(idText, out id)) {
                    steamworks_id = new PublishedFileId_t(id);
                }
            } catch (Exception e) {
                Debug.LogError("Error reading Steam Workshop file ID for mod: " + e);
            }
        }

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
            RequestUpload("Update");
        }
    }


    private void RequestUpload(string update_message) {
        Debug.Log("Doing Steam Workshop upload");
        title = mod.name;
                
        update_handle = SteamUGC.StartItemUpdate(SteamScript.RECEIVER1_APP_ID, steamworks_id);

        SteamUGC.SetItemTitle(update_handle, title);

        SteamUGC.SetItemDescription(update_handle, new string(description));

        SteamUGC.SetItemUpdateLanguage(update_handle, "english");

        //SteamUGC.SetItemMetadata(update_handle, metadata);

        SteamUGC.SetItemVisibility(update_handle, visibility);

        string path = new string(previewImagePath);
        if (File.Exists(path)) {
            SteamUGC.SetItemPreview(update_handle, path);
        }

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
        // Disabled for now doesn't seem to work (not present in Overgrowth either)?
        //ImGui.InputText("Preview Image", previewImagePath);

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
