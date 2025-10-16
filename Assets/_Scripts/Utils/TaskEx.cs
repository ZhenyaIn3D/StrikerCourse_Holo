using System;
using System.Threading.Tasks;
using UnityEngine;

namespace _Scripts.Utils
{
    public static class TaskEx
    {
        /// <summary>
        /// Blocks while condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The condition that will perpetuate the block.</param>
        /// <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task WaitWhile(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) await Task.Delay(frequency);
            });

            if(waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }

        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The break condition.</param>
        /// <param name="frequency">The frequency at which the condition will be checked.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask, 
                    Task.Delay(timeout))) 
                throw new TimeoutException();
        }
        
        
        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The break condition.</param>
        /// <param name="frequency">The frequency at which the condition will be checked.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        public static async Task WaitForConditionWithFeedback(
            Func<bool> condition, 
            Action onCorrect, 
            Action onWrong,
            int checkFrequency = 100,
            int timeout = -1)
        {
            var startTime = Time.time;
            var previousState = condition();
    
            while (true)
            {
                // Check for timeout
                // if (timeout > 0 && (Time.time - startTime) * 1000 >= timeout)
                // {
                //     throw new TimeoutException();
                // }
        
                await Task.Delay(checkFrequency);
        
                var currentState = condition();
        
                // Detect state change
                if (currentState != previousState)
                {
                    if (currentState) // Condition met
                    {
                        onCorrect?.Invoke();
                        return; // Exit function
                    }
                    else // Condition not met
                    {
                        onWrong?.Invoke();
                        previousState = currentState; // Continue waiting
                    }
                }
            }
        }
        
    }
}


// public async Task WaitForStateChange<T>(
//     Func<T> getState,
//     Func<T, bool> isCorrect,
//     Action onCorrect,
//     Action onWrong,
//     int checkFrequency = 100)
// {
//     var previousState = getState();
//     
//     while (true)
//     {
//         await Task.Delay(checkFrequency);
//         
//         var currentState = getState();
//         
//         if (!EqualityComparer<T>.Default.Equals(currentState, previousState))
//         {
//             if (isCorrect(currentState))
//             {
//                 onCorrect?.Invoke();
//                 return;
//             }
//             else
//             {
//                 onWrong?.Invoke();
//                 previousState = currentState;
//             }
//         }
//     }
// }
//


//await WaitForStateChange(
// getState: () => currentScanState,
// isCorrect: (state) => state == ScanState.Success,
// onCorrect: () => ShowCorrectIndicator(),
// onWrong: () => ShowWrongIndicator()
//     );
