using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float shakeTime;
    private float shakeIntensity;
    private bool IsPosPerlin = false;
    private bool IsRotPerlin = false;
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;
        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }
    /// <summary>
    /// ī�޶� ��鸲 ����
    /// </summary>
    /// <param name="shakeTime">ī�޶� ��鸲 ���� �ð�</param>
    /// <param name="shakeIntensity">ī�޶� ��鸲 ����</param>
    public void OnShakeCamera(float shakeTime = 1.0f, float shakeIntensity = 0.1f, bool isPerlin = false)
    {
        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;
        IsPosPerlin = isPerlin;

        StopCoroutine(ShakeByPosition());
        StartCoroutine(ShakeByPosition());
    }
    private IEnumerator ShakeByPosition()
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 shakePos = Vector3.zero;
        //Random.insideUnitSphere = ������ 1 ũ���� �� ���� �� ������ ��ǥ ����
        while (shakeTime > 0.0f)
        {
            float offset = shakeTime * shakeIntensity;
            shakePos.x = Mathf.PerlinNoise(offset, offset * 2f) - 0.5f;
            shakePos.y = Mathf.PerlinNoise(offset, offset * 2f) - 0.5f;
            shakePos.z = Mathf.PerlinNoise(offset, offset * 2f) - 0.5f;

            if (!IsPosPerlin)
                transform.localPosition = startPosition + Random.insideUnitSphere * shakeIntensity;
            else
                transform.localPosition = startPosition + shakePos * shakeIntensity;
            shakeTime -= Time.unscaledDeltaTime;

            yield return null;
        }
        transform.localPosition = startPosition;
    }
    public void OnRotateCamera(float shakeTime = 1.0f, float shakeIntensity = 0.1f, bool isPerlin = false)
    {
        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;
        IsRotPerlin = isPerlin;

        StopCoroutine(ShakeByRotation());
        StartCoroutine(ShakeByRotation());
    }
    private IEnumerator ShakeByRotation()
    {
        Quaternion startRotation = transform.localRotation;

        float power = 10f;

        while (shakeTime > 0.0f)
        {
            float offset = shakeTime * shakeIntensity;
            Vector3 rot = Vector3.zero;
            if (!IsRotPerlin)
                rot.z = Random.Range(-1f, 1f);
            else
                rot.z = Mathf.PerlinNoise(offset, offset * 2f) - 0.5f;
            rot = rot * power * shakeIntensity;
            transform.localRotation = startRotation * Quaternion.Euler(rot);
            shakeTime -= Time.unscaledDeltaTime;

            yield return null;
        }
        transform.localRotation = startRotation;
    }
}
