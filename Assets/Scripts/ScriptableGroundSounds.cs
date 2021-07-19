using UnityEngine;


[System.Serializable]
public class GroundSound
{
    public GroundType groundType;
    public AudioClip[] audioClips;
}
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableGroundSounds", order = 1)]
public class ScriptableGroundSounds : ScriptableObject
{
    public GroundSound[] groundSounds;

    public AudioClip playSound(GroundType p_groundType)
	{
		AudioClip clip = groundSounds[0].audioClips[Random.Range(0, groundSounds[0].audioClips.Length)];

		foreach (var item in groundSounds)
		{
            if(item.groundType == p_groundType)
			{
				clip = item.audioClips[Random.Range(0,item.audioClips.Length)];
			}
		}



        return clip;

	}
}