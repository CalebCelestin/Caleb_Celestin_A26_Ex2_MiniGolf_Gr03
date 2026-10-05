using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public class Balle : MonoBehaviour
{
    [Header("État de jeu")]
    Vector3 positionBalle;
    [SerializeField] int nbCoups;
    public bool peutJouer;


    [Header("Paramètres de tir")]
    [SerializeField] float forceTir;
    [SerializeField] float angleTir;
    [SerializeField] float vitesseRotation;
    Rigidbody rigidbodyBalle;

    [Header("Gauge de force")]

    [SerializeField] float accumulateurForce = 0.8f;

    [SerializeField] Slider jaugeForce;
    [SerializeField] float forceMin = 0f;
    [SerializeField] float forceMax = 100f;


    [Header("Input Actions")]
    [SerializeField] InputAction tirAction;
    [SerializeField] InputAction angleAction;

    [Header("Composant")]
    LineRenderer lineRendererBalle;
    AudioSource audioSourceBalle;
    [SerializeField] AudioClip sonFin;
    [SerializeField] AudioClip sonErreur;
    [SerializeField] TMP_Text texteCoups;


    void Start()
    {
        
        rigidbodyBalle = GetComponent<Rigidbody>();
        lineRendererBalle = GetComponent<LineRenderer>();
        audioSourceBalle = GetComponent<AudioSource>();
        nbCoups = 0;
        MettreAJourUI();

        peutJouer = true;
    }

    void Update()
    {

        if (peutJouer == true && GestJeu.instance.etat == EtatJeu.Jeu)
        {

            float inputRotation = angleAction.ReadValue<float>();

            angleTir += inputRotation;
            Vector3 direction = Quaternion.Euler(0, angleTir, 0) * Vector3.forward;
            lineRendererBalle.SetPosition(0, transform.position);
            lineRendererBalle.SetPosition(1, transform.position + direction);

            if (tirAction.WasPressedThisFrame())
            {
                forceTir = 0;
                jaugeForce.value = forceTir;
                MettreAJourUI();
            }

            if (tirAction.IsPressed())
            {
                Debug.Log("ESPACE EST DÉTECTÉ");
                forceTir += accumulateurForce;
                forceTir = Mathf.Clamp(forceTir, jaugeForce.minValue, jaugeForce.maxValue);
                jaugeForce.value = forceTir;
            }

            if (tirAction.WasReleasedThisFrame())
            {
                positionBalle = transform.position;
                rigidbodyBalle.AddForce(direction * forceTir * Time.deltaTime, ForceMode.Impulse);

                nbCoups++;
                MettreAJourUI();

                forceTir = 0;
                jaugeForce.value = forceTir;

                string positionJSON = JsonUtility.ToJson(transform.position);
                PlayerPrefs.SetString("dernierePosition", positionJSON);

                StartCoroutine(AtteindreFinCoup());
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "horsParcours")
        {
            audioSourceBalle.PlayOneShot(sonErreur);
            rigidbodyBalle.linearVelocity = Vector3.zero;

            transform.position = positionBalle;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "trou")
        {
            audioSourceBalle.PlayOneShot(sonFin);
            rigidbodyBalle.linearVelocity = Vector3.zero;
            transform.position = collision.transform.position;

            StartCoroutine(AttendreSonFin());

            PlayerPrefs.SetInt("score", nbCoups);

        }
    }

    // ===================
    IEnumerator AtteindreFinCoup()
    {
        peutJouer = false;
        lineRendererBalle.enabled = false;
        yield return new WaitForFixedUpdate();  
        float vitesse = rigidbodyBalle.linearVelocity.magnitude;
        while (vitesse > 0.1f)
        { 
            // recalcule
            vitesse = rigidbodyBalle.linearVelocity.magnitude;
            // attendre au prochaine frame
            yield return null; 
        }
        ;

        peutJouer = true;
        Vector3 direction = Quaternion.Euler(0, angleTir, 0) * Vector3.forward;
        lineRendererBalle.SetPosition(0, transform.position);
        lineRendererBalle.SetPosition(1, transform.position + direction);
        lineRendererBalle.enabled = true;
    }

    IEnumerator AttendreSonFin()
    {
        peutJouer = false;
        lineRendererBalle.enabled = false;
        
        yield return new WaitForSeconds(2f);

        StartCoroutine(GestJeu.instance.FinJeu());
        PlayerPrefs.DeleteKey("dernierePosition");
        SceneManager.LoadScene("Intro");
    }

    void MettreAJourUI()
    {
        texteCoups.text = $"{nbCoups} coup(s)";
        jaugeForce.value = forceTir;
    }

    //=================================
    // Gestion des inputs actions
    void OnEnable()
    {
        tirAction.Enable();
        angleAction.Enable();
    }

    void OnDisable()
    {
        tirAction.Disable();
        angleAction.Disable();
    }
}