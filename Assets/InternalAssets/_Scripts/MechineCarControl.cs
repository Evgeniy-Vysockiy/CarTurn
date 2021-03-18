using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Управление автомбилем игрока (carGameObj)
/// </summary>
public class MechineCarControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject carGameObj;
    [SerializeField] [Range(0f, 0.5f)] float maxSpeed = 0.3f;
    public float _speedPlayerCar;
    public float horizontal_speedPlayerCar = 20f, angelRotation, speedTurnRotation = 20f;
    public float leftBarierPosition = -18f, rightBarierPosition = -30f;
    private float _currentPositionCar = 0;
    private bool _breakeCar = false;
    private bool turnRight, turnLeft;
    public PlayScript PlayScript;
    public Rigidbody rigidbodyPlayCar;
    void Start()
    {
 
    }

    // Слайд
    public void OnBeginDrag(PointerEventData eventData)
    {
        DirectionRotation(eventData);
    }
    private void FixedUpdate()
    {
        DrivingCar();
    }
    void Update()
    {
        Turn();

    }

    // Нажатие и перемещение
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        if (Mathf.Abs(eventData.delta.x) > 15f && Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            DirectionRotation(eventData);
        }
    }

    // Нажатие
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!turnLeft && !turnRight)
        {
            _breakeCar = true;
        }
    }

    // Отпустить
    public void OnPointerUp(PointerEventData eventData)
    {     
            _breakeCar = false;      
    }

/// <summary>
/// Перемещение автомобиля игрока
/// </summary>
    public void DrivingCar()
    {
        if (_speedPlayerCar < maxSpeed && _breakeCar == false)
        {
            _speedPlayerCar += Time.fixedDeltaTime * 0.12f;

        }
        else if (_breakeCar == true && _speedPlayerCar > 0)
        {
            _speedPlayerCar -= Time.fixedDeltaTime * 0.3f;
        }
        else if (_breakeCar == true && _speedPlayerCar <= 0)
        {
            _speedPlayerCar = 0;
        }

        carGameObj.transform.position += new Vector3(_speedPlayerCar, 0, 0);
    }
    /// <summary>
    /// Ожидание поворота автомобиля
    /// Метод вызывается из Update
    /// </summary>
    public void Turn()
    {
        if (turnRight)
        {
            RightTurn();
        }
        else if (turnLeft)
        {
            LeftTurn();
        }
    }

    /// <summary>
    /// Проверка минимальной скорости авто для совершение поворота
    /// Если скорость меньше 0.1f, то скорость увеличивается до 0.3f
    /// </summary>
    /// <param name="speedCar">Текущая скорость автомобиля игрока</param>
    /// <returns>Возвращает необходимую для поворота скорость</returns>
    float CheckMinSpeedCarTurn(float speedCar) => speedCar < 0.1f ? 0.1f : speedCar;

    /// <summary>
    /// Проверка направления поворота
    /// </summary>
    /// <param name="eventData">Отслеживание напраления слайда</param>
    public void DirectionRotation(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y) && turnLeft == false && turnRight == false)
        {
            _currentPositionCar = carGameObj.transform.position.z;
           
            if (eventData.delta.x > 0 && !turnLeft && carGameObj.transform.position.z - 6f >= rightBarierPosition)
            {
                turnRight = true;
                _speedPlayerCar = CheckMinSpeedCarTurn(_speedPlayerCar);
                turnLeft = !turnRight;
            }
            else if (eventData.delta.x < 0 && !turnRight && carGameObj.transform.position.z + 6f <= leftBarierPosition)
            {
                turnLeft = true;
                _speedPlayerCar = CheckMinSpeedCarTurn(_speedPlayerCar);
                turnRight = !turnLeft;
            }
        }
        else if (Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y))
        {

        }
    }


/// <summary>
/// Левый поворот автомобиля игрока
/// </summary>
    void LeftTurn()
    {
        if (carGameObj.transform.position.z < _currentPositionCar + 6)
        {
            if (rigidbodyPlayCar.transform.rotation.eulerAngles.y > 270f - angelRotation)
            {
                rigidbodyPlayCar.MoveRotation(rigidbodyPlayCar.rotation * Quaternion.Euler(0, -speedTurnRotation * Time.fixedDeltaTime*3, 0));    
            }
            carGameObj.transform.position += new Vector3(0, 0, horizontal_speedPlayerCar * Time.fixedDeltaTime);
        }
        else
        {
            if (carGameObj.transform.rotation.eulerAngles.y < 270f)
            {
                rigidbodyPlayCar.MoveRotation(rigidbodyPlayCar.rotation * Quaternion.Euler(0, speedTurnRotation * Time.fixedDeltaTime*4, 0));
            }
            else
            {
                turnLeft = false;
                rigidbodyPlayCar.transform.localEulerAngles = new Vector3(0, 270, 0);
                rigidbodyPlayCar.transform.position = new Vector3(rigidbodyPlayCar.position.x,
                    rigidbodyPlayCar.position.y, _currentPositionCar + 6);
            }
        }
    }

    /// <summary>
    /// Правый поворот автомобиля игрока
    /// </summary>
    void RightTurn()
    {
        if (carGameObj.transform.position.z > _currentPositionCar - 6)
        {        
            if(carGameObj.transform.rotation.eulerAngles.y < 270f + angelRotation)
            {
                rigidbodyPlayCar.MoveRotation(rigidbodyPlayCar.rotation * Quaternion.Euler(0, speedTurnRotation * Time.fixedDeltaTime * 3, 0));
            }
            carGameObj.transform.position -= new Vector3(0, 0, horizontal_speedPlayerCar * Time.fixedDeltaTime);
        }
        else
        {
            if (carGameObj.transform.rotation.eulerAngles.y > 270f)
            {
                rigidbodyPlayCar.MoveRotation(rigidbodyPlayCar.rotation * Quaternion.Euler(0, -speedTurnRotation * Time.fixedDeltaTime * 4, 0));
            }
            else
            {
                turnRight = false;
                rigidbodyPlayCar.transform.localEulerAngles = new Vector3(0, 270, 0);
                rigidbodyPlayCar.transform.position = new Vector3(rigidbodyPlayCar.position.x,
                    rigidbodyPlayCar.position.y, _currentPositionCar - 6);
            }
        }
    }
}
