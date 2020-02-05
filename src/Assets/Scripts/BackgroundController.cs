using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode] // <-- So that I can see the background drawn also in the editor
public class BackgroundController : MonoBehaviour {

    public Sprite[] images;

    private Image _img;

    private void Awake ()
    {
        _img = gameObject.GetComponent<Image>();

        SeRandomBackground();
    }

    private void SeRandomBackground()
    {
        // It will select a random sprite from he list of sprites passed as property
        if (_img != null && images != null && images.Length > 0)
        {
            // Init the background sprite with a random sprite
            int index = Random.Range(0, images.Length);
            _img.sprite = images[index];
        }
    }
}
