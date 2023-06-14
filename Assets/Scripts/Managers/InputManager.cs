using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InputManager : Singleton<InputManager>
{
    public enum InputType { Keyboard, Mobile, GamePad } // Giriş türü enum tanımı

    public InputType inputMode = InputType.Keyboard; // Kullanılan giriş türü

    [Header("Inputs")]
    public Keyboard keyboard; // Klavye girişleri

    [Header("Joystick")]
    public Joystick moveJoystick; // Joystick bileşeni



    public Vector2 Move; // Hareket vektörü
    public UnityEvent OnClickBomb = new UnityEvent();
    public UnityEvent OnLeavekBomb = new UnityEvent();

 
    private void Update()
    {
        switch (inputMode)
        {
            case InputType.Keyboard:
                HandleKeyboard(); // Klavye girişini işle
                break;
            case InputType.Mobile:
                HandleMobile(); // Mobil girişi işle
                break;
            case InputType.GamePad:
                break;
            default:
                break;
        }


    }
    // Mobil joystick hareketini işle
    private void HandleMobile()
    {

        
        Vector2 joystickDirection = moveJoystick.Direction;

        float x = joystickDirection.x;
        float y = joystickDirection.y;
        Move = joystickDirection;
        // X ve Y değerlerini bağımsız olarak kontrol et
        //if (Mathf.Abs(x) > Mathf.Abs(y))
        //{
        //    Move = new Vector2(x, 0f).normalized; // Sadece x yönünde hareket et
        //}
        //else  
        //{
        //    Move = new Vector2(0f, y).normalized; // Sadece y yönünde hareket et
        //}
    }

    private void HandleKeyboard()
    {
        if (Input.GetKey(keyboard.inputUp))
        {
            Move = Vector2.up; // Yukarı yönde hareket et
        }
        else if (Input.GetKey(keyboard.inputDown))
        {
            Move = Vector2.down; // Aşağı yönde hareket et
        }
        else if (Input.GetKey(keyboard.inputLeft))
        {
            Move = Vector2.left; // Sola yönde hareket et
        }
        else if (Input.GetKey(keyboard.inputRight))
        {
            Move = Vector2.right; // Sağa yönde hareket et
        }
        else
        {
            Move = Vector2.zero; // Hareketsiz dur
        }
        if (Input.GetKeyDown(keyboard.bombPlaceKey))
        {
            PlaceBombButton();
        }

    }

   
    public void MoveUp()
    {
        Move = Vector2.up;
    }

    // Patlama düğmesine tıklama olayını işle
    public void ExplosionButtonCanvas()
    {
        if (OnClickBomb != null) OnClickBomb.Invoke();
    }

    public void PlaceBombButton()
    {
        if (OnLeavekBomb != null) OnLeavekBomb.Invoke();
    }


    public void ShowEmoji()
    {
        MovementController.Instance.EmojiBox1();
    }
    public void ShowEmoji2()
    {
        MovementController.Instance.EmojiBox2();
    }
    public void ShowEmoji3()
    {
        MovementController.Instance.EmojiBoxWar();
    }
    public void ShowEmoji4()
    {
        MovementController.Instance.EmojiBox4();
    }
}

[Serializable]
public class Keyboard
{
    public KeyCode inputUp = KeyCode.W; // Yukarı hareket için atanmış klavye tuşu
    public KeyCode inputDown = KeyCode.S; // Aşağı hareket için atanmış klavye tuşu
    public KeyCode inputLeft = KeyCode.A; // Sola hareket için atanmış klavye tuşu
    public KeyCode inputRight = KeyCode.D; // Sağa hareket için atanmış klavye tuşu
    [Header("Bomb")]
    public KeyCode bombPlaceKey = KeyCode.LeftShift; // Bomba yerleştirme tuşu
}
