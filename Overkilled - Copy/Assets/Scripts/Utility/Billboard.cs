using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Tooltip("Camera to look at. Leave empty for Main Camera")]
    [SerializeField] Camera _camera;
    [Tooltip("If the camera to look at is static")]
    [SerializeField] bool _isCameraStatic;

    void Start()
    {
        _camera = Camera.main;

        transform.LookAt(_camera.transform);
    }

    void Update()
    {
        if (_isCameraStatic)
            return;

        transform.LookAt(_camera.transform);
    }
}
