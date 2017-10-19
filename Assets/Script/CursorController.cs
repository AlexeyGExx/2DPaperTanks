using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour {
	private LineRenderer _lr;
	private TextMesh _tMesh;


	// Use this for initialization
	void Start () {
		_lr = GameObject.Find("HCursor").GetComponent<LineRenderer> ();
		_tMesh = GameObject.Find ("txtUnitStatement").GetComponent<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//	Снимаем координаты мыши
		GameBoardController.mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		GameBoardController.mousePos.z = -1f;

		//	Ограничиваем перемещения по оси Х, для игроков =)
		if ((GameBoardController.GameState == enumGameState.Player1Fire || GameBoardController.GameState == enumGameState.Player1Turn)  && GameBoardController.mousePos.x > -1) 
		{
			GameBoardController.mousePos.x = -1;
		}
		
		if ((GameBoardController.GameState == enumGameState.Player2Fire || GameBoardController.GameState == enumGameState.Player2Turn) && GameBoardController.mousePos.x < 1) 
		{
			GameBoardController.mousePos.x = 1;
		}

		if (Input.GetKey(KeyCode.Mouse0) || GameBoardController._actTC.UnderAIControl)
		{
			GameBoardController.ActivePos = GameBoardController.mousePos;
			GameBoardController.ActivePos.z = -0.9f;
			UpdatePosition(GameBoardController.ActivePos);
		}
	}

	public void UpdateText(string textCursorState)
	{
		_tMesh.text = textCursorState;
	}

	public void UpdatePosition()
	{
		//	Изменяем размеры
		switch (GameBoardController.GameState) 
		{
		case enumGameState.Player1Fire :
		case enumGameState.Player1Turn :
			_lr.SetPosition(0, new Vector3(-30,0,0));									//	Левая граница
			_lr.SetPosition(1, new Vector3(Mathf.Sqrt(Mathf.Pow(GameBoardController.ActivePos.x,2) - Mathf.Pow (GameBoardController.BorderPosition.x,2)),0,0));	//	Правая граница
			break;
		case enumGameState.Player2Fire :
		case enumGameState.Player2Turn :
			_lr.SetPosition(1, new Vector3(30,0,0));									//	Правая граница
			_lr.SetPosition(0, new Vector3(-(Mathf.Sqrt(Mathf.Pow(GameBoardController.ActivePos.x,2) - Mathf.Pow (GameBoardController.BorderPosition.x,2))),0,0));	//	Правая граница
			break;
		default:
			break;
		}
	}

	public void UpdatePosition(Vector3 positionXYZ)
	{
		transform.position = positionXYZ;
		//	Изменяем размеры
		this.UpdatePosition ();
	}

	public void HideCursor()
	{
		GameStatic.DeactivateChildren (gameObject, false, "FireShoot");
	}

	public void ShowCursor()
	{
		GameStatic.DeactivateChildren (gameObject, true, "FireShoot");
	}
}
