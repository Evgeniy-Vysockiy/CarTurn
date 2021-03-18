using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс описывает поведение городского трафика
/// </summary>
public class BehaviourIntelegemceBotCar : MonoBehaviour
{
    [Header("Скорость и ускорение автомобиля трафика")]
    [Tooltip("Скорость автомобиля трафика")] [SerializeField] [Range(0f, 5f)] public float speedBotCar = 0.3f;
    [Tooltip("Ускорение автомобиля трафика")] [SerializeField] [Range(0f, 3f)] private float accelerationBotCar = 0.3f;

    [Header("Дистанция автомобиля трафика")]
    [Tooltip("Дистанция автомобиля трафика во время движения")] [SerializeField] [Range(3f, 10f)]
    private float distanceBotCar = 10f;
    [Tooltip("Дистанция автомобиля трафика\nпосле остановки в потоке")] [SerializeField] [Range(1f, 50f)]
    private float startDistanceCar = 40f;

    [Space] [Tooltip("Дистанция автомобиля трафика во время движения")] [SerializeField][Range(0f, 60f)]
    private float lenghtRayBotCar = 12f;


    private Transform mainCamTranform;
    public bool stopCar = false;
    public float coordinationXCarBot;
    /// <summary>
    /// Направление движения автомобиля
    /// true - встречное направление
    /// false - направление в сторону автомобиля игрока
    /// </summary>
    private bool _comingGamingCar = false;

    /// <summary>
    /// Луч, исходящий из капота (передней части) автомобиля трафика
    /// </summary>
    private Ray _rayFrontCarBot;

    /// <summary>
    /// Размер коллайдера на автомобиле трафика
    /// по z
    /// </summary>
    private float sizeBoxColliderBotCar = 0.3f;

    [SerializeField] private float timeStopTimer;
    private float timeStopCarBot = 2f;
    private void Awake()
    {
        _comingGamingCar = SetDirectionGamingCar(gameObject.tag);
        accelerationBotCar = 2f;
        timeStopTimer = Random.Range(1, 10);
    }

    /// <summary>
    /// Установка направления автомобиля трафика
    /// в зависимости от его тега в gameObject.tag
    /// </summary>
    /// <param name="tagGamingCar">Тег gameObject</param>
    /// <returns>Возвращает значение True, если автомобиль для встречного движения и False в обратном случае</returns>
    private bool SetDirectionGamingCar(string tagGamingCar) => tagGamingCar == "OnComingGamingCar" ? true : false;

    /// <summary>
    /// Выбор направления движения автомобиля трафика в
    /// зависимости от его направления
    /// </summary>
    /// <param name="_comingGamingCar">Встречное навправление (true/false) </param>
    /// <returns>Возвращает new Vector3 </returns>
    private Vector3 SetNewVectorAtDrivingCar(bool _comingGamingCar) => _comingGamingCar == true ?
        new Vector3(-1*speedBotCar * Time.deltaTime, 0, 0) : new Vector3(speedBotCar * Time.fixedDeltaTime, 0, 0);

    private void CountDownTimerStopTime()
    {
        timeStopTimer -= Time.deltaTime;
        if(timeStopTimer <= 0)
        {
            stopCar = true;
        }
    }
    void Start()
    {
        mainCamTranform = Camera.main.transform;
        speedBotCar = 3;
        SetValueDistanceCarBot(10f, 14f, 10f);
        ///Присваиваем размер коллайдера автомобиля трафика и делим на 2
        sizeBoxColliderBotCar += gameObject.GetComponent<BoxCollider>().size.z / 2;
    }
    private void SetSpeed(float acceleration,ref float currentSpeed, float requiredSpeed,bool smoothness)
    {
        if(currentSpeed == requiredSpeed)
        {
            return;
        }
        if (smoothness)
        {
            if(requiredSpeed > currentSpeed)
            {
                currentSpeed += Time.deltaTime * acceleration;
                if (currentSpeed > requiredSpeed)
                {
                    currentSpeed = requiredSpeed;
                    return;
                }
            }
            else if(requiredSpeed < currentSpeed && requiredSpeed !=0f)
            {
                currentSpeed -= Time.deltaTime * acceleration;  
            }
            else if(requiredSpeed < currentSpeed && requiredSpeed == 0f)
            {
                currentSpeed -= Time.deltaTime * acceleration*3;
            }
            if(requiredSpeed == 0f && currentSpeed < 0)
            {
                currentSpeed = 0;
            }
            
        }
        else
        {
            currentSpeed = requiredSpeed;
        }
        if(currentSpeed < 0)
        {
            currentSpeed = 0;
        }
    }

    /// <summary>
    /// Движение автомобиля
    /// </summary>
    private void DrivingCar()
    {
        transform.position += SetNewVectorAtDrivingCar(_comingGamingCar);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Handheld.Vibrate();
        }
        Debug.Log(collider.gameObject.name);
    }
    /// <summary>
    /// Установка значений параметров дистанций и длины луча автомобиля трафика
    /// </summary>
    /// <param name="distanceBotCar">Дистанция автомобиля трафика</param>
    /// <param name="startDistanceCar">Дистанция автомобиля трафика после остановки</param>
    /// <param name="lenghtRayBotCar">Длина луча, выходящий из капота (передней части) автомобиля трафика</param>
    private void SetValueDistanceCarBot(float distanceBotCar,float startDistanceCar,float lenghtRayBotCar)
    {
        this.distanceBotCar = distanceBotCar;
        this.startDistanceCar = startDistanceCar;
        this.lenghtRayBotCar = lenghtRayBotCar;
    }

    private float GetDistanceCarBot(float coordinationXcarBotInFront, float coordinationXcurrectCarBot) =>
         Mathf.Abs(coordinationXcarBotInFront - coordinationXcurrectCarBot);
    
    /// <summary>
    /// Направление луча в зависимости от направления движения автомобиля трафика
    /// </summary>
    /// <param name="_comingGamingCar">Направление движения автомобиля трафика</param>
    /// <returns>Возвращает луч, направленный в сторону движения автомобиля трафика</returns>
    private Ray GetRayCarBot(bool _comingGamingCar)
    {
        if (_comingGamingCar)
        {
            Vector3 lineStart = new Vector3(transform.position.x - sizeBoxColliderBotCar, transform.position.y + 2f, transform.position.z);
            Debug.DrawRay(lineStart, -transform.forward * lenghtRayBotCar, Color.green);
            return new Ray(lineStart, -transform.forward * lenghtRayBotCar);
        }
        else
        {
            Vector3 lineStart = new Vector3(transform.position.x + sizeBoxColliderBotCar, transform.position.y + 2f, transform.position.z);
            Debug.DrawRay(lineStart, -transform.forward * lenghtRayBotCar, Color.green);
            return new Ray(lineStart, -transform.forward * lenghtRayBotCar);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
    }
   
    private void Update()
    {
        coordinationXCarBot = gameObject.transform.position.x;
        Ray ray = GetRayCarBot(_comingGamingCar);
        RaycastHit raycastHitDistanceCar = new RaycastHit();
        if (Physics.Raycast(ray, out raycastHitDistanceCar))
        {
            BehaviourIntelegemceBotCar behaviourIntelegemceBotCar = raycastHitDistanceCar.collider.GetComponent<BehaviourIntelegemceBotCar>();
            if (behaviourIntelegemceBotCar != null)
            {
                if(GetDistanceCarBot(behaviourIntelegemceBotCar.coordinationXCarBot,transform.position.x) <= distanceBotCar)
                {
                    if (GetDistanceCarBot(behaviourIntelegemceBotCar.coordinationXCarBot, transform.position.x) < distanceBotCar && speedBotCar!= 0)
                    {
                        SetSpeed(accelerationBotCar*1.5f, ref speedBotCar,
                            behaviourIntelegemceBotCar.speedBotCar, true);
                        if(GetDistanceCarBot(behaviourIntelegemceBotCar.coordinationXCarBot, transform.position.x) < distanceBotCar - 3f)
                        {
                            SetSpeed(accelerationBotCar * 1.5f, ref speedBotCar,
                            behaviourIntelegemceBotCar.speedBotCar, false);
                        }
                    }
                    else if(speedBotCar != 0)
                    {
                        SetSpeed(accelerationBotCar, ref speedBotCar,
                            behaviourIntelegemceBotCar.speedBotCar, true);
                    }
                }
                else
                {
                    if(GetDistanceCarBot(behaviourIntelegemceBotCar.coordinationXCarBot, transform.position.x) >= startDistanceCar)
                    {
                        SetSpeed(accelerationBotCar, ref speedBotCar,
                            behaviourIntelegemceBotCar.speedBotCar + 2f, true);                         
                    }   
                }
            }
            else
            {
                Transform transformCarPlayer = raycastHitDistanceCar.collider.GetComponent<Transform>();
                if (transformCarPlayer != null)
                {
                    if (GetDistanceCarBot(transformCarPlayer.position.x, transform.position.x) <= distanceBotCar)
                    {
                        if (GetDistanceCarBot(transformCarPlayer.position.x, transform.position.x) < distanceBotCar - 2f && speedBotCar != 0)
                        {
                            SetSpeed(accelerationBotCar * 3.5f, ref speedBotCar,
                                0, false);
                        }
                        else
                        {
                            SetSpeed(accelerationBotCar, ref speedBotCar,
                               1f, true);
                        }
                    }
                    else
                    {
                        if (GetDistanceCarBot(transformCarPlayer.position.x, transform.position.x) >= startDistanceCar)
                        {
                            SetSpeed(accelerationBotCar*3, ref speedBotCar,
                                5f, true);
                        }
                    }
                }
            }
        }
        else if (!stopCar)
        {
            SetSpeed(accelerationBotCar, ref speedBotCar,
                            3f, true);
            CountDownTimerStopTime();
        }else if (stopCar)
        {
            SetSpeed(accelerationBotCar, ref speedBotCar,
                            0f, true);
        }
    }

    private void ChangePositionCarBot(Transform mainCameraTransform)
    {
        if(transform.position.x - mainCameraTransform.position.x > 100)
        {
            transform.position = new Vector3(transform.position.x - 130f, transform.position.y, transform.position.z);
        }else if(mainCameraTransform.position.x - transform.position.x > 20)
        {
            transform.position = new Vector3(transform.position.x + 130f, transform.position.y, transform.position.z);
        }
    }
    private void CheckTimeStopCarBot(ref bool stopCarBot)
    {
        if (stopCarBot)
        {
            timeStopCarBot -= Time.deltaTime;
            if (timeStopCarBot <= 0)
            {
                timeStopTimer = Random.Range(3, 10);
               stopCarBot = false;
                timeStopCarBot = 2f;
            }
        }
    }
    private void FixedUpdate()
    {
        CheckTimeStopCarBot(ref stopCar);
        ChangePositionCarBot(mainCamTranform);
        DrivingCar();

    }
}


