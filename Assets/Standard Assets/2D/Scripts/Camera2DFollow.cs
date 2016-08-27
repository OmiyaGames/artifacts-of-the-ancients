using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
        public float verticalMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        // Use this for initialization
        private void Start()
        {
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
            m_LookAheadPos = m_LastTargetPosition;
        }


        // Update is called once per frame
        private void FixedUpdate()
        {
            // only update lookahead pos if accelerating or changed direction
            Vector3 moveDelta = (target.position - m_LastTargetPosition);
            bool adjustCamera = false;

            if (Mathf.Abs(moveDelta.x) > lookAheadMoveThreshold)
            {
                m_LookAheadPos.x = target.position.x + (lookAheadFactor * Mathf.Sign(moveDelta.x));
                adjustCamera = true;
            }
            if (Mathf.Abs(moveDelta.y) > verticalMoveThreshold)
            {
                m_LookAheadPos.y = target.position.y;
                adjustCamera = true;
            }
            //else
            //{
            //    m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            //}
            if (adjustCamera == true)
            {
                Vector3 aheadTargetPos = m_LookAheadPos + Vector3.forward * m_OffsetZ;
                Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

                transform.position = newPos;
                m_LastTargetPosition = target.position;
            }
        }
    }
}
