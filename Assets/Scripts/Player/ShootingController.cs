using UnityEngine;
using TheProphecy;

namespace TheProphecy.Player
{
    public class ShootingController : MonoBehaviour
    {
        public static ObjectPool _pool;
        private Vector2 _direction = new Vector2(1, 0);

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
        [SerializeField] private float _aimToleranceAngle = 15f; // Degrees
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
            if (_aimJoystick == null)
            {
                // Auto aim
                _direction = GetClosestEnemyDirection();

                // If we have a valid direction and the enemy is within shooting range, fire
                if (_direction != Vector2.zero && !_isFireOnCooldown && Vector3.Distance(transform.position, transform.position + (Vector3)_direction) <= _shootingRange)
                {
                    FireBullet();
                }
            }
            else
            {
                // Get joystick direction
                _direction = _aimJoystick.Direction;

                if (_direction.magnitude > 0.1f) // Check if joystick is moved significantly
                {
                    Vector2 closestPointDirection = GetClosestEnemyDirection();
                    float angleToEnemy = Vector2.Angle(_direction, closestPointDirection);

                    // Check if the enemy is within shooting range
                    if (angleToEnemy <= _aimToleranceAngle && !_isFireOnCooldown && Vector3.Distance(transform.position, transform.position + (Vector3)closestPointDirection) <= _shootingRange)
                    {
                        FireBullet();
                    }
                }
            }
        }

        private void FireBullet()
        {
            int bulletInitialDegree = 0;//-90;
            float directionAngle = Vector2.SignedAngle(Vector2.right, _direction);

            GameObject bullet = _pool.GetFromPool();
            Bullets bulletScript = bullet.GetComponent<Bullets>();

            bulletScript.FireAndMove(_bow.transform.position, _direction.normalized, directionAngle + bulletInitialDegree, _bulletSpeed);
            _isFireOnCooldown = true;
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
                Vector3 point = rotation * Vector3.right * _arcRadius;
                _lineRenderer.SetPosition(i, transform.position + point);
            }
        }
    }
}