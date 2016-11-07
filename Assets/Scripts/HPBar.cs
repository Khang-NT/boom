using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour {
	float defaultXScale;

	public Sprite sprite1;
	public Sprite sprite2;

	public IHpValue hpValue;
	public MonoBehaviour hpComponent;
	public GameObject objectToFollow;
	public bool isUnEffect = false;
	// Use this for initialization
	void Start () {
		this.defaultXScale = transform.localScale.x;
		this.hpValue = (IHpValue) hpComponent;
		this.setIsUnEffect (true);
	}
	
	// Update is called once per frame
	void Update () {
		this.update ();
	}

	void setIsUnEffect(bool enable) {
		if (this.isUnEffect != enable) {
			this.isUnEffect = enable;
			GetComponent<SpriteRenderer> ().sprite = isUnEffect ? sprite2 : sprite1;
		}
	}

	private void update() {
		if (hpValue != null && objectToFollow != null) {
			Vector3 pos = this.transform.position;
			pos.x = objectToFollow.GetComponent<Renderer> ().bounds.min.x;
			pos.y = objectToFollow.GetComponent<Renderer> ().bounds.max.y;
			this.transform.position = pos;
			Vector3 scale = this.transform.localScale;
			scale.x = (defaultXScale * hpValue.Hp) / hpValue.MaxHp;
			this.transform.localScale = scale;
			setIsUnEffect (hpValue.IsUnEffect);
		} else {
			Destroy (this.gameObject);
		}
	}
}
