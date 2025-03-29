using UnityEngine;
using TheProphecy.Interfaces;
using System.Diagnostics;
using System;

namespace TheProphecy
{
    public class Bullets : MonoBehaviour
    {
        [SerializeField] private int _damage = 1;
        [SerializeField] private LayerMask _targetLayer;

        [Header("Bullet Lifetime")]
        [SerializeField] private float _lifetime = 3f;
        private float _currentLifetime;

        private Rigidbody2D _rigidbody;
        private ObjectPool _pool;

        [SerializeField] private GameObject _hitPrefab;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _currentLifetime = _lifetime;
        }

        private void Update()
        {
            _currentLifetime -= Time.deltaTime;

            if (_currentLifetime <= 0)
            {
                if (_pool == null)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    _pool?.AddToPool(gameObject);
                }
            }
        }

        public void FireAndMove(Vector3 position, Vector3 direction, float angleZ, float bulletSpeed)
        {
            transform.rotation = Quaternion.Euler(0, 0, angleZ);
            transform.position = position;
            _rigidbody.linearVelocity = direction * bulletSpeed;

            // Reset lifetime
            _currentLifetime = _lifetime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((_targetLayer.value & (1 << collision.gameObject.layer)) != 0)
            {
                if (collision.TryGetComponent<IDamageable>(out IDamageable iDamageable))
                {
                    if (_hitPrefab != null)
                    {
                        GameObject.Destroy(GameObject.Instantiate(_hitPrefab, transform.position, Quaternion.identity), 1f);
                    }
                    iDamageable.OnTakeDamage(_damage);
                }

                _pool?.AddToPool(gameObject);
            }
        }

        public void InitializePool(ObjectPool pool)
        {
            _pool = pool;
        }
    }
}

