using UnityEngine;

namespace SingularBearStudio.Movement
{

    public class FlyingButterfly : MonoBehaviour
    {
        public float speed = 5f;
        public float changeDirectionTime = 2f;
        private Vector2 targetDirection;
        private Vector2 currentDirection;
        private float timer;
        public AnimationCurve movementCurve;
        public BoxCollider2D confinementZone;


        public float minScale = 0.8f;
        public float maxScale = 1.2f;
        public float scaleSpeed = 1f;
        private bool scalingUp = true;

        void Start()
        {
            ChangeDirection();
            currentDirection = targetDirection;
            transform.localScale = Vector3.one * minScale;
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > changeDirectionTime)
            {
                ChangeDirection();
                timer = 0;
            }

            currentDirection = Vector2.Lerp(currentDirection, targetDirection, movementCurve.Evaluate(timer / changeDirectionTime));
            Vector3 newPosition = transform.position + (Vector3)(currentDirection * speed * Time.deltaTime);

            if (!confinementZone.bounds.Contains(newPosition))
            {
                InvertDirection();
            }
            else
            {
                transform.position = newPosition;
            }
            ChangeScale();


        }

        void ChangeDirection()
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            targetDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        void InvertDirection()
        {
            currentDirection = -currentDirection;
            targetDirection = -targetDirection;

        }

        void ChangeScale()
        {
            if (scalingUp)
            {
                transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
                if (transform.localScale.x >= maxScale)
                {
                    scalingUp = false;
                }
            }
            else
            {
                transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
                if (transform.localScale.x <= minScale)
                {
                    scalingUp = true;
                }
            }

        }
    }
}