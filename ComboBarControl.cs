using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboBarControl : MonoBehaviour
{

    [SerializeField] private GameObject BoomParticle;
    [SerializeField] public Slider ComBoSlider;

    public void SliderValueUp()
    {
        ComBoSlider.value += 0.1f;
    }

    public bool SliderValueDown()
    {
        ComBoSlider.value -= 0.1f;
        if (ComBoSlider.value == ComBoSlider.minValue) return true;
        else return false;
    }

    public void ResetSliderValue()
    {
        ComBoSlider.value = 1.5f;
    }
    public void SliderDown(float combo)
    {
        ComBoSlider.value += combo;
    }
    public void ParticlePlay()
    {
        BoomParticle.SetActive(true);
    }
    public void ParticleStop()
    {
        BoomParticle.SetActive(false);
    }

}
