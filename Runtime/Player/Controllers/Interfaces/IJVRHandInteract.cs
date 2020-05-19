namespace Jichaels.VRSDK
{
    public interface IJVRHandInteract : IJVRInteraction
    {
        void HandInteractionStart(JVRHandController jvrHandController);
        void HandInteractionStay(JVRHandController jvrHandController);
        void HandInteractionStop(JVRHandController jvrHandController);
    }
}