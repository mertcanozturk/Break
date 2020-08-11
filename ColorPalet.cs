using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPalet : MonoBehaviour
{
    [SerializeField] private Material[] materials;

    [SerializeField] private List<GameObject> uiColorImages;

    Color image1Color;
    Color image2Color;
    Color image3Color;

    [SerializeField] private Image[] images;

    public void ChangeUiColorImages(int currentColorNumber)
    {
        int rightIndex = currentColorNumber + 1;

        int leftIndex = currentColorNumber - 1;

        if (rightIndex >= uiColorImages.Count)
        {
            rightIndex = 0;
        }
        if (leftIndex < 0)
        {
            leftIndex = uiColorImages.Count - 1;
        }

        if (currentColorNumber == 0)
        {
            images[0].color = image3Color;
            images[1].color = image1Color;
            images[2].color = image2Color;
        }
        else if (currentColorNumber == 1)
        {
            images[0].color = image1Color;
            images[1].color = image2Color;
            images[2].color = image3Color;
        }
        else
        {
            images[0].color = image2Color;
            images[1].color = image3Color;
            images[2].color = image1Color;
        }
    }

    public void StartChangeUI()
    {
        image1Color = materials[0].color;
        image2Color = materials[1].color;
        image3Color = materials[2].color;
        ChangeUiColorImages(GameManagerInGame.Instance.PlayerBall.GetCurrentColorId());
    }
}
