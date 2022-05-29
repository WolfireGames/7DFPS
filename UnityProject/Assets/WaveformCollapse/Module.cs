using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Direction {
    Up,
    Down,
    Left,
    Right,
    Forward,
    Back,
}

[System.Serializable]
public struct Socket {
    public Direction facing;
    public string model;

    public Socket(Direction facing, string model) {
        this.facing = facing;
        this.model = model;
    }
}

public class Module : MonoBehaviour {
    // TODO there can be optimizations made when the module is mirrored

    // TODO we want to make use of Unity's Serialization, but having a Dictionary<Direction, Socket> here might be cleaner
    [SerializeField] private Socket[] sockets;
    [SerializeField] public float weight = 1;

    // Returns a socket facing the given direction, returns invalid socket (with no module) if no socket is found
    public Socket GetSocket(Direction direction) {
        return sockets.FirstOrDefault((x) => x.facing == direction);
    }

    public void SetSocket(Direction direction, string model) {
        List<Socket> temp = sockets.ToList();
        temp.RemoveAll((x) => x.facing == direction);
        temp.Add(new Socket(direction, model));
        sockets = temp.ToArray();
    }

    public bool FitsWithSocket(Socket socket) {
        Socket connectingSocket = GetSocket(GetInverseDirection(socket.facing));
        return connectingSocket.model == socket.model || string.IsNullOrEmpty(socket.model) || string.IsNullOrEmpty(connectingSocket.model);
    }

    private static Direction GetInverseDirection(Direction direction) {
        switch (direction) {
            case Direction.Up: return Direction.Down;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            case Direction.Forward: return Direction.Back;
            case Direction.Back: return Direction.Forward;
        }
        throw new System.Exception("Invalid direction");
    }

    private void Reset() {
        sockets = new Socket[] {
            new Socket(Direction.Up, ""),
            new Socket(Direction.Down, ""),
            new Socket(Direction.Left, ""),
            new Socket(Direction.Right, ""),
            new Socket(Direction.Forward, ""),
            new Socket(Direction.Back, ""),
        };
    }

    [ContextMenu("Generate Alternatives")]
    private void GenerateRotationAlternatives() {
        for (int i = 1; i < 4; i++) {
            GameObject alternative = Instantiate(gameObject);
            Module newModule = alternative.GetComponent<Module>();

            alternative.name = $"{gameObject.name} {90 * i}°";

            // Rotate gameobjects
            foreach (Transform child in alternative.transform) {
                child.RotateAround(alternative.transform.position + Vector3.one, Vector3.up, 90 * i);
            }

            // Offset alternative
            alternative.transform.position += Vector3.forward * i * 2;

            // Rotate sockets
            Socket[] swaping = new Socket[4];
            swaping[0] = GetSocket(Direction.Left);
            swaping[1] = GetSocket(Direction.Back);
            swaping[2] = GetSocket(Direction.Right);
            swaping[3] = GetSocket(Direction.Forward);

            for (int j = 0; j < 4; j++) {
                newModule.SetSocket(swaping[j].facing, swaping[(j + i) % 4].model);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.one, Vector3.one * 2);

        Handles.Label(transform.position + Vector3.one + Vector3.right, GetSocket(Direction.Right).model);
        Handles.Label(transform.position + Vector3.one + Vector3.left, GetSocket(Direction.Left).model);
        Handles.Label(transform.position + Vector3.one + Vector3.up, GetSocket(Direction.Up).model);
        Handles.Label(transform.position + Vector3.one + Vector3.down, GetSocket(Direction.Down).model);
        Handles.Label(transform.position + Vector3.one + Vector3.forward, GetSocket(Direction.Forward).model);
        Handles.Label(transform.position + Vector3.one + Vector3.back, GetSocket(Direction.Back).model);
    }
#endif
}
