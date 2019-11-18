using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private SpriteRenderer _swordSprite;
    private BoxCollider2D _hitDetection;
    public float stabTiming = 0.25f;

    private bool _alreadyPlaying;
    // Start is called before the first frame update
    void Start()
    {
        _swordSprite = gameObject.GetComponent<SpriteRenderer>();
        _hitDetection = gameObject.GetComponent<BoxCollider2D>();
        _swordSprite.enabled = false;
        _hitDetection.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_alreadyPlaying == false)
            {
                _alreadyPlaying = true;
                StartCoroutine(SwordTiming());
            }
        }
    }

    private IEnumerator SwordTiming()
    {
        _swordSprite.enabled = true;
        _hitDetection.enabled = true;
        //Debug.Log(_swordSprite.enabled);
        yield return new WaitForSeconds(stabTiming);
        _swordSprite.enabled = false;
        //Debug.Log(_swordSprite.enabled);
        _hitDetection.enabled = false;
        yield return new WaitForSeconds(stabTiming);
        _alreadyPlaying = false;
    }
}

