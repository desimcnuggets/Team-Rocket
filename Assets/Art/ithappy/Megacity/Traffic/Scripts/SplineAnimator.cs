namespace ITHappy
{
    using UnityEngine;
    using UnityEngine.Splines;

    public class SplineAnimator : MonoBehaviour
    {
        [SerializeField]
        private SplineContainer m_Container;

        [SerializeField]
        private bool m_IsApplyRotation = true;
        [SerializeField, Range(0, 1)]
        private float m_Time;
        [SerializeField]
        private int m_Spline;
        
        public Vector3 positionOffset;

        public bool snapToSurface = true;
        public LayerMask surfaceLayer = 1;

        private void OnValidate()
        {
            if (!m_Container)
                return;

            m_Spline = Mathf.Clamp(m_Spline, 0, m_Container.Splines.Count);
            EvaluateTransform();
        }

        private void Update()
        {
            if (!m_Container)
                return;

            EvaluateTransform();
        }

        private void EvaluateTransform()
        {
            m_Container.Evaluate(m_Spline, m_Time, out var pos, out var tan, out var up);

            Vector3 finalPos = (Vector3)pos;
            Vector3 finalUp = (Vector3)up;

            if (snapToSurface)
            {
                RaycastHit[] hits = Physics.RaycastAll(finalPos + Vector3.up * 20f, Vector3.down, 50f, surfaceLayer);
                foreach (var hit in hits)
                {
                     if (hit.collider.transform != transform && hit.collider.transform.root != transform.root)
                     {
                          finalPos = hit.point;
                          finalUp = hit.normal;
                          break;
                     }
                }
            }

            transform.position = finalPos;
            if (m_IsApplyRotation)
            {
                transform.up = finalUp;
                transform.forward = tan;
            }
            transform.position += transform.TransformDirection(positionOffset);
        }
    }
}