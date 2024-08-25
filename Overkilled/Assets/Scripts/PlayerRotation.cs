using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] int _rotationSpeed = 1000;

    Vector3 _currentDir;

    void Update()
    {
        if (_currentDir.magnitude > 0)
            RotateToCurrentDir();
    }

    public void SetLookDirection(Vector3 dir)
    {
        _currentDir = dir;
    }

    void RotateToCurrentDir()
    { 
        Quaternion lookDir = Quaternion.LookRotation(new Vector3(_currentDir.x, 0, _currentDir.y), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDir, _rotationSpeed * Time.deltaTime);
    }
}
