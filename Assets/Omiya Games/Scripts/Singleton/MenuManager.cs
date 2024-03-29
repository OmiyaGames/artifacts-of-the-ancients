﻿using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MenuManager.cs" company="Omiya Games">
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
    /// <date>8/21/2015</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// A singleton script that retrieves all <code>IMenu</code>s in the scene.
    /// </summary>
    /// <seealso cref="IMenu"/>
    /// <seealso cref="Singleton"/>
    [RequireComponent(typeof(EventSystem))]
    public class MenuManager : ISingletonScript
    {
        static readonly Type[] IgnoreTypes = new Type[]
        {
            typeof(PopUpDialog)
        };

        [Header("Behaviors")]
        [Tooltip("Name of input under the InputManager that is going to pause the game")]
        [SerializeField]
        string pauseInput = "Pause";
        [SerializeField]
        float delaySelectingDefaultUiBy = 0.5f;

        [Header("Menu Label Templates")]
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating to return to a scene")]
        [SerializeField]
        string returnToTextTemplate = "Return to {0}";
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating to restart a scene")]
        [SerializeField]
        string restartTextTemplate = "Restart {0}";
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating a scene was completed")]
        [SerializeField]
        string completeTextTemplate = "{0} Complete";
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating game over")]
        [SerializeField]
        string failedTextTemplate = "{0} Failed";
        // TODO: consider using the translation patch instead
        [Tooltip("Template for any menus with button text indicating a scene was completed")]
        [SerializeField]
        string nextTextTemplate = "Proceed to {0}";

        [Header("Sound Templates")]
        [SerializeField]
        SoundEffect buttonClickSound = null;

        EventSystem eventSystemCache = null;
        WaitForSeconds delaySelection = null;
        string menuTextCache = null;
        PauseMenu pauseMenuCache = null;
        PopUpManager popUpManager = null;
        SceneTransitionManager transitionManagerCache = null;
        readonly Dictionary<Type, IMenu> typeToMenuMap = new Dictionary<Type, IMenu>();
        readonly Stack<IMenu> managedMenusStack = new Stack<IMenu>();

        public event Action<MenuManager> OnManagedMenusStackChanged;

        #region Properties
        public EventSystem Events
        {
            get
            {
                if(eventSystemCache == null)
                {
                    eventSystemCache = GetComponent<EventSystem>();
                }
                return eventSystemCache;
            }
        }

        public SoundEffect ButtonClick
        {
            get
            {
                return buttonClickSound;
            }
        }

        public IMenu LastManagedMenu
        {
            get
            {
                IMenu returnMenu = null;
                if (NumManagedMenus > 0)
                {
                    returnMenu = managedMenusStack.Peek();
                }
                return returnMenu;
            }
        }

        public int NumManagedMenus
        {
            get
            {
                return managedMenusStack.Count;
            }
        }

        public string ReturnToMenuText
        {
            get
            {
                if (menuTextCache == null)
                {
                    menuTextCache = CachedTransitionManager.MainMenu.DisplayName;
                    if (string.IsNullOrEmpty(returnToTextTemplate) == false)
                    {
                        menuTextCache = string.Format(returnToTextTemplate, menuTextCache);
                    }
                }
                return menuTextCache;
            }
        }

        public string RestartCurrentSceneText
        {
            get
            {
                string returnText = "";
                SceneInfo currentScene = CachedTransitionManager.CurrentScene;
                if (currentScene != null)
                {
                    returnText = currentScene.DisplayName;
                    if (string.IsNullOrEmpty(restartTextTemplate) == false)
                    {
                        returnText = string.Format(restartTextTemplate, currentScene.DisplayName);
                    }
                }
                return returnText;
            }
        }

        public string CompletedCurrentSceneText
        {
            get
            {
                string returnText = "";
                SceneInfo currentScene = CachedTransitionManager.CurrentScene;
                if (currentScene != null)
                {
                    returnText = currentScene.DisplayName;
                    if (string.IsNullOrEmpty(completeTextTemplate) == false)
                    {
                        returnText = string.Format(completeTextTemplate, currentScene.DisplayName);
                    }
                }
                return returnText;
            }
        }

        public string FailedCurrentSceneText
        {
            get
            {
                string returnText = "";
                SceneInfo currentScene = CachedTransitionManager.CurrentScene;
                if (currentScene != null)
                {
                    returnText = currentScene.DisplayName;
                    if (string.IsNullOrEmpty(failedTextTemplate) == false)
                    {
                        returnText = string.Format(failedTextTemplate, currentScene.DisplayName);
                    }
                }
                return returnText;
            }
        }

        public string NextSceneText
        {
            get
            {
                string returnText = "";
                SceneInfo nextScene = CachedTransitionManager.NextScene;
                if (nextScene != null)
                {
                    returnText = nextScene.DisplayName;
                    if (string.IsNullOrEmpty(nextTextTemplate) == false)
                    {
                        returnText = string.Format(nextTextTemplate, nextScene.DisplayName);
                    }
                }
                return returnText;
            }
        }

        public PopUpManager PopUps
        {
            get
            {
                return popUpManager;
            }
        }

        SceneTransitionManager CachedTransitionManager
        {
            get
            {
                if(transitionManagerCache == null)
                {
                    transitionManagerCache = Singleton.Get<SceneTransitionManager>();
                }
                return transitionManagerCache;
            }
        }
        #endregion

        public override void SingletonAwake(Singleton instance)
        {
            // Enable events
            Events.enabled = true;

            // Bind to update
            instance.OnRealTimeUpdate += QueryInput;


            delaySelection = new WaitForSeconds(delaySelectingDefaultUiBy);
        }

        public override void SceneAwake(Singleton instance)
        {
            // Clear out all the menus
            managedMenusStack.Clear();
            pauseMenuCache = null;

            // Populate typeToMenuMap dictionary
            SceneTransitionMenu transitionMenu = null;
            PopulateTypeToMenuDictionary(typeToMenuMap, out transitionMenu);

            // Attempt to find a pop-up manager
            popUpManager = UnityEngine.Object.FindObjectOfType<PopUpManager>();

            // Check to see if there was a transition menu
            if (transitionMenu == null)
            {
                // If not, run the scene manager's transition-in events immediately
                CachedTransitionManager.TransitionIn(null);
            }
            else
            {
                // If so, run the transition menu's transition-in animation
                transitionMenu.Hide(CachedTransitionManager.TransitionIn);
            }
        }

        void PopulateTypeToMenuDictionary(Dictionary<Type, IMenu> typeToMenuDictionary, out SceneTransitionMenu transitionMenu)
        {
            // Setup variables
            int index = 0;
            transitionMenu = null;
            typeToMenuDictionary.Clear();
            
            // Populate items to ignore into the type map
            for (; index < IgnoreTypes.Length; ++index)
            {
                typeToMenuDictionary.Add(IgnoreTypes[index], null);
            }

            // Search for all menus in the scene
            IMenu[] menus = UnityEngine.Object.FindObjectsOfType<IMenu>();
            if(menus != null)
            {
                // Add them into the dictionary
                Type menuType;
                IMenu displayedManagedMenu = null;
                for (index = 0; index < menus.Length; ++index)
                {
                    if (menus[index] != null)
                    {
                        // Add the menu to the dictionary
                        menuType = menus[index].GetType();
                        if (typeToMenuDictionary.ContainsKey(menuType) == false)
                        {
                            // Add the menu
                            typeToMenuDictionary.Add(menuType, menus[index]);

                            // Check if this menu is a SceneTransitionMenu
                            if (menuType == typeof(SceneTransitionMenu))
                            {
                                transitionMenu = (SceneTransitionMenu)menus[index];
                            }
                        }

                        // Check if this is the first displayed, managed menu
                        if ((menus[index].MenuType == IMenu.Type.DefaultManagedMenu) && (displayedManagedMenu == null))
                        {
                            // Grab this menu
                            displayedManagedMenu = menus[index];

                            // Indicate it should be visible
                            displayedManagedMenu.Show();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pushes a visible menu into the stack, and
        /// changes the other menus already in the stack to stand-by
        /// </summary>
        internal void PushToManagedStack(IMenu menu)
        {
            if (menu != null)
            {
                // Make sure the menu isn't already in the stack
                // (the stack is usually small, so this should be pretty efficient)
                if (managedMenusStack.Contains(menu) == false)
                {
                    // Change the top-most menu (if any) to stand-by
                    if (NumManagedMenus > 0)
                    {
                        managedMenusStack.Peek().CurrentState = IMenu.State.StandBy;
                    }
                    else
                    {
                        // Unlock the cursor
                        SceneTransitionManager.CursorMode = CursorLockMode.None;
                    }

                    // Push the current menu onto the stack
                    managedMenusStack.Push(menu);

                    // Run the event that indicates the stack changed
                    if(OnManagedMenusStackChanged != null)
                    {
                        OnManagedMenusStackChanged(this);
                    }
                }
            }
        }

        /// <summary>
        /// Pops a hidden menu out of the stack, and
        /// changes the last menu already in the stack to visible
        /// </summary>
        internal IMenu PopFromManagedStack()
        {
            // Make sure this menu is already on top of the stack
            IMenu returnMenu = null;
            if (NumManagedMenus > 0)
            {
                // If so, pop the menu
                returnMenu = managedMenusStack.Pop();

                // Check if there are any other menus left
                if (NumManagedMenus > 0)
                {
                    // Change the top-most menu into visible
                    managedMenusStack.Peek().CurrentState = IMenu.State.Visible;
                }
                else if(CachedTransitionManager.CurrentScene != null)
                {
                    // Lock the cursor to what the scene is set to
                    SceneTransitionManager.CursorMode = CachedTransitionManager.CurrentScene.LockMode;
                }

                // Run the event that indicates the stack changed
                if (OnManagedMenusStackChanged != null)
                {
                    OnManagedMenusStackChanged(this);
                }
            }
            return returnMenu;
        }

        /// <summary>
        /// Pops a hidden menu out of the stack, and
        /// changes the last menu already in the stack to visible
        /// </summary>
        internal IMenu PeekFromManagedStack()
        {
            // Make sure this menu is already on top of the stack
            IMenu returnMenu = null;
            if (NumManagedMenus > 0)
            {
                // If so, peek the stack
                returnMenu = managedMenusStack.Peek();
            }
            return returnMenu;
        }

        public MENU GetMenu<MENU>() where MENU : IMenu
        {
            IMenu returnMenu = null;
            if (typeToMenuMap.TryGetValue(typeof(MENU), out returnMenu) == false)
            {
                returnMenu = null;
            }
            return returnMenu as MENU;
        }

        public MENU Show<MENU>(Action<IMenu> action = null) where MENU : IMenu
        {
            MENU returnMenu = GetMenu<MENU>();
            if (returnMenu != null)
            {
                returnMenu.Show(action);
            }
            return returnMenu;
        }

        public MENU Hide<MENU>() where MENU : IMenu
        {
            MENU returnMenu = GetMenu<MENU>();
            if (returnMenu != null)
            {
                returnMenu.Hide();
            }
            return returnMenu;
        }

        public void SelectGuiGameObject(GameObject guiElement)
        {
            StartCoroutine(DelaySelection(guiElement));
        }

        void QueryInput(float unscaledDeltaTime)
        {
            // Detect input for pause button (make sure no managed dialogs are shown, either).
            if((NumManagedMenus <= 0) && (Input.GetButtonDown(pauseInput) == true))
            {
                // Attempt to grab the pause menu
                if(pauseMenuCache == null)
                {
                    pauseMenuCache = GetMenu<PauseMenu>();
                }
                if (pauseMenuCache != null)
                {
                    if(pauseMenuCache.CurrentState == IMenu.State.Hidden)
                    {
                        pauseMenuCache.Show();

                        // Indicate button is clicked
                        ButtonClick.Play();
                    }
                    else if(pauseMenuCache.CurrentState == IMenu.State.Visible)
                    {
                        pauseMenuCache.Hide();

                        // Indicate button is clicked
                        ButtonClick.Play();
                    }
                }
            }
        }

        IEnumerator DelaySelection(GameObject guiElement)
        {
            yield return delaySelection;
            Events.SetSelectedGameObject(guiElement);
        }
    }
}
