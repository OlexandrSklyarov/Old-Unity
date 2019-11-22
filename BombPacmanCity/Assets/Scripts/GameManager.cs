using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region GameManager

    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                if (instance == null) Debug.Log("No GameManager object in scene!!!");

                return instance;
            }

            return instance;
        }
    }

    #endregion

    #region PERSON

    Player player; //переменная игрока
    List<Enemy> enemysList = new List<Enemy>(); //список врагов

    #endregion

    #region UI

    Canvas canvasMassage; //переменная холста
    Text textMassage; // текст который должен отображаться в сообщении

    Canvas visualInfo; //холст с информацией во время игры
    Text textCherry, textCandy, textLives, textCoins, textLevels; //текстовые переменные для отображения инфы

    #endregion

    #region ANIMATORS

    public RuntimeAnimatorController[] animatorPlayer; //аниматор игрока
    public RuntimeAnimatorController[] animatorEnemy; //аниматор противника
    public RuntimeAnimatorController animatorCoin; //аниматор монетки

    #endregion

    #region SPRITE

    public Sprite sprFreeСell;
    public Sprite sprBox;
    public Sprite sprObsrtacle;
    public Sprite sprCoin;

    public Sprite[] sprBonus; //масив спрайтов бонусов

    #endregion

    #region LEVELS

    int coinsCount; //счетчик всех монет    

    public bool genMapTexFile; //переключатель способа генирировать уровень (из текстового файла / по заданому размеру)

    public int numberlevel; //номер текущего уровня (для теста публичная)

    public int maxLevel; //максимальное количество уровней

    int sizeX, sizeY; //переменные размера карты

    bool levelCreate = false; //флаг, создан ли уровень

    GameObject allSpritesLevel; //объект в котором будут храниться все спрайты на сцене на конкретном уровне     

    GameObject coinsLevel; //объект для хранения всех монеток    

    TextAsset[] levelMaps; // масив уровней в текстовом формате

    List<Vector2> freeCallPosition = new List<Vector2>(); //список позиций пустых ячеек   
       
    bool isGamePlay; //флаг, разрешена ли игра

    #endregion

    #region BONUS

    float timerDiactivateBonus; //таймер действия бонуса

    float timerCreateBonus; //таймер до создания следующего бонуса   

    float timerDestroyBonus; //таймер до удаления бонуса, если он есть на уровне

    bool bonusExist; //флаг, есть ли бонус на уровне

    BonusType activeBonus; //активный бонус

    bool isBonusActive; //флаг, активирован ли какой-нибуть бонус    

    Bonus bonus; //бонус в игре

    #endregion

    #region Sounds    

    AudioSource audioSoursSFX;

    public AudioClip sfxCoinsSound;
    public AudioClip sfxBonusesSound;
    public AudioClip sfxDamageSound;
    public AudioClip sfxPlayerMoveSound;

    #endregion
    

    void Start()
    {
        audioSoursSFX = GameObject.Find("SFX_sound").GetComponent<AudioSource>(); //находим внешний источник звука

        levelMaps = Resources.LoadAll<TextAsset>("Levels"); //получаем все карты уровней из папки Levels

        canvasMassage = GameObject.Find("CanvasMassage").GetComponent<Canvas>(); //находим в сцене канвас и присваиваем в переменную
        textMassage = canvasMassage.transform.Find("Text").GetComponent<Text>(); //получаем текст с канваса 
        canvasMassage.enabled = false; //отключаем холст с сообщениями

        visualInfo = GameObject.Find("VisualInfo").GetComponent<Canvas>(); //канвас с инфой игрока
        visualInfo.enabled = false; //отключаем холст с инфой игрока
        GameObject panelInfo = visualInfo.transform.Find("Panel").gameObject;
        textCherry = panelInfo.transform.Find("TextCherry").GetComponent<Text>(); //получаем текст вишенок
        textCandy = panelInfo.transform.Find("TextCandy").GetComponent<Text>(); //получаем текст конфет
        textLives = panelInfo.transform.Find("TextLives").GetComponent<Text>(); //получаем текст жизней
        textCoins = panelInfo.transform.Find("TextCoins").GetComponent<Text>(); //получаем текст монет
        textLevels = panelInfo.transform.Find("TextLevels").GetComponent<Text>(); //получаем текст номера уровня        

        coinsCount = 0; //начальное количество монет на уровне        

        CreateLevel(); //создаем уровень, если он еще не создан

    }


    void Update()
    {
        if (!isGamePlay) return;

        QuitGame();

        //если игрок выиграл
        if (IsWinner())
            WinGame();

        //если игрок проиграл
        if (IsLoseGame())
            GameOver();

        if (isBonusActive) //если есть активный бонус деактивируем его по таймеру
            BonusDisable();

        BonusGeneration(); //создать бонус

        SetTextInfo(); //задаем значения переменным очков и жизней

    }


    //метод задает значения текстовым переменным для отображения на экране
    void SetTextInfo()
    {
        //передаем в переменные значения счетчиков
        textCherry.text = " = " + player.CherryCount.ToString();
        textCandy.text = " = " + player.CandyCount.ToString();
        textLives.text = " = " + player.Lives.ToString();
        textCoins.text = "= " + player.MyCoins.ToString();
        textLevels.text = "LEVEL - " + (numberlevel + 1);
    }


    //метод выиграша в игре
    void WinGame()
    {
        //Debug.Log("ПБЕДА!!!");
        isGamePlay = false;//запрещаем игру

        StopGame();

        textMassage.text = "YOU WIN !!! coins: " + "coins: " + player.MyCoins;

        //если это последний уровень, меняем сообщение
        if (numberlevel == maxLevel)
            textMassage.text = "!!! GAME СOMPLITE !!!" + "coins: " + player.MyCoins;

        canvasMassage.enabled = true; //включаем канвас

        StartCoroutine(LoadNextLevel()); //загружаем новый уровень
    }


    //метод конца игры
    void GameOver()
    {
        isGamePlay = false; //запрещаем игру

        textMassage.text = "GAME OVER :(";

        canvasMassage.enabled = true; //включаем канвас

        StopGame();

        StartCoroutine(LoadFirstLevel());
    }


    //остановка игры
    void StopGame()
    {
        player.StopGame(); //останавливаем игрока

        foreach (var enemy in enemysList) //останавливаем врагов
            enemy.StopGame();
    }


    //метод очистки уровня
    void ClearLevel()
    {
        canvasMassage.enabled = false; //включаем канвас с сообщениями
        visualInfo.enabled = false; //отключаем холст с инфой игр

        //удаляем все с уровня 

        Destroy(coinsLevel);
        Destroy(allSpritesLevel);

        freeCallPosition.Clear(); //очищаем список пустых ячеек       

        coinsCount = 0; //начальное количество монет на уровне           

        levelCreate = false; // отмечаем что уровень еще не создан       

    }

    //корутина загрузки нового уровня
    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(3f);

        if (numberlevel != maxLevel)
        {
            ClearLevel(); //очищаем уровень

            numberlevel++; //увеличиваем номер уровня            

            CreateLevel(); //создаем новый

            //Application.LoadLevel(numberlevel);

        }

    }


    //метод загрузки первого уровня
    IEnumerator LoadFirstLevel()
    {
        yield return new WaitForSeconds(3f);

        ClearLevel(); //очищаем уровень

        numberlevel = 0; //выставляем уровень в начальный        

        CreateLevel(); //создаем новый
        
    }


    //метод выбора
    void CreateLevel()
    {
        if (levelCreate) return; //если уровень создан, то возвращаемся

        coinsLevel = new GameObject("Coins");
        
        if (genMapTexFile)
            MapGenerationTextFile(); // генерируем карту из текстового файла
        else
        {
            switch (numberlevel)
            {
                case 0:
                    sizeX = 9;
                    sizeY = 9;
                    break;
                case 1:
                    sizeX = 11;
                    sizeY = 11;
                    break;
                case 2:
                    sizeX = 15;
                    sizeY = 13;
                    break;
            }

            MapGenerationSize(sizeX, sizeY); //генерация уровня по заданомо размеру
        }

        StartCoroutine(MakePlayer()); //создаем игрока

        StartCoroutine(MakeEnemys(3 + numberlevel)); //создаем противников (в колличестве от 3 и до + номер уровня)

        CoinCreate();

        visualInfo.enabled = true; //включаем панель информации

        isGamePlay = true; //разрешаем игру

        //Debug.Log("Количество на уровне монет: " + coinsCount);
    }


    //метод генерации уровня из текстового файла
    void MapGenerationTextFile()
    {
        string levMap = levelMaps[numberlevel - 1].text; //получаем карту нужного уровня

        allSpritesLevel = new GameObject("Level"); //создаем объект в котором будут храниться все спрайты уровня        

        GameObject go = null;

        //узнаем размер ширины и высоты карты и берем половину от каждого размера
        int height = SizeMapHeight(levMap) / 2;
        int width = SizeMapWidth(levMap) / 2;

        //выставляем начальное значение счетчиков
        int ix = -width;
        int iy = -height;

        Debug.Log("Width/2: " + ix + " Heigh/2: " + iy);

        foreach (char c in levMap)
        {
            Vector2 position = new Vector2(ix * 1.28f, -iy * 1.28f);
            ix++;

            //проверяем какой символ выпал
            switch (c)
            {
                case '#': //созжаем ограждение уровня

                    go = Utils.CreateSprite(position, sprBox, "Box", true);

                    break;
                case '*': //созжаем пустую ячейку, и записуем центр ее координат в список свободных ячеек

                    go = Utils.CreateSprite(position, sprFreeСell, "FreeCall", false);
                    Vector2 posFree = new Vector2(position.x + 0.64f, position.y - 0.64f);
                    freeCallPosition.Add(posFree); //добавляем позицию свободной ячейки в список                    

                    break;
                case '@': //созжаем препятствие

                    go = Utils.CreateSprite(position, sprObsrtacle, "Obsrtacle", true);

                    break;
                case ';': // переустанавливаем счетчики

                    ix = -width;
                    iy++;

                    break;
                default: //если какщй-то другой символ отнимаем 1 от счетчика по горизонтали

                    ix--;

                    break;
            }

            //если объект создался, то назначаем ему родителя allSpritesLevel
            if (go != null)
            {
                go.transform.parent = allSpritesLevel.transform;
                go = null; //обнуляем переменную
            }
        }

        levelCreate = true; // ставим флаг в true (уровень загружен)
    }


    //генирация уровня по заданым размерам
    void MapGenerationSize(int maxX, int maxY)
    {
        allSpritesLevel = new GameObject("Level"); //создаем объект в котором будут храниться все спрайты уровня

        GameObject go = null;

        //выставляем начальное значение счетчиков
        float width = -maxX * 1.28f / 2;
        float height = -maxY * 1.28f / 2;

        for (int i = 0; i < maxX; i++)
            for (int j = 0; j < maxY; j++)
            {
                Vector3 position = new Vector2(width + i * 1.28f, height + j * 1.28f);

                //создаем крайние боковые полосы из ящиков
                if (j > 0 && j < maxY - 1)
                    if (i == 0 || i == maxX - 1)
                    {
                        go = Utils.CreateSprite(position, sprBox, "Box", true);
                        go.layer = 8; // устанавливаем оюъекту слой 8 "Wall"
                    }


                //создаем крайние верхнюю и нижнюю полосу из ящиков
                if (j == 0 || j == maxY - 1)
                    if (i > 0 || i < maxX - 1)
                    {
                        go = Utils.CreateSprite(position, sprBox, "Box", true);
                        go.layer = 8; // устанавливаем оюъекту слой 8 "Wall"
                    }


                //создаем препятствия
                if (i > 0 && i < maxX - 1)
                    if (j > 0 && j < maxY - 1)
                        if (i % 2 == 0 && j % 2 == 0)
                        {
                            go = Utils.CreateSprite(position, sprObsrtacle, "Obsrtacle", true);
                            go.layer = 8; // устанавливаем оюъекту слой 8 "Wall"
                        }


                //создаем свободное место для передвижения
                if (i > 0 && i < maxX - 1)
                    if (j > 0 && j < maxY - 1)
                        if (i % 2 != 0 || j % 2 != 0)
                        {
                            go = Utils.CreateSprite(position, sprFreeСell, "FreeCall", false);
                            Vector2 posFree = new Vector2(position.x + 0.64f, position.y - 0.64f);
                            freeCallPosition.Add(posFree); //добавляем позицию в список свободных координат                            
                        }


                if (go != null)
                {
                    go.transform.parent = allSpritesLevel.transform;
                    go = null; //обнуляем переменную
                }
            }

        levelCreate = true; // ставим флаг в true (уровень загружен)
    }


    //метод возвращает размер карты в ширину (x)
    int SizeMapWidth(string map)
    {
        int count = 0;

        //пробегаем по всему масиву 
        //если встречаем символ ';' прерываем цикл
        foreach (char c in map)
        {
            if (c == ';')
                break;

            count++;
        }

        return count;
    }


    //метод возвращает размер карты в высоту (y)
    int SizeMapHeight(string map)
    {
        int count = 0;

        //пробегаем по всему масиву 
        //если встречаем символ ';' увеличиваем счетчик
        foreach (char c in map)
        {
            if (c == ';')
                count++;

        }

        return count;
    }


    //метод проверки победил ли игрок
    bool IsWinner()
    {
        //если игрок собрал все монеты то вернуть true
        if (player != null && player.MyCoins == coinsCount)
        {
            return true;
        }

        return false;
    }


    //метод проверка не проиграл ли игрок
    bool IsLoseGame()
    {
        //если игрок существует и он мертв
        if (player != null && player.IsDead)
        {
            return true;
        }

        return false;
    }


    //метод создания игрока
    IEnumerator MakePlayer()
    {
        GameObject plr = new GameObject("Player");

        plr.transform.position = freeCallPosition[0];//позиция игрока

        plr.transform.parent = allSpritesLevel.transform; //делаем монетку дочерней к объекту allSpritesLevel

        player = plr.AddComponent<Player>();

        player.SoundClipMove = sfxPlayerMoveSound; //задаем звук шагов игроку
        
        yield return new WaitForEndOfFrame(); //ждем кадр, что б добавился аниматор

        if (player != null)
            player.Init(0, animatorPlayer);
    }


    //метод создания противников
    IEnumerator MakeEnemys(int countEnemys)
    {
        for (int i = 0; i < countEnemys; i++)
        {
            GameObject e = new GameObject("Enemy");

            int numPos = Random.Range(4, freeCallPosition.Count - (i + 1)); //выбираем случайную позицию от 4 и до максимальной - i

            e.transform.position = freeCallPosition[numPos]; //позицию для противника

            e.transform.parent = allSpritesLevel.transform; //делаем монетку дочерней к объекту allSpritesLevel  

            e.transform.localScale = new Vector2(1f, 0.60f);

            Enemy enemy = e.AddComponent<Enemy>();

            BoxCollider2D col2D = enemy.GetComponent<BoxCollider2D>(); //находим у противника компонент Collider2D

            if (col2D != null)
            {
                col2D.isTrigger = true; //делаем колайдер триггером                
            }

            yield return new WaitForEndOfFrame(); //ждем кадр, что б добавился аниматор            

            enemy.Init(0, animatorEnemy);

            enemysList.Add(enemy); //добавляем врага в список
        }
    }


    //метод создания монеток
    void CoinCreate()
    {
        bool isCreate = false;

        foreach (var pos in freeCallPosition)
        {
            //первую позицию пропускаем, там создается игрок
            if (!isCreate)
            {
                isCreate = true;
                continue; 
            } 

            GameObject go = Utils.CreateAnimationSprite(pos, animatorCoin, "Coin", true);

            go.AddComponent<Coin>();

            go.AddComponent<Rigidbody2D>();

            BoxCollider2D col2D = go.GetComponent<BoxCollider2D>();

            col2D.isTrigger = true;

            //отрисовываем монетки выше спрайтов уровня
            SpriteRenderer sprRen = go.GetComponent<SpriteRenderer>();
            sprRen.sortingOrder = 3;

            go.transform.parent = coinsLevel.transform;            

            coinsCount++; // увеличиваем счетчик монеток на 1
        }

    }


    //метод поиска позиции для создания бонуса
    Vector2 BonusFindPosition()
    {
        Vector2 pos = Vector2.zero;
        
        int i = Random.Range(0, freeCallPosition.Count - 1); //случайно выбираем индек пусой ячейки
        pos = freeCallPosition[i]; //берем позицию по индексу

        while (Vector2.Distance(player.transform.position, pos) < 1f || Vector2.Distance(player.transform.position, pos) > 4f)
        {
            int n = Random.Range(0, freeCallPosition.Count - 1); //случайно выбираем индек пусой ячейки
            pos = freeCallPosition[n]; //берем позицию по индексу
        }

        return pos;
    }


    //метод создания бонуса
    void BonusCreate(BonusType type)
    {
        Vector2 position = BonusFindPosition(); //ищем позицию

        int sprIndex = Bonus.BonusIndexByType(type); //находим индекс бонуса

        string nameBonus = "bonus_" + type.ToString() + "_" + sprIndex;

        position.x = position.x - 0.64f; //немного сдвинем бонус по (х) на 0.64

        position.y = position.y + 0.64f; //немного сдвинем бонус по (y) на 0.64

        GameObject go = Utils.CreateSprite(position, sprBonus[sprIndex], nameBonus, true);

        go.AddComponent<Rigidbody2D>();

        BoxCollider2D col2D = go.GetComponent<BoxCollider2D>();
        col2D.isTrigger = true;

        //отрисовываем бонус на выше уровня и монеток
        SpriteRenderer sprRen = go.GetComponent<SpriteRenderer>();
        sprRen.sortingOrder = 4;

        go.AddComponent<Bonus>();

        bonus = go.GetComponent<Bonus>();

        bonus.TypeBonus = type;        

        bonusExist = true; //отмечаем что бонус есть на уровне

        timerDestroyBonus = 4f;
    }


    //метод создает случайный бонус в игре, если нет ни одного
    void BonusGeneration()
    {
        if (!bonusExist) //если нет бонуса на уровне, нужно его создать
        {
            if (timerCreateBonus > 0f)
            {
                timerCreateBonus -= Time.deltaTime;
            }
            else if (timerDiactivateBonus == 0f) //если время действия бонуса закончилось, только тогда создаем
            {
                timerCreateBonus = 0f; //обнуляем таймер создания бонусов

                int randomIndex = Random.Range(0, 9); //получаем случайной номер от 0 до 8 включительно 

                BonusType bt = Bonus.BonusTypeByIndex(randomIndex); // находим нужний тип бонуса                

                BonusCreate(bt); //создаем бонус нужного типа

            }
        }  
        else
        {
            //если бонус есть на уровне и таймер до его удаления равен нулю
            //то удаляем бонус, выставляем все счетчики в ноль, и отмечаем что бонуса на уровне нет
            if (timerDestroyBonus > 0f)
            {
                timerDestroyBonus -= Time.deltaTime;
            }
            else
            { 
                //обнуляем таймеры
                timerDestroyBonus = 0f; 
                timerCreateBonus = 0f; 
                timerDiactivateBonus = 0; 

                bonus.SelfDestroy();//удаляем текущий бонус    

                bonusExist = false; //бонуса нет на уровне

            }
        }
    }


    //активация бонуса
    public void ActivateBonus(BonusType type)
    {
        SoundBonus();

        bonusExist = false; //отмечаем, что бонусов нет на уровне
        timerCreateBonus = Random.Range(5f, 10f); //запустить таймер в случайном диапазоне для создания бонуса

        switch (type)
        {
            case BonusType.plusSpeed:

                isBonusActive = true;
                activeBonus = type;
                timerDiactivateBonus = 10f;
                player.PlusSpeed();

                break;
            case BonusType.cherry:

                player.CherryCount++;
                Debug.Log("ВИШНЯ + 1 кол: " + player.CherryCount);

                break;
            case BonusType.candy:

                player.CandyCount++;
                Debug.Log("КОНФЕТА + 1 кол: " + player.CandyCount);

                break;
            case BonusType.minusEnemySpeed:

                isBonusActive = true;
                activeBonus = type;
                timerDiactivateBonus = 5f;

                foreach (var en in enemysList)
                    en.MinusSpeed();

                break;
            case BonusType.shield:

                isBonusActive = true;
                activeBonus = type;
                timerDiactivateBonus = 5f;
                player.IsActiveShield = true;

                break;
            case BonusType.lives:

                player.AddLive();

                break;
            case BonusType.plusEnemySpeed:

                isBonusActive = true;
                activeBonus = type;
                timerDiactivateBonus = 10f;

                foreach (var en in enemysList)
                    en.PlusSpeed();

                break;
            case BonusType.minusSpeed:

                isBonusActive = true;
                activeBonus = type;
                timerDiactivateBonus = 5f;
                player.MinusSpeed();

                break;
            case BonusType.minusLive:

                player.RemoveLive();

                break;
        }
    }


    //метод деактивации бонуса
    void BonusDisable()
    {
        if (timerDiactivateBonus > 0f)
        {
            timerDiactivateBonus -= Time.deltaTime;
        }
        else
        {
            isBonusActive = false; //деактивируем бонус     
            timerDiactivateBonus = 0f; //обнуляем таймер действия бонуса

            switch (activeBonus)
            {
                case BonusType.plusSpeed: //выключить ускорение

                    player.SpeedDefault();

                    break;
                case BonusType.minusSpeed: // вернуть скорость по умолчанию

                    player.SpeedDefault();

                    break;
                case BonusType.minusEnemySpeed: // вернуть скорость  ВРАГОВ по умолчанию

                    foreach (var en in enemysList)
                        en.SpeedDefault();

                    break;
                case BonusType.plusEnemySpeed: // выключить ускорение  ВРАГОВ 

                    foreach (var en in enemysList)
                        en.SpeedDefault();

                    break;
                case BonusType.shield: // выключить щит у игрока

                    player.IsActiveShield = false;

                    break;
            }
        }
    }


    //метод выхода из игры
    void QuitGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //SceneMansger.CancelQuit(); //выходим из игры       

            
        }
    }


    //методы проигрывания звуков
    public void SoundCoin()
    {
        Utils.PlaySound(audioSoursSFX, sfxCoinsSound);
    }

    public void SoundBonus()
    {
        Utils.PlaySound(audioSoursSFX, sfxBonusesSound);
    }

    public void SoundDamage()
    {
        Utils.PlaySound(audioSoursSFX, sfxDamageSound);
    }

    public void SoundMove()
    {
        Utils.PlaySound(audioSoursSFX, sfxPlayerMoveSound);
    }
}


