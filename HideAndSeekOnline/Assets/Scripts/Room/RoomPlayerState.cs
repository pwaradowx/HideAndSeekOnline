using System;
using Unity.Netcode;
using Unity.Collections;

namespace Project.Room
{
    public struct RoomPlayerState : INetworkSerializable, IEquatable<RoomPlayerState>
    {
        public FixedString64Bytes UserName;
        public ulong UserID;
        public bool IsReady;

        public RoomPlayerState(FixedString64Bytes userName, ulong userID, bool isReady)
        {
            UserName = userName;
            UserID = userID;
            IsReady = isReady;
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref UserName);
            serializer.SerializeValue(ref UserID);
            serializer.SerializeValue(ref IsReady);
        }

        public bool Equals(RoomPlayerState other)
        {
            return UserID == other.UserID;
        }
    }
}
