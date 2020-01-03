using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationFrame : MonoBehaviour {

	void OnEnable () {
        RandomFrame();
    }

    public void RandomFrame() {
        if (GetComponent<Animator>()) {
            Animator anim = GetComponent<Animator>();
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            anim.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
        }
    }

}
