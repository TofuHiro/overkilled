using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PredictedPhysicsObject : NetworkBehaviour
{
    Rigidbody _rigidBody;

    NetworkVariable<Vector3> _netPosition = new NetworkVariable<Vector3>();
    NetworkVariable<Quaternion> _netRotation = new NetworkVariable<Quaternion>();
    NetworkVariable<Vector3> _netVelocity = new NetworkVariable<Vector3>();

    const float _correctionThreshold = .5f;
    bool _isSyncing;

    /// <summary>
    /// Toggle whether this physics object should be synced with the server authoritative state
    /// </summary>
    /// <param name="state"></param>
    public void ToggleSync(bool state)
    {
        //Store before switch to prevent unwanted lerping from pickup location
        if (IsServer)
        {
            _netPosition.Value = _rigidBody.position;
            _netRotation.Value = _rigidBody.rotation;
        }

        _isSyncing = state;
    }

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();    
    }

    void FixedUpdate()
    {
        if (!_isSyncing)
            return;

        if (IsServer)
        {
            _netPosition.Value = _rigidBody.position;
            _netRotation.Value = _rigidBody.rotation;

            if (!_rigidBody.isKinematic)
                _netVelocity.Value = _rigidBody.linearVelocity;
        }    
        else
        {
            ClientPredictionAndReconciliation();
        }
    }

    void ClientPredictionAndReconciliation()
    {
        float distance = Vector3.Distance(_rigidBody.position, _netPosition.Value);

        if (distance > _correctionThreshold)
        {
            //Hard correction
            _rigidBody.position = Vector3.Lerp(_rigidBody.position, _netPosition.Value, .5f);
            _rigidBody.rotation = Quaternion.Lerp(_rigidBody.rotation, _netRotation.Value, .5f);

            if (!_rigidBody.isKinematic)
                _rigidBody.linearVelocity = Vector3.Lerp(_rigidBody.linearVelocity, _netVelocity.Value, .5f);
        }
        else
        {
            //Soft correction
            _rigidBody.position = Vector3.MoveTowards(_rigidBody.position, _netPosition.Value, Time.fixedDeltaTime * 2f);
            _rigidBody.rotation = Quaternion.Lerp(_rigidBody.rotation, _netRotation.Value, Time.fixedDeltaTime * 2f);

            if (!_rigidBody.isKinematic)
                _rigidBody.linearVelocity = Vector3.Lerp(_rigidBody.linearVelocity, _netVelocity.Value, Time.fixedDeltaTime * 5f);
        }
    }
}
