using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace MatchThree
{
    /// <summary>
    /// Manages akk the sound
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField]
        private AudioClip _bgMusic, _sfxClear, _sfxSelect, _sfxSwap;
        private AudioSource _audioSource;

        /// <summary>
        /// On Start gets its AudioSource and plays the background music
        /// </summary>
        private void Start()
        {
            _audioSource = this.GetComponent<AudioSource>();
            PlayMusic();
        }

        /// <summary>
        /// Plays the music assigned on the respective AudioClip on loop
        /// </summary>
        private void PlayMusic()
        {
            _audioSource.clip = _bgMusic;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        /// <summary>
        /// Plays the sfx selected gem assigned on the respective AudioClip
        /// </summary>
        public void PlaySFXSelectedGem()
        {
            _audioSource.PlayOneShot(_sfxSelect);
        }

        /// <summary>
        /// Plays the sfx swap gem assigned on the respective AudioClip
        /// </summary>
        public void PlaySFXSwapSound()
        {
            _audioSource.PlayOneShot(_sfxSwap);
        }

        /// <summary>
        /// Plays the sfx clear gem assigned on the respective AudioClip
        /// </summary>
        public void PlayClearedLevel()
        {
            _audioSource.PlayOneShot(_sfxClear);
        }
    }
}