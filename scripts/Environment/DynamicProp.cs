using Godot;

namespace Detymora.Environment;

#nullable enable

public partial class DynamicProp : RigidBody2D
{
    [ExportSubgroup("Shape")]
    [Export] public Vector2 Size { get; set; } = new(64, 40);
    [Export] public Color Tint { get; set; } = new(0.6f, 0.45f, 0.2f);

    [ExportSubgroup("Dynamics")]
    [Export(PropertyHint.Range, "0.2,5,0.1")] public float BodyMass { get; set; } = 2.0f;
    [Export(PropertyHint.Range, "0,1,0.05")] public float Bounciness { get; set; } = 0.2f;
    [Export(PropertyHint.Range, "0,1,0.05")] public float Friction { get; set; } = 0.6f;

    public override void _Ready()
    {
        EnsureCollisionShape();
        ApplyMaterial();
        Mass = Mathf.Max(0.2f, BodyMass);
        QueueRedraw();
    }

    public override void _Draw()
    {
        var rect = new Rect2(-Size / 2.0f, Size);
        DrawRect(rect, Tint);
    }

    private void EnsureCollisionShape()
    {
        var collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (collisionShape is null)
        {
            collisionShape = new CollisionShape2D();
            AddChild(collisionShape);
        }

        if (collisionShape.Shape is RectangleShape2D rectangle)
        {
            rectangle.Size = Size;
        }
        else
        {
            collisionShape.Shape = new RectangleShape2D
            {
                Size = Size
            };
        }
    }

    private void ApplyMaterial()
    {
        var material = PhysicsMaterialOverride as PhysicsMaterial ?? new PhysicsMaterial();
        material.Bounce = Bounciness;
        material.Friction = Friction;
        PhysicsMaterialOverride = material;
    }
}
