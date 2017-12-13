using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Paket.Bootstrapper.HelperProxies;

namespace Paket.Bootstrapper.DownloadStrategies
{
    internal class ScriptWrapper
    {
        private IFileSystemProxy FileSystemProxy { get; set; }

        public ScriptWrapper(IFileSystemProxy fileSystemProxy)
        {
            FileSystemProxy = fileSystemProxy;
        }

        public void CreateWrapper(string target)
        {
            string paketToolRuntimeRelativePath = "paket.exe";
            string paketToolRuntimeHostWin = "";
            string paketToolRuntimeHostLinux = "mono ";

            string cmdWrapperPath = Path.ChangeExtension(target, ".cmd");
            FileWriteAllLines(cmdWrapperPath, new[] {
                "@ECHO OFF",
                "",
                @"set np=%~dp0..\paket-files\paket\bin",
                "",
                "REM expand to full path",
                @"for %%a in (""%np%"") do (",
                "    set nppath=%%~fa",
                ")",
                "",
                "REM add to PATH if not exists already",
                @"echo %path%|find /i ""%nppath%"">nul  || set path=%nppath%;%path%",
                "",
                string.Format(@"{0}""%~dp0{1}"" %*", paketToolRuntimeHostWin, paketToolRuntimeRelativePath)
            });

            string shellWrapperPath = Path.ChangeExtension(target, null);
            FileWriteAllLines(shellWrapperPath, new[] {
                "#!/bin/sh",
                "",
                string.Format(@"{0}""$(dirname ""$0"")/{1}"" ""$@""", paketToolRuntimeHostLinux, paketToolRuntimeRelativePath.Replace('\\', '/'))
            }, "\n");

            if (!OSHelper.IsWindow)
            {
                try
                {
                    ConsoleImpl.WriteTrace("running chmod+x on '{0}' ...", shellWrapperPath);
                    int exitCode = RunShell("chmod", string.Format(@"+x ""{0}"" ", shellWrapperPath));
                    if (exitCode != 0)
                        ConsoleImpl.WriteError("chmod+x failed with exit code {0}, execute it manually", exitCode);
                }
                catch (Exception e)
                {
                    ConsoleImpl.WriteError("Running chmod+x failed with: {0}", e);
                    throw;
                }
            }
            else
            {
                ConsoleImpl.WriteTrace("chmod+x of '{0}' skipped on windows, execute it manually if needed", shellWrapperPath);
            }
        }

        private void FileWriteAllLines(string path, string[] lines, string lineEnding = null)
        {
            ConsoleImpl.WriteTrace("writing file '{0}' ...", path);

            using (var stream = FileSystemProxy.CreateFile(path))
            using (var writer = new StreamWriter(stream))
            {
                if (lineEnding != null)
                {
                    writer.NewLine = lineEnding;
                }

                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        private int RunShell(string program, string argString)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = program,
                    Arguments = argString,
                    UseShellExecute = true
                }
            };
            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
