using UnityEngine;

namespace GunSystemInterfaces {
    public delegate bool ConnectMagazine(GameObject mag);
    public delegate GameObject DisconnectMagazine();
    public delegate void SpinCylinder(int amount);
    public delegate Vector2 GetRecoilTransfer();
    public delegate Vector2 GetRecoilRotation();
    public delegate bool AddHeadRecoil();
}