using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using ImGuiNET;

public class SteamScript : MonoBehaviour
{
    public static AppId_t RECEIVER1_APP_ID = new AppId_t(234190);

    public static Vector4 backgroundColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
    public static Vector4 buttonColor = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
    public static Vector4 buttonHoveredColor = new Vector4(0.6f, 0.6f, 0.6f, 1.0f);
    public static Vector4 buttonActiveColor = new Vector4(0.65f, 0.65f, 0.65f, 1.0f);
    public static Vector4 headerColor = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);

    private bool loadItems;
    private SteamworksUGCItem uploadingItem;
    private List<SteamUGCDetails_t> steamItems;
    private ModManager modManager;

    protected Callback<ItemInstalled_t> m_ItemInstalled;
    protected Callback<DownloadItemResult_t> m_DownloadItemResult;
    private CallResult<DeleteItemResult_t> m_DeleteItemResult;
    private CallResult<SteamUGCQueryCompleted_t> m_callSteamUGCQueryCompleted;


    private void OnItemInstalled(ItemInstalled_t pCallback) {
        if (pCallback.m_unAppID != RECEIVER1_APP_ID) {
            // Not our game
            return;
        }

        LoadModIntoGame(pCallback.m_nPublishedFileId);
        // Refresh list
        QueryPersonalWorkshopItems();
    }


    private void OnItemDownloaded(DownloadItemResult_t pCallback) {
        if (pCallback.m_unAppID != RECEIVER1_APP_ID) {
            // Not our game
            return;
        }

    }


    private void OnItemDeleted(DeleteItemResult_t pResult, bool failed) {
        if (failed == false) {
            // Refresh list
            QueryPersonalWorkshopItems();
        }
    }


    private void OnUGCSteamUGCQueryCompleted(SteamUGCQueryCompleted_t pResult, bool failed) {
        Debug.Log("OnUGCSteamUGCQueryCompleted() " + pResult.m_eResult);

        if (failed == false) {
            steamItems.Clear();
            for (uint i = 0; i < pResult.m_unNumResultsReturned; i++) {
                SteamUGCDetails_t details;
                SteamUGC.GetQueryUGCResult(pResult.m_handle, i, out details);
                steamItems.Add(details);
                // Load items at startup, but not after later queries
                if (loadItems) {
                    uint itemState = SteamUGC.GetItemState(details.m_nPublishedFileId);
                    if ((itemState & (uint)EItemState.k_EItemStateInstalled) != 0) {
                        LoadModIntoGame(details.m_nPublishedFileId);
                    }
                }
            }
        } else {
            Debug.LogError("OnUGCSteamUGCQueryCompleted() error " + pResult.m_eResult);
        }
        loadItems = false;

        SteamUGC.ReleaseQueryUGCRequest(pResult.m_handle);
    }


    private void LoadModIntoGame(PublishedFileId_t publishedFileId) {
        ulong sizeOnDisk = 0;
        uint folderSize = 512;
        char[] temp = new char[folderSize];
        string folder = new string(temp);
        uint timeStamp = 0;

        uint retval = SteamUGC.GetItemState(publishedFileId);
        EItemState state = (EItemState)retval;
        Debug.Log("Item state: " + state.ToString());

        if (SteamUGC.GetItemInstallInfo(publishedFileId, out sizeOnDisk, out folder, folderSize, out timeStamp)) {
            try {
                foreach (Mod m in ModManager.availableMods) {
                    if (m.path.Contains(folder)) {
                        // Don't load twice
                        return;
                    }
                }
                modManager.LoadSteamItem(folder);
            } catch (System.Exception e) {
                Debug.LogWarning($"Failed to import {folder}: {e.Message}");
            }
            // Store Steamworks ID?
        } else {
            Debug.LogWarning("Attempted to load non-installed Steam Workshop item ID " + publishedFileId);
        }
    }


    void Awake() {
        loadItems = true;
        uploadingItem = null;
        steamItems = new List<SteamUGCDetails_t>();

        GameObject mm = GameObject.Find("ModManager");
        modManager = mm.GetComponent<ModManager>();

        if (SteamManager.Initialized) {
            m_ItemInstalled = Callback<ItemInstalled_t>.Create(OnItemInstalled);
            m_DownloadItemResult = Callback<DownloadItemResult_t>.Create(OnItemDownloaded);
            m_DeleteItemResult = CallResult<DeleteItemResult_t>.Create(OnItemDeleted);
            m_callSteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnUGCSteamUGCQueryCompleted);

            QueryPersonalWorkshopItems();
        }
    }


    // Update is called once per frame
    void Update() {
        if (optionsmenuscript.show_mod_ui) {
            DrawModWindow();
        }
        if (uploadingItem != null && uploadingItem.waiting_for_create) {
            uploadingItem.DrawItemWindow();
        }
    }


    void QueryPersonalWorkshopItems() {
        CSteamID userid = SteamUser.GetSteamID();

        UGCQueryHandle_t query_handle = SteamUGC.CreateQueryUserUGCRequest(
            userid.GetAccountID(),
            EUserUGCList.k_EUserUGCList_Subscribed,
            EUGCMatchingUGCType.k_EUGCMatchingUGCType_All,
            EUserUGCListSortOrder.k_EUserUGCListSortOrder_TitleAsc,
            RECEIVER1_APP_ID,
            RECEIVER1_APP_ID,
            1
        );

        SteamAPICall_t call = SteamUGC.SendQueryUGCRequest(query_handle);
        m_callSteamUGCQueryCompleted.Set(call);
    }


    void DrawModWindow() {
        const float hSpacing = 200.0f;
        ImGui.SetNextWindowSize(new Vector2(480.0f, 300.0f), ImGuiCond.FirstUseEver);

        ImGui.PushStyleColor(ImGuiCol.WindowBg, backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.Button, buttonColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, buttonHoveredColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, buttonActiveColor);
        ImGui.PushStyleColor(ImGuiCol.TitleBgActive, headerColor);

        ImGui.Begin("Mod window");
        ImGui.Text("Local installed mods");
        if (PlayerPrefs.GetInt("mods_enabled", 0) == 1) {
            int i = 0;
            foreach (Mod mod in ModManager.availableMods) {
                ImGui.Text(mod.name);
                ImGui.SameLine(hSpacing);
                ImGui.Text(mod.GetTypeString());
                ImGui.SameLine();
                if (ImGui.Button("Upload to Steam Workshop##" + i)) {
                    if (uploadingItem == null || !uploadingItem.waiting_for_create) {
                        uploadingItem = new SteamworksUGCItem(mod);
                        uploadingItem.waiting_for_create = true;
                    }
                }
                ImGui.SameLine();
                if (mod.loaded) {
                    if (ImGui.Button("Unload##" + i)) {
                        modManager.UnloadMod(mod);
                    }
                } else {
                    if (ImGui.Button("Load##" + i)) {
                        modManager.LoadMod(mod);
                    }
                }
                i++;
            }
        }

        ImGui.Text("Subscribed Steamworks items");
        int j = 0;
        foreach (SteamUGCDetails_t details in steamItems) {
            ImGui.Text(details.m_rgchTitle);
            ImGui.SameLine(hSpacing);
            ImGui.Text(details.m_rgchTags);
            ImGui.SameLine();
            uint itemState = SteamUGC.GetItemState(details.m_nPublishedFileId);
            if ((itemState & (uint)EItemState.k_EItemStateInstalled) == 0) {
                if (ImGui.Button("Install##" + j++)) {
                    SteamUGC.DownloadItem(details.m_nPublishedFileId, false);
                }
            } else {
                if (ImGui.Button("Uninstall (needs restart)##" + j++)) {
                    SteamUGC.UnsubscribeItem(details.m_nPublishedFileId);
                }
            }
        }

        if (ImGui.Button("Close")) {
            optionsmenuscript.show_mod_ui = false;
        }

        ImGui.End();

        ImGui.PopStyleColor(5);
    }
}


public class SteamworksUGCItem {
    public bool waiting_for_create;

    private bool uploading;
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
                uploading = false;
                return;
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
            uploading = false;
            return;
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
        uploading = false;
    }


    public SteamworksUGCItem(Mod _mod) {
        waiting_for_create = false;
        uploading = false;
        visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate;
        mod = _mod;
        title = mod.name;
        description = new char[1024]; description[0] = '\0';
        previewImagePath = new char[512]; previewImagePath[0] = '\0';

        if (SteamManager.Initialized) {
            m_CreateItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
            m_SubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
        }

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
    }


    private void RequestCreation() {
        if (steamworks_id == PublishedFileId_t.Invalid) {
            SteamAPICall_t hSteamAPICall = SteamUGC.CreateItem(SteamScript.RECEIVER1_APP_ID, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
            m_CreateItemResult.Set(hSteamAPICall);
        } else {
            RequestUpload("Update");
        }
        uploading = true;
    }


    private void RequestUpload(string update_message) {
        Debug.Log("Doing Steam Workshop upload");
                
        update_handle = SteamUGC.StartItemUpdate(SteamScript.RECEIVER1_APP_ID, steamworks_id);

        SteamUGC.SetItemTitle(update_handle, title);

        SteamUGC.SetItemDescription(update_handle, new string(description));

        SteamUGC.SetItemUpdateLanguage(update_handle, "english");

        //SteamUGC.SetItemMetadata(update_handle, metadata);

        List<string> tags = new List<string>();
        tags.Add(mod.GetTypeString());
        SteamUGC.SetItemTags(update_handle, tags);

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
            uploading = false;
            return;
        }

        SteamAPICall_t hSteamAPICall = SteamUGC.SubmitItemUpdate(update_handle, update_message);
        m_SubmitItemUpdateResult.Set(hSteamAPICall);
    }


    public void DrawItemWindow() {
        ImGui.PushStyleColor(ImGuiCol.WindowBg, SteamScript.backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.Button, SteamScript.buttonColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, SteamScript.buttonHoveredColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, SteamScript.buttonActiveColor);
        ImGui.PushStyleColor(ImGuiCol.TitleBgActive, SteamScript.headerColor);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, SteamScript.buttonColor);

        ImGui.SetNextWindowSize(new Vector2(500.0f, 300.0f), ImGuiCond.FirstUseEver);
        ImGui.Begin("Steam Workshop item");

        ImGui.Text("Title: " + title);

        ImGui.Text("Type: " + mod.GetTypeString());

        ImGui.InputTextMultiline("Description", description, new Vector2(400.0f, 120.0f));

        // Disabled for now doesn't seem to work (not present in Overgrowth either)?
        // Create automatically?
        //ImGui.InputText("Preview Image", previewImagePath);

        ImGui.Dummy(new Vector2(0.0f, 10.0f));
        
        if (uploading) {
            if (update_handle != UGCUpdateHandle_t.Invalid) {
                ulong bytesProcessed = 0;
                ulong bytesTotal = 1;
                SteamUGC.GetItemUpdateProgress(update_handle, out bytesProcessed, out bytesTotal);
                float progress = bytesProcessed / Math.Max(bytesTotal, 1);
                ImGui.ProgressBar(progress, new Vector2(0.0f, 0.0f));
            }
        } else {
            if (ImGui.Button("Submit")) {
                RequestCreation();
            }
            if (ImGui.Button("Cancel")) {
                waiting_for_create = false;
            }
        }

        ImGui.End();

        ImGui.PopStyleColor(6);
    }
}
