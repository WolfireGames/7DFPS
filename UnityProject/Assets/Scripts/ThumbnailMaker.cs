using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class ThumbnailMaker : MonoBehaviour {
    public Texture2D overlay;

    public Camera cam;
    public GameObject ground;
    public GameObject tapeScene;
    public GameObject[] cassetteTapes;

    public Bounds last;
    void Update() {
        if(last != null) { // DEBUG TODO this is for testing positioning inside the scene
            PositionCameraToFit(last);
            SetClippingDistance();
        }
    }

    /// <summary> positions the camera to fully show the selected bounds, topPadding allows to reserve "1 - 1/(1+n)"% of the topside for the logo </summary>
    public void PositionCameraToFit(Bounds bounds, float topPadding = 0.285f) {
        // Save converted camera rotation
        var rot = cam.transform.rotation.eulerAngles * Mathf.Deg2Rad;

        // Get required screen height
        float x1 = Mathf.Abs(last.extents.x * Mathf.Sin(rot.y) * Mathf.Sin(rot.x));
        float y1 = Mathf.Abs(last.extents.y * Mathf.Cos(rot.x));
        float z1 = Mathf.Abs(last.extents.z * Mathf.Sin(rot.x) * Mathf.Cos(rot.y));
        float requiredHeight = x1 + y1 + z1;

        // Get required screen width
        float x2 = Mathf.Abs(last.extents.x * Mathf.Cos(rot.y));
        float y2 = 0;
        float z2 = Mathf.Abs(last.extents.z * Mathf.Sin(rot.y));
        float requiredWidth = x2 + y2 + z2;

        cam.orthographicSize = Mathf.Max(requiredWidth, requiredHeight * (1 + topPadding));
        
        // Move camera far enough away to not be inside the bounds
        cam.transform.position = last.center - cam.transform.forward * Mathf.Sqrt(last.extents.x * last.extents.x + last.extents.z * last.extents.z + last.extents.y * last.extents.y) - cam.transform.forward
             + cam.transform.up * requiredHeight * topPadding;
    }

    private void PrepareTile(GameObject tiles) {
        ModTilesHolder modTilesHolder = tiles.GetComponent<ModTilesHolder>();
        if(modTilesHolder.tile_prefabs.Length <= 0)
            throw new System.ArgumentException("Tile mod doesn't have any tiles!");

        // Prepare Target
        GameObject model = Instantiate(modTilesHolder.tile_prefabs[0], Vector3.zero, Quaternion.identity);

        // Determine bounds
        Bounds bounds = GetBounds(model);
        ClampBoundsY(ref bounds);
        DrawBounds(bounds, Color.red, 10); // DEBUG
        last = bounds;

        // Move Camera in place
        cam.transform.rotation = Quaternion.Euler(35f, -65, 0);
        PositionCameraToFit(bounds);
    }

    private void SetClippingDistance() {
        var angle = cam.transform.eulerAngles.x;
        var highestY = cam.transform.position.y + cam.orthographicSize * Mathf.Cos(angle * Mathf.Deg2Rad);
        cam.farClipPlane = highestY / Mathf.Sin(angle * Mathf.Deg2Rad) + 0.1f;
    }

    private void PrepareGun(GameObject gun) {
        float cameraAngle = 35f;
        WeaponHolder weaponHolder = gun.GetComponent<WeaponHolder>();
        weaponHolder.Load();

        GameObject ammo = weaponHolder.bullet_object;
        gun = weaponHolder.gun_object;
        gun.GetComponent<GunScript>().enabled = false; // We don't want OnEnabled to be called in edit mode

        // Prepare Target
        GameObject model = Instantiate(gun, Vector3.zero, Quaternion.identity);
        model.transform.rotation = Quaternion.Euler(Vector3.forward * 90f + Vector3.right * 180);

        // Determine bounds
        Bounds bounds = GetBounds(model);

        // Translate object to match the ground
        model.transform.position = Vector3.up * (bounds.size.y - bounds.center.y);
        bounds.center = bounds.center + Vector3.up * (bounds.size.y - bounds.center.y);

        bounds = GetBounds(model);
        last = bounds;
        
        DrawBounds(bounds, Color.red, 10); // DEBUG

        // Move Camera in place
        cam.transform.rotation = Quaternion.Euler(60f, cameraAngle - 90, 0);
        PositionCameraToFit(bounds);
    }

    private void PrepareTape(GameObject tapes) {
        // Enable cassett tape gameobjects depending on how many tapes are inside the mod
        ModTapesHolder modTapesHolder = tapes.GetComponent<ModTapesHolder>();
        for (int i = 0; i < cassetteTapes.Length; i++) {
            cassetteTapes[i].SetActive(i < modTapesHolder.tapes.Length);
        }
        tapeScene.SetActive(true);

        // Determine bounds
        Bounds bounds = GetBounds(tapeScene);
        DrawBounds(bounds, Color.red, 10); // DEBUG
        last = bounds;

        // Move Camera in place
        cam.transform.rotation = Quaternion.Euler(45, -120, 0);
        PositionCameraToFit(bounds);
    }

    private void PrepareScene(Mod mod) {
        switch (mod.modType) {
            case ModType.Gun:
                PrepareGun(mod.mainAsset);
                break;
            case ModType.Tapes:
                PrepareTape(mod.mainAsset);
                break;
            case ModType.LevelTile:
                PrepareTile(mod.mainAsset);
                break;
            default:
                Debug.LogWarning($"Thumbnail creation doesn't know how to frame a mod of type {mod.modType}, will default to framing a LevelTile!");
                PrepareTile(mod.mainAsset);
                break;
        }

        // Set the camera's clipping plane back far enough to be fully filled by the ground plane
        SetClippingDistance();
    }

    public Texture2D CreateThumbnail(Mod mod) {
        int size = 450;
        
        // Prepare Camera
        RenderTexture renderTexture = RenderTexture.GetTemporary(size, size);
        RenderTexture temp = RenderTexture.active;
        RenderTexture.active = renderTexture;
        cam.targetTexture = renderTexture;

        PrepareScene(mod);

        // Photo Time
        cam.Render();

        Texture2D texture = new Texture2D(size, size);
        texture.ReadPixels(new Rect(0, 0, size, size), 0, 0);

        Texture2D texture2 = new Texture2D(size, size);
        texture2.SetPixels(overlay.GetPixels());
        texture2.Apply();

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                Color a = texture.GetPixel(x, y);
                Color b = overlay.GetPixel(x, y);
                texture.SetPixel(x, y, a * (1-b.a) + b * (b.a));
            }
        }
        texture.Apply();

        // Cleanup / Returning
        RenderTexture.active = temp;
        renderTexture.Release();
        RenderTexture.ReleaseTemporary(renderTexture);
        cam.targetTexture = null;

        GameObject.DestroyImmediate(this.gameObject);

        return texture;
    }

    private Bounds GetBounds(GameObject gameObject) {
        Bounds bounds = new Bounds();
        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            bounds.Encapsulate(renderer.bounds);
        
        return bounds;
    }

    private void ClampBoundsY(ref Bounds bounds) {
        if(bounds.center.y - bounds.extents.y < 0) {
            var zero_height = bounds.center.y + bounds.extents.y;
            bounds.center = new Vector3(bounds.center.x, zero_height / 2f, bounds.center.z);
            bounds.size = new Vector3(bounds.size.x, zero_height, bounds.size.z);
        }
    }

    // DEBUG method
    public static void DrawBounds(Bounds b, Color color, float duration) {
        Vector3[] points = new Vector3[] {
            b.center + new Vector3( b.extents.x,  b.extents.y,  b.extents.z),
            b.center + new Vector3( b.extents.x,  b.extents.y, -b.extents.z),
            b.center + new Vector3( b.extents.x, -b.extents.y,  b.extents.z),
            b.center + new Vector3( b.extents.x, -b.extents.y, -b.extents.z),
            b.center + new Vector3(-b.extents.x,  b.extents.y,  b.extents.z),
            b.center + new Vector3(-b.extents.x,  b.extents.y, -b.extents.z),
            b.center + new Vector3(-b.extents.x, -b.extents.y,  b.extents.z),
            b.center + new Vector3(-b.extents.x, -b.extents.y, -b.extents.z),
        };
        
        foreach (var p1 in points)
            foreach (var p2 in points)
                if(p1 != p2)
                    if(p1.x == p2.x && p1.z == p2.z || p1.y == p2.y && (p1.x == p2.x || p1.z == p2.z))
                        Debug.DrawLine(p1, p2, color, duration);
    }
}
