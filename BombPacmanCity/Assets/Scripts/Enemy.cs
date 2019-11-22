using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : BaseEntities
{
    int dir = 0; //направление
    float timerMove = 6f; //таймер смены направления

    //задаем управление персонажем
    protected override void GetControl()
    {
        timerMove -= Time.deltaTime; //отнимаем каждый кадр от таймера

        //если мы встретили препятствие, или таймер закончился
        //то меняем направление
        if (IsForwardObstacle() || timerMove <= 0f)
        {
            //dir = Random.Range(Random.Range(0, 1), Random.Range(2, 3)); //выбираем случайное направление
            dir++;
            if (dir > 3) dir = 0;

            //задаем таймер на случайное значение в диапазоне
            timerMove = Random.Range(Random.Range(2, 6) , Random.Range(7, 13)); 
            
        }

        switch(dir)
        {
            case 0:
                direction = Direction.DOWN;
                break;
            case 1:
                direction = Direction.LEFT;
                break;
            case 2:
                direction = Direction.RIGHT;
                break;
            case 3:
                direction = Direction.UP;
                break;
        }

        SetDirection(direction); //устанавливаем  направления движения
    }


    //метод смерти противника
    public override void Dead()
    {
        Destroy(this);
    }


    public override void Init(int type, RuntimeAnimatorController[] contrAnim)
    {
        base.Init(type, contrAnim);

        int indexAnim = Random.Range(0, contrAnim.Length); //выбираем случайный аниматор
        animator.runtimeAnimatorController = controllerAnim[indexAnim]; //назначаем контроллер анимации

        SetDirection(Direction.LEFT);
        animator.speed = 0f;
        move = Vector2.zero;
        dir = Random.Range(0, 3); //при старту задать случайное направление

        isInit = true;
    }        


    private void OnTriggerEnter2D(Collider2D other)
    {
        Player plr = other.GetComponent<Player>();

        if (plr != null)
        {
            plr.Dead(); // отнимаем жизнь у игрока            
        }

    }

    protected override void PlaySoundMove(bool isMove) { }
}
