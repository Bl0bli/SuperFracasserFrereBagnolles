using UnityEngine;

namespace Game
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        public AudioSource MenuMusic;
        public AudioSource AmbientSound;
        public AudioSource BumpingSound;
        public AudioSource PowerPickupSound;
        public AudioSource BornePickupSound;
        public AudioSource ThrowableSound;
        public AudioSource ShieldSound;
        public AudioSource EngineLoop;
        public AudioSource ExplosionSound;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void PlayMenuMusic()
            {
                MenuMusic.Play();
            }

        public void StopMenuMusic()
            {
                MenuMusic.Stop();
            }

        public void PlayAmbientSound()
            {
                AmbientSound.Play();
            }

        public void StopAmbientSound()
            {
                AmbientSound.Stop();
            }

        public void PlayBumpingSound()
            {
                BumpingSound.Play();
            }

        public void PlayPowerPickupSound()
            {
                PowerPickupSound.Play();
            }

        public void PlayThrowableSound()
            {
                ThrowableSound.Play();
            }

        public void PlayShieldSound()
            {
                ShieldSound.Play();
            }

        public void PlayBornePickupSound()
            {
                BornePickupSound.Play();
            }

        public void PlayEngineLoop()
            {
                EngineLoop.Play();
            }

        public void PlayExplosionSound()
            {
                ExplosionSound.Play();
            }
    }
}
