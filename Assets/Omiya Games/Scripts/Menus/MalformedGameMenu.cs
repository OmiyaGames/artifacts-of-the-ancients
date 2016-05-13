﻿using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="MalformedGameMenu.cs" company="Omiya Games">
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
    /// <date>5/11/2016</date>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Dialog indicating this game may not be genuine.
    /// You can retrieve this menu from the singleton script,
    /// <code>MenuManager</code>.
    /// </summary>
    /// <seealso cref="MenuManager"/>
    public class MalformedGameMenu : IMenu
    {
        public enum Reason
        {
            None = -1,
            IsNotGenuine = 0,
            CannotConfirmDomain,
            IsIncorrectDomain,
            JustTesting
        }

        [System.Serializable]
        public struct WebsiteInfo
        {
            [SerializeField]
            string display;
            [SerializeField]
            string redirectTo;

            public void UpdateButton(WebsiteButton button)
            {
                button.DisplayedText = display;
                button.RedirectTo = redirectTo;
            }
        }

        [Header("First Option")]
        [SerializeField]
        WebsiteButton websiteButton = null;
        [SerializeField]
        WebsiteInfo websiteInfo;

        [Header("Second Option")]
        [SerializeField]
        WebsiteButton otherSitesButton = null;
        [SerializeField]
        WebsiteInfo[] otherSites = null;
        [SerializeField]
        Text[] secondOptionSet = null;

        [Header("Error Messages")]
        [SerializeField]
        Text reasonMessage = null;
        [SerializeField]
        [Multiline]
        string gameIsNotGenuineMessage = "Internal tests confirm this game is not genuine.";
        [SerializeField]
        [Multiline]
        string cannotConfirmDomainMessage = "Unable to confirm this game is hosted by a domain the developers uploaded their game to.";
        [SerializeField]
        [Multiline]
        string domainDoesNotMatchMessage = "The detected url, \"{0},\" does not match any of the domains the developers uploaded their game to.";

        readonly List<WebsiteButton> allSecondOptionButtons = new List<WebsiteButton>();

        public override Type MenuType
        {
            get
            {
                return Type.ManagedMenu;
            }
        }

        public override GameObject DefaultUi
        {
            get
            {
                return websiteButton.gameObject;
            }
        }

        public override void Show(Action<IMenu> stateChanged)
        {
            // Call base function
            base.Show(stateChanged);

            // Setup the dialog
            websiteInfo.UpdateButton(websiteButton);
            if((otherSites != null) && (otherSites.Length > 0))
            {
                // Setup the second options
                SetupSecondOptions();
            }
            else
            {
                // Turn off everything related to the second options
                for(int index = 0; index < secondOptionSet.Length; ++index)
                {
                    secondOptionSet[index].gameObject.SetActive(false);
                }
                otherSitesButton.gameObject.SetActive(false);
            }
        }

        public override void Hide()
        {
            bool wasVisible = (CurrentState == State.Visible);

            // Call base function
            base.Hide();

            if (wasVisible == true)
            {
                // Lock the cursor to what the scene is set to
                SceneTransitionManager manager = Singleton.Get<SceneTransitionManager>();

                // Return to the menu
                manager.LoadMainMenu();
            }
        }

        public void UpdateReason(Reason reason)
        {
            // Grab the web checker
            WebLocationChecker webChecker = null;
            if (Singleton.Instance.IsWebplayer == true)
            {
                webChecker = Singleton.Get<WebLocationChecker>();
            }

            // Update the reason for this dialog to appear
            StringBuilder builder = new StringBuilder();
            switch(reason)
            {
                case Reason.CannotConfirmDomain:
                    builder.Append(cannotConfirmDomainMessage);
                    break;
                case Reason.IsIncorrectDomain:
                    if (webChecker != null)
                    {
                        builder.AppendFormat(domainDoesNotMatchMessage, webChecker.RetrievedHostName);
                    }
                    else
                    {
                        builder.Append(gameIsNotGenuineMessage);
                    }
                    break;
                case Reason.JustTesting:
                    BuildTestMessage(builder, webChecker);
                    break;
                default:
                    builder.Append(gameIsNotGenuineMessage);
                    break;
            }
            reasonMessage.text = builder.ToString();
        }

        #region Helper Methods
        void SetupSecondOptions()
        {
            // Populate the list of buttons with at least one button
            if (allSecondOptionButtons.Count <= 0)
            {
                allSecondOptionButtons.Add(otherSitesButton);
            }

            // Go through all the sites
            int index = 0;
            GameObject clone;
            for (; index < otherSites.Length; ++index)
            {
                // Check if we have enough buttons
                if (allSecondOptionButtons.Count <= index)
                {
                    // If not, create a new one
                    clone = Instantiate<GameObject>(otherSitesButton.gameObject);
                    allSecondOptionButtons.Add(clone.GetComponent<WebsiteButton>());

                    // Position this button properly
                    clone.transform.SetParent(otherSitesButton.transform.parent);
                    clone.transform.localScale = Vector3.one;
                    clone.transform.localRotation = Quaternion.identity;
                    clone.transform.SetSiblingIndex(otherSitesButton.transform.GetSiblingIndex() + index);
                }

                // Setup this button
                allSecondOptionButtons[index].gameObject.SetActive(true);
                otherSites[index].UpdateButton(allSecondOptionButtons[index]);
                
            }

            // Turn off the rest of the buttons
            for (; index < allSecondOptionButtons.Count; ++index)
            {
                allSecondOptionButtons[index].gameObject.SetActive(false);
            }
        }

        private void BuildTestMessage(StringBuilder builder, WebLocationChecker webChecker)
        {
            builder.Append("This menu is just a test.");
            if (webChecker != null)
            {
                builder.AppendLine(" More info according to the WebLocationChecker:");

                // Indicate the object's state
                int bulletNumber = 1;
                builder.Append(bulletNumber);
                builder.AppendLine(") the WebLocationChecker state is:");
                builder.AppendLine(webChecker.CurrentState.ToString());

                // Indicate the current domain information
                ++bulletNumber;
                builder.Append(bulletNumber);
                builder.AppendLine(") this game's domain is:");
                builder.AppendLine(webChecker.RetrievedHostName);

                // List entries from the default domain list
                ++bulletNumber;
                builder.Append(bulletNumber);
                builder.AppendLine(") the default domain list is:");
                int index = 0;
                for (; index < webChecker.DefaultDomainList.Length; ++index)
                {
                    builder.Append("- ");
                    builder.AppendLine(webChecker.DefaultDomainList[index]);
                }

                // Check if there's a download URL to list
                if(string.IsNullOrEmpty(webChecker.DownloadDomainsUrl) == false)
                {
                    // Print that URL
                    ++bulletNumber;
                    builder.Append(bulletNumber);
                    builder.AppendLine(") downloaded a list of domains from:");
                    builder.AppendLine(webChecker.DownloadDomainsUrl);

                    // Check if there are any downloaded domains
                    if (webChecker.DownloadedDomainList != null)
                    {
                        ++bulletNumber;
                        builder.Append(bulletNumber);
                        builder.AppendLine(") downloaded the following domains:");
                        for (index = 0; index < webChecker.DownloadedDomainList.Length; ++index)
                        {
                            builder.Append("- ");
                            builder.AppendLine(webChecker.DownloadedDomainList[index]);
                        }
                    }
                    else
                    {
                        ++bulletNumber;
                        builder.Append(bulletNumber);
                        builder.AppendLine(") downloading that list failed, however.");
                    }
                }

                // Show unique list of domains
                ++bulletNumber;
                builder.Append(bulletNumber);
                builder.AppendLine(") together, the full domain list is as follows:");
                foreach(string domain in webChecker.AllUniqueDomains)
                {
                    builder.Append("- ");
                    builder.AppendLine(domain);
                }
            }
        }
        #endregion
    }
}
