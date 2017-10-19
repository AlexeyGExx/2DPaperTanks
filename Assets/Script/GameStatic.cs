using UnityEngine;
using System.Collections;
using Gexx2DPaperTanks;

public enum enumGameType
{
	LeftVSRight,		//	На одном телефоне против себя
	VSRandom,			//	С компьютером Рандомом
	VSNetHost,			//	Хост - сетевая игра
	VSNetJoin,			//	Присоединится
	VSGexxNet			//	Через интернет
}

/// <summary>
/// Статика для игры
/// </summary>
public static class GameStatic 
{

	public static enumGameType NewGameType;
	public static Player LeftPlayer;
	public static Player RightPlayer;

	public static Player CurrentPlayer;

	/// <summary>
	/// Деактивация дочерних объектов для управления видимостью
	/// </summary>
	/// <param name="g">Родительский объект</param>
	/// <param name="a">Активный или не активный объект<c>true</c> a.</param>
	/// <param name="exeptName">Исключающее имя</param>
	public static void DeactivateChildren(GameObject g, bool a, string exeptName) {
		//	Если не имя исключение
		if (g.name != exeptName) 
		{
			g.SetActive (a);
		}

		foreach (Transform child in g.transform) {
			DeactivateChildren(child.gameObject, a, "");
		}
	}

	/// <summary>
	/// Hides the object. Через enabled
	/// </summary>
	/// <param name="obj">Object.</param>
	public static void HideObject(GameObject obj)
	{
		MeshRenderer mr = obj.GetComponent<MeshRenderer> ();
		mr.enabled = false;
	}
	/// <summary>
	/// Hides the object. Через enabled м имя объекта
	/// </summary>
	/// <param name="objName">Object name.</param>
	public static void HideObject(string objName)
	{
		GameObject _go = GameObject.Find (objName);
		if (_go == null) {Debug.Log ("Не найден объект : " + objName);}
		HideObject (_go);
	}
	
	/// <summary>
	/// Shows the object. Через Enabled
	/// </summary>
	/// <param name="obj">Object.</param>
	public static void ShowObject(GameObject obj)
	{
		MeshRenderer mr = obj.GetComponent<MeshRenderer> ();
		mr.enabled = true;
	}
	
	/// <summary>
	/// Shows the object. Через Enabled
	/// </summary>
	/// <param name="obj">Object.</param>
	public static void ShowObject(string objName)
	{
		GameObject _go = GameObject.Find (objName);
		if (_go == null) {Debug.Log ("Не найден объект : " + objName);}
		ShowObject (_go);
	}


}

