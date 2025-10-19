using Godot;

namespace Detymora.Environment;

#nullable enable

[Tool]
public partial class DynamicProp : RigidBody2D
{
    private Vector2 _size = new(64, 40);
    private Color _tint = new(0.6f, 0.45f, 0.2f);
    private float _bodyMass = 2.0f;
    private float _bounciness = 0.2f;
    private float _friction = 0.6f;

    [ExportSubgroup("Geometry")]
    [Export]
    public Vector2 Size
    {
        get => _size;
        set
        {
            if (_size.IsEqualApprox(value))
            {
                return;
            }

            _size = value;
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
    [Export(PropertyHint.Range, "0.2,5,0.1")]
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

    public override void _Ready()
    {
        UpdateGeometry();
        UpdatePhysicsProperties();
    }

    public override void _Draw()
    {
        var rect = new Rect2(-_size / 2.0f, _size);
        DrawRect(rect, _tint);
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

        if (collisionShape.Shape is RectangleShape2D rectangle)
        {
            rectangle.Size = _size;
        }
        else
        {
            collisionShape.Shape = new RectangleShape2D
            {
                Size = _size
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

        Mass = Mathf.Max(0.2f, _bodyMass);

        if (PhysicsMaterialOverride is not PhysicsMaterial material)
        {
            material = new PhysicsMaterial();
            PhysicsMaterialOverride = material;
        }

        material.Bounce = Mathf.Clamp(_bounciness, 0.0f, 1.0f);
        material.Friction = Mathf.Clamp(_friction, 0.0f, 1.0f);
    }
}
