using UnityEngine;
using System.Collections;

/// <summary>
/// Класс для работы с TextMesh
/// </summary>
public class MessageControl : MonoBehaviour {

	private TextMesh _tm;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{
		_tm = gameObject.GetComponent<TextMesh> ();
		this.HideMessage();
	}

	/// <summary>
	/// Вызов сообщения
	/// </summary>
	public void RaiseMessage(string msg, float raiseTime, Color msgColor, params string[] addArgs)
	{
		_tm.text = msg;
		_tm.color = msgColor;
		this.ShowMessage ();
		Invoke ("HideMessage", raiseTime);
	}

	private  void ShowMessage()
	{
		_tm.GetComponent<MeshRenderer> ().enabled = true;
	}

	private void HideMessage()
	{
		_tm.GetComponent<MeshRenderer> ().enabled = false;
	}
}
