using UnityEngine;
using Sendlers;
using Zenject;

public class SendersInstaller : MonoInstaller
{
    [SerializeField] private SpawnRequestSender _spawnRequestSender;
    [SerializeField] private TransformSender _transformSender;
    [SerializeField] private WeaponSender _weaponSender;

    public override void InstallBindings()
    {
        InstallServices();
    }

    private void InstallServices()
    {
        Container.Bind<SpawnRequestSender>().FromInstance(_spawnRequestSender).AsSingle();
        Container.Bind<TransformSender>().FromInstance(_transformSender).AsSingle();
        Container.Bind<WeaponSender>().FromInstance(_weaponSender).AsSingle();
    }
}