using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace JacktheRipperBot
{
    public partial class MainForm : Form
    {
        private bool Busy = false;
        private Drive Drive = new Drive();
        private Ripper Ripper = new Ripper();
        private Bot Bot = new Bot();
        private bool StopRipping;
        private DateTime StartTime;

        // maximum number of times to attempt to close the tray and read a disc
        private int MaxCloseTrayAttempts = 3;

        private enum Commands { OpenTray, CloseTray, LoadDisc, UnloadDisc, Rip };

        public MainForm()
        {
            InitializeComponent();

            UpdateUI();

            ProgressText.Text = "";
            ProgressTime.Text = "";

            CommandSelector.SelectedIndex = 0;

            // show initial settings
            NumberofDVDsInput.Text = Properties.Settings.Default.NumberofDVDs.ToString();
            RipCommandInput.Text = Properties.Settings.Default.RipCommand;
            IPAddressInput.Text = Properties.Settings.Default.IPAddress;

            BuildDriveList();
        }

        /// <summary>
        /// Constructs a list of available DVD drives
        /// </summary>
        private void BuildDriveList
            (
            )
        {
            DriveSelector.Items.Clear();
            
            foreach (DriveInfo Drive in DriveInfo.GetDrives())
            {
                if (Drive.DriveType == DriveType.CDRom)
                {
                    DriveSelector.Items.Add(Drive);
                }
            }

            // select first item
            if (DriveSelector.Items.Count > 0) DriveSelector.SelectedIndex = 0;
        }

        /// <summary>
        /// Starts ripping DVDs
        /// </summary>
        public void Start
            (
            )
        {
            if (Busy) return;

            Busy = true;
            UpdateUI();

            // start ripping thread
            StopRipping = false;
            StartTime = DateTime.Now;
            Thread RipThread = new Thread(new ThreadStart(Rip));
            RipThread.Name = "Rip";
            RipThread.Start();
        }

        /// <summary>
        /// Rips the DVDs
        /// Executes as a worker thread
        /// </summary>
        private void Rip
            (
            )
        {
            try
            {
                int NumberofDVDs = int.Parse(NumberofDVDsInput.Text);

                ExecuteCommand(Commands.OpenTray);

                // process each DVD
                for (int CurrentDVD = 0; CurrentDVD < NumberofDVDs; CurrentDVD++)
                {
                    if (StopRipping) break;

                    ExecuteCommand(Commands.LoadDisc);

                    // keep opening and closing tray until disc is successfully read or
                    // we have reached the maximum number of attempts
                    int Attempt = 0;
                    while (true)
                    {
                        if (StopRipping) break;

                        if (!ExecuteCommand(Commands.CloseTray))
                        {
                            ExecuteCommand(Commands.OpenTray);
                            if (++Attempt == MaxCloseTrayAttempts)
                            {
                                StopRipping = true;
                                break;
                            }
                            // wait a bit
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (StopRipping) break;

                    ExecuteCommand(Commands.Rip);

                    if (StopRipping) break;

                    ExecuteCommand(Commands.OpenTray);

                    if (StopRipping) break;

                    ExecuteCommand(Commands.UnloadDisc);

                    if (StopRipping) break;

                    UpdateProgress(CurrentDVD + 1, NumberofDVDs);
                }

                ExecuteCommand(Commands.CloseTray);
            }
            catch (Exception Exc)
            {
                MessageBox.Show(Exc.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                Busy = false;
                UpdateUI();
            }
        }

        /// <summary>
        /// Stops ripping DVDs
        /// </summary>
        public void Stop
            (
            )
        {
            if (!Busy) return;

            // terminate thread
            StopRipping = true;

            Busy = false;
            UpdateUI();
        }

        /// <summary>
        /// Updates the user interface depending on the current operation
        /// </summary>
        private void UpdateUI
            (
            )
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateUI));
                return;
            }

            StartButton.Enabled = !Busy;
            NumberofDVDsInput.Enabled = !Busy;
            DriveSelector.Enabled = !Busy;
            RipCommandInput.Enabled = !Busy;
            CommandSelector.Enabled = !Busy;
            ExecuteCommandButton.Enabled = !Busy;
            IPAddressInput.Enabled = !Busy;

            StopButton.Enabled = Busy;
        }

        /// <summary>
        /// Called when user clicks on the start button
        /// Starts DVD ripping
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, EventArgs e)
        {
            Start();
        }

        /// <summary>
        /// Called when user clicks on the stop button
        /// Stops the DVD ripping
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, EventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Updates the progress display
        /// </summary>
        /// <param name="CurrentDVD">Number of current DVD being ripped</param>
        /// <param name="TotalDVDs">Total number of DVDs to rip</param>
        private void UpdateProgress
            (
            int CurrentDVD,
            int TotalDVDs
            )
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int, int>(UpdateProgress), CurrentDVD, TotalDVDs);
                return;
            }

            double Percentage = ((double)CurrentDVD / (double)TotalDVDs) * 100.0;
            ProgressBar.Value = (int)Percentage;

            ProgressText.Text = String.Format("{0:0.00}%", Percentage);

            // time since start
            TimeSpan RunTime = DateTime.Now.Subtract(StartTime);
            // get estimated total time
            TimeSpan TotalTime = TimeSpan.FromTicks((long)(RunTime.Ticks / (Percentage / 100.0)));
            // time to go
            TimeSpan RemainingTime = TotalTime.Subtract(RunTime);
            // time of completion
            DateTime ETA = DateTime.Now.Add(RemainingTime);

            ProgressTime.Text = String.Format("Elapsed: {0} Remaining: {1} ETA: {2}", RunTime.ToString(@"dd\.hh\:mm\:ss"), RemainingTime.ToString(@"dd\.hh\:mm\:ss"), ETA.ToString("d-MMM-yyy h:mm:ss tt"));
        }

        /// <summary>
        /// Called when form is closing
        /// Saves settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // save settings
            try
            {
                Properties.Settings.Default.NumberofDVDs = int.Parse(NumberofDVDsInput.Text);
                Properties.Settings.Default.RipCommand = RipCommandInput.Text;
                Properties.Settings.Default.IPAddress = IPAddressInput.Text;
            }
            catch (FormatException)
            {
            }
        }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="Command">Command to execute</param>
        /// <returns>true for success, false for failure</returns>
        private bool ExecuteCommand
            (
            Commands Command
            )
        {
            if (InvokeRequired)
            {
                return (bool)Invoke(new Func<Commands, bool>(ExecuteCommand), Command);
            }

            switch (Command)
            {
                case Commands.OpenTray:
                    Drive.OpenTray(((DriveInfo)DriveSelector.SelectedItem));
                    break;

                case Commands.CloseTray:
                    return Drive.CloseTray(((DriveInfo)DriveSelector.SelectedItem));

                case Commands.LoadDisc:
                    Bot.LoadDisc(IPAddressInput.Text);
                    break;

                case Commands.UnloadDisc:
                    Bot.UnloadDisc(IPAddressInput.Text);
                    break;

                case Commands.Rip:
                    string Name;
                    try
                    {
                        Name = ((DriveInfo)DriveSelector.SelectedItem).VolumeLabel;
                    }
                    catch (IOException)
                    {
                        Name = "Unknown";
                    }

                    Text = Application.ProductName + " - " + Name;

                    Ripper.Rip(RipCommandInput.Text, ((DriveInfo)DriveSelector.SelectedItem).Name, Name);

                    Text = Application.ProductName;

                    break;
            }

            return true;
        }

        /// <summary>
        /// Called when user clicks on the execute command button
        /// Immediately executes the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteCommandButton_Click(object sender, EventArgs e)
        {
            switch ((string)CommandSelector.SelectedItem)
            {
                case "Open Tray":
                    ExecuteCommand(Commands.OpenTray);
                    break;

                case "Close Tray":
                    ExecuteCommand(Commands.CloseTray);
                    break;

                case "Load Disc":
                    ExecuteCommand(Commands.LoadDisc);
                    break;

                case "Unload Disc":
                    ExecuteCommand(Commands.UnloadDisc);
                    break;

                case "Rip":
                    ExecuteCommand(Commands.Rip);
                    break;
            }
        }

        /// <summary>
        /// Called when form is about to close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // stop ripping
            Stop();
        }
    }
}
