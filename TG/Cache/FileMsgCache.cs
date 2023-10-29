using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.Handler;

namespace TG.Client.Cache
{
    public class FileMsgCache
    {
        private List<string> fileMsgList = new List<string>();
        private List<string> imageMsgList = new List<string>();

        private static FileMsgCache server = new FileMsgCache();
        
        public static FileMsgCache Instance { get { return server; } }

        private FileMsgCache()
        {
            //ReadMsg();

            //ReadImage();
        }

        public void Init()
        {
            ReadMsgFromFile();

            ReadImage();
        }

        private void ReadMsgFromFile()
        {
            try
            {
                // 文件路径
                string filePath = "./data/MSG.txt";

                // 创建一个StreamReader对象来读取文件内容
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;

                    // 逐行读取文件内容，直到文件末尾
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line.Trim()))
                        {
                            fileMsgList.Add(line.Trim());
                            UserHandler.Instance.PublishMsg("读取消息:" + line.Trim());
                        }
                    }

                    UserHandler.Instance.PublishMsg("读取消息总数:" + fileMsgList.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取文件时出错：" + ex.Message);
            }
        }

        private void ReadMsg()
        {
            string msg1 = "RubyDex - The Future of Derivatives Trading! 🚀 Trade everything from cryptocurrencies to traditional financial assets like stocks and commodities all in one place. Dive in today and explore the vast world of perpetual contracts. Experience trading like never before! 🔥➡️ https://rubydex.com/en";      
            string msg2 = "Did You Hear About NFT Perpetuals? With RubyDex, you can now trade NFT collections at floor prices, using data sourced from leading platforms such as OpenSea. The future of NFT trading is here! 🎉 Discover more ➡️ ";
            string msg3 = "Bridge the Gap with RubyDex! 🌍 Merging traditional finance with the crypto world. From stocks, ETFs, commodities, to crypto, and even NFT perps. Multi-chain, diverse, and decentralized! Get started ➡️";
            string msg4 = "High Leverage Trading on Forex Pairs! With up to 1000x leverage on pairs like GBPUSDT and EUROUSDT, RubyDex caters to the sophisticated trader in you. Step into the world of high returns today! 💰 Join the revolution ➡️";
            string msg5 = "Exclusive RubyDex OG Gem NFT Offer! 🌟 Holding the OG Gem NFT? Enjoy trading fee discounts, staking rewards, and exclusive access to the RubyDex Token Launchpad! Get yours today and unlock a world of benefits! Discover the perks ➡️";
            string msg6 = "6. Multichain Magic with RubyDex! Supporting networks like Arbitrum, BNB Chain, Ethereum, and more, we're capturing the best from all worlds. Seamless trading, wider reach! Dive into the multichain marvel ➡️ ";
            string msg7 = "Experience Hybrid Trading with RubyDex! Combining the liquidity and efficiency of centralized exchanges with decentralized security. Your funds, your control, top-notch trading! 🛡️ Start now ➡️";
            string msg8 = "RubyDex Airdrop Missions are LIVE! Embark on our thrilling journey, collect points, and unlock exclusive token airdrop rewards! Don't miss out on this grand adventure! 🎁 Participate now ➡️";
            string msg9 = "Future-Ready with RubyDex! Our roadmap promises more TradFi perpetual contracts, multi-language support, and an innovative liquidity pool system. We're reshaping decentralized trading for the global community! 🌐 Dive in ➡️";
            string msg10 = "Mobile Support & Comprehensive Guides! Trade on the go with RubyDex and get the assistance you need with our detailed user guides. Perfect for both beginners and pros! 📚 Explore ➡️ https://support.rubydex.com/en";
            

            fileMsgList.Add(msg1);
            fileMsgList.Add(msg2);
            fileMsgList.Add(msg3);
            fileMsgList.Add(msg4);
            fileMsgList.Add(msg5);
            fileMsgList.Add(msg6);
            fileMsgList.Add(msg7);
            fileMsgList.Add(msg8);
            fileMsgList.Add(msg9);
            fileMsgList.Add(msg10);

            

        }

        private void ReadImage()
        {
            string basePath = "./Image/";
            // 检查目录是否存在
            if (Directory.Exists(basePath))
            {
                // 获取目录中的所有文件
                string[] files = Directory.GetFiles(basePath);
                string appStartupPath = AppDomain.CurrentDomain.BaseDirectory;
                // 筛选出图像文件（您可以根据需要添加其他文件类型的筛选条件）
                foreach (string filePath in files)
                {
                    string extension = Path.GetExtension(filePath).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
                    {
                        string fullPath = appStartupPath + "Image\\" + filePath.Substring(basePath.Length, filePath.Length - basePath.Length);
                        UserHandler.Instance.PublishMsg("读取图片:" + fullPath);
                        imageMsgList.Add(fullPath);
                    }
                }

                UserHandler.Instance.PublishMsg("读取图片总数:" + imageMsgList.Count);
            }
            else
            {
                // 目录不存在的处理逻辑
            }
        }


        private int currentTxtIndex = 0;
        public string GetMsg()
        {
            Random random = new Random();
            string result = fileMsgList[random.Next(fileMsgList.Count - 1)];
            currentTxtIndex++;

            if (currentTxtIndex == fileMsgList.Count)
            {
                currentTxtIndex = 0;
            }

            return result;
        }

        private int curremtImageIndex = 0;
        public string GetImage()
        {
            Random random = new Random();
            string result = imageMsgList[random.Next(imageMsgList.Count - 1)];
            curremtImageIndex++;

            if (curremtImageIndex == imageMsgList.Count)
            {
                curremtImageIndex = 0;
            }

            return result;
        }
    }
}
