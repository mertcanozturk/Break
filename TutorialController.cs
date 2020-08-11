using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialcanvas;
    [SerializeField] private GameObject singletap;
    [SerializeField] private GameObject presshold;

    private GameController _gameController;

    void Start()
    {
        if (GameManagerInGame.Instance.level < 20)
        {
            tutorialcanvas.SetActive(true);
        }

        _gameController = FindObjectOfType<GameController>();
        GameManagerInGame.Instance.SetHealth(2000);
    }

    void Update()
    {
        if (GameManagerInGame.Instance.gameState == 0)
        {
            tutorial();
        }
    }

    IEnumerator FreezeGame()
    {
        float time = 0;
        while (time < 0.7f)
        {
            Time.timeScale = 0.5f;
            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
        }
        Time.timeScale = 1f;
    }

    void BackToGame()
    {
        presshold.SetActive(false);
        GameManagerInGame.Instance.SetHealth(GameManagerInGame.Instance.PlayerBall.GetHealth());
    }


    void tutorial()
    {
        if (_gameController.getbn() == 1)
        {
            singletap.SetActive(true);
            StartCoroutine(FreezeGame());
        }
        else if (_gameController.getbn() == 3)
        {
            singletap.SetActive(false);
            presshold.SetActive(true);
            StartCoroutine(FreezeGame());
        }
        else if (_gameController.getbn() == 17)
        {
            BackToGame();
        }
    }
}
