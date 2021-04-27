using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    public BezierSpline spline;

    public float duration;
    public float offset;

    public bool lookForward;
    public bool useLocalPosition = true;

    public SplineWalkerMode mode;

    public bool useFixedUpdate = false;
    
    public float progress;
    private bool goingForward = true;

    private Rigidbody body;

    void Start()
    {
		      progress = offset / duration;
        body = GetComponent<Rigidbody>();

        if(body == null)
        {
            useFixedUpdate = false;
        }

        if (spline != null)
        {
            if (useLocalPosition)
            {
                transform.localPosition = spline.GetPoint(progress);
            }
            else
            {
                transform.position = spline.GetPoint(progress);
            }
        }
    }

	   private void Update ()
    {
        if(useFixedUpdate)
        {
            return;
        }


        if (goingForward)
        {
            progress += Time.deltaTime / duration;

            if (progress > 1f)
            {
                if (mode == SplineWalkerMode.Once)
                {
                    progress = 1f;
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        }
        else
        {
            progress -= Time.deltaTime / duration;

            if (progress < 0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }

        if (spline != null)
        {
            Vector3 position = spline.GetPoint(progress);

            if (useLocalPosition)
            {
                transform.localPosition = position;
            }
            else
            {
                transform.position = position;
            }

            if (lookForward)
            {
                transform.LookAt(position + spline.GetDirection(progress));
            }
        }
	   }
    
    private void FixedUpdate()
    {
        if (!useFixedUpdate)
        {
            return;
        }

        if (goingForward)
        {
            progress += Time.deltaTime / duration;

            if (progress > 1f)
            {
                if (mode == SplineWalkerMode.Once)
                {
                    progress = 1f;
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        }
        else
        {
            progress -= Time.deltaTime / duration;

            if (progress < 0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }

        if (spline != null)
        {
            Vector3 position = spline.GetPoint(progress);

            if (useLocalPosition)
            {
                transform.localPosition = position;
            }
            else
            {
                body.MovePosition(position);
            }

            if (lookForward)
            {
                body.MoveRotation(Quaternion.LookRotation(spline.GetDirection(progress)));
                //transform.LookAt(position + spline.GetDirection(progress));
            }
        }
    }
}