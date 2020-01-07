using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviourSingleton<Jump> {

    [Header("Jump Animation Settings")]
    [SerializeField] private int jumpAmount;
    [SerializeField] private float characterJumpHeight;
    [SerializeField] private Ease jumpEase;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float waitAfterJump;

    public IEnumerator JumpCoroutine(Transform target, int jumpAmount = 0) {
        if (jumpAmount == 0) {
            jumpAmount = this.jumpAmount;
        }

        int iterator = 0;
        while (iterator < jumpAmount) {
            Tween jump = target.DOLocalMoveY(characterJumpHeight, jumpSpeed).SetEase(jumpEase);
            yield return jump.WaitForCompletion();
            jump = jump = target.DOLocalMoveY(0, jumpSpeed).SetEase(jumpEase);
            yield return jump.WaitForCompletion();
            iterator++;
        }
    }
}
