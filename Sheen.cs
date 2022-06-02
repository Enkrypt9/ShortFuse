using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheen : MonoBehaviour
{
    [SerializeField] float animateTime;
    [SerializeField] private float animateTimer;

    private Animator animator;

    public bool pulsed;

    // Start is called before the first frame update
    void Start()
    {
        pulsed = false;
        animator = GetComponent<Animator>();
        animateTimer = animateTime;
    }

    // Update is called once per frame
    void Update()
    {
        animateTimer -= Time.deltaTime;

        if (animateTimer <= 0f)
        {
            if (pulsed)
            {
                animator.SetBool("ActivateSheen", true);
            }

            animateTimer = animateTime;
        }
    }

    public void ResetAnim()
    {
        pulsed = true;
        animator.SetBool("ActivateSheen", false);
        animator.SetBool("ActivatePulse", false);
    }
}
