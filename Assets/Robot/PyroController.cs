using DG.Tweening;
using UnityEngine;

public class PyroController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 0.4f;
    public float rotateSpeed = 0.3f;


    [Space(20)]
    [Header("Components")]
    public Transform pyro;
    public bool IsBusy = false;


    public void Move(PyroCommand.MoveDirection dir)
    {
        if(dir == PyroCommand.MoveDirection.forward)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Vector3 target = pyro.position - transform.up;

            pyro.DOMove(target, moveSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    IsBusy = false;
                });
        }
        else if(dir == PyroCommand.MoveDirection.backward)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Vector3 target = pyro.position + transform.up;

            pyro.DOMove(target, moveSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    IsBusy = false;
                });
        }
    }

    public void Rotate(PyroCommand.RotateDirection dir)
    {
        if (dir == PyroCommand.RotateDirection.right)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            pyro.DORotate(pyro.eulerAngles + new Vector3(0, 90, 0), rotateSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    IsBusy = false;
                });

        }
        else if (dir == PyroCommand.RotateDirection.left)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            pyro.DORotate(pyro.eulerAngles + new Vector3(0, -90, 0), rotateSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    IsBusy = false;
                });
        }
    }
}
