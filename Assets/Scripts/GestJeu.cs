using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum EtatJeu
{
    Debut,
    Initialisation,
    Attaque,
    Jeu,
    Mort,
    Fin
}

public class GestJeu : MonoBehaviour
{
    public static GestJeu instance;

    public EtatJeu etat;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        etat = EtatJeu.Jeu;
    }

    public IEnumerator FinJeu()
    {
        etat = EtatJeu.Fin;

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Intro");
    }
}
