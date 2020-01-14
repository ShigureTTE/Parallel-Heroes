using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Walk : MonoBehaviour {

    [Header("Party Settings")]
    [SerializeField] private Vector3 partyPosition;
    [SerializeField] private float characterSpacing;
    [SerializeField] private float tweenDuration;
    [SerializeField] private Ease tweenEase;
    [SerializeField] private Tweener exploreMenu;

    [Header("Walking Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float smoothing;
    [SerializeField] private float smoothTime;
    [SerializeField] private Ease cameraEase;

    [Header("Jump Animation Settings")]
    [SerializeField] private int jumpAmount;
    [SerializeField] private float characterJumpHeight;
    [SerializeField] private Ease jumpEase;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float waitAfterJump;

    [Header("Rotate Animation Settings")]
    [SerializeField] private Ease rotateEase;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float waitAfterRotate;

    [Header("Encounter Animation Settings")]
    [SerializeField] private float scaleSpeed;
    [SerializeField] private Ease scaleEase;
    [SerializeField] private float maxHeightExclamation;
    [SerializeField] private float exclamationMoveSpeed;
    [SerializeField] private float waitInBetweenScale;

    private bool walking = false;
    private bool readyToExplore = false;
    private const string characterWalkKey = "CharacterWalk";
    private const string characterIdleKey = "Idle";
    private Party party;
    private Transform cameraContainer;

    void Start() {
        party = Game.Instance.PlayerParty;
        cameraContainer = Game.Instance.CameraContainer.transform;
        party.transform.position = partyPosition;
    }

    private IEnumerator SetToExplorePosition() {
        readyToExplore = true;

        if (Game.Instance.CameraContainer) Game.Instance.CameraContainer.GetComponent<Tweener>().PlayTweenReversed();
        exploreMenu.PlayTween();

        List<Tween> characterTweens = new List<Tween>();

        for (int i = 0; i < party.characters.Count; i++) {
            float xOffset = i * characterSpacing;
            Tween t = party.characters[i].transform.DOLocalMove(new Vector3(xOffset, 0 , 0), tweenDuration).SetEase(tweenEase);
            characterTweens.Add(t);
        }

        yield return characterTweens[0].WaitForCompletion();
    }

    public void StartWalking() {
        StartCoroutine(StartWalkingCoroutine());
    }

    private IEnumerator StartWalkingCoroutine() {       
        Transform leader = party.characters[0].transform;

        Game.Instance.SetWalking();

        yield return StartCoroutine(Jump.Instance.JumpCoroutine(leader));

        yield return new WaitForSecondsRealtime(waitAfterJump);

        Tween cameraTween = cameraContainer.transform.DOMove(new Vector3(cameraContainer.position.x + smoothing, cameraContainer.position.y, cameraContainer.position.z), smoothTime).SetEase(cameraEase);
        walking = true;

        foreach (CharacterBase character in party.characters) {
            Animator anim = character.GetComponentInChildren<Animator>();
            anim.Play(characterWalkKey);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    private IEnumerator StopWalkingCoroutine() {
        Tween cameraTween = cameraContainer.DOMove(new Vector3(cameraContainer.position.x - smoothing, cameraContainer.position.y, cameraContainer.position.z), smoothTime).SetEase(cameraEase);

        foreach (CharacterBase character in party.characters) {
            Animator anim = character.GetComponentInChildren<Animator>();
            anim.Play(character.stats.characterName + characterIdleKey);
            yield return null;
            anim.GetComponent<RandomAnimationFrame>().RandomFrame();
            yield return new WaitForSecondsRealtime(0.05f);
        }

        yield return new WaitForSecondsRealtime(waitAfterRotate);

        party.Exclamation.DOLocalMoveY(party.Exclamation.localPosition.y + maxHeightExclamation, exclamationMoveSpeed).SetEase(Ease.Linear).OnComplete(() => {
            party.Exclamation.DOLocalMoveY(party.Exclamation.localPosition.y - maxHeightExclamation, 0);
        });

        Tween exclamationScale = party.Exclamation.DOScaleX(1, scaleSpeed).SetEase(scaleEase);
        yield return exclamationScale.WaitForCompletion();

        yield return new WaitForSecondsRealtime(waitInBetweenScale);

        exclamationScale = party.Exclamation.DOScaleX(0, scaleSpeed).SetEase(scaleEase);
        yield return exclamationScale.WaitForCompletion();

        yield return StartCoroutine(Jump.Instance.JumpCoroutine(party.characters[0].transform));

        yield return new WaitForSecondsRealtime(waitAfterJump);

        Game.Instance.ProcessEncounter();
    }

    private void Update() {
        if (Game.Instance.State == GameState.Walk && walking) {
            transform.position += Vector3.left * walkSpeed * Time.deltaTime;
        }

        if (Game.Instance.State != GameState.Walk && walking) {
            walking = false;
            readyToExplore = false;
            StartCoroutine(StopWalkingCoroutine());
        }

        if (Game.Instance.State == GameState.Walk && !walking && !readyToExplore) {
            StartCoroutine(SetToExplorePosition());
        }
    }

}
