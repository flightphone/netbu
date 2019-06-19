using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using netbu.Models;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;




namespace netbu.Controllers
{

    public class NodeJSController : Controller
    {

        private void startnode()
        {
            try
            {
                Program.isNodeStart = true;
                while (Program.isNodeStart)
                {
                    Program.nodesv = new System.Diagnostics.Process();
                    Program.nodesv.StartInfo.FileName = @"wwwroot\Run\nodesv.cmd"; ;
                    Program.nodesv.Start();
                    Program.nodesv.WaitForExit();
                }

            }
            catch
            {
                Program.isNodeStart = false;
            }
        }

        private async void startnodeAsync()
        {

            await Task.Run(() => startnode());

        }

        public string start()
        {
            startnodeAsync();
            return "Cервер NodeJs запущен.";
        }

        public string stop()
        {
            if (Program.nodesv == null)
                return "Сервер NodeJs  не был запущен.";

            Program.isNodeStart = false;
            if (!Program.nodesv.HasExited)
                Program.nodesv.Kill();
            Process[] nd = Process.GetProcessesByName("node");
            foreach (Process p in nd)
            {
                if (!p.HasExited)
                    p.Kill();
            }

            return "Сервер NodeJs остановлен.";
        }

    }
}