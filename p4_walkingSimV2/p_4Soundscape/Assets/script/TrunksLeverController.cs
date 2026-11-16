using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunksLeverController : MonoBehaviour
{
    [SerializeField] private Transform leverBase;
    [SerializeField] private Transform leverArm;
    [SerializeField] private Animator leverAnimator;
    [SerializeField] private Camera interactionCamera;
    [SerializeField, Min(0.1f)] private float interactionDistance = 5f;
    [SerializeField, Min(0.01f)] private float transitionDuration = 1f;
    [SerializeField] private string activateTrigger = "Inactive";
    [SerializeField] private string deactivateTrigger = "Active";

    private SawbladeSpin[] sawbladeSpins;
    private AudioSource[] sawbladeAudioSources;
    private float[] originalAudioVolumes;
    private float currentIntensity = 1f;
    private bool activated;
    private bool transitioning;

    private void Awake()
    {
        if (interactionCamera == null)
        {
            interactionCamera = Camera.main;
        }
    }

    private void Start()
    {
        if (leverBase == null || leverArm == null)
        {
            Debug.LogError("TrunksLeverController requires LeverBase and LeverArm references.", this);
            enabled = false;
            return;
        }

        if (leverAnimator == null)
        {
            Debug.LogError("TrunksLeverController requires a LeverArm Animator reference.", this);
            enabled = false;
            return;
        }

        if (interactionCamera == null)
        {
            Debug.LogError("TrunksLeverController requires an interaction camera or a MainCamera-tagged camera.", this);
            enabled = false;
            return;
        }

        if (!HasTrigger(leverAnimator, activateTrigger) || !HasTrigger(leverAnimator, deactivateTrigger))
        {
            Debug.LogError(
                $"The LeverArm Animator requires Trigger parameters named '{activateTrigger}' and '{deactivateTrigger}'.",
                leverAnimator);
            enabled = false;
            return;
        }

        if (!EnsureRaycastCollider(leverBase) || !EnsureRaycastCollider(leverArm))
        {
            Debug.LogError("TrunksLeverController requires raycast colliders on LeverBase and LeverArm.", this);
            enabled = false;
            return;
        }

        InitializeSawblades();
    }

    private static bool HasTrigger(Animator animator, string triggerName)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == triggerName && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (transitioning || !Input.GetMouseButtonDown(0))
        {
            return;
        }

        Ray ray = interactionCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance);
        Array.Sort(hits, (first, second) => first.distance.CompareTo(second.distance));

        Transform player = interactionCamera.transform.root;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                continue;
            }

            bool hitBase = hit.transform == leverBase || hit.transform.IsChildOf(leverBase);
            bool hitArm = hit.transform == leverArm || hit.transform.IsChildOf(leverArm);
            if (hitBase || hitArm)
            {
                StartCoroutine(TransitionMachinery(!activated));
            }
            else
            {
                Debug.Log($"Trunks lever click hit '{hit.collider.name}' instead of the active lever part.", this);
            }

            return;
        }
    }

    private static bool EnsureRaycastCollider(Transform target)
    {
        if (target.GetComponentInChildren<Collider>(true) != null)
        {
            return true;
        }

        foreach (MeshFilter meshFilter in target.GetComponentsInChildren<MeshFilter>(true))
        {
            if (meshFilter.sharedMesh != null)
            {
                meshFilter.gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
            }
        }

        return target.GetComponentInChildren<Collider>(true) != null;
    }

    private void InitializeSawblades()
    {
        sawbladeSpins = FindObjectsByType<SawbladeSpin>(FindObjectsSortMode.None);
        HashSet<AudioSource> audioSources = new HashSet<AudioSource>();

        foreach (SawbladeSpin spin in sawbladeSpins)
        {
            if (spin.transform.parent != null)
            {
                audioSources.UnionWith(spin.transform.parent.GetComponentsInChildren<AudioSource>(true));
            }
        }

        sawbladeAudioSources = new AudioSource[audioSources.Count];
        audioSources.CopyTo(sawbladeAudioSources);
        originalAudioVolumes = new float[sawbladeAudioSources.Length];

        for (int i = 0; i < sawbladeAudioSources.Length; i++)
        {
            originalAudioVolumes[i] = sawbladeAudioSources[i].volume;
        }
    }

    private IEnumerator TransitionMachinery(bool activate)
    {
        transitioning = true;
        activated = activate;

        leverAnimator.ResetTrigger(activateTrigger);
        leverAnimator.ResetTrigger(deactivateTrigger);
        leverAnimator.SetTrigger(activate ? activateTrigger : deactivateTrigger);

        if (!activate)
        {
            foreach (AudioSource audioSource in sawbladeAudioSources)
            {
                audioSource.UnPause();
            }
        }

        float startIntensity = currentIntensity;
        float targetIntensity = activate ? 0f : 1f;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed / transitionDuration);
            ApplyMachineryIntensity();
            yield return null;
        }

        currentIntensity = targetIntensity;
        ApplyMachineryIntensity();

        if (activate)
        {
            foreach (AudioSource audioSource in sawbladeAudioSources)
            {
                audioSource.Pause();
            }
        }

        transitioning = false;
    }

    private void ApplyMachineryIntensity()
    {
        foreach (SawbladeSpin spin in sawbladeSpins)
        {
            spin.SetSpeedMultiplier(currentIntensity);
        }

        for (int i = 0; i < sawbladeAudioSources.Length; i++)
        {
            sawbladeAudioSources[i].volume = originalAudioVolumes[i] * currentIntensity;
        }
    }
}
