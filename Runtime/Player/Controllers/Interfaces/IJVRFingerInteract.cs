namespace Jichaels.VRSDK
{
    public interface IJVRFingerInteract : IJVRInteraction
    {
        void InteractionStart(JVRFingerController jvrFingerController);
        void InteractionStay(JVRFingerController jvrFingerController);
        void InteractionStop(JVRFingerController jvrFingerController);
    }
}