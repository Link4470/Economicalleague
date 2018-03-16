using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FavoritItemJob;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace Economicalleague.FavoritItemJob
{
    class Program
    {
       
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                //x.UseLog4Net();

                x.Service<ServiceRunner>();

                x.SetDescription("淘宝联盟后台数据导入服务描述");
                x.SetDisplayName("淘宝联盟后台数据导入服务显示名称");
                x.SetServiceName("淘宝联盟后台数据导入服务名称");

                x.EnablePauseAndContinue();

                Log log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
                log.log("启动Job任务" + "----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            });
           
           

        }
    }
}
