using System.Collections.Generic;

public interface IGameState
{
    int PlayerCount { get; set; }
    List<string> PlayerNames { get; }
}
