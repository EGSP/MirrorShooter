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
            
            if(horizontalDelta != 0 || verticalDelta != 0)
                MoveBody(horizontalDelta, verticalDelta);
            
            // Здесь нужно формировать лист из нажаты = отжатых кнопок и отправлять его.

            var newDown = new List<int>();
            var newUp = new List<int>();
            
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                newUp.Add((int)KeyCode.LeftShift);
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                newDown.Add((int)KeyCode.LeftShift);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                newUp.Add((int) KeyCode.Space);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                newDown.Add((int) KeyCode.Space);
            }

            AddNewUp(newUp);
            AddNewDown(newDown);
        }
    }
}