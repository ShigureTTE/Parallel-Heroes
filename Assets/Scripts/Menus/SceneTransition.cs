using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneTransition : MonoBehaviour {

    [SerializeField] private ScreenFade screenFade;
    [SerializeField] private float waitTime;
    [SerializeField] private TextMeshProUGUI text;

    private Area nextArea;

    private const string floor = "Floor ";

    private void Start() {

        nextArea = Game.Instance.CurrentArea.nextPossibleAreas.GetRandom();
        if (nextArea.areaName == Game.Instance.CurrentArea.areaName) {
            Game.Instance.FloorNumber++;
        }
        else {
            Game.Instance.FloorNumber = 1;
        }

        Game.Instance.CurrentArea = nextArea;

        text.text = nextArea.areaName.ToString() + '\n' + floor + Game.Instance.FloorNumber.ToString();

        StartCoroutine(SceneChangeCoroutine());
    }

    private IEnumerator SceneChangeCoroutine() {

        yield return new WaitForSecondsRealtime(screenFade.WaitTime);

        yield return new WaitForSecondsRealtime(waitTime);

        screenFade.FadeOut(() => Game.Instance.ChangeScene(Game.Instance.CurrentArea.sceneName, LoadSceneMode.Single));
    }
}
