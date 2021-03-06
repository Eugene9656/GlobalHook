﻿using KeyRecord.UserForm;
using MetroFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyRecord
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //处理未捕获的异常   
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常   
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常   
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Process[] pro = Process.GetProcessesByName("KeyRecord");
                if (pro != null && pro.Length > 1)
                {
                    DialogResult dir=MetroMessageBox.Show(CSGlobal.GetInstance().hookFrm, "已经有一个KeyRecord程序正在运行,是否继续？", "系统提示", MessageBoxButtons.YesNo);
                    if(dir==DialogResult.No)
                    {
                        Application.Exit();
                        return;
                    }
                }
                log4net.Config.XmlConfigurator.Configure();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(CSGlobal.GetInstance().hookFrm);
                //Application.Run(new HeatMapFrm());
            }
            catch (Exception ex)
            {
                string str = GetExceptionMsg(ex, string.Empty);
                CSGlobal.GetInstance().Log.Info(DateTime.Now.ToString() + ":系统错误\n" + str);
                MetroMessageBox.Show(CSGlobal.GetInstance().hookFrm, 
                    "发生了未经处理的异常\n" + ex.ToString().Split(new char[2] { '\n', '\r' })[0], 
                    "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            CSGlobal.GetInstance().Log.Info(DateTime.Now.ToString() + ":系统错误\n" + str);
            MetroMessageBox.Show(CSGlobal.GetInstance().hookFrm, 
                "发生了未经处理的UI异常\n" + e.ToString().Split(new char[2] { '\n', '\r' })[0], 
                "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            CSGlobal.GetInstance().Log.Info(DateTime.Now.ToString() + ":系统错误\n" + str);
            MetroMessageBox.Show(CSGlobal.GetInstance().hookFrm, 
                "发生了未经处理的非UI异常\n" + e.ToString().Split(new char[2] { '\n', '\r' })[0], 
                "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>  
        /// 生成自定义异常消息  
        /// </summary>  
        /// <param name="ex">异常对象</param>  
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>  
        /// <returns>异常字符串文本</returns>  
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("****************************************************************");
            return sb.ToString();
        }
    }
}
