using TMPro;
using UnityEngine;

public class QuestTextA : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] TextMeshProUGUI questtext;
    public void setQuest(string text) {
        questtext.text = text;
    }
}
