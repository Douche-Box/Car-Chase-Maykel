using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class SteeringWheelInteractable : XRBaseInteractable
{
    [SerializeField] Transform _wheelTransform;

    [SerializeField] float _currentAngle;

    public UnityEvent<float> onTurning;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected && isHovered)
            {
                RotateWheel();
            }
        }
    }

    /// <summary>
    /// Rotate the wheels based on the steering wheel
    /// </summary>
    private void RotateWheel()
    {
        float totalAngle = FindWheelAngle();

        float angleDifference = _currentAngle - totalAngle;

        _currentAngle = totalAngle;
        _wheelTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, totalAngle));
        onTurning?.Invoke(angleDifference);
    }

    /// <summary>
    /// Find the agle of the steering wheel
    /// </summary>
    /// <returns></returns>
    private float FindWheelAngle()
    {
        float totalAngle = 0;

        foreach (IXRSelectInteractor interactor in interactorsSelecting)
        {
            Vector2 direction = FindLocalPoint(interactor.transform.position);
            totalAngle += ConvertToAngle(direction) * FindRotationSensitivity();
        }

        return totalAngle;
    }

    private Vector2 FindLocalPoint(Vector3 position)
    {
        return transform.InverseTransformPoint(position).normalized;
    }

    private float ConvertToAngle(Vector2 direction)
    {
        return Vector2.SignedAngle(Vector3.up, direction);
    }

    private float FindRotationSensitivity()
    {
        return 1.0f / interactorsSelecting.Count;
    }
}
