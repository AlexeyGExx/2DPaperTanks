using UnityEngine;
using System.Collections;
using Gexx2DPaperTanks;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Exits the app.
	/// </summary>
	public void ExitApp()
	{
		Application.Quit ();
	}

	public void LeftVSRightGame()
	{
		GameStatic.NewGameType = enumGameType.LeftVSRight;
		GameStatic.LeftPlayer = new Player ("LeftHand", false,false);
		GameStatic.LeftPlayer.Side = enumBoardSide.Left;

		GameStatic.RightPlayer = new Player ("RightHand", false,false);
		GameStatic.RightPlayer.Side = enumBoardSide.Right;

		Application.LoadLevel ("gameboard_v2");
	}

	public void VSRandom()
	{
		GameStatic.NewGameType = enumGameType.VSRandom;
		GameStatic.LeftPlayer = new Player ("LeftHand", false,false);
		GameStatic.RightPlayer = new Player ("RightHand", true,false);

		Application.LoadLevel ("gameboard_v2");
	}

	public void VSNetHost()
	{
		GameStatic.NewGameType = enumGameType.VSNetHost;
		GameStatic.LeftPlayer = new Player ("LeftHand", false,false);
		GameStatic.RightPlayer = new Player ("RightHand", false,true);
		
		Application.LoadLevel ("gameboard_v2");
	}

	public void VSNetJoin()
	{
		GameStatic.NewGameType = enumGameType.VSNetJoin;
		GameStatic.LeftPlayer = new Player ("LeftHand", false,false);
		GameStatic.RightPlayer = new Player ("RightHand", false,false);
		
		Application.LoadLevel ("gameboard_v2");
	}

}
