using UnityEngine;
using System.Collections;

public class TargetAudio : MonoBehaviour
{
    [SerializeField] private TargetCollisionNotes collisionNotes;

    [SerializeField] AudioSource aSrc1;

    [SerializeField] AudioSource aSrc2;

    [SerializeField] AudioClip properCollisionClip;

    [SerializeField] AudioClip improperCollisionClip;


    private bool lastSrcUsedIsOne;
    private float mostRecentClipStart;
    private AudioClip currentlyPlayClip;


    public void OnPlayerHit(int targetIdx, int numTotalTargets, bool hitCorrectly)
    {
        if (hitCorrectly)
        {
            var semiDiff = collisionNotes.GetSemiDiffForTarget(targetIdx, numTotalTargets);

            mostRecentClipStart = Time.time;
            currentlyPlayClip = properCollisionClip;

            aSrc1.pitch = Mathf.Pow(1.05946f, semiDiff);
            aSrc1.volume = .5f;
            aSrc1.PlayOneShot(currentlyPlayClip);

        }
        else
        {
            mostRecentClipStart = Time.time;
            currentlyPlayClip = improperCollisionClip;

            // Eflat at base... so maybe M7?
            aSrc1.pitch = 1; //Random.Range(.95f, 1.05f);
            aSrc1.volume = .25f;
            aSrc1.PlayOneShot(currentlyPlayClip);
        }

    }

    public bool currentlyPlaying => currentlyPlayClip != null 
        && (Time.time - mostRecentClipStart) < currentlyPlayClip.length;
}
