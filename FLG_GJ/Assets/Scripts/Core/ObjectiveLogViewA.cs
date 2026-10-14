using TMPro;
using UnityEngine;

/// <summary>Fills the pause menu's objectives panel each time it opens.</summary>
public class ObjectiveLogViewA : MonoBehaviour {
    [SerializeField] private TMP_Text body;

    private void OnEnable() {
        if (body != null) body.text = ObjectiveTrackerA.Describe();
    }
}
