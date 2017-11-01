using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowUpdate : MonoBehaviour {

    public Vector3 pivot = new Vector3();
    public Vector3 transGab = new Vector3();
    public float speed = 1.0f;
    protected Vector3 currentGab = new Vector3();
    protected IEnumerator ptrUpdate = null;
    
    private void OnEnable()
    {
        if(ptrUpdate == null)
        {
            ptrUpdate = CoroutineUpdate();

            StartCoroutine(ptrUpdate);
        }
    }

    private void OnDisable()
    {
        if (ptrUpdate != null)
            StopCoroutine(ptrUpdate);

        ptrUpdate = null;
    }

    private IEnumerator CoroutineUpdate()
    {
        currentGab = Vector3.zero;
        Vector3 goal = transGab;

        yield return null;

        while( gameObject.activeInHierarchy )
        {
            currentGab = Vector3.MoveTowards(currentGab, goal, speed * 0.1f);
            transform.localPosition = pivot + currentGab;

            if(currentGab.sqrMagnitude >= transGab.sqrMagnitude)
            {
                goal = Vector3.zero;
            }
            else if( currentGab.sqrMagnitude <= 0.0f )
            {
                goal = transGab;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
