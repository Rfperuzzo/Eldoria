using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TurnHudView : MonoBehaviour
{
    [SerializeField] private Text turnText;

    private TurnManager.TurnState? lastState;

    private void Awake()
    {
        if (turnText == null)
        {
            turnText = GetComponent<Text>();
        }

        Refresh(force: true);
    }

    private void Update()
    {
        Refresh(force: false);
    }

    private void Refresh(bool force)
    {
        if (turnText == null)
        {
            return;
        }

        TurnManager turnManager = TurnManager.Instance;
        if (turnManager == null)
        {
            if (force || turnText.text != "Turno: --")
            {
                turnText.text = "Turno: --";
            }

            lastState = null;
            return;
        }

        TurnManager.TurnState state = turnManager.currentState;
        if (!force && lastState == state)
        {
            return;
        }

        turnText.text = state == TurnManager.TurnState.EnemyTurn ? "Turno: Inimigos" : "Turno: Her\u00f3i";
        lastState = state;
    }
}
