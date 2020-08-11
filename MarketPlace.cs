using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class MarketPlace : MonoBehaviour
{
    GameObject mainball;

    [SerializeField] List<GameObject> ballPrefabs;

    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button prevButton;
    [SerializeField]
    private Button buyButton;

    [SerializeField]
    private TextMeshProUGUI name;
    [SerializeField]
    private TextMeshProUGUI ballFeatures;
    [SerializeField]
    private TextMeshProUGUI diamond;

    [SerializeField]
    private AudioSource selected;
    [SerializeField]
    private AudioSource notdiamond;

    public GameController _gameController;

    [SerializeField]
    private TextMeshProUGUI notEnoughDiamondsText;


    int ballIndex;

    void Start()
    {
        _gameController = FindObjectOfType<GameController>();
        mainball = GameManager.Instance.PlayerBall;
        ballIndex = PlayerPrefs.GetInt("SelectedBall");
        GetBallInformations();
        nextButton.onClick.AddListener(() => NextBall(+1));
        prevButton.onClick.AddListener(() => NextBall(-1));
        buyButton.onClick.AddListener(() => Buy());
    }

    public void NextBall(int ball)
    {

        DestroyBalls();
        ballIndex = ballIndex + ball;
        if (ballIndex == ballPrefabs.Count)
        {
            ballIndex = 0;
        }
        if (ballIndex < 0)
        {
            ballIndex = ballPrefabs.Count - 1;
        }

        Vector3 pos = mainball.transform.position;

        mainball = Instantiate(ballPrefabs[ballIndex], pos, Quaternion.identity);

        var hept = FindObjectsOfType<Heptagon>();
        GameManagerInGame.Instance.HeptagonCreator.DefaultPos();
        for (int j = 0; j < hept.Length; j++)
        {
            Destroy(hept[j].gameObject);
            _gameController.ShopHeptagon(mainball.GetComponent<PlayerBall>().BallID);
        }

        GetBallsInformation();
    }

    public void ResetHeptagon()
    {
        var hept = FindObjectsOfType<Heptagon>();
        GameManagerInGame.Instance.HeptagonCreator.GameDefaultPos();
        for (int j = 0; j < hept.Length; j++)
        {
            Destroy(hept[j].gameObject);
            _gameController.ShopHeptagon(GameManagerInGame.Instance.SelectBallId);
        }
    }

    private void DestroyBalls()
    {
        foreach (var item in FindObjectsOfType<PlayerBall>())
        {
            Destroy(item.gameObject);
        }
    }

    public void GetBallsInformation()
    {
        mainball = FindObjectOfType<PlayerBall>().gameObject;
        buyButton.GetComponentInChildren<TextMeshProUGUI>().text = mainball.GetComponent<PlayerBall>().GetPrice().ToString();
        hp = mainball.GetComponent<PlayerBall>().GetHealth().ToString();
        jp = (mainball.GetComponent<PlayerBall>().GetJumpHeight() - 6).ToString();
        ct = mainball.GetComponent<PlayerBall>().GetComboNumber().ToString();
        name.GetComponent<TextMeshProUGUI>().text = mainball.GetComponent<PlayerBall>().GetName();
        CanBuy();

        if (ballIndex == PlayerPrefs.GetInt("SelectedBall"))
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
            
           
        }
        else if (PlayerPrefs.GetInt("Ball" + ballIndex) == 1)
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
        }
    }

    bool CanBuy()
    {
        int total = Int32.Parse(diamond.text);
        int price = mainball.GetComponent<PlayerBall>().GetPrice();


        if (total < price)
        {

            return false;
        }
        return true;
    }

    void Buy()
    {
        if (CanBuy() && PlayerPrefs.GetInt("Ball" + ballIndex) == 0)
        {
            int total = Int32.Parse(diamond.text);
            int price = mainball.GetComponent<PlayerBall>().GetPrice();
            diamond.text = (total - price).ToString();
            GameManagerInGame.Instance.SetDiamond(Convert.ToInt32(diamond.text));
            PlayerPrefs.SetInt("diamond", PlayerPrefs.GetInt("diamond") - price);
            PlayerPrefs.SetInt("Ball" + ballIndex, 1);
            PlayerPrefs.SetInt("SelectedBall", ballIndex);


        }
        else if (PlayerPrefs.GetInt("Ball" + ballIndex) == 0)
        {
            notdiamond.Play();
            StartCoroutine(NotEnoughDiamonds());
        }
        else
        {
            selected.Play();
            PlayerPrefs.SetInt("SelectedBall", ballIndex);
            GameManagerInGame.Instance.SelectBallId = mainball.GetComponent<PlayerBall>().BallID;
            var hept = FindObjectsOfType<Heptagon>();
            GameManagerInGame.Instance.HeptagonCreator.DefaultPos();
            for (int j = 0; j < hept.Length; j++)
            {
                Destroy(hept[j].gameObject);
                _gameController.ShopHeptagon(mainball.GetComponent<PlayerBall>().BallID);
            }
        }
        GetBallInformations();
    }


    IEnumerator NotEnoughDiamonds()
    {
        notEnoughDiamondsText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        notEnoughDiamondsText.gameObject.SetActive(false);
    }

}