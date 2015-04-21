/*
 *
 * (c) Copyright Ascensio System Limited 2010-2015
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using AjaxPro;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.UserControls.Bookmarking
{
    public partial class CreateBookmarkUserControl : System.Web.UI.UserControl
    {
        public bool IsNewBookmark { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(BookmarkingUserControl));
            Utility.RegisterTypeForAjax(typeof(SingleBookmarkUserControl));
            Utility.RegisterTypeForAjax(typeof(CommentsUserControl));

            InitActionButtons();
        }

        #region Init Action Buttons

        private void InitActionButtons()
        {
            SaveBookmarkButton.ButtonText = BookmarkingUCResource.Save;
            SaveBookmarkButton.AjaxRequestText = BookmarkingUCResource.BookmarkCreationIsInProgressLabel;

            SaveBookmarkButtonCopy.ButtonText = BookmarkingUCResource.AddToFavourite;
            SaveBookmarkButtonCopy.AjaxRequestText = BookmarkingUCResource.BookmarkCreationIsInProgressLabel;

            AddToFavouritesBookmarkButton.ButtonText = BookmarkingUCResource.AddToFavourite;
            AddToFavouritesBookmarkButton.AjaxRequestText = BookmarkingUCResource.BookmarkCreationIsInProgressLabel;

            CheckBookmarkUrlLinkButton.ButtonText = BookmarkingUCResource.CheckBookmarkUrlButton;
            CheckBookmarkUrlLinkButton.AjaxRequestText = BookmarkingUCResource.CheckingUrlIsInProgressLabel;
        }

        #endregion

        public string NavigateToMainPage
        {
            get { return BookmarkingServiceHelper.BookmarkDisplayMode.CreateBookmark.Equals(BookmarkingServiceHelper.GetCurrentInstanse().DisplayMode).ToString().ToLower(); }
        }

        public bool CreateBookmarkMode
        {
            get { return BookmarkingServiceHelper.BookmarkDisplayMode.CreateBookmark.Equals(BookmarkingServiceHelper.GetCurrentInstanse().DisplayMode); }
        }

        public bool IsEditMode
        {
            get
            {
                var serviceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
                return BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark.Equals(serviceHelper.DisplayMode) && serviceHelper.IsCurrentUserBookmark();
            }
        }
    }
}