using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearts_System : MonoBehaviour
{
    public static Hearts_System Instance;
    [SerializeField] GameObject full_heart,half_heart,empty_heart,heart_content;
    private void Awake()
    {
        Instance = this;
    }

    public void Player_Create_Heart(int cur_player_health,int based_health)
    {
       Clear_Heart();
       Create_Heart(cur_player_health,based_health);
    }

    private void Clear_Heart()
    {
        foreach(Transform heart in heart_content.transform)
        {
           Destroy(heart.gameObject);
        }
    }

    private void Create_Heart(int cur_player_health,int based_health)
    {
        int fullHearts = cur_player_health / 2;
        int halfHeart = cur_player_health % 2;
        int emptyHearts = Mathf.Max(0, based_health - cur_player_health) / 2;
        for(int i = 0; i<fullHearts;i++)
        {
            Instantiate(full_heart,heart_content.transform);
        }
        if(halfHeart==1)
        {
            Instantiate(half_heart,heart_content.transform);
        }
        //Create EmptyHeart
        for(int i = 0; i<emptyHearts;i++)
        {
            Instantiate(empty_heart,heart_content.transform);
        }
    }
}
