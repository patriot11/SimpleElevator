using System.Threading;
using NUnit.Framework;

namespace Elevator.Test
{
    [TestFixture]
    public class Tests
    {
        [TestCase]
        public void Test1()
        {
            ElevatorController ec = new ElevatorController(10, 4, 8, 5);
            Assert.AreEqual(1, ec.CurrentFloor, "Starting floor should be equal to 1");
            ec.CommandInitiated(new UserElevatorCommand(3, CommandSources.Elevator));
            ec.CommandInitiated(new UserElevatorCommand(10, CommandSources.Floor));
            Thread.Sleep(1125);
            ec.CommandInitiated(new UserElevatorCommand(1, CommandSources.Floor));
            Assert.AreEqual(3, ec.CurrentFloor, "Starting floor should be equal to 3");
            Thread.Sleep(3375);
            Assert.AreEqual(10, ec.CurrentFloor, "Starting floor should be equal to 10");
            Thread.Sleep(6250);
            Assert.AreEqual(1, ec.CurrentFloor, "Starting floor should be equal to 1");
        }
    }
}
