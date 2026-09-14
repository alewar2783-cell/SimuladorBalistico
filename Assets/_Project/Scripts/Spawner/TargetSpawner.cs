using UnityEngine;

namespace BallisticSim.Core.Spawner
{
    public class TargetSpawner : MonoBehaviour
    {
        [Header("Grid Config")]
        [SerializeField] private GameObject _boxPrefab;
        [SerializeField] private int _columns = 5;
        [SerializeField] private int _rows = 5;

        [Header("Joint Config")]
        [SerializeField] private float _jointBreakForce = 500f;
        [SerializeField] private float _jointBreakTorque = 500f;

        private GameObject _wallParent;
        private Rigidbody[,] _grid;

        private float _currentBoxMass = 2f;

        public int TotalJoints { get; private set; }

        public void SetWallParameters(float boxMass, float jointBreakForce)
        {
            _currentBoxMass = boxMass;
            _jointBreakForce = jointBreakForce;
            _jointBreakTorque = jointBreakForce;
        }

        public void SpawnWall(float distanceFromWeapon)
        {
            DestroyWall();

            _wallParent = new GameObject("TargetWall");
            _grid = new Rigidbody[_columns, _rows];
            TotalJoints = 0;

            Vector3 boxScale = _boxPrefab.transform.localScale;
            // Bottom-left corner offset centers the wall on X, sits on Y=0
            float startX = -(_columns - 1) * boxScale.x * 0.5f;
            float startY = boxScale.y * 0.5f;

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    Vector3 position = new Vector3(
                        startX + x * boxScale.x,
                        startY + y * boxScale.y,
                        distanceFromWeapon
                    );

                    GameObject box = Instantiate(_boxPrefab, position, Quaternion.identity, _wallParent.transform);
                    box.name = $"Box_{x}_{y}";

                    Rigidbody rb = box.GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        rb = box.AddComponent<Rigidbody>();
                    }
                    
                    rb.mass = _currentBoxMass;

                    _grid[x, y] = rb;

                    if (x > 0)
                    {
                        AttachJoint(rb, _grid[x - 1, y]);
                    }
                    if (y > 0)
                    {
                        AttachJoint(rb, _grid[x, y - 1]);
                    }
                }
            }
        }

        private void AttachJoint(Rigidbody body, Rigidbody connectedBody)
        {
            FixedJoint joint = body.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = connectedBody;
            joint.breakForce = _jointBreakForce;
            joint.breakTorque = _jointBreakTorque;
            TotalJoints++;
        }

        public void DestroyWall()
        {
            if (_wallParent != null)
            {
                Destroy(_wallParent);
                _wallParent = null;
            }
            _grid = null;
            TotalJoints = 0;
        }

        public int CountBrokenJoints()
        {
            if (_wallParent == null) return 0;

            int remainingJoints = _wallParent.GetComponentsInChildren<FixedJoint>().Length;
            return TotalJoints - remainingJoints;
        }
    }
}
