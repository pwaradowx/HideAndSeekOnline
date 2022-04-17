using System;
using Unity.Netcode;

namespace Project.Room
{
    public struct RoomPlayerState : INetworkSerializable, IEquatable<RoomPlayerState>
    {
        public ulong UserID;
        public bool IsReady;

        public RoomPlayerState(ulong userID, bool isReady)
        {
            UserID = userID;
            IsReady = isReady;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref UserID);
            serializer.SerializeValue(ref IsReady);
        }

        public bool Equals(RoomPlayerState other)
        {
            return UserID == other.UserID;
        }
    }
}
