using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionDelay : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private float delay;

    private void Start()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        source.Play();
    }
}
