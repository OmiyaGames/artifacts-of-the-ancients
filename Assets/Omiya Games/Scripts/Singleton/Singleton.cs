﻿using UnityEngine;
using System;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="Singleton.cs" company="Omiya Games">
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
    /// <date>4/15/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Any GameObject with this script will not be destroyed when switching between
    /// scenes. However, only one instance of this script may exist in a scene.
    /// Allows retrieving any components in itself or its children.
    /// </summary>
    /// <seealso cref="ISingletonScript"/>
    public class Singleton : MonoBehaviour
    {
        private static Singleton msInstance = null;
        private readonly Dictionary<Type, Component> mCacheRetrievedComponent = new Dictionary<Type, Component>();

        public event Action<float> OnUpdate;
        public event Action<float> OnRealTimeUpdate;

        ISingletonScript[] allSingletonScriptsCache = null;

        [SerializeField]
        bool simulateMalformedGame = false;

#if UNITY_EDITOR
        [SerializeField]
        bool simulateWebplayer = false;
#endif

        public static Singleton Instance
        {
            get
            {
                return msInstance;
            }
        }

        public static COMPONENT Get<COMPONENT>() where COMPONENT : Component
        {
            COMPONENT returnObject = null;
            Type retrieveType = typeof(COMPONENT);
            if (msInstance != null)
            {
                if (msInstance.mCacheRetrievedComponent.ContainsKey(retrieveType) == true)
                {
                    returnObject = msInstance.mCacheRetrievedComponent[retrieveType] as COMPONENT;
                }
                else
                {
                    returnObject = msInstance.GetComponentInChildren<COMPONENT>();
                    msInstance.mCacheRetrievedComponent.Add(retrieveType, returnObject);
                }
            }
            return returnObject;
        }

        public bool IsWebplayer
        {
            get
            {
#if UNITY_EDITOR
                // Check if webplayer simulation checkbox is checked
                return simulateWebplayer;
#elif (UNITY_WEBPLAYER || UNITY_WEBGL)
                // Always return true if already on a webplayer
                return true;
#else
                // Always return false, otherwise
                return false;
#endif
            }
        }

        public bool IsSimulatingMalformedGame
        {
            get
            {
                bool returnFlag = simulateMalformedGame;

                // Check if we're not in the editor, and this build is in debug mode
#if !UNITY_EDITOR
                if (Debug.isDebugBuild == false)
                {
                    // Always return false
                    returnFlag = false;
                }
#endif
                // Check if simulation checkbox is checked
                return returnFlag;
            }
        }

        // Use this for initialization
        void Awake()
        {
            if (msInstance == null)
            {
                // Set the instance variable
                msInstance = this;

                // Run all the events
                RunSingletonEvents();

                // Prevent this object from destroying itself
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Run all the events
                RunSingletonEvents();

                // Destroy this gameobject
                Destroy(gameObject);
            }
        }

        void Update()
        {
            if (OnUpdate != null)
            {
                OnUpdate(Time.deltaTime);
            }
            if (OnRealTimeUpdate != null)
            {
                OnRealTimeUpdate(Time.unscaledDeltaTime);
            }
        }

        void RunSingletonEvents()
        {
            int index = 0;
            if(allSingletonScriptsCache == null)
            {
                // Cache all the singleton scripts
                allSingletonScriptsCache = Instance.GetComponentsInChildren<ISingletonScript>();

                // Go through every ISingletonScript, and run singleton awake
                for (index = 0; index < allSingletonScriptsCache.Length; ++index)
                {
                    // Run singleton awake
                    allSingletonScriptsCache[index].SingletonInstance = Instance;
                    allSingletonScriptsCache[index].SingletonAwake(msInstance);
                }
            }

            // Go through every ISingletonScript, and run scene awake
            for (index = 0; index < allSingletonScriptsCache.Length; ++index)
            {
                allSingletonScriptsCache[index].SceneAwake(msInstance);
            }
        }
    }
}
