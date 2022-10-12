using UnityEngine;

public class ExplodeCubes : MonoBehaviour
{
    public float StrenghtExplosion = 70f;
    private bool _colisionSet;
    public GameObject restartButton;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cube" && !_colisionSet)
        {
            for(int i = collision.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = collision.transform.GetChild(i);
                child.gameObject.AddComponent<Rigidbody>();
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(StrenghtExplosion, Vector3.up, 15f);
                child.SetParent(null);
            }
            restartButton.SetActive(true);
            Camera.main.transform.position -= new Vector3(0, 0, 3f);
            Destroy(collision.gameObject);
            _colisionSet = true;
        }
    }
}
