using System;
using System.Threading;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace MCHS_Resender
{
    class Program
    {


        static void Main(string[] args)
        {
            try
            {
                int num = 0;
                TimerCallback tm = new TimerCallback(Run);
                Timer timer = new Timer(tm, num, 0, 200000);
                Console.ReadLine();
            }
            catch
            {
                Console.WriteLine("Таймер не запущен");
                Console.ReadLine();
            }
        }


        public static void Run(object obj)
        {
            CFG cfg = new CFG(); 
            int count_messages = 0;
            string fr_id = String.Empty;
            Resender.CopyDataFile(cfg.Path);
            List<New_Message> New_Messages = new List<New_Message>();
            New_Messages = Resender.Get(cfg);
            Resender.Set(New_Messages, cfg);
            count_messages = New_Messages.Count;
            Console.WriteLine(DateTime.Now.ToString() + " переотправлено сообщений:" + count_messages);
            Console.ReadLine();
        }
    }
}
