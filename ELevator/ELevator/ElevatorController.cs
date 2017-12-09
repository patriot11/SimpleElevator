using System;
using System.Linq;
using System.Threading;

namespace Elevator
{
    /// <summary>
    /// Controls elevator's behaviour
    /// </summary>
    public class ElevatorController
    {
        #region Properties

        /// <summary>
        /// Floors number
        /// </summary>
        public int FloorsNumber { get; private set; }

        /// <summary>
        /// Floor height. Height between bottoms of two floors. In meters
        /// </summary>
        public decimal FloorHeight { get; private set; }

        /// <summary>
        /// Speed in meters per hour
        /// </summary>
        public decimal Speed { get; private set; }

        /// <summary>
        /// Time between doors open and close. In milliseconds
        /// </summary>
        public int DoorsOpenTime { get; private set; }

        /// <summary>
        /// Indicates if elevator is moving or not
        /// </summary>
        public bool IsElevatorMoving { get; private set; }

        /// <summary>
        /// Current floor. Starting floor is the first one
        /// </summary>
        public int CurrentFloor { get; private set; } = 1;

        #endregion

        #region Variables

        private readonly object _syncCommand = new object();
        private readonly object _syncSetFloor = new object();

        private Thread _thrd;

        #endregion

        #region ctors

        public ElevatorController(int floorsNumber, decimal floorHeight, decimal speed, int doorsOpenTime)
        {
            FloorsNumber = floorsNumber;
            FloorHeight = floorHeight;
            Speed = speed;
            DoorsOpenTime = doorsOpenTime;
        }

        #endregion

        #region Methods

        #region - public -

        public void CommandInitiated(UserElevatorCommand command)
        {
            lock (_syncCommand) {
                if (command.FloorNumber == CurrentFloor) {
                    Console.WriteLine($"You are already on floor {command.FloorNumber}");
                    return;
                }

                if (command.FloorNumber > FloorsNumber || command.FloorNumber < 1) {
                    Console.WriteLine(command.CommandSource == CommandSources.Elevator
                        ? $"Please choose floor from 1 to {FloorsNumber}"
                        : $"It looks like you pressed a button outside of a building which is not supposed to be. It's only {FloorsNumber} floors in the building");

                    return;
                }

                if (!IsElevatorMoving) {
                    IsElevatorMoving = true;
                    _thrd = new Thread(new ParameterizedThreadStart(ElevatorMovementAction));
                    _thrd.Start(new Tuple<ElevatorController, Action<int>, Action>(this, SetCurrentFloor, ElevatorStopped));
                }

                do {
                    Thread.Sleep(100);
                } while (_thrd.ThreadState != ThreadState.Running && _thrd.ThreadState != ThreadState.WaitSleepJoin);

                RaiseUserCommandReceivedEvent(command.FloorNumber);
            }
        }

        /// <summary>
        /// Get time in miliseconds which needs to elevator to move between floors
        /// </summary>
        /// <returns>Time delay in miliseconds</returns>
        public int GetOneFloorMoveTime()
        {
            int t = (int)Math.Round(FloorHeight / Speed * 1000);
            return t;
        }

        /// <summary>
        /// Get time in miliseconds which needs to elevator to move between floors
        /// </summary>
        /// <returns>Time delay in miliseconds</returns>
        public ElevatorMovementDirections GetMovementDirection(int floor)
        {
            ElevatorMovementDirections movementDirection = CurrentFloor < floor ? ElevatorMovementDirections.Up : ElevatorMovementDirections.Down;
            Console.WriteLine($"Moving {movementDirection}");
            return movementDirection;
        }

        #endregion

        #region - events -

        public EventHandler<int> UserCommandReceived;

        private void RaiseUserCommandReceivedEvent(int floor)
        {
            var evt = UserCommandReceived;
            evt?.Invoke(this, floor);
        }

        #endregion

        #region - private -

        private void SetCurrentFloor(int floor)
        {
            lock (_syncSetFloor) {
                CurrentFloor = floor;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Current floor is: {CurrentFloor}");
            }
        }

        private void ElevatorStopped()
        {
            IsElevatorMoving = false;
        }

        private static void ElevatorMovementAction(object o)
        {
            var tuple = o as Tuple<ElevatorController, Action<int>, Action>;
            if (tuple == null)
                return;

            ElevatorController controller = tuple.Item1;
            var setFloorAction = tuple.Item2;
            var stopElevatorFlag = tuple.Item3;
            ElevatorMovementDirections movementDirection = ElevatorMovementDirections.None;

            bool[] floorStops = new bool[controller.FloorsNumber];

            controller.UserCommandReceived += (sender, i) =>
            {
                floorStops[i - 1] = true;
            };

            do {
                //waiting for setting of any target floor
                Thread.Sleep(100);
            } while (floorStops.All(n => !n));

            for (int i = 0; i < floorStops.Length; i++) {
                if (floorStops[i]) {
                    movementDirection = controller.GetMovementDirection(i);
                }
            }

            int currentFloor = controller.CurrentFloor;
            do {
                Thread.Sleep(controller.GetOneFloorMoveTime());

                //calculate current floor
                currentFloor += (movementDirection == ElevatorMovementDirections.Up ? 1 : -1);

                //set current floor to controller
                setFloorAction.Invoke(currentFloor);

                //open door if needed
                if (floorStops[currentFloor - 1]) {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Open door");
                    Thread.Sleep(controller.DoorsOpenTime);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Close door");
                    floorStops[currentFloor - 1] = false;
                }

                //break if no any floors needed to reach
                if (floorStops.All(n => !n))
                    break;

                //we may need to change direction 
                if (currentFloor == 0 || currentFloor == controller.FloorsNumber
                    || (movementDirection == ElevatorMovementDirections.Up && floorStops.Skip(currentFloor).All(n => !n))
                    || (movementDirection == ElevatorMovementDirections.Down && floorStops.Take(currentFloor - 1).All(n => !n))) {
                    for (int i = 0; i < floorStops.Length; i++) {
                        if (floorStops[i]) {
                            movementDirection = controller.GetMovementDirection(i + 1);
                        }
                    }
                }

            } while (floorStops.Any(n => n));

            stopElevatorFlag.Invoke();
        }

        #endregion

        #endregion
    }
}
