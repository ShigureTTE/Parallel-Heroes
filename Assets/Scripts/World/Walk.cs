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
    [SerializeField] private float smoothing;

    void Start() {
        if (Game.Instance.State == GameState.Walk) {
            party.transform.position = partyPosition;
            StartCoroutine(StartWalking());
        }
    }

    private IEnumerator StartWalking() {
        List<Tween> characterTweens = new List<Tween>();

        for (int i = 0; i < party.characters.Count; i++) {
            float xOffset = i * characterSpacing;
            Tween t = party.characters[i].transform.DOLocalMove(new Vector3(xOffset, 0 , 0), tweenDuration).SetEase(tweenEase);
            characterTweens.Add(t);
        }

        yield return characterTweens[0].WaitForCompletion();

        yield return null;
    }

}
