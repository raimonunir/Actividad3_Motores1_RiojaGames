using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idol : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField] private AudioClip collectSound; // Sonido opcional al recogerlo
    [SerializeField] private ParticleSystem collectParticles; // Efecto visual opcional
    [SerializeField][Range(50f, 200f)] private float rotationSpeed;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Time.deltaTime * rotationSpeed *  Vector3.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (gameManagerSO != null)
        {
            gameManagerSO.GemCollected(); // Sumar los puntos al GameManagerSO
        }

        if (collectSound != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position); // Reproducir sonido
        }

        if (collectParticles != null)
        {
            Instantiate(collectParticles, transform.position, Quaternion.identity); // Crear efecto visual
        }

        Destroy(gameObject); // Eliminar el coleccionable
    }
}
