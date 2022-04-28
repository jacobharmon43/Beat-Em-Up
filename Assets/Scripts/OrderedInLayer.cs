using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OrderedInLayer : MonoBehaviour
{
    protected SpriteRenderer _renderer;
    [SerializeField] private float _customY = 0;
    protected virtual void Awake(){
        _renderer = GetComponent<SpriteRenderer>();
        UpdateLayer();
    }

    public void UpdateLayer(){
        if(_customY != 0){
            _renderer.sortingOrder = (int)(Mathf.Abs(transform.position.y - _customY/2)*100);
        }
        else{
             _renderer.sortingOrder = (int)(Mathf.Abs(transform.position.y - transform.localScale.y/2)*100);
        }
    }

    public void UpdateLayer(float yPos){
        if(_customY != 0){
            _renderer.sortingOrder = (int)(Mathf.Abs(yPos)*100);
        }
        else{
             _renderer.sortingOrder = (int)(Mathf.Abs(yPos)*100);
        }
    }
}
