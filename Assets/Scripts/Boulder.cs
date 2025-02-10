using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField][Range(10f, 100000f)] private float initialImpulseForce;
    [Tooltip("When the boulder speed is under this value it can not kill")]
    [SerializeField][Range(1f, 5f)] private float minimumKillingSpeed;
    [Header("SoundFX")]
    [SerializeField] private AudioClip exitAudioClip;
    [SerializeField] private AudioClip rollingAudioClip;

    private bool isKilling = false;
    private Rigidbody rb;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if boulder is moving and collide with player
        if (isKilling && collision.gameObject.CompareTag("Player"))
        {
            gameManagerSO.Damage(GameManagerSO.DamageType.boulder);
        }
    }

    private IEnumerator StopKilling()
    {
        // wait 1 second
        yield return new WaitForSeconds(1f);

        // wait until speed decreases
        yield return new WaitUntil(() => rb.velocity.magnitude < minimumKillingSpeed);

        Debug.Log("No more kills");

        // stop script
        isKilling = false ;
        rb.angularDrag = 0.5f;

        yield return new WaitUntil(() => rb.velocity.magnitude < 0.2f);
        Debug.Log("No more movement");
        rb.isKinematic = true ;
    }

    public void Activate()
    {
        isKilling = true;
        StartCoroutine(StopKilling());
        rb.useGravity = true;
        rb.AddForce(Vector3.down * initialImpulseForce, ForceMode.Impulse);
        StartCoroutine(PlaySFXandVFX());
    }

    private IEnumerator PlaySFXandVFX()
    {
        // audio
        audioSource.PlayOneShot(exitAudioClip);
        yield return new WaitUntil(() => rb.position.y < 5f);
        Debug.Log("Ground");
        audioSource.Stop();
        audioSource.clip = rollingAudioClip;
        audioSource.loop = true;
        audioSource.Play();
        yield return new WaitUntil(() => rb.velocity.magnitude < 8f);
        audioSource.Stop();
        audioSource.PlayOneShot(exitAudioClip);
        // shake camera
        gameManagerSO.Shake(0.15f, 0.02f, 1f);
    }
}
