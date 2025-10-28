using UnityEngine;

public class TimingPoint : MonoBehaviour
{
    public enum ActionType { Build, Destroy }
    public ActionType actionType;

    [Header("Objeto associado")]
    public GameObject associatedObject; // ponte ou parede relacionada

    [Header("Visual")]
    public Vector3 gizmoSize = new Vector3(2f, 0.2f, 1f);

    private void OnDrawGizmos()
    {
        Color fillColor = actionType == ActionType.Build ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.4f);
        Color lineColor = actionType == ActionType.Build ? Color.green : Color.red;

        Gizmos.color = fillColor;
        Gizmos.DrawCube(transform.position, gizmoSize);

        Gizmos.color = lineColor;
        Gizmos.DrawWireCube(transform.position, gizmoSize);
    }
}

