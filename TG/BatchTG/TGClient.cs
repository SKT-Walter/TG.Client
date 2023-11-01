using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Td.Api;
using TG.Client.Cache;
using TG.Client.Handler;
using TG.Client.Model;
using TG.Client.TG;
using TG.Client.ViewModel.MoreAccLogin;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.BatchTG
{
    public class TGClient : Td.ClientResultHandler, IMessage
    {

        private Telegram.Td.Client _client = null;
        private TdApi.AuthorizationState _authorizationState = null;
        private Telegram.Td.ClientResultHandler _defaultHandler = new DefaultHandler();
        private volatile bool _haveAuthorization = false;
        private volatile bool _needQuit = false;
        private volatile bool _canQuit = false;
        private volatile AutoResetEvent _gotAuthorization = new AutoResetEvent(false);
        
        private LoginViewModel loginData;
        
        public Td.Client GetClient()
        {
            return _client;
        }

        public Td.Client CreateTdClient(LoginViewModel loginData)
        {
            if (this.loginData == null)
            {
                this.loginData = loginData;
            }

            if (_client == null)
            {
                //Td.Client.Execute(new TdApi.SetLogVerbosityLevel(0));
                //if (Td.Client.Execute(new TdApi.SetLogStream(new TdApi.LogStreamFile("tdlib.log", 1 << 27, false))) is TdApi.Error)
                //{
                //    throw new System.IO.IOException("Write access to the current directory is required");
                //}
                //new Thread(() =>
                //{
                //    Thread.CurrentThread.IsBackground = true;
                //    Td.Client.Run();
                //}).Start();

                Td.Client.Execute(new TdApi.SetLogVerbosityLevel(0));
                int number = TGClientManager.Instance.GetCount();
                //string fileName = "tdlib" + number + ".log";
                //if (Td.Client.Execute(new TdApi.SetLogStream(new TdApi.LogStreamFile(fileName, 1 << 27, false))) is TdApi.Error)
                //{
                //    throw new System.IO.IOException("Write access to the current directory is required");
                //}
                
                _client = Td.Client.Create(new InnerCallHandler(this));

                AnalysisChatHandler.Instance.Init(_client);

                // test Client.Execute
                _defaultHandler.OnResult(Td.Client.Execute(new TdApi.GetTextEntities("@telegram /test_command https://telegram.org telegram.me @gif @test")));

                
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

                int number = TGClientManager.Instance.GetCount();

                request.DatabaseDirectory = "tdlib" + number;
                request.UseMessageDatabase = false;
                request.UseSecretChats = false;
                request.UseChatInfoDatabase = false;
                request.UseFileDatabase = false;
                request.UseMessageDatabase = false;
                request.UseTestDc = false;
                //request.ApiId = 94575;
                //request.ApiHash = "a3406de8d171bb422bb6ddf3bbd800e2";

                string api_id = loginData.APIID;//ConfigurationManager.AppSettings["API_ID"];
                string api_hash = loginData.APIHASH;//ConfigurationManager.AppSettings["API_HASH"];
                int API_ID = Int32.Parse(api_id);
                string API_HASH = api_hash;

                request.ApiId = API_ID;
                request.ApiHash = API_HASH;

                request.SystemLanguageCode = "en";
                request.DeviceModel = "Desktop";
                request.ApplicationVersion = "1.0";
                request.EnableStorageOptimizer = true;

                _client.Send(request, this);
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitCode)
            {
                if (_client != null && !isSendCode && loginData != null)
                {
                    string code = loginData.VerifyCode;//loginPo.AuthenticationCode;//ReadLine("Please enter authentication code: ");
                    _client.Send(new TdApi.CheckAuthenticationCode(code), this);

                    isSendCode = true;
                }
            }
            else if (_authorizationState is TdApi.AuthorizationStateReady)
            {
                BaseReplyPo replyPo = new BaseReplyPo();
                replyPo.Msg = string.Format("账号:{0},验证成功！", loginData.Account);
                loginData.OnLoginStatus(true);
                this.OnMessage(replyPo);

            }
            else
            {
                Print("Unsupported authorization state:" + (loginData == null ? string.Empty : loginData.ToString()) + " " + _authorizationState);
            }
        }

        public void ProcessLogin()
        {
            if (_authorizationState is TdApi.AuthorizationStateWaitPhoneNumber)
            {
                string phoneNumber = loginData.SendPhone;//ReadLine("Please enter phone number: ");
                if (_client != null && !string.IsNullOrEmpty(phoneNumber))
                    _client.Send(new TdApi.SetAuthenticationPhoneNumber(phoneNumber, null), new InnerCallHandler(this));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitEmailAddress)
            {
                string emailAddress = string.Empty;// loginPo.Email;//ReadLine("Please enter email address: ");
                if (_client != null && !string.IsNullOrEmpty(emailAddress))
                    _client.Send(new TdApi.SetAuthenticationEmailAddress(emailAddress), new InnerCallHandler(this));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitEmailCode)
            {
                string code = string.Empty;// loginPo.EmailAuthenticationCode;//ReadLine("Please enter email authentication code: ");
                if (_client != null && !string.IsNullOrEmpty(code))
                    _client.Send(new TdApi.CheckAuthenticationEmailCode(new TdApi.EmailAddressAuthenticationCode(code)), new InnerCallHandler(this));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitOtherDeviceConfirmation state)
            {
                Console.WriteLine("Please confirm this login link on another device: " + state.Link);
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitCode)
            {
                string code = loginData.VerifyCode;//loginPo.AuthenticationCode; ;//ReadLine("Please enter authentication code: ");
                if (_client != null && !string.IsNullOrEmpty(code))
                    _client.Send(new TdApi.CheckAuthenticationCode(code), new InnerCallHandler(this));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitRegistration)
            {
                string firstName = string.Empty;//loginPo.FirstName;//ReadLine("Please enter your first name: ");
                string lastName = string.Empty;//loginPo.LastName;//ReadLine("Please enter your last name: ");
                if (_client != null)
                    _client.Send(new TdApi.RegisterUser(firstName, lastName), new InnerCallHandler(this));
            }
            else if (_authorizationState is TdApi.AuthorizationStateWaitPassword)
            {
                string password = string.Empty;//loginPo.Password;//ReadLine("Please enter password: ");
                if (_client != null && !string.IsNullOrEmpty(password))
                    _client.Send(new TdApi.CheckAuthenticationPassword(password), new InnerCallHandler(this));
            }
            else if (_authorizationState is TdApi.AuthorizationStateReady)
            {
                _haveAuthorization = true;
                _gotAuthorization.Set();

                //_client.Send(new TdApi.LoadChats(null, 100), _defaultHandler);

                BaseReplyPo replyPo = new BaseReplyPo();
                replyPo.Msg = "验证成功！";
                loginData.OnLoginStatus(true);
                this.OnMessage(replyPo);
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
                    _client = CreateTdClient(loginData); // recreate _client after previous has closed
                }
                else
                {
                    _canQuit = true;
                }
            }
            else
            {
                Print("Unsupported authorization state:" + loginData.ToString() + _authorizationState);
            }
        }

        public void OnResult(BaseObject obj)
        {
            UserHandler.Instance.PublishMsg(obj);
        }

        private void Print(string str)
        {
            UserHandler.Instance.PublishMsg(str);
        }


        public void OnMessage(object obj)
        {
            UserHandler.Instance.PublishMsg(obj);
            BaseReplyPo baseReply = obj as BaseReplyPo;
            if (baseReply != null)
            {
                //Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    if (baseReply.Code == "1100")
                //    {
                //        this.VerifyForeground = this.ownUI.FindResource("Orange-1") as Brush;
                //    }
                //    else if (baseReply.IsSuccess())
                //    {
                //        this.VerifyForeground = this.ownUI.FindResource("Green-1") as Brush;
                //    }
                //    else
                //    {
                //        this.VerifyForeground = this.ownUI.FindResource("Orange-2") as Brush;
                //    }
                //    this.VerifyMsg = baseReply.Msg;

                //}));
            }
        }
    }


    public class InnerCallHandler : Td.ClientResultHandler
    {
        private TGClient tdClient;

        public InnerCallHandler(TGClient tdClient)
        {
            this.tdClient = tdClient;
        }

        public void OnResult(BaseObject result)
        {
            Console.WriteLine("TGClient:" + result);
            if (result is TdApi.UpdateAuthorizationState)
            {

                tdClient.OnAuthorizationStateUpdated((result as TdApi.UpdateAuthorizationState).AuthorizationState);
            }
            else
            {
                Console.WriteLine("Unsupported update: " + result);
            }
        }
    }
}
