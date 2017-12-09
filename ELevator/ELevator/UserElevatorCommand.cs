namespace Elevator
{
    /// <summary>
    /// Represents cuser command when user presses a button with floor number in elevator or press button on floor
    /// </summary>
    public class UserElevatorCommand
    {
        public UserElevatorCommand(int floorNumber, CommandSources commandSource)
        {
            FloorNumber = floorNumber;
            CommandSource = commandSource;
        }

        /// <summary>
        /// Command source
        /// </summary>
        public CommandSources CommandSource { get; private set; }

        /// <summary>
        /// Number of requested floor
        /// </summary>
        public int FloorNumber { get; private set; }
    }
}
