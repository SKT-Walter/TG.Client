using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TG.Client.Cache;
using TG.Client.Handler;
using TG.Client.Model;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.BatchTG
{
    public class AddContactHandler : Td.ClientResultHandler
    {


        public void SendBatchMsg(string users)
        {
            string[] userArr = users.Split(new char[] { '\n' });

            Dictionary<string, TGClient> dic = TGClientManager.Instance.GetAllClient();

            foreach (KeyValuePair<string, TGClient> kv in dic)
            {
                UserHandler.Instance.PublishMsg("TGClient:" + kv.Key + ", start send msg...");
                string account = kv.Key;
                Td.Client client = kv.Value.GetClient();
                if (client != null)
                {
                    Task.Run(() =>
                    {
                        foreach (string user in userArr)
                        {
                            long userId = 0;
                            string name = user.Replace("@", "").Replace("\r", "");
                            //TdUserPo userPo = UserHandler.Instance.QuoteUserByName(name);
                            TdUserEx userPo = UserHandler.Instance.QuoteUserExByName(name);
                            if (userPo != null)
                            {
                                userId = userPo.UserId;
                            }

                            
                            //MsgHandler.Instance.GetIdByName(user);
                            if (userId != 0)
                            {
                                //userId = 1480565976;
                                //userId = 1299076404;
                                TdApi.Contact contact = new TdApi.Contact();
                                contact.UserId = userId;
                                contact.FirstName = userPo.FirstName;
                                contact.LastName = userPo.LastName;
                                contact.Vcard = string.Empty;
                                contact.PhoneNumber = string.Empty;

                                UserHandler.Instance.PublishMsg("Account:" + account + ", 发送添加好友消息到用户：" + name + ", userId:" + userId);

                                client.Send(new TdApi.AddContact(contact, false), this);
                                //client.Send(new TdApi.GetUser() { UserId = userId }, this);

                                Thread.Sleep(1000);
                                
                            }
                            else
                            {
                                UserHandler.Instance.PublishMsg("userId:" + userId + ", 没有查询到数据从TdUserEX表.");
                            }
                        }
                    });
                }
                break;
            }
            
        }


        public void OnResult(TdApi.BaseObject baseObject)
        {
            //Console.WriteLine("BatchSendMsgHandler:" + baseObject.ToString());
            if (baseObject != null)
            {
                UserHandler.Instance.PublishMsg("resp:" + baseObject.ToString());
                
            }
        }

    }
}
