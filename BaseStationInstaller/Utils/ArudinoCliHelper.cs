using Avalonia.Threading;
using BaseStationInstaller.ViewModels;
using Cc.Arduino.Cli.Commands;
using DynamicData;
using Grpc.Core;
using Grpc.Net.Client;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            Process process = new Process();
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
            mainWindowView.Busy = true;
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
                    if (!String.IsNullOrEmpty(resp.DownloadProgress.File))
                    {
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
            mainWindowView.Busy = false;
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
        public async void ArduinoComplieSketch(string fqbn, string location)
        {
            AsyncServerStreamingCall<CompileResp> compile = client.Compile(new CompileReq { Instance = instance, Fqbn = fqbn, Verbose = true, SketchPath = $"./{location}" });
            try
            {
                int count = 0;
                while (await compile.ResponseStream.MoveNext())
                {
                    mainWindowView.Status += compile.ResponseStream.Current.OutStream.ToStringUtf8();
                    count++;
                    if (count % 3 == 0)
                    {
                        mainWindowView.Progress++;
                    }
                    if (compile.ResponseStream.Current.ErrStream.Length >= 1)
                    {
                        mainWindowView.Status += $"ERROR: {compile.ResponseStream.Current.ErrStream.ToStringUtf8()}";
                    }
                }
            }
            catch (RpcException e)
            {
                mainWindowView.Status += $"Failed to compile sketch in {location} got error {e.Status.Detail}";
            }
        }
        /// <summary>
        /// Attempt to compile and upload Arudino Sketch
        /// </summary>
        /// <param name="fqbn">Fully qualified board name for Arduino Board</param>
        /// <param name="location">Location of ino/cpp file</param>
        public async void UploadSketch(string fqbn, string port, string location)
        {
            mainWindowView.RefreshingPorts = true;
            AsyncServerStreamingCall<UploadResp> upload = client.Upload(new UploadReq { Instance = instance, Fqbn = fqbn, Port = port, SketchPath = location, Verbose = true, Verify = true });
            try
            {
                StringBuilder sb = new StringBuilder();
                Stopwatch time = new Stopwatch();
                time.Start();
                int count = 0;
                while (await upload.ResponseStream.MoveNext())
                {
                    mainWindowView.Status += $"Uploading elasped time {Math.Round(time.ElapsedMilliseconds * 0.001, 3).ToString("00.000")} seconds";
                    count++;
                    if (count % 10 == 0)
                    {
                        mainWindowView.Progress++;
                    }
                    sb.Append($"{upload.ResponseStream.Current.OutStream.ToStringUtf8()}");
                    sb.Append($"{upload.ResponseStream.Current.ErrStream.ToStringUtf8()}");
                }
                time.Stop();
                mainWindowView.Status += sb.ToString();
                mainWindowView.Status += $"Uploaded and Verified to {port} in {Math.Round(time.ElapsedMilliseconds * 0.001,3).ToString("00.000")} seconds";
                mainWindowView.Progress = 100;
                Thread.Sleep(2500);
                mainWindowView.Progress = 0;
                mainWindowView.Busy = false;
                mainWindowView.RefreshingPorts = false;
            }
            catch (RpcException e)
            {
                mainWindowView.Status += $"Failed to upload because {e.Status.Detail}";
                mainWindowView.Progress = 0;
                mainWindowView.Busy = false;
                mainWindowView.RefreshingPorts = false;
            }

        }

        /// <summary>
        /// Detect attached boards and select it.
        /// </summary>
        public async void DetectBoard()
        {

            mainWindowView.Busy = true;
            mainWindowView.RefreshingPorts = true;
            mainWindowView.AvailableComPorts = new ObservableCollection<string>();
            BoardListResp boards = client.BoardList(new BoardListReq { Instance = instance });
            foreach (DetectedPort port in boards.Ports)
            {
                if (port.Boards.Count > 0)
                {

                    mainWindowView.Status += $"Detected a {port.Boards[0].Name} on port {port.Address}";
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        mainWindowView.AvailableComPorts.Add(port.Address);
                    });
                    Thread.Sleep(1000);

                    if (String.IsNullOrEmpty(mainWindowView.SelectedComPort))
                    {
                        mainWindowView.SelectedBoard = mainWindowView.SelectedConfig.SupportedBoards.Find((b) => b.FQBN == port.Boards[0].FQBN);
                        mainWindowView.SelectedComPort = port.Address;
                    }
                    
                }
            }
            mainWindowView.Busy = false;
            mainWindowView.RefreshingPorts = false;
        }


        /// <summary>
        /// Install required libraries that do not have to be obtained via GIT
        /// </summary>
        /// <param name="name">Name of library to install</param>
        public async void GetLibrary(string name)
        {
            mainWindowView.Busy = true;
            AsyncServerStreamingCall<LibraryInstallResp> libInstall = client.LibraryInstall(new LibraryInstallReq { Instance = instance, Name = name });
            while (await libInstall.ResponseStream.MoveNext())
            {
                LibraryInstallResp resp = libInstall.ResponseStream.Current;
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
                    }
                    else
                    {
                        mainWindowView.Status += $"{currentFile} download complete";
                    }
                }
            }
            mainWindowView.Busy = false;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            mainWindowView.Status += e.Data;
        }
    }
}
