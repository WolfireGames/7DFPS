namespace ImGuiNET
{
    [System.Flags]
    public enum ImRasterizerFlags
    {
        None = 0,
        NoHinting = 1 << 0,
        NoAutoHint = 1 << 1,
        ForceAutoHint = 1 << 2,
        LightHinting = 1 << 3,
        MonoHinting = 1 << 4,
        Bold = 1 << 5,
        Oblique = 1 << 6,
    }
}
