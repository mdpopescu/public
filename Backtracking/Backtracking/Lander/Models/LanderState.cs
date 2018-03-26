using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Backtracking.Library.Contracts;

namespace Backtracking.Lander.Models
{
    public class LanderState : IState
    {
        public Position Position { get; }
        public Thrust Thrust { get; }
        public Speed Speed { get; }

        public LanderState(Position position, Thrust thrust, Speed speed)
        {
            Position = position;
            Thrust = thrust;
            Speed = speed;
        }

        public override string ToString() =>
            $"Tuple.Create({Thrust.Angle}, {Thrust.Power}), // X={Position.X}, Y={Position.Y}, dH={Speed.HSpeed:F2}, dV={Speed.VSpeed:F2}";

        public bool IsSolution(List<IState> history)
        {
            var current = (LanderState) history.Last();
            return Math.Abs(current.Speed.VSpeed) <= 40 &&
                Math.Abs(current.Speed.HSpeed) <= 20 &&
                current.Position.X >= 4000 &&
                current.Position.X <= 5500 &&
                current.Position.Y == 150 &&
                current.Thrust.Angle == 0;
        }

        public bool IsInvalid(List<IState> history)
        {
            var current = (LanderState) history.Last();

            // too many steps means out of fuel - failure
            if (history.Count > 500)
                return true;

            // going below the landing level is a failure
            if (current.Position.Y < 150)
                return true;

            // if the speed is too high, failure
            var vSpeed = current.Speed.VSpeed;
            var sum = vSpeed * (vSpeed + 1) / 2;
            var remaining = current.Position.Y - 150;
            if (sum > remaining)
                return true;

            //// going above the starting level is a failure
            //if (current.Position.Y > 2700)
            //    return true;

            //// revisiting a location is a failure
            //if (history.Cast<LanderState>().Take(history.Count - 1).Any(it => it.Position.X == current.Position.X && it.Position.Y == current.Position.Y))
            //    return true;

            // if the landing level hasn't been reached yet, it's not a failure
            if (current.Position.Y > 150)
                return false;

            // we've landed; if X is not within the landing bounds, it's a failure
            if (current.Position.X < 4000 || current.Position.X > 5500)
                return true;

            // it's a failure if the constraints are not respected
            return Math.Abs(current.Speed.VSpeed) > 40 || Math.Abs(current.Speed.HSpeed) > 20 || current.Position.X < 4000 || current.Position.X > 5500;
        }

        public IEnumerable<IState> GenerateCandidates()
        {
            var candidates = from tuple in deltas.Value
                             let thrust = tuple.Item1
                             let speed = tuple.Item2
                             let newSpeed = Speed.Add(speed)
                             let newPos = new Position((int) Math.Round(Position.X + newSpeed.HSpeed), (int) Math.Round(Position.Y + newSpeed.VSpeed))
                             select new LanderState(newPos, thrust, newSpeed);
            return candidates.OrderBy(GetScore);
        }

        //

        private const double G = 3.711;

        private static readonly Lazy<List<Tuple<Thrust, Speed>>> deltas = new Lazy<List<Tuple<Thrust, Speed>>>(
            () =>
            {
                // ensure there are no mistakes
                var rtVector = GetSpeed(-90);
                var upVector = GetSpeed(0);
                var ltVector = GetSpeed(90);

                Debug.Assert((int) rtVector.HSpeed == 1 && (int) rtVector.VSpeed == 0);
                Debug.Assert((int) upVector.HSpeed == 0 && (int) upVector.VSpeed == 1);
                Debug.Assert((int) ltVector.HSpeed == -1 && (int) ltVector.VSpeed == 0);

                //var angles = new[] { 0, -15, 15, -30, 30, -45, 45, -60, 60, -75, 75, -90, 90 };
                var angles = new[] { 0, -30, 30, -60, 60, -90, 90 };
                //var angles = Enumerable.Range(-18, 37).Select(i => i * 5).ToArray();
                var powers = new[] { 0, 1, 2, 3, 4 };
                var thrusts = from angle in angles
                              from power in powers
                              select new Thrust(angle, power);
                var result = from thrust in thrusts
                             let speed = GetSpeed(thrust.Angle).Multiply(thrust.Power)
                             let speedInGravity = new Speed(speed.HSpeed, speed.VSpeed - G)
                             select Tuple.Create(thrust, speedInGravity);
                return result.ToList();
            });

        private static double GrdToRad(int angle) => (angle + 90) * Math.PI / 180;

        private static Speed GetSpeed(int angle)
        {
            var rad = GrdToRad(angle);
            return new Speed(Math.Cos(rad), Math.Sin(rad));
        }

        private static int GetScore(LanderState state)
        {
            // prefer X between 4000 and 5500, Y as close as possible to 150, HSpeed <= 40, VSpeed <= 20

            var lDist = state.Position.X < 4000 ? 4000 - state.Position.X : 0;
            var rDist = state.Position.X > 5500 ? state.Position.X - 5500 : 0;
            var vDist = state.Position.Y - 150;
            var hsDist = state.Speed.HSpeed > 40 ? state.Speed.HSpeed - 40 : 0;
            var vsDist = state.Speed.VSpeed > 20 ? state.Speed.VSpeed - 20 : 0;

            return (lDist + rDist) * 5 + vDist + (int) Math.Round((hsDist + vsDist) * 2);
        }
    }
}