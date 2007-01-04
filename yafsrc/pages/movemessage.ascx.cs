/* Yet Another Forum.net
 * Copyright (C) 2003 Bj�rnar Henden
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace YAF.Pages
{
    /// <summary>
    /// Summary description for movetopic.
    /// </summary>
    public partial class movemessage : ForumPage
    {
        public movemessage()
            : base("MOVEMESSAGE")
        {
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (Request.QueryString["m"] == null || !ForumModeratorAccess)
                Data.AccessDenied();

            if (!IsPostBack)
            {
                PageLinks.AddLink(BoardSettings.Name, Forum.GetLink(ForumPages.forum));
                PageLinks.AddLink(PageCategoryName, Forum.GetLink(ForumPages.forum, "c={0}", PageCategoryID));
                PageLinks.AddForumLinks(PageForumID);
                PageLinks.AddLink(PageTopicName, Forum.GetLink(ForumPages.posts, "t={0}", PageTopicID));

                Move.Text = GetText("MOVE_MESSAGE");
                CreateAndMove.Text = GetText("CREATE_MOVE");

                ForumList.DataSource = YAF.Classes.Data.DB.forum_listall_sorted(PageBoardID, PageUserID);
                ForumList.DataTextField = "Title";
                ForumList.DataValueField = "ForumID";
                DataBind();
                ForumList.Items.FindByValue(PageForumID.ToString()).Selected = true;
                ForumList_SelectedIndexChanged(this.ForumList, e);
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion

        protected void Move_Click(object sender, System.EventArgs e)
        {
            if (Convert.ToInt32(TopicsList.SelectedValue) != PageTopicID)
            {
                YAF.Classes.Data.DB.message_move(Request.QueryString["m"], TopicsList.SelectedValue, true);
            }
            Forum.Redirect(ForumPages.topics, "f={0}", PageForumID);
        }

        protected void ForumList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            TopicsList.DataSource = YAF.Classes.Data.DB.topic_list(ForumList.SelectedValue, 0, null, 0, 32762);
            TopicsList.DataTextField = "Subject";
            TopicsList.DataValueField = "TopicID";
            TopicsList.DataBind();
            TopicsList_SelectedIndexChanged(this.ForumList, e);
            CreateAndMove.Enabled = Convert.ToInt32(ForumList.SelectedValue) <= 0 ? false : true;
        }

        protected void TopicsList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (TopicsList.SelectedValue == "")
                Move.Enabled = false;
            else
                Move.Enabled = true;
        }

        protected void CreateAndMove_Click(object sender, System.EventArgs e)
        {
            if (ThemeSubject.Text != "")
            {
                long nTopicId =
                    YAF.Classes.Data.DB.topic_create_by_message(Request.QueryString["m"], ForumList.SelectedValue, ThemeSubject.Text);
                YAF.Classes.Data.DB.message_move(Request.QueryString["m"], nTopicId, true);
                Forum.Redirect(ForumPages.topics, "f={0}", PageForumID);
            }
            else
                AddLoadMessage(GetText("EmptyTheme"));
        }
    }
}