namespace Jichaels.VRSDK
{
    public interface IJVRLaserInteract : IJVRInteraction
    {
        void JVRLaserInteract(JVRLaserController jvrLaserController);

        void LaserHoverEnter(JVRLaserController jvrLaserController);
        void LaserHoverStay(JVRLaserController jvrLaserController);
        void LaserHoverExit(JVRLaserController jvrLaserController);
    }
}