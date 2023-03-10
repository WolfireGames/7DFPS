using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvertismentScript : MonoBehaviour {
    public void OpenLink() {
        if(SteamAPI.IsSteamRunning()) {
            SteamFriends.ActivateGameOverlayToStore(new Steamworks.AppId_t(1129310), EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
        } else {
            Application.OpenURL("https://store.steampowered.com/app/1129310/Receiver_2/");
        }
    }
}
