using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace BotServer
{
    public class ServoController
    {
        private SerialPort Port;
        // identifier of a Pololu Maestro
        private byte MaestroId = 12;

        // Maestro commands
        private enum Commands : byte
        {
            SetPosition = 0x04,
            GetPosition = 0x10
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
            byte Channel,
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
            Command[3] = (byte)(Channel & 0x7F);
            Command[4] = (byte)(Pos & 0x7F);
            Command[5] = (byte)((Pos >> 7) & 0x7F);

            Port.Write(Command, 0, 6);
        }

        /// <summary>
        /// Gets the position of a servo
        /// </summary>
        /// <param name="Channel">Servo number 0 - 5</param>
        /// <returns>Current position in microseconds</returns>
        public int GetPosition
            (
            byte Channel
            )
        {
            if (Port == null) throw new Exception("Not connected to servo controller");

            byte[] Command = new byte[4];
            Command[0] = 0xAA;
            Command[1] = MaestroId;
            Command[2] = (byte)Commands.GetPosition;
            Command[3] = (byte)(Channel & 0x7F);

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
            byte Channel,
            int Position,
            int Timeout
            )
        {
            DateTime Start = DateTime.Now;

            SetPosition(Channel, Position);
            while (GetPosition(Channel) != Position)
            {
                if (DateTime.Now.Subtract(Start).Ticks / 10000 > Timeout)
                {
                    throw new Exception(String.Format("Servo {0} failed to reach position {1} in {2} ms - now at {3}", Channel, Position, Timeout, GetPosition(Channel)));
                }
            }
        }

        /// <summary>
        /// Turns a servo off
        /// </summary>
        /// <param name="Channel">Servo number 0 - 5</param>
        public void Off
            (
            byte Channel
            )
        {
            SetPosition(Channel, 0);
        }
    }
}
