using BaseStationInstaller.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace BaseStationInstaller.Utils
{
    public class ArudinoCliHelper
    {
        IMainWindowViewModel mainWindowView;
        public ArudinoCliHelper(IMainWindowViewModel mWindow)
        {
            mainWindowView = mWindow;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@"arduino-cli.exe";
            start.Arguments = "core update-index";
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            Process process = new Process();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process = new Process();
            start.Arguments = "core install arduino:avr";
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process = new Process();
            start.Arguments = "core install arduino:mega";
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process = new Process();
            start.Arguments = "core install arduino:samd";
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process = new Process();
            start.Arguments = "core install arduino:uno";
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process = new Process();
            start.Arguments = "core install SparkFun:samd";
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        /// <summary>
        /// Attempt to copile Arudino Sketch
        /// </summary>
        /// <param name="fqbn">Fully qualified board name for Arduino Board</param>
        /// <param name="location">Location of ino/cpp file</param>
        public void ArduinoComplieSketch(string fqbn, string location)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@".\arduino-cli.exe";
            start.Arguments = $"compile --fqbn {fqbn} ./{location} -o basestation.hex";
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            Process process = new Process();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        /// <summary>
        /// Attempt to compile and upload Arudino Sketch
        /// </summary>
        /// <param name="fqbn">Fully qualified board name for Arduino Board</param>
        /// <param name="location">Location of ino/cpp file</param>
        public void UploadSketch(string fqbn, string port)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@".\arduino-cli.exe";
            start.Arguments = $@"upload --fqbn {fqbn} -p {port} -i basestation.hex -v -t";
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            Process process = new Process();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        public void DetectBoard()
        {
            //arduino-cli board list
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@".\arduino-cli.exe";
            start.Arguments = "board list";
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            Process process = new Process();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        public void GetLibrary(string name)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = $@".\arduino-cli.exe";
            start.Arguments = $"lib install \"{name}\"";
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            Process process = new Process();
            process.OutputDataReceived += Process_OutputDataReceived;
            process.StartInfo = start;
            process.Start();
            mainWindowView.Status += process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            mainWindowView.Status += e.Data;
        }
    }
}
