using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;

public class LobbySceneController : BaseSceneController
{
	public static LobbySceneController Instance { get; private set; }

    private static string GAME_ROOMS_GROUP_NAME = "games";
    private const string EXTENSION_ID = "Shooter";
    private const string EXTENSION_CLASS = "sfs2x.extensions.games.shooter.ShooterExtension";

    public Text loggedInAsLabel;

	public WarningPanel warningPanel;

	public Transform gameListContent;
	public GameListItem gameListItemPrefab;

	private SmartFox sfs;
	private Dictionary<int, GameListItem> gameListItems;

	private int _playerCount = 10;

	public void SetPlayerCount(int count) => _playerCount = count;

    private void Start()
	{
		Instance = this;
		sfs = _gm.GetSfsClient();

		HideModals();

		loggedInAsLabel.text = "Вы авторизованны как <b>" + sfs.MySelf.Name + "</b>";

		AddSmartFoxListeners();

		PopulateGamesList();
	}

	#region
	public void OnLogoutButtonClick() => sfs.Disconnect();

	public void OnStartGameButtonClick()
	{
		RoomSettings settings = new RoomSettings(sfs.MySelf.Name + "'s комната");
		settings.GroupId = GAME_ROOMS_GROUP_NAME;
		settings.IsGame = true;
        settings.MaxUsers = (short)_playerCount;
        settings.MaxSpectators = 0;
        settings.Extension = new RoomExtension(EXTENSION_ID, EXTENSION_CLASS);
		settings.MaxVariables = 20;

        sfs.Send(new CreateRoomRequest(settings, true, sfs.LastJoinedRoom));
    }

	public void OnGameItemPlayClick(int roomId) => sfs.Send(new JoinRoomRequest(roomId));	
	#endregion

	#region
	private void AddSmartFoxListeners()
	{
		sfs.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
		sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
		sfs.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);
		sfs.AddEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChanged);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
	}

	override protected void RemoveSmartFoxListeners()
	{
		sfs.RemoveEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
		sfs.RemoveEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
		sfs.RemoveEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);
		sfs.RemoveEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChanged);
		sfs.RemoveEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
		sfs.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
	}

	override protected void HideModals()
	{
		warningPanel.Hide();
	}

	private void PopulateGamesList()
	{
		if (gameListItems == null)
			gameListItems = new Dictionary<int, GameListItem>();

		List<Room> rooms = sfs.RoomManager.GetRoomList();

		foreach (Room room in rooms)
			AddGameListItem(room);
	}

	private void AddGameListItem(Room room)
	{
		if (!room.IsGame || room.IsHidden || room.IsPasswordProtected)
			return;

		GameListItem gameListItem = Instantiate(gameListItemPrefab);
		gameListItems.Add(room.Id, gameListItem);

		gameListItem.Init(room);

		gameListItem.playButton.onClick.AddListener(() => OnGameItemPlayClick(room.Id));

		gameListItem.gameObject.transform.SetParent(gameListContent, false);
	}
	#endregion

	#region
	private void OnRoomCreationError(BaseEvent evt)
	{
		warningPanel.Show("Room creation failed: " + (string)evt.Params["errorMessage"]);
        Console.Send("Не удалось создать комнату", ConsoleMessageType.error);
    }

	private void OnRoomAdded(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];

		AddGameListItem(room);
	}

	public void OnRoomRemoved(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];

		gameListItems.TryGetValue(room.Id, out GameListItem gameListItem);

		if (gameListItem != null)
		{
			gameListItem.playButton.onClick.RemoveAllListeners();

			gameListItems.Remove(room.Id);

			GameObject.Destroy(gameListItem.gameObject);
		}
	}

	public void OnUserCountChanged(BaseEvent evt)
	{
		Room room = (Room)evt.Params["room"];

		gameListItems.TryGetValue(room.Id, out GameListItem gameListItem);

		if (gameListItem != null)
			gameListItem.SetState(room);
	}

	private void OnRoomJoin(BaseEvent evt) => SceneManager.LoadScene(2);
	
	private void OnRoomJoinError(BaseEvent evt)
	{
		warningPanel.Show("Room join failed: " + (string)evt.Params["errorMessage"]);
		Console.Send("Не удалось соедининтся с комнатой", ConsoleMessageType.error);
	}
	#endregion
}
