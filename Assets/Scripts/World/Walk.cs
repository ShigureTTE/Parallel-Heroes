using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Walk : MonoBehaviour {

    [Header("Party Settings")]
    [SerializeField] private Party party;
    [SerializeField] private Vector3 partyPosition;
    [SerializeField] private float characterSpacing;
    [SerializeField] private float tweenDuration;
    [SerializeField] private Ease tweenEase;

    [Header("Walking Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private float smoothing;
    [SerializeField] private float smoothTime;

    [Header("Animation Settings")]
    [SerializeField] private int jumpAmount;
    [SerializeField] private float characterJumpHeight;
    [SerializeField] private Ease jumpEase;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float waitAfterJump;

    private bool walking = false;
    private const string characterWalk = "CharacterWalk";

    void Start() {
        if (Game.Instance.State == GameState.Walk) {
            party.transform.position = partyPosition;
            StartCoroutine(SetToExplorePosition());
        }
    }

    private IEnumerator SetToExplorePosition() {
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
        int iterator = 0;
        Transform leader = party.characters[0].transform;
        while (iterator < jumpAmount) {
            Tween jump = leader.DOLocalMove(new Vector3(leader.localPosition.x, characterJumpHeight, leader.localPosition.z), jumpSpeed).SetEase(jumpEase);
            yield return jump.WaitForCompletion();
            jump = jump = leader.DOLocalMove(new Vector3(leader.localPosition.x, 0, leader.localPosition.z), jumpSpeed).SetEase(jumpEase);
            yield return jump.WaitForCompletion();
            iterator++;
        }

        yield return new WaitForSecondsRealtime(waitAfterJump);

        Tween cameraTween = cameraContainer.DOMove(new Vector3(cameraContainer.position.x + smoothing, cameraContainer.position.y, cameraContainer.position.z), smoothTime).SetEase(Ease.Linear);
        walking = true;

        foreach (CharacterBase character in party.characters) {
            Animator anim = character.GetComponentInChildren<Animator>();
            anim.Play(characterWalk);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    private void Update() {
        if (walking) {
            transform.position += Vector3.left * walkSpeed * Time.deltaTime;
        }
    }

}
