using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Scribble : VisualElement
{
    #region USS class names

    public static readonly string ussClassName = "scribble";

    #endregion

    #region Visual element implementation

    ScribbleManipulator _manipulator;

    public Scribble()
    {
        AddToClassList(ussClassName);
        _manipulator = new ScribbleManipulator(this);
        this.AddManipulator(_manipulator);
    }

    #endregion

    #region Command queue

    public enum EventType { Down, Move, Up, Clear }

    public struct Command
    {
        public EventType _type;
        public Vector2 _coords;

        public EventType Event => _type;
        public Vector2 Point => _coords;

        public static Command NewDown(Vector2 point)
          => new Command { _type = EventType.Down, _coords = point };

        public static Command NewMove(Vector2 point)
          => new Command { _type = EventType.Move, _coords = point };

        public static Command NewUp(Vector2 point)
          => new Command { _type = EventType.Up, _coords = point };
    }

    public bool IsLineSegmentAvailable
      => _manipulator.Commands.Count > 1;

    public (Vector2 p1, Vector2 p2) DequeueAsLineSegment()
    {
        var queue = _manipulator.Commands;
        var p1 = queue.Dequeue()._coords;
        while (queue.Count > 1)
        {
            var cmd = queue.Dequeue();
            if (cmd._type == EventType.Up)
                return (p1, cmd._coords);
        }
        var end = queue.Peek();
        if (end._type == EventType.Up) queue.Dequeue();
        return (p1, end._coords);
    }

    #endregion
}
