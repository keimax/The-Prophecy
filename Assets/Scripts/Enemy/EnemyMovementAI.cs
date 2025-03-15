using TheProphecy.Map.PathFinding;
using UnityEngine;
using TheProphecy.Map.DungeonGeneration;

namespace TheProphecy.Enemy
{
    public class EnemyMovementAI : MonoBehaviour
    {
        [Header("References")]
        private SpriteRenderer _spriteRenderer;
        private AccessReferencesForAI _accessReferencesForAI;

        private Transform _gridGameObject;
        private PathfindingGrid _grid;
        private Pathfinding _pathfinding;

        private Transform _targetLeft; // choosing 2 target points fixes bug of wrong calculation of nodes!
        private Transform _targetRight;

        InvisibilityController _invisibilityController;

        [Header("Pathfinding")]
        private Vector3[] _waypoints;
        private Vector3 _oldTargetPosition;

        private const float _PATH_UPDATE_TIME = 0.07f;
        private float _pathUpdateTimer = 0f;

        private int _currentCheckPointIndex = 0;
        private float _moveSpeed = 4f;

        private float _range = 9f;
        private bool _isInRange = false;

        // Idle settings
        [Header("Idle Settings")]
        [SerializeField] private bool enableIdle = true; // Option to enable idle behavior
        [SerializeField] private float idleRange = 3f; // Range within which to move while idle
        [SerializeField] private float idleSpeed = 2f; // Speed during idle movement
        private Vector3 _spawnPoint; // Initial spawn point
        private Vector3 _idleTarget; // Target point for idle movement
        private float _idleTimer = 0f;
        private float _idleChangeInterval = 2f; // Time interval to change idle target

        // Avoidance parameters
        [Header("Avoidance Settings")]
        [SerializeField] private bool enableAvoidance = false; // Option to enable avoidance
        [SerializeField] private float minDistance = 1.5f; // Minimum distance to maintain from other enemies
        [SerializeField] private float repulsionMultiplier = 1.5f; // Multiplier for the avoidance force

        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _accessReferencesForAI = transform.GetComponentInParent<AccessReferencesForAI>();

            _gridGameObject = _accessReferencesForAI.pathfindingGrid.transform;
            _targetLeft = _accessReferencesForAI.targetLeftPivot.transform;
            _targetRight = _accessReferencesForAI.targetRightPivot.transform;
            _invisibilityController = _accessReferencesForAI.invisibilityController;

            _pathfinding = _gridGameObject.GetComponent<Pathfinding>();
            _grid = _pathfinding.Grid;
            _oldTargetPosition = _targetLeft.position;
            _range = _gridGameObject.GetComponent<RandomWalkDungeonGenerator>().GetRoomRadius();

            _waypoints = _pathfinding.FindPath(transform.position, _targetLeft.position);

            // Set the spawn point
            _spawnPoint = transform.position;
            SetNewIdleTarget();
        }

        private void Update()
        {
            _isInRange = _range > (transform.position - _targetLeft.position).magnitude;

            if (_isInRange && !_invisibilityController._isInvisible)
            {
                UpdatePath();
                if (enableAvoidance)
                {
                    Avoidance();
                }
            }
            else
            {
                EnterIdleState();
            }
        }

        private void FixedUpdate()
        {
            if (_isInRange && !_invisibilityController._isInvisible)
            {
                FollowPath();
            }
            else
            {
                IdleMovement();
            }
        }

        private Vector3 ChooseTargetPivotOfCharacter()
        {
            Node targetNodeLeft = _grid.NodeFromWorldPoint(_targetLeft.position);

            if (targetNodeLeft.walkable)
            {
                return _targetLeft.position;
            }

            return _targetRight.position;
        }

        private void UpdatePath()
        {
            Vector3 target = ChooseTargetPivotOfCharacter();

            if (_pathUpdateTimer < _PATH_UPDATE_TIME)
            {
                _pathUpdateTimer += Time.deltaTime;
            }
            else
            {
                _pathUpdateTimer = 0f;

                Node targetNode = _grid.NodeFromWorldPoint(target);
                Node oldTargetNode = _grid.NodeFromWorldPoint(_oldTargetPosition);

                if (!(targetNode.Equals(oldTargetNode)))
                {
                    _waypoints = _pathfinding.FindPath(transform.position, target);
                    _oldTargetPosition = target;
                    _currentCheckPointIndex = 0;
                }
            }
        }

        private void FollowPath()
        {
            if (_currentCheckPointIndex < _waypoints.Length)
            {
                PathfindingGrid grid = _pathfinding.Grid;
                Node currentTransformNode = grid.NodeFromWorldPoint(transform.position);
                Node nextWaypointNode = grid.NodeFromWorldPoint(_waypoints[_currentCheckPointIndex]);

                if ((nextWaypointNode.worldPosition - transform.position).magnitude < 0.15f)
                {
                    _currentCheckPointIndex++;
                }

                if (_currentCheckPointIndex < _waypoints.Length)
                {
                    Vector3 moveDirection = (_waypoints[_currentCheckPointIndex] - transform.position).normalized;
                    transform.Translate(moveDirection * Time.deltaTime * _moveSpeed);

                    if (moveDirection.x > 0f)
                    {
                        _spriteRenderer.flipX = false;
                    }
                    else if (moveDirection.x < 0f)
                    {
                        _spriteRenderer.flipX = true;
                    }
                }
            }
        }

        private void EnterIdleState()
        {
            // Reset the current waypoint index to ensure idle movement when no target is present
            _currentCheckPointIndex = _waypoints.Length; // Stop following path
        }

        private void IdleMovement()
        {
            _idleTimer += Time.deltaTime;

            if (_idleTimer >= _idleChangeInterval)
            {
                SetNewIdleTarget();
                _idleTimer = 0f;
            }

            // Move towards the idle target
            Vector3 direction = (_idleTarget - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * idleSpeed);

            // Check if reached the idle target
            if (Vector3.Distance(transform.position, _idleTarget) < 0.1f)
            {
                SetNewIdleTarget(); // Change target when close to current target
            }
        }

        private void SetNewIdleTarget()
        {
            float randomX = Random.Range(-idleRange, idleRange);
            float randomY = Random.Range(-idleRange, idleRange);
            _idleTarget = _spawnPoint + new Vector3(randomX, randomY, 0);
        }

        private void Avoidance()
        {
            // Find all enemies in the vicinity
            EnemyMovementAI[] enemies = FindObjectsOfType<EnemyMovementAI>();
            foreach (EnemyMovementAI enemy in enemies)
            {
                if (enemy != this) // Avoid self
                {
                    float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

                    if (distanceToEnemy < minDistance)
                    {
                        Vector2 toEnemy = (enemy.transform.position - transform.position).normalized;
                        Vector2 avoidanceVector = (transform.position - enemy.transform.position).normalized;

                        // Adjust the movement direction to include the avoidance vector
                        transform.Translate((avoidanceVector + toEnemy * -1).normalized * Time.deltaTime * _moveSpeed);
                    }
                }
            }
        }

        public void OnDrawGizmos()
        {
            if (_waypoints != null)
            {
                for (int i = 0; i < _waypoints.Length; i++)
                {
                    Vector3 pos = _waypoints[i];
                    if (i < _currentCheckPointIndex)
                    {
                        Gizmos.color = Color.green;
                    }
                    else if (i == _currentCheckPointIndex)
                    {
                        Gizmos.color = Color.yellow;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawCube(pos, Vector3.one * (0.45f));
                }
            }
        }
    }
}