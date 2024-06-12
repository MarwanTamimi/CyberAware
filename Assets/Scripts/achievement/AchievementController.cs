using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class AchievementController : MonoBehaviour
{

    [SerializeField] Text achievementTitleLabel;

    private Animator m_animator;



    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (transform.parent != null)
        {
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        m_animator = GetComponent<Animator>();
    }

    public void ShowNotification(Achievement achievement)
    {
    
        gameObject.SetActive(true);
        achievementTitleLabel.text = achievement.title;

        
        m_animator.SetTrigger("Appear");
        }


}