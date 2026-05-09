using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EndTurnButtonConnector : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(EndPlayerTurn);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(EndPlayerTurn);
        }
    }

    public void EndPlayerTurn()
    {
        if (TurnManager.Instance == null)
        {
            Debug.LogWarning("EndTurnButton pressed, but no TurnManager is available.");
            return;
        }

        TurnManager.Instance.EndPlayerTurn();
    }
}
