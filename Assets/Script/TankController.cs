using UnityEngine;
using System.Collections;
using Gexx2DPaperTanks;

public class TankController : MonoBehaviour {


	public delegate void HitHandler(Player player);
	public static event HitHandler onHit;

    private Ammo _ammo;
    private enumTankState _state;		//	Состояние танка
    
	public Player player;					//	Игрок

    private Vector3 _moveTo;
	private float _speed = 8f;


	private float _rotSpeed = 2f;     	//  Скорость вращения
    private Vector3 _rdirBody;          //  Направление вращения шасси
	private Vector3 _rdirBodyFrom;      //  Направление вращения шасси

    private Vector3 _rdirTower;         //  Направление вращения орудия

	private Quaternion _rtrBodyFrom;
	private Quaternion _rtrBodyTo;

    public Vector3 _dirV;				//	Вектор напарвление
	public Vector3 _rdirV;

    public GameObject tower;			//	Башня

	public bool UnderAIControl;			//	Контроль осуществляется компьютером
	public bool NetPlayer;				//	Сетевой игрок


	void Start () 
	{
		_state = enumTankState.Wait;
		_rdirV = transform.right;
	}
	
	void Update () 
	{
        //  Вращение
		if (_state == enumTankState.Rotate) {
			Vector3 newDir = Vector3.RotateTowards (transform.right, _rdirBody, Time.deltaTime * _rotSpeed, 0.0F);
			transform.right = newDir;

			if (Vector3.Angle(newDir,_rdirBody) < 1)
				{
					transform.right = _rdirBody;
					_state = enumTankState.Move;
				}

		} 
		//  Перемещение
		if (transform.position != _moveTo & _state == enumTankState.Move) 
			{
				transform.position = Vector3.MoveTowards(transform.position,_moveTo, _speed * Time.deltaTime);
				if (transform.position == _moveTo) {_state = enumTankState.Wait; GameBoardController.AllowUserActivity = true;} 
			}
	}

	public void SetState(enumTankState newState)
	{
		_state = newState;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		onHit (player);
	} 

	public void MoveTo(Vector3 moveTo)
	{
		this._moveTo = moveTo;

		GameBoardController.AllowUserActivity = false;

		Vector3 v1 = new Vector3(_moveTo.x, _moveTo.y, 0);
		Vector3 v2 = new Vector3(transform.position.x, transform.position.y, 0);
		
		_rdirV = v1 - v2;
		
		_rdirBody = Vector3.Normalize(_rdirV);
		_rdirBodyFrom = transform.right;

		this._state = enumTankState.Rotate;

	}

	/// <summary>
	/// Установить тип боеприпаса
	/// </summary>
	/// <param name="ammoType">Ammo type.</param>
	public void SetAmmo(Ammo ammoType)
	{
		_ammo = ammoType;
	}

	/// <summary>
	/// Поворот техники
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public void RotateTowerTo(Vector3 toXYZ)
	{
        Vector3 v1 = new Vector3(toXYZ.x, toXYZ.y, 0);
        Vector3 v2 = new Vector3(transform.position.x, transform.position.y, 0);

        _rdirV = v1 - v2;
        _rdirTower = Vector3.Normalize(_rdirV);
        tower.transform.right = Vector3.Normalize (_rdirV);
        
	}

	/// <summary>
	/// Передвижение ИскИна
	/// </summary>
	public Vector2 AIMoveAt()
	{
		if (UnderAIControl == false) {
			Debug.Log ("Tank not under AI Control");
			throw new UnityException("Tank is not under AI Control");
		}

		Rect _rect = GameBoardController.RightPlayerGameField;
		float _x = Random.Range (_rect.xMin, _rect.xMax);
		float _y = Random.Range (_rect.yMin, _rect.yMax);

		return new Vector2(_x,_y);

	}

	public Vector2 AIFireAt(float AIAccuracy, GameObject target)
	{
		if (UnderAIControl == false) 
		{
			Debug.Log ("Tank is not under AI Control");
			throw new UnityException("Tank is not under AI Control");
		}

		Vector2 _vc2 = Random.insideUnitCircle;

		float _x = target.transform.position.x + _vc2.x * AIAccuracy;	//	Х со смещением
		float _y = target.transform.position.y + _vc2.y * AIAccuracy;	//	Y со смещением

		//	_y не меняется
		// x = 2 расстяния влево или 2 расстояния вправо

		
		float _bp = GameBoardController.BorderPosition.x;
		float _distToBP =  Mathf.Abs(_x - _bp);	//	Расстояние до границы
		
		_distToBP = _x < _bp ? _distToBP  : -_distToBP;  //	Смещение вправо или влево
		_x = _bp + _distToBP;								//	

		return new Vector2 (_x * AIAccuracy, _y * AIAccuracy);
	}
}

public enum enumTankState
{
	Live,		//	Живой ожидает действия
	Dead,		//	Мертвый не может быть активным
	Turn,		//	Живой совершает действия
	Move,
	Wait,
	Rotate
}

/// <summary>
/// Ammuntion.
/// </summary>
public class Ammuntion<Ammo> : System.Collections.DictionaryBase
{

}


public struct Ammo
{
	public string Type;
	public float Power;
	public float ExplosiveRange;

	public Ammo ( string type, float power, float explosiveRange)
	{
		this.Type = type; this.Power = power; this.ExplosiveRange = explosiveRange;
	}
}
