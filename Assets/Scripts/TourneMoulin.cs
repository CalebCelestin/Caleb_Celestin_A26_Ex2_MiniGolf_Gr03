using UnityEngine;

public class TourneMoulin : MonoBehaviour
{
    [SerializeField] float vitesseRotation;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * vitesseRotation * Time.deltaTime, Space.Self);
    }
}
