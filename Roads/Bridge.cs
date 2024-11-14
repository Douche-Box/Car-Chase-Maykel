using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Bridge : MonoBehaviour
{
    Animator _animator;

    public bool isOpened;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Open and closes the bridge also makes sure the CarAgent will stop
    /// </summary>
    /// <param name="open"></param>
    public void ToggleBridge(bool open)
    {
        _animator.SetBool("Bridge", open);
        if (!open && isOpened)
        {
            StartCoroutine(CloseBridge());
        }
        else
        {
            isOpened = true;
        }
    }

    IEnumerator CloseBridge()
    {
        yield return new WaitForSeconds(3);
        isOpened = false;
    }
}
