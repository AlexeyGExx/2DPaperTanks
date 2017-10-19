using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Gexx2DPaperTanks;

public class GameBoardController : MonoBehaviour {

	public static enumGameState GameState;
	public static Vector3 mousePos;
	public Vector3 pos2D;
	public GameObject obj;
	public static Vector3 BorderPosition;
	public static Vector3 ActivePos;

	private int _score1, _score2;				//	Очки
	public static TankController _actTC;		//	Контроллер активного танка
	private MessageControl _tm;					//	Ссылка на скрипт сообщений
	private CursorController _curC;				//	Контроллер курсора
    
	private GameObject _ply1;
    private GameObject _ply2;

	public static Rect LeftPlayerGameField;		//	Ограничения для левого игрока
	public static Rect RightPlayerGameField;	//	Ограничения от правого игрока

	public static bool AllowUserActivity;					//	Разрешено действие пользователя

	// Use this for initialization
	void Start () 
	{	
		GameState = enumGameState.BeginGame;
		_score1 = 0;
		_score2 = 0;

		this.ChangeBorderPlace();

		_tm = GameObject.Find ("txtMessage").GetComponent<MessageControl> ();
		_tm.RaiseMessage ("Start !", 2, Color.magenta);

		_curC = GameObject.Find ("FireShoot").GetComponent<CursorController> ();
        _ply1 = GameObject.Find("meshPlayer1");
        _ply2 = GameObject.Find("meshPlayer2");

		_ply1.GetComponent<TankController> ().UnderAIControl = GameStatic.LeftPlayer.isAI;
		_ply2.GetComponent<TankController> ().UnderAIControl = GameStatic.RightPlayer.isAI;

		_actTC = _ply1.GetComponent<TankController> ();

		_ply1.GetComponent<TankController> ().player = GameStatic.LeftPlayer;
		_ply2.GetComponent<TankController> ().player = GameStatic.RightPlayer;

		AllowUserActivity = true;
		RightPlayerGameField = new Rect (3.5f, -10.5f, 19.5f, 22.5f);	//	Ограничения по полю передвижения танков

		UpdatePlayers ();	//Обновляем имена игроков
    }

	void OnEnable()
	{
		TankController.onHit += this.TankHit;
	}

	void OnDisable()
	{
		TankController.onHit -= this.TankHit;
	}

	// Update is called once per frame
	void Update () {

        //  Направляем башни друг на друга

        _ply1.GetComponent<TankController>().RotateTowerTo(_ply2.transform.position);
        _ply2.GetComponent<TankController>().RotateTowerTo(_ply1.transform.position);


		if (!AllowUserActivity) {return;}	//	заприетить бработку ввода без дозволения пользовательской активности


		if (Input.GetKeyDown(KeyCode.Mouse1) || _actTC.UnderAIControl)
		{
			if (GameState == enumGameState.Player1Fire || GameState == enumGameState.Player2Fire){

				if (GameState == enumGameState.Player2Fire && GameStatic.RightPlayer.isAI)		//	Если управляет AI
				{
					Vector2 _vc2 = _actTC.AIFireAt(1f, _ply1);
					ActivePos.x = _vc2.x;
					ActivePos.y = _vc2.y;
				}

				float borderX = GameObject.Find("meshBorderLine").transform.position.x;
				float distToBorder =  Mathf.Abs(ActivePos.x - borderX);	//	Расстояние до границы

				distToBorder = ActivePos.x < borderX ? distToBorder  : -distToBorder;  
				ActivePos.x = borderX + distToBorder;
				ActivePos.z = -1.5f;
				Instantiate(obj,ActivePos, Quaternion.identity);
			}

			if (GameState == enumGameState.Player1Turn)
			{
				_ply1.GetComponent<TankController>().MoveTo(ActivePos);
			}

			if (GameState == enumGameState.Player2Turn)
			{
				//  mousePos.z = 0.5f;
				if (_actTC.UnderAIControl)		//	Если управляет AI
				{
					Vector2 _vc2 = _actTC.AIMoveAt();
					ActivePos.x = _vc2.x;
					ActivePos.y = _vc2.y;
				}

				_ply2.GetComponent<TankController>().MoveTo(ActivePos);
			}

			this.NextBattleState();
			SetCursorInDefaultPosition();

			GameObject.Find("txtState").GetComponent<TextMesh>().text = GameState.ToString();
		}
	}

	public void SetCursorInDefaultPosition()
	{
		//	Изменяем размеры
		switch (GameBoardController.GameState) 
		{
		case enumGameState.Player1Fire :
		case enumGameState.Player1Turn :
			ActivePos = new Vector3(-6,0,-1);
			break;
		case enumGameState.Player2Fire :
		case enumGameState.Player2Turn :
			ActivePos = new Vector3(6,0,-1);
			break;
		default:
			break;
		}

		GameObject.Find ("FireShoot").transform.position = ActivePos;
		_curC.UpdatePosition ();
	}

	private void ChangeBorderPlace()
	{
		Vector3 vcr = GameObject.Find ("meshBorderLine").transform.position;
		vcr.x = (float)Random.Range (-10, 11) / 10f;
		GameObject.Find ("meshBorderLine").transform.position = vcr;
		BorderPosition = vcr;
	}

	public void NextBattleState()
	{
		ChangeGameStateTo((enumGameState)(++GameState));
		if ((int)GameState > 5) {
			ChangeGameStateTo((enumGameState)2);
		}
	}

	void ChangeGameStateTo(enumGameState state)
	{
		switch (state) 
		{
		//case enumGameState.BeginGame:
		//	_curC.UpdateText("Press right key ...");
		//	break;
		case enumGameState.Player1Fire:
			_actTC = _ply1.GetComponent<TankController>();
			_ply1.GetComponent<Rigidbody2D>().Sleep();
            _ply2.GetComponent<Rigidbody2D>().WakeUp();
			_curC.UpdateText("Fire");
			break;

		case enumGameState.Player2Fire:
			_actTC = _ply2.GetComponent<TankController>();
            _ply1.GetComponent<Rigidbody2D>().WakeUp();
            _ply2.GetComponent<Rigidbody2D>().Sleep();
			_curC.UpdateText("Fire");
			break;

		case enumGameState.Player1Turn:
			_actTC = _ply1.GetComponent<TankController>();
			_curC.UpdateText("Move");
			break;

		case enumGameState.Player2Turn:
			_actTC = _ply2.GetComponent<TankController>();
			_curC.UpdateText("Move");
			break;
		}


		ChangeBorderPlace ();

		SetCursorInDefaultPosition();
		_curC.UpdatePosition();

		GameState = state;
	}	

	//	Попадание в танк
	private void TankHit(Player player)
	{
		if (player.Side == enumBoardSide.Right) 
		{ 
			_score1++;
			Debug.Log ("Player "  + player.Name + " Hit");
			_tm.RaiseMessage ("Player "  + player.ToString() + " Hit", 3, Color.magenta);
		}
		else if (player.Side == enumBoardSide.Left)
		{
			_score2++;
			Debug.Log ("Player "  + player.Name + " Hit");
			_tm.RaiseMessage ("Player "  + player.ToString() + " Hit", 3, Color.magenta);
		}
		this.UpdateScore ();
	}

	public void UpdateScore()
	{
		GameObject.Find ("txtPlayer1Score").GetComponent<TextMesh> ().text = _score1.ToString();
		GameObject.Find ("txtPlayer2Score").GetComponent<TextMesh> ().text = _score2.ToString();
	}

	public void UpdatePlayers()
	{
		GameObject.Find ("txtPlayerLeftName").GetComponent<TextMesh> ().text = GameStatic.LeftPlayer.Name;
		GameObject.Find ("txtPlayerRightName").GetComponent<TextMesh> ().text = GameStatic.RightPlayer.Name;
	}

	public void ReturnToMenu()
	{
		Application.LoadLevel ("MenuScene");
	}
}

public enum enumGameState
{
	BeginGame = 1,
	Player1Turn = 2,
	Player2Turn = 3,
	Player1Fire = 4,
	Player2Fire = 5,
	BetweenTurn = 6,
	EndGame = 7
}
