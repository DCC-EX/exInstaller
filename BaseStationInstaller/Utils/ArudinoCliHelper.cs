using BaseStationInstaller.ViewModels;
using Cc.Arduino.Cli.Commands;
using Grpc.Core;
using Grpc.Net.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BaseStationInstaller.Utils
{
    public class ArudinoCliHelper
    {
        IMainWindowViewModel mainWindowView;
        GrpcChannel channel;
        ArduinoCore.ArduinoCoreClient client;
        Instance instance;
        public ArudinoCliHelper(IMainWindowViewModel mWindow)
        {
            mainWindowView = mWindow;
            mainWindowView.Busy = true;
            ProcessStartInfo start = new ProcessStartInfo();
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            start.FileName = $@"arduino-cli";
            start.Arguments = "core update-index";
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            Process process = new Process();
            //process.OutputDataReceived += Process_OutputDataReceived;
            //process.ErrorDataReceived += Process_OutputDataReceived;
            //process.StartInfo = start;
            //process.Start();
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            //process.WaitForExit();
            //process = new Process();
            //start.Arguments = "core install arduino:avr";
            //process.OutputDataReceived += Process_OutputDataReceived;
            //process.ErrorDataReceived += Process_OutputDataReceived;
            //process.StartInfo = start;
            //process.Start();
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            //process.WaitForExit();
            //process = new Process();
            //start.Arguments = "core install arduino:mega";
            //process.OutputDataReceived += Process_OutputDataReceived;
            //process.ErrorDataReceived += Process_OutputDataReceived;
            //process.StartInfo = start;
            //process.Start();
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            //process.WaitForExit();
            //process = new Process();
            //start.Arguments = "core install arduino:samd";
            //process.OutputDataReceived += Process_OutputDataReceived;
            //process.ErrorDataReceived += Process_OutputDataReceived;
            //process.StartInfo = start;
            //process.Start();
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            //process.WaitForExit();
            //process = new Process();
            //start.Arguments = "core install arduino:uno";
            //process.OutputDataReceived += Process_OutputDataReceived;
            //process.ErrorDataReceived += Process_OutputDataReceived;
            //process.StartInfo = start;
            //process.Start();
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            //process.WaitForExit();
            //process = new Process();
            //start.Arguments = "core install SparkFun:samd";
            //process.OutputDataReceived += Process_OutputDataReceived;
            //process.ErrorDataReceived += Process_OutputDataReceived; ;
            //process.StartInfo = start;
            //process.Start();
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            //process.WaitForExit();
            //mainWindowView.Busy = false;
            process = new Process();
            start.Arguments = "daemon";
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            process.StartInfo = start;
            process.Start();
            Task task = new Task(Init);
            task.Start();
            
        }


        string currentFile = "";
        long totalSize = 0;
        public async void Init()
        {
            channel = GrpcChannel.ForAddress("http://127.0.0.1:27160", new GrpcChannelOptions { Credentials = ChannelCredentials.Insecure });
            client = new ArduinoCore.ArduinoCoreClient(channel);
            AsyncServerStreamingCall<InitResp> init = client.Init(new InitReq());
            
            while (await init.ResponseStream.MoveNext())
            {
                InitResp resp = init.ResponseStream.Current;
                instance = resp.Instance;
            }

            AsyncServerStreamingCall<UpdateIndexResp> update = client.UpdateIndex(new UpdateIndexReq { Instance = instance });
            while (await update.ResponseStream.MoveNext())
            {
                UpdateIndexResp resp = update.ResponseStream.Current;
                if (resp.DownloadProgress != null)
                {
                    if (!String.IsNullOrEmpty(resp.DownloadProgress.File)) {
                        currentFile = resp.DownloadProgress.File;
                        totalSize = resp.DownloadProgress.TotalSize;
                    }
                    if (!resp.DownloadProgress.Completed)
                    {
                        mainWindowView.Status += $"Downloading {currentFile}: {resp.DownloadProgress.Downloaded}/{totalSize}";
                    }
                    else
                    {
                        mainWindowView.Status += $"{currentFile} download complete";
                    }
                } 
                
            }

            AsyncServerStreamingCall<PlatformInstallResp> avr = client.PlatformInstall(new PlatformInstallReq { Instance = instance, Architecture = "avr", PlatformPackage = "arduino" });
            while (await avr.ResponseStream.MoveNext())
            {
                SendProgress(avr.ResponseStream.Current);
            }

            AsyncServerStreamingCall<PlatformInstallResp> mega = client.PlatformInstall(new PlatformInstallReq { Instance = instance, Architecture = "megaavr", PlatformPackage = "arduino" });
            while (await mega.ResponseStream.MoveNext())
            {
                SendProgress(mega.ResponseStream.Current);
            }

            //AsyncServerStreamingCall<PlatformInstallResp> uno = client.PlatformInstall(new PlatformInstallReq { Instance = instance, Architecture = "uno", PlatformPackage = "arduino" });
            //while (await uno.ResponseStream.MoveNext())
            //{
            //    PlatformInstallResp resp = uno.ResponseStream.Current;
            //    if (resp.Progress != null)
            //    {
            //        mainWindowView.Status += $"Downloading {resp.Progress.File}: {resp.Progress.Downloaded}/{resp.Progress.TotalSize}";
            //    }
            //}

            AsyncServerStreamingCall<PlatformInstallResp> samd = client.PlatformInstall(new PlatformInstallReq { Instance = instance, Architecture = "samd", PlatformPackage = "arduino" });
            while (await samd.ResponseStream.MoveNext())
            {
                SendProgress(samd.ResponseStream.Current);
            }

            AsyncServerStreamingCall<PlatformInstallResp> sparkfun = client.PlatformInstall(new PlatformInstallReq { Instance = instance, Architecture = "samd", PlatformPackage = "SparkFun" });
            while (await sparkfun.ResponseStream.MoveNext())
            {
                SendProgress(sparkfun.ResponseStream.Current);
            }

            mainWindowView.Status += "Boards Installed and Arduino CLI initialized";
            mainWindowView.Progress = 0;
        }


        
        private void SendProgress(PlatformInstallResp resp)
        {
            if (resp.Progress != null)
            {
                if (!String.IsNullOrEmpty(resp.Progress.File))
                {
                    currentFile = resp.Progress.File;
                    totalSize = resp.Progress.TotalSize;
                }
                if (!resp.Progress.Completed)
                {
                    mainWindowView.Status += $"Downloading {currentFile}: {resp.Progress.Downloaded}/{totalSize}";
                    mainWindowView.Progress = (int)(resp.Progress.Downloaded / totalSize);
                } 
                else
                {
                    mainWindowView.Status += $"{currentFile} download complete";
                } 
            }
        }

        /// <summary>
        /// Attempt to copile Arudino Sketch
        /// </summary>
        /// <param name="fqbn">Fully qualified board name for Arduino Board</param>
        /// <param name="location">Location of ino/cpp file</param>
        public void ArduinoComplieSketch(string fqbn, string location, string port)
        {
            mainWindowView.Busy = true;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@"arduino-cli";
            start.Arguments = $"compile -v --fqbn {fqbn} ./{location} -u -p {port} -t";
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.CreateNoWindow = true;
            Process process = new Process();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            mainWindowView.Busy = false;
        }
        /// <summary>
        /// Attempt to compile and upload Arudino Sketch
        /// </summary>
        /// <param name="fqbn">Fully qualified board name for Arduino Board</param>
        /// <param name="location">Location of ino/cpp file</param>
        public void UploadSketch(string fqbn, string port, string file)
        {
            mainWindowView.Busy = true;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@"arduino-cli";
            start.Arguments = $@"upload --fqbn {fqbn} -p {port} --input-dir ./{file}/build -v -t";
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.CreateNoWindow = true;
            Process process = new Process();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            mainWindowView.Busy = false;
        }

        public async void DetectBoard()
        {
            
            mainWindowView.Busy = true;
            BoardListResp boards = client.BoardList(new BoardListReq { Instance = instance });
            foreach(DetectedPort port in boards.Ports)
            {
                if (port.Boards.Count > 0)
                {
                    mainWindowView.Status += $"Detected a {port.Boards[0].Name} on port {port.Address}";
                }
            }
            mainWindowView.Busy = false;
        }

        public void GetLibrary(string name)
        {
            mainWindowView.Busy = true;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@"arduino-cli";
            start.Arguments = $"lib install \"{name}\"";
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.CreateNoWindow = true;
            Process process = new Process();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            mainWindowView.Busy = false;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            mainWindowView.Status += e.Data;
        }
    }
}
