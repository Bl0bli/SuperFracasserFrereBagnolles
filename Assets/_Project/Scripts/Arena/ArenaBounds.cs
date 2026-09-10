using UnityEngine;

namespace Game
{
    /// <summary>
    /// Declare la zone jouable. Sert a tirer des positions au hasard dans l'arene
    /// pour les flaques, les roues et les cartes.
    /// </summary>
    public class ArenaBounds : MonoBehaviour
    {
        public static ArenaBounds Instance { get; private set; }

        [Tooltip("Collider couvrant la zone jouable. Un BoxCollider2D en trigger suffit.")]
        [SerializeField] private Collider2D _area;

        [Tooltip("Marge interieure, pour ne pas semer contre les murs.")]
        [SerializeField] private float _padding = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[ArenaBounds] Un second ArenaBounds a ete ignore.", this);
                return;
            }

            Instance = this;

            if (_area == null) _area = GetComponent<Collider2D>();
            if (_area == null) Debug.LogError("[ArenaBounds] Aucun Collider2D de zone.", this);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public bool Contains(Vector2 point)
        {
            return _area != null && _area.OverlapPoint(point);
        }

        public Vector2 RandomPoint()
        {
            if (_area == null) return transform.position;

            Bounds b = _area.bounds;
            Vector2 min = new Vector2(b.min.x + _padding, b.min.y + _padding);
            Vector2 max = new Vector2(b.max.x - _padding, b.max.y - _padding);

            // Une arene non rectangulaire a des trous dans sa bounding box : on retente.
            for (int i = 0; i < 12; i++)
            {
                Vector2 candidate = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
                if (Contains(candidate)) return candidate;
            }

            return b.center;
        }

        private void OnDrawGizmosSelected()
        {
            if (_area == null) return;

            Gizmos.color = new Color(0.2f, 0.8f, 0.6f, 0.35f);
            Bounds b = _area.bounds;
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}
