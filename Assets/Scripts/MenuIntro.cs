using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuIntro : MonoBehaviour
{
  [SerializeField] TMP_Text texteCoups;

    void Start()
    {
        int nbCoups = PlayerPrefs.GetInt("score", 0);

        texteCoups.text = $"Dernier match : {nbCoups} coups";
    }
    

    public void DemarrerJeu()
    {
        SceneManager.LoadScene("Jeu");
    }
}
