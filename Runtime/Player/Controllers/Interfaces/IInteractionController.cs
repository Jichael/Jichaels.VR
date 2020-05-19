namespace Jichaels.VRSDK
{
    public interface IInteractionController
    {
        void InitializeController(JVRPlayer player);
        void UpdateController();
        void PrimaryAction();
        void SecondaryAction();
    }
}