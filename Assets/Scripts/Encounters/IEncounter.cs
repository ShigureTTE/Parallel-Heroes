using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEncounter {
    void Encounter();
    IEnumerator EncounterCoroutine();
}
