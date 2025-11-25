using UnityEngine;
using UnityEngine.XR;

public class ControllerTracker : MonoBehaviour
{
    public XRNode controllerNode = XRNode.LeftHand;

    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    void Update()
    {
        InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
        InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot);

        Position = pos;
        Rotation = rot;
    }
}
