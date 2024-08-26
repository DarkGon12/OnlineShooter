using System;
using UnityEngine;
using UnityEngine.UI;
using Sfs2X.Entities;
using TMPro;

public class GameListItem : MonoBehaviour
{
	public Button playButton;
	public TextMeshProUGUI NameText;
	public TextMeshProUGUI DetailsText;
	public TextMeshProUGUI UserInRoomText;
	public TextMeshProUGUI MaxUsersText;

	public int roomId;

	public void Init(Room room)
	{
		NameText.text = room.Name;
		roomId = room.Id;

		SetState(room);
	}

	public void SetState(Room room)
	{
		int playerSlots = room.MaxUsers - room.UserCount;

		DetailsText.text = String.Format("{0}", playerSlots);
		MaxUsersText.text = String.Format("{0}", room.MaxUsers);
        UserInRoomText.text = String.Format("{0}", room.UserCount);

        playButton.interactable = playerSlots > 0;
	}
}
