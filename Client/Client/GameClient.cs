using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Client
{
    public enum CommandType
    {
        MAIN_LOGIN,
        GAMESESSION_LIST,
        GAMESESSION_JOIN,
        GAME_STATS,
        GAME_HIT,
        GAME_LEAVE
    };

    public enum ResponseType
    {
        MAIN_LOGIN,
        GAMESESSION_LIST,
        GAMESESSION_JOIN,
        GAME_STATS,
        GAME_HIT,
        GAME_OVER,
        GAME_PLAYER_LEAVED
    };

    public delegate void BoolResponseEventHandler(object sender, BoolEventArgs e);

    public class BoolEventArgs : EventArgs
    {
        public BoolEventArgs(bool ok, int errorCode = 0)
        {
            Ok = ok;
            ErrorCode = errorCode;
        }

        public bool Ok { get; protected set; }

        public int ErrorCode { get; protected set; }

        public string Error
        {
            get
            {
                switch (ErrorCode)
                {
                    case 0:
                        return "Everything is Ok";
                    default:
                        return "Some error occuried";
                }
            }
        }
    }

    public class GameClient
    {
        protected enum EventType
        {
            NetError,
            Login,
            GameSessionJoin
        }

        public NetClient Client { get; protected set; }

        public static GameClient Instance { get; protected set; }

        protected delegate void EventSync(EventType type, object e);

        protected EventSync eventSyncDelegate;

        public event NetErrorEventHandler NetErrorEvent;

        public event BoolResponseEventHandler LoginEvent;

        public event BoolResponseEventHandler GameSessionJoinEvent;

        static GameClient()
        {
            Instance = new GameClient("192.168.33.55");
            //Instance = new GameClient("luckygeck.dyndns-home.com");
        }

        public GameClient(string host)
        {
            Client = new NetClient(host, protocol: new GameProtocol(), defaultEncoding: Encoding.ASCII, fireResponseEventAsync: false);
            Client.ResponseEvent += OnResponse;
            eventSyncDelegate = EventSyncDelegate;
        }

        /// <summary>
        /// Stops NetworkClient if it working, and creates another one with new host
        /// </summary>
        /// <param name="host"></param>
        public void ResetHost(string host)
        {
            if(Client.Status == NetClientStatus.Working)
                Client.Stop();
            Client = new NetClient(host, Client.Port, new GameProtocol(), Encoding.ASCII, Client.SleepTimeout);
        }

        public void Login(string username, string password)
        {
            var operands = new Dictionary<string, object>
                               {
                                   {"LOGIN", username},
                                   {"PASSWORD", password},
                                   {"TIME", DateTime.UtcNow.Ticks}
                               };

            Send(CommandType.MAIN_LOGIN, operands);
        }

        public void GameSessionList()
        {
            Send(CommandType.GAMESESSION_LIST);
        }

        public void GameSessionJoin(int sessionId)
        {
            Send(CommandType.GAMESESSION_JOIN, "GAMESESSION_ID", sessionId);
        }

        public void GameStats()
        {
            Send(CommandType.GAME_STATS);
        }

        public void GameHit(int number)
        {
            var operands = new Dictionary<string, object>
                               {
                                   {"NUMBER", number},
                                   {"TIMESTAMP", DateTime.UtcNow.Ticks}
                               };

            Send(CommandType.GAME_HIT, operands);
        }

        public void GameLeave()
        {
            Send(CommandType.GAME_LEAVE);
        }

        public void Send(CommandType command)
        {
            Send(command, null);
        }

        public void Send(CommandType command, string operand, object operandValue)
        {
            Send(command, new Dictionary<string, object> {{operand, operandValue}});
        }

        public void Send(CommandType command, Dictionary<string, object> operands)
        {
            if (operands == null)
                operands = new Dictionary<string, object>();

            Send(new Dictionary<string, object>(operands) {{"cmd", command.ToString()}});
        }

        public void Send(Dictionary<string, object> command)
        {
            var json = fastJSON.JSON.Instance.ToJSON(command);
            try
            {
                Client.Send(json);
            }
            catch (Exception e)
            {
                NetErrorEvent(this, new NetErrorEventArgs("Error occuried while message sending: " + e.Message));
            }
        }

        public void Start()
        {
            try
            {
                Client.Start();
            }
            catch (Exception e)
            {
                SendEvent(EventType.NetError, new NetErrorEventArgs("Client start error occuried: " + e.Message));
            }
        }

        public void Stop()
        {
            try
            {
                Client.Stop();
            }
            catch (Exception e)
            {
                SendEvent(EventType.NetError, new NetErrorEventArgs("Client stop error occuried: " + e.Message));
            }
        }

        protected void OnResponse(object o, MessageEventArgs e)
        {
            var messagesArray = fastJSON.JSON.Instance.Parse(e.Message()) as ArrayList;

            if (messagesArray == null)
            {
                SendEvent(EventType.NetError, new NetErrorEventArgs("Can't parse server's response. " + e.Message()));
                return;
            }
            foreach (Dictionary<string, object> dictionary in messagesArray)
            {
                if (!dictionary.ContainsKey("cmd"))
                {
                    SendEvent(EventType.NetError, new NetErrorEventArgs(
                                      "Server's response doesn't contain \"cmd\" key, it can't be parsed. " + e.Message()));
                    return;
                }
                try
                {
                    ParseResponse(dictionary, e.Message());
                }
                catch (Exception exception)
                {
                    SendEvent(EventType.NetError, new NetErrorEventArgs("Some error occuried, while parsing:" + exception.Message));
                }
            }

        }

        protected void ParseResponse(Dictionary<string, object> command, string message)
        {
            var cmd = (string)command["cmd"];

            if (cmd == ResponseType.GAMESESSION_JOIN.ToString())
            {
                var ok = (bool)command["OK"];
                var errorCode = ok ? 0 : int.Parse(command["ERROR_CODE"].ToString());

                SendEvent(EventType.GameSessionJoin, new BoolEventArgs(ok, errorCode));
            }
            else if (cmd == ResponseType.GAMESESSION_LIST.ToString())
            {

            }
            else if (cmd == ResponseType.GAME_HIT.ToString())
            {

            }
            else if (cmd == ResponseType.GAME_OVER.ToString())
            {

            }
            else if (cmd == ResponseType.GAME_PLAYER_LEAVED.ToString())
            {

            }
            else if (cmd == ResponseType.GAME_STATS.ToString())
            {

            }
            else if (cmd == ResponseType.MAIN_LOGIN.ToString())
            {
                var ok = (bool) command["OK"];
                var errorCode = ok ? 0 : int.Parse(command["ERROR_CODE"].ToString());

                SendEvent(EventType.Login, new BoolEventArgs(ok, (int)errorCode));
            }
            else 
                SendEvent(EventType.NetError, new NetErrorEventArgs("Unknow response command found: " + message));
        }

        protected void EventSyncDelegate(EventType type, object e)
        {
            switch (type)
            {
                case EventType.NetError:
                    if (NetErrorEvent != null)
                        NetErrorEvent(this, e as NetErrorEventArgs);
                    break;
                case EventType.Login:
                    if (LoginEvent != null)
                        LoginEvent(this, e as BoolEventArgs);
                    break;
                case EventType.GameSessionJoin:
                    if (GameSessionJoinEvent != null)
                        GameSessionJoinEvent(this, e as BoolEventArgs);
                    break;
            }
        }

        protected void SendEvent(EventType type, object e)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, eventSyncDelegate, type, e);
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

