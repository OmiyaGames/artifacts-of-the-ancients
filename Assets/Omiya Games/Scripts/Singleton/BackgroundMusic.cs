﻿using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="BackgroundMusic.cs" company="Omiya Games">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2014-2016 Omiya Games
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// </copyright>
    /// <author>Taro Omiya</author>
    /// <date>8/18/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script that allows smooth transitions between 2 background musics.
    /// </summary>
    /// <seealso cref="Singleton"/>
    /// <seealso cref="AudioSource"/>
    /// <seealso cref="SoundEffect"/>
    /// <seealso cref="AmbientMusic"/>
    /// <seealso cref="OptionsMenu"/>
    public class BackgroundMusic : IAudio
    {
        [System.Serializable]
        public class MusicInfo
        {
            [SerializeField]
            AudioSource source = null;
            [SerializeField]
            AudioMixerSnapshot snapshot = null;

            public AudioSource Source
            {
                get
                {
                    return source;
                }
            }

            public AudioClip Clip
            {
                get
                {
                    return source.clip;
                }
            }

            public AudioMixerSnapshot Snapshot
            {
                get
                {
                    return snapshot;
                }
            }

            public void ChangeClip(AudioClip clip, float transitionTime)
            {
                if(Source.clip != null)
                {
                    Source.Stop();
                }
                source.clip = clip;
                if(clip != null)
                {
                    Source.Play();
                }
                Snapshot.TransitionTo(transitionTime);
            }
        }

        [Tooltip("The transition length (in seconds) between 2 background musics. Set to -1 if you want no transition.")]
        [SerializeField]
        float transitionDuration = 1;
        [SerializeField]
        MusicInfo music1 = null;
        [SerializeField]
        MusicInfo music2 = null;

        bool isPlayingMusic1 = true;

        #region Static Properties
        /// <summary>
        /// Gets or sets the volume of the background music, which is a value between 0 and 1.
        /// </summary>
        /// <value>The background music's volume.</value>
        public static float GlobalVolume
        {
            get
            {
                return Mathf.Clamp01(Singleton.Get<GameSettings>().MusicVolume);
            }
            set
            {
                // Set volume
                GameSettings settings = Singleton.Get<GameSettings>();
                settings.MusicVolume = Mathf.Clamp01(value);

                // Update the AudioMixerReference, if NOT muted
                if (settings.IsMusicMuted == false)
                {
                    Singleton.Get<AudioMixerReference>().BackgroundMusicVolumeNormalized = settings.MusicVolume;
                }
            }
        }

        public static bool GlobalMute
        {
            get
            {
                return Singleton.Get<GameSettings>().IsMusicMuted;
            }
            set
            {
                // Set mute
                GameSettings settings = Singleton.Get<GameSettings>();
                settings.IsMusicMuted = value;

                // Update the AudioMixerReference to either mute or revert the volume back to settings
                AudioMixerReference audioMixer = Singleton.Get<AudioMixerReference>();
                if (settings.IsMusicMuted == true)
                {
                    audioMixer.BackgroundMusicVolumeDb = audioMixer.MuteVolumeDb;
                }
                else
                {
                    audioMixer.BackgroundMusicVolumeNormalized = settings.MusicVolume;
                }
            }
        }

        public static float GlobalPitch
        {
            get
            {
                return Singleton.Get<AudioMixerReference>().BackgroundMusicPitch;
            }
            set
            {
                Singleton.Get<AudioMixerReference>().BackgroundMusicPitch = value;
            }
        }
        #endregion

        #region Properties
        public AudioClip CurrentMusic
        {
            get
            {
                return CurrentAudioSource.Clip;
            }
            set
            {
                // Check if this is a different clip
                if (CurrentAudioSource.Clip != value)
                {
                    // Swap to the next audio source
                    isPlayingMusic1 = !isPlayingMusic1;
                    if(isPlayingMusic1 == true)
                    {
                        music1.ChangeClip(value, transitionDuration);
                    }
                    else
                    {
                        music2.ChangeClip(value, transitionDuration);
                    }
                }
            }
        }

        public override AudioSource Audio
        {
            get
            {
                return CurrentAudioSource.Source;
            }
        }
        #endregion

        #region Helper Properties & Methods
        MusicInfo CurrentAudioSource
        {
            get
            {
                if (isPlayingMusic1 == true)
                {
                    return music1;
                }
                else
                {
                    return music2;
                }
            }
        }

        MusicInfo TransitionAudioSource
        {
            get
            {
                if (isPlayingMusic1 == true)
                {
                    return music2;
                }
                else
                {
                    return music1;
                }
            }
        }

        IEnumerator DelayPlay(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            Play();
        }
        #endregion
    }
}
