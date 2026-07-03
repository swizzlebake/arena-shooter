using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Self-destructs the GameObject after a configurable lifetime.
    /// Used for hit-flash and death-burst VFX that should auto-remove.
    /// </summary>
    public class VFXSelfDestruct : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.5f;

        public void Configure(float newLifetime)
        {
            lifetime = newLifetime;
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}