namespace Jichaels.VRSDK
{
    public interface IJVRMouseInteract : IJVRInteraction
    {
        void PrimaryInteraction(JVRMouseController mouseController);
        void SecondaryInteraction(JVRMouseController mouseController);

        void MouseHoverEnter(JVRMouseController mouseController);
        void MouseHoverStay(JVRMouseController mouseController);

        void MouseHoverExit(JVRMouseController mouseController);

        CursorInfo HoverCursor { get; }
    }
}