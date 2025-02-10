
using UnityEngine;


public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField] private ParticleSystem fireParticleSystem;
    [SerializeField] private Transform respawnTrasnform;
    [SerializeField] private bool isActive;

    public bool IsActive { get => isActive; }

    // Start is called before the first frame update
    void Start()
    {
        if (isActive)
        {
            fireParticleSystem.Play();
        }
        else
        {
            fireParticleSystem.Stop();
        }
    }

    public void Activate()
    {
        if (!isActive)
        {
            fireParticleSystem.Play();
            isActive = true;
            gameManagerSO.SetLastRespawnPoint(respawnTrasnform);
        }
    }
}
