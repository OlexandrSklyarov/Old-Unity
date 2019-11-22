using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{

    //метод для создания спрайтов
    public static GameObject CreateSprite(Vector2 position, Sprite sprite, string name, bool isAddCollider)
    {
        GameObject go = new GameObject(name); //создаем объект с именем name

        go.transform.position = position; // устанавливаем позицию

        SpriteRenderer sprRen = go.AddComponent<SpriteRenderer>(); //добавляем компонент SpriteRenderer

        sprRen.sortingOrder = 0; //устанавливаем глубину отрисовки в 0

        sprRen.sprite = sprite; // назначаем нужный спрайт

        //если нужно, то добавляем объекту колайдер 2D
        if (isAddCollider)
        {
            BoxCollider2D boxCol = go.AddComponent<BoxCollider2D>();
            boxCol.size = new Vector2(1.28f, 1.28f);
        }
            

        return go;
    }

    //метод для создания аниматора
    public static GameObject CreateAnimationSprite(Vector2 position, RuntimeAnimatorController anim, string name, bool isAddCollider)
    {
        GameObject go = new GameObject(name); //создаем объект с именем name

        go.transform.position = position; // устанавливаем позицию

        SpriteRenderer sprRen = go.AddComponent<SpriteRenderer>(); //добавляем компонент SpriteRenderer

        sprRen.sortingOrder = 0; //устанавливаем глубину отрисовки в 0

        Animator animator = go.AddComponent<Animator>();// добавляем компонент Animator

        animator.runtimeAnimatorController = anim; // присваиваем аниматор контроллер

        //если нужно, то добавляем объекту колайдер 2D
        if (isAddCollider)
        {
            BoxCollider2D boxCol = go.AddComponent<BoxCollider2D>();
            boxCol.size = new Vector2(1.28f, 1.28f);            
        }

        return go;
    }


    //метод проигрывания звуков
    public static void PlaySound(AudioSource audioSours, AudioClip clip)
    {
        audioSours.Stop();
        audioSours.clip = clip;
        audioSours.loop = false;
        audioSours.time = 0f;
        audioSours.Play();
    }
       

}
