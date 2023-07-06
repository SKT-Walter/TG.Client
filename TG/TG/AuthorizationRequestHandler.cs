using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.Model;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;


namespace TG.Client.TG
{
    class AuthorizationRequestHandler : Td.ClientResultHandler
    {
        private TdClientHandler tdClientHandler;
        private IMessage msgListener;
        private TdApi.AuthorizationState authorizationState;

        public AuthorizationRequestHandler(TdClientHandler tdClientHandler, IMessage msgListener, TdApi.AuthorizationState authorizationState)
        {
            this.tdClientHandler = tdClientHandler;
            this.msgListener = msgListener;
            this.authorizationState = authorizationState;
        }

        void Td.ClientResultHandler.OnResult(TdApi.BaseObject @object)
        {
            BaseReplyPo resp = new BaseReplyPo();
            if (@object != null)
            {
                resp.Msg = @object.ToString();
            }
            else
            {
                resp.Msg = "AuthorizationRequestHandler result is null";
            }
            
            msgListener.OnMessage(resp);
            if (@object is TdApi.Error)
            {
                Console.WriteLine("Receive an error:" + @object);
                tdClientHandler.OnAuthorizationStateUpdated(null); // repeat last action
            }
            else
            {
                // result is already received through UpdateAuthorizationState, nothing to do
                if (authorizationState != null)
                {
                    BaseReplyPo replyPo = new BaseReplyPo();

                    if (authorizationState is TdApi.AuthorizationStateWaitPhoneNumber)
                    {
                        replyPo.Msg = "进入电话号码验证，格式如：+86 132123456789";
                        replyPo.Code = "1001";
                        msgListener.OnMessage(replyPo);

                    }
                    else if (authorizationState is TdApi.AuthorizationStateWaitEmailAddress)
                    {
                        replyPo.Msg = "进入邮件地址验证";
                        replyPo.Code = "1002";
                        msgListener.OnMessage(replyPo);
                    }
                    else if (authorizationState is TdApi.AuthorizationStateWaitEmailCode)
                    {
                        replyPo.Msg = "进入邮件地址验证验证";
                        replyPo.Code = "1003";
                        msgListener.OnMessage(replyPo);
                    }
                    else if (authorizationState is TdApi.AuthorizationStateWaitOtherDeviceConfirmation state)
                    {
                        replyPo.Msg = "Please confirm this login link on another device: " + state.Link;
                        replyPo.Code = "1004";
                        msgListener.OnMessage(replyPo);
                    }
                    else if (authorizationState is TdApi.AuthorizationStateWaitCode)
                    {
                        replyPo.Msg = "进入验证码验证";
                        replyPo.Code = "1005";
                        msgListener.OnMessage(replyPo);
                    }
                    else if (authorizationState is TdApi.AuthorizationStateWaitRegistration)
                    {
                        replyPo.Msg = "进入注册阶段，输入firstName，lastName";
                        replyPo.Code = "1006";
                        msgListener.OnMessage(replyPo);
                    }
                    else if (authorizationState is TdApi.AuthorizationStateWaitPassword)
                    {
                        replyPo.Msg = "进入密码验证";
                        replyPo.Code = "1007";
                        msgListener.OnMessage(replyPo);
                    }
                    else if (authorizationState is TdApi.AuthorizationStateReady)
                    {
                        replyPo.Msg = "验证成功！";
                        msgListener.OnMessage(replyPo);
                    }
                    else if (authorizationState is TdApi.AuthorizationStateLoggingOut)
                    {
                        replyPo.Msg = "Logo out";

                        msgListener.OnMessage(replyPo);
                    }
                    else
                    {
                        //replyPo.Msg = "Unsupported authorization state:" + authorizationState;
                        //replyPo.Code = "1100";
                        //msgListener.OnMessage(replyPo);
                        Console.WriteLine("Unsupported authorization state:" + authorizationState);
                    }

                }
            }
        }
    }
}
