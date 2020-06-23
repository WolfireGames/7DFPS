using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using ImGuiNET;
using SimpleJSON;

public class SteamScript : MonoBehaviour
{
    public static AppId_t RECEIVER1_APP_ID = new AppId_t(234190);

    public static Vector4 backgroundColor = new Vector4(0.65f, 0.65f, 0.65f, 1.0f);
    public static Vector4 buttonColor = new Vector4(0.95f, 0.95f, 0.95f, 1.0f);
    public static Vector4 buttonHoveredColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
    public static Vector4 buttonActiveColor = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
    public static Vector4 buttonTextColor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
    public static Vector4 headerColor = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);

    private bool loadItems;
    private SteamworksUGCItem uploadingItem;
    private List<SteamUGCDetails_t> steamItems;
    private ModManager modManager;

    protected Callback<ItemInstalled_t> m_ItemInstalled;
    protected Callback<DownloadItemResult_t> m_DownloadItemResult;
    private CallResult<DeleteItemResult_t> m_DeleteItemResult;
    private CallResult<SteamUGCQueryCompleted_t> m_callSteamUGCQueryCompleted;


    public static List<string> GetTagList(string tagString) {
        List<string> taglist = new List<string>();
        int start = 0;
        int end = tagString.IndexOf(',');
        while (end != -1) {
            taglist.Add(tagString.Substring(start, end - start));
            start = end + 1;
            end = tagString.IndexOf(',', start);
        }
        taglist.Add(tagString.Substring(start, tagString.Length - start));
        return taglist;
    }


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

        // Preload metadata
        foreach (Mod mod in ModManager.availableMods) {
            mod.steamworksItem = new SteamworksUGCItem(mod);
        }

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
        ImGui.SetNextWindowSize(new Vector2(550.0f, 300.0f), ImGuiCond.FirstUseEver);

        ImGui.PushStyleColor(ImGuiCol.WindowBg, backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.Button, buttonColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, buttonHoveredColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, buttonActiveColor);
        ImGui.PushStyleColor(ImGuiCol.TitleBgActive, headerColor);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, buttonColor);

        ImGui.Begin("Mod window");
        ImGui.Text("Local installed mods");
        if (PlayerPrefs.GetInt("mods_enabled", 0) == 1) {
            int i = 0;
            foreach (Mod mod in ModManager.availableMods) {
                ImGui.Text(mod.name);
                ImGui.SameLine(hSpacing);
                ImGui.Text(mod.GetTypeString());
                ImGui.SameLine(1.2f * hSpacing);
                ImGui.PushStyleColor(ImGuiCol.Text, buttonTextColor);
                if (ImGui.Button("Show info##" + i)) {
                    if (uploadingItem == null || !uploadingItem.waiting_for_create) {
                        uploadingItem = mod.steamworksItem;
                        uploadingItem.waiting_for_create = true;
                    }
                }
                if (ImGui.IsItemHovered()) {
                    ImGui.SetTooltip("Show mod info and Workshop upload window");
                }
                ImGui.SameLine();
                if (mod.loaded) {
                    if (ImGui.Button("Unload##" + i)) {
                        mod.Unload();
                    }
                } else {
                    if (ImGui.Button("Load##" + i)) {
                        mod.Load();
                    }
                }
                ImGui.PopStyleColor(1);
                i++;
            }
        }

        ImGui.Text("Subscribed Steamworks items");
        int j = 0;
        foreach (SteamUGCDetails_t details in steamItems) {
            ImGui.Text(details.m_rgchTitle);
            ImGui.SameLine(hSpacing);
            List<string> tagList = GetTagList(details.m_rgchTags);
            ImGui.Text(tagList[tagList.Count - 1]); // Type tag inserted last
            ImGui.SameLine(1.2f * hSpacing);
            uint itemState = SteamUGC.GetItemState(details.m_nPublishedFileId);
            ImGui.PushStyleColor(ImGuiCol.Text, buttonTextColor);
            if ((itemState & (uint)EItemState.k_EItemStateInstalled) == 0) {
                if (ImGui.Button("Install##" + j)) {
                    SteamUGC.DownloadItem(details.m_nPublishedFileId, false);
                }
            } else {
                if (ImGui.Button("Uninstall (needs restart)##" + j)) {
                    SteamUGC.UnsubscribeItem(details.m_nPublishedFileId);
                }
                ImGui.SameLine();
                if (ImGui.Button("Show in Steam##" + j)) {
                    string itemPath = "steam://url/CommunityFilePage/" + details.m_nPublishedFileId.ToString();
                    SteamFriends.ActivateGameOverlayToWebPage(itemPath);
                }
                if (ImGui.IsItemHovered()) {
                    ImGui.SetTooltip("Open Steam overlay to Workshop page");
                }
            }
            ImGui.PopStyleColor(1);
            j++;
        }

        ImGui.PushStyleColor(ImGuiCol.Text, buttonTextColor);
        if (ImGui.Button("Close")) {
            optionsmenuscript.show_mod_ui = false;
        }
        ImGui.PopStyleColor(1);

        ImGui.End();

        ImGui.PopStyleColor(6);
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
    private char[] name;
    private char[] description;
    private char[] tags;
    private char[] author;
    private char[] version;

    private CallResult<CreateItemResult_t> m_CreateItemResult;
    private CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdateResult;


    private string GetChars(char[] text) {
        string t = new string(text);
        int pos = t.IndexOf('\0');
        return t.Substring(0, pos);
    }


    private void CopyChars(string source, char[] dest) {
        int i = 0;
        while (i < source.Length && i < dest.Length - 1) {
            dest[i] = source[i];
            i++;
        }
        dest[i] = '\0';
    }


    private void OnCreateItemResult(CreateItemResult_t pResult, bool failed) {
        if (failed == false) {
            if (pResult.m_eResult != EResult.k_EResultOK) {
                Debug.LogError("Steam CreateItem error " + pResult.m_eResult.ToString());
                uploading = false;
                return;
            } else {
                steamworks_id = pResult.m_nPublishedFileId;

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
        name = new char[1024];
        CopyChars(title, name);
        description = new char[1024]; description[0] = '\0';
        tags = new char[512]; tags[0] = '\0';
        author = new char[256]; author[0] = '\0';
        version = new char[128]; version[0] = '\0';

        if (SteamManager.Initialized) {
            m_CreateItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
            m_SubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
        }

        // Load metadata
        string metaPath = Path.GetDirectoryName(mod.path) + "/metadata.json";
        if (File.Exists(metaPath)) {
            try {
                string metaText = File.ReadAllText(metaPath);
                JSONNode jnRoot = JSON.Parse(metaText);

                CopyChars(jnRoot["name"].Value, name);
                CopyChars(jnRoot["description"].Value, description);
                CopyChars(jnRoot["tags"].Value, tags);
                CopyChars(jnRoot["author"].Value, author);
                CopyChars(jnRoot["version"].Value, version);
                steamworks_id = new PublishedFileId_t((ulong)jnRoot["steamworks_id"].AsLong);
                mod.name = GetChars(name);
            } catch (Exception e) {
                Debug.LogError("Error reading metadata for mod: " + e);
            }
        }
    }


    private void UpdateMetadata() {
        JSONObject jn = new JSONObject();
        jn.Add("name", new JSONString(GetChars(name)));
        jn.Add("description", new JSONString(GetChars(description)));
        jn.Add("tags", new JSONString(GetChars(tags)));
        jn.Add("author", new JSONString(GetChars(author)));
        jn.Add("version", new JSONString(GetChars(version)));
        jn.Add("steamworks_id", new JSONNumber(steamworks_id.m_PublishedFileId));

        string metaPath = Path.GetDirectoryName(mod.path) + "/metadata.json";
        try {
            File.Create(metaPath).Close();
            File.WriteAllText(metaPath, jn.ToString());
        } catch (Exception e) {
            Debug.LogError("Failed to write metadata for mod: " + e);
        }

        mod.name = GetChars(name);
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

        // Store metadata
        UpdateMetadata();

        update_handle = SteamUGC.StartItemUpdate(SteamScript.RECEIVER1_APP_ID, steamworks_id);

        SteamUGC.SetItemTitle(update_handle, GetChars(name));

        SteamUGC.SetItemDescription(update_handle, GetChars(description));

        SteamUGC.SetItemUpdateLanguage(update_handle, "english");

        JSONObject jn = new JSONObject();
        jn.Add("author", new JSONString(GetChars(author)));
        jn.Add("version", new JSONString(GetChars(version)));

        SteamUGC.SetItemMetadata(update_handle, jn.ToString());

        // Add custom tags
        string tagString = GetChars(tags);
        List<string> taglist = SteamScript.GetTagList(tagString);
        taglist.Add(mod.GetTypeString());   // Add mod type tag

        SteamUGC.SetItemTags(update_handle, taglist);

        SteamUGC.SetItemVisibility(update_handle, visibility);

        // TODO: create automatically?
        /*
        string path = new string(previewImagePath);
        if (File.Exists(path)) {
            SteamUGC.SetItemPreview(update_handle, path);
        }
        */

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
        ImGui.PushStyleColor(ImGuiCol.FrameBg, SteamScript.buttonActiveColor);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, SteamScript.buttonColor);

        ImGui.SetNextWindowSize(new Vector2(500.0f, 350.0f), ImGuiCond.FirstUseEver);
        ImGui.Begin("Local mod info");

        ImGui.Text("Type: " + mod.GetTypeString());

        ImGui.InputText("Title", name);

        ImGui.InputTextMultiline("Description", description, new Vector2(400.0f, 120.0f));

        ImGui.InputText("Tags (comma separated)", tags);
        if (ImGui.IsItemHovered()) {
            ImGui.PushStyleColor(ImGuiCol.Text, SteamScript.buttonTextColor);
            ImGui.SetTooltip("Tags for Steam Workshop items. Mod type is always added.");
            ImGui.PopStyleColor(1);
        }

        ImGui.InputText("Author", author);

        ImGui.InputText("Version", version);

        ImGui.Dummy(new Vector2(0.0f, 10.0f));
        
        if (uploading) {
            if (update_handle != UGCUpdateHandle_t.Invalid) {
                ulong bytesProcessed = 0;
                ulong bytesTotal = 1;
                EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(update_handle, out bytesProcessed, out bytesTotal);
                switch (status) {
                    case EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges:
                        ImGui.Text("Committing Changes");
                        break;
                    case EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig:
                        ImGui.Text("Preparing Config");
                        break;
                    case EItemUpdateStatus.k_EItemUpdateStatusPreparingContent:
                        ImGui.Text("Preparing Content");
                        break;
                    case EItemUpdateStatus.k_EItemUpdateStatusUploadingContent:
                        ImGui.Text("Uploading Content");
                        break;
                    case EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile:
                        ImGui.Text("Uploading Preview File");
                        break;
                    default:
                        break;
                }
                float progress = bytesProcessed / Math.Max(bytesTotal, 1);
                ImGui.ProgressBar(progress, new Vector2(0.0f, 0.0f));
            }
        } else {
            ImGui.PushStyleColor(ImGuiCol.Text, SteamScript.buttonTextColor);
            if (ImGui.Button("Upload to Workshop")) {
                RequestCreation();
            }
            if (ImGui.Button("Update metadata")) {
                UpdateMetadata();
            }
            if (ImGui.IsItemHovered()) {
                ImGui.SetTooltip("Update local mod metadata without uploading to Steam Workshop");
            }
            if (ImGui.Button("Close")) {
                waiting_for_create = false;
            }
            ImGui.PopStyleColor(1);
        }

        ImGui.End();

        ImGui.PopStyleColor(7);
    }
}
