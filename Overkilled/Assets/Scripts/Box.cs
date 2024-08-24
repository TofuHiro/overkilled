using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IDamagable
{
    public float Health
    {
        get { return _health; }
        set
        {
            _health = Mathf.Clamp(value, 0f, 100f);
        }
    }
    private float _health;

    void Start()
    {
        _health = 100f;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }
}
