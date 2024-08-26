using Zenject;

public class PlayerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<FPSController>().FromComponentOnRoot().AsSingle();
    }
}
