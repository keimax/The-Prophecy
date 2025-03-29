using UnityEngine;
using TheProphecy;

namespace TheProphecy.Player
{
    public class ShootingController : MonoBehaviour
    {
        public static ObjectPool _pool;
        private Vector2 _direction = new Vector2(0, 1); // Default facing north

        [Header("References")]
        [SerializeField] private VirtualJoystick _aimJoystick;
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GameObject _projectileContainer;
        [SerializeField] private GameObject _center;
        [SerializeField] private GameObject _bow;
        [SerializeField] private Transform _enemyContainer;

        [Header("Bullet Stats")]
        [SerializeField] private float _bulletSpeed = 15f;
        private const float FIRE_COOLDOWN_TIME = 0.1f;
        private float _fireCooldownTimer = 0f;
        private bool _isFireOnCooldown = false;

        [Header("Auto Aim")]
        [SerializeField] private bool _autoAim = true;
        [SerializeField] private bool _autoShoot = true;
        [SerializeField] private float _autoAimRange = 10f; // Range for auto aim

        [Header("Shooting Angle")]
        [SerializeField] private float _aimToleranceAngle = 45f; // Degrees (±45 degrees)
        [SerializeField] private float _shootingRange = 5f; // Maximum shooting range

        [Header("Arc Visualization")]
        [SerializeField] private int _arcSegments = 30;
        [SerializeField] private float _arcRadius = 1f; // Radius of the arc
        private LineRenderer _lineRenderer;

        void Start()
        {
            _enemyContainer = GameManager.Instance.EnemyContainer.transform;
            _projectileContainer = GameManager.Instance.PlayerContainer;

            _pool = new ObjectPool(_bulletPrefab, _projectileContainer);
            _pool.FillThePool(30);

            Debug.Log("ShootingController Start");
            Debug.Log(_enemyContainer);
            Debug.Log(_projectileContainer);

            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        private void Update()
        {
            CalculateFireCooldown();
            DrawShootingArc();
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
            Vector2 targetDirection;

            if (_aimJoystick == null)
            {
                // Auto aim
                targetDirection = GetClosestEnemyDirection();

                // Check if we have a valid direction
                if (targetDirection != Vector2.zero)
                {
                    // Calculate the angle between the ship's forward direction and the target direction
                    float angleToEnemy = Vector2.SignedAngle(transform.up, targetDirection);
                //    Debug.Log($"Auto Aim - Target Direction: {targetDirection}, Angle to Enemy: {angleToEnemy}");

                    // Check if the angle is within the allowed range
                    if (angleToEnemy >= -_aimToleranceAngle && angleToEnemy <= _aimToleranceAngle && !_isFireOnCooldown)
                    {
                        FireBullet(targetDirection); // Pass the target direction
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                // Get joystick direction
                _direction = _aimJoystick.Direction.normalized;

                if (_direction.magnitude > 0.1f) // Check if joystick is moved significantly
                {
                    targetDirection = GetClosestEnemyDirection();
                    float angleToEnemy = Vector2.SignedAngle(transform.up, targetDirection);
                    Debug.Log($"Joystick Aim - Direction: {_direction}, Target Direction: {targetDirection}, Angle to Enemy: {angleToEnemy}");

                    // Check if the angle is within the allowed range
                    if (angleToEnemy >= -_aimToleranceAngle && angleToEnemy <= _aimToleranceAngle && !_isFireOnCooldown)
                    {
                        FireBullet(targetDirection); // Pass the target direction
                    }
                    else
                    {
                    }
                }
            }
        }

        private void FireBullet(Vector2 targetDirection)
        {
            GameObject bullet = _pool.GetFromPool();
            Bullets bulletScript = bullet.GetComponent<Bullets>();

            // Calculate the angle of the direction vector
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

            // Set the bullet's rotation
            bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Fire the bullet in the direction of the target
            bulletScript.FireAndMove(_bow.transform.position, targetDirection.normalized, angle, _bulletSpeed);
            _isFireOnCooldown = true;

            // Debug log for firing
            Debug.Log($"Bullet Fired! Direction: {targetDirection.normalized}, Angle: {angle}");
        }

        private void CalculateFireCooldown()
        {
            if (_isFireOnCooldown)
            {
                _fireCooldownTimer += Time.deltaTime;

                if (_fireCooldownTimer >= FIRE_COOLDOWN_TIME)
                {
                    _fireCooldownTimer = 0f;
                    _isFireOnCooldown = false;
                }
            }
        }

        private Vector2 GetClosestEnemyDirection()
        {
            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            for (int i = 0; i < _enemyContainer.childCount; i++)
            {
                Transform child = _enemyContainer.GetChild(i).transform;

                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                float distance = Vector3.Distance(child.position, transform.position);

                // Only consider enemies within the auto aim range
                if (distance < minDistance && distance <= _autoAimRange)
                {
                    minDistance = distance;
                    direction = child.position - transform.position;
                }
            }

            return direction.normalized;
        }

        private void DrawShootingArc()
        {
            if (_aimJoystick == null) return;

            float angleStep = _aimToleranceAngle * 2 / _arcSegments;
            float startAngle = -_aimToleranceAngle;

            for (int i = 0; i <= _arcSegments; i++)
            {
                float angle = startAngle + i * angleStep;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Vector3 point = rotation * Vector3.up * _arcRadius; // Using Vector3.up for top-down
                _lineRenderer.SetPosition(i, transform.position + point);
            }
        }
    }
}