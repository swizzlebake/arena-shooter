using UnityEngine;

namespace Gameplay
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip musicClip;
        [SerializeField] private AudioClip fireSFX;
        [SerializeField] private AudioClip hitSFX;
        [SerializeField] private AudioClip deathSFX;

        public static AudioManager Instance { get; private set; }

        private AudioSource musicSource;
        private AudioSource sfxSource;

        public void Configure(AudioClip music, AudioClip fire, AudioClip hit, AudioClip death)
        {
            musicClip = music;
            fireSFX = fire;
            hitSFX = hit;
            deathSFX = death;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlayMusic()
        {
            if (musicClip != null && musicSource.clip != musicClip)
            {
                musicSource.clip = musicClip;
                musicSource.Play();
            }
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlayFireSFX()
        {
            if (fireSFX != null) sfxSource.PlayOneShot(fireSFX);
        }

        public void PlayHitSFX()
        {
            if (hitSFX != null) sfxSource.PlayOneShot(hitSFX);
        }

        public void PlayDeathSFX()
        {
            if (deathSFX != null) sfxSource.PlayOneShot(deathSFX);
        }
    }
}
