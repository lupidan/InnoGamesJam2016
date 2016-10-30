using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    IPointerEnterHandler,
    IPointerExitHandler
{
    public static UnitController _selectedUnit = null;

    public static UnitController SelectedUnit
    {
        get { return _selectedUnit; }
        set
        {
            if (_selectedUnit != null)
            {
                LeanTween.cancel(_selectedUnit.gameObject);
                _selectedUnit.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                TileController.SetAllTilesUnreachable();
            }

            if (value != null)
            {
                LeanTween.value(value.gameObject, Color.white, Color.green, 0.25f)
                    .setLoopPingPong()
                    .setEaseOutBack()
                    .setOnUpdate((color) => { value.gameObject.GetComponent<SpriteRenderer>().color = color; });

                List<Position> availablePositions = value.GetMovementPositions();
                TileController.SetTilesAtPositionsReachable(availablePositions);
            }
            _selectedUnit = value;
        }
    }

    private Animator animator;

    private AudioSource audioSource;

    private TileController destination = null;
    private List<Position> pathToDestination = null;
    private List<GameObject> visiblePathObjects = new List<GameObject>();

    public Unit unitData;

    public SpriteRenderer pathObjectPrefab;
    public Sprite pathSprite;

    private ClientGameLogicManager _clientLogic;

    // checks if the death animation has been played to revive them
    private bool isDed = false;

    public enum UnitAnimationEvents
    {
        StartMoving,
        StopMoving,
        StartAttacking,
        StartDying,
        Revive
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        _clientLogic = GameObject.Find("NetworkClient").GetComponent<ClientGameLogicManager>();

        _clientLogic.PlayerMayInteractHandler += OnAnimationsFinished;
    }

    private void OnAnimationsFinished()
    {
        // resync with actual unit state
        LeanTween.value(gameObject, gameObject.transform.position.x, unitData.position.x, 2.0f)
            .setEaseInOutCubic()
            .setOnUpdate((position) =>
            {
                var vect = gameObject.transform.position;
                vect.x = position;
                gameObject.transform.position = vect;
            });
        LeanTween.value(gameObject, gameObject.transform.position.y, unitData.position.y, 2.0f)
            .setEaseInOutCubic()
            .setOnUpdate((position) =>
            {
                var vect = gameObject.transform.position;
                vect.y = position;
                gameObject.transform.position = vect;
            })
            .setOnComplete(() =>
            {
                UpdateHitpointsInTextMesh(unitData.healthPoints);
            });

        if (isDed && unitData.healthPoints > 0) {
            animator.SetTrigger(UnitAnimationEvents.Revive.ToString());
            isDed = false;
        }

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
        audioSource.clip = unitData.Definition.clickedSound;
        audioSource.Play();
    }

    private void startPlayingMovingSound()
    {
        audioSource.clip = unitData.Definition.movingSound;
        audioSource.Play();
    }

    private void playAttackSound()
    {
        audioSource.clip = unitData.Definition.attackSound;
        audioSource.Play();
    }

    private void playDamagedSound()
    {
        audioSource.clip = unitData.Definition.damagedSound;
        audioSource.Play();
    }

    private void playDyingSound()
    {
        audioSource.clip = unitData.Definition.dyingSound;
        audioSource.Play();
    }

    private void PlaySound(string audioFilename)
    {
        SoundController.Instance.PlayAudioClip(audioFilename);
    }

    private void DisplayPoof()
    {
        GameObject gameObject = FindObjectOfType<MapGenerator>().InstantiatePrefab("DeathPoof");
        Vector3 position = this.transform.position;
        position.z -= 0.1f;
        gameObject.transform.position = position;
    }

    private void DisplayTeleport()
    {
        GameObject gameObject = FindObjectOfType<MapGenerator>().InstantiatePrefab("EffectTeleport");
        Vector3 position = this.transform.position;
        position.z -= 0.1f;
        gameObject.transform.position = position;
    }

    private void DisplayHeavyShotRelative()
    {
        GameObject gameObject = FindObjectOfType<MapGenerator>().InstantiatePrefab("HeavyEffect");
        gameObject.transform.SetParent(this.transform);
        Vector3 localPosition = new Vector3(0.9f, 0.05f, -0.1f);
        gameObject.transform.localPosition = localPosition;
    }

    private void DisplayRangeShotRelative()
    {
        GameObject gameObject = FindObjectOfType<MapGenerator>().InstantiatePrefab("RangeEffect");
        gameObject.transform.SetParent(this.transform);
        Vector3 localPosition = new Vector3(0.737f, 0.521f, -0.1f);
        gameObject.transform.localPosition = localPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IngameSubmitButtonManager.GetIngameSubmitButtonManager().IsWaiting()
            && ClientNetworkingManager.GetClientNetworkingManager().PlayerId == unitData.owningPlayerId)
        {
            ClientGameLogicManager logicManager = ClientGameLogicManager.GetClientLogicFromScene();
            if (true)// (logicManager.CurrentServerSideState.CurrentPhase == GamePhase.Planning)
            {
                if (destination == null)
                {
                    SelectedUnit = this;
                }
                else
                {
                    SetDestinationTileController(null);
                }
                playClickedSound();
            }
            else if (logicManager.CurrentServerSideState.CurrentPhase == GamePhase.Revision)
            {
                List<GameAction> gameActions = logicManager.QueuedGameActions;
                for (int i = 0; i < gameActions.Count; i++)
                {
                    if (gameActions[i].UnitId == unitData.unitId) {
                        gameActions[i].moveToPositions.Clear();
                    }
                }
                
            }
            
        }
    }

    public void SetDestinationTileController(TileController tileController)
    {
        destination = tileController;
        if (destination)
        {
            Position current = new Position(unitData.position.x, unitData.position.y);
            Position destiny = new Position(tileController.tileData.position.x,
                tileController.tileData.position.y);
            pathToDestination = TileController.PathFromPositionToPosition(current, destiny);
            _clientLogic.AddQueuedActionForUnitId(unitData.unitId, pathToDestination);
        }
        else
        {
            pathToDestination = null;
            _clientLogic.RemoveQueuedActionForUnitId(unitData.unitId);
        }
        SetupLine();
    }

    private void SetupLine()
    {
        for (int i = visiblePathObjects.Count - 1; i >= 0; i--)
        {
            Destroy(visiblePathObjects[i]);
            visiblePathObjects.RemoveAt(i);
        }

        if (pathToDestination != null && pathToDestination.Count > 0)
        {
            visiblePathObjects = new List<GameObject>();
            for (int i = 0; i < pathToDestination.Count; i++)
            {
                SpriteRenderer renderer = Instantiate(pathObjectPrefab);
                renderer.transform.position = new Vector3(pathToDestination[i].x,
                    pathToDestination[i].y,
                    transform.position.z + 0.001f);
                renderer.sprite = pathSprite;
                visiblePathObjects.Add(renderer.gameObject);
            }
        }
    }

    public void MoveCameraToPoint(Position toPosition, Action onFinished)
    {
        Vector3 newCameraPosition = Camera.main.transform.position;
        newCameraPosition.x = toPosition.x;
        newCameraPosition.y = toPosition.y;
        LeanTween.move(Camera.main.gameObject, newCameraPosition, 0.2f)
            .setEaseLinear()
            .setOnComplete(() => { onFinished.Invoke(); });
    }

    public void PlayMoveAnimation(Position toPosition, Action onFinished)
    {
        float time = 0.2f;
        startPlayingMovingSound();
        animator.SetTrigger(UnitAnimationEvents.StartMoving.ToString());
        Vector3 newPosition = transform.position;
        newPosition.x = toPosition.x;
        newPosition.y = toPosition.y;
        LeanTween.move(gameObject, newPosition, time)
            .setEaseLinear()
            .setOnComplete(() =>
            {
                transform.position = newPosition;
                animator.SetTrigger(UnitAnimationEvents.StopMoving.ToString());
                onFinished.Invoke();
            });

        Vector3 newCameraPosition = Camera.main.transform.position;
        newCameraPosition.x = toPosition.x;
        newCameraPosition.y = toPosition.y;
        LeanTween.move(Camera.main.gameObject, newCameraPosition, time)
            .setEaseLinear();
    }

    public void PlayRotateAnimation(Unit.Direction toDirection, Action onFinished)
    {
        if (toDirection == Unit.Direction.Left)
        {
            Vector3 scale = transform.localScale;
            scale.x = -1.0f;
            transform.localScale = scale;
        }
        else if (toDirection == Unit.Direction.Left)
        {
            Vector3 scale = transform.localScale;
            scale.x = 1.0f;
            transform.localScale = scale;
        }
        onFinished();
    }

    public void PlayAttackAnimation(Position targetPosition, Action onFinished)
    {
        playAttackSound();
        animator.SetTrigger(UnitAnimationEvents.StartAttacking.ToString());
        StartCoroutine(ExecuteActionAfterTime(onFinished, 1.0f));
    }

    public void PlayHitpointChange(int oldHitpoints, int newHitpoints, Action onFinished)
    {
        LeanTween.value(gameObject, Color.white, Color.red, 0.5f)
                 .setLoopOnce()
                 .setEaseInOutCubic()
                 .setOnUpdate((color) => {
                     GetComponent<SpriteRenderer>().color = color;
                 })
                 .setOnComplete(RemoveHitpointColorAnimation);

        LeanTween.value(gameObject, (float)oldHitpoints, (float)newHitpoints, 0.5f)
            .setLoopOnce()
            .setEaseInOutCubic()
            .setOnUpdate((value) =>
            {
                var hitpoints = (int)Math.Round(value);
                UpdateHitpointsInTextMesh(hitpoints);
            })
            .setOnComplete(onFinished);
    }

    private void UpdateHitpointsInTextMesh(int hitpoints)
    {
        if (hitpoints > 0)
        {
            GetComponentInChildren<TextMesh>().text = "" + hitpoints;
        }
        else
        {
            GetComponentInChildren<TextMesh>().text = "";
        }
    }

    private void RemoveHitpointColorAnimation()
    {
        LeanTween.value(gameObject, Color.red, Color.white, 0.1f)
            .setLoopOnce()
            .setEaseInOutCubic()
            .setOnUpdate((color) => { GetComponent<SpriteRenderer>().color = color; });
    }

    public void PlayDeathAnimation(Action onFinished)
    {
        playDyingSound();
        animator.SetTrigger(UnitAnimationEvents.StartDying.ToString());
        StartCoroutine(ExecuteActionAfterTime(onFinished, 1.0f));
        isDed = true;
    }


    private List<Position> GetMovementPositions()
    {
        List<Position> options = new List<Position>();
        int length = unitData.Definition.maxMovements;
        for (int row = unitData.position.y - length; row <= unitData.position.y + length; row++)
        {
            for (int column = unitData.position.x - length; column <= unitData.position.x + length; column++)
            {
                int actualLength = Mathf.Abs(column - unitData.position.x) + Mathf.Abs(row - unitData.position.y);
                if (actualLength <= length && (row != unitData.position.y || column != unitData.position.x))
                    options.Add(new Position(column, row));
            }
        }
        return options;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var attackPatternGameObject = GameObject.Find("AttackPatternRenderer");
        var attackPatternRenderer = attackPatternGameObject.GetComponent<AttackPatternRenderer>();
        attackPatternRenderer.SetPattern(unitData.position, transform,
            unitData.Definition.attackPattern);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var attackPatternGameObject = GameObject.Find("AttackPatternRenderer");
        var attackPatternRenderer = attackPatternGameObject.GetComponent<AttackPatternRenderer>();
        attackPatternRenderer.HidePattern();
    }
}