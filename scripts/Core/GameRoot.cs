using System;
using System.Collections.Generic;
using Detymora.Physics;
using Godot;

namespace Detymora.Core;

#nullable enable

public partial class GameRoot : Node2D
{
    [ExportGroup("Scene References")]
    [Export] public NodePath? BallContainerPath { get; set; }
    [Export] public NodePath? EnvironmentContainerPath { get; set; }

    private readonly List<Ball> _balls = new();
    private Node2D? _ballContainer;
    private Node2D? _environmentContainer;

    public override void _Ready()
    {
        _ballContainer = ResolveNode<Node2D>(BallContainerPath, nameof(BallContainerPath));
        _environmentContainer = ResolveNode<Node2D>(EnvironmentContainerPath, nameof(EnvironmentContainerPath));

        _balls.Clear();
        if (_ballContainer is not null)
        {
            foreach (var child in _ballContainer.GetChildren())
            {
                if (child is Ball ball)
                {
                    ball.InitializeForPlay();
                    _balls.Add(ball);
                }
            }
        }
    }

    public void ResetSimulation()
    {
        // Keeping reload logic explicit allows future expansion (e.g., warm start from sub-viewport results).
        Callable.From(() => GetTree().ReloadCurrentScene()).CallDeferred();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("reset_simulation"))
        {
            ResetSimulation();
        }
    }

    private T? ResolveNode<T>(NodePath? path, string propertyName) where T : class
    {
        if (path is null || path.IsEmpty)
        {
            GD.PushWarning($"{nameof(GameRoot)}: {propertyName} is not set.");
            return null;
        }

        var node = GetNodeOrNull(path);
        if (node is T typedNode)
        {
            return typedNode;
        }

        GD.PushWarning($"{nameof(GameRoot)}: Node at '{path}' is not of expected type {typeof(T).Name}.");
        return null;
    }
}
