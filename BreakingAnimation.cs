using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingAnimation : MonoBehaviour
{
    private Animator animation;

    private void Start()
    {
        animation = GetComponent<Animator>();
        animation.SetBool("Start", true);
    }

}
