using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable 
{
    private const int NUM_OF_PLAYER_MODELS = 3;

    public ulong clientId;

    public int PlayerModelId
    {
        get { return _playerModelId; }
        set
        {
            _playerModelId = value;

            if (_playerModelId >= NUM_OF_PLAYER_MODELS)
                _playerModelId = 0;
            else if (_playerModelId < 0)
                _playerModelId = NUM_OF_PLAYER_MODELS - 1;
        }
    }
    private int _playerModelId;

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && PlayerModelId == other.PlayerModelId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref _playerModelId);
    }
}
