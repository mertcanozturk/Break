using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeptagonCreator : MonoBehaviour
{
    [System.Serializable]
    public class Heptagons
    {
        public GameObject[] heptagonPrefabs;
        public GameObject[] breakHeptagonAnimation;
    }

    public Heptagons[] heptagons;

    [SerializeField] private Transform lastObjectTransform;

    PlayerBall playerBall;

    public void CreateHexagon(GameManagerInGame.GameColors color)
    {
        GameObject hexagon;
        Vector3 pos;

        var lastObject = lastObjectTransform;

        if (color == GameManagerInGame.GameColors.Blue)
        {
            pos = new Vector3(lastObject.position.x, lastObject.position.y - 0.15f, lastObject.position.z);
            hexagon = Instantiate(heptagons[GameManagerInGame.Instance.SelectBallId].heptagonPrefabs[0], pos, Quaternion.Euler(lastObject.eulerAngles.x, lastObject.eulerAngles.y - 3, lastObject.eulerAngles.z));
        }
        else if (color == GameManagerInGame.GameColors.Red)
        {
            pos = new Vector3(lastObject.position.x, lastObject.position.y - 0.15f, lastObject.position.z);
            hexagon = Instantiate(heptagons[GameManagerInGame.Instance.SelectBallId].heptagonPrefabs[1], pos, Quaternion.Euler(lastObject.eulerAngles.x, lastObject.eulerAngles.y - 3, lastObject.eulerAngles.z));
        }
        else
        {
            pos = new Vector3(lastObject.position.x, lastObject.position.y - 0.15f, lastObject.position.z);
            hexagon = Instantiate(heptagons[GameManagerInGame.Instance.SelectBallId].heptagonPrefabs[2], pos, Quaternion.Euler(lastObject.eulerAngles.x, lastObject.eulerAngles.y - 3, lastObject.eulerAngles.z));
        }

        lastObjectTransform = hexagon.transform;

    }

    public void SetDefaultPosition()
    {
        playerBall = FindObjectOfType<PlayerBall>();
        lastObjectTransform.position = new Vector3(-0.009595871f, playerBall.transform.position.y - 0.8f, -0.01231194f);
    }

    public void SetDefaultPositionInGame()
    {
        playerBall = FindObjectOfType<PlayerBall>();
        lastObjectTransform.position = new Vector3(-0.009595871f, playerBall.transform.position.y - 1.2f, -0.01231194f);
    }
    
    public void CreateHeptagonShop(GameManagerInGame.GameColors color, int ball)
    {
        GameObject hexagon;
        Vector3 pos;

        var lastObject = lastObjectTransform;

        if (color == GameManagerInGame.GameColors.Blue)
        {
            pos = new Vector3(lastObject.position.x, lastObject.position.y - 0.15f, lastObject.position.z);
            hexagon = Instantiate(heptagons[ball].heptagonPrefabs[0], pos, Quaternion.Euler(lastObject.eulerAngles.x, lastObject.eulerAngles.y - 3, lastObject.eulerAngles.z));
        }
        else if (color == GameManagerInGame.GameColors.Red)
        {
            pos = new Vector3(lastObject.position.x, lastObject.position.y - 0.15f, lastObject.position.z);
            hexagon = Instantiate(heptagons[ball].heptagonPrefabs[1], pos, Quaternion.Euler(lastObject.eulerAngles.x, lastObject.eulerAngles.y - 3, lastObject.eulerAngles.z));
        }
        else
        {
            pos = new Vector3(lastObject.position.x, lastObject.position.y - 0.15f, lastObject.position.z);
            hexagon = Instantiate(heptagons[ball].heptagonPrefabs[2], pos, Quaternion.Euler(lastObject.eulerAngles.x, lastObject.eulerAngles.y - 3, lastObject.eulerAngles.z));
        }

        lastObjectTransform = hexagon.transform;
    }

    public Transform GetLastObject()
    {
        return lastObjectTransform;
    }
}
