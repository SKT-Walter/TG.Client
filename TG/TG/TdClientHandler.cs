using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TG.Client.Handler;
using TG.Client.Model;
using TG.Client.Model.Login;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    public class TdClientHandler : Td.ClientResultHandler
    {
        private static TdClientHandler tdClientHandler = new TdClientHandler();

        public static TdClientHandler Instance { get { return tdClientHandler; } }

        private TdClientHandler()
        {

        }

        private Telegram.Td.Client _client = null;
        private Telegram.Td.ClientResultHandler _defaultHandler = new DefaultHandler();
        private TdApi.AuthorizationState _authorizationState = null;
        private volatile bool _haveAuthorization = false;
        private volatile bool _needQuit = false;
        private volatile bool _canQuit = false;
        private volatile AutoResetEvent _gotAuthorization = new AutoResetEvent(false);

        public event Action<TdApi.User> OnUserChange;

        private IMessage msgListener;

        public void SetMsgListener(IMessage msgListener)
        {
            this.msgListener = msgListener;
        }

        private LoginPo loginPo;
        public void SetLoginMsg(LoginPo loginPo)
        {
            this.loginPo = loginPo;
        }

        public Td.Client CreateTdClient(IMessage msgListener)
        {
            if (this.msgListener == null)
                this.msgListener = msgListener;

            if (_client == null)
            {
                Td.Client.Execute(new TdApi.SetLogVerbosityLevel(0));
                if (Td.Client.Execute(new TdApi.SetLogStream(new TdApi.LogStreamFile("tdlib.log", 1 << 27, false))) is TdApi.Error)
                {
                    throw new System.IO.IOException("Write access to the current directory is required");
                }
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    Td.Client.Run();
                }).Start();

                _client = Td.Client.Create(new UpdateHandler(this));

                // test Client.Execute
                _defaultHandler.OnResult(Td.Client.Execute(new TdApi.GetTextEntities("@telegram /test_command https://telegram.org telegram.me @gif @test")));

                BatchSendMsgHandler.Instance.Init(_client);

                AnalysisChatHandler.Instance.Init(_client);
            }

            return _client;
        }



        private bool isSendCode = false;
        public void OnAuthorizationStateUpdated(TdApi.AuthorizationState authorizationState)
        {
            if (authorizationState != null)
            {
                _authorizationState = authorizationState;
            }
            if (_authorizationState is TdApi.AuthorizationStateWaitTdlibParameters)
            {
                TdApi.SetTdlibParameters request = new TdApi.SetTdlibParameters();
                request.DatabaseDirectory = "tdlib";
                request.UseMessageDatabase = true;
                request.UseSecretChats = true;
                //request.ApiId = 94575;
                //request.ApiHash = "a3406de8d171bb422bb6ddf3bbd800e2";

                string api_id = ConfigurationManager.AppSettings["API_ID"];
                string api_hash = ConfigurationManager.AppSettings["API_HASH"];
                int API_ID = Int32.Parse(api_id);
                string API_HASH = api_hash;

                request.ApiId = API_ID;
                request.ApiHash = API_HASH;

                request.SystemLanguageCode = "en";
                request.DeviceModel = "Desktop";
                request.ApplicationVersion = "1.0";
                request.EnableStorageOptimizer = true;

                _client.Send(request, new AuthorizationRequestHandler(this, msgListener, _authorizationState));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitCode)
            {
                if (_client != null && !isSendCode && loginPo != null)
                {
                    string code = loginPo.AuthenticationCode;//ReadLine("Please enter authentication code: ");
                    _client.Send(new TdApi.CheckAuthenticationCode(code), new AuthorizationRequestHandler(this, msgListener, _authorizationState));

                    isSendCode = true;
                }
            }
            else if (_authorizationState is TdApi.AuthorizationStateReady)
            {
                if (msgListener != null)
                {
                    BaseReplyPo replyPo = new BaseReplyPo();
                    replyPo.Msg = "验证成功！";
                    msgListener.OnMessage(replyPo);
                }

            }
            else
            {
                Print("Unsupported authorization state:" + (loginPo == null?string.Empty:loginPo.ToString()) + " " + _authorizationState);
            }
        }


        public void ProcessLogin(LoginPo loginPo)
        {
            this.loginPo = loginPo;
            if (_authorizationState is TdApi.AuthorizationStateWaitPhoneNumber)
            {
                string phoneNumber = loginPo.Phone;//ReadLine("Please enter phone number: ");
                if (_client != null && !string.IsNullOrEmpty(phoneNumber))
                    _client.Send(new TdApi.SetAuthenticationPhoneNumber(phoneNumber, null), new AuthorizationRequestHandler(this, msgListener, _authorizationState));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitEmailAddress)
            {
                string emailAddress = loginPo.Email;//ReadLine("Please enter email address: ");
                if (_client != null && !string.IsNullOrEmpty(emailAddress))
                    _client.Send(new TdApi.SetAuthenticationEmailAddress(emailAddress), new AuthorizationRequestHandler(this, msgListener, _authorizationState));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitEmailCode)
            {
                string code = loginPo.EmailAuthenticationCode;//ReadLine("Please enter email authentication code: ");
                if (_client != null && !string.IsNullOrEmpty(code))
                    _client.Send(new TdApi.CheckAuthenticationEmailCode(new TdApi.EmailAddressAuthenticationCode(code)), new AuthorizationRequestHandler(this, msgListener, _authorizationState));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitOtherDeviceConfirmation state)
            {
                Console.WriteLine("Please confirm this login link on another device: " + state.Link);
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitCode)
            {
                string code = loginPo.AuthenticationCode; ;//ReadLine("Please enter authentication code: ");
                if (_client != null && !string.IsNullOrEmpty(code))
                    _client.Send(new TdApi.CheckAuthenticationCode(code), new AuthorizationRequestHandler(this, msgListener, _authorizationState));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitRegistration)
            {
                string firstName = loginPo.FirstName;//ReadLine("Please enter your first name: ");
                string lastName = loginPo.LastName;//ReadLine("Please enter your last name: ");
                if (_client != null)
                    _client.Send(new TdApi.RegisterUser(firstName, lastName), new AuthorizationRequestHandler(this, msgListener, _authorizationState));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitPassword)
            {
                string password = loginPo.Password;//ReadLine("Please enter password: ");
                if (_client != null && !string.IsNullOrEmpty(password))
                    _client.Send(new TdApi.CheckAuthenticationPassword(password), new AuthorizationRequestHandler(this, msgListener, _authorizationState));
            }
            else if (_authorizationState is TdApi.AuthorizationStateReady)
            {
                _haveAuthorization = true;
                _gotAuthorization.Set();

                //_client.Send(new TdApi.LoadChats(null, 100), _defaultHandler);

                
                if (msgListener != null)
                {
                    BaseReplyPo replyPo = new BaseReplyPo();
                    replyPo.Msg = "验证成功！";
                    msgListener.OnMessage(replyPo);
                }

            }
            else if (_authorizationState is TdApi.AuthorizationStateLoggingOut)
            {
                _haveAuthorization = false;
                Print("Logging out");
            }
            else if (_authorizationState is TdApi.AuthorizationStateClosing)
            {
                _haveAuthorization = false;
                Print("Closing");
            }
            else if (_authorizationState is TdApi.AuthorizationStateClosed)
            {
                Print("Closed");
                if (!_needQuit)
                {
                    _client = CreateTdClient(msgListener); // recreate _client after previous has closed
                }
                else
                {
                    _canQuit = true;
                }
            }
            else
            {
                Print("Unsupported authorization state:" + loginPo.ToString() + _authorizationState);
            }
        }



        public void GetChatMembers()
        {
            long chatId = 12345; //Replace with your group chat Id
            DateTimeOffset fromDate = DateTimeOffset.Parse("2023-06-07"); //Replace with the start date you want
            DateTimeOffset toDate = DateTimeOffset.Parse("2023-06-08"); //Replace with the end date you want

            var fromUnixTime = new DateTimeOffset(fromDate.Date).ToUnixTimeSeconds();
            var toUnixTime = new DateTimeOffset(toDate.Date).ToUnixTimeSeconds();

            TdApi.ChatListMain chatListMain = new TdApi.ChatListMain();

            _client.Send(new TdApi.GetChatHistory()
            {
                ChatId = -1001078465602L, // 替换specificChatId为要查询的chat_id
                Limit = 10,
                FromMessageId = 0,
                Offset = 0,
                OnlyLocal = false
            }, new TestClientResultHandler());

        }


        public void GetGroupMembers(long groupId)
        {
            _client.Send(new TdApi.GetChatAdministrators(groupId), new ParseGroupHandler(this));

        }

        private OperatorType currentOperatorType = OperatorType.SearchChat;
        private long currentChatId = 0;
        private long currentGruopId = 0;
        private long currentChatLastMsgId = 0;
        private int startIndex = 0;
        private int endIndex = 0;
        private TimeFilterType currentTimeFilterType = TimeFilterType.None;
        private int currentCollectNum = 0;
        private int limit = 200;
        private string currentGroupName = string.Empty;
        private string currentGroupUrl = string.Empty;


        public void CollectUser(string groupUrl, TimeFilterType timeFilterType, int collectNum)
        {
            MsgHandler.Instance.ClearCache();
            currentGroupUrl = groupUrl;
            currentOperatorType = OperatorType.None;
            currentCollectNum = collectNum;
            currentTimeFilterType = timeFilterType;
            if (!string.IsNullOrEmpty(groupUrl))
            {
                string name = groupUrl.Replace("https://t.me/", "");
                currentGroupName = name;
                currentOperatorType = OperatorType.SearchChat;
                _client.Send(new TdApi.SearchPublicChat() { Username = name }, this);


            }
            
        }


        public void GetCommand(LoginPo loginPo, string command)
        {
            command = "me";
            command = "groupsInCommon";
            command = "chats";
            this.loginPo = loginPo;
            string[] commands = command.Split(new char[] { ' ' }, 2);
            try
            {
                switch (commands[0])
                {
                    case "groupsInCommon":
                        //_client.Send(new TdApi.GetContacts(), new TestClientResultHandler());
                        _client.Send(new TdApi.GetUser(974541270), new TestClientResultHandler());
                        _client.Send(new TdApi.GetGroupsInCommon(974541270, 0, 100), new TestClientResultHandler());

                        break;
                    case "chats":
                        TdApi.GetChats getChats = new TdApi.GetChats();
                        getChats.Limit = 100;
                        _client.Send(getChats, new TestClientResultHandler());

                        
                        TdApi.GetChat getChat = new TdApi.GetChat() { ChatId = -1001307554905L };

                        _client.Send(getChat, new TestClientResultHandler());


                        break;
                    case "gc":
                        _client.Send(new TdApi.GetChat(GetChatId(commands[1])), _defaultHandler);
                        break;
                    case "me":
                        _client.Send(new TdApi.GetMe(), new TestClientResultHandler());
                        break;
                    case "sm":
                        string[] args = commands[1].Split(new char[] { ' ' }, 2);
                        sendMessage(GetChatId(args[0]), args[1]);
                        break;
                    case "lo":
                        _haveAuthorization = false;
                        _client.Send(new TdApi.LogOut(), _defaultHandler);
                        break;
                    case "r":
                        _haveAuthorization = false;
                        _client.Send(new TdApi.Close(), _defaultHandler);
                        break;
                    case "q":
                        _needQuit = true;
                        _haveAuthorization = false;
                        _client.Send(new TdApi.Close(), _defaultHandler);
                        break;
                    default:
                        Print("Unsupported command: " + command);
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Print("Not enough arguments");
            }
        }

        private void sendMessage(long chatId, string message)
        {
            // initialize reply markup just for testing
            TdApi.InlineKeyboardButton[] row = { new TdApi.InlineKeyboardButton("https://telegram.org?1", new TdApi.InlineKeyboardButtonTypeUrl()), new TdApi.InlineKeyboardButton("https://telegram.org?2", new TdApi.InlineKeyboardButtonTypeUrl()), new TdApi.InlineKeyboardButton("https://telegram.org?3", new TdApi.InlineKeyboardButtonTypeUrl()) };
            TdApi.ReplyMarkup replyMarkup = new TdApi.ReplyMarkupInlineKeyboard(new TdApi.InlineKeyboardButton[][] { row, row, row });

            TdApi.InputMessageContent content = new TdApi.InputMessageText(new TdApi.FormattedText(message, null), false, true);
            _client.Send(new TdApi.SendMessage(chatId, 0, 0, null, replyMarkup, content), _defaultHandler);
        }



        private long GetChatId(string arg)
        {
            long chatId = 0;
            try
            {
                chatId = Convert.ToInt64(arg);
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }
            return chatId;
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
                    if (currentOperatorType == OperatorType.SearchChat)
                    {
                        TdApi.Chat chat = @object as TdApi.Chat;

                        if (chat != null && chat.Type is TdApi.ChatTypeSupergroup)
                        {
                            TdApi.ChatTypeSupergroup chatTypeSupergroup = chat.Type as TdApi.ChatTypeSupergroup;
                            currentGruopId = chatTypeSupergroup.SupergroupId;
                            if (chat.LastMessage != null)
                                currentChatLastMsgId = chat.LastMessage.Id;

                            currentChatId = chat.Id;
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
                        currentOperatorType = OperatorType.SearchSupergroupFullInfo;
                        _client.Send(new TdApi.GetSupergroupFullInfo() { SupergroupId = currentGruopId }, this);


                    }
                    else if (currentOperatorType == OperatorType.SearchSupergroupFullInfo)
                    {
                        TdApi.SupergroupFullInfo supergroupFullInfo = @object as TdApi.SupergroupFullInfo;

                        if (supergroupFullInfo != null)
                        {
                            if (supergroupFullInfo.CanGetMembers)
                            {
                                if (currentTimeFilterType == TimeFilterType.None)//无活跃条件筛选时，获取全部成员
                                {
                                    //获取用户列表
                                    startIndex = 0;
                                    endIndex = limit;

                                    currentOperatorType = OperatorType.SearchSupergroupMembers;

                                    _client.Send(new TdApi.GetSupergroupMembers() { SupergroupId = currentGruopId, Filter = new TdApi.SupergroupMembersFilterRecent(), Offset = startIndex, Limit = limit }, this);

                                }
                                else
                                {
                                    currentOperatorType = OperatorType.SearchChatHistory;


                                    startIndex = -50;
                                    endIndex = 100;
                                    _client.Send(new TdApi.GetChatHistory()
                                    {
                                        ChatId = currentChatId,
                                        Limit = endIndex,
                                        FromMessageId = 0,//currentChatLastMsgId,
                                        Offset = 0,//startIndex,
                                        OnlyLocal = false
                                    }, this);
                                }
                            }
                            else
                            {
                                UserHandler.Instance.PublishMsg("-------GetSupergroupFullInfo:" + currentGroupUrl + " is not CanGetMembers");
                                return;
                            }
                        }
                        else
                        {
                            UserHandler.Instance.PublishMsg("-------GetSupergroupFullInfo:" + currentGroupUrl + " is null");
                            return;
                        }

                    }
                    else if (currentOperatorType == OperatorType.SearchSupergroupMembers)//无活跃条件筛选时，获取全部成员
                    {
                        TdApi.ChatMembers chatMembers = @object as TdApi.ChatMembers;
                        if (chatMembers != null)
                        {
                            CommonHandler.Instance.PublishMemberChange(chatMembers.TotalCount, chatMembers.Members.Length);

                            MsgHandler.Instance.AddOrUpdateByMember(chatMembers, currentCollectNum);

                            int loadCount = MsgHandler.Instance.GetLoadUserCount();

                            if (chatMembers.Members.Length >= limit && (currentCollectNum == 0 || (currentCollectNum > 0 && startIndex <= currentCollectNum)))
                            {
                                Thread.Sleep(500);

                                startIndex += chatMembers.Members.Length;

                                _client.Send(new TdApi.GetSupergroupMembers() { SupergroupId = currentGruopId, Filter = new TdApi.SupergroupMembersFilterRecent(), Offset = startIndex, Limit = limit }, this);

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
                    else if (currentOperatorType == OperatorType.SearchChatHistory)
                    {
                        TdApi.Messages messages = @object as TdApi.Messages;

                        if (messages != null)
                        {
                            int lastTime = MsgHandler.Instance.AddOrUpdateByMessage(messages, currentTimeFilterType);

                            bool isContinue = MsgHandler.Instance.ContinueSearchHis(lastTime, currentTimeFilterType);


                            if (isContinue && messages.TotalCount > 50 && messages.MessagesValue.Length >= 50)
                            {
                                long lastMsgId = messages.MessagesValue[messages.MessagesValue.Length - 1].Id;

                                startIndex = -50;
                                endIndex = 50;
                                _client.Send(new TdApi.GetChatHistory()
                                {
                                    ChatId = currentChatId,
                                    Limit = limit,
                                    FromMessageId = lastMsgId,
                                    Offset = startIndex,
                                    OnlyLocal = false
                                }, this);

                                Thread.Sleep(100);
                            }
                            else
                            {
                                Task.Run(() =>
                                {
                                    GetUserDetail();
                                });

                            }
                        }
                    }
                    else if (currentOperatorType == OperatorType.SearchChatUser)
                    {
                        TdApi.User user = @object as TdApi.User;
                        if (user != null)
                        {
                            if (user.Type is TdApi.UserTypeBot)
                            {
                                UserHandler.Instance.PublishMsg("Filter bot:" + user.FirstName + " " + user.LastName);
                                return;
                            }
                            user.RestrictionReason = currentGroupName;
                            OnUserChange?.Invoke(user);

                            MsgHandler.Instance.AddOrUpdateUser(user);
                        }
                    }
                }
                else
                {
                    UserHandler.Instance.PublishMsg("APICollect OnResult currentOperatorType:" + currentOperatorType + " result is null");
                }
            }
            catch (Exception e)
            {
                UserHandler.Instance.PublishMsg("APICollect OnResult exeception:" + e.Message);
            }
        }

        private void GetUserDetail()
        {
            startIndex = 0;
            endIndex = 0;

            Thread.Sleep(2000);
            endIndex += 500;
            currentOperatorType = OperatorType.SearchChatUser;

            HashSet<long> idSet = MsgHandler.Instance.GetAllId();

            int index = 0;
            foreach (long userId in idSet)
            {
                if (currentCollectNum != 0 && index >= currentCollectNum)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("GetUser by UserId:" + userId);
                    _client.Send(new TdApi.GetUser() { UserId = userId }, this);
                }

                index++;


                if (index % 100 == 0)
                    Thread.Sleep(2000);
                else
                    Thread.Sleep(150);
            }

            UserHandler.Instance.PublishMsg("Get user detail end...");
        }

    }

    
}
