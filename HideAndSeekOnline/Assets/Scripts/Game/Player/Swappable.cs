using Unity.Netcode;
using UnityEngine;

namespace Project.Game.Player
{
    public class Swappable : NetworkBehaviour
    {
        public Mesh MyMesh;
        public MeshRenderer MyMeshRenderer;
        public Vector3 ModelPosition;
        public float ControllerRadius;
        public float ControllerHeight;
        public float SkinWidth;
        public float Health;
    }
}
