using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteAlways]
public class OrderedInLayer : MonoBehaviour
{
    private SpriteRenderer _renderer;
    [SerializeField] private float _customY = 0;
    private void Awake(){
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update(){
        if(_customY != 0){
            _renderer.sortingOrder = (int)(Mathf.Abs(transform.position.y - _customY/2)*100);
        }
        else{
             _renderer.sortingOrder = (int)(Mathf.Abs(transform.position.y - transform.localScale.y/2)*100);
        }
    }
}
