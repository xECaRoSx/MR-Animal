using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.InputSystem;
using System;

public class AnchorManager : MonoBehaviour
{
    public static AnchorManager Instance { get; private set; }

    [Header("Game Component")]
    [SerializeField] private GameObject previewObject;
    [SerializeField] private Transform anchorRoot;

    [Header("Controller Reference")]
    [SerializeField] private Transform leftController;

    [Header("Preview Offset")]
    [SerializeField] private Vector3 previewOffset = new Vector3(0f, 0f, -0.25f);

    private bool providerReady = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartProvider();
        previewObject.SetActive(false);
    }

    private async void StartProvider()
    {
        var result = await PXR_MixedReality.StartSenseDataProvider(PxrSenseDataProviderType.SpatialAnchor);
        Debug.Log($"SpatialAnchor Provider Ready = {result == PxrResult.SUCCESS}");
    }

    private void Update()
    {
        if (previewObject.activeSelf && leftController != null)
        {
            Vector3 offsetWorld = leftController.rotation * previewOffset;
            previewObject.transform.position = leftController.position + offsetWorld;

            Vector3 euler = leftController.rotation.eulerAngles;
            previewObject.transform.rotation = Quaternion.Euler(0f, euler.y, 0f);
        }
    }

    public void EnablePreview(bool enable)
    {
        previewObject.SetActive(enable);
    }

    public void ConfirmAnchor()
    {
        anchorRoot.position = previewObject.transform.position;
        Quaternion fixedRot = previewObject.transform.rotation * Quaternion.Euler(0, 180f, 0);
        anchorRoot.rotation = fixedRot;

        anchorRoot.gameObject.SetActive(true);
        previewObject.SetActive(false);

        Debug.Log("[AnchorManager] Anchor confirmed at controller preview position.");
    }

    public Pose GetAnchorPose()
    {
        return new Pose(anchorRoot.position, anchorRoot.rotation);
    }
}