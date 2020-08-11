using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private struct Shape
    {
        public GameManager.GameColors shapeColor;
        public string name;
    }

    #region variables
    [SerializeField] private GameObject[] ballPrefabs;
    [SerializeField] private Material[] materials;
    [SerializeField] private GameObject kristalPrefab;

    [Header("Settings")]
    [SerializeField] private float cameraRotateRate = 0.2f;
    [SerializeField] private int availableColorNumber = 2;
    [SerializeField] private GameObject cameraFollowObject;


    [Header("Cameras")]
    [SerializeField] private GameObject mainCam;

    [Header("UI Fields")]

    [SerializeField] private TextMeshProUGUI HighScoreText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject waitingForStartPanel;
    [SerializeField] private GameObject HighScoreImage;
    [SerializeField] private GameObject playingPanel;
    [SerializeField] private GameObject colorpalet;

    private Transform firstobject;
    private ComboBarControl comboBar;
    private MarketPlace marketPlace;
    private int contscore = 0;
    private int leveltimer = 18;
    private int breaktimer = 0;
    private bool isGamePlaying = false;
    private float progress;
    private int breaknumber = 0;
    private Slider progressBar;

    public bool vibonoff = true;
    public bool firstTap = false;


    #endregion

    private void Awake()
    {
        SetGameMode(PlayerPrefs.GetInt("gm"));
        SetGameLevelMode(PlayerPrefs.GetInt("glm"));
        LoadBall();

        _combabar = FindObjectOfType<ComboBarControl>();
        playingPanel.SetActive(false);
        HighScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
        GameManager.Instance.score = 0;
        scoreText.text = "0";
        GameManager.Instance.gameState = GameManager.GameStates.WaitingForPlay;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.gameState == GameManager.GameStates.Playing) cameraRotateRate += 0.0005f;

        if (cameraRotateRate < 5f) cameraFollowObject.transform.Rotate(0, cameraRotateRate, 0);
        else cameraFollowObject.transform.Rotate(0, 5, 0);
    }

    private void LoadBall()
    {
        Vector3 pos = new Vector3(cameraFollowObject.transform.position.x, cameraFollowObject.transform.position.y + 0.2f, cameraFollowObject.transform.position.z);
        var PlayerBallInstantiate = Instantiate(ballPrefabs[PlayerPrefs.GetInt("SelectedBall")], pos, Quaternion.identity);
        GameManager.Instance.SetPlayerBall(PlayerBallInstantiate.GetComponent<PlayerBall>());
    }

    IEnumerator AddPoint()
    {
        scoreText.GetComponent<Animator>().SetBool("AddPoint", true);
        yield return new WaitForSeconds(0.2f);
        scoreText.GetComponent<Animator>().SetBool("AddPoint", false);
    }

    public int GetAvailableColorNumber()
    {
        return availableColorNumber;
    }

    public void IncreaseScore(int score)
    {
        GameManager.Instance.score += score;
        scoreText.text = GameManager.Instance.score.ToString();
        StartCoroutine(AddPoint());
    }

    private void ReChangeColorHeptagons(GameObject firstObject, int colorId)
    {
        foreach (var heptagon in FindObjectsOfType<Heptagon>())
        {
            if (firstObject.name == heptagon.name)
            {
                heptagon.GetComponent<Heptagon>().ChangeMaterial(GameManager.Instance.PlayerBall.GetCurrentColorId());
            }
            else if (colorId == 2)
            {
                heptagon.GetComponent<Heptagon>().ChangeMaterial(0);
            }
            else
            {
                heptagon.GetComponent<Heptagon>().ChangeMaterial();
            }
        }
    }

    IEnumerator DiamondDelay(Vector3 position)
    {
        yield return new WaitForSeconds(0.2f);
        Instantiate(kristalPrefab, position, Quaternion.Euler(-90, 0, 0));
    }


    List<Shape> remainderShapes = new List<Shape>();

    
    IEnumerator SetDefaultCameraRotate()
    {
        while (cameraRotateRate >= 2f)
        {
            cameraRotateRate -= 0.1f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator Shake()
    {
        float elapsed = 0.0f;

        Vector3 originalCamPos = cameraFollowObject.transform.position;

        while (elapsed < 0.2f)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / 1;
            float damper = 1.0f - Mathf.Clamp(0.5f * percentComplete - 0.0f, 0.0f, 1.0f);

            cameraFollowObject.transform.position = originalCamPos + UnityEngine.Random.insideUnitSphere * 0.1f;

            yield return null;
        }

        cameraFollowObject.transform.position = originalCamPos;
    }

    public int GetMaterialId(Material mat)
    {
        int index = 0;

        foreach (var material in materials)
        {
            if (material.color == mat.color)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    GameManager.GameColors GetRandomColor()
    {
        Array values = Enum.GetValues(typeof(GameManager.GameColors));
        GameManager.GameColors randomColor = (GameManager.GameColors)values.GetValue(UnityEngine.Random.Range(0, availableColorNumber));
        return randomColor;
    }

    public void GameOver()
    {
        contscore = 0;
        GameManager.Instance.score = 0;
        scoreText.text = "0";
        isGamePlaying = false;
        GameManager.Instance.health = GameManager.Instance.PlayerBall.GetHealth();
        WrongBreak();
    }

    public bool WrongImpact(Material mat, GameObject heptagon) // when failed the player
    {
        GameManager.Instance.health--;
        GameManager.Instance.Heart.HPDown();
        if (GameManager.Instance.health == 0)
        {
            GameManager.Instance.AudioManager.gameover.Play();
            contscore = GameManager.Instance.score;
            SetGameState(GameManager.GameStates.WaitingForPlay);
            ReChangeColorHeptagons(heptagon, GetMaterialId(GameManager.Instance.PlayerBall.GetComponent<MeshRenderer>().material));
            GameManager.Instance.PlayerBall.transform.position =
                new Vector3(cameraFollowObject.transform.position.x, cameraFollowObject.transform.position.y + 0.22f, cameraFollowObject.transform.position.z);
            GameManager.Instance.PlayerBall.DisableInput(); // Disable Game Input    
            return true;
        }
        else
        {
            StartCoroutine(Shake());
            return false;
        }
    }

    IEnumerator LerpFromTo(GameObject obj, Vector3 pos1, Vector3 pos2, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            obj.transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            yield return 0;
        }
        obj.transform.position = pos2;
    }

    public void CameraDown()
    {
        Vector3 pos = cameraFollowObject.transform.position;
        var LastObje = GameManager.Instance.HeptagonCreator.GetLastObject();
        Vector3 newPos = new Vector3(LastObje.position.x, LastObje.position.y + 3.57f, LastObje.position.z);
        StartCoroutine(LerpFromTo(cameraFollowObject, pos, newPos, 0.3f));
    }

    public void SetCameraPosition(float value)
    {
        Vector3 pos = cameraFollowObject.transform.position;
        var LastObje = GameManager.Instance.HeptagonCreator.GetLastObject();
        Vector3 newPos = new Vector3(LastObje.position.x, pos.y - value, LastObje.position.z);
        StartCoroutine(LerpFromTo(cameraFollowObject, pos, newPos, 0.3f));
    }

    public void CreateNewDiamond(Vector3 position)
    {
        int rnd = UnityEngine.Random.Range(0, 100);
        position.y += 0.15f;
        if (rnd > 85)
        {
            StartCoroutine(DiamondDelay(position));
        }

    }

    public Material[] GetMaterials()
    {
        return materials;
    }

    public void GoToMarketPlace()
    {
        marketPlace = FindObjectOfType<MarketPlace>();
        marketPlace.GetBallInformations();

        mainCam.transform.localPosition = new Vector3(0, 0.2f, -3.08f);

        Vector3 newBallPos = new Vector3(cameraFollowObject.transform.position.x, cameraFollowObject.transform.position.y - 0.1f, cameraFollowObject.transform.position.x + 0.1f);

        GameManager.Instance.PlayerBall.transform.position = newBallPos;

        GameManager.Instance.gameState = GameManager.GameStates.MarketPlace;
        GameManager.Instance.PlayerBall.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void ProgressValueUp()
    {
        breaknumber++;

        if (GameManager.Instance.level == leveltimer)
        {
            breaktimer++;
        }

        if (GameManager.Instance.gameMode == GameManager.GameMode.Level)
        {
            progressBar.value -= progress;
        }
    }

    private void ResetLevel()
    {
        GameManager.Instance.PlayerBall.combotruefalse();
        breaktimer = 0;
        _combabar.ParticleStop();
        _combabar.ResetSliderValue();
        progressBar.value = 1;
        progress = 1f / GameManager.Instance.level;
    }

    public void WrongBreak()
    {
        leveltimer = 0;
        for (int i = 0; i < breaktimer; i++)
        {
            CreateNewShapes(false);
        }
        leveltimer = 17;
        if (GameManager.Instance.gameMode == GameManager.GameMode.Level)
        {
            ResetLevel();
        }
    }

    public int GetBreakNumber()
    {
        return breaknumber;
    }

    public void LevelUp()
    {
        if (GameManager.Instance.level == 0 || GameManager.Instance.level == 1) { GameManager.Instance.level = 19; }
        GameManager.Instance.level++;
        firstobject = GameManager.Instance.HeptagonCreator.GetLastObject();
        PlayerPrefs.SetInt("level", GameManager.Instance.level);
        leveltimer = 1;
        ResetLevel();
        isGamePlaying = false;
        GameManager.Instance.LevelManager.LevelUpText();
        GameManager.Instance.PlayerBall.ChangeMaterial(GetMaterialId(firstobject.GetComponent<Renderer>().material));
        StopTheGame();
        GameManager.Instance.PlayerBall.SetAutoBreak();
        GameManager.Instance.PlayerBall.transform.position =
                new Vector3(cameraFollowObject.transform.position.x, cameraFollowObject.transform.position.y + 0.22f, cameraFollowObject.transform.position.z);
        firstobject.GetComponent<Renderer>().material = GameManager.Instance.PlayerBall.GetComponent<Renderer>().material;
        GameManager.Instance.PlayerBall.counter = 0;
        waitingForStartPanel.transform.GetChild(3).gameObject.SetActive(false);
    }


    public void SetGameState(GameManager.GameStates state)
    {
        switch (state)
        {
            case GameManager.GameStates.WaitingForPlay:
                {
                    StopTheGame();
                    break;
                }
            case GameManager.GameStates.Playing:
                {
                    StartTheGame();
                    break;
                }
            case GameManager.GameStates.MarketPlace:
                {
                    GoToMarketPlace();
                    break;
                }
            default: break;
        }
    }

    public void TapToPlay()
    {
        firstTap = true;
        SetGameState(GameManager.GameStates.Playing);
    }

    public void StartTheGame()
    {
        mainCam.transform.localPosition = new Vector3(0, 0.65f, -3.08f);

        if (GameManager.Instance.gameMode == GameManager.GameMode.Level)
        {
            scoreText.rectTransform.anchoredPosition = new Vector2(165, 191);
            progressBar.gameObject.SetActive(true);
        }
        else GameManager.Instance.PlayerBall.combotruefalse();


        if (GameManager.GameLevelMode.Classic == GameManager.Instance.gameLevelMode) colorpalet.SetActive(false);
        else
        {
            availableColorNumber = 3;
            colorpalet.SetActive(true);
        }

        GameManager.Instance.AudioManager.background.volume = 0.2f;
        GameManager.Instance.gameState = GameManager.GameStates.Playing;
        GameManager.Instance.PlayerBall.GetComponent<Rigidbody>().isKinematic = false;
        GameManager.Instance.PlayerBall.EnableInput();
        mainCam.SetActive(true);
        scoreText.gameObject.SetActive(true);
        waitingForStartPanel.SetActive(false);
        playingPanel.SetActive(true);

        if (PlayerPrefs.GetInt("HighScore") < GameManager.Instance.score)
        {
            PlayerPrefs.SetInt("HighScore", GameManager.Instance.score);
        }

        GameManager.Instance.Heart.CreateHP();

    }

    public void HeptagonOptimization()
    {
        var hept = FindObjectsOfType<Heptagon>();
        var ball = FindObjectOfType<PlayerBall>();
        if (GameManager.Instance.gameState != GameManager.GameStates.MarketPlace)
            GameManager.Instance.HeptagonCreator.GameDefaultPos();
        else
            GameManager.Instance.HeptagonCreator.DefaultPos();
        for (int j = 0; j < hept.Length; j++)
        {
            Destroy(hept[j].gameObject);
            ShopHeptagon(ball.BallID);
        }
    }

    public void StopTheGame()
    {
        GameManager.Instance.AudioManager.background.volume = 0.7f;
        mainCam.SetActive(true);
        scoreText.gameObject.SetActive(true);
        waitingForStartPanel.SetActive(true);
        waitingForStartPanel.transform.GetChild(0).gameObject.SetActive(false);
        waitingForStartPanel.transform.GetChild(1).gameObject.SetActive(true);
        waitingForStartPanel.transform.GetChild(2).gameObject.SetActive(true);

        if (isGamePlaying)
            waitingForStartPanel.transform.GetChild(3).gameObject.SetActive(false);
        else
            waitingForStartPanel.transform.GetChild(3).gameObject.SetActive(true);

        GameManager.Instance.gameState = GameManager.GameStates.WaitingForPlay;
        GameManager.Instance.PlayerBall.GetComponent<Rigidbody>().isKinematic = true;
        playingPanel.SetActive(false);
        switch (GameManager.Instance.gameMode)
        {
            case GameManager.GameMode.Level:
                {
                    scoreText.rectTransform.anchoredPosition = new Vector2(0, -215);
                    break;
                }
            case GameManager.GameMode.Endless:
                {

                    break;
                }
        }
        if (GameManager.Instance.score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", GameManager.Instance.score);
            HighScoreText.text = GameManager.Instance.score.ToString();
        }
        availableColorNumber = 2;

        StartCoroutine(SetDefaultCameraRotate()); //set default cameraRotateRate
    }

    public void BackToGame()
    {
        GameManager.Instance.SetPlayerBall(FindObjectOfType<PlayerBall>());
        Destroy(GameManager.Instance.PlayerBall.gameObject);
        var PlayerBallInstantiate = Instantiate(ballPrefabs[PlayerPrefs.GetInt("SelectedBall")]).gameObject.GetComponent<PlayerBall>();
        GameManager.Instance.SetPlayerBall(PlayerBallInstantiate);

        Vector3 newBallPos = new Vector3(cameraFollowObject.transform.position.x, cameraFollowObject.transform.position.y + 0.2f, cameraFollowObject.transform.position.x);

        GameManager.Instance.PlayerBall.transform.position = newBallPos;

        GameManager.Instance.gameState = GameManager.GameStates.WaitingForPlay;
        GameManager.Instance.PlayerBall.GetComponent<Rigidbody>().isKinematic = true;
        waitingForStartPanel.SetActive(true);
        HighScoreImage.SetActive(true);
        HighScoreText.gameObject.SetActive(true);
        playingPanel.SetActive(false);
        scoreText.gameObject.SetActive(false);
    }

    public void CreateNewShapes(bool autobreak)
    {
        switch (GameManager.Instance.gameMode)
        {
            case GameManager.GameMode.Level:
                {
                    if (GameManager.GameLevelMode.Classic == GameManager.Instance.gameLevelMode)
                    {
                        if (GameManager.Instance.level - leveltimer > 0)
                        {
                            leveltimer++;
                            if (remainderShapes.Count == 0)
                            {
                                int rnd = UnityEngine.Random.Range(0, 100);
                                if (rnd > 90)                                            // %20 3-15 red or blue 
                                {                                                       // %80 single shape

                                    int rndRate = UnityEngine.Random.Range(0, 10);

                                    if (rndRate < 9)
                                    {

                                        rnd = UnityEngine.Random.Range(3, 10);
                                    }
                                    else
                                    {
                                        rnd = UnityEngine.Random.Range(10, 15);
                                    }

                                    GameManager.GameColors color = GetRandomColor();

                                    for (int i = 0; i < rnd; i++)
                                    {
                                        Shape shape;
                                        shape.shapeColor = color;
                                        shape.name = "Hexagon";
                                        remainderShapes.Add(shape);
                                    }
                                    GameManager.Instance.HeptagonCreator.CreateHexagon(color);
                                }
                                else
                                {
                                    GameManager.Instance.HeptagonCreator.CreateHexagon(GetRandomColor());
                                }
                            }
                            else
                            {
                                GameManager.Instance.HeptagonCreator.CreateHexagon(remainderShapes[0].shapeColor);
                                remainderShapes.RemoveAt(0);
                            }
                        }
                        else
                        {
                            if (autobreak)
                                SetCameraPosition(0.25f);
                            else
                                SetCameraPosition(0.15f);
                        }
                    }
                    if (GameManager.GameLevelMode.Hard == GameManager.Instance.gameLevelMode)
                    {
                        if (GameManager.Instance.level - leveltimer > 0)
                        {
                            leveltimer++;
                            if (remainderShapes.Count == 0)
                            {
                                int rnd = UnityEngine.Random.Range(0, 100);
                                if (rnd > 80)                                            // %20 3-15 red or blue 
                                {                                                       // %80 single shape

                                    int rndRate = UnityEngine.Random.Range(0, 10);

                                    if (rndRate < 9)
                                    {
                                        rnd = UnityEngine.Random.Range(3, 10);
                                    }
                                    else
                                    {
                                        rnd = UnityEngine.Random.Range(10, 15);
                                    }

                                    GameManager.GameColors color = GetRandomColor();

                                    for (int i = 0; i < rnd; i++)
                                    {
                                        Shape shape;
                                        shape.shapeColor = color;
                                        shape.name = "Hexagon";
                                        remainderShapes.Add(shape);
                                    }
                                    GameManager.Instance.HeptagonCreator.CreateHexagon(color);
                                }
                                else
                                {
                                    GameManager.GameColors color = GetRandomColor();

                                    for (int i = 0; i < 1; i++)
                                    {
                                        Shape shape;
                                        shape.shapeColor = color;
                                        shape.name = "Hexagon";
                                        remainderShapes.Add(shape);
                                    }
                                    GameManager.Instance.HeptagonCreator.CreateHexagon(color);
                                }
                            }
                            else
                            {
                                GameManager.Instance.HeptagonCreator.CreateHexagon(remainderShapes[0].shapeColor);
                                remainderShapes.RemoveAt(0);
                            }
                        }
                        else
                        {
                            if (autobreak)
                                SetCameraPosition(0.25f);
                            else
                                SetCameraPosition(0.15f);
                        }
                    }
                    break;
                }
            case GameManager.GameMode.Endless:
                {
                    if (GameManager.Instance.gameLevelMode == GameManager.GameLevelMode.Classic)
                    {
                        if (remainderShapes.Count == 0)
                        {
                            int rnd = UnityEngine.Random.Range(0, 100);
                            if (rnd > 90)                                            // %20 3-15 red or blue 
                            {                                                       // %80 single shape

                                int rndRate = UnityEngine.Random.Range(0, 10);

                                if (rndRate < 9)
                                {

                                    rnd = UnityEngine.Random.Range(3, 10);
                                }
                                else
                                {
                                    rnd = UnityEngine.Random.Range(10, 15);
                                }

                                GameManager.GameColors color = GetRandomColor();

                                for (int i = 0; i < rnd; i++)
                                {
                                    Shape shape;
                                    shape.shapeColor = color;
                                    shape.name = "Hexagon";
                                    remainderShapes.Add(shape);
                                }
                                GameManager.Instance.HeptagonCreator.CreateHexagon(color);
                            }
                            else
                            {
                                GameManager.Instance.HeptagonCreator.CreateHexagon(GetRandomColor());
                            }
                        }
                        else
                        {
                            GameManager.Instance.HeptagonCreator.CreateHexagon(remainderShapes[0].shapeColor);
                            remainderShapes.RemoveAt(0);
                        }

                    }
                    else
                    {
                        if (remainderShapes.Count == 0)
                        {
                            int rnd = UnityEngine.Random.Range(0, 100);
                            if (rnd > 70)                                            // %20 3-15 red or blue 
                            {                                                       // %80 single shape

                                int rndRate = UnityEngine.Random.Range(0, 10);

                                if (rndRate < 9)
                                {

                                    rnd = UnityEngine.Random.Range(3, 10);
                                }
                                else
                                {
                                    rnd = UnityEngine.Random.Range(10, 15);
                                }

                                GameManager.GameColors color = GetRandomColor();

                                for (int i = 0; i < rnd; i++)
                                {
                                    Shape shape;
                                    shape.shapeColor = color;
                                    shape.name = "Hexagon";
                                    remainderShapes.Add(shape);
                                }
                                GameManager.Instance.HeptagonCreator.CreateHexagon(color);
                            }
                            else
                            {
                                int scRnd = UnityEngine.Random.Range(0, 5);
                                if (scRnd == 0)
                                {
                                    GameManager.Instance.HeptagonCreator.CreateHexagon(GetRandomColor());
                                }
                                else
                                {
                                    GameManager.GameColors color = GetRandomColor();

                                    for (int i = 0; i < 1; i++)
                                    {
                                        Shape shape;
                                        shape.shapeColor = color;
                                        shape.name = "Hexagon";
                                        remainderShapes.Add(shape);
                                    }
                                    GameManager.Instance.HeptagonCreator.CreateHexagon(color);
                                }

                            }
                        }
                        else
                        {
                            GameManager.Instance.HeptagonCreator.CreateHexagon(remainderShapes[0].shapeColor);
                            remainderShapes.RemoveAt(0);
                        }

                    }
                    break;
                }
        }
    }

    public void ShopHeptagon(int ball)
    {
        if (remainderShapes.Count == 0)
        {
            int rnd = UnityEngine.Random.Range(0, 100);
            if (rnd > 90)
            {
                int rndRate = UnityEngine.Random.Range(0, 10);

                if (rndRate < 9)
                {

                    rnd = UnityEngine.Random.Range(3, 10);
                }
                else
                {
                    rnd = UnityEngine.Random.Range(10, 15);
                }

                GameManager.GameColors color = GetRandomColor();

                for (int i = 0; i < rnd; i++)
                {
                    Shape shape;
                    shape.shapeColor = color;
                    shape.name = "Hexagon";
                    remainderShapes.Add(shape);
                }
                GameManager.Instance.HeptagonCreator.CreateHeptagonShop(color, ball);
            }
            else
            {
                GameManager.Instance.HeptagonCreator.CreateHeptagonShop(GetRandomColor(), ball);
            }
        }
        else
        {
            GameManager.Instance.HeptagonCreator.CreateHeptagonShop(remainderShapes[0].shapeColor, ball);
            remainderShapes.RemoveAt(0);
        }
    }
}

