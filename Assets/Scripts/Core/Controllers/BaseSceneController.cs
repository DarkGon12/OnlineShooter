using UnityEngine;
using Zenject;

public abstract class BaseSceneController : MonoBehaviour
{
	protected GlobalManager _gm;

	[Inject]
    protected void Construct(GlobalManager gm)
    {
		_gm = gm;
    }

    protected virtual void Update()
	{
		if (Input.GetKeyDown("escape"))
			HideModals();
	}

	protected virtual void OnDestroy()
	{
		RemoveSmartFoxListeners();
	}

	protected abstract void RemoveSmartFoxListeners();

	protected abstract void HideModals();
}
