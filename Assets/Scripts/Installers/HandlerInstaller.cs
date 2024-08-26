using UnityEngine;
using Handlers;
using Zenject;

public class HandlerInstaller : MonoInstaller
{
    [SerializeField] private AnimationHandleManager _animationHandleManager;
    [SerializeField] private TransformHandleManager _tranformHandler;
    [SerializeField] private HealthHandleManager _healthHandleManager;
    [SerializeField] private ShootHandleManager _shootHandleManager;
    [SerializeField] private ChatHandleManager _chatHandleManager;
    [SerializeField] private KillHandleManager _killHandleManager;
    [SerializeField] private WeaponHandleManager _weaponManager;
    [SerializeField] private ReloadHandleManager _reloadManager;
    [SerializeField] private ScoreHandleManager _scoreManager;
    [SerializeField] private AmmoHandleManager _ammoManager;
    [SerializeField] private ItemHandleManager _itemManager;
    [SerializeField] private TimeHandleManager _timeManager;

    public override void InstallBindings()
    {
        InstallServices();
    }

    private void InstallServices()
    {
        Container.Bind<TransformHandleManager>().FromInstance(_tranformHandler).AsSingle();
        Container.Bind<HealthHandleManager>().FromInstance(_healthHandleManager).AsSingle();
        Container.Bind<AnimationHandleManager>().FromInstance(_animationHandleManager).AsSingle();
        Container.Bind<ShootHandleManager>().FromInstance(_shootHandleManager).AsSingle();
        Container.Bind<KillHandleManager>().FromInstance(_killHandleManager).AsSingle();
        Container.Bind<ChatHandleManager>().FromInstance(_chatHandleManager).AsSingle();
        Container.Bind<WeaponHandleManager>().FromInstance(_weaponManager).AsSingle();
        Container.Bind<ReloadHandleManager>().FromInstance(_reloadManager).AsSingle();
        Container.Bind<ScoreHandleManager>().FromInstance(_scoreManager).AsSingle();
        Container.Bind<AmmoHandleManager>().FromInstance(_ammoManager).AsSingle();
        Container.Bind<ItemHandleManager>().FromInstance(_itemManager).AsSingle();
        Container.Bind<TimeHandleManager>().FromInstance(_timeManager).AsSingle();
    }
}