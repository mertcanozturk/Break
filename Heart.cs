using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField] private GameObject hpPrefab;

    List<GameObject> HPSprite;

    public void CreateHP()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        int TempY = 0;
        HPSprite = new List<GameObject>();
        for (int i = 0; i < GameManagerInGame.Instance.health; i++)
        {
            GameObject hp = Instantiate(hpPrefab, transform);
            hp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, TempY);
            hp.name = i.ToString();
            TempY -= 50;
            HPSprite.Add(hp.gameObject);
        }
    }

    public void HPDown()
    {
        HPSprite[GameManagerInGame.Instance.health].transform.GetChild(0).gameObject.SetActive(false);
    }
}
