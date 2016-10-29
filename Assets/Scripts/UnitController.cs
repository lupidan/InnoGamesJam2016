using System;
using UnityEngine;
using System.Collections;
using Combat.Events;
using UnityEngine.EventSystems;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class UnitController :
    MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private Animator animator;

    private AudioSource audioSource;

    private Position selectedTarget;

    private Position target;

    public Unit unitData;

    public AudioClip clickedSound;

    public AudioClip attackSound;

    public AudioClip damagedSound;

    public AudioClip movingSound;

    public AudioClip dyingSound;


    public enum UnitAnimationEvents
    {
        StartMoving,
        StopMoving,
        StartAttacking,
        StopAttacking,
        StartDying
    }

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        unitData = new Unit();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasArrivedAtDestination())
        {
            //animator.SetBool(UnitAnimationEvents.StopMoving.ToString(), true);
        }
    }

    public Boolean hasArrivedAtDestination()
    {
        return true;
    }

    public void moveTo()
    {
        animator.SetBool("startedMoving", true);
    }


    private void showRange()
    {
        Debug.LogError("Not yet implemented");
    }

    private void showAttackRange()
    {
        Debug.LogError("Not yet implemented");
    }

    private void playClickedSound()
    {
        audioSource.clip = clickedSound;
        audioSource.Play();
    }

    private void startPlayingMovingSound()
    {
        audioSource.clip = movingSound;
        audioSource.Play();
    }

    private void stopPlayingMovingSound()
    {
        audioSource.Stop();
    }

    private void playAttackSound()
    {
        audioSource.clip = attackSound;
        audioSource.Play();
    }

    private void playDamagedSound()
    {
        audioSource.clip = damagedSound;
        audioSource.Play();
    }

    private void playDyingSound()
    {
        audioSource.clip = dyingSound;
        audioSource.Play();
    }

    private void highlightUnit()
    {
        Debug.LogError("Highlighting Unit is not implemented yet");
    }

    private void removeHighlighting()
    {
        Debug.LogError("removing Highliting Unit is not implemented yet");
    }

    private void markSelected()
    {
        Debug.LogError("mark unit selected Unit is not implemented yet");
    }

    private void removeSelection()
    {
        Debug.LogError(" Unit is not implemented yet");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        playClickedSound();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightUnit();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        removeHighlighting();
    }

    public void PlayMoveAnimation(Position toPosition, Action onFinished)
    {
        Debug.LogError("(☞ﾟヮﾟ)☞ (" + unitData.unitId + ")");
        onFinished();
    }

    public void PlayRotateAnimation(Unit.Direction toDirection, Action onFinished)
    {
        Debug.LogError("☜(ﾟヮﾟ☜) (" + unitData.unitId + ")");
        onFinished();
    }

    public void PlayAttackAnimation(Position targetPosition, Action onFinished)
    {
        Debug.LogError("┌(ﾟдﾟ)┘ (" + unitData.unitId + ")");
        animator.SetBool("StartAttacking" , false);
        Debug.LogError(UnitAnimationEvents.StartAttacking.ToString());
        //playAttackSound();
        //onFinished();
    }

    public void PlayHitpointChange(int newHitpoints, Action onFinished)
    {
        Debug.LogError("(╯°□°）╯︵ ┻━┻ (" + unitData.unitId + ")");
        onFinished();
    }

    public void PlayDeathAnimation(Action onFinished)
    {
        Debug.LogError("he ded. (" + unitData.unitId + ")");
        onFinished();
    }

}