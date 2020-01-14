using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitEncounter : MonoBehaviour, IEncounter {

    [SerializeField] private Area areaObject;

    public void Encounter() {
        areaObject = Game.Instance.CurrentArea;
        StartCoroutine(EncounterCoroutine());
    }

    public IEnumerator EncounterCoroutine() {
        yield return null;
        ScreenFade.Instance.FadeOut(() => Game.Instance.ChangeScene(areaObject.sceneNames.GetRandom(), LoadSceneMode.Single));
    }

}
