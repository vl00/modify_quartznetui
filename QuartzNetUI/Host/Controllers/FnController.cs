using Host.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Host.Controllers
{
    [Route("api/[controller]/[Action]")]
    [EnableCors("AllowSameDomain")] //允许跨域 
    public class FnController : Controller
    {
        [HttpPost]
        public async Task<SimResult> Process([FromBody] ProcessModel model)
        {
            var psi = new ProcessStartInfo
            {
                FileName = exnv(model.Name),
                Arguments = exnv(model.Args),
                WorkingDirectory = model.Wd ?? Path.GetDirectoryName(exnv(model.Name)),
                UseShellExecute = !model.Noshell,
                CreateNoWindow = model.NewWin,
                RedirectStandardInput = false,
                RedirectStandardError = false,
                RedirectStandardOutput = false,
            };            
            if (model.Envs != null)
            {
                foreach (var kv in model.Envs)
                {
                    if (kv.Key.StartsWith('!')) psi.Environment.Remove(kv.Key);
                    else psi.Environment[kv.Key] = kv.Value;
                }
            }

            var ok = new TaskCompletionSource<int?>();

            var p = new Process { StartInfo = psi };
            p.EnableRaisingEvents = true;
            p.Exited += (_, e) => 
            {
                ok.TrySetResult(p.ExitCode);
                p.Dispose();
            };
            p.Start();

            var pid = p.Id;
            Task t = ok.Task;

            if (model.Wait == null) ok.TrySetResult(null);
            else t = Task.WhenAny(Task.Delay(model.Wait.Value), ok.Task);
            await t;

            return new SimResult
            {
                Data = new
                {
                    Pid = pid,
                    ExitCode = ok.Task.IsCompleted ? ok.Task.Result : null,
                }
            };
        }

        static string exnv(string str) => string.IsNullOrEmpty(str) ? "" : Environment.ExpandEnvironmentVariables(str);
    }
}
