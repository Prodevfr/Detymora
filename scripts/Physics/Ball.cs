using Godot;

namespace Detymora.Physics;

#nullable enable

public partial class Ball : RigidBody2D
{
    [ExportSubgroup("Visual")]
    [Export] public float Radius { get; set; } = 18.0f;
    [Export] public Color Tint { get; set; } = new(0.8f, 0.2f, 0.2f);

    [ExportSubgroup("Dynamics")]
    [Export(PropertyHint.Range, "0.1,10,0.1")] public float BodyMass { get; set; } = 1.0f;
    [Export(PropertyHint.Range, "0,1,0.05")] public float Bounciness { get; set; } = 0.6f;
    [Export(PropertyHint.Range, "0,1,0.05")] public float Friction { get; set; } = 0.4f;
    [Export] public Vector2 InitialLinearVelocity { get; set; } = Vector2.Zero;

    private bool _initialVelocityApplied;

    public override void _Ready()
    {
        EnsureCollisionShape();
        ApplyMaterialProperties();
        UpdateMass();
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_initialVelocityApplied)
        {
            return;
        }

        LinearVelocity = InitialLinearVelocity;
        _initialVelocityApplied = true;
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (!_initialVelocityApplied)
        {
            state.LinearVelocity = InitialLinearVelocity;
        }
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, Radius, Tint);
    }

    public void InitializeForPlay()
    {
        _initialVelocityApplied = false;
        LinearVelocity = Vector2.Zero;
        AngularVelocity = 0.0f;
        Sleeping = false;
        QueueRedraw();
    }

    private void EnsureCollisionShape()
    {
        var shapeNode = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (shapeNode is null)
        {
            shapeNode = new CollisionShape2D();
            AddChild(shapeNode);
        }

        if (shapeNode.Shape is CircleShape2D circleShape)
        {
            circleShape.Radius = Radius;
        }
        else
        {
            shapeNode.Shape = new CircleShape2D
            {
                Radius = Radius
            };
        }
    }

    private void ApplyMaterialProperties()
    {
        var material = PhysicsMaterialOverride as PhysicsMaterial ?? new PhysicsMaterial();
        material.Bounce = Bounciness;
        material.Friction = Friction;
        PhysicsMaterialOverride = material;
    }

    private void UpdateMass()
    {
        Mass = Mathf.Max(0.1f, BodyMass);
    }
}
