using System;
using System.Threading.Tasks;
using UnityEngine;
using VCHStateMachine;

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
            Func<VCHControlState> getCurrentRelevantState, 
            Func<bool> condition, 
            Action onCorrect, 
            Action onWrong,
            int checkFrequency = 1000,
            int timeout = -1)
        {
            var startTime = Time.time;
            var initialState = getCurrentRelevantState();
            var previousState = initialState;
            
            while (true)
            {

                await Task.Delay(checkFrequency);
                
                var currentState = getCurrentRelevantState();
                
                var conditionMet = condition();

                if (currentState != initialState)
                {
                    // Detect state change
                    Debug.Log("CHANGED STATE");
                    if (conditionMet) // Condition met
                    {
                        onCorrect?.Invoke();
                        return; // Exit function
                    }
                    else if (currentState != previousState)// Condition not met currentState != previousState
                    {
                        Debug.Log("CHANGED STATE BUT NOT CORRECT");
                        onWrong?.Invoke();
                    }
                } 
                // If currentState == initialState, no action performed yet - no feedback

                previousState = currentState;
            }
        }
        
    }
}
