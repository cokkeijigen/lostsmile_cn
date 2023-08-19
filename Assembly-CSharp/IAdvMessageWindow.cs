using UnityEngine;
using Utage;

public interface IAdvMessageWindow
{
	GameObject gameObject { get; }

	void OnInit(AdvMessageWindowManager windowManager);

	void OnReset();

	void OnChangeCurrent(bool isCurrent);

	void OnChangeActive(bool isActive);

	void OnTextChanged(AdvMessageWindow window);
}
