using Godot;

namespace Detymora.Environment;

#nullable enable

public partial class StaticPlatform : StaticBody2D
{
    [ExportSubgroup("Shape")]
    [Export] public Vector2 Size { get; set; } = new(320, 32);
    [Export] public Color Tint { get; set; } = new(0.3f, 0.3f, 0.35f);

    public override void _Ready()
    {
        EnsureCollisionShape();
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
}
