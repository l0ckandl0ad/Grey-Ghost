using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DamageableEntityFX : MonoBehaviour
{
    private IDamageable damagebleEntity;
    private VisualEffect vfx;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] audioFiles;

    [SerializeField]
    private Transform floatingTextPrefab;
    private FloatingText floatingText;
    private Color damageTextColor = Color.red;
    private Color repairTextColor = Color.green;

    private void Start()
    {
        vfx = GetComponent<VisualEffect>();
        audioSource = GetComponent<AudioSource>();

        damagebleEntity = GetComponentInParent<IDamageable>();

        if (damagebleEntity != null && vfx != null)
        {
            damagebleEntity.OnHit += PlayHitEffects;
            damagebleEntity.OnRepair += ShowHealthRepair;
        }
        else
        {
            Debug.LogError(this + ": critical components not found!");
        }
    }
    
    private void PlayHitEffects(IDamageable damageableEntity, int damage, int vpValue)
    {
        PlayHitVisualFX(damage);
        ShowHealthChange("-" + damage.ToString(), damageTextColor);
        PlayHitSoundFX(damage);
    }

    private void PlayHitVisualFX(int damage)
    {
        vfx.SetFloat("Strength", damage);
        vfx.Play();
    }

    private void PlayHitSoundFX(int damage)
    {
        audioSource.PlayOneShot(SelectRandomAudioClip(audioFiles), (0.25f * damage) + 0.5f);
    }

    private void ShowHealthChange(string text, Color color)
    {
        floatingText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity).GetComponent<FloatingText>();
        floatingText.Initialize(text, color);
    }

    private void ShowHealthRepair(IDamageable damageableEntity, int amount)
    {
        if (damageableEntity.IsVisible)
        {
            ShowHealthChange("+" + amount.ToString(), repairTextColor);
        }
    }

    private AudioClip SelectRandomAudioClip(AudioClip[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    private void OnDestroy()
    {
        if (damagebleEntity != null)
        {
            damagebleEntity.OnHit -= PlayHitEffects;
            damagebleEntity.OnRepair -= ShowHealthRepair;
        }
    }
}
