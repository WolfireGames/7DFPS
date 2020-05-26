using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using ImGuiNET;

public class SteamScript : MonoBehaviour
{
    string steamName;

    // Start is called before the first frame update
    void Start()
    {
        if (SteamManager.Initialized) {
            steamName = SteamFriends.GetPersonaName();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ImGui.Begin("Steam Test Window");
        ImGui.Text("User Steam Name: " + steamName);
        ImGui.End();
    }
}
