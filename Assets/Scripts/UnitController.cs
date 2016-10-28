using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class UnitController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;

    private AudioSource audioSource;

    public UnitDefinition unit;

    public Vector2 selectedTarget;

    public AudioClip clickedSound;

    public AudioClip attackSound;

    public AudioClip damagedSound;

    public AudioClip movingSound;

    public AudioClip dyingSound;


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void move()
    {
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}