using UnityEngine;

namespace BLBits
{
    public class GenericIKController : MonoBehaviour
    {
        [Range(0.0f,1.0f)]
        public float weight;
        public Transform headBone;
        public Transform lookTarget;
        public Vector3 turnAxis;
        public Vector3 tiltAxis;
        public Vector3 lookOffset = Vector3.zero;

        public bool automateWeight = false;
        public float weightAutomationSpeed = 1.0f;

        public float maxHeadTurn = 30.0f;
        public float maxHeadTilt = 15.0f;

        //public float headTurnLerpSpeed = 2.5f;
        //public float headTurnLerpSpeedIgnoreState = 10.0f;

        public bool objectUsesAnimation = true;

        public Animator targetAnimator;
        public string[] ignoreStates;

        private Quaternion targetRotation = Quaternion.identity;
        private Quaternion startHeadRotation;

        private float targetWeight = 0;

        protected void Start()
        {
            maxHeadTurn = Mathf.Min(Mathf.Abs(maxHeadTurn), 180);
            maxHeadTilt = Mathf.Min(Mathf.Abs(maxHeadTilt), 180);

            if (headBone)
            {
                startHeadRotation = headBone.localRotation;
            }

            if (targetAnimator == null)
            {
                targetAnimator = GetComponent<Animator>();
            }

            targetWeight = weight;
        }

        void LateUpdate()
        {
            if(automateWeight && targetWeight != weight)
            {
                weight = Mathf.MoveTowards(weight, targetWeight, weightAutomationSpeed * Time.deltaTime);
            }

            if (headBone)
            {
                bool shouldIgnore = false;

                if (!objectUsesAnimation)
                {
                    headBone.localRotation = startHeadRotation;
                }

                if (targetAnimator != null && ignoreStates != null)
                {
                    for (int i = 0; i < ignoreStates.Length; i++)
                    {
                        if (targetAnimator.GetCurrentAnimatorStateInfo(0).IsName(ignoreStates[i]) || targetAnimator.GetNextAnimatorStateInfo(0).IsName(ignoreStates[i]))
                        {
                            shouldIgnore = true;
                            if(automateWeight)
                            {
                                targetWeight = 0;
                            }
                            break;
                        }
                    }
                }

                if(automateWeight && shouldIgnore == false)
                {
                    targetWeight = lookTarget != null ? 1 : 0;
                }

                if (lookTarget != null && shouldIgnore == false)
                {
                    Vector3 headRight = headBone.TransformDirection(tiltAxis);
                    Vector3 headUp = headBone.TransformDirection(turnAxis);
                    Vector3 headForward = Vector3.Cross(headRight, headUp).normalized;

                    Vector3 targetPos = lookTarget.transform.position + lookOffset;

                    Vector3 lookDirection = (targetPos - headBone.position);

                    float turnAngle = Mathf.Clamp(Vector3.SignedAngle(Vector3.ProjectOnPlane(lookDirection, headUp), headForward, -headUp), -maxHeadTurn, maxHeadTurn);
                    float tiltAngle = Mathf.Clamp(Vector3.SignedAngle(Vector3.ProjectOnPlane(lookDirection, headRight), headForward, -headRight), -maxHeadTilt, maxHeadTilt);

                    Quaternion targetTurn = Quaternion.AngleAxis(turnAngle, turnAxis);
                    Quaternion targetTilt = Quaternion.AngleAxis(tiltAngle, tiltAxis);

                    targetRotation = headBone.rotation * targetTilt * targetTurn;
                }
                /*else if (shouldIgnore)
                {
                    targetRotation = Quaternion.Lerp(targetRotation, headBone.rotation, headTurnLerpSpeedIgnoreState * Time.deltaTime);
                    Quaternion baseRotation = objectUsesAnimation ? headBone.rotation : startHeadRotation;
                    if (Quaternion.Angle(targetRotation, baseRotation) < 5)
                    {
                        targetRotation = baseRotation;
                    }
                    headBone.rotation = targetRotation;
                }
                else
                {
                    targetRotation = Quaternion.Lerp(targetRotation, headBone.rotation, headTurnLerpSpeed * Time.deltaTime);
                    headBone.rotation = targetRotation;
                }*/

                headBone.rotation = Quaternion.Lerp(headBone.rotation, targetRotation, weight);
            }
        }

        public void SetLookTarget(Transform target)
        {
            lookTarget = target;
            if(automateWeight)
            {
                targetWeight = lookTarget != null ? 1 : 0;
            }
        }

        public void SnapToTarget()
        {
            if (!objectUsesAnimation)
            {
                headBone.localRotation = startHeadRotation;
            }

            if (lookTarget != null)
            {
                Vector3 headRight = headBone.TransformDirection(tiltAxis);
                Vector3 headUp = headBone.TransformDirection(turnAxis);
                Vector3 headForward = Vector3.Cross(headRight, headUp).normalized;

                Vector3 targetPos = lookTarget.transform.position + lookOffset;

                Vector3 lookDirection = (targetPos - headBone.position);

                float turnAngle = Mathf.Clamp(Vector3.SignedAngle(Vector3.ProjectOnPlane(lookDirection, headUp), headForward, -headUp), -maxHeadTurn, maxHeadTurn);
                float tiltAngle = Mathf.Clamp(Vector3.SignedAngle(Vector3.ProjectOnPlane(lookDirection, headRight), headForward, -headRight), -maxHeadTilt, maxHeadTilt);

                Quaternion targetTurn = Quaternion.AngleAxis(turnAngle, turnAxis);
                Quaternion targetTilt = Quaternion.AngleAxis(tiltAngle, tiltAxis);

                targetRotation = headBone.rotation * targetTilt * targetTurn;
                headBone.rotation = targetRotation;
            }
            else
            {
                targetRotation = headBone.transform.rotation;
                headBone.rotation = targetRotation;
            }
        }

        public void OnDrawGizmosSelected()
        {
            if (headBone)
            {
                Gizmos.color = Color.green;
                Vector3 turn = headBone.TransformDirection(turnAxis).normalized;
                Gizmos.DrawLine(headBone.position, headBone.position + turn * 2.5f);

                Gizmos.color = Color.red;
                Vector3 tilt = headBone.TransformDirection(tiltAxis).normalized;
                Gizmos.DrawLine(headBone.position, headBone.position + tilt * 2.5f);

                Gizmos.color = Color.blue;
                Vector3 forward = Vector3.Cross(headBone.TransformDirection(tiltAxis), headBone.TransformDirection(turnAxis)).normalized;
                Gizmos.DrawLine(headBone.position, headBone.position + forward * 2.5f);
            }
        }
    }
}