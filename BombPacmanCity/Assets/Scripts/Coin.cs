using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Coin : MonoBehaviour
{

    GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    

    //если с монеткой столкнулся игрок, то удалить монетку,
    //и добавить к счетчику монет в GameManager +1
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player plr = other.GetComponent<Player>();

        if (plr != null)
        {
            plr.CoinPickUp(); //добавляем монету игроку 
            gameManager.SoundCoin();
            Destroy(gameObject); //удалить себя

            //Debug.Log("МОНЕТКА подобрана, всего монет " + plr.MyCoins);
        }

    }

}
