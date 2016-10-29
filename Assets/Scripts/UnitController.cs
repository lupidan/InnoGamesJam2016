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
    IPointerDownHandler,
    IPointerClickHandler,
    IPointerUpHandler,
    ISelectHandler,
    IDeselectHandler
{
    public static UnitController SelectedUnit { get; private set; }


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

    public AudioClip tombAppearSound;


    public enum UnitAnimationEvents
    {
        StartMoving,
        StopMoving,
        StartAttacking,
        StartDying
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        unitData = new Unit();
    }

    IEnumerator ExecuteActionAfterTime(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
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
        if (clickedSound) {
            audioSource.clip = clickedSound;
            audioSource.Play();
        }
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

    private void playSound(AudioClip audio)
    {
        audioSource.clip = audio;
        audioSource.Play();
    }

    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }

    public void OnPointerClick(PointerEventData eventData)
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        playClickedSound();
    }

    public void OnSelect(BaseEventData eventData) {
        UnitController.SelectedUnit = this;
        LeanTween.value(gameObject, Color.white, Color.green, 0.5f)
                 .setLoopPingPong()
                 .setEaseInOutCubic()
                 .setOnUpdate((color) => {
                     GetComponent<SpriteRenderer>().color = color;
                 });
    }

    public void OnDeselect(BaseEventData eventData) {
        LeanTween.cancel(gameObject);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;

        if (UnitController.SelectedUnit == this && TileController.HighlightedTile != null) {
            Vector3 pos = UnitController.SelectedUnit.transform.position;
            pos.x = TileController.HighlightedTile.transform.position.x;
            pos.y = TileController.HighlightedTile.transform.position.y;
            UnitController.SelectedUnit.transform.position = pos;
        }
    }

    public void PlayMoveAnimation(Position toPosition, Action onFinished)
    {
        animator.SetTrigger(UnitAnimationEvents.StartMoving.ToString());
        Vector3 newPosition = transform.position;
        newPosition.x = toPosition.x;
        newPosition.y = toPosition.y;
        LeanTween.move(gameObject, newPosition, 0.2f)
                 .setEaseLinear()
                 .setOnComplete(() => {
                     transform.position = newPosition;
                     animator.SetTrigger(UnitAnimationEvents.StopMoving.ToString());
                     onFinished.Invoke();
                 });
    }

    public void PlayRotateAnimation(Unit.Direction toDirection, Action onFinished)
    {
        Debug.LogError("☜(ﾟヮﾟ☜) (" + unitData.unitId + ")");
        onFinished();
    }

    public void PlayAttackAnimation(Position targetPosition, Action onFinished)
    {
        animator.SetTrigger(UnitAnimationEvents.StartAttacking.ToString());
        StartCoroutine(ExecuteActionAfterTime(onFinished, 1.0f));
    }

    public void PlayHitpointChange(int newHitpoints, Action onFinished)
    {
        Debug.LogError("(╯°□°）╯︵ ┻━┻ (" + unitData.unitId + ")");
        onFinished();
    }

    public void PlayDeathAnimation(Action onFinished)
    {
        animator.SetTrigger(UnitAnimationEvents.StartDying.ToString());
        StartCoroutine(ExecuteActionAfterTime(onFinished, 1.0f));
    }

}