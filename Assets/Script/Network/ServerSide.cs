using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ServerSide : NetworkBehaviour {

	//	Параметры сервера
	const int NETWORK_PORT = 7777; 		// сетевой порт
	const int MAX_CONNECTIONS = 4; 		// наивысшее число входящих подключений
	const bool USE_NAT = false; 		// применять NAT?

	private NetworkClient _nc;

	//	СОБЫТИЯ ID
	private const short POINT_TO_ID = 1001;				//	Указана точка
	private const short SCORES_CHANGED_ID = 1101;		//	Изменены очки


	[SyncVar]
	public string svVar;

	static public string remoteServer = "127.0.0.1"; // адрес сервера (также дозволено localhost)

	private int playerCount = 0; // хранит число подключенных игроков
	public int PlayersCount { get { return playerCount; } } // публичный доступ для внешних компонентов касательно числа игроков на сервере

	//	Создание сообщение
	void AddMessage(string msg)
	{
		Text _txt = GameObject.Find ("ScrollViewServer").GetComponentInChildren<Text> ();
		_txt.text = _txt.text + "\n" + msg;
	}

	public void ClearMessages()
	{
		Text _txt = GameObject.Find ("ScrollViewServer").GetComponentInChildren<Text> ();
		_txt.text = "";
	}

	void OnEnable()
	{
		Application.runInBackground = true;
		svVar = "ServSide";
	}

	public void StartServer()
	{
		NetworkConnection _nc = new NetworkConnection ();
		_nc.address = "127.0.0.1";

		AddMessage ("Server Start function run ...");
		AddMessage ("active = " + NetworkServer.active.ToString());
		AddMessage ("NetworkServer.Listen() function run ...");
		NetworkServer.Listen (NETWORK_PORT);
		AddMessage ("NetworkServer.DisconnectAll() function run ...");
		NetworkServer.DisconnectAll ();
		AddMessage ("active = " + NetworkServer.active.ToString());
		AddMessage ("channels = " + NetworkServer.numChannels.ToString());
		AddMessage ("localClientActive = " + NetworkServer.localClientActive.ToString());
		InvokeRepeating ("ServerState", 5f, 5f);
		NetworkServer.RegisterHandler (MsgType.AddPlayer, this.OnPlayerConn);
		NetworkServer.RegisterHandler (MsgType.RemovePlayer, this.OnPlayerDisconn);
		NetworkServer.RegisterHandler (MsgType.Connect, this.OnConnectedSrv);
		NetworkServer.RegisterHandler (POINT_TO_ID, this.OnPointReceiveServer);	//	Событие 1001

	}

	void ServerState()
	{
		AddMessage ("--ServerState--");
		AddMessage ("active = " + NetworkServer.active.ToString());
		AddMessage ("localClientActive = " + NetworkServer.localClientActive.ToString());
		AddMessage ("connections = " + NetworkServer.connections.Count.ToString());
	}

	void ClientState()
	{
		AddMessage ("--ClientState--");
		AddMessage ("serverIP = " + _nc.serverIp.ToString());
		AddMessage ("serverPort = " + _nc.serverPort.ToString());
		AddMessage ("isConnected = " + _nc.isConnected.ToString());
	}

	public void JoinServer()
	{
		if (_nc != null && _nc.isConnected) {AddMessage ("Already connected.");return;}	//	Уже соединено

		AddMessage ("Joining Server");
		_nc = new NetworkClient ();
		_nc.RegisterHandler (MsgType.Connect, OnConnected);
		_nc.RegisterHandler (POINT_TO_ID, this.OnPointReceiveClient);
		_nc.Connect ("127.0.0.1",NETWORK_PORT);
		ClientState ();
	}

	public void OnConnected(NetworkMessage netMsg)
	{
		AddMessage ("On Connected callback " + netMsg.ToString());
	}

	public void OnConnectedSrv(NetworkMessage netMsg)
	{
		AddMessage ("On Connected callback Server " + netMsg.ToString());
	}

	public void SendMsg()
	{
		_nc.Send (POINT_TO_ID, new PointedTo(new Vector2(1.52f,1.42f)));
	}

	public void OnPointReceiveServer(NetworkMessage netMsg)
	{
	 	PointedTo xx = netMsg.ReadMessage<PointedTo> ();
		AddMessage("Recieve msg Server, Send To Client;" + xx.PointXY.x.ToString() + "/" + xx.PointXY.y.ToString());
		NetworkServer.SendToAll (POINT_TO_ID, xx);
	}

	public void OnPointReceiveClient(NetworkMessage netMsg)
	{
		PointedTo xx = netMsg.ReadMessage<PointedTo> ();
		AddMessage("Recieve msg Client;" + xx.PointXY.x.ToString() + "/" + xx.PointXY.y.ToString());
	}

	void OnPlayerConn( NetworkMessage netMsg ) {
		playerCount++; // при подключении всякого нового игрока увеличиваем число подключенных игроков
		AddMessage ("player connected " + netMsg);
	}
	
	void OnPlayerDisconn( NetworkMessage netMsg ) {
		--playerCount; // сокращаем число игроков

		AddMessage ("player disconnected" + netMsg);
	}
}

