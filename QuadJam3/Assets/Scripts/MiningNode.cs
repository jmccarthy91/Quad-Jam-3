using System.Collections;
using UnityEngine;

public class MiningNode : MonoBehaviour
{
    [Header("MiningNode Data")]
    [SerializeField] private int _health = 0;
    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private GameObject _sparkObject = null;
    [SerializeField] private GameObject _explosionObject = null;

    public void Hit()
    {
        _health--;

        Instantiate(_sparkObject, transform.position, transform.rotation);

        if (_health < 1)
        {
            Mined();
        }
    }

    private void Mined()
    {
        EventManager.Current.MineralMined();
        FindObjectOfType<AudioManager>().Play("MineralFinish");
        Debug.Log("[MineralNode]: Mineral Mined.");
        StartCoroutine(Explosion());
        Destroy(gameObject);
    }

    private IEnumerator Explosion()
    {
        GameObject o = Instantiate(_explosionObject, transform.position, transform.rotation);
        _spriteRenderer.enabled = false;

        yield return new WaitForSeconds(1.0f);

        Destroy(o);
    }
}
