using Avalonia.Threading;
using Cc.Arduino.Cli.Commands.V1;
using DynamicData;
using DynamicData.Kernel;
using exInstaller.ViewModels;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace exInstaller.Utils
{
    public class ArudinoCliHelper
    {
        IMainWindowViewModel mainWindowView;
        GrpcChannel channel;
        ArduinoCoreService.ArduinoCoreServiceClient client;
        Instance instance;
        public bool daemonRunning = false;
        Process daemon;


        public ArudinoCliHelper(IMainWindowViewModel mWindow)
        {
            mainWindowView = mWindow;
            mainWindowView.Busy = true;
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    foreach (Process proc in Process.GetProcessesByName("arduino-cli"))
                    {
                        while (!proc.HasExited)
                        {
                            proc.Kill();
                        }
                    }
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    foreach (Process proc in Process.GetProcessesByName("arduino-cli.exe"))
                    {
                        while (!proc.HasExited)
                        {
                            proc.Kill();
                        }

                    }
                }
                ProcessStartInfo start = new ProcessStartInfo();
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                start.FileName = $@"arduino-cli";
                start.UseShellExecute = false;
                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.CreateNoWindow = true;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                daemon = new Process();
                start.Arguments = "daemon";
                start.RedirectStandardOutput = false;
                start.RedirectStandardError = false;
                daemon.StartInfo = start;
                daemon.Start();

            }
            catch (Exception ex)
            {
                mWindow.Status += $"Failed to start arduino cli due to {ex.Message}";
            }

        }

        bool ProcessIsRunning(Process p)
        {
            bool isRunning;
            try
            {
                isRunning = !p.HasExited && p.Threads.Count > 0;
            }
            catch (SystemException sEx)
            {
                isRunning = false;
            }

            return isRunning;
        }


        string currentFile = "";
        long totalSize = 0;
        public async Task<bool> Init()
        {
            if (ProcessIsRunning(daemon))
            {
                mainWindowView.Status += "Starting arduino cli please wait";

                mainWindowView.Busy = true;
                mainWindowView.RefreshingPorts = true;
                int count = 0;
                while (count < 10)
                {
                    mainWindowView.Status += ".";
                    Thread.Sleep(1000);
                    count++;
                }

                mainWindowView.Status += Environment.NewLine;

                AsyncServerStreamingCall<InitResponse> init;
                try
                {
                    channel = GrpcChannel.ForAddress("http://127.0.0.1:27160", new GrpcChannelOptions { Credentials = ChannelCredentials.Insecure });
                    client = new ArduinoCoreService.ArduinoCoreServiceClient(channel);
                    init = client.Init(new InitRequest());
                }
                catch (RpcException e)
                {
                    mainWindowView.Status += $"\r\n Failed to connect to gRPC due to {e.Message}";
                    return false;
                }


                while (await init.ResponseStream.MoveNext())
                {
                    InitResponse resp = init.ResponseStream.Current;
                    instance = resp.Instance;
                }
                if (instance != null)
                {
                    try
                    {
                        AsyncServerStreamingCall<UpdateIndexResponse> update = client.UpdateIndex(new UpdateIndexRequest { Instance = instance });
                        while (await update.ResponseStream.MoveNext())
                        {
                            UpdateIndexResponse resp = update.ResponseStream.Current;
                            if (resp.DownloadProgress != null)
                            {
                                if (!String.IsNullOrEmpty(resp.DownloadProgress.File))
                                {
                                    currentFile = resp.DownloadProgress.File;
                                    totalSize = resp.DownloadProgress.TotalSize;
                                }
                                if (!resp.DownloadProgress.Completed)
                                {
                                    mainWindowView.Status += $"Downloading {currentFile}: {resp.DownloadProgress.Downloaded}/{totalSize}{Environment.NewLine}";
                                }
                                else
                                {
                                    mainWindowView.Status += $"{currentFile} download complete{Environment.NewLine}";
                                }
                            }

                        }

                        AsyncServerStreamingCall<UpdateCoreLibrariesIndexResponse> coreLibUpdate = client.UpdateCoreLibrariesIndex(new UpdateCoreLibrariesIndexRequest { Instance = instance });
                        mainWindowView.Status += "Updating core libraries index";
                        while (await coreLibUpdate.ResponseStream.MoveNext())
                        {
                            UpdateCoreLibrariesIndexResponse resp = coreLibUpdate.ResponseStream.Current;
                            if (resp.DownloadProgress != null && !resp.DownloadProgress.Completed)
                            {
                                mainWindowView.Status += ".";
                            }
                        }
                        mainWindowView.Status += "Core Library indexes updated" + Environment.NewLine;
                        AsyncServerStreamingCall<UpdateLibrariesIndexResponse> liUpdate = client.UpdateLibrariesIndex(new UpdateLibrariesIndexRequest { Instance = instance });
                        mainWindowView.Status += "Updating libraries index";
                        while (await liUpdate.ResponseStream.MoveNext())
                        {
                            UpdateLibrariesIndexResponse resp = liUpdate.ResponseStream.Current;
                            if (resp.DownloadProgress != null && !resp.DownloadProgress.Completed)
                            {
                                mainWindowView.Status += ".";
                            }
                        }
                        mainWindowView.Status += "Libraries Indexes updated" + Environment.NewLine;


                        //AsyncServerStreamingCall<PlatformInstallResp> avr = client.PlatformInstall(new PlatformInstallReq { Instance = instance, Architecture = "avr", PlatformPackage = "arduino" });
                        //while (await avr.ResponseStream.MoveNext())
                        //{
                        //    SendProgress(avr.ResponseStream.Current);
                        //}

                        //AsyncServerStreamingCall<PlatformInstallResp> mega = client.PlatformInstall(new PlatformInstallReq { Instance = instance, Architecture = "megaavr", PlatformPackage = "arduino" });
                        //while (await mega.ResponseStream.MoveNext())
                        //{
                        //    SendProgress(mega.ResponseStream.Current);
                        //}

                        //AsyncServerStreamingCall<PlatformInstallResp> samd = client.PlatformInstall(new PlatformInstallReq { Instance = instance, Architecture = "samd", PlatformPackage = "arduino" });
                        //while (await samd.ResponseStream.MoveNext())
                        //{
                        //    SendProgress(samd.ResponseStream.Current);
                        //}

                        //AsyncServerStreamingCall<PlatformInstallResp> sparkfun = client.PlatformInstall(new PlatformInstallReq { Instance = instance, Architecture = "samd", PlatformPackage = "SparkFun" });
                        //while (await sparkfun.ResponseStream.MoveNext())
                        //{
                        //    SendProgress(sparkfun.ResponseStream.Current);
                        //}

                        mainWindowView.Status += $"Arduino CLI initialized{Environment.NewLine}";
                        mainWindowView.Progress = 0;
                        mainWindowView.Busy = false;
                        mainWindowView.RefreshingPorts = false;
                        return true;
                    }
                    catch (RpcException e)
                    {
                        mainWindowView.Status += $"Arduino CLI failed to initalize please restart Installer{Environment.NewLine}";
                        mainWindowView.Status += $"Error: {e.Message}{Environment.NewLine}";
                        mainWindowView.Progress = 0;
                        mainWindowView.Busy = true;
                        mainWindowView.RefreshingPorts = true;
                        return false;
                    }

                }
                else
                {
                    mainWindowView.Status += $"Arduino CLI failed to initalize please restart Installer{Environment.NewLine}";
                    mainWindowView.Progress = 0;
                    mainWindowView.Busy = true;
                    mainWindowView.RefreshingPorts = true;
                    return false;
                }

            }
            else
            {
                return false;
            }
        }



        private void SendProgress(PlatformInstallResponse resp)
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
                    mainWindowView.Status += $"Downloading {currentFile}: {resp.Progress.Downloaded}/{totalSize}{Environment.NewLine}";
                    mainWindowView.Progress = (int)(resp.Progress.Downloaded / totalSize);
                }
                else
                {
                    mainWindowView.Status += $"{currentFile} download complete{Environment.NewLine}";
                }
            }
        }

        /// <summary>
        /// Attempt to copile Arudino Sketch
        /// </summary>
        /// <param name="fqbn">Fully qualified board name for Arduino Board</param>
        /// <param name="location">Location of ino/cpp file</param>
        public async Task<bool> ArduinoComplieSketch(string fqbn, string location)
        {
            AsyncServerStreamingCall<CompileResponse> compile = client.Compile(new CompileRequest { Instance = instance, Fqbn = fqbn, Verbose = true, SketchPath = $"./{location}" });
            bool success = false;
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
                        mainWindowView.Status += $"ERROR: {compile.ResponseStream.Current.ErrStream.ToStringUtf8()}{Environment.NewLine}";
                    }
                }
                success = true;
            }
            catch (RpcException e)
            {
                mainWindowView.Status += $"Failed to compile sketch in {location} got error {e.Status.Detail}{Environment.NewLine}";
            }
            return success;
        }
        /// <summary>
        /// Attempt to compile and upload Arudino Sketch
        /// </summary>
        /// <param name="fqbn">Fully qualified board name for Arduino Board</param>
        /// <param name="location">Location of ino/cpp file</param>
        public async Task<bool> UploadSketch(string fqbn, string port, string location)
        {
            bool success = false;
            mainWindowView.RefreshingPorts = true;
            AsyncServerStreamingCall<UploadResponse> upload = client.Upload(new UploadRequest { Instance = instance, Fqbn = fqbn, Port = port, SketchPath = location, Verbose = true, Verify = true });
            try
            {
                StringBuilder sb = new StringBuilder();
                Stopwatch time = new Stopwatch();
                time.Start();
                int count = 0;
                while (await upload.ResponseStream.MoveNext())
                {
                    count++;
                    if (count % 10 == 0)
                    {
                        mainWindowView.Status += $"Uploading elasped time {Math.Round(time.ElapsedMilliseconds * 0.001, 3).ToString("00.000")} seconds{Environment.NewLine}";
                        mainWindowView.Progress++;
                    }
                    sb.Append($"{upload.ResponseStream.Current.OutStream.ToStringUtf8()}");
                    sb.Append($"{upload.ResponseStream.Current.ErrStream.ToStringUtf8()}");
                }
                time.Stop();
                mainWindowView.Status += sb.ToString();
                mainWindowView.Status += $"Uploaded and Verified to {port} in {Math.Round(time.ElapsedMilliseconds * 0.001, 3).ToString("00.000")} seconds{Environment.NewLine}";
                mainWindowView.Progress = 100;
                Thread.Sleep(2500);
                mainWindowView.Progress = 0;
                mainWindowView.Busy = false;
                mainWindowView.RefreshingPorts = false;
                success = true;
            }
            catch (RpcException e)
            {
                mainWindowView.Status += $"Failed to upload because {e.Status.Detail}{Environment.NewLine}";
                mainWindowView.Progress = 0;
                mainWindowView.Busy = false;
                mainWindowView.RefreshingPorts = false;
            }
            return success;
        }

        /// <summary>
        /// Detect attached boards and select it.
        /// </summary>
        public async void DetectBoard()
        {

            mainWindowView.Busy = true;
            mainWindowView.RefreshingPorts = true;
            mainWindowView.AvailableComPorts = new ObservableCollection<Tuple<string, string>>();
            BoardListResponse boards = client.BoardList(new BoardListRequest { Instance = instance });
            foreach (DetectedPort port in boards.Ports)
            {
                if (port.MatchingBoards.Count > 0)
                {

                    mainWindowView.Status += $"Detected a {port.MatchingBoards[0].Name} on port {port.Port}{Environment.NewLine}";
                    await Dispatcher.UIThread.InvokeAsync(() =>
                     {
                         mainWindowView.AvailableComPorts.Add(new Tuple<string, string>(port.Port, $"{port.MatchingBoards[0].Name}"));

                     });
                    Thread.Sleep(1000);

                    //if (String.IsNullOrEmpty(mainWindowView.SelectedComPort.Item1))
                    //{
                    //    mainWindowView.SelectedBoard = mainWindowView.SelectedConfig.SupportedBoards.Find((b) => b.FQBN == port.Boards[0].FQBN);
                    //}
                }
            }
            await Dispatcher.UIThread.InvokeAsync(() =>
                     {
                         foreach (string avail in SerialPort.GetPortNames())
                             if (!mainWindowView.AvailableComPorts.AsList().Exists(x => x.Item1 == avail))
                             {
                                 mainWindowView.AvailableComPorts.Add(new Tuple<string, string>(avail, $"Unknown Board"));
                             }
                     });
            mainWindowView.Busy = false;
            mainWindowView.RefreshingPorts = false;
        }


        /// <summary>
        /// Install required libraries that do not have to be obtained via GIT
        /// </summary>
        /// <param name="name">Name of library to install</param>
        public async Task<bool> GetLibrary(string name)
        {
            mainWindowView.Busy = true;
            try
            {
                AsyncServerStreamingCall<LibraryInstallResponse> libInstall = client.LibraryInstall(new LibraryInstallRequest { Instance = instance, Name = name });
                while (await libInstall.ResponseStream.MoveNext())
                {
                    LibraryInstallResponse resp = libInstall.ResponseStream.Current;
                    if (resp.Progress != null)
                    {
                        if (!String.IsNullOrEmpty(resp.Progress.File))
                        {
                            currentFile = resp.Progress.File;
                            totalSize = resp.Progress.TotalSize;
                        }
                        if (!resp.Progress.Completed)
                        {
                            mainWindowView.Status += $"Downloading {currentFile}: {resp.Progress.Downloaded}/{totalSize}{Environment.NewLine}";
                        }
                        else
                        {
                            mainWindowView.Status += $"{currentFile} download complete{Environment.NewLine}";
                        }
                    }
                }
                mainWindowView.Busy = false;
                return true;
            }
            catch (RpcException e)
            {
                mainWindowView.Busy = false;
                mainWindowView.Status += $"Failed to download library by name due to {e.Message} attempting to obtain via git{Environment.NewLine}";
                return false;
            }

        }

        public async Task<bool> GetBoard(string arch, string pkg)
        {
            mainWindowView.Busy = true;
            mainWindowView.Status = $"Attempting to download {arch}:{pkg}{Environment.NewLine}";
            try
            {
                AsyncServerStreamingCall<PlatformInstallResponse> platInstall = client.PlatformInstall(new PlatformInstallRequest { Instance = instance, Architecture = arch, PlatformPackage = pkg });
                while (await platInstall.ResponseStream.MoveNext())
                {
                    SendProgress(platInstall.ResponseStream.Current);
                }
                mainWindowView.Busy = false;
                return true;
            }
            catch (RpcException e)
            {
                mainWindowView.Status += $"Failed to download platform {arch}:{pkg}{Environment.NewLine}";
                mainWindowView.Busy = false;
                return false;
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            mainWindowView.Status += e.Data;
        }
    }
}
