using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] int _rotationSpeed = 1000;

    Vector2 _currentDir;
    bool _lockedRotationSpeed = true;

    void Update()
    {
        if (_currentDir.magnitude > 0)
            RotateToCurrentDir();
    }

    public void ToggleLockRotationSpeed(bool state)
    {
        _lockedRotationSpeed = state;
    }

    public void SetLookDirection(Vector3 dir)
    {
        _currentDir.x = dir.x;
        _currentDir.y = dir.z;
    }

    public void SetLookDirection(Vector2 dir)
    {
        _currentDir = dir;
    }

    void RotateToCurrentDir()
    { 
        Quaternion lookDir = Quaternion.LookRotation(new Vector3(_currentDir.x, 0, _currentDir.y), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDir, _lockedRotationSpeed ? _rotationSpeed * Time.deltaTime : 3600);
    }
}
