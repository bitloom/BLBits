//from Catlike Coding: https://catlikecoding.com/unity/tutorials/curves-and-splines/

using UnityEngine;

namespace BLBits
{
    public class MoveAlongSpline : MonoBehaviour
    {
        public BezierSpline spline;

        [Range(0.0f, 1.0f)]
        public float t = 0;

        public bool lookAlongSpline = false;
        public Vector3 lookOffset;

        public AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public bool clampTime = true;

        public float raiseSpeed = 1;
        public float lowerSpeed = 1;

        public bool loop = false;

        public bool overrideButton = false;
        public bool sendSpeedToAnimator = false;
        public float maxAnimationSpeed = 2.0f;
        public string animatorKey = "Speed";

        private Animator animator;
        private float prevT;

        public bool active = false;

        public void Activate()
        {
            active = true;
        }

        public void Deactivate()
        {
            active = false;
        }

        void Start()
        {
            if (sendSpeedToAnimator)
            {
                animator = GetComponent<Animator>();
            }

            UpdateSplinePosition();
        }

        void Update()
        {
            prevT = t;

            if (active || overrideButton)
            {
                if (!loop)
                {
                    t = Mathf.Clamp01(t + (raiseSpeed * Time.deltaTime));
                }
                else
                {
                    t = Mathf.Repeat(t + (raiseSpeed * Time.deltaTime), 1.0f);
                }

                UpdateSplinePosition();
            }
            else
            {
                t = Mathf.Clamp01(t - (lowerSpeed * Time.deltaTime));

                UpdateSplinePosition();
            }

            if (sendSpeedToAnimator && animator)
            {
                float speed = Mathf.Abs(t - prevT) > 0 ? maxAnimationSpeed : 0;
                animator.SetFloat(animatorKey, speed);
            }
        }

        void UpdateSplinePosition()
        {
            if (spline)
            {
                float curT = easing.Evaluate(t);
                transform.position = spline.GetPoint(curT);

                if (lookAlongSpline)
                {
                    transform.rotation = Quaternion.LookRotation(Quaternion.Euler(lookOffset) * spline.GetDirection(curT));
                }
            }
        }

        public void SwitchSpline(BezierSpline newSpline)
        {
            spline = newSpline;
        }

        public void SetT(float newT)
        {
            t = newT;
        }
    }
}
