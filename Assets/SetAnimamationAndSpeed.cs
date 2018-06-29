using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SetAnimamationAndSpeed : MonoBehaviour
{
    private Animator animator;

    public float speed = 1;

    public string initAnimation;
    public string animationToggle
    {
        set
        {
            animator.SetBool(value, !animator.GetBool(value));
        }
    }
	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        animator.speed = speed;
        animationToggle = initAnimation;
	}
}
