using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Client & Server Sides
/// </summary>
public class NetWork : NetworkBehaviour {

	//	Параметры сервера
	const int NETWORK_PORT = 7777; 		// сетевой порт

	const int MAX_CONNECTIONS = 4; 		// наивысшее число входящих подключений
	const bool USE_NAT = false; 		// применять NAT?
	
	private NetworkClient _nc;
	
	//	СОБЫТИЯ ID
	private const short POINT_TO_ID = 1001;				//	Указана точка
	private const short SCORES_CHANGED_ID = 1101;		//	Изменены очки


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//	СЕРВЕРНАЯ СТОРОНА
	void ServerStart()
	{
		Debug.Log ("Server Start function run ...");
		Debug.Log ("active = " + NetworkServer.active.ToString());
		Debug.Log ("NetworkServer.Listen() function run ...");
		NetworkServer.Listen (NETWORK_PORT);
		Debug.Log ("NetworkServer.DisconnectAll() function run ...");
		NetworkServer.DisconnectAll ();
		Debug.Log ("active = " + NetworkServer.active.ToString());
		Debug.Log ("channels = " + NetworkServer.numChannels.ToString());
		Debug.Log ("localClientActive = " + NetworkServer.localClientActive.ToString());
		InvokeRepeating ("ServerState", 5f, 5f);

		//	Регистрация событий
		NetworkServer.RegisterHandler (MsgType.Connect, this.SR_Connect);
		NetworkServer.RegisterHandler (POINT_TO_ID, this.SR_PointToID);	//	Событие 1001
	}

	void SR_Connect(NetworkMessage nmsg)
	{
		Debug.Log ("Client connected ...");
	}

	void SR_PointToID(NetworkMessage nmsg)
	{
		Debug.Log ("Server Receive ...");
	}

	void SR_Sync(NetworkMessage nmsg)
	{

	}

	public void SR_Send()
	{

	}

	public void CL_Send()
	{

	}

	//	КЛИЕНТСКАЯ СТОРОНА
	void ClientStart()
	{
		_nc = new NetworkClient ();
		_nc.RegisterHandler (POINT_TO_ID, this.CR_Sync);
		_nc.Connect ("127.0.0.1",NETWORK_PORT);
	}

	void ClientClose()
	{

	}

	void CR_Sync(NetworkMessage nmsg)
	{
		Debug.Log ("Client Receive");
	}
}


//	Передвижение
public class PointedTo : MessageBase
{
	public Vector2 PointXY;			//	Передача X,Y координат ткущей задачи стрельба или передвижения

	public int ScoreLP, ScoreRP;		//	Синхронизация очков
	public enumGameState GameState;		//	Синхронизация состояния игры
	public Transform PosLP, PosRP;		//	Синхронизация конечного положения объектов

	public PointedTo()
	{
		
	}
	
	public PointedTo (Vector2 pointXY)
	{
		PointXY = pointXY;
	}
}
