using Godot;

namespace Detymora.Environment;

#nullable enable

[Tool]
public partial class StaticPlatform : StaticBody2D
{
    private Vector2 _size = new(320, 32);
    private Color _tint = new(0.3f, 0.3f, 0.35f);

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

    public override void _Ready()
    {
        UpdateGeometry();
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
}
