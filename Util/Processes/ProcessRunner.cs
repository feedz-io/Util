// Apache 2.0 Licence
// From https://github.com/OctopusDeploy/Calamari/blob/master/source/Calamari/Integration/Processes/SilentProcessRunner.cs

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Feedz.Util.Extensions;

namespace Feedz.Util.Processes
{
    public class ProcessRunner
    {
        public class Result
        {
            public int ExitCode { get; }
            public string Output { get; }
            public string Error { get; }

            public Result(int exitCode, string output, string error)
            {
                ExitCode = exitCode;
                Output = output;
                Error = error;
            }
        }

        public static Result Execute(
            string executable,
            string arguments,
            string workingDirectory = null,
            StringDictionary environmentVars = null,
            CancellationToken token = default(CancellationToken),
            TimeSpan? waitForFinalOutputTimeout = null)
        {
            var output = new StringBuilder();
            var error = new StringBuilder();
            var exitCode = Execute(
                executable,
                arguments,
                s => output.AppendLine(s),
                s => error.AppendLine(s),
                workingDirectory,
                environmentVars,
                token
            );
            return new Result(exitCode, output.ToString(), error.ToString());
        }

        public static int Execute(
            string executable,
            string arguments,
            Action<string> output,
            Action<string> error,
            string workingDirectory = null,
            StringDictionary environmentVars = null,
            CancellationToken token = default(CancellationToken),
            TimeSpan? waitForFinalOutputTimeout = null)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = executable;
                    process.StartInfo.Arguments = arguments;
                    if (workingDirectory != null)
                        process.StartInfo.WorkingDirectory = workingDirectory;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardInput = true;

                    if (environmentVars != null)
                        foreach (string environmentVar in environmentVars.Keys)
                            process.StartInfo.EnvironmentVariables[environmentVar] = environmentVars[environmentVar];


                    using (var outputWaitHandle = new AutoResetEvent(false))
                    using (var errorWaitHandle = new AutoResetEvent(false))
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            try
                            {
                                if (e.Data == null)
                                    outputWaitHandle.Set();
                                else
                                    output(e.Data);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    error($"Error occured handling message: {ex.PrettyPrint()}");
                                }
                                catch
                                {
                                    // Ignore
                                }
                            }
                        };

                        process.ErrorDataReceived += (sender, e) =>
                        {
                            try
                            {
                                if (e.Data == null)
                                    errorWaitHandle.Set();
                                else
                                    error(e.Data);
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    error($"Error occured handling message: {ex.PrettyPrint()}");
                                }
                                catch
                                {
                                    // Ignore
                                }
                            }
                        };

                        RegisterCancel(token, process);

                        process.Start();
                        ChildProcessTracker.AddProcess(process);

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        while(!process.WaitForExit(100))
                        {}

                        outputWaitHandle.WaitOne(waitForFinalOutputTimeout ?? Timeout.InfiniteTimeSpan);
                        errorWaitHandle.WaitOne(waitForFinalOutputTimeout ?? Timeout.InfiniteTimeSpan);

                        return process.ExitCode;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when attempting to execute {executable}: {ex.Message}", ex);
            }
        }

        private static void RegisterCancel(CancellationToken token, Process process)
        {
            token.Register(() =>
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                }
            });
        }
    }
}