using System;
using Unity.Collections;
using Unity.Netcode;

[System.Serializable]
public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId; // �÷��̾��� Ŭ���̾�Ʈ ID
    public int skinIndex;
    public FixedString64Bytes username; // �÷��̾� �̸� (�ִ� 64����Ʈ ���ڿ�)

    public bool Equals(PlayerData other) // �ٸ� PlayerData ��ü�� �񱳰����� �޼���
    {
        return clientId == other.clientId &&
            skinIndex == other.skinIndex &&
            username == other.username;
    }

    /// <summary>
    /// PlayerData ����ü�� ��Ʈ��ũ�� ���� ����ȭ/������ȭ�ϴ� NetworkSerialize �޼��带 ����
    /// BufferSerializer<T>�� ����Ͽ� �����͸� ����ȭ�ϰų� ������ȭ�Ѵ�.
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
