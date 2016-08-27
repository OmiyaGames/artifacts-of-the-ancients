using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAboveFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private float m_OffsetZ;
        private float m_LookAheadX;
        private float m_LookAheadY;

        // Use this for initialization
        private void Start()
        {
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
        }


        // Update is called once per frame
        private void Update()
        {
            // only update lookahead pos if accelerating or changed direction
            Vector3 moveDelta = (target.position - m_LastTargetPosition);
            if (Mathf.Abs(moveDelta.x) > lookAheadMoveThreshold)
            {
                m_LookAheadX = lookAheadFactor * Mathf.Sign(moveDelta.x);
            }
            if (Mathf.Abs(moveDelta.y) > lookAheadMoveThreshold)
            {
                if(moveDelta.y < 0)
                {
                    m_LookAheadY = 0;
                }
                else
                {
                    m_LookAheadY = lookAboveFactor;
                }
                //m_LookAheadY = lookAboveFactor * Mathf.Sign(moveDelta.y);
            }
            //else
            //{
            //    m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            //}
            Vector3 aheadTargetPos = target.position;
            aheadTargetPos.x += m_LookAheadX;
            aheadTargetPos.y += m_LookAheadY;
            aheadTargetPos.z += m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}
