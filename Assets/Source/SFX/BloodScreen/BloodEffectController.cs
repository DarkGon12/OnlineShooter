using System.Collections;
using UnityEngine;
using UnityEngine.PostProcessing;

public class BloodEffectController : MonoBehaviour
{
    [SerializeField] private PostProcessingBehaviour _profiles;
    [SerializeField] private GameObject[] _bloodsImage;

    [SerializeField] private float pulseSpeed = 4.0f;
    [SerializeField] private float pulseIntensity = 0.2f;
    [SerializeField] private float fadeDuration = 2.0f;

    public int _maxHealth = 100;
    private Coroutine pulseCoroutine;
    private Coroutine fadeCoroutine;

    public void Damage(int healthValue)
    {
        float intensityValue = Mathf.Lerp(0.6f, 0f, (float)healthValue / _maxHealth);

        var profile = _profiles.profile;

        if (profile != null)
        {
            VignetteModel.Settings vignetteSettings = profile.vignette.settings;
            vignetteSettings.intensity = intensityValue;
            profile.vignette.settings = vignetteSettings;

            if (healthValue == 100)
            {
                vignetteSettings.intensity = 0;
                profile.vignette.settings = vignetteSettings;
                StopPulseCoroutine();
                UpdateBloodImages(healthValue);
                return;
            }

            if (healthValue < 50)
            {
                if (pulseCoroutine == null)
                {
                    pulseCoroutine = StartCoroutine(PulseVignette(profile));
                }
            }
            else
            {
                StopPulseCoroutine();
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                fadeCoroutine = StartCoroutine(FadeVignette(profile));
            }
        }

        UpdateBloodImages(healthValue);
    }

    private void StopPulseCoroutine()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
    }

    private void UpdateBloodImages(int healthValue)
    {
        if (healthValue < 50)
        {
            for (int i = 0; i < _bloodsImage.Length; i++)
            {
                _bloodsImage[i].SetActive(true);
                _bloodsImage[i].transform.position = new Vector2(Random.Range(-150f, 150f), Random.Range(-100f, 100f));
            }
        }
        if(healthValue == 100)
        {
            for (int i = 0; i < _bloodsImage.Length; i++)
                _bloodsImage[i].SetActive(false);
        }
    }

    private IEnumerator PulseVignette(PostProcessingProfile profile)
    {
        VignetteModel.Settings vignetteSettings = profile.vignette.settings;
        float baseIntensity = vignetteSettings.intensity;
        float targetIntensity = baseIntensity + pulseIntensity;

        while (true)
        {
            float elapsedTime = 0f;
            while (elapsedTime < pulseSpeed)
            {
                vignetteSettings.intensity = Mathf.Lerp(baseIntensity, targetIntensity, (Mathf.Sin(elapsedTime / pulseSpeed * Mathf.PI * 2) + 1f) / 2f);
                profile.vignette.settings = vignetteSettings;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator FadeVignette(PostProcessingProfile profile)
    {
        VignetteModel.Settings vignetteSettings = profile.vignette.settings;
        vignetteSettings.intensity = 0.35f;
        profile.vignette.settings = vignetteSettings;

        float elapsedTime = 0f;
        float startIntensity = 0.35f;
        float endIntensity = 0f;

        while (elapsedTime < fadeDuration)
        {
            vignetteSettings.intensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / fadeDuration);
            profile.vignette.settings = vignetteSettings;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        vignetteSettings.intensity = endIntensity;
        profile.vignette.settings = vignetteSettings;
    }
}
