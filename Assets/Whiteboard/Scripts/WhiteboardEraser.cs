using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Tooltip("Must use a mesh collider on Whiteboard!")]
public class WhiteboardEraser : MonoBehaviour
{
    [SerializeField] private Transform _eraserTip;
    [SerializeField] private int _tipLength;
    [SerializeField] private int _tipWidth;

    [SerializeField] private PhysicMaterial slipperyPhysMat;
    [SerializeField] private PhysicMaterial normalPhysMat;

    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;

    private RaycastHit _hit;
    private Whiteboard _detectedWhiteboard;
    private Vector2 _hitPos;
    private Vector2 _lastHitPos;
    private Quaternion _lastHitrot;
    private bool _hitLastFrame;

    private Rigidbody _rb;
    private BoxCollider _collider; 
    public bool lockRotation = true;

    [Tooltip("How much drawing is interpolated from 0 to 1.")]
    [SerializeField] private float _drawSmoothing = 0.1f;

    void Start()
    {
        _renderer = _eraserTip.GetComponent<Renderer>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponentInChildren<BoxCollider>();
        // In order to draw to the texture, we have to designate an area of pixels to draw to.
        _colors = Enumerable.Repeat(_renderer.material.color, _tipLength * _tipWidth).ToArray();
        _tipHeight = _eraserTip.localScale.y;

    }

    private void FixedUpdate()
    {
        Draw();
    }

    private void Draw()
    {

        //tipHeight is the length of the raycast we are going to shoot from the marker tip
        // It uses the Y SCALE of a designated tip, it must be this height of the pen base to the tip

        if (Physics.Raycast(_eraserTip.position + (-transform.up * _tipHeight/2), transform.up, out _hit, _tipHeight))
        {
            if (_hit.transform.CompareTag("Whiteboard"))
            {

                //Grabs the component on hit only once, so we dont have to keep calling it
                if (_detectedWhiteboard == null)
                {
                    _rb.freezeRotation = true;
                    _collider.material = slipperyPhysMat;
                    _detectedWhiteboard = _hit.transform.GetComponent<Whiteboard>();
                    Color whiteboardColor = _detectedWhiteboard.GetComponent<Renderer>().material.color;
                    _colors = Enumerable.Repeat(whiteboardColor, _tipLength * _tipWidth).ToArray();

                    
                }

                //Get the position on the white board based on where our marker is.

                _hitPos = new Vector2(_hit.textureCoord.x, _hit.textureCoord.y);
                int x = (int)(_hitPos.x * _detectedWhiteboard.texSize.x - (_tipLength/2));
                int y = (int)(_hitPos.y * _detectedWhiteboard.texSize.y - (_tipWidth/2));


                //To make sure we are trying to draw within the bounds of our whiteboard
                if (y < 0 || y >= _detectedWhiteboard.texSize.y || x < 0 || x >= _detectedWhiteboard.texSize.x) return;
                


                //If we hit the whiteboard last frame, draw a line between the last hit position and the current hit position
                try{
                    if (_hitLastFrame)
                    {

                        _detectedWhiteboard.texture.SetPixels(x, y, _tipLength, _tipWidth, _colors);

                        for (float i = 0.01f; i < 1; i += _drawSmoothing)
                        {
                            int lerpX = (int)Mathf.Lerp(_lastHitPos.x, x, i);
                            int lerpY = (int)Mathf.Lerp(_lastHitPos.y, y, i);
                            _detectedWhiteboard.texture.SetPixels(lerpX, lerpY, _tipLength, _tipWidth, _colors);
                        }

                        //if (lockRotation) transform.rotation = _lastHitrot;

                        _detectedWhiteboard.texture.Apply();
                    }
                }
                catch { 
                    return; 
                }


                //Set last frame information
                _lastHitPos = new Vector2(x, y);
                //_lastHitrot = transform.rotation;
                _hitLastFrame = true;
                return;
            }
        }

        //If we didnt find a whiteboard with our raycast, then we are not drawing on a whiteboard. 
        if (_detectedWhiteboard != null)
        {
            _collider.material = normalPhysMat;
            _rb.freezeRotation = false;
            _detectedWhiteboard = null;
            _hitLastFrame = false;
        }
    }


    //Unused ATM
    public void setPenColor(Color color)
    {
        _renderer.material.color = color;
        _colors = Enumerable.Repeat(_renderer.material.color, _tipLength * _tipWidth).ToArray();
    }

    public void setTipSizes(int length, int width)
    {
        _tipLength = length;
        _tipWidth = width;
        _colors = Enumerable.Repeat(_renderer.material.color, _tipLength * _tipWidth).ToArray();
    }   

}
