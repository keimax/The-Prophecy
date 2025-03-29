using UnityEngine;
using TheProphecy;

namespace TheProphecy.Enemy
{
    public class EnemyShootingController : MonoBehaviour
    {
        public static ObjectPool _pool;
        private Vector2 _direction = new Vector2(1, 0);

        [Header("References")]
        [SerializeField] private GameObject _bulletPrefab;
        private GameObject _projectileContainer;
        [SerializeField] private GameObject _shootPoint;
        private Transform _playerContainer;

        [Header("Shooting")]
        [SerializeField] private float _fireCooldownTime = 0.5f;
        private float _fireCooldownTimer = 0f;
        private bool _isFireOnCooldown = false;
        [SerializeField] private float _bulletSpeed = 10f;

        [Header("Auto Aim")]
        [SerializeField] private bool _autoAim = true;
        [SerializeField] private bool _autoShoot = true;
        [SerializeField] private float _autoAimRange = 7f;

        [Header("Shooting Angle")]
        [SerializeField] private float _aimToleranceAngle = 15f;
        [SerializeField] private float _shootingRange = 5f;

        void Start()
        {
            _projectileContainer = GameObject.Find("EnemyProjectilesContainer");
            _playerContainer = GameObject.Find("PlayerContainer").transform;
            _pool = new ObjectPool(_bulletPrefab, _projectileContainer);
            _pool.FillThePool(20);
        }

        private void Update()
        {
            CalculateFireCooldown();
        }

        private void FixedUpdate()
        {
            if (_autoShoot)
            {
                ShootProjectile();
            }
        }

        public void ShootProjectile()
        {
            _direction = GetPlayerDirection();

            if (_direction != Vector2.zero && !_isFireOnCooldown &&
                Vector3.Distance(transform.position, transform.position + (Vector3)_direction) <= _shootingRange)
            {
                FireBullet();
            }
        }

        private void FireBullet()
        {
            int bulletInitialDegree = 0;
            float directionAngle = Vector2.SignedAngle(Vector2.right, _direction);

            GameObject bullet = _pool.GetFromPool();
            Bullets bulletScript = bullet.GetComponent<Bullets>();

            bulletScript.FireAndMove(_shootPoint.transform.position, _direction.normalized, directionAngle + bulletInitialDegree, _bulletSpeed);
            _isFireOnCooldown = true;
        }

        private void CalculateFireCooldown()
        {
            if (_isFireOnCooldown)
            {
                _fireCooldownTimer += Time.deltaTime;

                if (_fireCooldownTimer >= _fireCooldownTime)
                {
                    _fireCooldownTimer = 0f;
                    _isFireOnCooldown = false;
                }
            }
        }

        private Vector2 GetPlayerDirection()
        {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            if (_playerContainer.childCount > 0)
            {
                for (int i = 0; i < _playerContainer.childCount; i++)
                {
                    Transform player = _playerContainer.GetChild(i);

                    if (player.gameObject.activeSelf)
                    {
                        float distance = Vector3.Distance(player.position, transform.position);

                        if (distance <= _autoAimRange && distance < minDistance)
                        {
                            minDistance = distance;
                            direction = player.position - transform.position;
                        }
                    }
                }
            }

            return direction.normalized;
        }
    }
}