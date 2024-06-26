using System;
using Unity.Collections;
using Unity.Netcode;

[System.Serializable]
public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId; // 플레이어의 클라이언트 ID
    public int skinIndex;
    public FixedString64Bytes username; // 플레이어 이름 (최대 64바이트 문자열)

    public bool Equals(PlayerData other) // 다른 PlayerData 객체와 비교가능한 메서드
    {
        return clientId == other.clientId &&
            skinIndex == other.skinIndex &&
            username == other.username;
    }

    /// <summary>
    /// PlayerData 구조체를 네트워크를 통해 직렬화/역직렬화하는 NetworkSerialize 메서드를 정의
    /// BufferSerializer<T>를 사용하여 데이터를 직렬화하거나 역직렬화한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref skinIndex);
        serializer.SerializeValue(ref username);
    }
}
