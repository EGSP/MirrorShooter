using UnityEngine;

namespace Gasanov.SpeedUtils.Time
{
    public class HourTimer
    {
        public HourTimer(int startHour, int startMinute, int endHour, int endMinute)
        {
            StartHour = startHour;
            StartMinute = startMinute;

            EndHour = endHour;
            EndMinute = endMinute;
            
            var targetHours = 0;
            TargetMinutes = 0;
            
            if (startHour > endHour)
            {
                targetHours = (endHour + 24) - startHour;
            }// Если меньше стартовый час
            else if(startHour < endHour)
            {
                targetHours = endHour - startHour;
                
            }// Если равны, то смотрим на минуты
            else
            {
                if (startMinute > endMinute)
                {
                    targetHours = (endHour + 24) - startHour;
                }
                else if (startMinute < endMinute)
                {
                    targetHours = endHour - startHour;
                }
                else
                {
                    throw new System.Exception("Время начала и конца отсчета таймера совпадает");
                }
            }

            // Превращаем часы в минуты
            TargetMinutes += targetHours * 59;

            TargetMinutes -= startMinute;
            TargetMinutes += endMinute;
        }
        
        /// <summary>
        /// Час с которого начинается отсчет
        /// </summary>
        public int StartHour { get; private set; }
        
        /// <summary>
        /// Минута стартового часа
        /// </summary>
        public int StartMinute { get; private set; }

        /// <summary>
        /// Час которым заканчивается отсчет
        /// </summary>
        public int EndHour { get; private set; }
        
        /// <summary>
        /// Минута последнего часа
        /// </summary>
        public int EndMinute { get; private set; }
        
        /// <summary>
        /// Сколько должно пройти минут
        /// </summary>
        public float TargetMinutes { get; private set; }

        /// <summary>
        /// Пройденное время
        /// </summary>
        public float ElapsedMinutes { get; private set; }
        
        /// <summary>
        /// Какая часть времени прошла (0 - 1)
        /// </summary>
        public float Opacity => ElapsedMinutes / TargetMinutes;

        public void UpdateTimer(float deltaTime, float timeScale = 1f)
        {
            ElapsedMinutes += deltaTime*timeScale;

            if (ElapsedMinutes > TargetMinutes)
            {
                ElapsedMinutes = TargetMinutes;
            }
        }

        /// <summary>
        /// Получение текущего часа и минут (не учитывая стартовое время)
        /// </summary>
        public void GetTime(out float hour, out float minutes)
        {
            hour = 0f;
            minutes = 0f;
            if (ElapsedMinutes >= TargetMinutes)
            {
                hour = Mathf.Floor(TargetMinutes / 59);
                if (hour > 24)
                    hour -= 24;

                minutes = TargetMinutes % 59;
            }
            else
            {
                hour = Mathf.Floor(ElapsedMinutes / 59);
                if (hour > 24)
                    hour -= 24;

                minutes = ElapsedMinutes % 59;
            }
        }

        public void GetWorldTime(out float hour, out float minutes)
        {
            GetTime(out hour,out minutes);

            hour += StartHour;
            if (hour > 24)
                hour -= 24;
            
            minutes += StartMinute;
            if (minutes > 59)
            {
                hour += 1;
                minutes -=59;
            }
            // else if (minutes > 60)
            // {
            //     hour += 1;
            //     minutes -= 60;
            // }
            
            if (minutes < 0)
                minutes = 0;

            
        }
        
        
    }
}