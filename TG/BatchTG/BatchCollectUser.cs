using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TG.Client.Cache;
using TG.Client.Handler;
using TG.Client.Model;
using TG.Client.Model.Login;
using TG.Client.TG;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.BatchTG
{
    public class BatchCollectUser : Td.ClientResultHandler
    {
        private TdCollectParam collectParam;
        public void SetCollectParam(TdCollectParam collectParam)
        {
            this.collectParam = collectParam;
        }

        public BatchCollectUser()
        {

        }

        public BatchCollectUser(TdCollectParam collectParam)
        {

        }

        public void CollectUser(string groupUrl, TimeFilterType timeFilterType, int collectNum)
        {
            TdCollectParam temCollectParam = new TdCollectParam();
            temCollectParam.CurrentGroupUrl = groupUrl;
            temCollectParam.CurrentOperatorType = OperatorType.None;
            temCollectParam.CurrentCollectNum = collectNum;
            temCollectParam.CurrentTimeFilterType = timeFilterType;
            
            if (!string.IsNullOrEmpty(groupUrl))
            {
                string name = groupUrl.Replace("https://t.me/", "");
                temCollectParam.CurrentGroupName = name;
                temCollectParam.CurrentOperatorType = OperatorType.SearchChat;


                //批量发送
                Dictionary<string, TGClient> dic = TGClientManager.Instance.GetAllClient();
                foreach (KeyValuePair<string, TGClient> kv in dic)
                {
                    temCollectParam.Account = kv.Key;
                    TGClient client = kv.Value;
                    client.GetClient().Send(new TdApi.SearchPublicChat() { Username = name }, new BatchCollectUser(temCollectParam));
                }
                
            }

        }
        

        private void Print(string str)
        {
            Console.WriteLine(str);
        }

        public void OnResult(TdApi.BaseObject @object)
        {
            try
            {
                if (@object != null)
                {
                    Print(@object.ToString());
                    UserHandler.Instance.PublishMsg(@object);
                    if (collectParam.CurrentOperatorType == OperatorType.SearchChat)
                    {
                        TdApi.Chat chat = @object as TdApi.Chat;

                        if (chat != null && chat.Type is TdApi.ChatTypeSupergroup)
                        {
                            TdApi.ChatTypeSupergroup chatTypeSupergroup = chat.Type as TdApi.ChatTypeSupergroup;
                            collectParam.CurrentGruopId = chatTypeSupergroup.SupergroupId;
                            if (chat.LastMessage != null)
                                collectParam.CurrentChatLastMsgId = chat.LastMessage.Id;

                            collectParam.CurrentChatId = chat.Id;
                        }
                        else
                        {
                            if (chat == null)
                            {
                                Print("SearchChat result chat is null");
                            }
                            else
                            {
                                Print("chat is not supergroup.");
                            }
                            return;
                        }
                        //_client.Send(new TdApi.AddChatMembers(chatId, new long[] { userId, userId }), this);
                        collectParam.CurrentOperatorType = OperatorType.SearchSupergroupFullInfo;
                        TGClient client = TGClientManager.Instance.GetClientByAcc(collectParam.Account);
                        if (client != null)
                        {
                            client.GetClient().Send(new TdApi.GetSupergroupFullInfo() { SupergroupId = collectParam.CurrentGruopId }, this);

                        }
                    }
                    else if (collectParam.CurrentOperatorType == OperatorType.SearchSupergroupFullInfo)
                    {
                        TdApi.SupergroupFullInfo supergroupFullInfo = @object as TdApi.SupergroupFullInfo;

                        if (supergroupFullInfo != null)
                        {
                            if (supergroupFullInfo.CanGetMembers)
                            {
                                if (collectParam.CurrentTimeFilterType == TimeFilterType.None)//无活跃条件筛选时，获取全部成员
                                {
                                    //获取用户列表
                                    collectParam.StartIndex = 0;
                                    collectParam.EndIndex = collectParam.Limit;

                                    collectParam.CurrentOperatorType = OperatorType.SearchSupergroupMembers;

                                    TGClient client = TGClientManager.Instance.GetClientByAcc(collectParam.Account);
                                    if (client != null)
                                    {
                                        client.GetClient().Send(new TdApi.GetSupergroupMembers() { SupergroupId = collectParam.CurrentGruopId, Filter = new TdApi.SupergroupMembersFilterRecent(), Offset = collectParam.StartIndex, Limit = collectParam.Limit }, this);
                                    }
                                }
                                //else
                                //{
                                //    collectParam.CurrentOperatorType = OperatorType.SearchChatHistory;


                                //    startIndex = -50;
                                //    endIndex = 100;
                                //    _client.Send(new TdApi.GetChatHistory()
                                //    {
                                //        ChatId = collectParam.CurrentChatId,
                                //        Limit = endIndex,
                                //        FromMessageId = 0,//currentChatLastMsgId,
                                //        Offset = 0,//startIndex,
                                //        OnlyLocal = false
                                //    }, this);
                                //}
                            }
                            else
                            {
                                UserHandler.Instance.PublishMsg("Client:" + collectParam.Account + ",----GetSupergroupFullInfo:" + collectParam.CurrentGroupUrl + " is not CanGetMembers");
                                return;
                            }
                        }
                        else
                        {
                            UserHandler.Instance.PublishMsg("Client:" + collectParam.Account + ",----GetSupergroupFullInfo:" + collectParam.CurrentGroupUrl + " is null");
                            return;
                        }

                    }
                    else if (collectParam.CurrentOperatorType == OperatorType.SearchSupergroupMembers)//无活跃条件筛选时，获取全部成员
                    {
                        TdApi.ChatMembers chatMembers = @object as TdApi.ChatMembers;
                        if (chatMembers != null)
                        {
                            CommonHandler.Instance.PublishMemberChange(chatMembers.TotalCount, chatMembers.Members.Length);

                            //MsgHandler.Instance.AddOrUpdateByMember(chatMembers, currentCollectNum);

                            int loadCount = MsgHandler.Instance.GetLoadUserCount();

                            if (chatMembers.Members.Length >= collectParam.Limit && (collectParam.CurrentCollectNum == 0 || (collectParam.CurrentCollectNum > 0 && collectParam.StartIndex <= collectParam.CurrentCollectNum)))
                            {
                                Thread.Sleep(500);

                                collectParam.StartIndex += chatMembers.Members.Length;

                                TGClient client = TGClientManager.Instance.GetClientByAcc(collectParam.Account);
                                if (client != null)
                                {
                                    client.GetClient().Send(new TdApi.GetSupergroupMembers() { SupergroupId = collectParam.CurrentGruopId, Filter = new TdApi.SupergroupMembersFilterRecent(), Offset = collectParam.StartIndex, Limit = collectParam.Limit }, this);

                                }
                            }
                            else
                            {
                                UserHandler.Instance.PublishMsg("Start get user detail");
                                Task.Run(() =>
                                {
                                    GetUserDetail();
                                });
                            }
                        }
                    }
                    //else if (collectParam.CurrentOperatorType == OperatorType.SearchChatHistory)
                    //{
                    //    TdApi.Messages messages = @object as TdApi.Messages;

                    //    if (messages != null)
                    //    {
                    //        int lastTime = MsgHandler.Instance.AddOrUpdateByMessage(messages, collectParam.CurrentTimeFilterType);

                    //        bool isContinue = MsgHandler.Instance.ContinueSearchHis(lastTime, collectParam.CurrentTimeFilterType);


                    //        if (isContinue && messages.TotalCount > 50 && messages.MessagesValue.Length >= 50)
                    //        {
                    //            long lastMsgId = messages.MessagesValue[messages.MessagesValue.Length - 1].Id;

                    //            startIndex = -50;
                    //            endIndex = 50;
                    //            _client.Send(new TdApi.GetChatHistory()
                    //            {
                    //                ChatId = currentChatId,
                    //                Limit = limit,
                    //                FromMessageId = lastMsgId,
                    //                Offset = startIndex,
                    //                OnlyLocal = false
                    //            }, this);

                    //            Thread.Sleep(100);
                    //        }
                    //        else
                    //        {
                    //            Task.Run(() =>
                    //            {
                    //                GetUserDetail();
                    //            });

                    //        }
                    //    }
                    //}
                    else if (collectParam.CurrentOperatorType == OperatorType.SearchChatUser)
                    {
                        TdApi.User user = @object as TdApi.User;
                        if (user != null)
                        {
                            if (sendUserIdHs.Contains(user.Id))
                            {
                                sendUserIdHs.Remove(user.Id);
                            }
                            if (user.Type is TdApi.UserTypeBot)
                            {
                                UserHandler.Instance.PublishMsg("Filter bot:" + user.FirstName + " " + user.LastName);

                                //OnBotUserChange?.Invoke(user);

                                return;
                            }
                            user.RestrictionReason = collectParam.CurrentGroupName;
                            //OnUserChange?.Invoke(user);

                            MsgHandler.Instance.AddOrUpdateUser(user);
                        }
                    }
                }
                else
                {
                    UserHandler.Instance.PublishMsg("Client:" + collectParam.Account + ", APICollect OnResult currentOperatorType:" + collectParam.CurrentOperatorType + " result is null");
                }
            }
            catch (Exception e)
            {
                UserHandler.Instance.PublishMsg("Client:" + collectParam.Account + ", APICollect OnResult exeception:" + e.Message);
            }
        }

        private HashSet<long> sendUserIdHs = new HashSet<long>();

        private void GetUserDetail()
        {
            collectParam.StartIndex = 0;
            collectParam.EndIndex = 0;

            sendUserIdHs.Clear();

            Thread.Sleep(2000);
            collectParam.EndIndex += 500;
            collectParam.CurrentOperatorType = OperatorType.SearchChatUser;

            HashSet<long> idSet = MsgHandler.Instance.GetAllId();

            int index = 0;
            foreach (long userId in idSet)
            {
                if (collectParam.CurrentCollectNum != 0 && index >= collectParam.CurrentCollectNum)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("GetUser by UserId:" + userId);
                    sendUserIdHs.Add(userId);
                    TGClient client = TGClientManager.Instance.GetClientByAcc(collectParam.Account);
                    if (client != null)
                    {
                        client.GetClient().Send(new TdApi.GetUser() { UserId = userId }, this);
                    }
                }

                index++;


                if (index % 100 == 0)
                    Thread.Sleep(5000);
                else
                    Thread.Sleep(150);
            }

            Thread.Sleep(3000);

            UserHandler.Instance.PublishMsg("Client:" + collectParam.Account + ", Get user detail end...");

            //OnFailUserChange?.Invoke(sendUserIdHs.Count);

        }
    }
}
