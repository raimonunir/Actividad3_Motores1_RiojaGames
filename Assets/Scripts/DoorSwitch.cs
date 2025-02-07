using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    [SerializeField] private int idDoorSwitch;
    [SerializeField][Range(1f, 3f)] private float switchActivatedPause;

    private Animator animator;
    private bool isActive = true;
    private AudioSource audioSource;


    public int IdDoorSwitch { get => idDoorSwitch;}
    public bool IsSwitchActive { get => isActive;}

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ActivateAnimation()
    {
        if (isActive)
        {
            isActive = false; // Desactiva inmediatamente el switch para evitar múltiples llamadas
            StartCoroutine(AnimateSwitch());
        }
    }

    private IEnumerator AnimateSwitch()
    {
        isActive = false;
        animator.SetTrigger("TriggerOpen");
        yield return new WaitForSeconds(switchActivatedPause);
        animator.SetTrigger("TriggerClose");
        yield return new WaitForSeconds(switchActivatedPause);
        isActive = true;
    }

    public void PlaySound()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
