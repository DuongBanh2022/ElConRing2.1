using UnityEngine;

public class Pot : MonoBehaviour
{
    [Header("Break Settings")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip breakClip;
    [SerializeField, Range(0f, 1f)] private float breakVolume = 1f;

    private GameObject hiddenSwitch;
    private bool isBroken;

    public void SetHiddenSwitch(GameObject switchObject)
    {
        hiddenSwitch = switchObject;

        if (hiddenSwitch != null)
            hiddenSwitch.SetActive(false);
    }

    public void BreakPot()
    {
        if (isBroken)
            return;

        isBroken = true;

        // Reveal the hidden switch, if this pot has one.
        if (hiddenSwitch != null)
            hiddenSwitch.SetActive(true);

        if (explosionPrefab != null)
        {
            Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        if (breakClip != null)
        {
            AudioSource.PlayClipAtPoint(
                breakClip,
                transform.position,
                breakVolume
            );
        }

        // Destroy the entire pot prefab.
        Destroy(transform.root.gameObject);
    }
}
