using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BonusType
{
    plusSpeed, //увеличение скорости игрока
    minusEnemySpeed, //замедление врагов
    cherry, //вишинка
    candy, //конфета
    shield, //щит
    lives, //жизнь

    plusEnemySpeed, //увеличение скорости врагов
    minusSpeed, // замедление игрока
    minusLive //минус жизнь
};

public class Bonus : MonoBehaviour
{
    public BonusType TypeBonus { get; set; }

    GameManager gameManager; //переменная в которой храниться GameManager

    

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>(); //находим GameManager           
    }


    //метод удаления бонуса вне класса
    public void SelfDestroy()
    {
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Player plr = other.GetComponent<Player>();

        if (plr != null)
        {
            gameManager.ActivateBonus(TypeBonus); //активируем бонус в GameManager

            Destroy(gameObject);
        }

    }


    //метод возвращает индекс бонуса по его типу
    public static int BonusIndexByType(BonusType typeBonus)
    {
        int index;

        switch (typeBonus)
        {
            case BonusType.plusSpeed:

                index = 0;

                break;
            case BonusType.cherry:

                index = 1;

                break;
            case BonusType.candy:

                index = 2;

                break;
            case BonusType.minusEnemySpeed:

                index = 3;

                break;
            case BonusType.shield:

                index = 4;

                break;
            case BonusType.lives:

                index = 5;

                break;
            case BonusType.plusEnemySpeed:

                index = 6;

                break;
            case BonusType.minusSpeed:

                index = 7;

                break;
            case BonusType.minusLive:

                index = 8;

                break;
            default:

                index = 8;

                break;


        }

        return index; //возвращаем тип бонуса
    }


    //метод возвращает тип бонуса по индексу
    public static BonusType BonusTypeByIndex(int index)
    {
        BonusType bonType; //тип бонуса

        switch (index)
        {
            case 0:

                bonType = BonusType.plusSpeed;

                break;
            case 1:

                bonType = BonusType.cherry;

                break;
            case 2:

                bonType = BonusType.candy;

                break;
            case 3:

                bonType = BonusType.minusEnemySpeed;

                break;
            case 4:

                bonType = BonusType.shield;

                break;
            case 5:

                bonType = BonusType.lives;

                break;
            case 6:

                bonType = BonusType.plusEnemySpeed;

                break;
            case 7:

                bonType = BonusType.minusSpeed;

                break;
            case 8:

                bonType = BonusType.minusLive;

                break;
            default:

                bonType = BonusType.minusLive;

                break;


        }

        return bonType; //возвращаем тип бонуса
    }
}
