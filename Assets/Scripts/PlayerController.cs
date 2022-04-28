using States;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    

    [SerializeField][Range(1,5)] private float _movementSpeed = 2.75f;
    [SerializeField][Range(0.25f,5)] private float _jumpDurationUp = 0.5f;
    [SerializeField][Range(0.25f,5)] private float _jumpDurationDown = 0.25f;
    [SerializeField][Range(1,5)] private float _jumpVelocity = 1;

    private InputActionMap _map;
    private InputAction _input;
    private InputAction _jump;
    private Animator _anim;
    private SpriteRenderer _renderer;

    private StateMachine _playerControlStateMachine;

    private StateMachine _standingStateMachine;
    private StateMachine _crouchingStateMachine;
    private StateMachine _airborneStateMachine;

    private float _timer = 0;
    private float _trueYFeetPos;


    private void Awake(){
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _map = GetComponent<PlayerInput>().currentActionMap;
        _input = _map.FindAction("Movement");
        _jump = _map.FindAction("Jump");
        _trueYFeetPos = transform.position.y - 1.22f;

        _standingStateMachine = new StateMachineBuilder()
            .WithState("IDLE")
            .WithOnEnter(() => _anim.Play("Idle"))
            .WithTransition("WALK", () => _input.ReadValue<Vector2>() != Vector2.zero)

            .WithState("WALK")
            .WithOnEnter(() => _anim.Play("Walk"))
            .WithOnRun(() => Move())
            .WithTransition("IDLE", () => _input.ReadValue<Vector2>() == Vector2.zero)

            .Build();

        _airborneStateMachine = new StateMachineBuilder()
            .WithState("JUMP")
            .WithOnEnter(() => _anim.Play("Jump"))
            .WithOnEnter(() => _timer = _jumpDurationUp)
            
            .WithOnRun(() => Move())
            .WithOnRun(() => VerticalMovement(1, _jumpDurationUp))
            .WithOnRun(() => _timer -= Time.deltaTime)
            .WithTransition("FALL", () => _timer <= 0)

            .WithState("FALL")
            .WithOnEnter(() => _anim.Play("Fall"))
            .WithOnEnter(() => _timer = _jumpDurationDown)
            .WithOnRun(() => Move())
            .WithOnRun(() => VerticalMovement(-1, _jumpDurationDown))
            .WithOnRun(() => _timer -= Time.deltaTime)
            .WithTransition("HITGROUND", () => _timer <= 0)

            .WithState("HITGROUND")
            .WithOnEnter(() => _anim.Play("HitGround"))
            .WithOnEnter(() => _timer = 0.15f)
            .WithOnRun(() => _timer -= Time.deltaTime)
            .WithTransition("DONE", () => _timer <= 0)

            .WithState("DONE")
            .Build();

        _playerControlStateMachine = new StateMachineBuilder()
            .WithState("STANDING")
            .WithOnEnter(() => _standingStateMachine.ResetStateMachine())
            .WithOnRun(() => _standingStateMachine.RunStateMachine())
            .WithTransition("AIRBORNE", () => _jump.ReadValue<float>() != 0)

            .WithState("AIRBORNE")
            .WithOnEnter(() => _airborneStateMachine.ResetStateMachine())
            .WithOnRun(() => _airborneStateMachine.RunStateMachine())
            .WithTransition("STANDING", () => _airborneStateMachine.CurrentState.Name == "DONE")

            .Build();
        
        _playerControlStateMachine.ResetStateMachine();
    }

    private void FixedUpdate(){
        
        _playerControlStateMachine.RunStateMachine();
    }

    private void Move(){
        CollisionData data = CheckCollision();
        Vector3 input = (Vector3)_input.ReadValue<Vector2>();
        Debug.Log($"{data.left}  {data.right}   {data.up}   {data.down}");
        if(input.x > 0 && data.right || input.x < 0 && data.left){
            input.x = 0;
        }
        if(input.y > 0 && data.up || input.y < 0 && data.down){
            input.y = 0;
        }
        transform.position += input * _movementSpeed * Time.deltaTime;
        _trueYFeetPos += input.y * _movementSpeed * Time.deltaTime;
        if(!_renderer.flipX && input.x < 0 || _renderer.flipX && input.x > 0){
            _renderer.flipX = !_renderer.flipX;
        }
    }

    private void VerticalMovement(float sign, float duration){
        transform.position += new Vector3(0, sign, 0) * _jumpVelocity / duration * Time.deltaTime;
    }

    private CollisionData CheckCollision(){
        return new CollisionData(){
            left = Physics2D.Raycast(new Vector2(transform.position.x, _trueYFeetPos), Vector2.left, 0.35f),
            right = Physics2D.Raycast(new Vector2(transform.position.x, _trueYFeetPos), Vector2.right, 0.35f),
            up = Physics2D.Raycast(new Vector2(transform.position.x, _trueYFeetPos), Vector2.up, 0.35f),
            down = Physics2D.Raycast(new Vector2(transform.position.x, _trueYFeetPos), Vector2.down, 0.35f)
        };
    }

    public struct CollisionData{
        public bool up, down, left, right;
    }
}
