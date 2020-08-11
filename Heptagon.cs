using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heptagon : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] private int hitForBeDestroyed = 1;

    [SerializeField] private int autoBreakingPoint = 26;

    [SerializeField] private int breakingPoint = 13;

    private GameManagerInGame.GameColors currentColor;

    private MeshRenderer mRd;
    private Renderer rd;
    private GameController _gameController;

    private void Start()
    {
        mRd = GetComponent<MeshRenderer>();
        rd = GetComponent<Renderer>();
        _gameController = FindObjectOfType<GameController>();
    }

    public Material GetMaterial()
    {
        return GetComponent<Renderer>().material;
    }

    public void Break(bool autoBreak)
    {
        //if (hitForBeDestroyed == 0)
        //{
        _gameController.ProgressValueUp();


        if (gameObject.transform.position.y != GameManagerInGame.Instance.HeptagonCreator.GetLastObject().transform.position.y)
        {
            GameObject obj;

            GameManagerInGame.Instance.AudioManager.jump.Play();

            if (GetColorNumber() == 0)
            {
                obj = Instantiate(GameManagerInGame.Instance.HeptagonCreator._Heptagons[GameManagerInGame.Instance.SelectBallId].breakHeptagonAnimation[0], transform.position, Quaternion.identity);
            }
            else if (GetColorNumber() == 1)
            {
                obj = Instantiate(GameManagerInGame.Instance.HeptagonCreator._Heptagons[GameManagerInGame.Instance.SelectBallId].breakHeptagonAnimation[1], transform.position, Quaternion.identity);
            }
            else
            {
                obj = Instantiate(GameManagerInGame.Instance.HeptagonCreator._Heptagons[GameManagerInGame.Instance.SelectBallId].breakHeptagonAnimation[2], transform.position, Quaternion.identity);
            }

            Destroy(obj, 2);


            _gameController.CameraDown();

            if (autoBreak)
            {
                _gameController.IncreaseScore(autoBreakingPoint); // Add Auto Breaking Point;
                Time.timeScale = 1.5f;
            }
            else
            {
                _gameController.IncreaseScore(breakingPoint);    // Add Breaking Point
                Time.timeScale = 1f;
                _gameController.CreateNewDiamond(gameObject.transform.position);
            }
            _gameController.CreateNewShapes(autoBreak);
            Destroy(gameObject);
        }
        else
        {
            _gameController.levelup();
            GameManagerInGame.Instance.LevelManager.LevelUpCreateNewShapes();
        }
        //}
        //else
        //{
        //    --hitForBeDestroyed;
        //    Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        //    ChangeMaterial();
        //}

    }

    private int GetColorNumber()
    {
        int index = 0;
        foreach (var material in _gameController.GetMaterials())
        {
            if (rd.material.color == material.color)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    public void ChangeMaterial(int Id)
    {
        rd.material = _gameController.GetMaterials()[Id];
    }
    public void ChangeMaterial()
    {
        int rndNumber = Random.Range(0, 2);

        Material material;
        switch (rndNumber)
        {
            case 0:
                material = _gameController.GetMaterials()[0];
                break;
            default:
                material = _gameController.GetMaterials()[1];
                break;
        }

        rd.material = material;
    }

}
