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
            for (uint i = 0; i < pResult.m_unNumResultsReturned; i++) {
                SteamUGCDetails_t details;
                SteamUGC.GetQueryUGCResult(pResult.m_handle, i, out details);
                // Only load items when explicitly requested by something
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
                foreach (Mod m in ModManager.importedMods) {
                    if (m.path.Contains(folder)) {
                        // Don't load twice
                        return;
                    }
                }
                
                // Register new mod in the ModManager
                ModImporter.ImportMod(folder, false);
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

            if (uploadingItem != null && uploadingItem.waiting_for_create) {
                uploadingItem.DrawItemWindow();
            }
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
        ImGui.PushStyleColor(ImGuiCol.ResizeGrip, buttonColor);
        ImGui.PushStyleColor(ImGuiCol.ResizeGripHovered, buttonActiveColor);
        ImGui.PushStyleColor(ImGuiCol.ResizeGripActive, buttonActiveColor);

        ImGui.Begin("Mod window");
        ImGui.Text("Installed mods"); // TODO gray them out when mods are not enabled
        
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, buttonTextColor);
        if(ImGui.Button("Refresh Steam")) {
            QueryPersonalWorkshopItems();
            loadItems = true;
        }
        ImGui.PopStyleColor(1);

        for (int i = 0; i < ModManager.importedMods.Count; i++) {
            Mod mod = ModManager.importedMods[i];

            ImGui.Text(mod.steamworksItem.GetName());
            ImGui.SameLine(hSpacing);
            ImGui.Text(mod.GetTypeString());
            ImGui.SameLine(1.2f * hSpacing);
            ImGui.PushStyleColor(ImGuiCol.Text, buttonTextColor);
            if (ImGui.Button("Show info##" + i)) {
                if (uploadingItem != null) {
                    uploadingItem.waiting_for_create = false;
                }
                uploadingItem = mod.steamworksItem;
                uploadingItem.waiting_for_create = true;
            }
            if (ImGui.IsItemHovered()) {
                ImGui.SetTooltip("Show mod info and Workshop upload window");
            }
            ImGui.PopStyleColor(1);
            
            ImGui.SameLine();
            ImGui.Checkbox($"Disabled ##{mod.path}", ref mod.ignore);

            ImGui.PushStyleColor(ImGuiCol.Text, buttonTextColor);
            if(!mod.IsLocalMod()) {
                ImGui.SameLine();
                if (ImGui.Button("Unsubscribe##" + i)) {
                    ModManager.importedMods.Remove(mod);

                    SteamUGC.UnsubscribeItem(mod.steamworksItem.steamworks_id);
                    QueryPersonalWorkshopItems();
                    i--;
                    continue;
                }
                ImGui.SameLine();
                if (ImGui.Button("Show in Steam##" + i)) {
                    string itemPath = $"steam://url/CommunityFilePage/{mod.steamworksItem.steamworks_id}";
                    SteamFriends.ActivateGameOverlayToWebPage(itemPath);
                }
                if (ImGui.IsItemHovered()) {
                    ImGui.SetTooltip("Open Steam overlay to Workshop page");
                }
            }
            ImGui.PopStyleColor(1);
        }

        ImGui.PushStyleColor(ImGuiCol.Text, buttonTextColor);
        if (ImGui.Button("Close")) {
            optionsmenuscript.show_mod_ui = false;
        }
        ImGui.PopStyleColor(1);

        ImGui.End();

        ImGui.PopStyleColor(9);
    }
}


public class SteamworksUGCItem {
    public bool waiting_for_create;

    private bool uploading;
    public PublishedFileId_t steamworks_id;
    private ERemoteStoragePublishedFileVisibility visibility;
    private UGCUpdateHandle_t update_handle;

    private Mod mod;
    private char[] name;
    private char[] description;
    private char[] tags;
    private char[] author;
    private char[] version;

    private CallResult<CreateItemResult_t> m_CreateItemResult;
    private CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdateResult;

    public string GetName() {
        return GetChars(name);
    }

    public void SetName(string name) {
        this.name = new char[1024];
        CopyChars(name, this.name);
        UpdateMetadata();
    }

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
        if(pResult.m_bUserNeedsToAcceptWorkshopLegalAgreement) {
            Debug.LogError("Player needs to agree to the user agreement.");
        }
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
        name = new char[1024];
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
    
    private void RequestPreviewUpload(string update_message) {
        Debug.Log("Doing Steam Workshop preview update");

        // Store metadata
        UpdateMetadata();

        update_handle = SteamUGC.StartItemUpdate(SteamScript.RECEIVER1_APP_ID, steamworks_id);

        string previewImagePath = Path.Combine(Path.GetDirectoryName(mod.path), "thumbnail.png");
        if (File.Exists(previewImagePath)) {
            if(SteamUGC.SetItemPreview(update_handle, previewImagePath) == false) {
                Debug.LogError("SetItemPreview failed");
            }
        } else {
            Debug.LogError("Preview image path \"" + previewImagePath + "\" is invalid.");
        }

        SteamAPICall_t hSteamAPICall = SteamUGC.SubmitItemUpdate(update_handle, update_message);
        m_SubmitItemUpdateResult.Set(hSteamAPICall);
    }

    private void RequestUpload(string update_message) {
        Debug.Log("Doing Steam Workshop upload");

        // Store metadata
        UpdateMetadata();

        update_handle = SteamUGC.StartItemUpdate(SteamScript.RECEIVER1_APP_ID, steamworks_id);

        if(SteamUGC.SetItemTitle(update_handle, GetChars(name)) == false) {
            Debug.LogError("SetItemTitle failed");
        }

        if(SteamUGC.SetItemDescription(update_handle, GetChars(description)) == false) {
            Debug.LogError("SetItemDescription failed");
        }

        if(SteamUGC.SetItemUpdateLanguage(update_handle, "english") == false) {
            Debug.LogError("SetItemUpdateLanguage failed");
        }

        JSONObject jn = new JSONObject();
        jn.Add("author", new JSONString(GetChars(author)));
        jn.Add("version", new JSONString(GetChars(version)));

        if(SteamUGC.SetItemMetadata(update_handle, jn.ToString()) == false) {
            Debug.LogError("SetItemMetadata failed");
        }

        // Add custom tags
        string tagString = GetChars(tags);
        List<string> taglist = SteamScript.GetTagList(tagString);
        taglist.Add(mod.GetTypeString());   // Add mod type tag

        if(SteamUGC.SetItemTags(update_handle, taglist) == false) {
            Debug.LogError("SetItemTags failed");
        }

        // Add preview image
        string thumbnailPath = Path.Combine(Path.GetDirectoryName(mod.path), "thumbnail.png");
        if (File.Exists(thumbnailPath)) {
            SteamUGC.SetItemPreview(update_handle, thumbnailPath);
        }

        if(SteamUGC.SetItemVisibility(update_handle, visibility) == false) {
            Debug.LogError("SetItemVisibility failed");
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
        ImGui.PushStyleColor(ImGuiCol.FrameBg, SteamScript.buttonActiveColor);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, SteamScript.buttonColor);
        ImGui.PushStyleColor(ImGuiCol.ResizeGrip, SteamScript.buttonColor);
        ImGui.PushStyleColor(ImGuiCol.ResizeGripHovered, SteamScript.buttonActiveColor);
        ImGui.PushStyleColor(ImGuiCol.ResizeGripActive, SteamScript.buttonActiveColor);

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

        ImGui.Image(mod.thumbnail, Vector2.one * 100);

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
            if (steamworks_id == PublishedFileId_t.Invalid) {
                if (ImGui.Button("Upload to Workshop")) {
                    RequestCreation();
                }
            } else {
                if (ImGui.Button("Update Workshop item")) {
                    RequestCreation();
                }
            }
            if (ImGui.Button("Update Workshop preview image")) {
                RequestPreviewUpload("Update preview");
            }
            if (ImGui.Button("Update local metadata")) {
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

        ImGui.PopStyleColor(10);
    }
}
