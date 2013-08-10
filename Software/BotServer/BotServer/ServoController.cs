using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace BotServer
{
    /// <summary>
    /// Servo channel assignments
    /// </summary>
    public enum ServoChannels : byte
    {
        Pivot = 0,
        YAxis = 1,
        Toolhead = 2,
        RaisedSwitch = 3,
        DiscSwitch = 4
    }

    public class ServoController
    {
        private SerialPort Port;
        // identifier of a Pololu Maestro
        private byte MaestroId = 12;

        // Maestro commands
        private enum Commands : byte
        {
            SetPosition = 0x04,
            SetSpeed = 0x07,
            SetAcceleration = 0x09,
            GetPosition = 0x10,
            ServosMoving = 0x13
        }

        public ServoController
            (
            )
        {
        }

        /// <summary>
        /// Connects to the servo controller
        /// </summary>
        /// <param name="PortName">Name of the port to connect to</param>
        public void Connect
            (
            string PortName
            )
        {
            if (Port != null) return;

            Port = new SerialPort(PortName, 9600);
            Port.Open();
        }

        /// <summary>
        /// Disconnects from the servo controller
        /// </summary>
        public void Disconnect
            (
            )
        {
            if (Port != null)
            {
                Port.Close();
                Port = null;
            }
        }

        /// <summary>
        /// Sets the position of a servo
        /// Does not wait for the servo to reach the position - returns immediately
        /// </summary>
        /// <param name="Channel">Servo number 0 - 5</param>
        /// <param name="Position">Position in microseconds</param>
        public void SetPosition
            (
            ServoChannels Channel,
            int Position
            )
        {
            if (Port == null) throw new Exception("Not connected to servo controller");

            // convert to quarter-microseconds
            UInt16 Pos = (UInt16)(Position * 4);

            byte[] Command = new byte[6];
            Command[0] = 0xAA;
            Command[1] = MaestroId;
            Command[2] = (byte)Commands.SetPosition;
            Command[3] = (byte)((byte)Channel & 0x7F);
            Command[4] = (byte)(Pos & 0x7F);
            Command[5] = (byte)((Pos >> 7) & 0x7F);

            Port.Write(Command, 0, 6);
        }

        /// <summary>
        /// Sets the speed of a servo
        /// </summary>
        /// <param name="Channel">Servo number 0 - 5</param>
        /// <param name="Speed">New speed, 0 = unlimited</param>
        public void SetSpeed
            (
            ServoChannels Channel,
            int Speed
            )
        {
            if (Port == null) throw new Exception("Not connected to servo controller");

            byte[] Command = new byte[6];
            Command[0] = 0xAA;
            Command[1] = MaestroId;
            Command[2] = (byte)Commands.SetSpeed;
            Command[3] = (byte)((byte)Channel & 0x7F);
            Command[4] = (byte)(Speed & 0x7F);
            Command[5] = (byte)((Speed >> 7) & 0x7F);

            Port.Write(Command, 0, 6);
        }

        /// <summary>
        /// Sets the acceleration of a servo
        /// </summary>
        /// <param name="Channel">Servo number 0 - 5</param>
        /// <param name="Speed">New acceleration, 0 = unlimited</param>
        public void SetAcceleration
            (
            ServoChannels Channel,
            int Acceleration
            )
        {
            if (Port == null) throw new Exception("Not connected to servo controller");

            byte[] Command = new byte[6];
            Command[0] = 0xAA;
            Command[1] = MaestroId;
            Command[2] = (byte)Commands.SetAcceleration;
            Command[3] = (byte)((byte)Channel & 0x7F);
            Command[4] = (byte)(Acceleration & 0x7F);
            Command[5] = (byte)((Acceleration >> 7) & 0x7F);

            Port.Write(Command, 0, 6);
        }

        /// <summary>
        /// Gets the position of a servo
        /// WARNING: not valid while servo is still moving
        /// </summary>
        /// <param name="Channel">Servo number 0 - 5</param>
        /// <returns>Current position in microseconds</returns>
        public int GetPosition
            (
            ServoChannels Channel
            )
        {
            if (Port == null) throw new Exception("Not connected to servo controller");

            byte[] Command = new byte[4];
            Command[0] = 0xAA;
            Command[1] = MaestroId;
            Command[2] = (byte)Commands.GetPosition;
            Command[3] = (byte)((byte)Channel & 0x7F);

            Port.Write(Command, 0, 4);

            byte[] Response = new byte[2];
            Port.Read(Response, 0, 2);

            UInt16 Position = (UInt16)(((UInt16)Response[1] << 8) | Response[0]);

            return Position / 4;
        }

        /// <summary>
        /// Sets the servo to a specific position and waits for the servo to
        /// reach the position
        /// </summary>
        /// <param name="Channel">Servo number 0 - 5</param>
        /// <param name="Position">Position in microseconds</param>
        /// <param name="Timeout">Maximum time to wait in milliseconds</param>
        public void SetPositionandWait
            (
            ServoChannels Channel,
            int Position,
            int Timeout
            )
        {
            Stopwatch Watch = new Stopwatch();

            SetPosition(Channel, Position);

            // wait for motion to start
            Thread.Sleep(100);

            Watch.Start();
            while (ServosMoving())
            {
                if (Watch.ElapsedMilliseconds > Timeout)
                {
                    throw new Exception(String.Format("Servo {0} failed to reach position {1} in {2} ms - now at {3}", Channel, Position, Timeout, GetPosition(Channel)));
                }
            }
        }

        /// <summary>
        /// Checks if any servos are moving
        /// </summary>
        /// <returns>true if any servo is moving</returns>
        public bool ServosMoving
            (
            )
        {
            if (Port == null) throw new Exception("Not connected to servo controller");

            byte[] Command = new byte[3];
            Command[0] = 0xAA;
            Command[1] = MaestroId;
            Command[2] = (byte)Commands.ServosMoving;

            Port.Write(Command, 0, 3);

            byte[] Response = new byte[1];
            Port.Read(Response, 0, 1);

            if (Response[0] == 0x01) return true;
            return false;
        }

        /// <summary>
        /// Checks if a switch is pressed
        /// </summary>
        /// <param name="Channel">Channel that switch is on</param>
        /// <returns>true if switch is pressed</returns>
        public bool SwitchPressed
            (
            ServoChannels Channel
            )
        {
            if (GetPosition(Channel) < 10) return true;
            return false;
        }

        /// <summary>
        /// Turns a servo off
        /// </summary>
        /// <param name="Channel">Servo number 0 - 5</param>
        public void Off
            (
            ServoChannels Channel
            )
        {
            SetPosition(Channel, 0);
        }
    }
}
