using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour {

    [SerializeField] private RandomCharacter randomCharacter;
    [SerializeField] private ScreenFade fade;
    [SerializeField] private Area areaObject;
    [SerializeField] private string sceneTransitionName;

    public void StartGame() {
        randomCharacter.SpawnCharacter();
        Game.Instance.PlayerParty.ResetCharacters();
        Game.Instance.CurrentArea = areaObject;

        fade.FadeOut(() => Game.Instance.ChangeScene(sceneTransitionName, UnityEngine.SceneManagement.LoadSceneMode.Single));
    }
}
