using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Generator : MonoBehaviour {
    [SerializeField] private Module[] modules;
    [SerializeField] private Module initialModule;
    [SerializeField] private Module fallbackModule;
    [SerializeField] private Module foundationModule;

    private int blockSize = 2;

    private Grid3<Module> grid = new Grid3<Module>(6, 4, 15);
    private List<Vector3Int> lockedFields = new List<Vector3Int>();

    [ContextMenu("Clear Level")]
    private void ClearLevel() {
        lockedFields.Clear();
        grid.Clear();
        for (int i = transform.childCount - 1; i >= 0 ; i--) {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    [ContextMenu("GenerateLevel")]
    private void GenerateLevel() {
        ClearLevel();

        // Generate initial module
        Vector3Int center = new Vector3Int(grid.Width / 2, 1, grid.Depth / 2);
        SetField(center, initialModule);

        // Generate lowest layer
        if (foundationModule != null) {
            for (int x = 0; x < grid.Width; x++) {
                for (int z = 0; z < grid.Depth; z++) {
                    SetField(new Vector3Int(x, 0, z), foundationModule);
                }
            }
        }

        // Generate the rest of the level
        var iterations = 0;
        while (TryGenerateField()) {
            if (iterations > 10000) {
                Debug.LogError("Level generation aborted after 1000 iterations");
                break;
            }

            iterations++;
        }
    }

    private void SetField(Vector3Int position, Module module) {
        Module instance = Instantiate(module, transform);
        instance.transform.position = position * blockSize;

        grid[position] = instance;
        lockedFields.Add(position);
    }

    private Dictionary<Direction, Module> GetNeighborModules(Vector3Int position) {
        Dictionary<Direction, Module> neighbors = new Dictionary<Direction, Module>();
        if (grid.TryGet(position + Vector3Int.up, out Module moduleUp)) neighbors[Direction.Up] = moduleUp;
        if (grid.TryGet(position + Vector3Int.down, out Module moduleDown)) neighbors[Direction.Down] = moduleDown;
        if (grid.TryGet(position + Vector3Int.left, out Module moduleLeft)) neighbors[Direction.Left] = moduleLeft;
        if (grid.TryGet(position + Vector3Int.right, out Module moduleRight)) neighbors[Direction.Right] = moduleRight;
        if (grid.TryGet(position + new Vector3Int(0, 0, 1), out Module moduleForward)) neighbors[Direction.Forward] = moduleForward;
        if (grid.TryGet(position + new Vector3Int(0, 0, -1), out Module moduleBack)) neighbors[Direction.Back] = moduleBack;
        return neighbors;
    }

    private List<Module> GetPossibleModules(Vector3Int position) {
        List<Module> possibleModules = modules.ToList();
        foreach (var keyValuePair in GetNeighborModules(position)) {
            // No change if there is no neighbor
            if (keyValuePair.Value == null) continue;

            // Exclude modules that don't fit with the neighbor
            for (int i = 0; i < possibleModules.Count; i++) {
                if (!keyValuePair.Value.FitsWithSocket(possibleModules[i].GetSocket(keyValuePair.Key))) {
                    possibleModules.RemoveAt(i);
                    i--;
                }
            }
        }

        if (possibleModules.Count == 0) {
            possibleModules.Add(fallbackModule);
        }

        return possibleModules;
    }

    private Vector3Int[] GetNeighborCoords(Vector3Int position) {
        Vector3Int[] positions = new Vector3Int[6];
        positions[0] = position + Vector3Int.up;
        positions[1] = position + Vector3Int.down;
        positions[2] = position + Vector3Int.left;
        positions[3] = position + Vector3Int.right;
        positions[4] = position + new Vector3Int(0, 0, 1);
        positions[5] = position + new Vector3Int(0, 0, -1);
        return positions;
    }

    [ContextMenu("Try Generate Single Field")]
    public bool TryGenerateField() {
        // Find the field with the least options
        Dictionary<Vector3Int, List<Module>> possibleFields = new Dictionary<Vector3Int, List<Module>>();
        foreach (Vector3Int lockedPosition in lockedFields) {
            foreach (Vector3Int neighborCoord in GetNeighborCoords(lockedPosition)) {
                if (grid.IsInBounds(neighborCoord) && !lockedFields.Contains(neighborCoord)) {
                    possibleFields[neighborCoord] = GetPossibleModules(neighborCoord);
                }
            }
        }

        // If there are no options, we're done
        if (possibleFields.Count == 0) {
            Debug.Log("Unable to generate more fields");
            return false;
        }

        // Attempt to clean out impossible modules
        foreach (var brokenField in possibleFields.Where((x) => x.Value.Count == 0).Select((x) => x.Key)) {
            foreach (var toBeDeleted in GetNeighborCoords(brokenField)) {
                // TODO this can remove the starting field lol
                if (grid.TryGet(toBeDeleted, out _)) {
                    lockedFields.RemoveAll((x) => x == toBeDeleted);
                    GameObject.DestroyImmediate(grid[toBeDeleted]);
                    grid[toBeDeleted] = null;
                    return true;
                }
            }
        }

        KeyValuePair<Vector3Int, List<Module>> next = possibleFields.Where((x) => x.Value.Count > 0 ).OrderBy(pair => pair.Value.Count).First();
        Module newModule = WeightedRandom(next.Value, next.Value.Select( (x) => x.weight));
        SetField(next.Key, newModule);
        return true;
    }

    private T WeightedRandom<T>(IEnumerable<T> list, IEnumerable<float> weights) {
        Debug.Assert(list.Count() == weights.Count());

        if (list.Count() == 1) {
            return list.First();
        }

        float totalWeight = weights.Sum();
        float randomPoint = Random.value * totalWeight;
        foreach (var item in list.Zip(weights, (item, weight) => new { item, weight })) {
            if (randomPoint < item.weight) {
                return item.item;
            }
            randomPoint -= item.weight;
        }
        return default(T);
    }
}

public class Grid3<T> : IEnumerable {
    private T[,,] grid;
    private int width;
    private int height;
    private int depth;

    public Grid3(int width, int height, int depth) {
        this.width = width;
        this.height = height;
        this.depth = depth;
        grid = new T[width, height, depth];
    }

    public T this[Vector3Int position] {
        get {
            return this[position.x, position.y, position.z];
        }
        set {
            this[position.x, position.y, position.z] = value;
        }
    }

    public T this[int x, int y, int z] {
        get {
            return grid[x, y, z];
        }
        set {
            grid[x, y, z] = value;
        }
    }

    public bool IsInBounds(Vector3Int position) {
        return IsInBounds(position.x, position.y, position.z);
    }

    public bool IsInBounds(int x, int y, int z) {
        return x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth;
    }

    public void Clear() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < depth; z++) {
                    grid[x, y, z] = default(T);
                }
            }
        }
    }

    public int Width => width;
    public int Height => height;
    public int Depth => depth;

    public bool TryGet(Vector3Int position, out T value) {
        return TryGet(position.x, position.y, position.z, out value);
    }

    public bool TryGet(int x, int y, int z, out T value) {
        if (!IsInBounds(x, y, z)) {
            value = default(T);
            return false;
        }

        value = grid[x, y, z];
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return grid.GetEnumerator();
    }
}
