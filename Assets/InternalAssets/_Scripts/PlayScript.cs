using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс реализует управление игрой
/// </summary>
/// <remarks>
/// Начало игры, победа, поражение
/// </remarks>
public class PlayScript : MonoBehaviour
{
    public GameObject playerCarObj,canvasMnu;
    public GameObject[] carGaming;
    public Transform mainCameraTransform;
    public GameObject[] wheelsCar = new GameObject[4];
    public GameObject[] prefabsRoad = new GameObject[3];

    public int roadLine = 3;
    public float rightBarierPosition = -30f;
    public float speedCar = 0.3f,nextRoadCordinationX = 72f,speedMainCamera = 0.3f;

    private bool _checkStart = false;
    public Animation animationStartPlayMnu;

    GamingCar _GamingCarClass;
    GamingRoad _GamingRoadClass;
    PlayerCar _PlayerCarClass;
    GamingCamera _GamingCameraClass;
    public float _metersDrive;

    void Start()
    {
        int _selectRoadId = 0;
        int _selectGamingCar = 0;

        Application.targetFrameRate = 30;

        _GamingCarClass = new GamingCar(rightBarierPosition, playerCarObj);
        
        _GamingRoadClass = new GamingRoad(new GameObject[2] {
            Instantiate(prefabsRoad[_selectRoadId], prefabsRoad[_selectRoadId].transform.position, Quaternion.identity),
            Instantiate(prefabsRoad[_selectRoadId], prefabsRoad[_selectRoadId].transform.position+new Vector3(205,0), Quaternion.identity) });
        _PlayerCarClass = new PlayerCar(playerCarObj,speedCar);
        _GamingCameraClass = new GamingCamera(mainCameraTransform,speedMainCamera);
        _GamingCarClass.SetCarPrefabsInList(Instantiate(carGaming[_selectGamingCar],new Vector3(-100,200),Quaternion.identity));
    
    }

    public void StartPlay()
    {
        _PlayerCarClass.SetSpeedCar(20f);
        _PlayerCarClass.metersDriven = 0;
        _checkStart = true;
        animationStartPlayMnu.Play(); 
        canvasMnu.SetActive(false);
       // _GamingCarClass.SetStartPosition(mainCameraTransform, _PlayerCarClass.carObj.transform);
        _GamingCarClass.CreateCarsGame(carGaming[0], mainCameraTransform);
    }
  
    private void FixedUpdate()
    {      
       // _PlayerCarClass.DrivingCar();
        _GamingCameraClass.MovementCamera(playerCarObj);      
    }

    void Update()
    {

        //_PlayerCarClass.UpSpeedCar(0.5f, _checkStart, ref _GamingCameraClass);
        _GamingCameraClass.StartGame(_checkStart, _PlayerCarClass);
        _GamingRoadClass.ReplaceRoad(mainCameraTransform.position.x);    
    }

    class GamingCar
    {
        public GamingCar(float _rightBarierPosition, GameObject _playerCarObj)
        {
            this._rightBarierPosition = _rightBarierPosition;
            this._playerCarObj = _playerCarObj;
        }
        public GamingCar()
        {

        }
        private float _rightBarierPosition;
        private GameObject _playerCarObj;
        List<Transform> _gamingCarObjList = new List<Transform>();

        public void SetCarPrefabsInList(GameObject prefabCarGaming)
        {
            foreach (Transform car in prefabCarGaming.GetComponentsInChildren<Transform>())
            {
                if(car.gameObject.tag == "GamingCar" || car.gameObject.tag == "OnComingGamingCar")
                {
                    _gamingCarObjList.Add(car);
                }
                
            }
        }
      
        public void SetStartPosition(Transform transformMainCamera,Transform playerCar)
        {
            for(int i = 0,lineRoad = 1; i< _gamingCarObjList.Count; i++)
            {
                if(_gamingCarObjList[i].tag == "OnComingGamingCar")
                {
                    _gamingCarObjList[i].SetPositionAndRotation(new Vector3(transformMainCamera.position.x + 118f + Random.Range(0, 200f), playerCar.position.y, -18), Quaternion.Euler(0, 90, 0));
                }
                else
                {
                    if (lineRoad > 1)
                    {
                        lineRoad = 0;
                    }

                    _gamingCarObjList[i].SetPositionAndRotation(new Vector3(transformMainCamera.position.x + 118f + Random.Range(0, 200f), playerCar.position.y, _rightBarierPosition + lineRoad * 6), Quaternion.Euler(0, -90, 0));
                    lineRoad++;
                }
            }      
        }

        public void CreateCarsGame(GameObject gameObjectCars,Transform mainCamTransform)
        {
            Instantiate(gameObjectCars, new Vector3(mainCamTransform.position.x+130f, gameObjectCars.transform.position.y, gameObjectCars.transform.position.z), Quaternion.identity);
        }
    }

    class GamingRoad
    {
        public GamingRoad(GameObject[] prefabSelectRoad)
        {
            this.roadPrefabs = prefabSelectRoad;
        }
        public GameObject[] roadPrefabs = new GameObject[2];
        private int _roadLine;

        public int roadLine
        {
            get
            {
                return _roadLine;
            }
            set
            {
                if (value > 0)
                {
                    _roadLine = value;
                }
            }
        }

        int _finalRoadIndex = 1;

        
        
        public void ReplaceRoad(float mainCamTranformX)
        {
            if(mainCamTranformX - roadPrefabs[_finalRoadIndex].transform.position.x>= 80f)
            {
                ReplaceRoad();
            }
        }

        private void ReplaceRoad()
        {
            _finalRoadIndex++;
            if(_finalRoadIndex > 1)
            {
                _finalRoadIndex = 0;
            }
            roadPrefabs[_finalRoadIndex].transform.position += new Vector3 (205 * 2,0,0);
        }
    }

    class GamingCamera
    {
        public GamingCamera(Transform mainCamTransform,float speedMainCam)
        {
            this.mainCameraTransform = mainCamTransform;
            this.speedMainCam = speedMainCam;
        }

        public Transform mainCameraTransform;
        private float _speedMainCam = 0;

        public float speedMainCam
        {
            get
            {
                return _speedMainCam;
            }
            set
            {
                if(value > 100f)
                {
                    _speedMainCam = 100f;
                }
                else
                {
                    _speedMainCam = value;
                }
            }
        }
        /// <summary>
        /// Метод, срабатвывающий при запуске игры
        /// </summary>
        /// <param name="checkStart">Проверяет запущена ли игра</param>
        /// <param name="playerCarClass"></param>
        public void StartGame(bool checkStart, PlayerCar playerCarClass)
        {
            if (checkStart && Mathf.Abs(mainCameraTransform.transform.position.x - playerCarClass.carObj.transform.position.x) >= 8)
            {
                //speedMainCam += 4 * Time.deltaTime;
                if (_speedMainCam < 20)
                {
                    _speedMainCam += 6 * Time.deltaTime;
                }
                else if (_speedMainCam > playerCarClass.speedCar)
                {
                    _speedMainCam = playerCarClass.speedCar;
                }
            }
        }


        /// <summary>
        /// Движение камеры по X
        /// </summary>
        /// <param name="playerCar">Автомобиль игрока</param>
        public void MovementCamera(GameObject playerCar)
        {
            if (playerCar.transform.position.x - mainCameraTransform.position.x >= 36)
            {
                mainCameraTransform.position = new Vector3(playerCar.transform.position.x - 36, mainCameraTransform.position.y,
                    mainCameraTransform.transform.position.z);
            }
            else
            {
                mainCameraTransform.Translate(Vector3.right * _speedMainCam * Time.deltaTime, Space.World);
            }
        }
    }

    class PlayerCar : Car
    {
        public PlayerCar(GameObject playerCarObj, float speedCar)
        {
            this.carObj = playerCarObj;
            this.speedCar = speedCar;
           wheelsCar = carObj.GetComponentsInChildren<Transform>();
            wheelsCar[0] = wheelsCar[1];
        }

        private Transform[] wheelsCar = new Transform[4];
        private float _metersDriven;
        private float _speedCar;
        private float metersStartOncomingLane = 0;
        public float metersDriven
        {
            get
            {
                return _metersDriven;
            }

            set
            {
                if(value >= 0)
                {
                    _metersDriven = value;
                }
            }
        }

        public float speedCar
        {
            get
            {
                return _speedCar;
            }

            private set
            {
                if (value > 100)
                {
                    _speedCar = 100;
                }
                else if (value < 8)
                {
                    _speedCar = 8;
                }
                else
                {
                    _speedCar = value;
                }
            }
        }

        private void RotateWheels()
        {
            foreach (Transform wheel in wheelsCar)
            {
                wheel.transform.Rotate(_speedCar, 0, 0);
            }
        }
        public void SetSpeedCar(float speedCar)
        {
            this.speedCar = speedCar;
        }

        public void UpSpeedCar(float plusSpeedCar)
        {
            this.speedCar += plusSpeedCar;
        }

        public void UpSpeedCar(float plusSpeedCar,bool checkStart, ref GamingCamera GamingCameraClass)
        {
            if (checkStart && _metersDriven >= 100)
            {
                _metersDriven = 0;
                speedCar += 0.5f;
                GamingCameraClass.speedMainCam = speedCar;
            }           
        }

        public void DrivingCar()
        {
            RotateWheels();
            _metersDriven += Time.deltaTime * speedCar;
            
            carObj.transform.position += new Vector3(_speedCar * Time.deltaTime, 0, 0);
        }
    }

    abstract class Car
    {
        public GameObject carObj = new GameObject();
    }
}
