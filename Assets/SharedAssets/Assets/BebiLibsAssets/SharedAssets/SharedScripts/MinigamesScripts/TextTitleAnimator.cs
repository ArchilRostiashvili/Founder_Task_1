using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using BebiLibs;

public class TextTitleAnimator : MonoBehaviour
{
    private Color32[] _vertexColors;

    private Matrix4x4 _matrix;

    private TMP_Text _tmp;

    private TMP_TextInfo _textInfo;

    private TMP_MeshInfo[] _meshInfo;

    private TMP_CharacterInfo _charInfo;

    private VertexAnim[] _vertexAnim;

    public bool animate = false;

    private int _materialIndex, _vertexIndex;
    private Vector2 _charMidBasline;
    private Vector3 _offset, _newPosition, _bottomLeft, _topRight;
    private Vector3[] _sourceVertices, _destinationVertices;
    private VertexAnim _vertAnim;


    public float speedXMax;
    public float speedYMax;
    public float speedFadeMax;

    //private bool _initDone = false;
    //private bool initMeshDataDone = false;

    private float _defaultScale;

    void Awake()
    {
        this.animate = false;
        //this.gameObject.SetActive(false);
        this.Init();
    }

    public void Init()
    {
        _tmp = this.GetComponent<TMP_Text>();
        _defaultScale = this.transform.localScale.y;
        if (_tmp != null && _tmp.isActiveAndEnabled)
        {
            //_initDone = true;
        }
    }

    public void InitMeshData()
    {
        //Debug.Log("Initialize..");
        _tmp.ForceMeshUpdate();
        if (_tmp.textInfo == null)
        {
            Debug.LogWarning("texInfo is null");
            return;
        }



        _textInfo = _tmp.textInfo;
        if (_textInfo.characterCount == 0 || _textInfo.CopyMeshInfoVertexData() == null)
        {
            return;
        }

        _meshInfo = _textInfo.CopyMeshInfoVertexData();

        VertexAnim tmpVertAnim;
        List<VertexAnim> tmpList = new List<VertexAnim>();
        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            if ((_textInfo.characterInfo[i].isVisible) && (_textInfo.characterInfo[i].elementType.Equals(TMP_TextElementType.Character)))
            {
                tmpVertAnim = new VertexAnim();
                tmpVertAnim.animate = true;
                tmpVertAnim.charIndex = i;
                tmpVertAnim.dir = new Vector2(Random.Range(5.0f, this.speedXMax) * (UnityEngine.Random.Range(1, 3) == 1 ? 1 : -1), Random.Range(5.0f, this.speedYMax) * (UnityEngine.Random.Range(1, 3) == 1 ? 1 : -1));
                tmpVertAnim.angle = new Vector3(0.0f, 0.0f, Random.Range(-10f, 10f));
                tmpVertAnim.fade = (byte)Random.Range(1, this.speedFadeMax);
                tmpVertAnim.color = new Color32(_textInfo.characterInfo[i].color.r, _textInfo.characterInfo[i].color.g, _textInfo.characterInfo[i].color.b, _textInfo.characterInfo[i].color.a);
                tmpList.Add(tmpVertAnim);
            }
        }

        _bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero) * 2.0f;
        _topRight = Camera.main.ViewportToWorldPoint(Vector3.one) * 2.0f;
        _vertexAnim = tmpList.ToArray();

        //this.initMeshDataDone = true;
    }

    public void Show(string text, bool anim)
    {
        //_tmp.text = text;
        this.gameObject.SetActive(true);
        if (anim)
        {
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(_defaultScale, 0.2f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                ManagerTime.Delay(0.3f, () =>
                {
                    this.InitMeshData();
                });
            });
        }
        else
        {
            this.transform.localScale = Vector3.one * _defaultScale;
            this.InitMeshData();
        }
    }

    public void Show(string text, System.Action onComplete = null, bool anim = true)
    {
        //_tmp.text = text;
        this.gameObject.SetActive(true);
        if (anim)
        {
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(_defaultScale, 0.2f)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                ManagerTime.Delay(0.4f, () =>
               {
                   this.InitMeshData();
               });
                onComplete?.Invoke();
            });
        }
        else
        {
            this.transform.localScale = Vector3.one * _defaultScale;
            this.InitMeshData();
        }
    }

    public void Hide(bool anim)
    {
        if (anim)
        {
            this.animate = true;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (this.animate && _textInfo != null)
        {
            _meshInfo = _textInfo.CopyMeshInfoVertexData();

            for (int i = 0; i < _vertexAnim.Length; i++)
            {
                if (_vertexAnim[i].animate)
                {
                    // Retrieve the pre-computed animation data for the given character.
                    _vertAnim = _vertexAnim[i];

                    // Only change the vertex color if the text element is visible and type of character.
                    TMP_CharacterInfo charInfo = _textInfo.characterInfo[_vertAnim.charIndex];
                    // Skip characters that are not visible and thus have no geometry to manipulate.

                    // Get the index of the material used by the current character.
                    _materialIndex = charInfo.materialReferenceIndex;

                    // Get the cached vertices of the mesh used by this text element (character or sprite).
                    _sourceVertices = _meshInfo[_materialIndex].vertices;

                    // Get the vertex colors of the mesh used by this text element (character or sprite).
                    _vertexColors = _textInfo.meshInfo[_materialIndex].colors32;

                    // Get the index of the first vertex used by this text element.
                    _vertexIndex = charInfo.vertexIndex;

                    // Determine the center point of each character at the baseline.
                    //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                    // Determine the center point of each character.
                    _charMidBasline = (_sourceVertices[_vertexIndex + 0] + _sourceVertices[_vertexIndex + 2]) / 2;

                    // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                    // This is needed so the matrix TRS is applied at the origin for each character.
                    _offset = _charMidBasline;

                    _destinationVertices = _textInfo.meshInfo[_materialIndex].vertices;

                    _destinationVertices[_vertexIndex + 0] = _sourceVertices[_vertexIndex + 0] - _offset;
                    _destinationVertices[_vertexIndex + 1] = _sourceVertices[_vertexIndex + 1] - _offset;
                    _destinationVertices[_vertexIndex + 2] = _sourceVertices[_vertexIndex + 2] - _offset;
                    _destinationVertices[_vertexIndex + 3] = _sourceVertices[_vertexIndex + 3] - _offset;

                    _newPosition = new Vector3(_offset.x + _vertAnim.dir.x * Time.deltaTime, _offset.y + _vertAnim.dir.y * Time.deltaTime, 0);
                    //_vertAnim.dir.x -= ((_vertAnim.dir.x > 0) ? 0.1f : 0) * Time.deltaTime;
                    //_vertAnim.dir.y -= 0.1f * Time.deltaTime;

                    _matrix = Matrix4x4.TRS(_newPosition, Quaternion.Euler(_vertAnim.angle.x, _vertAnim.angle.y, _vertAnim.angle.z), Vector3.one);
                    //To test:
                    //The position transform
                    //matrix = Matrix4x4.TRS(newPosition, Quaternion.Euler(0, 0, 0), Vector3.one);
                    //The rotation
                    //matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(vertAnim.angle.x, vertAnim.angle.y, vertAnim.angle.z), Vector3.one);
                    //The alpha fade
                    //matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0), Vector3.one);

                    _destinationVertices[_vertexIndex + 0] = _matrix.MultiplyPoint3x4(_destinationVertices[_vertexIndex + 0]);
                    _destinationVertices[_vertexIndex + 1] = _matrix.MultiplyPoint3x4(_destinationVertices[_vertexIndex + 1]);
                    _destinationVertices[_vertexIndex + 2] = _matrix.MultiplyPoint3x4(_destinationVertices[_vertexIndex + 2]);
                    _destinationVertices[_vertexIndex + 3] = _matrix.MultiplyPoint3x4(_destinationVertices[_vertexIndex + 3]);

                    if ((((_offset.x <= _bottomLeft.x) || (_offset.x >= _topRight.x)) || ((_offset.y <= _bottomLeft.y) || (_offset.y >= _topRight.y))) || (_vertAnim.color.a - _vertAnim.fade) < 0)
                    {
                        _vertAnim.color.a = 0;
                        _vertAnim.animate = false;
                    }
                    else
                    {
                        _vertAnim.color.a -= _vertAnim.fade;
                    }

                    _vertexColors[_vertexIndex + 0] = _vertAnim.color;
                    _vertexColors[_vertexIndex + 1] = _vertAnim.color;
                    _vertexColors[_vertexIndex + 2] = _vertAnim.color;
                    _vertexColors[_vertexIndex + 3] = _vertAnim.color;

                    _vertexAnim[i] = _vertAnim;
                }
            }

            // Push changes into meshes
            for (int i = 0; i < _textInfo.meshInfo.Length; i++)
            {
                _textInfo.meshInfo[i].mesh.vertices = _textInfo.meshInfo[i].vertices;
                _tmp.UpdateGeometry(_textInfo.meshInfo[i].mesh, i);
            }

            _tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            //yield return new WaitForSeconds(0.1f);

            this.animate = false;
            foreach (VertexAnim v in _vertexAnim)
            {
                if (v.animate)
                {
                    this.animate = true;
                    break;
                }
            }

            if (!this.animate)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    public void StopAnimateText()
    {
        if (animate)
        {
            animate = false;

            int i, materialIndex, vertexIndex;
            Vector3[] sourceVertices;
            VertexAnim vertAnim;

            _meshInfo = _textInfo.CopyMeshInfoVertexData();

            for (i = 0; i < _vertexAnim.Length; i++)
            {
                if (_vertexAnim[i].animate)
                {
                    vertAnim = _vertexAnim[i];

                    TMP_CharacterInfo charInfo = _textInfo.characterInfo[vertAnim.charIndex];

                    materialIndex = charInfo.materialReferenceIndex;

                    sourceVertices = _meshInfo[materialIndex].vertices;

                    _vertexColors = _textInfo.meshInfo[materialIndex].colors32;

                    vertexIndex = charInfo.vertexIndex;

                    vertAnim.animate = false;
                    vertAnim.color.a = 0;

                    _vertexColors[vertexIndex + 0] = vertAnim.color;
                    _vertexColors[vertexIndex + 1] = vertAnim.color;
                    _vertexColors[vertexIndex + 2] = vertAnim.color;
                    _vertexColors[vertexIndex + 3] = vertAnim.color;

                    _vertexAnim[i] = vertAnim;
                }
            }

            // Push changes into meshes
            for (i = 0; i < _textInfo.meshInfo.Length; i++)
            {
                _textInfo.meshInfo[i].mesh.vertices = _textInfo.meshInfo[i].vertices;
                _tmp.UpdateGeometry(_textInfo.meshInfo[i].mesh, i);
            }

            _tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }

    /// <summary>
    /// Structure to hold pre-computed animation data.
    /// </summary>
    private struct VertexAnim
    {
        public bool animate;
        public int charIndex;
        public Vector2 dir;
        public Vector3 angle;
        public byte fade;
        public Color32 color;
    }

}