using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CMP.Scripts.Shaders
{

    public class DissolveController : MonoBehaviour
    {
        [Header("Initial State")]
        [Tooltip("If true, the object will start fully dissolved (invisible).")]
        public bool startDissolved = false;

        [Header("Animation Settings")]
        [Tooltip("How long the appear animation takes to complete.")]
        public float appearDuration = 1f;
        [Tooltip("How long the dissolve animation takes to complete.")]
        public float dissolveDuration = 1f;
        public AnimationCurve transitionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Shader Settings")]
        [Tooltip("Name of the dissolve float property in the shader")]
        public string dissolveProperty = "_Dissolve";
        
        [Tooltip("Value when fully visible")]
        public float visibleValue = 0f;
        
        [Tooltip("Value when fully dissolved")]
        public float dissolvedValue = 1f;

        // to handle both standard and ui elements
        private Renderer[] _renderers;
        private Graphic[] _graphics;
        
        private MaterialPropertyBlock _mpb;
        private int _dissolveID;
        
        private Coroutine _animationCoroutine;
        private float _currentDissolveValue;

        void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            _graphics = GetComponentsInChildren<Graphic>(true);
            
            _mpb = new MaterialPropertyBlock();
            _dissolveID = Shader.PropertyToID(dissolveProperty);

            _currentDissolveValue = startDissolved ? dissolvedValue : visibleValue;
            SetDissolve(_currentDissolveValue);
        }

        /// <summary>
        /// Fades the object out.
        /// </summary>
        public void Dissolve()
        {
            if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(AnimateShader(_currentDissolveValue, dissolvedValue, dissolveDuration));
        }

        /// <summary>
        /// Fades the object in.
        /// </summary>
        public void Appear()
        {
            if (!gameObject.activeInHierarchy) gameObject.SetActive(true);

            if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(AnimateShader(_currentDissolveValue, visibleValue, appearDuration));
        }
        
        private IEnumerator AnimateShader(float startVal, float targetVal, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
                float curveValue = transitionCurve.Evaluate(normalizedTime);
                
                _currentDissolveValue = Mathf.Lerp(startVal, targetVal, curveValue);
                SetDissolve(_currentDissolveValue);
                
                yield return null; 
            }

            _currentDissolveValue = targetVal;
            SetDissolve(_currentDissolveValue);
        }

        private void SetDissolve(float value)
        {
            // Handle standard renderers using property blocks
            if (_renderers != null && _renderers.Length > 0)
            {
                foreach (var r in _renderers)
                {
                    if (r != null)
                    {
                        r.GetPropertyBlock(_mpb);
                        _mpb.SetFloat(_dissolveID, value);
                        r.SetPropertyBlock(_mpb);
                    }
                }
            }

            // Handle canvas elements using material instances
            if (_graphics != null && _graphics.Length > 0)
            {
                foreach (var graphic in _graphics)
                {
                    if (graphic != null && graphic.material != null)
                    {
                        graphic.material.SetFloat(_dissolveID, value);
                    }
                }
            }
        }
    }
}