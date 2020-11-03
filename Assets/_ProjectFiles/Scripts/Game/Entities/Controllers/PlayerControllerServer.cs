using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Controllers
{
    public class PlayerControllerServer : PlayerController
    {
        public override void UpdateOnClient()
        {
            // Ничего не делаем, т.к. этот контроллер только на стороне сервера.
        }

        public override void UpdateOnServer()
        {
            if (PlayerEntity == null)
                return;
            
            // Mouse rotation Input
            float rotationY = Input.GetAxis("Mouse X") * mouseSensivity.x; // Вращение по горизонтали
            float rotationX = -Input.GetAxis("Mouse Y") * mouseSensivity.y; // Вращение по вертикали

            float horizontalDelta = Input.GetAxisRaw("Horizontal");
            float verticalDelta = Input.GetAxisRaw("Vertical");
            
            if(rotationX != 0)
                RotateCamera(rotationX);
            
            if(rotationY != 0)
                RotateBody(rotationY);
            
            // Здесь нужно формировать лист из нажаты - отжатых кнопок и отправлять его.

            DefineKeyCodeState(KeyCode.W);
            DefineKeyCodeState(KeyCode.S);
            DefineKeyCodeState(KeyCode.A);
            DefineKeyCodeState(KeyCode.D);
            
            DefineKeyCodeState(KeyCode.LeftShift);
            DefineKeyCodeState(KeyCode.Space);
            
            AddNewDown(down);
            AddNewUp(up);
            
            down.Clear();
            up.Clear();
        }
    }
}