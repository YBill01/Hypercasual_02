using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CAvatar : MonoBehaviour, IPausable
{
	[SerializeField]
	private float m_animMoveSpeedRatio = 1.0f;
	[SerializeField]
	private AnimationCurve m_animMoveCorrectionCurve;

	private float _moveSpeed = 1.0f;
	private float _moveVelocity;

	private Animator _animator;

	private int _animIDVelocity;
	private int _animIDJump;
	private int _animIDCollect;
	private int _animIDAnimMoveSpeed;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	private void Start()
	{
		AssignAnimationIDs();
	}

	private void LateUpdate()
	{
		UpdateAnimation();
	}

	public void SetPause(bool pause)
	{
		if (pause)
		{
			_animator.speed = 0.0f;
		}
		else
		{
			_animator.speed = 1.0f;
		}
	}

	public void SetMoveSpeed(float value)
	{
		_moveSpeed = value;
	}
	public void SetMoveVelocity(float value)
	{
		_moveVelocity = value;
	}

	public void SetJump()
	{
		_animator.SetTrigger(_animIDJump);
	}
	public void SetCollect()
	{
		_animator.SetTrigger(_animIDCollect);
	}

	private void AssignAnimationIDs()
	{
		_animIDVelocity = Animator.StringToHash("Velocity");
		_animIDJump = Animator.StringToHash("Jump");
		_animIDCollect = Animator.StringToHash("Collect");
		_animIDAnimMoveSpeed = Animator.StringToHash("AnimMoveSpeed");
	}

	private void UpdateAnimation()
	{
		_animator.SetFloat(_animIDAnimMoveSpeed, (_moveSpeed / m_animMoveSpeedRatio) * m_animMoveCorrectionCurve.Evaluate(_moveVelocity));
		_animator.SetFloat(_animIDVelocity, _moveVelocity);
	}
}