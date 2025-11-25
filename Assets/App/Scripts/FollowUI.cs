using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUI : MonoBehaviour
{
    public enum FollowMode
    {
        FollowUser,
        FollowObject
    }

    [Header("Settings")]
    [SerializeField] private FollowMode mode = FollowMode.FollowUser;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    [Header("Anchor Mode")]
    [SerializeField] private Transform targetAnchor;

    [Header("User Mode")]
    [SerializeField] private bool lockPitch = false;

    [Header("Follow Improvements")]
    [SerializeField] private float rotationLerp = 8f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float maxDistance = 1.4f;

    private Transform mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        switch (mode)
        {
            case FollowMode.FollowUser:
                FollowUser();
                break;

            case FollowMode.FollowObject:
                FollowObject();
                break;
        }
    }

    private void FollowUser()
    {
        if (mainCamera == null) return;

        Vector3 flatForward = mainCamera.forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        Vector3 targetPos =
            mainCamera.position +
            (mainCamera.right * offset.x) +
            (flatForward * offset.z);

        targetPos.y = mainCamera.position.y + offset.y;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * 6f
        );

    Vector3 lookDir = transform.position - mainCamera.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRot = Quaternion.LookRotation(lookDir)
                                    * Quaternion.Euler(rotationOffset);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRot,
                Time.deltaTime * rotationLerp
            );
        }
    }

    private void FollowObject()
    {
        if (targetAnchor == null) return;
        transform.position = targetAnchor.position + targetAnchor.rotation * offset;
        transform.rotation = targetAnchor.rotation * Quaternion.Euler(rotationOffset);
    }
}
