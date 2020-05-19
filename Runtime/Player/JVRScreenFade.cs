using System.Collections;
using Jichaels.Core;
using UnityEngine;

namespace Jichaels.VRSDK
{

    public class JVRScreenFade : MonoBehaviour
    {

        [SerializeField] private bool fadeInAwake;

        [SerializeField] private FadeEffectSO awakeEffect;

        private Renderer _renderer;

        private GameObject _model;

        private bool _isFading;
        
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {

            _model = transform.GetChild(0).gameObject;
            _renderer = _model.GetComponent<Renderer>();

            if (fadeInAwake)
            {
                FadeEffect(awakeEffect);
            }
        }

        public void FadeEffect(FadeEffectSO fadeEffectSo)
        {
            if (_isFading) return;
            _isFading = true;
            StartCoroutine(FadeEffectCo(fadeEffectSo));
        }

        private IEnumerator FadeEffectCo(FadeEffectSO fadeEffectSo)
        {
            _renderer.material.SetColor(BaseColor, fadeEffectSo.colorA);

            if (fadeEffectSo.delayStart > 0.01f) yield return new WaitForSeconds(fadeEffectSo.delayStart);

            _model.SetActive(true);

            yield return StartCoroutine(Fade(fadeEffectSo.colorA, fadeEffectSo.colorB, fadeEffectSo.durationAB));

            if (fadeEffectSo.fadeInAndOut)
            {
                if (fadeEffectSo.delayAfterB > 0.01f) yield return new WaitForSeconds(fadeEffectSo.delayAfterB);

                yield return StartCoroutine(Fade(fadeEffectSo.colorB, fadeEffectSo.colorC, fadeEffectSo.durationBC));
            }

            if (fadeEffectSo.delayEnd > 0.01f) yield return new WaitForSeconds(fadeEffectSo.delayEnd);

            _model.SetActive(false);

            _isFading = false;
        }

        private IEnumerator Fade(Color a, Color b, float duration)
        {
            Color tmpColor = a;
            float delta = 0;

            while (tmpColor != b)
            {
                delta += Time.deltaTime / duration;
                tmpColor = Color.Lerp(a, b, delta);
                _renderer.material.SetColor(BaseColor, tmpColor);
                yield return Yielders.EndOfFrame;
            }
        }
    }

}