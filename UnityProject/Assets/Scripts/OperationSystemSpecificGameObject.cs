using UnityEngine;

public class OperationSystemSpecificGameObject : MonoBehaviour {
    [Header("Supported System Families")]
    public bool windows = true;
    public bool linux = true;
    public bool mac = true;
    public bool others = true;

    void Awake() {
        switch (SystemInfo.operatingSystemFamily) {
            case OperatingSystemFamily.Windows:
                if(windows) return;
                break;
            case OperatingSystemFamily.Linux:
                if(linux) return;
                break;
            case OperatingSystemFamily.MacOSX:
                if(mac) return;
                break;
            case OperatingSystemFamily.Other:
                if(others) return;
                break;
            default:
                return;
        }

        GameObject.Destroy(this.gameObject);
    }
}
