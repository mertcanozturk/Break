using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBall : MonoBehaviour
{
    #region variables
    [Header("Settings")]
    [SerializeField] private int price = 0;

    [SerializeField] private float jumpHeight = 10;

    [SerializeField] public int health = 1;

    [SerializeField] private string name = "Player";

    [SerializeField] private int currentMaterialId;

    [SerializeField] private long vibrateMs = 10;

    [SerializeField] public int BallID;

    [Header("Objects")]

    public GameObject[] childs;

    public Material[] colorMaterials;

    public ParticleSystem followPartical;

    private bool disableInput = false;
    private bool autoBreak = false;
    private bool combotf = false;

    public int counter = 0;
    public int combonumber = 0;

    private int solocounter = 0;

    private float combo = 0;
    private float clickTimer = 0;

    private Rigidbody rb;
    private Renderer rd;
    private MeshRenderer mRd;

    GameController _gameController;
    ComboBarControl _comboBarControl;
    ColorPalet colorPalet;


    #endregion

    #region Shared
    void Start()
    {
        colorPalet = FindObjectOfType<ColorPalet>();
        _gameController = FindObjectOfType<GameController>();
        GameManagerInGame.Instance.SetHealth(health);
        rb = GetComponent<Rigidbody>();
        rd = GetComponent<Renderer>();
        mRd = GetComponent<MeshRenderer>();
        rb.isKinematic = true;
        combo = 1.5f / combonumber;
        ChangeMaterial(GetCurrentColorId());
        GameManagerInGame.Instance.SelectBallId = BallID;
    }
    private void Update()
    {
        InputControl();
    }

    public void SetAutoBreak()
    {
        autoBreak = false;
    }
    public void combotruefalse()
    {
        combotf = false;
    }
    public int GetPrice()
    {
        return price;
    }
    public int GetHealth()
    {
        return health;
    }
    public int GetComboNumber()
    {
        return combonumber;
    }
    public int GetCurrentColorId()
    {
        return currentMaterialId;
    }
    public float GetJumpHeight()
    {
        return jumpHeight;
    }
    public string GetName()
    {
        return name;
    }
    public void ChangeMaterial(int Id)
    {
        rd.material = colorMaterials[Id];
        var main = followPartical.main;
        main.startColor = colorMaterials[Id].color;
        currentMaterialId = Id;

        if (GameManagerInGame.Instance.gameLevelMode == GameManagerInGame.GameLevelMode.Hard)
        {
            colorPalet.ChangeUiColorImages(Id);
        }


        foreach (var child in childs)
        {
            child.GetComponent<Renderer>().material = colorMaterials[Id];
        }
    }

    public void DisableInput()
    {
        disableInput = true;
    }
    public void EnableInput()
    {
        disableInput = false;
    }
    #endregion

    void InputControl()
    {
        if (GameManagerInGame.Instance.gameState == GameManagerInGame.GameStates.Playing)
        {
            if (!_gameController.firstCm)
            {
                _comboBarControl = FindObjectOfType<ComboBarControl>();
                if (!disableInput && Time.timeScale != 0)
                {

                    if (Input.GetMouseButton(0))
                    {
                        clickTimer += Time.deltaTime;
                        if (clickTimer > 0.2f)
                        {
                            autoBreak = true;
                            ChangeMaterial(currentMaterialId);
                        }
                        else
                        {
                            autoBreak = false;
                        }
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        autoBreak = false;
                        if (GameManagerInGame.Instance.gameLevelMode == GameManagerInGame.GameLevelMode.Classic)
                        {
                            if (clickTimer < 0.2f)
                            {
                                if (Input.mousePosition.x < Screen.width / 2) --currentMaterialId;
                                else if (Input.mousePosition.x > Screen.width / 2) ++currentMaterialId;

                                if (currentMaterialId < 0)
                                {
                                    currentMaterialId = _gameController.GetAvailableColorNumber() - 1;
                                    ChangeMaterial(currentMaterialId);
                                }
                                else if (currentMaterialId <= _gameController.GetAvailableColorNumber() - 1) ChangeMaterial(currentMaterialId);
                                else
                                {
                                    currentMaterialId = 0;
                                    ChangeMaterial(currentMaterialId);
                                }
                            }
                        }
                        else
                        {
                            if (clickTimer < 0.2f)
                            {

                                if (Input.mousePosition.x < Screen.width / 2)
                                {
                                    --currentMaterialId;

                                }
                                else if (Input.mousePosition.x > Screen.width / 2)
                                {
                                    ++currentMaterialId;
                                }

                                if (currentMaterialId < 0)
                                {
                                    currentMaterialId = _gameController.GetAvailableColorNumber() - 1;
                                    ChangeMaterial(currentMaterialId);
                                }
                                else if (currentMaterialId <= _gameController.GetAvailableColorNumber() - 1)
                                {
                                    ChangeMaterial(currentMaterialId);
                                }

                                else
                                {
                                    currentMaterialId = 0;
                                    ChangeMaterial(currentMaterialId);
                                }
                            }
                        }
                        clickTimer = 0;
                    }
                }
            }
            _gameController.firstCm = false;
        }
    }

    void OnCollisionEnter(Collision Col)
    {
        var _Heptagon = Col.gameObject.GetComponent<Heptagon>();

        if (combotf == false)
        {
            if (autoBreak)
            {
                if (Col.gameObject.CompareTag("Heptagon"))
                {
                    if (_Heptagon.GetMaterial().color == mRd.material.color) // if materials equals each other
                    {
                        if (solocounter == 3) solocounter = 0;

                        _Heptagon.Break(true);

                        if (_comboBarControl.SliderValueDown())
                        {
                            combotf = true;
                            _comboBarControl.ParticlePlay();
                        }

                        if (_gameController.Vibonoff) Vibration.Vibrate(vibrateMs);
                    }
                    else
                    {
                        solocounter = 0;
                        autoBreak = false;
                        _comboBarControl.ResetSliderValue();

                        if (!_gameController.WrongImpact(_Heptagon.GetMaterial(), Col.gameObject))
                        {
                            rb.velocity = new Vector3(0, 0, 0); // set y velocity to zero
                            rb.AddForce(new Vector3(0, jumpHeight * 25, 0)); // some constant force here
                        }
                    }
                }
            }
            else
            {
                rb.velocity = new Vector3(0, 0, 0); // set y velocity to zero
                rb.AddForce(new Vector3(0, jumpHeight * 25, 0)); // some constant force here

                if (Col.gameObject.CompareTag("Heptagon"))
                {
                    if (_Heptagon.GetMaterial().color == mRd.material.color) // if materials equals each other
                    {
                        if (_gameController.Vibonoff) Vibration.Vibrate(vibrateMs);

                        _Heptagon.Break(false);

                        solocounter++;
                        if (solocounter == 3)
                        {
                            _comboBarControl.SliderValueUp();
                            solocounter = 0;
                        }
                    }
                    else
                    {
                        solocounter = 0;
                        _comboBarControl.ResetSliderValue();
                        _gameController.WrongImpact(_Heptagon.GetMaterial(), Col.gameObject);
                    }
                }

            }
        }
        else
        {

            _Heptagon.Break(true);
            _comboBarControl.SliderDown(combo);
            counter++;

            if (counter == combonumber)
            {
                Time.timeScale = 0.95f;
                rb.velocity = new Vector3(0, 0, 0);
                rb.AddForce(new Vector3(0, jumpHeight * 25, 0));
                combotf = false;
                _comboBarControl.ParticleStop();
                _comboBarControl.ResetSliderValue();
                counter = 0;
            }

        }

    }
}
