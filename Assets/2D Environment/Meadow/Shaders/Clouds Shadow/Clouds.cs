using UnityEngine;
using System.Collections;

namespace SingularBearStudio.Movement
{

    [RequireComponent(typeof(SpriteRenderer))]

    public class Clouds : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private Vector2 speedMin;
        [SerializeField] private Vector2 speedMax;
        [SerializeField] private AnimationCurve speedCurve;
        [SerializeField] private float opacityMin = 0.0f;
        [SerializeField] private float opacityMax = 1.0f;
        [SerializeField] private Color tintColor = Color.white;
        [SerializeField] private float opacityDuration = 5.0f;

        private Vector2 speed;
        private float opacity;
        private float time;

        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.material = new Material(_spriteRenderer.material);
            _spriteRenderer.material.SetColor("_Color", tintColor);

            speed = new Vector2(
                Random.Range(speedMin.x, speedMax.x),
                Random.Range(speedMin.y, speedMax.y)
            );
            opacity = Random.Range(opacityMin, opacityMax);
            _spriteRenderer.material.SetFloat("_Opacity", opacity);

            StartCoroutine(AnimateOpacity());
        }

        void Update()
        {
            time += Time.deltaTime;
            float curveValue = speedCurve.Evaluate(time % speedCurve.length);
            Vector2 modulatedSpeed = new Vector2(
                Mathf.Lerp(speedMin.x, speedMax.x, curveValue),
                Mathf.Lerp(speedMin.y, speedMax.y, curveValue)
            );
            _spriteRenderer.material.mainTextureOffset += modulatedSpeed * Time.deltaTime;
        }

        public void SetTintColor(Color newColor)
        {
            tintColor = newColor;
            _spriteRenderer.material.SetColor("_Color", tintColor);
        }

        public void SetOpacity(float newOpacity)
        {
            opacity = Mathf.Clamp(newOpacity, opacityMin, opacityMax);
            _spriteRenderer.material.SetFloat("_Opacity", opacity);
        }

        public void SetSpeed(Vector2 newSpeed)
        {
            speed = new Vector2(
                Mathf.Clamp(newSpeed.x, speedMin.x, speedMax.x),
                Mathf.Clamp(newSpeed.y, speedMin.y, speedMax.y)
            );
        }

        private IEnumerator AnimateOpacity()
        {
            while (true)
            {
                float elapsedTime = 0f;
                float startOpacity = opacity;

                while (elapsedTime < opacityDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / opacityDuration;
                    opacity = Mathf.Lerp(startOpacity, opacityMax, t);
                    _spriteRenderer.material.SetFloat("_Opacity", opacity);
                    yield return null;
                }

                float temp = opacityMax;
                opacityMax = opacityMin;
                opacityMin = temp;
            }
        }
    }
}