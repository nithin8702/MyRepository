using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using University.Api.Models;
using University.Context;
using University.Common.Models.Security;

namespace University.Api.Hubs
{
    public class MyHub : Hub
    {
        static List<MessageInfo> MessageList = new List<MessageInfo>();
        static List<UserInfo> UsersList = new List<UserInfo>();

        public void Connect(CurrentUser currentUser)
        {
            var id = Context.ConnectionId;
            string userGroup = "";
            //Manage Hub Class
            //if freeflag==0 ==> Busy
            //if freeflag==1 ==> Free

            //if tpflag==0 ==> User
            //if tpflag==1 ==> Admin


            var ctx = new UniversityContext();

            //var userInfo =
            //     (from m in ctx.ApplicationUsers
            //      where m.UserName == userName && m.Password == password
            //      select new { m.UserID, m.UserName, m.AdminCode }).FirstOrDefault();

            try
            {
                //You can check if user or admin did not login before by below line which is an if condition
                if (UsersList.Count(x => x.ConnectionId == id) == 0)
                {
                    return;
                }

                UsersList.Add(new UserInfo
                {
                    ConnectionId = id,
                    UserID = currentUser.UserId,
                    UserName = currentUser.UserName,
                    UserGroup = userGroup,
                    freeflag = "0",
                    tpflag = "0",
                });
                Groups.Add(Context.ConnectionId, userGroup);
                Clients.Caller.onConnected(id, currentUser.UserName, currentUser.UserId, userGroup);
            }

            catch
            {
                //string msg = "All Administrators are busy, please be patient and try again";
                //***** Return to Client *****
                Clients.Caller.NoExistAdmin();
            }
        }


        public void SendMessageToGroup(string userName, string message)
        {
            if (UsersList.Count != 0)
            {
                var strg = (from s in UsersList where (s.UserName == userName) select s).First();
                MessageList.Add(new MessageInfo { UserName = userName, Message = message, UserGroup = strg.UserGroup });
                string strgroup = strg.UserGroup;
                // If you want to Broadcast message to all UsersList use below line
                // Clients.All.getMessages(userName, message);

                //If you want to establish peer to peer connection use below line 
                //so message will be send just for user and admin who are in same group
                //***** Return to Client *****
                Clients.Group(strgroup).getMessages(userName, message);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool a)
        {

            var item = UsersList.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                UsersList.Remove(item);

                var id = Context.ConnectionId;

                if (item.tpflag == "0")
                {
                    //user logged off == user
                    try
                    {
                        var stradmin = (from s in UsersList
                                        where
                                            (s.UserGroup == item.UserGroup) && (s.tpflag == "1")
                                        select s).First();
                        //become free
                        stradmin.freeflag = "1";
                    }
                    catch
                    {
                        //***** Return to Client *****
                        Clients.Caller.NoExistAdmin();
                    }
                }

                //save conversation to dat abase
            }

            return base.OnDisconnected(a);
        }
    }
}