using UnityEngine;
using System.Collections;

public interface IHpValue {
	int Hp {
		get;
	}

	int MaxHp {
		get;
	}

	bool IsUnEffect {
		get;
	}
}
