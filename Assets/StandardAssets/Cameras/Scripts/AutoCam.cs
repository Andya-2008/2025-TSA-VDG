using System;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace UnityStandardAssets.Cameras
{
    [ExecuteInEditMode]
    public class AutoCam : PivotBasedCameraRig
    {
        [SerializeField] private float m_MoveSpeed = 3; // How fast the rig will move to keep up with target's position
        [SerializeField] private float m_TurnSpeed = 1; // How fast the rig will turn to keep up with target's rotation
        [SerializeField] private float m_RollSpeed = 0.2f;// How fast the rig will roll (around Z axis) to match target's roll.
        [SerializeField] private bool m_FollowVelocity = false;// Whether the rig will rotate in the direction of the target's velocity.
        [SerializeField] private bool m_FollowTilt = true; // Whether the rig will tilt (around X axis) with the target.
        [SerializeField] private float m_SpinTurnLimit = 90;// The threshold beyond which the camera stops following the target's rotation. (used in situations where a car spins out, for example)
        [SerializeField] private float m_TargetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
        [SerializeField] private float m_SmoothTurnTime = 0.2f; // the smoothing for the camera's rotatio
        [SerializeField] private float m_VerticalOffset = 3f; // vertical offset from target position
        [SerializeField] private float m_BackwardOffset = 5f; // distance behind the target

        private float m_LastFlatAngle; // The relative angle of the target and the rig from the previous frame.
        private float m_CurrentTurnAmount; // How much to turn the camera
        private float m_TurnSpeedVelocityChange; // The change in the turn speed velocity
        private Vector3 m_RollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )


        protected override void FollowTarget(float deltaTime)
        {
            if (!(deltaTime > 0) || m_Target == null)
                return;

            // --- Determine target forward and up ---
            Vector3 targetForward = m_Target.forward;
            Vector3 targetUp = m_Target.up;

            if (m_FollowVelocity && Application.isPlaying)
            {
                if (targetRigidbody.linearVelocity.magnitude > m_TargetVelocityLowerLimit)
                {
                    targetForward = targetRigidbody.linearVelocity.normalized;
                    targetUp = Vector3.up;
                }
                else
                {
                    targetUp = Vector3.up;
                }
                m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, 1, ref m_TurnSpeedVelocityChange, m_SmoothTurnTime);
            }
            else
            {
                float currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z) * Mathf.Rad2Deg;

                if (m_SpinTurnLimit > 0)
                {
                    float targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(m_LastFlatAngle, currentFlatAngle)) / deltaTime;
                    float desiredTurnAmount = Mathf.InverseLerp(m_SpinTurnLimit, m_SpinTurnLimit * 0.75f, targetSpinSpeed);
                    float turnReactSpeed = (m_CurrentTurnAmount > desiredTurnAmount ? 0.1f : 1f);

                    m_CurrentTurnAmount = Application.isPlaying
                        ? Mathf.SmoothDamp(m_CurrentTurnAmount, desiredTurnAmount, ref m_TurnSpeedVelocityChange, turnReactSpeed)
                        : desiredTurnAmount;
                }
                else
                {
                    m_CurrentTurnAmount = 1f;
                }

                m_LastFlatAngle = currentFlatAngle;
            }

            // --- Calculate desired camera position with backward and vertical offsets ---
            Vector3 desiredPosition = m_Target.position 
                                    - m_Target.forward * m_BackwardOffset // behind target
                                    + Vector3.up * m_VerticalOffset;       // vertical offset
            transform.position = Vector3.Lerp(transform.position, desiredPosition, deltaTime * m_MoveSpeed);

            // --- Calculate rotation ---
            if (!m_FollowTilt)
            {
                targetForward.y = 0;
                if (targetForward.sqrMagnitude < float.Epsilon)
                    targetForward = transform.forward;
            }

            Quaternion rollRotation = Quaternion.LookRotation(targetForward, m_RollUp);
            m_RollUp = m_RollSpeed > 0 ? Vector3.Slerp(m_RollUp, targetUp, m_RollSpeed * deltaTime) : Vector3.up;
            transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, m_TurnSpeed * m_CurrentTurnAmount * deltaTime);
        }
    }
}
