using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEncounter : MonoBehaviour, IEncounter {

    [SerializeField] private RandomCharacter randomCharacter;
    [SerializeField] private Tweener doorTweener;
    [SerializeField] private int jumpAmount;
    [SerializeField] private float jumpStrength;
    [SerializeField] private float outOfCellDistance;
    [SerializeField] private float outOfCellSpeed;
    [SerializeField] private float waitAfterAction;

    public void Encounter() {
        StartCoroutine(EncounterCoroutine());
    }

    public IEnumerator EncounterCoroutine() {
        GameObject character = randomCharacter.SpawnCharacter();
        character.transform.parent = Game.Instance.PlayerParty.transform;
        doorTweener.PlayTween();

        yield return StartCoroutine(Jump.Instance.JumpCoroutine(character.transform, jumpAmount));
       
        Tween outOfCell = character.transform.DOLocalJump(new Vector3(character.transform.localPosition.x, 0 , character.transform.localPosition.z - outOfCellDistance), jumpStrength, 1, outOfCellSpeed).SetEase(Ease.InOutQuart);

        yield return outOfCell.WaitForCompletion();

        yield return new WaitForSecondsRealtime(waitAfterAction);

        BattleSystem.Instance.SetupPartyPlayer();
        BattleSystem.Instance.UpdateStats();
        LevelGenerator.Instance.Regenerate();
        Game.Instance.SetWalking();
    }
}
