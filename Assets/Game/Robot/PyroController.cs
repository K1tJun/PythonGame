using DG.Tweening;
using UnityEngine;

public class PyroController : MonoBehaviour
{
    [Header("Tests")]
    public float moveOnPick;
    public float twistedUp;
    public float speedOnPick;
    public float speedTwistedUp;
    public bool twistedIsUp = false;


    [Space(20)]
    [Header("Settings")]
    public float moveSpeed = 0.4f;
    public float rotateSpeed = 0.3f;


    [Space(20)]
    [Header("Components")]
    public Transform pyro;
    public Transform twisted;

    public bool IsBusy = false;



    private Transform cubeForward;


    public void Move(PyroCommand.MoveDirection dir)
    {
        if(dir == PyroCommand.MoveDirection.forward)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Vector3 target = pyro.position + transform.forward;

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

            Vector3 target = pyro.position - transform.forward;

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


    public void PickAndDrop(PyroCommand.pickAndDropDirection dir)
    {
        if (IsBusy)
            return;

        if (dir == PyroCommand.pickAndDropDirection.up && !twistedIsUp)
            PickBox();
        else if(dir == PyroCommand.pickAndDropDirection.drop && twistedIsUp)
            DropBox();
    }

    [ContextMenu("TestPickUP")]
    public void PickBox()
    {
        twistedIsUp = true;
        IsBusy = true;

        Vector3 target = pyro.position + transform.forward * moveOnPick;

        pyro.DOMove(target, speedOnPick)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if(cubeForward != null)
                    cubeForward.parent = twisted;
                target = twisted.position + twisted.up * twistedUp;

                twisted.DOMove(target, speedTwistedUp)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        target = pyro.position - transform.forward * moveOnPick;

                        pyro.DOMove(target, speedOnPick)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                IsBusy = false;
                            });
                    });
            });

    }

    [ContextMenu("TestDropBox")]
    public void DropBox()
    {
        twistedIsUp = false;
        IsBusy = true;

        Vector3 target = pyro.position + transform.forward * moveOnPick;

        pyro.DOMove(target, speedOnPick)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                target = twisted.position - twisted.up * twistedUp;

                twisted.DOMove(target, speedTwistedUp)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        if (cubeForward != null)
                            cubeForward.parent = null;

                        target = pyro.position - transform.forward * moveOnPick;

                        pyro.DOMove(target, speedOnPick)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                IsBusy = false;
                            });
                    });
            });

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("box"))
            cubeForward = other.transform;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform == cubeForward && other.transform.parent != twisted.transform)
            cubeForward = null;
    }
}
