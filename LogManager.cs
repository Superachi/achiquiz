using Achi.Godot.Common;
using Godot;

namespace AchiQuiz
{
    public partial class LogManager : CanvasLayer
    {
        [Export]
        public PackedScene LogEntryScene { get; set; } = null!;

        public static Singleton<LogManager> Singleton { get; private set; } = new();

        public override void _Ready()
        {
            Singleton.MarkAsSingleton(this);
        }

        public override void _Notification(int notification)
        {
            if (notification == NotificationPredelete)
            {
                Singleton.ClearSingleton();
            }
        }

        public override void _Process(double delta)
        {
        }
    }
}
