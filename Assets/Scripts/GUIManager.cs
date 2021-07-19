using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Image lifeBar;
    public Image lifeBarEffect;
    public PlayerControl player;
    public GameObject floatingDamagePrefab;
    public TextMeshProUGUI timer;
    public GameObject finishGui;
    public Text finishText;
    public float velocity;
    public bool finish;
    public GameObject screen;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        finishGui.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        lifeBar.fillAmount = Mathf.Lerp(lifeBar.fillAmount,(float)player.currentLife/(float)player.maxlife,Time.deltaTime* velocity);
        lifeBarEffect.fillAmount = lifeBar.fillAmount;
		if (!finish)
		{
            timer.text = System.TimeSpan.FromSeconds((int)Time.timeSinceLevelLoad).ToString();
        }
        


		if (Input.GetKeyDown(KeyCode.Escape))
		{
            Application.Quit();
		}
        
        if (Input.GetKeyDown(KeyCode.F1))
		{
            SceneManager.LoadScene(0);
        }

		if (Input.GetKeyDown(KeyCode.F3))
		{
            screen.SetActive(!screen.activeInHierarchy);

        }
    }

    public void Damaged(int damage)
	{
        StartCoroutine(FadeTo(2));
	}



    IEnumerator FadeTo(float aTime)
    {
       // for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * aTime)
      //  {
            Color newColor1 = new Color(lifeBarEffect.color.r, lifeBarEffect.color.g, lifeBarEffect.color.b, 1);
            lifeBarEffect.color = newColor1;
          //  yield return new WaitForEndOfFrame();
       // }

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime * aTime)
        {
            Color newColor = new Color(lifeBarEffect.color.r, lifeBarEffect.color.g, lifeBarEffect.color.b, 1-t);
            lifeBarEffect.color = newColor;
            yield return new WaitForEndOfFrame();
        }
    }



    public void FloatingText(Transform transform,string text,Color textColor)
    {
        Transform floating = GameObject.Instantiate(floatingDamagePrefab, transform.position + new Vector3(0,0.2f,0), floatingDamagePrefab.transform.rotation).transform;
        floating.GetComponentInChildren<TextMeshProUGUI>().text = text;
        floating.GetComponentInChildren<TextMeshProUGUI>().color = textColor;
        floating.GetComponentInChildren<FollowTarget>().SetTarget(transform, new Vector3(0, 0.2f, 0));

       // floating.SetParent(transform);
    }

    public void Finish()
	{
        finish = true;
        finishText.text = timer.text;
        finishGui.SetActive(true);


    }

}
