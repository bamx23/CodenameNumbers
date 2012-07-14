using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public delegate void NetErrorEventHandler(object sender, NetErrorEventArgs e);

    public class NetErrorEventArgs : EventArgs
    {
        public NetErrorEventArgs(string error)
        {
            Error = error;
        }

        public string Error { get; protected set; }
    }

    public enum CommandType
    {
        MAIN_LOGIN,
        GAMESESSION_LIST,
        GAMESESSION_JOIN,
        GAME_STATS,
        GAME_HIT,
        GAME_LEAVE
    };

    public class GameClient
    {
        public NetClient NetworkClient { get; set; }

        public event NetErrorEventHandler NetErrorEvent;

        public GameClient(string host)
        {
            NetworkClient = new NetClient(host, protocol: new GameProtocol(), defaultEncoding: Encoding.ASCII);
        }

        /// <summary>
        /// Stops NetworkClient if it working, and creates another one with new host
        /// </summary>
        /// <param name="host"></param>
        public void ResetHost(string host)
        {
            if(NetworkClient.Status == NetClientStatus.Working)
                NetworkClient.Stop();
            NetworkClient = new NetClient(host, NetworkClient.Port, new GameProtocol(), Encoding.ASCII);
        }

        public void Login(string username, string password)
        {
            var operands = new Dictionary<string, string>
                               {
                                   {"LOGIN", username},
                                   {"PASSWORD", password},
                                   {"TIME", DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture)}
                               };

            Send(CommandType.MAIN_LOGIN, operands);
        }

        public void GameSessionList()
        {
            Send(CommandType.GAMESESSION_LIST);
        }

        public void GameSessionJoin(int sessionId)
        {
            Send(CommandType.GAMESESSION_JOIN, "GAMESESSION_ID", sessionId.ToString(CultureInfo.InvariantCulture));
        }

        public void GameStats()
        {
            Send(CommandType.GAME_STATS);
        }

        public void GameHit(int number)
        {
            var operands = new Dictionary<string, string>
                               {
                                   {"NUMBER", number.ToString(CultureInfo.InvariantCulture)},
                                   {"TIMESTAMP", DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture)}
                               };

            Send(CommandType.MAIN_LOGIN, operands);
        }

        public void GameLeave()
        {
            Send(CommandType.GAME_LEAVE);
        }

        public void Send(CommandType command)
        {
            Send(command, null);
        }

        public void Send(CommandType command, string operand, string operandValue)
        {
            Send(command, new Dictionary<string, string> { { operand, operandValue } });
        }

        public void Send(CommandType command, Dictionary<string, string> operands)
        {
            if (operands == null)
                operands = new Dictionary<string, string>();

            Send(new Dictionary<string, string>(operands) { { "cmd", command.ToString() } });
        }

        public void Send(Dictionary<string, string> command)
        {
            var json = fastJSON.JSON.Instance.ToJSON(command);
            try
            {
                NetworkClient.Send(json);
            }
            catch (Exception e)
            {
                if (NetErrorEvent != null)
                    NetErrorEvent(this, new NetErrorEventArgs("Error occuried while message sending: " + e.Message));
            }
        }

        public void Start()
        {
            try
            {
                NetworkClient.Start();
            }
            catch (Exception e)
            {
                if (NetErrorEvent != null)
                    NetErrorEvent(this, new NetErrorEventArgs("Client start error occuried" + e.Message));
            }
        }

        public void Stop()
        {
            try
            {
                NetworkClient.Stop();
            }
            catch (Exception e)
            {
                if (NetErrorEvent != null)
                    NetErrorEvent(this, new NetErrorEventArgs("Client stop error occuried" + e.Message));
            }
        }
    }

    /// <summary>
    /// Our game protocol sends and receives one 4-byte integer before message, that indicates length of message
    /// </summary>
    public class GameProtocol : NetProtocol
    {
        public override void Send(byte[] message, BinaryWriter output)
        {
            output.Write(message.Length);
            output.Write(message);
        }

        public override byte[] Receive(NetworkStream stream, TcpClient tcpClient)
        {
            var intBuffer = new byte[4];
            // Read message length first 
            stream.Read(intBuffer, 0, 4);
            var messageLength = BitConverter.ToInt32(intBuffer, 0);
            var buffer = new byte[messageLength];

            stream.Read(buffer, 0, messageLength);

            return buffer;
        }

        public override string ToString()
        {
            return "GameProtocol";
        }
    }
}

