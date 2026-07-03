using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Self-removing VFX component. Destroys the GameObject after a configurable lifetime.
    /// Used for hit-flash and death-burst effects that should not be pooled.
    /// </summary>
    public class VFXLifetime : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.3f;

        public void Configure(float duration)
        {
            lifetime = duration;
        }

        private void Update()
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}