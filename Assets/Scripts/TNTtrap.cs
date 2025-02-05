
using UnityEngine;

public class TNTtrap : MonoBehaviour
{

    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField] private ParticleSystem wickParticleSystem;
    [SerializeField] private ParticleSystem explosionParticleSystemOne;
    [SerializeField] private ParticleSystem explosionParticleSystemTwo;
    [SerializeField] private ParticleSystem explosionCylinder;
    [SerializeField] private GameObject cylinder;
    [SerializeField] private GameObject wick;
    [SerializeField] private GameObject explosionStain;
    [SerializeField][Range(4f, 10f)] private float letalRange;
    [SerializeField] private AudioSource audioSourceWick;

    private Animator animationController;
    private AudioSource audioSourceExplosion;

    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animator>();
        audioSourceExplosion = GetComponent<AudioSource>();
    }

    public void ActivateWick()
    {
        wickParticleSystem.gameObject.SetActive(true); 
        wickParticleSystem.Play();
        animationController.SetTrigger("TriggerWick");
        audioSourceWick.Play(); 
    }

    public void Explosion()
    {
        audioSourceExplosion.Play();
        explosionStain.SetActive(true);
        Destroy(wick);
        Destroy(cylinder);
        wickParticleSystem.Stop();
        audioSourceWick.Stop();
        wickParticleSystem.gameObject.SetActive(false);
        explosionParticleSystemOne.Play();
        explosionParticleSystemTwo.Play();
        explosionCylinder.Play();

        gameManagerSO.Shake();

        Collider[] colliders = Physics.OverlapSphere(transform.position, letalRange);

        foreach (Collider collider in colliders) {
            if (collider.CompareTag("Player"))
            {
                gameManagerSO.Death();
            }
        }
    }

    
}
