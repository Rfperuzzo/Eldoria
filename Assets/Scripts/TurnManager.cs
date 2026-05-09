using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public enum TurnState { PlayerTurn, EnemyTurn, Waiting }
    public TurnState currentState { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        currentState = TurnState.PlayerTurn;
        Debug.Log("Turno do Jogador Iniciado");
    }

    public void EndPlayerTurn()
    {
        currentState = TurnState.EnemyTurn;
        Debug.Log("Turno do Jogador Encerrado. Turno do Inimigo (Simulado)...");
        // Simulating enemy turn for now
        Invoke(nameof(StartPlayerTurn), 1f);
    }

    public bool IsPlayerTurn() => currentState == TurnState.PlayerTurn;
}
