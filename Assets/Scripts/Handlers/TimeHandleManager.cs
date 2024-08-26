using Sfs2X.Entities.Data;
using UnityEngine;
using Services;
using Zenject;
using System;
using Sfs2X;

namespace Handlers
{
    public class TimeHandleManager : MonoBehaviour
    {
        private NetworkService _networkService;

        [SerializeField] private double _lastServerTime = 0;
        [SerializeField] private double _lastLocalTime = 0;

        [Inject]
        private void Construct(NetworkService netService)
        {
            _networkService = netService;
        }

        public void HandleServerTime(ISFSObject sfsobject, SmartFox sfs)
        {
            long time = sfsobject.GetLong("t");
            double timePassed = _networkService.GetClientServerLag() / 2.0f;
            _lastServerTime = Convert.ToDouble(time) + timePassed;
            _lastLocalTime = Time.time;
        }
    }
}