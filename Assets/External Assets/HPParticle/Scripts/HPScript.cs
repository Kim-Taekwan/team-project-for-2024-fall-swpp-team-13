// this script controls the HP and Instantiates an HP Particle

using UnityEngine;
using TMPro;
using System.Collections;

public class HPScript : MonoBehaviour {

	//the HP Particle
	public GameObject HPParticle;

	//Default Forces
	public Vector3 DefaultForce = new Vector3(0f,1f,0f);
	public float DefaultForceScatter = 0.5f;

	public void ChangeHP(Vector3 Position)
	{
		Debug.Log("ChangeHP");
		GameObject NewHPP = Instantiate(HPParticle,Position,gameObject.transform.rotation) as GameObject;
		NewHPP.GetComponent<AlwaysFace>().Target = GameObject.Find("Main Camera").gameObject;
		
		TextMesh TM  = NewHPP.transform.Find("HPLabel").GetComponent<TextMesh>();

		TM.text = "-HP";
		TM.color =  new Color(1f,0f,0f,1f);

		
		NewHPP.GetComponent<Rigidbody>().AddForce( new Vector3(DefaultForce.x + Random.Range(-DefaultForceScatter,DefaultForceScatter),DefaultForce.y + Random.Range(-DefaultForceScatter,DefaultForceScatter),DefaultForce.z + Random.Range(-DefaultForceScatter,DefaultForceScatter)));
	}
	
}
