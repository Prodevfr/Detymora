using Godot;

namespace Detymora.Physics;

#nullable enable

[Tool]
public partial class Ball : RigidBody2D
{
    private float _radius = 18.0f;
    private Color _tint = new(0.8f, 0.2f, 0.2f);
    private float _bodyMass = 1.0f;
    private float _bounciness = 0.6f;
    private float _friction = 0.4f;
    private Vector2 _initialLinearVelocity = Vector2.Zero;

    private bool _initialVelocityApplied;

    [ExportSubgroup("Visual")]
    [Export]
    public float Radius
    {
        get => _radius;
        set
        {
            if (Mathf.IsEqualApprox(_radius, value))
            {
                return;
            }

            _radius = Mathf.Max(0.5f, value);
            UpdateGeometry();
        }
    }

    [Export]
    public Color Tint
    {
        get => _tint;
        set
        {
            if (_tint == value)
            {
                return;
            }

            _tint = value;
            QueueRedraw();
        }
    }

    [ExportSubgroup("Dynamics")]
    [Export(PropertyHint.Range, "0.1,10,0.1")]
    public float BodyMass
    {
        get => _bodyMass;
        set
        {
            if (Mathf.IsEqualApprox(_bodyMass, value))
            {
                return;
            }

            _bodyMass = value;
            UpdatePhysicsProperties();
        }
    }

    [Export(PropertyHint.Range, "0,1,0.05")]
    public float Bounciness
    {
        get => _bounciness;
        set
        {
            if (Mathf.IsEqualApprox(_bounciness, value))
            {
                return;
            }

            _bounciness = value;
            UpdatePhysicsProperties();
        }
    }

    [Export(PropertyHint.Range, "0,1,0.05")]
    public float Friction
    {
        get => _friction;
        set
        {
            if (Mathf.IsEqualApprox(_friction, value))
            {
                return;
            }

            _friction = value;
            UpdatePhysicsProperties();
        }
    }

    [Export]
    public Vector2 InitialLinearVelocity
    {
        get => _initialLinearVelocity;
        set => _initialLinearVelocity = value;
    }

    public override void _Ready()
    {
        UpdateGeometry();
        UpdatePhysicsProperties();
        ResetInitialVelocity();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_initialVelocityApplied)
        {
            return;
        }

        LinearVelocity = _initialLinearVelocity;
        _initialVelocityApplied = true;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (!_initialVelocityApplied)
        {
            state.LinearVelocity = _initialLinearVelocity;
        }
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, _radius, _tint);
    }

    public void InitializeForPlay()
    {
        ResetInitialVelocity();
        QueueRedraw();
    }

    private void ResetInitialVelocity()
    {
        _initialVelocityApplied = false;
        LinearVelocity = Vector2.Zero;
        AngularVelocity = 0.0f;
        Sleeping = false;
    }

    private void UpdateGeometry()
    {
        if (!IsInsideTree())
        {
            return;
        }

        var collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (collisionShape is null)
        {
            collisionShape = new CollisionShape2D
            {
                Name = "CollisionShape2D"
            };
            AddChild(collisionShape);
        }

        if (collisionShape.Shape is CircleShape2D circleShape)
        {
            circleShape.Radius = _radius;
        }
        else
        {
            collisionShape.Shape = new CircleShape2D
            {
                Radius = _radius
            };
        }

        QueueRedraw();
    }

    private void UpdatePhysicsProperties()
    {
        if (!IsInsideTree())
        {
            return;
        }

        Mass = Mathf.Max(0.1f, _bodyMass);

        if (PhysicsMaterialOverride is not PhysicsMaterial material)
        {
            material = new PhysicsMaterial();
            PhysicsMaterialOverride = material;
        }

        material.Bounce = Mathf.Clamp(_bounciness, 0.0f, 1.0f);
        material.Friction = Mathf.Clamp(_friction, 0.0f, 1.0f);
    }
}
