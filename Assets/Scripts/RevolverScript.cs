using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class RevolverScript : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;       //Serializamos el GameManager de Raimon. Esto lo hago con más fe que conocimiento

    [SerializeField] public float rangoRevolver = 100f;         //Rango efectivo del revolver
    [SerializeField] public int damageRevolver = 15;         //Daño efectivo del revolver
    [SerializeField] public int balasActuales = 3;              //Cuantas balas tengo actualmente en el tambor
    [SerializeField] public int balasReserva = 180;             //Cuantas balas tengo en mi inventario
    [SerializeField] private int maxCapacidad = 6;              //A priori esto no va a cambiar a no ser que metamos en algún momento mejoras de la capacidad en el tambor del revolver
    public int balasPorRecargar;                                // Cantidad real de balas a cargar

    public Animator revolverAnimator;                           //Animator para las balas del revolver

    //[SerializeField] TMP_Text textoDebug;

    [SerializeField] AudioClip revolverDisparo;
    [SerializeField] AudioClip revolverDisparoSinBalas;
    [SerializeField] AudioClip revolverVaciaCargador;
    [SerializeField] AudioClip revolverCargaBala;
    [SerializeField] AudioClip revolverCierraCargador;

    public AudioSource fuenteSonido;

    public Camera camaraFPS;    //Es la cámara que nos servirá de punto de origen para cada disparo. Vamos a usar nuestra "WeaponCamera" de primeras pero como sólo
                                //renderiza el revolver y las manos no sé yo si funcionará con el raycasting...probemos

    [SerializeField] LayerMask capaEnemigo;                     //Sólo haremos el impacto del disparo sobre aquellas entidades en la capa "enemigo"
    [SerializeField] LayerMask capaEntorno;                     //Si le damos a algún otro elemento que no sea el enemigo habremos fallado

    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject bulletImpactSuccess;            //Vamos a instanciar esto donde de impacte nuestro raycast sobre un enemigo
    [SerializeField] GameObject bulletImpactMiss;               //Vamos a instanciar esto donde de impacte nuestro raycast y así mostrar el imapacto de bala en las paredes y suelo
    [SerializeField] TMP_Text CurrentAmmo;
    [SerializeField] TMP_Text TotalAmmo;
    

    // Start is called before the first frame update
    void Start()
    {
        revolverAnimator= GetComponent<Animator>();
        fuenteSonido= GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetRevolverInputs();
        MuestraMunicion();
        //MuestraDatosDebug();

    }

    public void MuestraDatosDebug()
    {
        string mensaje = "";
        mensaje += "\nBalas actuales:"+balasActuales;
        mensaje += "\nBalas en reserva:" + balasReserva;
        mensaje += "\nBalas por recargar:" + balasPorRecargar;
        
        //textoDebug.text = mensaje;

    }

    public void MuestraMunicion()
    {
        Color colorBlanco = Color.white;
        Color colorRojo = Color.red;

        if (balasActuales <= 0)
        {
            CurrentAmmo.color = colorRojo;
        }
        else
        {
            CurrentAmmo.color = colorBlanco;
        }

        if (balasReserva <= 0)
        {
            TotalAmmo.color = colorRojo;
            
        }
        else
        {
            TotalAmmo.color = colorBlanco;
            
        }

        CurrentAmmo.text = balasActuales.ToString();
        TotalAmmo.text = "/"+balasReserva.ToString();
        
    }

    //Aquí metemos todo lo relacionado con los inputs del revolver, como disparar o recargar
    public void GetRevolverInputs()
    {
        
        //Si pulsamos la recla R (recarga)...
        if (Input.GetKeyDown(KeyCode.R)){
            
            //Llamamos al método ComienzaRecarga();
            ComienzaRecarga();
        }

        if (Input.GetMouseButtonDown(0))
        {

            //Sólo podremos volver a disparar si no hay una animación de disparo en curso
            //así que, ¿El parámetro "RevolverShooting" está a false?
            if (!revolverAnimator.GetBool("RevolverShooting"))
            {
                //Tenemos balas para disparar?
                if (balasActuales > 0)
                {
                    //Debug.Log("DISPARAMOS");
                    DisparoRevolver();
                }
                else
                {
                    //Sonido de click al intentar disparar sin tener balas    
                    fuenteSonido.PlayOneShot(revolverDisparoSinBalas);
                }
            }
            
        }
    }


    //Llamamos a la animación de recarga
    public void ComienzaRecarga()
    {
        //Se cumplen los requisitos para recargar?
        if (balasActuales < maxCapacidad && balasReserva > 0)
        {
            
            //Tomamos el mínimo entre dos valores; las balas que me faltan para llegar a tener el tambor lleno y las balas que tengo en reserva
            //Así evitamos cargar balas si no nos quedan balas en el inventario
            balasPorRecargar = Mathf.Min(maxCapacidad - balasActuales, balasReserva);


            fuenteSonido.PlayOneShot(revolverVaciaCargador);

            //Llamamos a la animación pertinente poniendo a true el parámetro que corresponde
            revolverAnimator.SetBool("RevolverRecharging", true);
        }
    }

    public void DisparoRevolver()
    {
        //Debug.Log("PIUM!!");
        
        //Reproducimos el sonido de disparo
        fuenteSonido.PlayOneShot(revolverDisparo);
        
        //Reproducimos la animación de disparo
        revolverAnimator.SetBool("RevolverShooting", true);

        

        //Restamos una unidad de las balas del cargador
        balasActuales--;

        // shake camera. De nuevo un copy-pega de Raimon. Si no me equivoco sólo necesito llamar al evento Shake del GameManager y la cámara se moverá...
        //gameManagerSO.Shake(0.15f, 1f, 0.15f);

        //Llamamos a un método propio para el recoil
        StartCoroutine(WeaponRecoil());

        //reproducimos el sistema de partículas para mostrar el muzzleFlash
        muzzleFlash.Play();

        //Y vamos a pelearnos con el raycast
        RaycastHit impacto;

        //Lanzamos nuestro rayo
        if (Physics.Raycast(camaraFPS.transform.position, camaraFPS.transform.forward, out impacto, rangoRevolver, capaEntorno))
        {

            //Vamos a instanciar el VFX de impacto de bala
            Instantiate(bulletImpactMiss, impacto.point, Quaternion.LookRotation(impacto.normal));  //Esto ha salido del tutorial de FPS de Brackeys
        }

        if (Physics.Raycast(camaraFPS.transform.position, camaraFPS.transform.forward, out impacto, rangoRevolver, capaEnemigo))
        {
            //Impactamos, así que vamos a ir tomando info de impacto a través de su componente GameObject
            //el cual almacenaremos en la variable objetivo
            GameObject objetivo = impacto.collider.gameObject;

            float vida = objetivo.GetComponent<EnemyController>().HealthPoints;

            if (vida > 0) { 

                int indiceEnemigo = objetivo.GetComponent<EnemyController>().EnemyId;

                objetivo.GetComponent<EnemyController>().TakeDamage(indiceEnemigo, damageRevolver);
            }

            //Vamos a instanciar el VFX de sangre para enfatizar que le hemos acertado a un enemigo
            Instantiate(bulletImpactSuccess, impacto.point, Quaternion.LookRotation(impacto.normal));  //Esto ha salido del tutorial de FPS de Brackeys

        }

        
    }

    IEnumerator WeaponRecoil()
    {
        //Tiempo de retroceso
        float recoilTime = 0.1f;      //Tiempo que tardamos en el retroceso
        float currentTime = 0f;     //Tiempo actual a efectos de comparar
        float recoveryTime = 0.1f;    //Tiempo que tardamos en volver a la posición original (porque quizá queramos que vuelva más lento)

        //distancia de retroceso
        float recoilDistance = -0.05f;

        //Posición original
        Vector3 originalPosition = transform.localPosition;

        //A dónde lo queremos desplazar?
        Vector3 recoilPosition = originalPosition + new Vector3(0f,0f,recoilDistance);

        //Mientras que el tiempo sea menor o igual que el tiempo que hemos establecido para el retroceso...
        while (currentTime <= recoilTime)
        {
            //Desplazamos hacia atrás
            transform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, currentTime);

            currentTime += Time.deltaTime;  //Aumentamos currentTime

            yield return null;
        }

        //Para garantizar que hemos llegado a la posición final forzamos el transform
        transform.localPosition = recoilPosition;

        //Y ahora volvemos al punto de partida
        
        currentTime = 0f;   //Esto lo reseteamos a cero

        while (currentTime <= recoveryTime)
        {
            //Desplazamos hacia atrás
            transform.localPosition = Vector3.Lerp(recoilPosition, originalPosition, currentTime);

            currentTime += Time.deltaTime;  //Aumentamos currentTime

            yield return null;
        }

        //Y como en el caso anterior forzamos el localposition
        transform.localPosition = originalPosition;
        
        //Quedamos listos para volver a disparar
        revolverAnimator.SetBool("RevolverShooting", false);
    }


    //En lugar de poner "RevolverShooting" a false con una corrutina he preferido hacerlo con un evento de animación
    //por si en el futuro hubiese una mejora de disparo rápido, por ejemplo
    public void FinDisparoRevolver()
    {
        //revolverAnimator.SetBool("RevolverShooting", false);
    }

    //Si todo va bien este método debería ser llamado desde los eventos de la animación "revolverRecharging.anim"
    public void RecargaBala()
    {
        
        
        //Si las balas en el tambor son menos de la capacidad máxima y nos quedan balas en el inventario...
        if(balasActuales<maxCapacidad && balasReserva > 0)
        {
           
            //Actualizamos el número de balas según el caso 
            balasActuales++;
            balasReserva--;
            balasPorRecargar--;
            fuenteSonido.PlayOneShot(revolverCargaBala);


            if (balasPorRecargar <= 0)
            {
                //Ya no tenemos por qué seguir ejecutando la animación de recarga así que la interrumpimos
                
                revolverAnimator.SetBool("RevolverRecharging", false);   //Ahora entiendo lo que decía Fernando de los nombres largos. Espero no haber cometido ningún "typo"
                revolverAnimator.SetTrigger("RevolverRechargeInterrupt");
                fuenteSonido.PlayOneShot(revolverCierraCargador);
            }
        }
    }

}
