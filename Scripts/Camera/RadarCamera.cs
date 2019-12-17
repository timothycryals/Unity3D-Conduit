using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform targetBlip;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AcquireTarget());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!target)
            return;

        transform.position = new Vector3(target.position.x, target.position.y + 30, target.position.z);
        UpdateBlips();
    }

    void UpdateBlips()
    {
        targetBlip.position = new Vector3(transform.position.x, target.position.y + 10, target.position.z);

        targetBlip.localRotation = Quaternion.Euler(
            new Vector3(targetBlip.localRotation.x, targetBlip.localRotation.y, -target.eulerAngles.y));
    }

    private IEnumerator AcquireTarget()
    {
        while (!target)
        {
            yield return new WaitForSeconds(0.1f);
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
