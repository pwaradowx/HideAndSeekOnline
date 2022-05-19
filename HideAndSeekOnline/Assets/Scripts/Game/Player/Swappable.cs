using Unity.Netcode;
using UnityEngine;

namespace Project.Game.Player
{
    public class Swappable : NetworkBehaviour
    {
        public MeshFilter MyMeshFilter;
        public MeshRenderer MyMeshRenderer;
        public Vector3 ModelPosition;
    }
}
