using UnityEngine;
using ImGuiNET;

public class ImGuiMesh {
    public Mesh mesh;

    private Vector3[] vertices;
    private Vector2[] uv;
    private Color32[] colors;
    private int[] triangles;


    public ImGuiMesh()
    {
        mesh = new Mesh();

        const int initialSize = 128;
        vertices = new Vector3[initialSize];
        uv       = new Vector2[initialSize];
        colors   = new Color32[initialSize];
        triangles = new int[initialSize];
    }


    public void UpdateMesh(ImDrawCmd cmd, ImVector_ushort idxBuffer, ImVector_ImDrawVert vtxBuffer, int startIndex)
    {
        mesh.Clear();

        // recreate arrays only if too small
        if (vertices.Length < vtxBuffer.Size)
        {
            vertices = new Vector3[vtxBuffer.Size];
            uv       = new Vector2[vtxBuffer.Size];
            colors   = new Color32[vtxBuffer.Size];
        }

        // Unity can't hoist Screen.height fetch from the loop
        // Fine, we'll do it ourselves
        int screenHeight = Screen.height;
        // populate mesh from draw data
        for (int i = 0; i < vtxBuffer.Size; i++)
        {
            ImDrawVert vertex = vtxBuffer[i];
            vertices[i].x = vertex.pos.x;
            vertices[i].y = screenHeight - vertex.pos.y;
            vertices[i].z = 0.0f;
            uv[i].x = vertex.uv.x;
            uv[i].y = vertex.uv.y;
            colors[i].a = (byte)((vertex.col >> 24) & 255);
            colors[i].b = (byte)((vertex.col >> 16) & 255);
            colors[i].g = (byte)((vertex.col >> 8) & 255);
            colors[i].r = (byte)((vertex.col) & 255);
        }

        if (triangles.Length != cmd.ElemCount)
        {
            triangles = new int[cmd.ElemCount];
        }
        for (int i = 0; i < cmd.ElemCount; i++)
        {
            triangles[i] = idxBuffer[i + startIndex];
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.colors32 = colors;
        mesh.triangles = triangles;
    }
}
