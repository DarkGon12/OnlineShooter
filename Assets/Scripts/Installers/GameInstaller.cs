using UnityEngine;
using Services;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GlobalManager _globalManager;
    [SerializeField] private NetworkService _networkService;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private GameState _gameState;

    public override void InstallBindings()
    {
        InstallServices();
    }

    private void InstallServices()
    {
        Container.Bind<GlobalManager>().FromInstance(_globalManager).AsSingle().NonLazy();
        Container.Bind<NetworkService>().FromInstance(_networkService).AsSingle();
        Container.Bind<PlayerManager>().FromInstance(_playerManager).AsSingle();
        Container.Bind<GameState>().FromInstance(_gameState).AsSingle();
    }
}
