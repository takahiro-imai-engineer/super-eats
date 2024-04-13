using UnityEngine;
using UnityEngine.UI;

public class DebugButton : MonoBehaviour {

    [SerializeField] private Text debugText;
    public void UpdateText (string text) {
        debugText.text = text;
    }
    public void Show () {
        this.gameObject.SetActive (true);
    }
    public void Hide () {
        this.gameObject.SetActive (false);
    }
}