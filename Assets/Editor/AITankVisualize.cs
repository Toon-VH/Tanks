using Ai_s;
using Ai_s.V_1;
using Tank;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [UnityEditor.CustomEditor(typeof(AIController))]
    public class AITankVisualize : UnityEditor.Editor
    {
        [SerializeField] private AIController _tankController;
        [SerializeField] private AITankData data;


        private void OnEnable()
        {
            _tankController = (AIController)target;
            data = _tankController.data;
        }

        private void OnSceneGUI()
        {
            //FOV
            var position = _tankController.transform.position;
            Handles.color = Color.cyan;
            var viewAngleA = DirFromAngle(-data.ViewAngle / 2, false);
            var viewAngleB = DirFromAngle(data.ViewAngle / 2, false);
            Handles.DrawLine(position, position + viewAngleA * data.ViewRadius);
            Handles.DrawLine(position, position + viewAngleB * data.ViewRadius);
            var forward1 = _tankController.transform.forward;
            Handles.DrawWireArc(position, Vector3.up, forward1, data.ViewAngle / 2, data.ViewRadius);
            Handles.DrawWireArc(position, Vector3.up, forward1, -data.ViewAngle / 2, data.ViewRadius);
            Handles.color = Color.blue;
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, data.AgroRadius);
            //Angle radius
            Handles.color = Color.red;
            Handles.DrawWireArc(position, Vector3.up, forward1, data.TurretRotatingDegree / 2, 1);
            Handles.DrawWireArc(position, Vector3.up, forward1, -data.TurretRotatingDegree / 2, 1);
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += _tankController.transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}