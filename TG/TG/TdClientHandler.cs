using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TG.Client.Model;
using TG.Client.Model.Login;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    public class TdClientHandler
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
            }

            return _client;
        }




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


        public void CollectUser(string groupUrl)
        {
            if (!string.IsNullOrEmpty(groupUrl))
            {
                //https://t.me/+wa-XgMOKHRxlYTU9
                string inviteLink = groupUrl;//.Replace("https://t.me/", "");
                _client.Send(new TdApi.CheckChatInviteLink(inviteLink), new ParseGroupHandler(this));
            }
        }


        public void GetCommand(LoginPo loginPo, string command)
        {
            this.loginPo = loginPo;
            string[] commands = command.Split(new char[] { ' ' }, 2);
            try
            {
                switch (commands[0])
                {
                    case "chats":
                        TdApi.GetChats getChats = new TdApi.GetChats();
                        getChats.Limit = 100;
                        _client.Send(getChats, _defaultHandler);
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
    }

    
}
