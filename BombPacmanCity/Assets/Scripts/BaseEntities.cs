using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//направления движения
public enum Direction { DOWN, LEFT, RIGHT, UP };

//вешаем компонеты на объект
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]

public abstract class BaseEntities : MonoBehaviour
{
    protected Vector2 move; //вектор передвижения
    protected Vector2 myPosition2D; //моя позиция

    //массив векторов передвижения
    protected Vector2[] Moves = { new Vector2(0f, -1f), //down
                                  new Vector2(-1f, 0f), //left
                                  new Vector2(1f, 0f), //right
                                  new Vector2(0f, 1f) //up
                                };

    protected float speed = 2f; //скорость передвижения
    protected float speedDefault; // скорость по умолчанию
    protected Direction direction; //папраление движения
    protected int typeEntitie; //тип объекта
    protected float animSpeed; //скорость анимации
    protected Vector2 targetPosition; //будущая позиция    

    protected RuntimeAnimatorController[] controllerAnim; // контроллеров анимации

    protected Animator animator; //аниматор
    protected SpriteRenderer sprRender;    
    protected BoxCollider2D col2D;
    protected Rigidbody2D rBody;
    public AudioSource audioSours; //звук

    protected bool isInit = false; //флаг, поинициализированы ли переменные
    
    public virtual void Start()
    {
        isInit = false;

        audioSours = GetComponent<AudioSource>();

        animator = GetComponent<Animator>();
        animSpeed = speed / 2f; //скорость анимации скорость анимации устанавливаем половину скорости движения
        animator.speed = 0f; //ставим скорость анимации в 0        

        sprRender = GetComponent<SpriteRenderer>();
        sprRender.sortingOrder = 100; //устанавливаем глубину отрисовки в 100 чтоб повер всех спрайтов                     

        col2D = GetComponent<BoxCollider2D>();
        col2D.size.Set(1f, 1f);

        rBody = GetComponent<Rigidbody2D>(); //получаем компонет Rigidbody2D

        myPosition2D = (Vector2)transform.position;
        targetPosition = myPosition2D; //отмечаем что мы уже находимся в нужной позиции

        speedDefault = speed; //устанавливаем скорость по умолчанию

    }

    public void Update()
    {
        if (!isInit) return; //возвращаемся из метода если переменные не поинициализированы

        Move();
    }


    public void OnGUI()
    {
        if (!isInit) return; //возвращаемся из метода если переменные не поинициализированы

        GetControl();
    }


    //метод передвижения
    void Move()
    {
        if (targetPosition == myPosition2D && !IsForwardObstacle())
        {
            targetPosition = myPosition2D + move * 1.28f;
            
            animator.speed = 0f; //обнуляем скорость анимации
        }
        else
        {
            //если нет препятствий, то двигаем персонажа к цели
            myPosition2D = Vector2.MoveTowards(myPosition2D, targetPosition, speed * Time.deltaTime);

            animator.SetInteger("DIRECTION", (int)direction); //меняем парметр для внимации

            PlaySoundMove(targetPosition != myPosition2D); //если не достиг позиции, то проигрываем звук
        }

        transform.position = myPosition2D; //присваиваем позицию трансформу   
    }


    //метод переключает анимацию, скорость анимации , вектор направления движения
    protected void SetDirection(Direction dir)
    {
        int dirInt = (int)dir; //присваиваем направление

        animator.speed = animSpeed; //устанавливаем скорость анимации        

        move = Moves[dirInt]; //присваиваем нужный вектор направления      
    }


    public virtual void Init(int type, RuntimeAnimatorController[] contrAnim)
    {
        controllerAnim = contrAnim; //присваиваем контроллер в переменную
        typeEntitie = type;
        animator.runtimeAnimatorController = controllerAnim[0]; //назначаем контроллер анимации

        isInit = true; //отмечаем, что пременные проинициализированы
    }


    //метод проверки на препятствие
    protected bool IsForwardObstacle()
    {
        //проверяем есть ли в переди что-то на слое "Wall"
        return Physics2D.Raycast(myPosition2D, move, move.magnitude, LayerMask.GetMask("Wall"));
    }

    protected abstract void PlaySoundMove(bool isMove); //абстрактный метод проигрывания звуков
    protected abstract void GetControl(); //мето задающий управление персонажем
    public abstract void Dead(); //метод смерти персонажа


    //метод остановки игры
    public void StopGame()
    {
        if(animator)
            animator.speed = 0f;

        isInit = false; //запрещаем движение 
    }


    //метод замедляющий уменшающий скорость
    public void MinusSpeed()
    {
        speed--;
    }


    //метод замедляющий увеличивающий скорость
    public void PlusSpeed()
    {
        speed++;
    }


    //метод устанавливает скорость по умолчанию
    public void SpeedDefault()
    {
        speed = speedDefault;
    }

}
