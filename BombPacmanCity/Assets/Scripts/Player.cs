using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Player : BaseEntities
{
    GameManager gameManager;    

    public AudioClip SoundClipMove { get; set; } //звук шагов игрока    

    public int MyCoins { get; set; } //колличество монет игрока
    public int CherryCount { get; set; } //колличество вишенок
    public int CandyCount { get; set; } //колличество конфет
    public int Lives { get; private set; } //жизни игрока

    public bool IsActiveShield { get; set; } //флаг, активен ли щит    
    public bool IsDead { get; private set; } //флаг, мертв ли игрок

    float timerInmortal; //таймер, до выключения безсмертия

    bool isInmortal; //флаг, включено ли безсмертие
    bool isCoinPickUp; //влаг, поднята ли монетка


    public override void Start()
    {
        base.Start();

        gameManager = GameObject.FindObjectOfType<GameManager>(); //находим GameManager       

    }

    //задаем управление персонажем
    protected override void GetControl()
    {
        MoveControl();

        if (isInmortal)  //если я безсмертный, то запускаем корутину выключения безсмертия
            DisableInmortal();

        ShieldWork(); //работа щита
    }


    //метод отображает работу щита, если тот включен
    void ShieldWork()
    {
        if (IsActiveShield)
        {
            sprRender.color = Color.green;

            timerInmortal = 0f;
            isInmortal = false; //выключаем безсмертие
        }
        else if (!isInmortal) //если выключено безсмертие
        {
            sprRender.color = Color.white; //цвет по умолчанию
        }
    }


    //метод управления игрока
    void MoveControl()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (vertical != 0)
        {
            if (vertical > 0f)
                direction = Direction.UP;

            if (vertical < 0f)
                direction = Direction.DOWN;

            SetDirection(direction); //устанавливаем параметр анимации, и направления движения

        }
        else if (horizontal != 0)
        {
            if (horizontal > 0f)
                direction = Direction.RIGHT;

            if (horizontal < 0f)
                direction = Direction.LEFT;

            SetDirection(direction); //устанавливаем параметр анимации, и направления движения

        }
        else
        {
            move = Vector2.zero; //обнуляем вектор напрвления            
        }
    }


    //корутина отключения безсмертия
    void DisableInmortal()
    {
        if (timerInmortal > 0f)
        {
            timerInmortal -= 1f * Time.deltaTime;
        }
        else
        {
            timerInmortal = 0f;

            isInmortal = false; //выключаем безсмертие
            sprRender.color = Color.white;
            //Debug.Log("Игрок НЕ безсмертен");
        }

    }

    //метод инициализации переменных
    public override void Init(int type, RuntimeAnimatorController[] contrAnim)
    {
        base.Init(type, contrAnim);

        SetDirection(Direction.RIGHT);
        animator.speed = 0f;
        move = Vector3.zero; //обнкляем вектор передвижения игрока

        CandyCount = 0;
        CherryCount = 0;
        MyCoins = 0; //даем одну монетку при старте
        Lives = 3; //даем три жизни игроку при старте
         
        //настраиваем аудиоисточник на игроке
        audioSours.clip = SoundClipMove;
        audioSours.loop = false;
        audioSours.playOnAwake = false;

        isInit = true;
    }


    //метод смерти игрока
    public override void Dead()
    {
        if (IsDead) return; //если игрок мертв - возвращаемся 
        if (isInmortal) return; //если я безсмертный - возвращаемся         
        if (IsActiveShield) return; // если включён щит - возвращаемся               

        sprRender.color = Color.red;// меняем цвет спрайта
        Lives--; //отнимаем одну жизнь
        isInmortal = true; // включаем безсмертие 
        timerInmortal = 3f;

        gameManager.SoundDamage();        

        if (Lives <= 0)
        {
            IsDead = true;
            Lives = 0;
            sprRender.color = Color.red;// меняем цвет спрайта
            audioSours.Stop(); //останавливаем звук шагов
        }
    }
           

    //метод проигрывания звука шагов
    protected override void PlaySoundMove(bool isMove)
    {
        if (isMove)
        {
            if (!audioSours.isPlaying)
                audioSours.Play();
        }
        else
        {
            audioSours.Stop();
        }
    }


    //метод добавляет одну жизнь
    public void AddLive()
    {
        if (Lives < 3)
            Lives++;
    }


    //метод добавляет отнимает одну  жизнь
    public void RemoveLive()
    {
        if (Lives > 1)
        {
            Dead();
        }
    }


    //метод подбора монетки
    public void CoinPickUp()
    {
        MyCoins++;        
    }
}
