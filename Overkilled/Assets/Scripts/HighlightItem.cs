using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightItem : MonoBehaviour
{
    [SerializeField] Material _highlightMaterial;
    [SerializeField] Transform _modelParent;

    MeshRenderer[] _meshes;
    Material[] _initialMaterials;

    bool _isHighlighted = false;

    void Start()
    {
        _meshes = GetComponentsInChildren<MeshRenderer>();
        _initialMaterials = new Material[_meshes.Length];
        
        for (int i = 0; i < _meshes.Length; i++)
        {
            _initialMaterials[i] = _meshes[i].material;
        }
    }

    public void SetHighlight(bool state)
    {
        if (_isHighlighted == state)
            return;

        _isHighlighted = state;

        for (int i = 0; i < _meshes.Length; i++)
        {
            _meshes[i].material = state ? _highlightMaterial : _initialMaterials[i];
        }
    }
}
